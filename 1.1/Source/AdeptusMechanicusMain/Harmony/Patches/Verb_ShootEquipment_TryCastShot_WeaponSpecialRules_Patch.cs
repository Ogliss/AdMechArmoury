using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using AdeptusMechanicus.settings;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(AbilitesExtended.Verb_EquipmentLaunchProjectile), "TryCastShot")]
    public static class Verb_ShootEquipment_TryCastShot_WeaponSpecialRules_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(ref AbilitesExtended.Verb_EquipmentLaunchProjectile __instance)
        {
            //    Log.Warning("TryCastShot");
            bool GetsHot = __instance.verbProperties.GetsHot;
            bool Jams = __instance.verbProperties.Jams;
            bool GetsHotCrit = __instance.verbProperties.GetsHotCrit;
            float GetsHotCritChance = __instance.verbProperties.GetsHotCritChance;
            bool GetsHotCritExplosion = __instance.verbProperties.GetsHotCritExplosion;
            float GetsHotCritExplosionChance = __instance.verbProperties.GetsHotCritExplosionChance;
            bool canDamageWeapon = __instance.verbProperties.HotDamageWeapon || __instance.verbProperties.JamsDamageWeapon;
            float extraWeaponDamage = (Jams && __instance.verbProperties.JamsDamageWeapon) ? __instance.verbProperties.JamDamage : (GetsHot && __instance.verbProperties.HotDamageWeapon) ? __instance.verbProperties.HotDamage : 0f;
            bool TwinLinked = __instance.verbProperties.TwinLinked;
            bool Multishot = __instance.verbProperties.Multishot;
            int ScattershotCount = __instance.verbProperties.ScattershotCount;
            bool UserEffect = __instance.verbProperties.EffectsUser;
            HediffDef UserHediff = __instance.verbProperties.UserEffect;
            float AddHediffChance = __instance.verbProperties.EffectsUserChance;
            List<string> Immunitylist = __instance.verbProperties.UserEffectImmuneList;
            if ((GetsHot && AMSettings.Instance.AllowGetsHot) || (Jams && AMSettings.Instance.AllowJams))
            {
                string msg = string.Format("");
                string reliabilityString;
                float failChance;
                AbilitesExtended.StatPart_Reliability.GetReliability(__instance.verbProperties, out reliabilityString, out failChance);
                failChance = GetsHot ? (failChance / 10) : (failChance / 100);
                if (Rand.Chance(failChance))
                {
                    if (GetsHot)
                    {
                        DamageDef damageDef = __instance.Projectile.projectile.damageDef;
                        HediffDef HediffToAdd = damageDef.hediff;
                        float ArmorPenetration = __instance.Projectile.projectile.GetArmorPenetration(__instance.EquipmentSource, null);
                        float DamageAmount = 0;
                        Pawn launcherPawn = __instance.caster as Pawn;
                        if (Rand.Chance(GetsHotCritChance))
                        {
                            DamageAmount = __instance.Projectile.projectile.GetDamageAmount(__instance.EquipmentSource, null);
                            msg = string.Format("{0}'s {1} critically overheated. ({2} chance) causing {3} damage", __instance.caster.LabelCap, __instance.EquipmentSource.LabelCap, failChance.ToStringPercent(), DamageAmount);
                            if (GetsHotCritExplosion && Rand.Chance(GetsHotCritExplosionChance)) { CriticalOverheatExplosion(ref __instance); }
                        }
                        else
                        {
                            DamageAmount = __instance.Projectile.projectile.GetDamageAmount(__instance.EquipmentSource, null);
                            msg = string.Format("{0}'s {1} overheated. ({2} chance) causing {3} damage", __instance.caster.LabelCap, __instance.EquipmentSource.LabelCap, failChance.ToStringPercent(), DamageAmount);
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
                        msg = string.Format("{0}'s {1} had a weapon jam. ({2} chance)", __instance.caster.LabelCap, __instance.EquipmentSource.LabelCap, failChance.ToStringPercent());
                        Messages.Message(msg, MessageTypeDefOf.SilentInput);
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
                            if (__instance.EquipmentSource != null)
                            {
                                if (__instance.EquipmentSource.HitPoints > 0)
                                {
                                    __instance.EquipmentSource.HitPoints--;
                                }
                            }
                        }
                    }
                    if (Jams)
                    {
                        if (__instance.EquipmentSource != null)
                        {
                            SpinningLaserGun spinner = (SpinningLaserGun)__instance.EquipmentSource;
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
            if (ScattershotCount > 0 && Multishot && AMSettings.Instance.AllowMultiShot || TwinLinked)
            {
                Traverse traverse = Traverse.Create(__instance);
                LocalTargetInfo currentTarget = (LocalTargetInfo)Verb_Shoot_TryCastShot_WeaponSpecialRules_Patch.currentTarget.GetValue(__instance);
                bool canHitNonTargetPawnsNow = (bool)Verb_Shoot_TryCastShot_WeaponSpecialRules_Patch.canHitNonTargetPawnsNow.GetValue(__instance);
                //    Log.Message(string.Format("AllowMultiShot: {0} Projectile Count: {1}", AMASettings.Instance.AllowMultiShot && Multishot, ScattershotCount));
                if (TwinLinked)
                {
                    TryCastExtraShot(ref __instance, currentTarget, canHitNonTargetPawnsNow);
                }
                else
                {
                    for (int i = 0; i < ScattershotCount; i++)
                    {
                        TryCastExtraShot(ref __instance, currentTarget, canHitNonTargetPawnsNow);
                    }
                }

            }
            if (UserEffect && AMSettings.Instance.AllowUserEffects)
            {
                if (__instance.caster.def.category == ThingCategory.Pawn)
                {
                    bool Immunityflag = false;
                    Pawn launcherPawn = __instance.caster as Pawn;
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
            return true;
        }

        public static FieldInfo currentTarget = typeof(Verb_Shoot).GetField("currentTarget", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
        public static FieldInfo canHitNonTargetPawnsNow = typeof(Verb_Shoot).GetField("canHitNonTargetPawnsNow", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
        // Token: 0x0600651E RID: 25886 RVA: 0x001B8BC0 File Offset: 0x001B6FC0
        public static bool TryCastExtraShot(ref AbilitesExtended.Verb_EquipmentLaunchProjectile __instance, LocalTargetInfo currentTarget, bool canHitNonTargetPawnsNow)
        {
            if (currentTarget.HasThing && currentTarget.Thing.Map != __instance.caster.Map)
            {
                return false;
            }
            ThingDef projectile = __instance.Projectile;
            if (projectile == null)
            {
                return false;
            }
            ShootLine shootLine;
            bool flag = __instance.TryFindShootLineFromTo(__instance.caster.Position, currentTarget, out shootLine);
            if (__instance.verbProps.stopBurstWithoutLos && !flag)
            {
                return false;
            }
            if (__instance.EquipmentSource != null)
            {
                CompChangeableProjectile comp = __instance.EquipmentSource.GetComp<CompChangeableProjectile>();
                if (comp != null)
                {
                    comp.Notify_ProjectileLaunched();
                }
            }
            Thing launcher = __instance.caster;
            Thing equipment = __instance.EquipmentSource;
            CompMannable compMannable = __instance.caster.TryGetComp<CompMannable>();
            if (compMannable != null && compMannable.ManningPawn != null)
            {
                launcher = compMannable.ManningPawn;
                equipment = __instance.caster;
            }
            Vector3 drawPos = __instance.caster.DrawPos;
            Projectile projectile2 = (Projectile)GenSpawn.Spawn(projectile, shootLine.Source, __instance.caster.Map, WipeMode.Vanish);
            if (__instance.verbProps.forcedMissRadius > 0.5f)
            {
                float num = VerbUtility.CalculateAdjustedForcedMiss(__instance.verbProps.forcedMissRadius, currentTarget.Cell - __instance.caster.Position);
                if (num > 0.5f)
                {
                    int max = GenRadial.NumCellsInRadius(num);
                    int num2 = Rand.Range(0, max);
                    if (num2 > 0)
                    {
                        IntVec3 c = currentTarget.Cell + GenRadial.RadialPattern[num2];

                        ProjectileHitFlags projectileHitFlags = ProjectileHitFlags.NonTargetWorld;
                        if (Rand.Chance(0.5f))
                        {
                            projectileHitFlags = ProjectileHitFlags.All;
                        }
                        if (!canHitNonTargetPawnsNow)
                        {
                            projectileHitFlags &= ~ProjectileHitFlags.NonTargetPawns;
                        }
                        projectile2.Launch(launcher, drawPos, c, currentTarget, projectileHitFlags, equipment, null);
                        return true;
                    }
                }
            }
            ShotReport shotReport = ShotReport.HitReportFor(__instance.caster, __instance, currentTarget);
            Thing randomCoverToMissInto = shotReport.GetRandomCoverToMissInto();
            ThingDef targetCoverDef = (randomCoverToMissInto == null) ? null : randomCoverToMissInto.def;
            if (!Rand.Chance(shotReport.AimOnTargetChance_IgnoringPosture))
            {
                shootLine.ChangeDestToMissWild(shotReport.AimOnTargetChance_StandardTarget);
                ProjectileHitFlags projectileHitFlags2 = ProjectileHitFlags.NonTargetWorld;
                if (Rand.Chance(0.5f) && canHitNonTargetPawnsNow)
                {
                    projectileHitFlags2 |= ProjectileHitFlags.NonTargetPawns;
                }
                projectile2.Launch(launcher, drawPos, shootLine.Dest, currentTarget, projectileHitFlags2, equipment, targetCoverDef);
                return true;
            }
            if (currentTarget.Thing != null && currentTarget.Thing.def.category == ThingCategory.Pawn && !Rand.Chance(shotReport.PassCoverChance))
            {

                ProjectileHitFlags projectileHitFlags3 = ProjectileHitFlags.NonTargetWorld;
                if (canHitNonTargetPawnsNow)
                {
                    projectileHitFlags3 |= ProjectileHitFlags.NonTargetPawns;
                }
                projectile2.Launch(launcher, drawPos, randomCoverToMissInto, currentTarget, projectileHitFlags3, equipment, targetCoverDef);
                return true;
            }
            ProjectileHitFlags projectileHitFlags4 = ProjectileHitFlags.IntendedTarget;
            if (canHitNonTargetPawnsNow)
            {
                projectileHitFlags4 |= ProjectileHitFlags.NonTargetPawns;
            }
            if (!currentTarget.HasThing || currentTarget.Thing.def.Fillage == FillCategory.Full)
            {
                projectileHitFlags4 |= ProjectileHitFlags.NonTargetWorld;
            }
            if (currentTarget.Thing != null)
            {
                projectile2.Launch(launcher, drawPos, currentTarget, currentTarget, projectileHitFlags4, equipment, targetCoverDef);
            }
            else
            {
                projectile2.Launch(launcher, drawPos, shootLine.Dest, currentTarget, projectileHitFlags4, equipment, targetCoverDef);
            }
            return true;
        }
        public static void CriticalOverheatExplosion(ref AbilitesExtended.Verb_EquipmentLaunchProjectile __instance)
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
            GenExplosion.DoExplosion(position, map2, explosionRadius, damageDef, launcher, DamageAmount, ArmorPenetration, soundExplode);//, equipmentDef, def, thing, postExplosionSpawnThingDef, postExplosionSpawnChance, postExplosionSpawnThingCount, EquipmentSource.def.projectile.applyDamageToExplosionCellsNeighbors, preExplosionSpawnThingDef, EquipmentSource.def.projectile.preExplosionSpawnChance, EquipmentSource.def.projectile.preExplosionSpawnThingCount, EquipmentSource.def.projectile.explosionChanceToStartFire, EquipmentSource.def.projectile.explosionDamageFalloff);
            return;
        }
    }

}
