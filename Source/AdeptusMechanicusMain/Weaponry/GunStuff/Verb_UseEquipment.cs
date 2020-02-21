using System;
using System.Collections.Generic;
using System.Linq;
using AbilityUser;
using AdeptusMechanicus.settings;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
	
	// Token: 0x02000024 RID: 36
	public class VerbProperties_EquipmentAbility : VerbProperties_Ability
    {
        public bool RapidFire = false;

        public bool TyranidBurstBodySize = false;

        public Reliability reliability = Reliability.NA;
        public bool GetsHot = false;
        public bool HotDamageWeapon = false;
        public float HotDamage = 0f;
        public bool GetsHotCrit = false;
        public float GetsHotCritChance = 0f;
        public bool GetsHotCritExplosion = false;
        public float GetsHotCritExplosionChance = 0f;

        public bool Jams = false;
        public bool JamsDamageWeapon = false;
        public float JamDamage = 0f;

        public bool TwinLinked = false;
        public bool Multishot = false;

        public bool EffectsUser = false;
        public float EffectsUserChance = 0f;
        public StatDef ResistEffectStat = null;
        public HediffDef UserEffect = null;
        public bool Rending = false;
        public float RendingChance = 0.167f;

        public DamageDef ForceWeaponEffect = null;
        public HediffDef ForceWeaponHediff = null;
        public float ForceWeaponKillChance = 0f;
        public SoundDef ForceWeaponTriggerSound = null;

        public int ScattershotCount = 0;
        public ResearchProjectDef requiredResearch = null;
        public VerbProperties VerbProps;
        public List<string> UserEffectImmuneList = new List<string>();
    }

    // Token: 0x02000025 RID: 37
    public class Verb_UseEquipment : Verb_UseAbility
    {
        public new VerbProperties_EquipmentAbility UseAbilityProps
        {
            get
            {
                return (VerbProperties_EquipmentAbility)this.verbProps;
            }
        }
        // Token: 0x1700002C RID: 44
        // (get) Token: 0x060000E4 RID: 228 RVA: 0x00007E7B File Offset: 0x0000607B
        protected override int ShotsPerBurst
        {
            get
            {
                if (this.UseAbilityProps.RapidFire)
                {
                    float rangeRF = this.UseAbilityProps.range / 2;
                    if (caster.Position.InHorDistOf(((Pawn)caster).TargetCurrentlyAimingAt.Cell, rangeRF))
                    {
                        return verbProps.burstShotCount * 2;
                    }
                }
                else if (this.UseAbilityProps.TyranidBurstBodySize)
                {

                    return verbProps.burstShotCount * (int)CasterPawn.BodySize;
                }
                return this.verbProps.burstShotCount;
            }
        }

        public ThingWithComps EquipmentSource
        {
            get
            {
                return AbilityUserComp.AbilityData.TemporaryWeaponPowers.Contains(Ability)? CasterPawn.equipment.Primary : null ;
            }
        }
        // Token: 0x060000E5 RID: 229 RVA: 0x00007E88 File Offset: 0x00006088
        public override float HighlightFieldRadiusAroundTarget(out bool needLOSToCenter)
        {
            needLOSToCenter = true;
            VerbProperties verbProps = this.verbProps;
            float? num;
            if (verbProps == null)
            {
                num = null;
            }
            else
            {
                ThingDef defaultProjectile = verbProps.defaultProjectile;
                if (defaultProjectile == null)
                {
                    num = null;
                }
                else
                {
                    ProjectileProperties projectile = defaultProjectile.projectile;
                    num = ((projectile != null) ? new float?(projectile.explosionRadius) : null);
                }
            }
            float result = num ?? 1f;
            bool flag = this.UseAbilityProps.abilityDef.MainVerb.TargetAoEProperties != null;
            if (flag)
            {
                bool showRangeOnSelect = this.UseAbilityProps.abilityDef.MainVerb.TargetAoEProperties.showRangeOnSelect;
                if (showRangeOnSelect)
                {
                    result = (float)this.UseAbilityProps.abilityDef.MainVerb.TargetAoEProperties.range;
                }
            }
            return result;
        }

        private void DebugMessage(string s)
        {
            bool flag = this.debugMode;
            if (flag)
            {
                Log.Message(s, false);
            }
        }
        private void ThrowDebugText(string text)
        {
            if (DebugViewSettings.drawShooting)
            {
                MoteMaker.ThrowText(this.caster.DrawPos, this.caster.Map, text, -1f);
            }
        }
        // Token: 0x06006520 RID: 25888 RVA: 0x001B9057 File Offset: 0x001B7457
        private void ThrowDebugText(string text, IntVec3 c)
        {
            if (DebugViewSettings.drawShooting)
            {
                MoteMaker.ThrowText(c.ToVector3Shifted(), this.caster.Map, text, -1f);
            }
        }
        protected new virtual void UpdateTargets()
        {
            TargetsAoE.Clear();
            if (UseAbilityProps.AbilityTargetCategory == AbilityTargetCategory.TargetAoE)
            {
                //Log.Message("AoE Called");
                if (UseAbilityProps.TargetAoEProperties == null)
                    Log.Error("Tried to Cast AoE-Ability without defining a target class");

                var targets = new List<Thing>();

                //Handle TargetAoE start location.
                var aoeStartPosition = caster.PositionHeld;
                if (!UseAbilityProps.TargetAoEProperties.startsFromCaster)
                    aoeStartPosition = currentTarget.Cell;

                //Handle friendly fire targets.
                if (!UseAbilityProps.TargetAoEProperties.friendlyFire)
                {
                    targets = caster.Map.listerThings.AllThings.Where(x =>
                        x.Position.InHorDistOf(aoeStartPosition, UseAbilityProps.TargetAoEProperties.range) &&
                        UseAbilityProps.TargetAoEProperties.targetClass.IsAssignableFrom(x.GetType()) &&
                        x.Faction.HostileTo(Faction.OfPlayer)).ToList();
                }
                else if (UseAbilityProps.TargetAoEProperties.targetClass == typeof(Plant) ||
                         UseAbilityProps.TargetAoEProperties.targetClass == typeof(Building))
                {
                    targets = caster.Map.listerThings.AllThings.Where(x =>
                        x.Position.InHorDistOf(aoeStartPosition, UseAbilityProps.TargetAoEProperties.range) &&
                        UseAbilityProps.TargetAoEProperties.targetClass.IsAssignableFrom(x.GetType())).ToList();
                    foreach (var targ in targets)
                    {
                        var tinfo = new LocalTargetInfo(targ);
                        TargetsAoE.Add(tinfo);
                    }
                    return;
                }
                else
                {
                    targets.Clear();
                    targets = caster.Map.listerThings.AllThings.Where(x =>
                        x.Position.InHorDistOf(aoeStartPosition, UseAbilityProps.TargetAoEProperties.range) &&
                        UseAbilityProps.TargetAoEProperties.targetClass.IsAssignableFrom(x.GetType()) &&
                        (x.HostileTo(Faction.OfPlayer) || UseAbilityProps.TargetAoEProperties.friendlyFire)).ToList();
                }

                var maxTargets = UseAbilityProps.abilityDef.MainVerb.TargetAoEProperties.maxTargets;
                var randTargets = new List<Thing>(targets.InRandomOrder());
                for (var i = 0; i < maxTargets && i < randTargets.Count(); i++)
                {
                    var tinfo = new TargetInfo(randTargets[i]);
                    if (UseAbilityProps.targetParams.CanTarget(tinfo))
                        TargetsAoE.Add(new LocalTargetInfo(randTargets[i]));
                }
            }
            else
            {
                TargetsAoE.Clear();
                TargetsAoE.Add(currentTarget);
            }
        }

        protected override bool TryCastShot()
        {
            //    Log.Message("Try Cast Shot Called");
            //Log.Message("Cast");
            bool GetsHot = this.UseAbilityProps.GetsHot;
            bool Jams = this.UseAbilityProps.Jams;
            bool GetsHotCrit = this.UseAbilityProps.GetsHotCrit;
            float GetsHotCritChance = this.UseAbilityProps.GetsHotCritChance;
            bool GetsHotCritExplosion = this.UseAbilityProps.GetsHotCritExplosion;
            float GetsHotCritExplosionChance = this.UseAbilityProps.GetsHotCritExplosionChance;
            bool canDamageWeapon = this.UseAbilityProps.HotDamageWeapon || this.UseAbilityProps.JamsDamageWeapon;
            float extraWeaponDamage = (Jams && this.UseAbilityProps.JamsDamageWeapon) ? this.UseAbilityProps.JamDamage : (GetsHot && this.UseAbilityProps.HotDamageWeapon) ? this.UseAbilityProps.HotDamage : 0f;
            bool TwinLinked = this.UseAbilityProps.TwinLinked;
            bool Multishot = this.UseAbilityProps.Multishot;
            int ScattershotCount = this.UseAbilityProps.ScattershotCount;
            bool UserEffect = this.UseAbilityProps.EffectsUser;
            HediffDef UserHediff = this.UseAbilityProps.UserEffect;
            float AddHediffChance = this.UseAbilityProps.EffectsUserChance;
            List<string> Immunitylist = this.UseAbilityProps.UserEffectImmuneList;
            var result = false;
            TargetsAoE.Clear();
            UpdateTargets();
            var burstShots = ShotsPerBurst;
            if (UseAbilityProps.AbilityTargetCategory != AbilityTargetCategory.TargetAoE && TargetsAoE.Count > 1)
                TargetsAoE.RemoveRange(0, TargetsAoE.Count - 1);
            if (UseAbilityProps.mustHaveTarget && TargetsAoE.Count == 0)
            {
                Messages.Message("AU_NoTargets".Translate(), MessageTypeDefOf.RejectInput);
                Ability.Notify_AbilityFailed(true);
                return false;
            }

            for (var i = 0; i < TargetsAoE.Count; i++)
            {
                //                for (int j = 0; j < burstshots; j++)
                //                {
                if (verbProps.defaultProjectile != null) //ranged attacks WILL have projectiles
                {
                    if ((GetsHot && AMASettings.Instance.AllowGetsHot) || (Jams && AMASettings.Instance.AllowJams))
                    {
                        string msg = string.Format("");
                        string reliabilityString;
                        float failChance;
                        StatPart_Reliability.GetReliability(this.UseAbilityProps, out reliabilityString, out failChance);
                        failChance = GetsHot ? (failChance / 10) : (failChance / 100);
                        if (Rand.Chance(failChance))
                        {
                            if (GetsHot)
                            {
                                DamageDef damageDef = this.Projectile.projectile.damageDef;
                                HediffDef HediffToAdd = damageDef.hediff;
                                float ArmorPenetration = this.Projectile.projectile.GetArmorPenetration(this.EquipmentSource, null);
                                float DamageAmount = 0;
                                Pawn launcherPawn = this.caster as Pawn;
                                if (Rand.Chance(GetsHotCritChance))
                                {
                                    DamageAmount = this.Projectile.projectile.GetDamageAmount(this.EquipmentSource, null);
                                    msg = string.Format("{0}'s {1} critically overheated. ({2} chance) causing {3} damage", this.caster.LabelCap, this.EquipmentSource.LabelCap, failChance.ToStringPercent(), DamageAmount);
                                    if (GetsHotCritExplosion && Rand.Chance(GetsHotCritExplosionChance)) { CriticalOverheatExplosion(); }
                                }
                                else
                                {
                                    DamageAmount = this.Projectile.projectile.GetDamageAmount(this.EquipmentSource, null);
                                    msg = string.Format("{0}'s {1} overheated. ({2} chance) causing {3} damage", this.caster.LabelCap, this.EquipmentSource.LabelCap, failChance.ToStringPercent(), DamageAmount);
                                }
                                float maxburndmg = DamageAmount / 10;
                                while (DamageAmount > 0f)
                                {
                                    List<BodyPartRecord> list = launcherPawn.health.hediffSet.GetNotMissingParts().Where(x => x.def.defName.Contains("Finger") || x.def.defName.Contains("Hand")).ToList<BodyPartRecord>();
                                    if (list.NullOrEmpty())
                                    {
                                        list = launcherPawn.health.hediffSet.GetNotMissingParts().Where(x => x.def.defName.Contains("Arm") || x.def.defName.Contains("Shoulder")).ToList<BodyPartRecord>();
                                    }
                                    if (list.NullOrEmpty())
                                    {
                                        list = launcherPawn.health.hediffSet.GetNotMissingParts().Where(x => x.def.tags.Contains(BodyPartTagDefOf.ManipulationLimbCore) || x.def.tags.Contains(BodyPartTagDefOf.ManipulationLimbSegment) || x.def.tags.Contains(BodyPartTagDefOf.ManipulationLimbDigit)).ToList<BodyPartRecord>();
                                    }
                                    if (list.NullOrEmpty())
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        BodyPartRecord part = list.RandomElement();
                                        Hediff hediff;
                                        float severity = Rand.Range(Math.Min(0.1f, DamageAmount), Math.Min(DamageAmount, maxburndmg));
                                        hediff = HediffMaker.MakeHediff(HediffToAdd, launcherPawn, null);
                                        hediff.Severity = severity;
                                        launcherPawn.health.AddHediff(hediff, part, null);
                                        DamageAmount -= severity;
                                    }
                                }
                                Messages.Message(msg, MessageTypeDefOf.NegativeHealthEvent);
                            }
                            else
                            {
                                msg = string.Format("{0}'s {1} had a weapon jam. ({2} chance)", this.caster.LabelCap, this.EquipmentSource.LabelCap, failChance.ToStringPercent());
                                Messages.Message(msg, MessageTypeDefOf.SilentInput);
                            }
                            float defaultCooldownTime = this.verbProps.defaultCooldownTime * 2;
                            this.verbProps.defaultCooldownTime = defaultCooldownTime;
                            if (canDamageWeapon)
                            {
                                if (extraWeaponDamage != 0f)
                                {
                                    if (this.EquipmentSource != null)
                                    {

                                        if (this.EquipmentSource.HitPoints - (int)extraWeaponDamage >= 0)
                                        {
                                            this.EquipmentSource.HitPoints = this.EquipmentSource.HitPoints - (int)extraWeaponDamage;
                                        }
                                        else if (this.EquipmentSource.HitPoints - (int)extraWeaponDamage < 0)
                                        {
                                            this.EquipmentSource.HitPoints = 0;
                                            this.EquipmentSource.Destroy();
                                        }
                                    }
                                    if (this.HediffCompSource != null)
                                    {
                                        /*
                                        if (__instance.HediffCompSource.parent.Part..HitPoints - (int)extraWeaponDamage >= 0)
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
                                    if (this.EquipmentSource != null)
                                    {
                                        if (this.EquipmentSource.HitPoints > 0)
                                        {
                                            this.EquipmentSource.HitPoints--;
                                        }
                                    }
                                }
                            }
                            if (Jams)
                            {
                                if (this.EquipmentSource != null)
                                {
                                    SpinningLaserGun spinner = (SpinningLaserGun)this.EquipmentSource;
                                    if (spinner != null)
                                    {
                                        spinner.state = SpinningLaserGunBase.State.Idle;
                                        spinner.ReachRotationSpeed(0, 0);
                                    }
                                }
                                return false;
                            }
                        }
                    }

                    var attempt = TryLaunchProjectile(verbProps.defaultProjectile, TargetsAoE[i]);

                    if (ScattershotCount > 0 && Multishot && AMASettings.Instance.AllowMultiShot)
                    {
                        //    Log.Message(string.Format("AllowMultiShot: {0} Projectile Count: {1}", AMASettings.Instance.AllowMultiShot && Multishot, ScattershotCount));
                        for (int ii = 0; ii < ScattershotCount; ii++)
                        {
                            //    Log.Message(string.Format("Launching extra projectile {0} / {1}", i+1, ScattershotCount));
                            //    AccessTools.Method(typeof(Verb_Shoot).BaseType, "TryCastShot", null, null).Invoke(__instance, null);
                            TryLaunchProjectile(verbProps.defaultProjectile, TargetsAoE[i]);
                        }
                    }
                    else
                    if (TwinLinked)
                    {
                        TryLaunchProjectile(verbProps.defaultProjectile, TargetsAoE[i]);
                    }
                    if (UserEffect && AMASettings.Instance.AllowUserEffects)
                    {
                        if (caster.def.category == ThingCategory.Pawn)
                        {
                            bool Immunityflag = false;
                            Pawn launcherPawn = this.caster as Pawn;
                            if (!Immunitylist.NullOrEmpty())
                            {
                                foreach (var item in Immunitylist)
                                {
                                    Immunityflag = launcherPawn.def.defName.Contains(item);
                                    if (Immunityflag)
                                    {
                                        //    Log.Message(string.Format("{0} is immune to their {1}'s UseEffect", launcherPawn.LabelShortCap, __instance.EquipmentSource.LabelShortCap));
                                    }
                                }
                                /*
                                List<string> list = GunExt.UserEffectImmuneList.Where(x => DefDatabase<ThingDef>.GetNamedSilentFail(x) != null).ToList();
                                bool Immunityflag = list.Contains(launcherPawn.def.defName);
                                if (Immunityflag)
                                {
                                    return;
                                }
                                */
                            }
                            if (!Immunityflag)
                            {

                                var rand = Rand.Value; // This is a random percentage between 0% and 100%
                                                       //    Log.Message(string.Format("GunExt.EffectsUser Effect: {0}, Chance: {1}, Roll: {2}, Result: {3}" + GunExt.ResistEffectStat != null ? ", Resist Stat: "+GunExt.ResistEffectStat.LabelCap+", Resist Amount"+ __instance.caster.GetStatValue(GunExt.ResistEffectStat, true) : null, GunExt.UserEffect.LabelCap, AddHediffChance, rand, rand <= AddHediffChance));
                                if (rand <= AddHediffChance) // If the percentage falls under the chance, success!
                                {
                                    var randomSeverity = Rand.Range(0.05f, 0.15f);
                                    var effectOnPawn = launcherPawn?.health?.hediffSet?.GetFirstHediffOfDef(UserHediff);
                                    if (effectOnPawn != null)
                                    {
                                        effectOnPawn.Severity += randomSeverity;
                                    }
                                    else
                                    {
                                        Hediff hediff = HediffMaker.MakeHediff(UserHediff, launcherPawn, null);
                                        hediff.Severity = randomSeverity;
                                        launcherPawn.health.AddHediff(hediff, null, null);
                                    }
                                }
                            }
                        }
                    }
                    ////Log.Message(TargetsAoE[i].ToString());
                    if (attempt != null)
                    {
                        if (attempt == true) result = true;
                        if (attempt == false) result = false;
                    }
                }
                else //melee attacks WON'T have projectiles
                {
                //    Log.Message("No Projectile");
                    var victim = TargetsAoE[i].Thing;
                    if (victim != null)
                    {
                    //    Log.Message("Yes victim");
                        if (victim is Pawn pawnVictim)
                        {
                        //    Log.Message("Yes victim is pawn");
                            AbilityEffectUtility.ApplyMentalStates(pawnVictim, CasterPawn, UseAbilityProps.mentalStatesToApply, UseAbilityProps.abilityDef, null);
                            AbilityEffectUtility.ApplyHediffs(pawnVictim, CasterPawn, UseAbilityProps.hediffsToApply, null);
                            AbilityEffectUtility.SpawnSpawnables(UseAbilityProps.thingsToSpawn, pawnVictim, victim.MapHeld, victim.PositionHeld);
                        }
                    }
                    else
                    {
                    //    Log.Message("Victim is null");
                        AbilityEffectUtility.SpawnSpawnables(UseAbilityProps.thingsToSpawn, CasterPawn, CasterPawn.MapHeld, CasterPawn.PositionHeld);
                    }
                }
                //                }
            }

            PostCastShot(result, out result);
            if (result == false)
            {
                Ability.Notify_AbilityFailed(UseAbilityProps.refundsPointsAfterFailing);
            }
            return result;
        }

        protected new bool? TryLaunchProjectile(ThingDef projectileDef, LocalTargetInfo launchTarget)
        {
            DebugMessage(launchTarget.ToString());
            var flag = TryFindShootLineFromTo(caster.Position, launchTarget, out var shootLine);
            if (verbProps.stopBurstWithoutLos && !flag)
            {
                DebugMessage("Targeting cancelled");
                return false;
            }
            var drawPos = caster.DrawPos;
            var projectile2 = (Projectile_AbilityBase)GenSpawn.Spawn(projectileDef, shootLine.Source, caster.Map);
            projectile2.extraDamages = UseAbilityProps.extraDamages;
            projectile2.localSpawnThings = UseAbilityProps.thingsToSpawn;
            verbProps.soundCast?.PlayOneShot(new TargetInfo(caster.Position, caster.Map, false));
            verbProps.soundCastTail?.PlayOneShotOnCamera();
            if (DebugViewSettings.drawShooting)
                MoteMaker.ThrowText(caster.DrawPos, caster.Map, "ToHit", -1f);
			

            if (this.verbProps.forcedMissRadius > 0.5f)
            {
                float num = VerbUtility.CalculateAdjustedForcedMiss(this.verbProps.forcedMissRadius, this.currentTarget.Cell - this.caster.Position);
                if (num > 0.5f)
                {
                    int max = GenRadial.NumCellsInRadius(num);
                    int num2 = Rand.Range(0, max);
                    if (num2 > 0)
                    {
                        IntVec3 c = this.currentTarget.Cell + GenRadial.RadialPattern[num2];
                        this.ThrowDebugText("ToRadius");
                        this.ThrowDebugText("Rad\nDest", c);
                        ProjectileHitFlags projectileHitFlags = ProjectileHitFlags.NonTargetWorld;
                        if (Rand.Chance(0.5f))
                        {
                            projectileHitFlags = ProjectileHitFlags.All;
                        }
                        if (!this.canHitNonTargetPawnsNow)
                        {
                            projectileHitFlags &= ~ProjectileHitFlags.NonTargetPawns;
                        }
                    //    projectile2.Launch(CasterPawn, drawPos, c, this.currentTarget, projectileHitFlags, caster, null);
                        projectile2.Launch(caster, Ability.Def, drawPos, c, projectileHitFlags, null,
                            UseAbilityProps.hediffsToApply,
                            UseAbilityProps.mentalStatesToApply, UseAbilityProps.thingsToSpawn);
                        return true;
                    }
                }
            }
            ShotReport shotReport = ShotReport.HitReportFor(this.caster, this, this.currentTarget);
            Thing randomCoverToMissInto = shotReport.GetRandomCoverToMissInto();
            ThingDef targetCoverDef = (randomCoverToMissInto == null) ? null : randomCoverToMissInto.def;
            if (!Rand.Chance(shotReport.AimOnTargetChance_IgnoringPosture))
            {
                shootLine.ChangeDestToMissWild(shotReport.AimOnTargetChance_StandardTarget);
                this.ThrowDebugText("ToWild" + ((!this.canHitNonTargetPawnsNow) ? string.Empty : "\nchntp"));
                this.ThrowDebugText("Wild\nDest", shootLine.Dest);
                ProjectileHitFlags projectileHitFlags2 = ProjectileHitFlags.NonTargetWorld;
                if (Rand.Chance(0.5f) && this.canHitNonTargetPawnsNow)
                {
                    projectileHitFlags2 |= ProjectileHitFlags.NonTargetPawns;
                }
                //    projectile2.Launch(CasterPawn, drawPos, shootLine.Dest, this.currentTarget, projectileHitFlags2, caster, targetCoverDef);
                projectile2.Launch(caster, Ability.Def, drawPos, shootLine.Dest, projectileHitFlags2, null,
                    UseAbilityProps.hediffsToApply,
                    UseAbilityProps.mentalStatesToApply, UseAbilityProps.thingsToSpawn);
                return true;
            }
            if (this.currentTarget.Thing != null && this.currentTarget.Thing.def.category == ThingCategory.Pawn && !Rand.Chance(shotReport.PassCoverChance))
            {
                this.ThrowDebugText("ToCover" + ((!this.canHitNonTargetPawnsNow) ? string.Empty : "\nchntp"));
                this.ThrowDebugText("Cover\nDest", randomCoverToMissInto.Position);
                ProjectileHitFlags projectileHitFlags3 = ProjectileHitFlags.NonTargetWorld;
                if (this.canHitNonTargetPawnsNow)
                {
                    projectileHitFlags3 |= ProjectileHitFlags.NonTargetPawns;
                }
            //    projectile2.Launch(CasterPawn, drawPos, randomCoverToMissInto, this.currentTarget, projectileHitFlags3, caster, targetCoverDef);
                projectile2.Launch(caster, Ability.Def, drawPos, randomCoverToMissInto, projectileHitFlags3, null,
                    UseAbilityProps.hediffsToApply,
                    UseAbilityProps.mentalStatesToApply, UseAbilityProps.thingsToSpawn);
                return true;
            }
            ProjectileHitFlags projectileHitFlags4 = ProjectileHitFlags.IntendedTarget;
            if (this.canHitNonTargetPawnsNow)
            {
                projectileHitFlags4 |= ProjectileHitFlags.NonTargetPawns;
            }
            if (!this.currentTarget.HasThing || this.currentTarget.Thing.def.Fillage == FillCategory.Full)
            {
                projectileHitFlags4 |= ProjectileHitFlags.NonTargetWorld;
            }
            this.ThrowDebugText("ToHit" + ((!this.canHitNonTargetPawnsNow) ? string.Empty : "\nchntp"));
            if (this.currentTarget.Thing != null)
            {
            //    projectile2.Launch(CasterPawn, drawPos, this.currentTarget, this.currentTarget, projectileHitFlags4, caster, targetCoverDef);
                projectile2.Launch(caster, Ability.Def, drawPos, currentTarget, projectileHitFlags4, null,
                    UseAbilityProps.hediffsToApply,
                    UseAbilityProps.mentalStatesToApply, UseAbilityProps.thingsToSpawn);
                this.ThrowDebugText("Hit\nDest", this.currentTarget.Cell);
            }
            else
            {
            //    projectile2.Launch(CasterPawn, drawPos, shootLine.Dest, this.currentTarget, projectileHitFlags4, caster, targetCoverDef);
                projectile2.Launch(caster, Ability.Def, drawPos, shootLine.Dest, projectileHitFlags4, null,
                    UseAbilityProps.hediffsToApply,
                    UseAbilityProps.mentalStatesToApply, UseAbilityProps.thingsToSpawn);
                this.ThrowDebugText("Hit\nDest", shootLine.Dest);
            }
			/*
            ProjectileHitFlags projectileHitFlags4 = ProjectileHitFlags.IntendedTarget;
            if (this.canHitNonTargetPawnsNow)
            {
                projectileHitFlags4 |= ProjectileHitFlags.NonTargetPawns;
            }
            if (!this.currentTarget.HasThing || this.currentTarget.Thing.def.Fillage == FillCategory.Full)
            {
                projectileHitFlags4 |= ProjectileHitFlags.NonTargetWorld;
            }
            DebugMessage(launchTarget.ToString());
            projectile2.Launch(caster, Ability.Def, drawPos, launchTarget, projectileHitFlags4, null,
                UseAbilityProps.hediffsToApply,
                UseAbilityProps.mentalStatesToApply, UseAbilityProps.thingsToSpawn);
			*/
            return true;
        }

        public void CriticalOverheatExplosion()
        {
            Map map = this.caster.Map;
            if (this.Projectile.projectile.explosionEffect != null)
            {
                Effecter effecter = this.Projectile.projectile.explosionEffect.Spawn();
                effecter.Trigger(new TargetInfo(this.EquipmentSource.Position, map, false), new TargetInfo(this.EquipmentSource.Position, map, false));
                effecter.Cleanup();
            }
            IntVec3 position = this.caster.Position;
            Map map2 = map;
            float explosionRadius = this.Projectile.projectile.explosionRadius;
            DamageDef damageDef = this.Projectile.projectile.damageDef;
            Thing launcher = this.EquipmentSource;
            int DamageAmount = this.Projectile.projectile.GetDamageAmount(this.EquipmentSource, null);
            float ArmorPenetration = this.Projectile.projectile.GetArmorPenetration(this.EquipmentSource, null);
            SoundDef soundExplode = this.Projectile.projectile.soundExplode;
            ThingDef equipmentDef = this.EquipmentSource.def;
            ThingDef def = this.EquipmentSource.def;
            Thing thing = this.EquipmentSource;
            ThingDef postExplosionSpawnThingDef = this.Projectile.projectile.postExplosionSpawnThingDef;
            float postExplosionSpawnChance = this.Projectile.projectile.postExplosionSpawnChance;
            int postExplosionSpawnThingCount = this.Projectile.projectile.postExplosionSpawnThingCount;
            ThingDef preExplosionSpawnThingDef = this.Projectile.projectile.preExplosionSpawnThingDef;
            GenExplosion.DoExplosion(position, map2, explosionRadius, damageDef, launcher, DamageAmount, ArmorPenetration, soundExplode);//, equipmentDef, def, thing, postExplosionSpawnThingDef, postExplosionSpawnChance, postExplosionSpawnThingCount, EquipmentSource.def.projectile.applyDamageToExplosionCellsNeighbors, preExplosionSpawnThingDef, EquipmentSource.def.projectile.preExplosionSpawnChance, EquipmentSource.def.projectile.preExplosionSpawnThingCount, EquipmentSource.def.projectile.explosionChanceToStartFire, EquipmentSource.def.projectile.explosionDamageFalloff);
            return;
        }

        public override void WarmupComplete()
        {
            if (verbTracker == null)
                verbTracker = CasterPawn.verbTracker;
            burstShotsLeft = ShotsPerBurst;
            state = VerbState.Bursting;
            TryCastNextBurstShot();
            //Find.BattleLog.Add(new BattleLogEntry_RangedFire(this.caster,
            //    (!this.currentTarget.HasThing) ? null : this.currentTarget.Thing,
            //    (base.EquipmentSource == null) ? null : base.EquipmentSource.def, this.Projectile,
            //    this.ShotsPerBurst > 1));
        }
		
        // Token: 0x040000A2 RID: 162
        public List<LocalTargetInfo> TargetsAoE = new List<LocalTargetInfo>();

        // Token: 0x040000A3 RID: 163
        public Action<Thing> timeSavingActionVariable = null;

        // Token: 0x040000A5 RID: 165
        private bool debugMode = false;
    }
}
