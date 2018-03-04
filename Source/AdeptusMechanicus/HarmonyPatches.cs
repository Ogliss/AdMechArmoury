using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        //For alternating fire on some weapons
        public static Dictionary<Thing, int> AlternatingFireTracker = new Dictionary<Thing, int>();

    }
}