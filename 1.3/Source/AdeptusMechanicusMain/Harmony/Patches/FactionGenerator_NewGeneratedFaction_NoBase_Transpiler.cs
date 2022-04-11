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
using AdeptusMechanicus.settings;
using System.Reflection.Emit;
using System.Reflection;

namespace AdeptusMechanicus.HarmonyInstance
{
    
    [HarmonyPatch(typeof(FactionGenerator), "NewGeneratedFaction")]
    public static class FactionGenerator_NewGeneratedFaction_NoBase_Transpiler
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            MethodInfo hidden =  AccessTools.TypeByName("RimWorld.Faction").GetMethod("get_Hidden");
            MethodInfo genbase = AccessTools.Method(typeof(FactionGenerator_NewGeneratedFaction_NoBase_Transpiler), "GenerateBase", null, null);
            

            for (int i = 0; i < instructionsList.Count; i++)
            {
                CodeInstruction current = instructionsList[i];
                
                if (i > 1)
                {
                    CodeInstruction previous = instructionsList[i - 1];

                    if (previous.OperandIs(hidden))
                    {
                        CodeInstruction clone = new CodeInstruction(current.opcode, current.operand);
                        yield return current;
                        yield return new CodeInstruction(OpCodes.Ldloc_1);
                        yield return new CodeInstruction(OpCodes.Call, genbase);
                        current = clone;
                        
                    }
                }

                yield return current;
            }
        //    return instructionsList;
        }

        public static bool GenerateBase(Faction faction)
        {
            bool result = faction.def.defName.StartsWith("OG_") && !faction.def.isPlayer && faction.def.settlementGenerationWeight == 0;
            if (result) Log.Message($"{faction} NOT allowed to spawn inital base");
            return result;
        }
    }
    
}
