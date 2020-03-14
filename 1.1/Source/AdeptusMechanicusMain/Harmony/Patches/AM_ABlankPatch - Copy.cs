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
using AdeptusMechanicus;
using AdeptusMechanicus.ExtensionMethods;
using UnityEngine;
using System.Reflection;

namespace AdeptusMechanicus
{

    // Token: 0x02000020 RID: 32 AdeptusMechanicus.PsySilencerExt
    public class PsySilencerExt : DefModExtension
    {

    }

    [HarmonyPatch(typeof(ThingRequiringRoyalPermissionUtility), "IsViolatingRulesOfAnyFaction", new Type[] { typeof(Def), typeof(Pawn), typeof(int), typeof(bool) })]
    public static class AMA_ThingRequiringRoyalPermissionUtility_IsViolatingRulesOfAnyFaction_ExtraSilencer_Patch
    {
        [HarmonyPrefix]
        public static bool Post_IsViolatingRulesOfAnyFaction(Def implantOrWeapon, Pawn pawn, int implantLevel, bool ignoreSilencer, ref bool __result)
        {
            Log.Message(string.Format("should {0} cast?", implantOrWeapon));
            bool silenced = pawn.health.hediffSet.hediffs.Any(x => x.def.HasModExtension<PsySilencerExt>());
            if (silenced)
            {
                Log.Message("ignoreing this cast");
                __result = false;
                return false;
            }
            return true;
        }
        //    public static FieldInfo graphic = typeof(Graphic).GetField("graphicInt", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
    }

    [HarmonyPatch(typeof(AbilityDef), "GetTooltip")]
    public static class AMA_AbilityDef_GetTooltip_ExtraSilencer_Patch
    {
        [HarmonyPostfix]
        public static void Post_GetTooltip(AbilityDef __instance, Pawn pawn, ref string __result)
        {
            if (pawn != null)
            {
                bool silenced = pawn.health.hediffSet.hediffs.Any(x => x.def.HasModExtension<PsySilencerExt>());
                if (silenced)
                {
                    __result = __instance.LabelCap + ((__instance.level > 0) ? ("\n" + "Level".Translate() + " " + __instance.level) : "") + "\n\n" + __instance.description + "\n\n" + __instance.StatSummary.ToLineList(null, false);
                }
            }
        }
    }

}
