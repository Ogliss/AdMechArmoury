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
        public static MapComponent_DeepStrike DeepStrike(this Map map)
        {
            return map.GetComponent<MapComponent_DeepStrike>();
        }

        public static MapComponent_Infiltrate Infiltrate(this Map map)
        {
            return map.GetComponent<MapComponent_Infiltrate>();
        }

    }
}
