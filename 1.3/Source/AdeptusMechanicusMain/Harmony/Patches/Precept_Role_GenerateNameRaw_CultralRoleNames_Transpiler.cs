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
using System.Reflection;
using System.Reflection.Emit;
using Verse.Grammar;

namespace AdeptusMechanicus.HarmonyInstance
{

    [HarmonyPatch(typeof(Precept_Role), "GenerateNameRaw")]
    public static class Precept_Role_GenerateNameRaw_CultralRoleNames_Transpiler
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            FieldInfo nameMaker = AccessTools.Field(typeof(RimWorld.PreceptDef), "nameMaker");
            FieldInfo preceptDef = AccessTools.Field(typeof(Precept_Role), "def");
            FieldInfo ideo = AccessTools.Field(typeof(Precept_Role), "ideo");
            for (int i = 0; i < instructionsList.Count; i++)
            {
                CodeInstruction inst = instructionsList[i];
                if (inst.OperandIs(nameMaker))
                {
                    yield return inst;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, preceptDef);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, ideo);
                    inst = new CodeInstruction(OpCodes.Call, typeof(Precept_Role_GenerateNameRaw_CultralRoleNames_Transpiler).GetMethod("CulturedRoleName"));

                }
                yield return inst;
            }

        }

        public static RulePackDef CulturedRoleName(RulePackDef original, PreceptDef role, Ideo ideo)
        {
            if (ideo.culture is CultureDef def && !def.rolesNames.NullOrEmpty())
            {
                //    Log.Message($"CultureDef: {def} found Role nameMaker: {original} rolesRenamers: {def.rolesNames.Count}");
                foreach (var item in def.rolesNames)
                {
                    if (item.role == role)
                    {
                        //   Log.Message($"RoleDef renamer: {item.role} RulePack used: {item.rulePack}");
                        return item.rulePack;
                    }
                }
            }
            return original;
        }
    }
    
    [HarmonyPatch(typeof(Precept), "AddIdeoRulesTo")]
    public static class Precept_AddIdeoRulesTo_CultralRoleNames_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Precept __instance, ref GrammarRequest request)
        {

            if (__instance.ideo.culture is CultureDef def && def.generalRules != null)
            {
                Log.Message($"CultureDef: {def} found GrammarRequest: {request.Rules.ToString()} generalRules: {def.generalRules.Rules.Count}");

                request.IncludesBare.Add(def.generalRules);
            }
        }
    }

}
