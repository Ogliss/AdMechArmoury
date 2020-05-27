﻿using System;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using RimWorld;
using UnityEngine;
using Verse;

namespace CompActivatableEffect
{
    [StaticConstructorOnStartup]
    internal static class HarmonyCompActivatableEffect
    {
        static HarmonyCompActivatableEffect()
        {
            var harmony = HarmonyInstance.Create("rimworld.jecrell.comps.activator");

            harmony.Patch(typeof(Pawn).GetMethod("GetGizmos"), null,
                new HarmonyMethod(typeof(HarmonyCompActivatableEffect).GetMethod("GetGizmosPrefix")));
            harmony.Patch(typeof(PawnRenderer).GetMethod("DrawEquipmentAiming"), null,
                new HarmonyMethod(typeof(HarmonyCompActivatableEffect), nameof(DrawEquipmentAimingPostFix)));
            harmony.Patch(typeof(Verb).GetMethod("TryStartCastOn"),
                new HarmonyMethod(typeof(HarmonyCompActivatableEffect), nameof(TryStartCastOnPrefix)), null);
            harmony.Patch(typeof(Pawn_DraftController).GetMethod("set_Drafted"), null,
                new HarmonyMethod(typeof(HarmonyCompActivatableEffect).GetMethod("set_Drafted_PostFix")));
            harmony.Patch(typeof(Pawn).GetMethod("ExitMap"),
                new HarmonyMethod(typeof(HarmonyCompActivatableEffect).GetMethod("ExitMap_PreFix")), null);
            harmony.Patch(typeof(Pawn_EquipmentTracker).GetMethod("TryDropEquipment"),
                new HarmonyMethod(typeof(HarmonyCompActivatableEffect).GetMethod("TryDropEquipment_PreFix")), null);
            harmony.Patch(typeof(Pawn_DraftController).GetMethod("set_Drafted"), null,
                new HarmonyMethod(typeof(HarmonyCompActivatableEffect).GetMethod("set_DraftedPostFix")));
        }

        //=================================== COMPACTIVATABLE

        // Verse.Pawn_EquipmentTracker
        public static void TryDropEquipment_PreFix(Pawn_EquipmentTracker __instance, ThingWithComps eq)
        {
            if (__instance is Pawn_EquipmentTracker eqq &&
                eqq.Primary is ThingWithComps t &&
                t.GetComp<CompActivatableEffect>() is CompActivatableEffect compActivatableEffect &&
                compActivatableEffect.CurrentState == CompActivatableEffect.State.Activated)
                compActivatableEffect.TryDeactivate();
        }

        public static void ExitMap_PreFix(Pawn __instance, bool allowedToJoinOrCreateCaravan)
        {
            if (__instance is Pawn p && p.equipment is Pawn_EquipmentTracker eq &&
                eq.Primary is ThingWithComps t &&
                t.GetComp<CompActivatableEffect>() is CompActivatableEffect compActivatableEffect &&
                compActivatableEffect.CurrentState == CompActivatableEffect.State.Activated)
                compActivatableEffect.TryDeactivate();
        }

#pragma warning disable IDE1006 // Naming Styles
        public static void set_DraftedPostFix(Pawn_DraftController __instance, bool value)
#pragma warning restore IDE1006 // Naming Styles
        {
            if (__instance.pawn is Pawn p && p.equipment is Pawn_EquipmentTracker eq &&
                eq.Primary is ThingWithComps t &&
                t.GetComp<CompActivatableEffect>() is CompActivatableEffect compActivatableEffect)
                if (value == false)
                {
                    if (compActivatableEffect.CurrentState == CompActivatableEffect.State.Activated)
                        compActivatableEffect.TryDeactivate();
                }
                else
                {
                    if (compActivatableEffect.CurrentState == CompActivatableEffect.State.Deactivated)
                        compActivatableEffect.TryActivate();
                }
        }

        public static bool TryStartCastOnPrefix(ref bool __result, Verb __instance)
        {
            if (__instance.caster is Pawn pawn)
            {
                var pawn_EquipmentTracker = pawn?.equipment;
                if (pawn_EquipmentTracker == null) return true;

                var thingWithComps =
                    pawn_EquipmentTracker?.Primary; //(ThingWithComps)AccessTools.Field(typeof(Pawn_EquipmentTracker), "primaryInt").GetValue(pawn_EquipmentTracker);

                var compActivatableEffect = thingWithComps?.GetComp<CompActivatableEffect>();
                if (compActivatableEffect == null) return true;

                //Equipment source throws errors when checked while casting abilities with a weapon equipped.
                // to avoid this error preventing our code from executing, we do a try/catch.
                try
                {
                    if (__instance?.EquipmentSource != thingWithComps)
                        return true;
                }
                catch (Exception e)
                {
                }

                if (compActivatableEffect.CurrentState == CompActivatableEffect.State.Activated) return true;
                
                if (Find.TickManager.TicksGame % 250 == 0)
                    Messages.Message("DeactivatedWarning".Translate(pawn.Label),
                        MessageTypeDefOf.RejectInput);
                __result = false;
                return false;
            }
            return true;
        }

        ///// <summary>
        ///// Prevents the user from having damage with the verb.
        ///// </summary>
        ///// <param name="__instance"></param>
        ///// <param name="__result"></param>
        ///// <param name="pawn"></param>
        //public static void GetDamageFactorForPostFix(Verb __instance, ref float __result, Pawn pawn)
        //{
        //    Pawn_EquipmentTracker pawn_EquipmentTracker = pawn.equipment;
        //    if (pawn_EquipmentTracker != null)
        //    {
        //        //Log.Message("2");
        //        ThingWithComps thingWithComps = (ThingWithComps)AccessTools.Field(typeof(Pawn_EquipmentTracker), "primaryInt").GetValue(pawn_EquipmentTracker);

        //        if (thingWithComps != null)
        //        {
        //            //Log.Message("3");
        //            CompActivatableEffect compActivatableEffect = thingWithComps.GetComp<CompActivatableEffect>();
        //            if (compActivatableEffect != null)
        //            {
        //                if (compActivatableEffect.CurrentState != CompActivatableEffect.State.Activated)
        //                {
        //                    //Messages.Message("DeactivatedWarning".Translate(), MessageSound.RejectInput);
        //                    __result = 0f;
        //                }
        //            }
        //        }
        //    }
        //}


        /// <summary>
        ///     Adds another "layer" to the equipment aiming if they have a
        ///     weapon with a CompActivatableEffect.
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="eq"></param>
        /// <param name="drawLoc"></param>
        /// <param name="aimAngle"></param>
        public static void DrawEquipmentAimingPostFix(PawnRenderer __instance, Thing eq, Vector3 drawLoc,float aimAngle)
        {
            var pawn = (Pawn) AccessTools.Field(typeof(PawnRenderer), "pawn").GetValue(__instance);

            var pawn_EquipmentTracker = pawn.equipment;
            var thingWithComps =
                pawn_EquipmentTracker?.Primary; //(ThingWithComps)AccessTools.Field(typeof(Pawn_EquipmentTracker), "primaryInt").GetValue(pawn_EquipmentTracker);
            CompOversizedWeapon.CompOversizedWeapon compOversized = thingWithComps.TryGetComp<CompOversizedWeapon.CompOversizedWeapon>();
            var compActivatableEffect = thingWithComps?.GetComp<CompActivatableEffect>();
            if (compActivatableEffect?.Graphic == null) return;
            if (compActivatableEffect.CurrentState != CompActivatableEffect.State.Activated) return;
            var num = aimAngle - 90f;
            var flip = false;

            if (aimAngle > 20f && aimAngle < 160f)
            {
                //mesh = MeshPool.GridPlaneFlip(thingWithComps.def.graphicData.drawSize);
                num += eq.def.equippedAngleOffset;
            }
            else if (aimAngle > 200f && aimAngle < 340f)
            {
                //mesh = MeshPool.GridPlane(thingWithComps.def.graphicData.drawSize);
                flip = true;
                num -= 180f;
                num -= eq.def.equippedAngleOffset;
            }
            else
            {
                if (compOversized != null)
                    num = AdjustOffsetAtPeace(eq, pawn, compOversized, num);
                else
                    num += eq.def.equippedAngleOffset;
            }
            if (compOversized != null)
            {
                if (compOversized.Props != null && (!pawn.IsFighting() && (compOversized.Props.verticalFlipNorth && pawn.Rotation == Rot4.North)))
                {
                    num += 180f;
                }
                if (!pawn.IsFighting() || pawn.TargetCurrentlyAimingAt == null)
                {
                    num = AdjustNonCombatRotation(pawn, num, compOversized);
                }
            }

            Vector3 offset = Vector3.zero;

            if (eq is ThingWithComps eqComps)
            {
                if (compOversized!=null)
                {
                    if (pawn.Rotation == Rot4.East)
                        offset = compOversized.Props.eastOffset;
                    else if (pawn.Rotation == Rot4.West)
                        offset = compOversized.Props.westOffset;
                    else if (pawn.Rotation == Rot4.North)
                        offset = compOversized.Props.northOffset;
                    else if (pawn.Rotation == Rot4.South)
                        offset = compOversized.Props.southOffset;
                    offset += compOversized.Props.offset;
                }
                                
                                    
                var deflector = eqComps.AllComps.FirstOrDefault(y =>
                    y.GetType().ToString().Contains("Deflect"));
                if (deflector != null)
                {
                    var isActive = (bool) AccessTools
                        .Property(deflector.GetType(), "IsAnimatingNow").GetValue(deflector, null);
                    if (isActive)
                    {
                        float numMod = (int) AccessTools
                            .Property(deflector.GetType(), "AnimationDeflectionTicks")
                            .GetValue(deflector, null);
                        //float numMod2 = new float();
                        //numMod2 = numMod;
                        if (numMod > 0)
                            if (!flip) num += (numMod + 1) / 2;
                            else num -= (numMod + 1) / 2;
                    }
                }
            }
            num %= 360f;

            //ThingWithComps eqComps = eq as ThingWithComps;
            //if (eqComps != null)
            //{
            //    ThingComp deflector = eqComps.AllComps.FirstOrDefault<ThingComp>((ThingComp y) => y.GetType().ToString() == "CompDeflector.CompDeflector");
            //    if (deflector != null)
            //    {
            //        float numMod = (float)((int)AccessTools.Property(deflector.GetType(), "AnimationDeflectionTicks").GetValue(deflector, null));
            //        //Log.ErrorOnce("NumMod " + numMod.ToString(), 1239);
            //numMod = (numMod + 1) / 2;
            //if (subtract) num -= numMod;
            //else num += numMod;
            //    }
            //}

            var matSingle = compActivatableEffect.Graphic.MatSingle;
            //if (mesh == null) mesh = MeshPool.GridPlane(thingWithComps.def.graphicData.drawSize);

            var s = new Vector3(eq.def.graphicData.drawSize.x, 1f, eq.def.graphicData.drawSize.y);
            var matrix = default(Matrix4x4);
            drawLoc.y = compActivatableEffect.Altitude(drawLoc);
            matrix.SetTRS(drawLoc + offset , Quaternion.AngleAxis(num, Vector3.up), s);
            if (!flip) Graphics.DrawMesh(MeshPool.plane10, matrix, matSingle, 0);
            else Graphics.DrawMesh(MeshPool.plane10Flip, matrix, matSingle, 0);
            //Graphics.DrawMesh(mesh, drawLoc, Quaternion.AngleAxis(num, Vector3.up), matSingle, 0);

            if (compOversized != null)
            {
                if (compOversized.Props != null && compOversized.Props.isDualWeapon)
                {
                    offset = new Vector3(-1f * offset.x, offset.y, offset.z);
                    Mesh curPool;
                    if (pawn.Rotation == Rot4.North || pawn.Rotation == Rot4.South)
                    {
                        num += 135f;
                        num %= 360f;
                        curPool = !flip ? MeshPool.plane10Flip : MeshPool.plane10;
                    }
                    else
                    {
                        offset = new Vector3(offset.x, offset.y - 0.1f, offset.z + 0.15f);
                        curPool = !flip ? MeshPool.plane10 : MeshPool.plane10Flip;
                    }
                    matrix.SetTRS(drawLoc + offset, Quaternion.AngleAxis(num, Vector3.up), s);
                    Graphics.DrawMesh(curPool, matrix, matSingle, 0);
                }
            }
        }

        private static float AdjustOffsetAtPeace(Thing eq, Pawn pawn, CompOversizedWeapon.CompOversizedWeapon compOversizedWeapon, float num)
        {
            if (compOversizedWeapon == null)
            {
                return num;
            }
            Mesh mesh;
            mesh = MeshPool.plane10;
            var offsetAtPeace = eq.def.equippedAngleOffset;
            if (compOversizedWeapon.Props != null && (!pawn.IsFighting() && compOversizedWeapon.Props.verticalFlipOutsideCombat))
            {
                offsetAtPeace += 180f;
            }
            num += offsetAtPeace;
            return num;
        }

        private static float AdjustNonCombatRotation(Pawn pawn, float num, CompOversizedWeapon.CompOversizedWeapon compOversizedWeapon)
        {
            if (compOversizedWeapon==null)
            {
                return num;
            }
            if (compOversizedWeapon.Props != null)
            {
                if (pawn.Rotation == Rot4.North)
                {
                    num += compOversizedWeapon.Props.angleAdjustmentNorth;
                }
                else if (pawn.Rotation == Rot4.East)
                {
                    num += compOversizedWeapon.Props.angleAdjustmentEast;
                }
                else if (pawn.Rotation == Rot4.West)
                {
                    num += compOversizedWeapon.Props.angleAdjustmentWest;
                }
                else if (pawn.Rotation == Rot4.South)
                {
                    num += compOversizedWeapon.Props.angleAdjustmentSouth;
                }
            }
            return num;
        }

        private static Vector3 AdjustRenderOffsetFromDir(Pawn pawn, CompOversizedWeapon.CompOversizedWeapon compOversizedWeapon)
        {
            var curDir = pawn.Rotation;

            Vector3 curOffset = Vector3.zero;

            if (compOversizedWeapon.Props != null)
            {

                curOffset = compOversizedWeapon.Props.northOffset;
                if (curDir == Rot4.East)
                {
                    curOffset = compOversizedWeapon.Props.eastOffset;
                }
                else if (curDir == Rot4.South)
                {
                    curOffset = compOversizedWeapon.Props.southOffset;
                }
                else if (curDir == Rot4.West)
                {
                    curOffset = compOversizedWeapon.Props.westOffset;
                }
            }

            return curOffset;
        }

        public static IEnumerable<Gizmo> GizmoGetter(CompActivatableEffect compActivatableEffect)
        {
            //Log.Message("5");
            if (compActivatableEffect.GizmosOnEquip)
            {
                //Log.Message("6");
                //Iterate EquippedGizmos
                var enumerator = compActivatableEffect.EquippedGizmos().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    //Log.Message("7");
                    var current = enumerator.Current;
                    yield return current;
                }
            }
        }

        public static void GetGizmosPrefix(Pawn __instance, ref IEnumerable<Gizmo> __result)
        {
            //Log.Message("1");
            var pawn_EquipmentTracker = __instance.equipment;
            if (pawn_EquipmentTracker != null)
            {
                //Log.Message("2");
                //ThingWithComps thingWithComps = (ThingWithComps)AccessTools.Field(typeof(Pawn_EquipmentTracker), "primaryInt").GetValue(pawn_EquipmentTracker);
                var thingWithComps = pawn_EquipmentTracker.Primary;

                if (thingWithComps != null)
                {
                    //Log.Message("3");
                    var compActivatableEffect = thingWithComps.GetComp<CompActivatableEffect>();
                    if (compActivatableEffect != null)
                        if (__instance != null)
                            if (__instance.Faction == Faction.OfPlayer)
                            {
                                __result = __result.Concat(GizmoGetter(compActivatableEffect));
                            }
                            else
                            {
                                if (compActivatableEffect.CurrentState == CompActivatableEffect.State.Deactivated)
                                    compActivatableEffect.Activate();
                            }
                }
            }
        }
    }
}