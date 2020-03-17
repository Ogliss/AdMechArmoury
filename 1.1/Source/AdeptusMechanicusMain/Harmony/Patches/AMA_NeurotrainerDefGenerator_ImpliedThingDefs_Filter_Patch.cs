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

namespace AdeptusMechanicus
{
    [HarmonyPatch(typeof(NeurotrainerDefGenerator), "ImpliedThingDefs")]
    public static class AMA_NeurotrainerDefGenerator_ImpliedThingDefs_Filter_Patch
    {
        [HarmonyPostfix]
        public static IEnumerable<ThingDef> Post_ImpliedThingDefs(IEnumerable<ThingDef> __result)
        {
            foreach (ThingDef item in __result)
            {
                CompProperties_Neurotrainer compProperties = item.GetCompProperties<CompProperties_Neurotrainer>();
                if (compProperties.ability.GetType() != typeof(EquipmentAbilityDef))
                {
                    yield return item;
                }
                else
                {
                    Log.Message(string.Format("skipping {0}", item.LabelCap));
                }
            }
        }
        //    public static FieldInfo graphic = typeof(Graphic).GetField("graphicInt", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
    }


}
