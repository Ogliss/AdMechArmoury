using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using Verse.Sound;
using AdeptusMechanicus;
using AdeptusMechanicus.ExtensionMethods;
using UnityEngine;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Pawn), "get_DrawPos")]
    public static class AM_Pawn_get_DrawPos_CompFloater_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn __instance, ref Vector3 __result)
        {
            CompFloating floater = __instance.TryGetComp<CompFloating>();
            if (floater!=null)
            {
            //    Log.Message("get_DrawPos patch for floater " + __instance);
                if (floater.Props.useZOffset)
                {
                //    Log.Message("get_DrawPos modified by " + floater.Props.zOffset + " for floater " + __instance);
                    __result.z += floater.Props.zOffset;
                }
            }

        }
    }
    [HarmonyPatch(typeof(Pawn), "DrawAt")]
    public static class AM_Pawn_DrawAt_CompFloater_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn __instance, Vector3 drawLoc)
        {
            CompFloating floater = __instance.TryGetComp<CompFloating>();
            if (floater!=null)
            {
            //    Log.Message("DrawAt patch for floater " + __instance);
                if (floater.Props.useShadow)
                {
                //    Log.Message("DrawAt Draw Shadow for floater " + __instance);
                    Vector3 vector = drawLoc;
                    if (floater.Props.useZOffset)
                    {
                        vector.z -= floater.Props.zOffset;
                    }
                    vector.z -= floater.Props.zOffsetShadow;
                    floater.DrawDropSpotShadow(vector);
                }
            }

        }
    }
    
}
