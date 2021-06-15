using AdeptusMechanicus.ExtensionMethods;
using AdeptusMechanicus.Lasers;
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
    public partial class AdvancedVerbProperties : VerbProperties, IMuzzlePosition, IAdvancedVerb
    {
        public bool debug = true;

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

        public bool muzzleFlareRotates = true;
        public Color muzzleFlareColor = Color.white;
        public Color muzzleFlareColorTwo = Color.white;
        public string muzzleFlareDef = "Mote_SparkFlash";
        public float muzzleFlareSize = -1f;
        private FleckDef _muzzleFlareDef;
        public FloatRange? muzzleFlareSizeRange;

        public string muzzleSmokeDef = "OG_Mote_SmokeTrail";
        public float muzzleSmokeSize = 0.35f;
        private FleckDef _muzzleSmokeDef;
        public FloatRange? muzzleSmokeSizeRange;

        public bool gizmosOnEquip = true;
        #region IMuzzleposition
        public float BarrelOffset => barrelOffset;
        public float BarrelLength => barrelLength;
        public float BulletOffset => defaultProjectile as Lasers.LaserBeamDef == null ? bulletOffset : bulletOffset + laserOffset;
        public float MuzzleSmokeSize => muzzleSmokeSizeRange?.RandomInRange ?? muzzleSmokeSize;
        public float MuzzleFlareSize => muzzleFlareSizeRange?.RandomInRange ?? (muzzleFlareSize > 0 ? muzzleFlareSize : muzzleFlashScale * 0.25f);
        public FleckDef MuzzleFlareDef => _muzzleFlareDef ??= !muzzleFlareDef.NullOrEmpty() ? DefDatabase<FleckDef>.GetNamed(muzzleFlareDef) : null;
        public bool MuzzleFlareRotates => muzzleFlareRotates;
        public Color MuzzleFlareColor => muzzleFlareColor;
        public Color MuzzleFlareColorTwo => muzzleFlareColorTwo;
        public FleckDef MuzzleSmokeDef => _muzzleSmokeDef ??= !muzzleSmokeDef.NullOrEmpty() ? DefDatabase<FleckDef>.GetNamed(muzzleSmokeDef) : null;
        #endregion

        #region IAdvancedVerb    
        public bool Debug => debug;
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
        public new float AdjustedAccuracy(RangeCategory cat, Thing equipment)
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


        public virtual float FailChance(Thing gun, out string reliabilityString)
        {
            float failChance = 0;
            reliabilityString = string.Empty;
            if (GetsHot || Jams)
            {
                StatPart_Reliability.GetReliability(this, gun, out reliabilityString, out failChance);
                failChance *= (GetsHot ? 0.1f : 0.01f);
            }
            if (Debug)
            {
                Log.Message("FailChance for"+ reliabilityString + " "+gun+": "+failChance);
            }
            return failChance;
        }

        public virtual bool CheckFail(ref Verb_Shoot __instance)
        {
            string msg = string.Format("");
            Thing gun = __instance.EquipmentSource;
            bool failed = false;
            float failChance = FailChance(gun, out string reliabilityString);
            if (failChance > 0f)
            {
                Rand.PushState();
                failed = Rand.Chance(failChance);
                Rand.PopState();
                if (Debug) Log.Message((GetsHot ? "Overheat" : "Jam") + " Chance: " + failChance + "% Result: " + (failed ? (GetsHot ? "Overheated" : "Jamed") : "Passed"));
            }
            if (failed)
            {
                bool stillFire = true;
                bool canDamageWeapon = HotDamageWeapon || JamsDamageWeapon;
                MessageTypeDef msgDef = GetsHot ? MessageTypeDefOf.NegativeHealthEvent : MessageTypeDefOf.SilentInput;
                float extraWeaponDamage = HotDamageWeapon ? HotDamage : JamDamage;
                if (GetsHot)
                {
                    string overheat = "overheated";
                    string causing = "causing";
                    DamageDef damageDef = __instance.Projectile.projectile.damageDef;
                    HediffDef HediffToAdd = damageDef.hediff;
                    Pawn launcherPawn = __instance.caster as Pawn;
                    Rand.PushState();
                    bool crit = Rand.Chance(GetsHotCritChance);
                    Rand.PopState();
                    bool critExplode = false;
                    if (crit)
                    {
                        overheat = "critically overheated";
                        if (GetsHotCritExplosion)
                        {
                            critExplode = Rand.Chance(GetsHotCritExplosionChance);
                            if (critExplode)
                            {
                                causing = "causing an explosion";
                                CriticalOverheatExplosion(__instance);
                            }
                        }
                    }
                    float ArmorPenetration = __instance.Projectile.projectile.GetArmorPenetration(__instance.EquipmentSource, null) * (crit ? 1f : 0.25f);
                    float DamageAmount = __instance.Projectile.projectile.GetDamageAmount(__instance.EquipmentSource, null) * (crit ? 1f : 0.25f);
                    msg = string.Format("{0}'s {1} " + overheat + ". ({2} chance) " + causing + " {3} damage", __instance.caster.LabelCap, __instance.EquipmentSource.LabelCap, failChance.ToStringPercent(), DamageAmount);
                    float damageLeft = DamageAmount;
                    List<BodyPartTagDef> tagDefs = new List<BodyPartTagDef>() { BodyPartTagDefOf.ManipulationLimbDigit, BodyPartTagDefOf.ManipulationLimbSegment };
                    List<BodyPartRecord> list = launcherPawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Outside, tagDefs).ToList<BodyPartRecord>();
                    if (!list.NullOrEmpty())
                    {
                        while (damageLeft > 0f && !list.NullOrEmpty())
                        {
                            Rand.PushState();
                            BodyPartRecord part = list.RandomElement();
                            list.Remove(part);
                            float maxPartDamage = Math.Min(damageLeft, launcherPawn.health.hediffSet.GetPartHealth(part));
                            float amount = Rand.Range(1f, Math.Min(damageLeft, maxPartDamage));
                            Rand.PopState();
                            if (amount > 0)
                            {
                                /*
                                Hediff hediff = HediffMaker.MakeHediff(HediffToAdd, launcherPawn, part);
                                hediff.Severity = severity;
                                launcherPawn.health.AddHediff(hediff, part, null);
                                */
                                DamageInfo info = new DamageInfo(damageDef, amount, ArmorPenetration, -1, __instance.EquipmentSource, part, __instance.EquipmentSource.def, DamageInfo.SourceCategory.ThingOrUnknown, __instance.CurrentTarget.Thing ?? null);
                                launcherPawn.TakeDamage(info);
                                damageLeft -= amount;
                            }
                        }
                    }
                }
                if (Jams)
                {
                    if (!__instance.GetsHot())
                    {
                        msg = string.Format("{0}'s {1} had a weapon jam. ({2} chance)", __instance.caster.LabelCap, __instance.EquipmentSource.LabelCap, failChance.ToStringPercent());

                    }
                    float defaultCooldownTime = __instance.verbProps.defaultCooldownTime * 2;
                    __instance.verbProps.defaultCooldownTime = defaultCooldownTime;
                    if (canDamageWeapon)
                    {
                        if (extraWeaponDamage != 0f)
                        {
                            if (__instance.EquipmentSource != null)
                            {

                                if (__instance.EquipmentSource.HitPoints - (int)extraWeaponDamage >= 0)
                                {
                                    __instance.EquipmentSource.HitPoints = __instance.EquipmentSource.HitPoints - (int)extraWeaponDamage;
                                }
                                else if (__instance.EquipmentSource.HitPoints - (int)extraWeaponDamage < 0)
                                {
                                    __instance.EquipmentSource.HitPoints = 0;
                                    __instance.EquipmentSource.Destroy();
                                }
                            }
                            if (__instance.HediffCompSource != null)
                            {
                                /*
                                if (__instance.HediffCompSource.parent.Part.HitPoints - (int)extraWeaponDamage >= 0)
                                {
                                    __instance.HediffCompSource.HitPoints = __instance.HediffCompSource.HitPoints - (int)extraWeaponDamage;
                                }
                                else if (__instance.HediffCompSource.HitPoints - (int)extraWeaponDamage < 0)
                                {
                                    __instance.HediffCompSource.HitPoints = 0;
                                    __instance.HediffCompSource.Destroy();
                                }
                                */
                            }
                        }
                        else
                        {
                            if (__instance.EquipmentSource != null)
                            {
                                if (__instance.EquipmentSource.HitPoints > 0)
                                {
                                    __instance.EquipmentSource.HitPoints--;
                                }
                            }
                        }
                    }
                    if (__instance.EquipmentSource != null)
                    {
                        SpinningLaserGun spinner = __instance.EquipmentSource as SpinningLaserGun;
                        if (spinner != null)
                        {
                            spinner.state = SpinningLaserGunBase.State.Idle;
                            spinner.ReachRotationSpeed(0, 0);
                        }
                    }
                }
                Messages.Message(msg, msgDef);
            }
            return failed;
        }

        public static void CriticalOverheatExplosion(Verb_Shoot __instance)
        {
            Map map = __instance.caster.Map;
            if (__instance.Projectile.projectile.explosionEffect != null)
            {
                Effecter effecter = __instance.Projectile.projectile.explosionEffect.Spawn();
                effecter.Trigger(new TargetInfo(__instance.EquipmentSource.Position, map, false), new TargetInfo(__instance.EquipmentSource.Position, map, false));
                effecter.Cleanup();
            }
            IntVec3 position = __instance.caster.Position;
            Map map2 = map;
            float explosionRadius = __instance.Projectile.projectile.explosionRadius;
            DamageDef damageDef = __instance.Projectile.projectile.damageDef;
            Thing launcher = __instance.EquipmentSource;
            int DamageAmount = __instance.Projectile.projectile.GetDamageAmount(__instance.EquipmentSource, null);
            float ArmorPenetration = __instance.Projectile.projectile.GetArmorPenetration(__instance.EquipmentSource, null);
            SoundDef soundExplode = __instance.Projectile.projectile.soundExplode;
            ThingDef equipmentDef = __instance.EquipmentSource.def;
            ThingDef def = __instance.EquipmentSource.def;
            Thing thing = __instance.EquipmentSource;
            ThingDef postExplosionSpawnThingDef = __instance.Projectile.projectile.postExplosionSpawnThingDef;
            float postExplosionSpawnChance = __instance.Projectile.projectile.postExplosionSpawnChance;
            int postExplosionSpawnThingCount = __instance.Projectile.projectile.postExplosionSpawnThingCount;
            ThingDef preExplosionSpawnThingDef = __instance.Projectile.projectile.preExplosionSpawnThingDef;
            GenExplosion.DoExplosion(position, map2, explosionRadius, damageDef, launcher, DamageAmount, ArmorPenetration, soundExplode, equipmentDef, def, thing);//, postExplosionSpawnThingDef, postExplosionSpawnChance, postExplosionSpawnThingCount, EquipmentSource.def.projectile.applyDamageToExplosionCellsNeighbors, preExplosionSpawnThingDef, EquipmentSource.def.projectile.preExplosionSpawnChance, EquipmentSource.def.projectile.preExplosionSpawnThingCount, EquipmentSource.def.projectile.explosionChanceToStartFire, EquipmentSource.def.projectile.explosionDamageFalloff);
            return;
        }

    }
}
