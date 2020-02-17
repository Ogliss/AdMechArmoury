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
using System.Reflection;
using AdeptusMechanicus.ExtensionMethods;
using UnityEngine;
using AdeptusMechanicus.settings;

namespace AdeptusMechanicus.Harmony
{
    [HarmonyPatch(typeof(VerbTracker), "GetVerbsCommands")]
    public static class AM_VerbTracker_GetVerbsCommands_Patch
    {
        [HarmonyPostfix]
        public static void GetVerbsCommands_Postfix(ref VerbTracker __instance,ref IEnumerable<Command> __result)
        {

        }
    }
    
}