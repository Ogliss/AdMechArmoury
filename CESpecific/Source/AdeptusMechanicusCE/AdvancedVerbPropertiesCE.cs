using AdeptusMechanicus;
using AdeptusMechanicus.ExtensionMethods;
using AdeptusMechanicus.Lasers;
using CombatExtended;
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
    // AdeptusMechanicus.AdvancedVerbPropertiesCE
    public class AdvancedVerbPropertiesCE : VerbPropertiesCE, IMuzzlePosition, IAdvancedVerb
    {
        private float heavyWeaponSetupTime = -1f;

        private Reliability reliability = Reliability.NA;

        private bool rapidFire = false;
        private float rapidFireRange = 0.55f;

        private bool bodySizeBurst = false;
        private float bodySizeBurstModifier = 1f;

        private float hotDamage = 0f;
        private float getsHotCritChance = 0f;
        private float getsHotCritExplosionChance = 0f;

        private bool getsHot = false;

        private float jamDamage = 0f;
        private float extraCooldown = 0f;

        private float effectsUserChance = 0f;
        private StatDef resistEffectStat = null;
        private HediffDef userEffect = null;

        private bool rending = false;
        private float rendingChance = 0.167f;

        private int scattershotCount = 0;
        private ResearchProjectDef requiredResearch = null;
        private List<string> userEffectImmuneList = new List<string>();
        private float barrelLength = 1f;
        private float barrelOffset = 0f;
        private float bulletOffset = 0.2f;
        private float laserOffset = -0.4f;

        private bool muzzleFlareRotates = true;
        private Color muzzleFlareColor = Color.white;
        private Color muzzleFlareColorTwo = Color.white;
        private string muzzleFlareDef = "Mote_SparkFlash";
        private float muzzleFlareSize = -1f;
        private ThingDef _muzzleFlareDef;
        private FloatRange? muzzleFlareSizeRange;

        private string muzzleSmokeDef = "OG_Mote_SmokeTrail";
        private float muzzleSmokeSize = 0.35f;
        private ThingDef _muzzleSmokeDef;
        private FloatRange? muzzleSmokeSizeRange;

        public bool gizmosOnEquip = true;
        #region IMuzzleposition
        public float BarrelOffset => barrelOffset;
        public float BarrelLength => barrelLength;
        public float BulletOffset => defaultProjectile as LaserBeamDefCE == null ? bulletOffset : bulletOffset + laserOffset;
        public float MuzzleSmokeSize => muzzleSmokeSizeRange?.RandomInRange ?? muzzleSmokeSize;
        public float MuzzleFlareSize => muzzleFlareSizeRange?.RandomInRange ?? (muzzleFlareSize > 0 ? muzzleFlareSize : muzzleFlashScale * 0.25f);
        public ThingDef MuzzleFlareDef => _muzzleFlareDef ??= !muzzleFlareDef.NullOrEmpty() ? DefDatabase<ThingDef>.GetNamed(muzzleFlareDef) : null;
        public bool MuzzleFlareRotates => muzzleFlareRotates;
        public Color MuzzleFlareColor => muzzleFlareColor;
        public Color MuzzleFlareColorTwo => muzzleFlareColorTwo;
        public ThingDef MuzzleSmokeDef => _muzzleSmokeDef ??= !muzzleSmokeDef.NullOrEmpty() ? DefDatabase<ThingDef>.GetNamed(muzzleSmokeDef) : null;
        #endregion

        #region IAdvancedVerb    
        public bool RapidFire => rapidFire;
        public float RapidFireRange => rapidFireRange;
        public Reliability Reliability => reliability;
        public int ScattershotCount => defaultProjectile?.GetModExtensionFast<ScattershotProjectileExtension>()?.projectileCount ?? scattershotCount;
        public bool Jams => reliability != Reliability.NA;
        public bool JamsDamageWeapon => jamDamage > 0;
        public float JamDamage => jamDamage;
        public bool GetsHot => getsHot;
        public bool HotDamageWeapon => hotDamage > 0f;
        public float HotDamage => hotDamage;
        public bool GetsHotCrit => getsHotCritChance > 0f;
        public float GetsHotCritChance => getsHotCritChance;
        public bool GetsHotCritExplosion => getsHotCritExplosionChance > 0f;
        public float GetsHotCritExplosionChance => getsHotCritExplosionChance;
        public bool Rending => rending;
        public float RendingChance => rendingChance;
        public bool EffectsUser => effectsUserChance > 0 && userEffect != null;
        public float EffectsUserChance => effectsUserChance;
        public HediffDef UserEffect => userEffect;
        public StatDef ResistEffectStat => resistEffectStat;
        public List<string> UserEffectImmuneList => userEffectImmuneList;
        public bool TwinLinked => ScattershotCount == 1;
        public bool Multishot => ScattershotCount > 1;
        public bool HeavyWeapon => heavyWeaponSetupTime > 0;
        public float HeavyWeaponSetupTime => heavyWeaponSetupTime;
        public ResearchProjectDef RequiredResearch => requiredResearch;
        public int ticksHere => Find.TickManager.TicksGame - LastMovedTick;
        public int LastMovedTick { get; set; }
        #endregion

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
                Log.Message(attacker.Name + " needs to set up");
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
        public float AdjustedAccuracy(RangeCategory cat, Thing equipment)
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
