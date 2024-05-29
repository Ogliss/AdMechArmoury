using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus.ExtensionMethods
{
    public static class FleshTypeDefExtensions
    {
        private static List<FleshTypeDef> constructFleshTypes;
        public static bool isConstruct(this FleshTypeDef def)
        {
            if (constructFleshTypes == null)
            {
                constructFleshTypes = DefDatabase<FleshTypeDef>.AllDefs.Where(x => x.defName.StartsWith("OG_Flesh_Construct")).ToList();
            //    Log.Message($"populated constructFleshTypes {constructFleshTypes.Count}");
            }
            foreach (var item in constructFleshTypes)
            {
                if (def == item)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
