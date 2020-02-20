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

namespace AdeptusMechanicus.HarmonyInstance
{
    /*
    [HarmonyPatch(typeof(PawnGroupMakerUtility), "GeneratePawns")]
    public static class AM_PawnGroupMakerUtility_GeneratePawns__Patch
    {
        [HarmonyPostfix]
        public static void GeneratePawns_DeepStrike(PawnGroupMakerUtility __instance ,PawnGroupMakerParms parms, bool warnOnZeroResults, ref IEnumerable<Pawn> __result)
        {
            if (__result != null)
            {
                List<Pawn> DeepStrikers = new List<Pawn>();
                List<Pawn> Infiltrators = new List<Pawn>();
                List<Pawn> MormalDeployment = new List<Pawn>();
                foreach (Pawn p in __result)
                {
                    bool flagDeepStrike = p.kindDef.HasModExtension<DeepStrikeExtension>();
                    bool flagInfiltrator = p.kindDef.HasModExtension<InfiltratorExtension>();
                    bool flagStandard = !flagDeepStrike && !flagInfiltrator;
                    if (!flagStandard)
                    {
                        if (flagDeepStrike)
                        {
                            DeepStrikers.Add(p);
                        }
                        if (flagInfiltrator)
                        {
                            Infiltrators.Add(p);
                        }
                    }
                    else
                    {
                        MormalDeployment.Add(p);
                    }
                    string Deployment = flagStandard ? "Standard" : (flagDeepStrike ? "Deep Strike" : "Infiltration" );
                    Log.Message(string.Format("pawn: {0}, Deployment: {1}",p.LabelShortCap, Deployment));
                }

                if (MormalDeployment.NullOrEmpty())
                {
                    Log.Message(string.Format("MormalDeployment.NullOrEmpty()"));
                }
                else
                {
                    __result = MormalDeployment;
                }
            }
        }
    }
    */
}
