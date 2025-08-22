using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AdeptusMechanicus.settings;
using HarmonyLib;
using RimWorld;
using Verse;

namespace AdeptusMechanicus.HarmonyInstance
{
    [StaticConstructorOnStartup]
    public static class CEMain
    {
        static CEMain()
        {
            var harmony = new Harmony("com.ogliss.rimworld.mod.AdeptusMechanicus.CE");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            if (Prefs.DevMode) Log.Message(string.Format("Adeptus Mechanicus: Detected Combat Extended, successfully completed {0} additional harmony patches.", harmony.GetPatchedMethods().Select(new Func<MethodBase, Patches>(Harmony.GetPatchInfo)).SelectMany((Patches p) => p.Prefixes.Concat(p.Postfixes).Concat(p.Transpilers)).Count((Patch p) => p.owner.Contains(harmony.Id))));
        }

    }
}
