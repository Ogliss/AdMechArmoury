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
    [HarmonyPatch(typeof(IncidentWorker_RaidEnemy), "TryExecuteWorker")]
    public static class IncidentWorker_RaidEnemy_TryExecute_RaidAlert_Patch
    {
        [HarmonyPostfix]
        public static void RaidAlert(bool __result, IncidentParms parms)
        {
            if (__result && parms != null)
            {
                if (parms.target is Map map && (parms.target as Map).IsPlayerHome)
                {
                    if (parms.faction != Faction.OfPlayer && parms.faction!=null)
                    {
                        if (parms.faction.def.modExtensions != null)
                        {
                            FactionDefRaidSoundExtension raidsound = parms.faction.def.GetModExtensionFast<FactionDefRaidSoundExtension>();
                            if (raidsound != null)
                            {
                                if (raidsound.raidSound != null)
                                {
                                    raidsound.raidSound.PlayOneShotOnCamera();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
