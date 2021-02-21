using RimWorld;
using Verse;
using HarmonyLib;
using UnityEngine;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Pawn), "DrawAt")]
    public static class Pawn_DrawAt_CompFloater_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn __instance, Vector3 drawLoc)
        {
            CompFloating floater = __instance.TryGetCompFast<CompFloating>();
            if (floater != null && !__instance.Dead && !__instance.Downed && __instance.Awake())
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
