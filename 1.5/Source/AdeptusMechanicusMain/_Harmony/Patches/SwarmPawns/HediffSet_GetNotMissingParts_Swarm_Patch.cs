using System.Collections.Generic;
using System.Linq;
using Verse;
using HarmonyLib;

namespace AdeptusMechanicus
{
    [HarmonyPatch(typeof(HediffSet), "GetNotMissingParts")]
    public static class HediffSet_GetNotMissingParts_Swarm_Patch
    {
        [HarmonyPostfix]
        public static IEnumerable<BodyPartRecord> Postfix(IEnumerable<BodyPartRecord> __result, ref HediffSet __instance)
        {
            if (__instance.pawn.RaceProps.body.corePart.def == AdeptusBodyPartDefOf.OG_SwarmCore)
            {
                return __result.Where(x => x != null && x.def != AdeptusBodyPartDefOf.OG_SwarmCore);
            }
            return __result;
        }
    }

}
