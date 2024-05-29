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
        public static void DamageInfosToApply_ForceWeapon_Postfix(ref Verb_MeleeAttackDamage __instance, LocalTargetInfo target, ref IEnumerable<DamageInfo> __result)
        {
            if (__instance.EquipmentSource != null)
            {
                if (!__instance.EquipmentSource.AllComps.NullOrEmpty())
                {
                    if (__instance.EquipmentSource.GetComp<CompWeapon_MeleeSpecialRules>() != null)
                    {
                        if (__instance.EquipmentSource.GetComp<CompWeapon_MeleeSpecialRules>() is CompWeapon_MeleeSpecialRules WeaponRules)
                        {
                            if (AMASettings.Instance.AllowForceWeaponEffect)
                            {
                                bool ForceAttack = __result.Any(x => x.Def.forceWeapon());
                                if (WeaponRules.ForceWeapon && ForceAttack && __instance.CasterPawn is Pawn Caster)
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
                                            else { int targetPsychiclySensitiveDegree = 0; }
                                            if (__result.Any(x => x.Def.forceWeapon()))
                                            {
                                                Log.Message(string.Format("1"));
                                                float CasterMood = Caster.needs.mood.CurLevelPercentage;
                                                float VictimMood = Victim?.needs?.mood != null ? Victim.needs.mood.CurLevelPercentage : 1;
                                                foreach (var item in __result.Where(x => x.Def.forceWeapon()))
                                                {
                                                    float? casterRoll = Rand.Range(0, (int)casterPsychicSensitivity) * CasterMood;
                                                    float? targetRoll = Rand.Range(0, (int)targetPsychicSensitivity) * VictimMood;
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
                                                        float num2 = armorPenetration;
                                                        DamageInfo mainDinfo = new DamageInfo(damDef, num, num2, -1f, caster, bodyPart, source, DamageInfo.SourceCategory.ThingOrUnknown, null);
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
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    /*
    [HarmonyPatch(typeof(Verb_MeleeAttackDamage), "ApplyMeleeDamageToTarget")]
    public static class AM_Verb_MeleeAttackDamage_ApplyMeleeDamageToTarget_ForceWeapon_Patch
    {
        [HarmonyPrefix]
        public static void ApplyMeleeDamageToTarget_ForceWeapon_Prefix(DamageWorker.DamageResult __instance ,LocalTargetInfo target)
        {
            
        }
    }
    */
}