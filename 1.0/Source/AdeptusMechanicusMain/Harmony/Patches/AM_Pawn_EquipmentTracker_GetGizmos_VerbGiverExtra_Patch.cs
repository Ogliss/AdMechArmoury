using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Harmony;
using Verse.Sound;

namespace AdeptusMechanicus.Harmony
{
    [HarmonyPatch(typeof(Pawn_EquipmentTracker), "GetGizmos")]
    public static class AM_Pawn_EquipmentTracker_GetGizmos_VerbGiverExtra_Patch
    {
        [HarmonyPostfix]
        public static void GetGizmosPostfix(Pawn_EquipmentTracker __instance, ref IEnumerable<Gizmo> __result)
        {
            for (int o = 0; o < __instance.pawn.health.hediffSet.hediffs.Count; o++)
            {
                HediffComp_VerbGiverExtra _VerbGiverExtra;
                if ((_VerbGiverExtra = __instance.pawn.health.hediffSet.hediffs[o].TryGetComp<HediffComp_VerbGiverExtra>()) != null)
                {
                    foreach (Command command in _VerbGiverExtra.GetVerbsCommands())
                    {
                        if (o != 0)
                        {
                            if (o != 1)
                            {
                                if (o == 2)
                                {
                                    command.hotKey = KeyBindingDefOf.Misc3;
                                }
                            }
                            else
                            {
                                command.hotKey = KeyBindingDefOf.Misc2;
                            }
                        }
                        else
                        {
                            command.hotKey = KeyBindingDefOf.Misc1;
                        }
                        __result.Add(command);
                    }
                }

            }
        }
    }

}
