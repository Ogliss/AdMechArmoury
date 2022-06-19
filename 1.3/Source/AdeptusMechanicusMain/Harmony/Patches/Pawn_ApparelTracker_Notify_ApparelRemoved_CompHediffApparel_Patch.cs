using RimWorld;
using HarmonyLib;
using Verse;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Pawn_ApparelTracker), "Notify_ApparelRemoved")]
    public static class Pawn_ApparelTracker_Notify_ApparelRemoved_CompHediffApparel_Patch
    {
        [HarmonyPrefix, HarmonyPriority(Priority.First)]
        public static void Prefix(Pawn_ApparelTracker __instance, Apparel apparel)
        {
            if (__instance.pawn is Pawn pawn)
            {
                apparel.BroadcastCompSignal(CompHediffApparel.RemoveHediffsFromPawnSignal);
                if (apparel.def.GetModExtensionFast<ApparelRestrictionDefExtension>() is ApparelRestrictionDefExtension ext)
                {
                    if (!ext.BodytypeDefs.NullOrEmpty() && ext.forcedBodytype)
                    {
                        OriginalBodyTracker tracker = Find.World.GetComponent<OriginalBodyTracker>();
                        if (tracker.ModifiedBody(pawn, out BodyTypeDef original))
                        {
                            AdeptusHarmonyPatches.ChangeBodyType(pawn, original);
                            tracker.originalBody.Remove(pawn);
                        }
                    }
                }
            }
        }
    }
    
}
