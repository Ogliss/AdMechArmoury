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
using AdeptusMechanicus.Abilities;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(NeurotrainerDefGenerator), "ImpliedThingDefs"),StaticConstructorOnStartup]
    public static class AM_NeurotrainerDefGenerator_ImpliedThingDefs_Patch
    {
        [HarmonyPrefix]
        public static void NeurotrainerDefGenerator_ImpliedThingDefs_Postfix(ref IEnumerable<ThingDef> __result)
        {
            IEnumerable<ThingDef> result = __result.Where(x=> !x.descriptionHyperlinks.Any(y=> y.def.GetType()==typeof(EquipmentAbilityDef)));
            Log.Message(string.Format("trying to remove {0} psytrainers",__result.Count()- result.Count()));
            __result = result;
        }
    }
    
}