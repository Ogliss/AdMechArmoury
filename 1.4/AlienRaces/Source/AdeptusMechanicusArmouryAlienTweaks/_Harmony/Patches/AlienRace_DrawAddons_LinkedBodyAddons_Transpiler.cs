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
using System.Reflection.Emit;
using AdeptusMechanicus.settings;
using AlienRace;
using System.Reflection;

namespace AdeptusMechanicus.HarmonyInstance
{

    //    [HarmonyPatch(typeof(AlienRace.HarmonyPatches), "DrawAddons")]
    public static class AlienRace_DrawAddons_LinkedBodyAddons_Transpiler
    {
        //    static FieldInfo alien = AccessTools.TypeByName("RimWorld.JobDriver_PlantWork").GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Instance).First();
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            bool drawOffsetPatched = false;

            FieldInfo alienCompField = AccessTools.TypeByName("AlienRace.HarmonyPatches").GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Instance).First(x => x.GetFields().Any(y => y.Name.Contains("alienComp"))).GetField("alienComp");

            for (int i = 0; i < instructionsList.Count; i++)
            {
                CodeInstruction instruction = instructionsList[i];
                //    //    Log.Message(i + " opcode: " + instruction.opcode + " operand: " + instruction.operand);
                if (i > 1 && i < instructionsList.Count)
                {
                    /*
                    if (!drawSizePatched)
                    {
                        if (instructionsList[index: i].opcode == OpCodes.Stfld && instructionsList[index: i].OperandIs(typeof(Graphic).GetField("drawSize")))
                        {
                            drawSizePatched = true;
                            if (alienCompField != null)
                            {
                            //    if (AMAMod.Dev) Log.Message("DrawSize At " + (i) + " opcode: " + instruction.opcode + " operand: " + instruction.operand);
                                //    yield return instruction;                                               // Vector2
                                yield return new CodeInstruction(opcode: OpCodes.Ldarg_0);              // bool
                                yield return new CodeInstruction(opcode: OpCodes.Ldloc_S, 4);           // Addon
                                yield return new CodeInstruction(opcode: OpCodes.Ldloc_0);              // Pawn
                                yield return new CodeInstruction(opcode: OpCodes.Ldfld, operand: alienCompField);           // AlienComp
                                yield return new CodeInstruction(opcode: OpCodes.Call, operand: typeof(AlienRace_DrawAddons_LinkedBodyAddons_Transpiler).GetMethod("DrawSize"));
                            }
                            else
                            {
                                Log.Warning("alienCompField == null, LinkedBodyAddon HeadAddon Scaling disabled");
                            }

                        }
                    }
                    */
                    /*
                    if (!drawOffsetPatched)
                    {
                        if (instruction.operand is LocalBuilder lb && lb.LocalIndex == 14 && instruction.opcode == OpCodes.Ldloc_S)
                        {
                            drawOffsetPatched = true;
                        //    if (AMAMod.Dev) Log.Message("DrawOffset At " + (i) + " opcode: " + instruction.opcode + " operand: " + instruction.operand);

                            yield return instruction;                                               // Vector3
                            yield return new CodeInstruction(opcode: OpCodes.Ldloc_S, 5);           // Addon
                            yield return new CodeInstruction(opcode: OpCodes.Ldarg_S, 5);              // Rotation
                            instruction = new CodeInstruction(opcode: OpCodes.Call, operand: typeof(AlienRace_DrawAddons_LinkedBodyAddons_Transpiler).GetMethod("DrawOffset"));

                        }
                    }
                    */
                    /*
                    if (!drawLocPatched)
                    {
                        if (instructionsList[index: i].opcode == OpCodes.Ldarg_1 && instructionsList[index: i+1].opcode != OpCodes.Ldarg_3)
                        {
                            yield return instruction;                                               // bool Portrait
                            drawLocPatched = true;
                            if (AMAMod.Dev) Log.Message("DrawPosition At " + (i) + " opcode: " + instruction.opcode + " operand: " + instruction.operand);

                            yield return new CodeInstruction(opcode: OpCodes.Ldarg_3);              // Pawn
                            yield return new CodeInstruction(opcode: OpCodes.Ldloc_S, 4);           // Addon
                            yield return new CodeInstruction(opcode: OpCodes.Ldarg_S, 4);              // Quaternion
                            yield return new CodeInstruction(opcode: OpCodes.Ldarg_S, 5);              // Rotation
                            instruction = new CodeInstruction(opcode: OpCodes.Call, operand: typeof(AlienRace_DrawAddons_LinkedBodyAddons_Transpiler).GetMethod("DrawPosition"));

                        }
                    }
                    */

                }

                yield return instruction;
            }

        }
        /*
        public static float DrawSize(float original, bool portrait, Pawn pawn, AlienRace.AlienPartGenerator.BodyAddon addon)
        {
            float result = original;
            LinkedBodyAddon linked = addon as LinkedBodyAddon;
            if (linked != null && linked.linkLifeStageDrawSize)
            {
                ThingDef_AlienRace thingDef_AlienRace = pawn.def as ThingDef_AlienRace;
                GraphicPaths paths = thingDef_AlienRace.alienRace.graphicPaths.GetCurrentGraphicPath(pawn.ageTracker.CurLifeStage);
                Vector2 v = linked.alignWithHead ? (portrait? paths.customPortraitHeadDrawSize : paths.customHeadDrawSize) : (portrait ? paths.customPortraitDrawSize : paths.customDrawSize);
                result *= (v.x + v.y) / 2f;
                //    Log.Message("DrawSize result: " + result);
            }

            return result;
        }
        */
        /*
        public static Vector2 DrawSize(Vector2 original, bool portrait,AlienRace.AlienPartGenerator.BodyAddon addon, AlienPartGenerator.AlienComp alienComp)
        {
            
            if (addon is LinkedBodyAddon linked && linked.linkLifeStageDrawSize)
            {
                // ThingDef_AlienRace thingDef_AlienRace = pawn.def as ThingDef_AlienRace;
                // AlienPartGenerator.AlienComp alienComp = pawn.GetComp<AlienPartGenerator.AlienComp>();
                return ((portrait && addon.drawSizePortrait != Vector2.zero) ? addon.drawSizePortrait : addon.drawSize) * (addon.scaleWithPawnDrawsize ? (linked.alignWithHead ? alienComp.customHeadDrawSize : alienComp.customDrawSize) : Vector2.one) * 1.5f;
            }
            
            return original;
        }
        */

        /*
        public static Vector3 DrawOffset(Vector3 original, AlienRace.AlienPartGenerator.BodyAddon addon, Rot4 rotation)
        {
            Vector3 result = original;
            LinkedBodyAddon linked = addon as LinkedBodyAddon;
            if (linked != null && linked.useDefautZeroOffset)
            {
                Vector3 v = new Vector3(rotation == Rot4.North ? result.x : (result.x - (rotation == Rot4.East ? -0.42f : 0.42f)), result.y, result.z + (rotation == Rot4.North ? 0.55f : 0.22f));
                result = v;
                //    Log.Message("DrawOffset useDefautZeroOffset Original: " + original + " Modifier: " + v + " result: " + result);
            }
           // if (linked != null && linked.useHeadPosition)
           // {
           //     result = new Vector3();
           //     result += quat * pawn.Drawer.renderer.BaseHeadOffsetAt(rotation);
           //    Log.Message("DrawOffset useHeadPosition Original: " + original + " Modifier: " + pawn.Drawer.renderer.BaseHeadOffsetAt(rotation) + " result: " + result);
           // }

            return result;
        }
        */
        public static Vector3 DrawPosition(Vector3 original, Pawn pawn, AlienPartGenerator.BodyAddon addon, Quaternion quat, Rot4 rotation)
        {
            Vector3 result = original;
            LinkedBodyAddon linked = addon as LinkedBodyAddon;
            if (linked != null && linked.alignWithHead)
            {
                Vector3 b = quat * pawn.Drawer.renderer.BaseHeadOffsetAt(rotation);
                Vector3 v = original + b;
                result = v;
                //    Log.Message("DrawPosition Original: " + original + " Modifier: " + b + " result: " + result);
            }

            return result;
        }
        /*
        public static AlienPartGenerator.RotationOffset DrawOffsets(AlienPartGenerator.RotationOffset original, Pawn pawn, AlienRace.AlienPartGenerator.BodyAddon addon)
        {
            AlienPartGenerator.RotationOffset result = original;
            LinkedBodyAddon linked = addon as LinkedBodyAddon;
            if (linked != null && linked.alignWithHead)
            {
                ThingDef_AlienRace thingDef_AlienRace = pawn.def as ThingDef_AlienRace;
                GraphicPaths paths = thingDef_AlienRace.alienRace.graphicPaths.GetCurrentGraphicPath(pawn.ageTracker.CurLifeStage);
                result = new AlienPartGenerator.RotationOffset();
                result.bodyTypes = original.bodyTypes;
                
               // for (int i = 0; i < result.bodyTypes.Count; i++)
               // {
               //     AlienPartGenerator.BodyTypeOffset body = result.bodyTypes[i];
               //     body.offset += linked.useHeadDrawSize ? paths.hea : paths.customDrawSize
               // }
                
                result.crownTypes = original.crownTypes;
                for (int i = 0; i < result.crownTypes.Count; i++)
                {
                    AlienPartGenerator.CrownTypeOffset body = result.crownTypes[i];
                    body.offset += paths.headOffsetDirectional?.GetOffset(pawn.Rotation) ?? Vector2.zero;
                }
            //    Log.Message("DrawOffsets result: " + result);
            }

            return result;
        }
        */
    }

}
