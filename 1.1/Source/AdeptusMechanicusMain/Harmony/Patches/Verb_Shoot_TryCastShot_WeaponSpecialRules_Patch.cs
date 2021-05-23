using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using Verse.Sound;
using UnityEngine;
using System.Reflection;
using AdeptusMechanicus.ExtensionMethods;
using AdeptusMechanicus.Lasers;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Verb_Shoot), "TryCastShot")]
    public static class Verb_Shoot_TryCastShot_WeaponSpecialRules_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(ref Verb_Shoot __instance, MethodBase __originalMethod)
        {
            //    Log.Warning("TryCastShot");
            IAdvancedVerb entry = __instance.SpecialRules();
            if (entry==null)
            {
            //    Log.Message("no SpecialRules detected");
                return true;
            }
            bool UserEffect = entry.EffectsUser;
            HediffDef UserHediff = entry.UserEffect;
            float AddHediffChance = entry.EffectsUserChance;
            List<string> Immunitylist = entry.UserEffectImmuneList;
            string msg = string.Format("");
            float failChance = entry.FailChance(__instance.EquipmentSource, out string reliabilityString);

            bool failed = false;
            if (failChance > 0f)
            {
                if (entry.Debug) Log.Message("failChance: "+failChance);
                Rand.PushState();
                failed = Rand.Chance(failChance);
                Rand.PopState();
                if (entry.Debug) Log.Message((entry.GetsHot ? "Overheat" : "Jam") + " Chance: " + failChance +"% Result: "+ (failed ? (entry.GetsHot ? "Overheated" : "Jamed") : "Passed"));
                //    Log.Message("failed: "+failed);
            }
            if (failed)
            {
                bool stillFire = true;
                bool canDamageWeapon = entry.HotDamageWeapon || entry.JamsDamageWeapon;
                MessageTypeDef msgDef = entry.GetsHot ? MessageTypeDefOf.NegativeHealthEvent : MessageTypeDefOf.SilentInput;
                float extraWeaponDamage = entry.HotDamageWeapon ? entry.HotDamage : entry.JamDamage;
                if (entry.GetsHot)
                {
                    string overheat = "overheated";
                    string causing = "causing";
                    DamageDef damageDef = __instance.Projectile.projectile.damageDef;
                    HediffDef HediffToAdd = damageDef.hediff;
                    Pawn launcherPawn = __instance.caster as Pawn;
                    Rand.PushState();
                    bool crit = Rand.Chance(entry.GetsHotCritChance);
                    Rand.PopState();
                    bool critExplode = false;
                    if (crit)
                    {
                        overheat = "critically overheated";
                        if (entry.GetsHotCritExplosion)
                        {
                            critExplode = Rand.Chance(entry.GetsHotCritExplosionChance);
                            if (critExplode)
                            {
                                causing = "causing an explosion";
                                CriticalOverheatExplosion(__instance);
                            }
                        }
                    }
                    float ArmorPenetration = __instance.Projectile.projectile.GetArmorPenetration(__instance.EquipmentSource, null) * (crit ? 1f : 0.25f);
                    float DamageAmount = __instance.Projectile.projectile.GetDamageAmount(__instance.EquipmentSource, null) * (crit ? 1f : 0.25f);
                    msg = string.Format("{0}'s {1} " + overheat + ". ({2} chance) "+ causing + " {3} damage", __instance.caster.LabelCap, __instance.EquipmentSource.LabelCap, failChance.ToStringPercent(), DamageAmount);
                    float damageLeft = DamageAmount;
                    List<BodyPartTagDef> tagDefs = new List<BodyPartTagDef>() { BodyPartTagDefOf.ManipulationLimbDigit, BodyPartTagDefOf.ManipulationLimbSegment };
                    List<BodyPartRecord> list = launcherPawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Outside, tagDefs).ToList<BodyPartRecord>();
                    if (list.NullOrEmpty())
                    {
                        list = launcherPawn.health.hediffSet.GetNotMissingParts().Where(x => x.def.defName.Contains("Arm") || x.parent.def.defName.Contains("Arm")).ToList<BodyPartRecord>();
                    }
                    if (list.NullOrEmpty())
                    {
                        list = launcherPawn.health.hediffSet.GetNotMissingParts().Where(x => x.def.tags.Contains(BodyPartTagDefOf.ManipulationLimbCore) || x.def.tags.Contains(BodyPartTagDefOf.ManipulationLimbSegment) || x.def.tags.Contains(BodyPartTagDefOf.ManipulationLimbDigit)).ToList<BodyPartRecord>();
                    }
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
                    else
                    {
                    }
                    Messages.Message(msg, MessageTypeDefOf.NegativeHealthEvent);
                }
                if (entry.Jams)
                {
                    if (!__instance.GetsHot())
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
                    return false;

                }
            }
            if (__instance.UserEffect(out float Chance, out HediffDef Effect, out StatDef ResistStat, out List<string> ImmuneList))
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
                    Rand.PushState();
                    var rand = Rand.Value; // This is a random percentage between 0% and 100%
                    Rand.PopState();
                    //    Log.Message(string.Format("GunExt.EffectsUser Effect: {0}, Chance: {1}, Roll: {2}, Result: {3}" + GunExt.ResistEffectStat != null ? ", Resist Stat: "+GunExt.ResistEffectStat.LabelCap+", Resist Amount"+ __instance.caster.GetStatValue(GunExt.ResistEffectStat, true) : null, GunExt.UserEffect.LabelCap, AddHediffChance, rand, rand <= AddHediffChance));
                    if (rand <= AddHediffChance) // If the percentage falls under the chance, success!
                    {
                        Rand.PushState();
                        var randomSeverity = Rand.Range(0.05f, 0.15f);
                        Rand.PopState();
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
            return true;
        }

        public static FieldInfo currentTarget = typeof(Verb_Shoot).GetField("currentTarget", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
        public static FieldInfo canHitNonTargetPawnsNow = typeof(Verb_Shoot).GetField("canHitNonTargetPawnsNow", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
        // Token: 0x0600651E RID: 25886 RVA: 0x001B8BC0 File Offset: 0x001B6FC0
        public static bool TryCastExtraShot(ref Verb_Shoot __instance, LocalTargetInfo currentTarget, bool canHitNonTargetPawnsNow)
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
            if (__instance.verbProps.stopBurstWithoutLos && !__instance.TryFindShootLineFromTo(__instance.caster.Position, currentTarget, out ShootLine shootLine))
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
            CompMannable compMannable = __instance.caster.TryGetCompFast<CompMannable>();
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
                    Rand.PushState();
                    int num2 = Rand.Range(0, max);
                    Rand.PopState();
                    if (num2 > 0)
                    {
                        IntVec3 c = currentTarget.Cell + GenRadial.RadialPattern[num2];

                        ProjectileHitFlags projectileHitFlags = ProjectileHitFlags.NonTargetWorld;
                        Rand.PushState();
                        if (Rand.Chance(0.5f))
                        {
                            projectileHitFlags = ProjectileHitFlags.All;
                        }
                        Rand.PopState();
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
            ThingDef targetCoverDef = randomCoverToMissInto?.def;

            Rand.PushState();
            bool f1 = !Rand.Chance(shotReport.AimOnTargetChance_IgnoringPosture);
            Rand.PopState();
            if (f1)
            {
                shootLine.ChangeDestToMissWild(shotReport.AimOnTargetChance_StandardTarget);
                ProjectileHitFlags projectileHitFlags2 = ProjectileHitFlags.NonTargetWorld;
                Rand.PushState();
                if (Rand.Chance(0.5f) && canHitNonTargetPawnsNow)
                {
                    projectileHitFlags2 |= ProjectileHitFlags.NonTargetPawns;
                }
                Rand.PopState();
                projectile2.Launch(launcher, drawPos, shootLine.Dest, currentTarget, projectileHitFlags2, equipment, targetCoverDef);
                return true;
            }
            Rand.PushState();
            bool f2 = !Rand.Chance(shotReport.PassCoverChance);
            Rand.PopState();
            if (currentTarget.Thing != null && currentTarget.Thing.def.category == ThingCategory.Pawn && f2)
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
            GenExplosion.DoExplosion(position, map2, explosionRadius, damageDef, launcher, DamageAmount, ArmorPenetration, soundExplode);//, equipmentDef, def, thing, postExplosionSpawnThingDef, postExplosionSpawnChance, postExplosionSpawnThingCount, EquipmentSource.def.projectile.applyDamageToExplosionCellsNeighbors, preExplosionSpawnThingDef, EquipmentSource.def.projectile.preExplosionSpawnChance, EquipmentSource.def.projectile.preExplosionSpawnThingCount, EquipmentSource.def.projectile.explosionChanceToStartFire, EquipmentSource.def.projectile.explosionDamageFalloff);
            return;
        }
    }

}
