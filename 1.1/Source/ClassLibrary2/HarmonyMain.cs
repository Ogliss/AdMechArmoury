using RimWorld;
using Verse;
using Harmony;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace JurassicRimworld
{
    [StaticConstructorOnStartup]
    class Main
    {
        static Main()
        {
            var harmony = HarmonyInstance.Create("com.ogliss.rimworld.mod.AdeptusMechanicus");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
    
}