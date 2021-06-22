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

        public static bool isConstruct(this FleshTypeDef def)
        {
            return def.defName.Contains("OG_Flesh_Construct");
        }
    }
}
