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
using System.Reflection;
using AdeptusMechanicus.ExtensionMethods;
using UnityEngine;
using AdeptusMechanicus.settings;
/*

namespace AdeptusMechanicus.Harmony
{
    [HarmonyPatch(typeof(VerbTracker), "CreateVerbTargetCommand")]
    public static class AM_VerbTracker_CreateVerbTargetCommand_Patch
    {
        [HarmonyPostfix]
        public static void PrimaryVerb_Postfix(ref VerbTracker __instance,Thing ownerThing, Verb verb ,ref Command_VerbTarget __result)
        {

        }
    }
}
*/