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

namespace AdeptusMechanicus.HarmonyInstance
{

    [HarmonyPatch(typeof(Pawn_ApparelTracker), "Notify_ApparelAdded")]
    public static class Pawn_ApparelTracker_Notify_ApparelAdded_CompHediffApparel_Patch
    {
        [HarmonyPrefix, HarmonyPriority(Priority.First)]
        public static void Prefix(Pawn_ApparelTracker __instance, Apparel apparel)
        {
            if (apparel.Wearer is Pawn pawn)
            {
                apparel.BroadcastCompSignal(CompHediffApparel.AddHediffsToPawnSignal);
                if (apparel.def.GetModExtensionFast<ApparelRestrictionDefExtension>() is ApparelRestrictionDefExtension ext && pawn.story != null)
                {
                    if (!ext.BodytypeDefs.NullOrEmpty() && ext.forcedBodytype && !ext.BodytypeDefs.Contains(pawn.story.bodyType))
                    {
                        OriginalBodyTracker tracker = Find.World.GetComponent<OriginalBodyTracker>();
                        if (!tracker.ModifiedBody(pawn, out BodyTypeDef original))
                        {
                            tracker.originalBody.SetOrAdd(apparel.Wearer, apparel.Wearer.story.bodyType);
                            HarmonyPatches.ChangeBodyType(pawn, ext.BodytypeDefs.RandomElement());
                        }
                    }
                }
            }
        }
    }
    
}
