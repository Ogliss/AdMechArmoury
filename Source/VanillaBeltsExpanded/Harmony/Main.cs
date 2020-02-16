using RimWorld;
using Verse;
using Harmony;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace VanillaApparelExpandedBelts.Harmony
{
    [StaticConstructorOnStartup]
    class Main
    {
        static Main()
        {
            var harmony = HarmonyInstance.Create("com.ogliss.rimworld.mod.VanillaApparelExpandedBelts");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}