using System.Collections.Generic;
using System.Linq;
using Verse;
using HarmonyLib;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(VerbTracker), "GetVerbsCommands")]
    public class VerbTracker_GetVerbsCommands_CompToggleFireMode_Patch
    {
        private static void Postfix(VerbTracker __instance, ref IEnumerable<Command> __result)
        {
            var list = __result.ToList();
            CompEquippable compEquippable = __instance.directOwner as CompEquippable;
            var comp = compEquippable?.parent.TryGetCompFast<CompToggleFireMode>();
            if (comp != null)
            {
                list.RemoveAll(x => x is Command_VerbTarget verbTarget && verbTarget.verb != comp.ActiveVerb);
                __result = list;
            }
        }
    }
}