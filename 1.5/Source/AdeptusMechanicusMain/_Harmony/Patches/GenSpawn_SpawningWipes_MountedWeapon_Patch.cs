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
        public static bool Postfix(bool __result, BuildableDef newEntDef, BuildableDef oldEntDef)
        {
            if (__result && newEntDef is ThingDef newDef && oldEntDef is ThingDef oldDef)
            {
                if ((!newDef.placeWorkers.NullOrEmpty() && newDef.placeWorkers.Contains(typeof(PlaceWorker_OnTopOfLowWalls))) && (!newDef.tradeTags.NullOrEmpty() && newDef.tradeTags.Contains("OG_Mounted_Weapon")))
                {
                    Log.Message(newDef.defName+" vs "+oldDef.defName);
                    if (oldDef.graphicData != null)
                    {
                        LinkFlags linkFlags = oldDef.graphicData.linkFlags;
                        if (linkFlags.HasFlag(LinkFlags.Barricades) || linkFlags.HasFlag(LinkFlags.Sandbags) || linkFlags.HasFlag(LinkFlags.Fences))
                        {
                            //    Log.Message("SpawningWipes linkFlags " + oldDef);
                            return false;
                        }
                    }
                }
            }
            return __result;
        }
    }
    
}
