using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus.ExtensionMethods
{
    public static class MapExtensions
    {
        public static MapComponent_Reserves Reserves(this Map map)
        {
            return map.GetComponent<MapComponent_Reserves>();
        }
    }
}
