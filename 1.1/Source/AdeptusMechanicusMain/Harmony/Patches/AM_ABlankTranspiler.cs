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
    /*
    [HarmonyPatch(typeof(ApparelUtility), "HasPartsToWear")]
    public static class AMAA_ApparelUtility_HasPartsToWear_Astartes_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            foreach (var instruction in instructionsList)
            {
                yield return instruction;
                if (instruction.operand as MethodInfo == typeof(CellRect).GetMethod("get_CenterCell"))
                {
                    yield return new CodeInstruction(OpCodes.Call, typeof(AvP_GenStep_ScatterRuinsSimple_ScatterAt_Patch).GetMethod("CenterCellValue"));
                }

            }
        }
        public static IntVec3 CenterCellValue(IntVec3 pos)
        {
            Map map = MapGenerator.mapBeingGenerated;
            MapComponent_HiveGrid hiveGrid = map.HiveGrid();
            hiveGrid.PotentialHiveLoclist.Add(new PotentialXenomorphHiveLocation(pos));
        //    Log.Message(string.Format("Ruin spawned: {0}, adding to Maps Potential Hive locations, Total: {1}", pos, hiveGrid.PotentialHiveLoclist.Count));
            
            return pos;
        }
    }
    
    }
    */
}
