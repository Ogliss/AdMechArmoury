using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace AdeptusMechanicus.HarmonyInstance
{
    [StaticConstructorOnStartup]
    public static class AlienRacesMain
    {
        static AlienRacesMain()
        {
            var harmony = new Harmony("com.ogliss.rimworld.mod.AdeptusMechanicus.AlienRaces");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            if (Prefs.DevMode) Log.Message(string.Format("Adeptus Mechanicus: AlienRaces successfully completed {0} harmony patches.", harmony.GetPatchedMethods().Select(new Func<MethodBase, Patches>(Harmony.GetPatchInfo)).SelectMany((Patches p) => p.Prefixes.Concat(p.Postfixes).Concat(p.Transpilers)).Count((Patch p) => p.owner.Contains(harmony.Id))), false);
        }

    }
}
