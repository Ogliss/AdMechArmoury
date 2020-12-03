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
using System.Reflection.Emit;
using UnityEngine;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(ArmorUtility), "GetPostArmorDamage")]
    public static class ArmorUtility_GetPostArmorDamage_InvunerableSave_Transpiler
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions); 
            bool seen = true;
            int first = 0;
            for (int i = 0; i < instructionsList.Count; i++)
            {
                CodeInstruction inst = instructionsList[i];
                if (i > 1)
                {
                    CodeInstruction prev = instructionsList[i - 1];
                    if (i < instructionsList.Count-4)
                    {
                        int extra = 4;
                        CodeInstruction next = instructions.ToList()[i + extra];
                        if (next.OperandIs(AccessTools.Method(type: typeof(ApparelProperties), name: nameof(ApparelProperties.CoversBodyPart), parameters: new[] { typeof(BodyPartRecord) })) && inst.opcode == OpCodes.Ldloc_S && ((LocalBuilder)inst.operand).LocalIndex == 5 && seen)
                        {
                            first = i;
                            /*
                            Log.Message("Current: " + inst.operand.GetType().FullName + " : " + inst.operand);
                            Log.Message("+ " + extra + ": " + next.operand.GetType().FullName + " : " + next.operand.ToString() + " +" + (i - first));
                            */
                            yield return new CodeInstruction(opcode: OpCodes.Ldarga_S, operand: 1);
                            yield return new CodeInstruction(opcode: OpCodes.Ldarga_S, operand: 2);
                            yield return new CodeInstruction(opcode: OpCodes.Ldloc_S, operand: 5);
                            yield return new CodeInstruction(opcode: OpCodes.Ldarg_S, operand: 4);
                            yield return new CodeInstruction(opcode: OpCodes.Ldarg_0);
                            /*
                            */
                            yield return new CodeInstruction(opcode: OpCodes.Call, operand: typeof(ArmorUtility_GetPostArmorDamage_InvunerableSave_Transpiler).GetMethod("ApplyInvSave"));

                            //    yield return new CodeInstruction(opcode: OpCodes.Call, operand: typeof(ArmorUtility_GetPostArmorDamage_Name_Transpiler).GetMethod("ApplyInvSave2"));
                            seen = false;
                        }
                    }
                }
                yield return inst;
            }
        }
        public static void ApplyInvSave(ref float damAmount, ref float armorPenetration, Thing armorThing, ref DamageDef damageDef, Pawn pawn)
        {
            float ir = armorThing.GetStatValue(StatDef.Named("OG_ArmorRating_InvunerableSave"), true);
            if (ir>0)
            {
                float ar = armorThing.GetStatValue(damageDef.armorCategory.armorRatingStat, true);
                float ap = armorPenetration;
                if (armorPenetration > ar)
                {
                    ap = Mathf.Max(armorPenetration - ir, 0f);
                }
                float value = Rand.Value;
                float num2 = ap * 0.5f;
                if (value < num2)
                {
                    damAmount = 0f;
                    if (Prefs.DevMode) Log.Message("Damage negated by Inv save");
                    return;
                }
                if (value < ap)
                {
                    damAmount = (float)GenMath.RoundRandom(damAmount / 2f);
                    if (Prefs.DevMode) Log.Message("Damage halved by Inv save");
                }
            }
        }
    }
    
}
