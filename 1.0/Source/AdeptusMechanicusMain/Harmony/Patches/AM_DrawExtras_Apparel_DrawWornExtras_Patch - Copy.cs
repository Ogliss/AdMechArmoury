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
using AdeptusMechanicus;
using UnityEngine;

namespace AdeptusMechanicus.Harmony
{
    [HarmonyPatch(typeof(Corpse), "SpawnSetup")]
    public static class AM_Corpse_SpawnSetup_Patch
    {
        [HarmonyPrefix]
        public static bool Corpse_SpawnSetup_Postfix(ref Corpse __instance)
        {
            
            if (__instance.Bugged && __instance.def.defName.Contains("Mechanicus"))
            {
                return false;
            }
            return true;
        }
    }
}
