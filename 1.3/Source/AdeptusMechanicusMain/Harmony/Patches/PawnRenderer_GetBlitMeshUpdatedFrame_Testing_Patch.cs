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
using UnityEngine;
using System.Reflection;

namespace AdeptusMechanicus.HarmonyInstance
{
    /*
    [HarmonyPatch(typeof(PawnRenderer), "GetBlitMeshUpdatedFrame")]
    public static class PawnRenderer_GetBlitMeshUpdatedFrame_Testing_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(PawnRenderer __instance, ref PawnTextureAtlasFrameSet frameSet, Rot4 rotation, PawnDrawMode drawMode, ref Mesh __result)
        {
			int index = frameSet.GetIndex(rotation, drawMode);
			if (frameSet.isDirty[index])
			{

                Find.PawnCacheCamera.rect = frameSet.uvRects[index];
				Find.PawnCacheRenderer.RenderPawn(__instance.pawn, frameSet.atlas, Vector3.zero, 2f, 0f, rotation, true, drawMode == PawnDrawMode.BodyAndHead, true, true, false, default(Vector3), null, false);
				Find.PawnCacheCamera.rect = new Rect(0f, 0f, 1f, 1f);
				frameSet.isDirty[index] = false;
			}
            __result = frameSet.meshes[index];
            return false;
        }
    }


    //    [HarmonyPatch(typeof(PawnTextureAtlas), MethodType.StaticConstructor)]
    public static class PawnTextureAtlas_Constructor_Name_Patch
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo target = AccessTools.Method(typeof(TextureAtlasHelper), "CreateMeshForUV");
            MethodInfo patch = AccessTools.Method(typeof(PawnTextureAtlas_Constructor_Name_Patch), nameof(PawnTextureAtlas_Constructor_Name_Patch.patched));
            var instructionsList = new List<CodeInstruction>(instructions);
        //    instructionsList = new List<CodeInstruction>(instructions.MethodReplacer(target, patch));
            for (int i = 0; i < instructionsList.Count; i++)
            {
                CodeInstruction code = instructionsList[i];
                
                if (code.opcode == OpCodes.Ldc_R4 && code.OperandIs(0.0625f))
                {
                    code.operand = 0.10f;
                }
                
                yield return code;
            }
        }

        public static Mesh patched(Rect uv, float scale = 1f)
        {
            return TextureAtlasHelper.CreateMeshForUV(uv, scale * 1.5f);
        }
    }
*/
}
