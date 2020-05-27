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
using System.Reflection;
using AdeptusMechanicus.ExtensionMethods;
using UnityEngine;
using AdeptusMechanicus.settings;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Verb_MeleeAttackDamage), "DamageInfosToApply")]
    public static class AM_Verb_MeleeAttackDamage_DamageInfosToApply_ForceWeapon_Patch
    {
        [HarmonyPostfix]
        public static IEnumerable<DamageInfo> Postfix(IEnumerable<DamageInfo> __result, Verb_MeleeAttackDamage __instance, LocalTargetInfo target)
        {
            foreach (DamageInfo info in __result)
            {
                Log.Message(string.Format("info: Amount {0}, AP {1}", info.Amount, info.ArmorPenetrationInt));
                bool returnoriginal = true;
                if (AMSettings.Instance.AllowForceWeaponEffect && info.Def.forceWeapon())
                {
                    returnoriginal = false;
                    DamageInfo dinfo = GetForceDamage(info, __instance, target);
                    Log.Message(string.Format("post GetForceDamage"));
                    yield return dinfo;
                }
                else if (info.Def.powerWeapon() || (!AMSettings.Instance.AllowForceWeaponEffect && info.Def.forceWeapon()))
                {
                    returnoriginal = false;
                    //    info.SetIgnoreArmor(true);
                    DamageInfo dinfo = GetPowerDamage(info, __instance, target);
                    Log.Message(string.Format("post GetPowerDamage: AP {0}", dinfo.ArmorPenetrationInt));
                    yield return dinfo;
                }
                else if (info.Def.witchbladeWeapon())
                {
                    returnoriginal = false;
                    DamageInfo dinfo = GetWitchbladeDamage(info, __instance, target);
                    Log.Message(string.Format("post GetWitchbladeDamage: Amount {0}, AP {1}", dinfo.Amount, dinfo.ArmorPenetrationInt));
                    yield return dinfo;
                }
                if (returnoriginal)
                {
                    Log.Message(string.Format("Original"));
                    yield return info;
                }
            }
            yield break;
        }

        public static DamageInfo GetPowerDamage(DamageInfo cloneSource, Verb_MeleeAttackDamage __instance, LocalTargetInfo target)
        {
            float AP = 2;
            DamageInfo damage = new DamageInfo(
            cloneSource.Def,
            cloneSource.Amount,
            AP,
            cloneSource.Angle,
            cloneSource.Instigator,
            cloneSource.HitPart,
            cloneSource.Weapon,
            cloneSource.Category,
            cloneSource.IntendedTarget
            );
            return damage;
        }

        public static DamageInfo GetWitchbladeDamage(DamageInfo cloneSource, Verb_MeleeAttackDamage __instance, LocalTargetInfo target)
        {
            Pawn Caster = cloneSource.Instigator as Pawn;
            if (Caster != null)
            {
                if (Caster.isPsyker(out int Level, out float Mult))
                {
                    float AP = 2;
                    float Amount = cloneSource.Amount;
                    DamageInfo damage = new DamageInfo(
                    cloneSource.Def,
                    Amount * (Level * Mult),
                    AP,
                    cloneSource.Angle,
                    cloneSource.Instigator,
                    cloneSource.HitPart,
                    cloneSource.Weapon,
                    cloneSource.Category,
                    cloneSource.IntendedTarget
                    );
                    return damage;
                }
            }
            return cloneSource;
        }

        public static DamageInfo GetForceDamage(DamageInfo cloneSource, Verb_MeleeAttackDamage __instance, LocalTargetInfo target)
        {
            Pawn Caster = __instance.CasterPawn as Pawn;
            if (Caster != null)
            {
                if (Caster.isPsyker(out int Level, out float Mult))
                {
                    Log.Message(Caster.NameShortColored + " Level " + Level + " Mult " + Mult);
                    if (__instance.EquipmentSource != null)
                    {
                        CompWeapon_MeleeSpecialRules WeaponRules = __instance.EquipmentSource.TryGetComp<CompWeapon_MeleeSpecialRules>();
                        if (WeaponRules != null)
                        {
                            if (WeaponRules.ForceWeapon)
                            {
                                bool casterPsychiclySensitive = Caster.RaceProps.Humanlike ? Caster.story.traits.HasTrait(TraitDefOf.PsychicSensitivity) : false;
                                bool Activate = false;
                                if ((casterPsychiclySensitive || !WeaponRules.ForceEffectRequiresPsyker) && target.Thing.def.category == ThingCategory.Pawn && target.Thing is Pawn Victim)
                                {
                                    int casterPsychiclySensitiveDegree = casterPsychiclySensitive ? Caster.story.traits.DegreeOfTrait(TraitDefOf.PsychicSensitivity) : 0;
                                    if ((casterPsychiclySensitiveDegree >= 1 || !WeaponRules.ForceEffectRequiresPsyker))
                                    {
                                        float? casterPsychicSensitivity = Caster.GetStatValue(StatDefOf.PsychicSensitivity, true) * 100f;
                                        bool targetPsychiclySensitive = Victim.RaceProps.Humanlike ? Victim.story.traits.HasTrait(TraitDefOf.PsychicSensitivity) : false;
                                        float? targetPsychicSensitivity = Victim.GetStatValue(StatDefOf.PsychicSensitivity, true) * 100f;
                                        if (targetPsychiclySensitive == true)
                                        {
                                            int targetPsychiclySensitiveDegree = Victim.story.traits.DegreeOfTrait(TraitDefOf.PsychicSensitivity);
                                            if (targetPsychiclySensitiveDegree == -1) { targetPsychicSensitivity = Victim.def.statBases.GetStatValueFromList(StatDefOf.PsychicSensitivity, 1.5f) * 100f; }
                                            else if (targetPsychiclySensitiveDegree == -2) { targetPsychicSensitivity = Victim.def.statBases.GetStatValueFromList(StatDefOf.PsychicSensitivity, 2f) * 100f; }
                                        }
                                        else { /*int targetPsychiclySensitiveDegree = 0;*/ }
                                        {
                                            float CasterMood = Caster.needs.mood.CurLevelPercentage;
                                            float VictimMood = Victim?.needs?.mood != null ? Victim.needs.mood.CurLevelPercentage : 1;
                                            Rand.PushState();
                                            float? casterRoll = Rand.Range(0, (int)casterPsychicSensitivity) * CasterMood;
                                            float? targetRoll = Rand.Range(0, (int)targetPsychicSensitivity) * VictimMood;
                                            Rand.PopState();
                                            casterRoll = (casterRoll - (targetPsychicSensitivity / 2));
                                            Activate = (casterRoll > targetRoll);
                                            Log.Message(string.Format("Caster:{0}, Victim:{1}", casterRoll, targetRoll));
                                            if (Activate)
                                            {
                                                DamageDef damDef = WeaponRules.ForceWeaponEffect;
                                                float damAmount = __instance.verbProps.AdjustedMeleeDamageAmount(__instance, __instance.CasterPawn);
                                                float armorPenetration = __instance.verbProps.AdjustedArmorPenetration(__instance, __instance.CasterPawn);
                                                BodyPartRecord bodyPart = Rand.Chance(0.05f) && Victim.RaceProps.body.AllParts.Any(x => x.def.defName.Contains("Brain")) ? Victim.RaceProps.body.AllParts.Find(x => x.def.defName.Contains("Brain")) : null;
                                                BodyPartGroupDef bodyPartGroupDef = null;
                                                HediffDef hediffDef = WeaponRules.ForceWeaponHediff;
                                                damAmount = Rand.Range(damAmount * 0.1f, damAmount * 0.5f);
                                                ThingDef source = __instance.EquipmentSource.def;
                                                Thing caster = __instance.caster;
                                                Vector3 direction = (target.Thing.Position - __instance.CasterPawn.Position).ToVector3();
                                                float num = damAmount;
                                                DamageInfo mainDinfo = new DamageInfo(damDef, num, 2, -1f, caster, bodyPart, source, DamageInfo.SourceCategory.ThingOrUnknown, null);
                                                mainDinfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
                                                mainDinfo.SetWeaponBodyPartGroup(bodyPartGroupDef);
                                                mainDinfo.SetWeaponHediff(hediffDef);
                                                mainDinfo.SetAngle(direction);
                                                Victim.TakeDamage(mainDinfo);
                                                Map map = Caster.Map;
                                                IntVec3 position = target.Cell;
                                                Map map2 = map;
                                                float explosionRadius = 0f;
                                                Thing launcher = __instance.EquipmentSource;
                                                SoundDef soundExplode = WeaponRules.ForceWeaponTriggerSound;
                                                Thing thing = target.Thing;
                                                GenExplosion.DoExplosion(position, map2, explosionRadius, damDef, launcher, (int)damAmount, armorPenetration, soundExplode, source, null, thing, null, 0f, 0, false, null, 0, 0, 0, false);
                                                float KillChance = WeaponRules.ForceWeaponKillChance;
                                                if (KillChance != 0)
                                                {
                                                    float KillRoll = Rand.Range(0, 100);
                                                    if (Rand.Chance(WeaponRules.ForceWeaponKillChance))
                                                    {
                                                        string msg = string.Format("{0} was slain by a force strike", target.Thing.LabelCap);
                                                        target.Thing.Kill(mainDinfo);
                                                        if (target.Thing.Faction == Faction.OfPlayer) { Messages.Message(msg, MessageTypeDefOf.PawnDeath); }
                                                    }
                                                }
                                                /*
                                                */
                                                return mainDinfo;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return cloneSource;
        }
    }
    /*
    [HarmonyPatch(typeof(Verb_MeleeAttackDamage), "ApplyMeleeDamageToTarget")]
    public static class AM_Verb_MeleeAttackDamage_ApplyMeleeDamageToTarget_ForceWeapon_Patch
    {
        [HarmonyPrefix]
        public static void ApplyMeleeDamageToTarget_ForceWeapon_Prefix(DamageWorker.DamageResult __instance ,LocalTargetInfo target)
        {
            
                            bool PowerAttack = __result.Any(x => x.Def.forceWeapon());
                            if (WeaponRules.ForceWeapon && ForceAttack)
                            {

                            }
        }
    }
    */
}