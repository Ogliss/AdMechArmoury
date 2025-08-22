using System.Linq;
using Verse;
using HarmonyLib;
using RimWorld;
using AlienRace;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(PawnGenerator), "GenerateSkills")]
    public static class PawnGenerator_GenerateSkills_BackstoryPassion_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn pawn)
        {
            foreach (RimWorld.BackstoryDef backstory in pawn.story.AllBackstories)
            {
                if (backstory is BackstoryDef Backstory)
                {
                    IEnumerable<SkillGain> passions = Backstory.passions;
                    if (AdeptusIntergrationUtility.enabled_AlienRaces)
                    {
                        passions = DoHARStuff(passions, pawn);
                    }
                    foreach (SkillGain passion in passions)
                    {
                        pawn.skills.GetSkill(passion.skill).passion = (Passion)passion.amount;
                    }
                }
            }
        }

        public static IEnumerable<SkillGain> DoHARStuff(IEnumerable<SkillGain> passions, Pawn pawn)
        {
            if (pawn.def is ThingDef_AlienRace alienProps)
            {
                passions = passions.Concat(alienProps.alienRace.generalSettings.passions);
            }
            return passions;
        }
    }

    [HarmonyPatch(typeof(PawnBioAndNameGenerator), "FillBackstorySlotShuffled")]
    public static class PawnBioAndNameGenerator_FillBackstorySlotShuffled_LinkedBackstory_Patch
    {
        [HarmonyPostfix]
        public static bool Prefix(Pawn pawn, BackstorySlot slot)
        {
            AdeptusHarmonyPatches.bioReference = null;
            if (slot == BackstorySlot.Adulthood)
            {
                BackstoryDef absd = pawn.story.Childhood as BackstoryDef;
                if (absd != null && absd.linkedBackstory != null)
                {
                    pawn.story.Adulthood = absd.linkedBackstory;
                    return false;
                }
            }
            return true;
        }
        /*
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo backstoryDatabaseInfo = AccessTools.PropertyGetter(typeof(DefDatabase<RimWorld.BackstoryDef>), "AllDefs");
            bool done = false && AdeptusIntergrationUtility.enabled_AlienRaces;
            List<CodeInstruction> instructionList = instructions.ToList<CodeInstruction>();
            int num;
            for (int i = 0; i < instructionList.Count; i = num + 1)
            {
                CodeInstruction codeInstruction = instructionList[i];
                yield return codeInstruction;
                if (!done && i > 1 && codeInstruction.Calls(backstoryDatabaseInfo))
                {
                    done = true;
                    yield return new CodeInstruction(OpCodes.Ldarg_0, null);
                    yield return new CodeInstruction(OpCodes.Ldarg_1, null);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PawnBioAndNameGenerator_FillBackstorySlotShuffled_LinkedBackstory_Patch), "FilterBackstories", null, null));
                }
                codeInstruction = null;
                num = i;
            }
            yield break;
        }

        public static IEnumerable<RimWorld.BackstoryDef> FilterBackstories(IEnumerable<RimWorld.BackstoryDef> backstories, Pawn pawn, BackstorySlot slot)
        {
            return backstories.Where(delegate (RimWorld.BackstoryDef bs)
            {
                BackstoryDef abs = bs as BackstoryDef;
                return abs == null || (abs.Approved(pawn) && (slot != BackstorySlot.Adulthood || abs.linkedBackstory == null || pawn.story.Childhood == abs.linkedBackstory));
            });
        }
        */
    }

}
