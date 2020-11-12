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
using CombatExtended;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Verb_ShootCE), "WarmupComplete")]
    public static class Verb_ShootCE_TryCastShot_WeaponSpecialRules_Patch
    {
        public static bool Prefix(ref Verb_ShootCE __instance)
        {
        //    Log.Warning("CE_TryCastShot_WeaponSpecialRules_Patch WarmupComplete ");
            GunVerbEntry entry = __instance.SpecialRules();
            if (entry==null)
            {
            //    Log.Message("no SpecialRules detected");
                return true;
            }
            bool canDamageWeapon;
            float extraWeaponDamage;
            bool TwinLinked = entry.TwinLinked;
            bool Multishot = entry.Multishot;
            int ScattershotCount = entry.ScattershotCount;
            bool UserEffect = entry.EffectsUser;
            HediffDef UserHediff = entry.UserEffect;
            float AddHediffChance = entry.EffectsUserChance;
            List<string> Immunitylist = entry.UserEffectImmuneList;
            string msg = string.Format("");
            string reliabilityString;
            float failChance;
            
            StatPart_Reliability.GetReliability(entry, __instance.EquipmentSource, out reliabilityString, out failChance);
            failChance = (__instance.GetsHot()) ? (failChance / 10) : (failChance / 100);
            bool failed = false;
            if (__instance.GetsHot() || __instance.Jams())
            {
            //    Log.Message("failChance: "+failChance);
                Rand.PushState();
                failed = Rand.Chance(failChance);
                Rand.PopState();
            //    Log.Message("failed: "+failed);
            }
            if (__instance.GetsHot(out bool GetsHotCrit, out float GetsHotCritChance, out bool GetsHotCritExplosion, out float GetsHotCritExplosionChance, out canDamageWeapon, out extraWeaponDamage))
            {
                if (failed)
                {
                    DamageDef damageDef = __instance.Projectile.projectile.damageDef;
                    HediffDef HediffToAdd = damageDef.hediff;
                    float ArmorPenetration = __instance.Projectile.projectile.GetArmorPenetration(__instance.EquipmentSource, null);
                    float DamageAmount = 0;
                    Pawn launcherPawn = __instance.caster as Pawn;
                    Rand.PushState();
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
                    Rand.PopState();
                    float maxburndmg = DamageAmount / 10;
                    while (DamageAmount > 0f)
                    {
                        List<BodyPartRecord> list = launcherPawn.health.hediffSet.GetNotMissingParts().Where(x => x.def.defName.Contains("Finger") || x.def.defName.Contains("Hand")).ToList<BodyPartRecord>();
                        if (list.NullOrEmpty())
                        {
                            list = launcherPawn.health.hediffSet.GetNotMissingParts().Where(x => x.def.defName.Contains("Arm") || x.parent.def.defName.Contains("Arm")).ToList<BodyPartRecord>();
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
                            Rand.PushState();
                            float severity = Rand.Range(Math.Min(0.1f, DamageAmount), Math.Min(DamageAmount, maxburndmg));
                            Rand.PopState();
                            hediff = HediffMaker.MakeHediff(HediffToAdd, launcherPawn, part);
                            hediff.Severity = severity;
                            launcherPawn.health.AddHediff(hediff, part, null);
                            DamageAmount -= severity;
                        }
                    }
                    Messages.Message(msg, MessageTypeDefOf.NegativeHealthEvent);
                }
            }
            if (__instance.Jams(out canDamageWeapon, out extraWeaponDamage))
            {
                if (failed)
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
            
            if (__instance.MultiShot() || __instance.TwinLinked())
            {
                if (__instance.TwinLinked())
                {
                    
                }
                else
                {

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

        public static void CriticalOverheatExplosion(ref CombatExtended.Verb_ShootCE __instance)
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
