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
using UnityEngine;
using System.Reflection;

namespace AdeptusMechanicus.HarmonyInstance
{
    
    [HarmonyPatch(typeof(NeurotrainerDefGenerator), "ImpliedThingDefs")]
    public static class AMA_NeurotrainerDefGenerator_ImpliedThingDefs_Filter_Patch
    {
        public static IEnumerable<ThingDef> Postfix(IEnumerable<ThingDef> list)
        {
            foreach (ThingDef item in list)
            {
                CompProperties_Neurotrainer compProperties = item.GetCompProperties<CompProperties_Neurotrainer>();
                if (compProperties.ability!=null)
                {
                    if (compProperties.ability.GetType() != typeof(AdeptusMechanicus.EquipmentAbilityDef))
                    {
                        yield return item;
                    }
                }
                else yield return item;
            }
            yield break;
        }
    }
    

}
