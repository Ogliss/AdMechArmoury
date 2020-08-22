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
    [HarmonyPatch(typeof(VerbProperties), "AdjustedArmorPenetration", new[] { typeof(Tool), typeof(Pawn), typeof(Thing), typeof(HediffComp_VerbGiver) })]
    public static class VerbProperties_AdjustedArmorPenetration_RendingWeapon_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref VerbProperties __instance, Tool tool, Pawn attacker, Thing equipment, HediffComp_VerbGiver hediffCompSource, ref float __result)
        {
            if (tool != null)
            {
                if (!tool.capacities.NullOrEmpty())
                {
                    if (tool.capacities.Any(x => x.defName.Contains("OG_RendingWeapon")))
                    {
                        float RendingChance = 0.167f;

                        if (equipment != null)
                        {
                            CompWeapon_MeleeSpecialRules _MeleeSpecialRules = equipment?.TryGetComp<CompWeapon_MeleeSpecialRules>();
                            if (_MeleeSpecialRules != null)
                            {
                                RendingChance = _MeleeSpecialRules.RendingChance;
                            }
                        }
                        else
                        {
                            if (attacker != null)
                            {
                                foreach (Tool item in attacker.Tools.Where(x => x != tool))
                                {
                                    if (item.capacities.Any(x => x.defName.Contains("OG_RendingWeapon")))
                                    {
                                        RendingChance += 0.167f;
                                    }
                                }
                            }
                        }

                        if (Rand.Chance(RendingChance))
                        {
                        //    MoteMaker.ThrowText(attacker.Position.ToVector3(), attacker.MapHeld, "AMA_Rending_Strike".Translate(), 3f);
                            __result = 2f;
                            return;
                        }
                    }
                }
            }
            /*
            if (__instance.EquipmentSource != null)
            {
                if (!__instance.EquipmentSource.AllComps.NullOrEmpty())
                {
                    if (__instance.EquipmentSource.GetComp<CompWeapon_MeleeSpecialRules>() != null)
                    {
                        if (__instance.EquipmentSource.GetComp<CompWeapon_MeleeSpecialRules>() is CompWeapon_MeleeSpecialRules WeaponRules)
                        {
                            if (AMASettings.Instance.AllowRendingMeleeEffect)
                            {
                                bool RendingAttack = __result.Any(x => x.Def.rendingWeapon());
                                if (WeaponRules.RendingWeapon && RendingAttack && __instance.CasterPawn is Pawn Caster)
                                {

                                }
                            }
                        }
                    }
                }
            }
            */
        }
    }

}