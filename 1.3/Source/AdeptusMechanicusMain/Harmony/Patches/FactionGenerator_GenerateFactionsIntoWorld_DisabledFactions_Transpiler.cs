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

namespace AdeptusMechanicus.HarmonyInstance
{
    
    [HarmonyPatch(typeof(FactionGenerator), "GenerateFactionsIntoWorld")]
    public static class FactionGenerator_GenerateFactionsIntoWorld_DisabledFactions_Transpiler
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            instructionsList[2].operand = AccessTools.Method(typeof(FactionGenerator_GenerateFactionsIntoWorld_DisabledFactions_Transpiler), "FactionsToGenerate", null, null);
 
        //   bool AstartesHook = false;

            for (int i = 0; i < instructionsList.Count; i++)
            {
                CodeInstruction instruction = instructionsList[i];
                /*
                // callvirt instance void RimWorld.FactionManager::Add(class RimWorld.Faction)
                if (instruction.opcode == OpCodes.Ldloc_0 && instructionsList[i-1].OperandIs(typeof(FactionManager).GetMethod("Add")) && !AstartesHook)
                {
                //    Log.Message(i + " opcode: " + instruction.opcode + " operand: " + instruction.operand + " Labels: " + instruction.ExtractLabels());
                    AstartesHook = true;
                    yield return new CodeInstruction(opcode: OpCodes.Ldloc_0);
                    yield return new CodeInstruction(opcode: OpCodes.Call, operand: typeof(FactionGenerator_GenerateFactionsIntoWorld_DisabledFactions_Transpiler).GetMethod("AstartesFactionsToGenerate"));
                }
                */
                yield return instruction;
            }
        //    return instructionsList;
        }

        public static IEnumerable<FactionDef> FactionsToGenerate()
        {
            foreach (FactionDef factionDef in DefDatabase<FactionDef>.AllDefs)
            {
                if (!factionDef.isPlayer)
                {
                    string defName = factionDef.defName;
                    if (factionDef.defName.Contains("OG_Astartes_"))
                    {
                        continue;
                    }
                    if (factionDef.defName.Contains("OG_Mechanicus_"))
                    {
                        if (!AMAMod.settings.AllowAdeptusMechanicus)
                        {
                            continue;
                        }
                    }
                    if (factionDef.defName.Contains("OG_Militarum_"))
                    {
                        if (!AMAMod.settings.AllowAdeptusMilitarum)
                        {
                            continue;
                        }
                    }
                    if (factionDef.defName.Contains("OG_Sororitas_"))
                    {
                        if (!AMAMod.settings.AllowAdeptusSororitas)
                        {
                            continue;
                        }
                    }
                    if (factionDef.defName.Contains("OG_Chaos_"))
                    {
                        if (factionDef.defName.Contains("Deamon"))
                        {
                            if (!AMAMod.settings.AllowChaosDeamons)
                            {
                                continue;
                            }
                        }
                        if (factionDef.defName.Contains("Marine"))
                        {
                            if (!AMAMod.settings.AllowChaosMarine)
                            {
                                continue;
                            }
                        }
                        if (factionDef.defName.Contains("Guard"))
                        {
                            if (!AMAMod.settings.AllowChaosGuard)
                            {
                                continue;
                            }
                        }
                        if (factionDef.defName.Contains("Mechanicus"))
                        {
                            if (!AMAMod.settings.AllowChaosMechanicus)
                            {
                                continue;
                            }
                        }
                    }

                    if (factionDef.defName.Contains("OG_Eldar_"))
                    {
                        if (factionDef.defName.Contains("Craftworld"))
                        {
                            if (!AMAMod.settings.AllowEldarCraftworld)
                            {
                                continue;
                            }
                        }
                        if (factionDef.defName.Contains("Exodite"))
                        {
                            if (!AMAMod.settings.AllowEldarExodite)
                            {
                                continue;
                            }
                        }
                        if (factionDef.defName.Contains("Harlequin"))
                        {
                            if (!AMAMod.settings.AllowEldarHarlequinn)
                            {
                                continue;
                            }
                        }
                    }
                    if (factionDef.defName.Contains("OG_DarkEldar_"))
                    {
                        if (!AMAMod.settings.AllowDarkEldar)
                        {
                            continue;
                        }
                    }
                    if (factionDef.defName.Contains("OG_Kroot_"))
                    {
                        if (!AMAMod.settings.AllowKroot)
                        {
                            continue;
                        }
                    }
                    if (factionDef.defName.Contains("OG_Tau_"))
                    {
                        if (!AMAMod.settings.AllowTau)
                        {
                            continue;
                        }
                    }

                    if (factionDef.defName.Contains("OG_Necron_"))
                    {
                        if (!AMAMod.settings.AllowNecron)
                        {
                            continue;
                        }
                    }
                    if (factionDef.defName.Contains("OG_Ork_Tek_"))
                    {
                        if (!AMAMod.settings.AllowOrkTek)
                        {
                            continue;
                        }
                    }
                    if (factionDef.defName.Contains("OG_Ork_Feral_"))
                    {
                        if (!AMAMod.settings.AllowOrkFeral)
                        {
                            continue;
                        }
                    }
                    if (factionDef.defName.Contains("OG_Ork_Hulk") || factionDef.defName.Contains("OG_Ork_Rok"))
                    {
                        if (!AMAMod.settings.AllowOrkRok || !AMAMod.settings.AllowOrkTek)
                        {
                            continue;
                        }
                    }

                    if (factionDef.defName.Contains("OG_Tyranid_") || factionDef.defName.Contains("OG_Genestealer_Cult"))
                    {
                        if (!AMAMod.settings.AllowTyranid)
                        {
                            continue;
                        }
                    }
                    if (factionDef.defName.Contains("OG_Vespid_") || factionDef.defName.Contains("OG_Vespid_Feral_"))
                    {
                        if (!AMAMod.settings.AllowVespid)
                        {
                            continue;
                        }
                    }
                //    Log.Message("Spawning Faction of Def " + factionDef.defName);
                }
                yield return factionDef;
            }
            yield break;
        }

        public static int AstartesFactionsToGenerate(int num)
        {


        //    Log.Message("AstartesFactionsToGenerate");
            //    Log.Message("number of factions: "+num);
            return num;
        }

    }
    
}
