using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Harmony;
using Verse.Sound;
using UnityEngine;
using System.Reflection;

namespace AdeptusMechanicus.Harmony
{
 //   [HarmonyPatch(typeof(Graphic_RandomRotated), "DrawWorker")]
    public static class AM_Graphic_RandomRotated_DrawWorker_Debuff_Patch
    {
        public static FieldInfo subgraphics = typeof(Graphic_Random).GetField("subGraphics", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
        public static FieldInfo subgraphic = typeof(Graphic_RandomRotated).GetField("subGraphic", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
        /*
        [HarmonyPostfix]
        public static void Graphic_RandomRotated_DrawWorker_Postfix(ref Graphic_RandomRotated __instance, Graphic ___subGraphic, Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
        {
            if (thing!=null)
            {
                ThingWithCompsRandomGraphic thingrg = thing as ThingWithCompsRandomGraphic;
                if (thingrg != null)
                {
                    if (___subGraphic != null)
                    {
                        Traverse traverse = Traverse.Create(___subGraphic);
                        Graphic[] subGraphics = (Graphic[])AM_Graphic_RandomRotated_DrawWorker_Debuff_Patch.subgraphics.GetValue(___subGraphic);
                        if (!subGraphics.NullOrEmpty())
                        {
                            Log.Message(string.Format("subGraphics: {0}", subGraphics.Count()));
                            if (thingrg.gfxint==-1)
                            {
                                thingrg.gfxint = Rand.Range(0, subGraphics.Count());
                                Log.Message(string.Format("randomized gfxint: {0}", thingrg.gfxint));
                                thingrg._graphic = subGraphics[thingrg.gfxint];
                                Log.Message(string.Format("Set _graphic: {0}", thingrg._graphic));
                            }
                        }
                        else
                        {
                            Log.Warning("subGraphics = NullOrEmpty");
                        }
                    }
                }
            }
        }
        */
    }
}
