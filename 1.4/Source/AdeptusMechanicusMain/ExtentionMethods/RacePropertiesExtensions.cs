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
            if (def == FleshTypeDefOf.Normal || def == FleshTypeDefOf.Insectoid) return false;
            if (constructFleshTypes == null)
            {
                constructFleshTypes = DefDatabase<FleshTypeDef>.AllDefs.Where(x => x.defName.StartsWith("OG_Flesh_Construct")).ToList();
            }
            if (!constructFleshTypes.NullOrEmpty())
            {
                for (int i = 0; i < constructFleshTypes.Count; i++)
                {
                    FleshTypeDef item = constructFleshTypes[i];
                    if (def == item)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
