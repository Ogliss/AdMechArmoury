using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse.AI;
using Verse.AI.Group;
using AdeptusMechanicus;
using AdeptusMechanicus.ExtensionMethods;
using Verse.Sound;
using System;
using Verse;
using HarmonyLib;
using UnityEngine;
using AdeptusMechanicus.settings;
using RimWorld;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(UIRoot_Entry), "Init")]
    public static class UIRoot_Entry_Init_HereticalModifications_Patch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            AdeptusDialogMaker.CreateWarningDialogIfNecessary();
        }
    }

//    [HarmonyPatch(typeof(PawnRelationWorker_Parent), "ResolveMyName")]
    public static class PawnRelationWorker_Parent_ResolveMyName_Name_Patch
    {
    //    [HarmonyPrefix]
        public static bool Prefix(ref PawnGenerationRequest request, Pawn generatedChild)
        {

            if (request.FixedLastName != null)
            {
                return true;
            }
            Log.Message("E 0 ");
            if (ChildRelationUtility.ChildWantsNameOfAnyParent(generatedChild))
            {
                Log.Message("E 0 1 1");
                bool flag = Rand.Value < 0.5f || generatedChild.GetMother() == null;
                Log.Message("E 0 1 2");
                if (generatedChild.GetFather() == null)
                {
                    flag = false;
                }
                Log.Message("E 0 1 3");
                if (flag)
                {
                    Log.Message($"E 0 1 3 1 generatedChild.GetFather().Name: {generatedChild.GetFather().Name} {generatedChild.GetFather().Name.GetType()}");
                    request.SetFixedLastName(((NameTriple)generatedChild.GetFather().Name).Last);

                    Log.Message("E 0 1 3 2");
                    return false;
                }
                Log.Message($"E 0 1 4 generatedChild.GetMother().Name: {generatedChild.GetMother().Name} {generatedChild.GetMother().Name.GetType()}");
                request.SetFixedLastName(((NameTriple)generatedChild.GetMother().Name).Last);
                Log.Message("E 0 1 5");
            }
            return false;
        }

        /*
        [HarmonyPostfix]
        public static void Postfix(ref PawnGenerationRequest request, Pawn generatedChild)
        {

        }
        */
    }
}
