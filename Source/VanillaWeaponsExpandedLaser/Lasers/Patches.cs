using RimWorld;
using Verse;
using Harmony;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace VanillaWeaponsExpandedLaser.Harmony
{
    [StaticConstructorOnStartup]
    class Main
    {
        static Main()
        {
            var harmony = HarmonyInstance.Create("com.ogliss.rimworld.mod.VanillaWeaponsExpandedLaser");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}