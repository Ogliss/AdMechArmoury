using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse.AI;
using Verse.AI.Group;
using AdeptusMechanicus;
using AdeptusMechanicus.ExtensionMethods;
using Verse.Sound;
using System;
using RimWorld;
using Verse;
using HarmonyLib;

namespace AdeptusMechanicus.HarmonyInstance
{
    
    [HarmonyPatch(typeof(GenSpawn), "SpawningWipes")]
    public static class GenSpawn_SpawningWipes_MountedWeapon_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(BuildableDef newEntDef, BuildableDef oldEntDef, ref bool __result)
        {
            if (__result && newEntDef is ThingDef newDef && oldEntDef is ThingDef oldDef)
            {
                if (newDef.tradeTags.NullOrEmpty() || !newDef.tradeTags.Contains("OG_Mounted_Weapon"))
                {
                    return;
                }
                if (oldDef.graphicData != null)
                {
                    LinkFlags linkFlags = oldDef.graphicData.linkFlags;
                    if (linkFlags.HasFlag(LinkFlags.Barricades) || linkFlags.HasFlag(LinkFlags.Sandbags))
                    {
                        Log.Message("SpawningWipes linkFlags " + oldDef);
                        __result = false;
                        return;
                    }
                }
            }
        }
    }
    
}
