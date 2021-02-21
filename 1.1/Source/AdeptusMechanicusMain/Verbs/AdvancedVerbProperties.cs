using AdeptusMechanicus.ExtensionMethods;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.AdvancedVerbProperties
    public partial class AdvancedVerbProperties : VerbProperties, IMuzzlePosition
    {
        public float heavyWeaponSetupTime = -1f;

        public Reliability reliability = Reliability.NA;

        public bool rapidFire = false;
        public float rapidFireRange = 0.55f;

        public bool bodySizeBurst = false;
        public float bodySizeBurstModifier = 1f;

        public float hotDamage = 0f;
        public float getsHotCritChance = 0f;
        public float getsHotCritExplosionChance = 0f;

        public bool getsHot = false;
         
        public float jamDamage = 0f;
        public float extraCooldown = 0f;

        public float effectsUserChance = 0f;
        public StatDef resistEffectStat = null;
        public HediffDef userEffect = null;

        public bool rending = false;
        public float rendingChance = 0.167f;

        public int scattershotCount = 0;
        public ResearchProjectDef requiredResearch = null;
        public List<string> userEffectImmuneList = new List<string>();
        public float barrelLength = 1f;
        public float barrelOffset = 0f;
        public float bulletOffset = 0.2f;
        public float laserOffset = -0.4f;

        public string muzzleFlareDef = "Mote_SparkFlash";
        public float muzzleFlareSize = -1f;
        private ThingDef _muzzleFlareDef;
        public FloatRange? muzzleFlareSizeRange;

        public string muzzleSmokeDef = "OG_Mote_SmokeTrail";
        public float muzzleSmokeSize = 0.35f;
        private ThingDef _muzzleSmokeDef;
        public FloatRange? muzzleSmokeSizeRange;

        public bool gizmosOnEquip = true;
         
        public int ScattershotCount => defaultProjectile?.GetModExtensionFast<ScattershotProjectileExtension>()?.projectileCount ?? scattershotCount;
        public bool Jams => reliability != Reliability.NA;
        public bool JamsDamageWeapon => jamDamage > 0;
        public bool HotDamageWeapon => hotDamage > 0f;
        public bool GetsHotCrit => getsHotCritChance > 0f;
        public bool GetsHotCritExplosion => getsHotCritExplosionChance > 0f;
        public float BarrelOffset => barrelOffset;
        public float BarrelLength => barrelLength;
        public float BulletOffset => defaultProjectile as Lasers.LaserBeamDef == null ? bulletOffset : bulletOffset + laserOffset;
        public float MuzzleSmokeSize => muzzleSmokeSizeRange?.RandomInRange ?? muzzleSmokeSize;
        public float MuzzleFlareSize => muzzleFlareSizeRange?.RandomInRange ?? (muzzleFlareSize > 0 ? muzzleFlareSize : muzzleFlashScale * 0.25f);
        public ThingDef MuzzleFlareDef => _muzzleFlareDef ??= !muzzleFlareDef.NullOrEmpty() ? DefDatabase<ThingDef>.GetNamed(muzzleFlareDef) : null;
        public ThingDef MuzzleSmokeDef => _muzzleSmokeDef ??= !muzzleSmokeDef.NullOrEmpty() ? DefDatabase<ThingDef>.GetNamed(muzzleSmokeDef) : null;

        public bool EffectsUser => effectsUserChance > 0 && userEffect != null;
        public bool TwinLinked => ScattershotCount == 1;
        public bool Multishot => ScattershotCount > 1;
        public bool HeavyWeapon => heavyWeaponSetupTime > 0;

        public int LastMovedTick { get; set; }
        public int ticksHere => Find.TickManager.TicksGame - LastMovedTick;

    //    public new float warmupTime { get => base.warmupTime; set => base.warmupTime = value; }
        public new float AdjustedCooldown(Verb ownerVerb, Pawn attacker)
        {
            if (ownerVerb.verbProps != this)
            {
                Log.ErrorOnce("Tried to calculate cooldown for a verb with different verb props. verb=" + ownerVerb, 19485711, false);
                return 0f;
            }
            float baseCD = this.AdjustedCooldown(ownerVerb.tool, attacker, ownerVerb.EquipmentSource);
            baseCD += extraCooldown;
            if (ownerVerb.RapidFire(baseCD, out bool InRange, out float modified))
            {
                if (InRange)
                {
                    return modified;
                }
            }
            if (attacker.pather.MovedRecently(warmupTime.SecondsToTicks()))
            {
                Log.Message(attacker.Name+ " needs to set up");
            }
            return baseCD;
        }

        public void DrawExtraRadiusRings(IntVec3 center)
        {
            if (Find.CurrentMap == null)
            {
                return;
            }
            if (!this.IsMeleeAttack && this.targetable)
            {
                float num = this.EffectiveMinRange(true);
                if (this.range < (float)(Find.CurrentMap.Size.x + Find.CurrentMap.Size.z) && this.range < GenRadial.MaxRadialPatternRadius)
                {
                    Func<IntVec3, bool> predicate = null;
                    if (this.drawHighlightWithLineOfSight)
                    {
                        predicate = ((IntVec3 c) => GenSight.LineOfSight(center, c, Find.CurrentMap, false, null, 0, 0));
                    }

                    if (rapidFire)
                    {
                        DrawRapidFireRadiusRing(center, predicate);
                    }
                }
            }
        }
        public void DrawRapidFireRadiusRing(IntVec3 center, Func<IntVec3, bool> predicate = null)
        {
            Color color = new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, 100);
            GenDraw.DrawRadiusRing(center, this.range * this.rapidFireRange, color, predicate);
        }
        public new float GetHitChanceFactor(Thing equipment, float dist)
        {
            float value;
            if (dist <= 3f)
            {
                value = this.AdjustedAccuracy(RangeCategory.Touch, equipment);
            }
            else if (dist <= 12f)
            {
                value = Mathf.Lerp(this.AdjustedAccuracy(RangeCategory.Touch, equipment), this.AdjustedAccuracy(RangeCategory.Short, equipment), (dist - 3f) / 9f);
            }
            else if (dist <= 25f)
            {
                value = Mathf.Lerp(this.AdjustedAccuracy(RangeCategory.Short, equipment), this.AdjustedAccuracy(RangeCategory.Medium, equipment), (dist - 12f) / 13f);
            }
            else if (dist <= 40f)
            {
                value = Mathf.Lerp(this.AdjustedAccuracy(RangeCategory.Medium, equipment), this.AdjustedAccuracy(RangeCategory.Long, equipment), (dist - 25f) / 15f);
            }
            else
            {
                value = this.AdjustedAccuracy(RangeCategory.Long, equipment);
            }
            return Mathf.Clamp(value, 0.01f, 1f);
        }
        // Token: 0x0600055E RID: 1374 RVA: 0x0001AE90 File Offset: 0x00019090
        private float AdjustedAccuracy(RangeCategory cat, Thing equipment)
        {
            if (equipment != null)
            {
                StatDef stat = null;
                float mult = 1f;
                switch (cat)
                {
                    case RangeCategory.Touch:
                        stat = StatDefOf.AccuracyTouch;
                        mult = this.accuracyTouch;
                        break;
                    case RangeCategory.Short:
                        stat = StatDefOf.AccuracyShort;
                        mult = this.accuracyShort;
                        break;
                    case RangeCategory.Medium:
                        stat = StatDefOf.AccuracyMedium;
                        mult = this.accuracyMedium;
                        break;
                    case RangeCategory.Long:
                        stat = StatDefOf.AccuracyLong;
                        mult = this.accuracyLong;
                        break;
                }
                return equipment.GetStatValue(stat, true) * mult;
            }
            switch (cat)
            {
                case RangeCategory.Touch:
                    return this.accuracyTouch;
                case RangeCategory.Short:
                    return this.accuracyShort;
                case RangeCategory.Medium:
                    return this.accuracyMedium;
                case RangeCategory.Long:
                    return this.accuracyLong;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
