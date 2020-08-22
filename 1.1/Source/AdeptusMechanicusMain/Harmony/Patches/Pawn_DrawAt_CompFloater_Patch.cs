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
    [HarmonyPatch(typeof(Pawn), "DrawAt")]
    public static class Pawn_DrawAt_CompFloater_Patch
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
