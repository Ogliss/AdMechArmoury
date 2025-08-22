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
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{
    /*
    [HarmonyPatch(typeof(Pawn_StoryTracker), "TryGetRandomHeadFromSet")]
    public static class Pawn_StoryTracker_TryGetRandomHeadFromSet_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn_StoryTracker __instance, IEnumerable<HeadTypeDef> options, ref bool __result)
        {

            if (__instance.headType == null)
            {
                Rand.PushState(__instance.pawn.thingIDNumber);
                IEnumerable<HeadTypeDef> heads = options.Where(h=> CanUseHeadType(h, __instance.pawn));
                Log.Message($"heads useable: {heads.Count()}");
                if (heads.TryRandomElementByWeight((HeadTypeDef x) => x.selectionWeight, out __instance.headType))
                {
                    __result = true;
                }
                Rand.PopState();
            }

        }

        private static bool CanUseHeadType(HeadTypeDef head, Pawn pawn)
        {


            if (ModsConfig.BiotechActive && !head.requiredGenes.NullOrEmpty())
            {
                if (pawn.genes == null)
                {
                    return false;
                }
                foreach (GeneDef requiredGene in head.requiredGenes)
                {
                    if (!pawn.genes.HasGene(requiredGene))
                    {
                        return false;
                    }
                }
            }
            if (head.gender != 0)
            {
                return head.gender == pawn.gender;
            }
            return true;

        }
    }
    */
    [HarmonyPatch(typeof(Pawn_EquipmentTracker), "EquipmentTrackerTick")]
    public static class Pawn_EquipmentTracker_EquipmentTrackerTick_ActivatableEffect_Patch
    {
        [HarmonyPostfix]
        public static void Notify_EquipmentAddedPostfix(Pawn_EquipmentTracker __instance)
        {
            if (__instance == null || __instance.pawn == null || __instance.pawn.Map == null || __instance.AllEquipmentListForReading.NullOrEmpty())
            {
                return;
            }
            foreach (var item in __instance.AllEquipmentListForReading)
            {
                ThingWithComps eq = item as ThingWithComps;
                if (eq != null) eq.BroadcastCompSignal(CompAlwaysActivatableEffect.ActivateSignal);
            }
        }
    }
}
