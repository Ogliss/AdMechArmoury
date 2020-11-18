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
using System.Reflection.Emit;
using UnityEngine;
using OgsCompOversizedWeapon;

namespace AdeptusMechanicus.HarmonyInstance
{
    
    [HarmonyPatch(typeof(Verb), "TryCastNextBurstShot")]
    public static class Verb_TryCastNextBurstShot_MuzzlePosition_Transpiler
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            /*
            int i = 0;
            foreach (var instruction in instructionsList)
            {
                Log.Message(i+" opcode: " + instruction.opcode + " operand: " + instruction.operand);
                i++;
                //    yield return instruction;
            }
            */
            List<CodeInstruction> list = instructions.ToList<CodeInstruction>();
            list[24].operand = AccessTools.Method(typeof(Verb_TryCastNextBurstShot_MuzzlePosition_Transpiler), "ThrowMuzzleFlash", null, null);
            list.InsertRange(24, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldarg_0, null)
            });
            return list;
        }
        public static void ThrowMuzzleFlash(IntVec3 cell, Map map, ThingDef moteDef, float scale, Verb verb)
        {
            if (verb.EquipmentSource != null)
            {
                ThingDef mote = moteDef;
                Vector3 origin = verb.CasterIsPawn ? verb.CasterPawn.Drawer.DrawPos : verb.Caster.DrawPos;
                OgsCompOversizedWeapon.CompOversizedWeapon compOversized = verb.EquipmentSource.TryGetComp<OgsCompOversizedWeapon.CompOversizedWeapon>();
                CompEquippable equippable = verb.EquipmentSource.TryGetComp<CompEquippable>();
                float aimAngle = (verb.CurrentTarget.CenterVector3 - origin).AngleFlat();
                if (compOversized != null)
                {
                    bool DualWeapon = compOversized.Props != null && compOversized.Props.isDualWeapon;
                    Vector3 offsetMainHand = default(Vector3);
                    Vector3 offsetOffHand = default(Vector3);
                    float offHandAngle = aimAngle;
                    float mainHandAngle = aimAngle;
                    Harmony_PawnRenderer_DrawEquipmentAiming_Transpiler.SetAnglesAndOffsets(compOversized.parent, compOversized.parent, aimAngle, verb.Caster, ref offsetMainHand, ref offsetOffHand, ref offHandAngle, ref mainHandAngle, true, DualWeapon && !compOversized.FirstAttack);
                    if (DualWeapon) Log.Message("Throwing flash for "+ compOversized.parent.LabelCap + " offsetMainHand: " + offsetMainHand + " offsetOffHand: " + offsetOffHand + " Using "+(!compOversized.FirstAttack ? "OffHand" : "MainHand")+ " FirstAttack: " + compOversized.FirstAttack);
                    origin += DualWeapon && !compOversized.FirstAttack ? offsetOffHand : offsetMainHand;
                    // origin += compOversized.AdjustRenderOffsetFromDir(equippable.PrimaryVerb.CasterPawn, !compOversized.FirstAttack);
                    if (compOversized.Props.isDualWeapon) compOversized.FirstAttack = !compOversized.FirstAttack;
                }
                if (verb.EquipmentSource.def.HasModExtension<BarrelOffsetExtension>())
                {
                    BarrelOffsetExtension ext = verb.EquipmentSource.def.GetModExtension<BarrelOffsetExtension>() ;
                    EffectProjectileExtension ext2 = verb.GetProjectile().HasModExtension<EffectProjectileExtension>() ? verb.GetProjectile().GetModExtension<EffectProjectileExtension>() : null;
                    float offset = ext.barrellength;
                    origin += (verb.CurrentTarget.CenterVector3 - origin).normalized * (verb.EquipmentSource.def.graphic.drawSize.magnitude * (offset));
                    if (ext2 !=null && ext2.muzzleFlare)
                    {
                        ThingDef muzzleFlaremote = DefDatabase<ThingDef>.GetNamed(!ext2.muzzleSmokeDef.NullOrEmpty() ? ext2.muzzleFlareDef : "Mote_SparkFlash");
                        MoteMaker.MakeStaticMote(origin, map, muzzleFlaremote, ext2.muzzleFlareSize);
                    }
                    else if (ext.muzzleFlare)
                    {
                        ThingDef muzzleFlaremote = DefDatabase<ThingDef>.GetNamed(!ext.muzzleSmokeDef.NullOrEmpty() ? ext.muzzleFlareDef : "Mote_SparkFlash");
                        MoteMaker.MakeStaticMote(origin, map, muzzleFlaremote, ext.muzzleFlareSize);
                    }
                    if (ext2 != null && ext2.muzzleSmoke)
                    {
                        string muzzleSmokemote = !ext2.muzzleSmokeDef.NullOrEmpty() ? ext2.muzzleSmokeDef : "OG_Mote_SmokeTrail";
                        TrailThrower.ThrowSmoke(origin, ext2.muzzleSmokeSize, map, muzzleSmokemote);
                    }
                    else if (ext.muzzleSmoke)
                    {
                        string muzzleSmokemote = !ext.muzzleSmokeDef.NullOrEmpty() ? ext.muzzleSmokeDef : "OG_Mote_SmokeTrail";
                        TrailThrower.ThrowSmoke(origin, ext.muzzleSmokeSize, map, muzzleSmokemote);
                    }
                }
                MoteMaker.MakeStaticMote(origin, map, mote, scale);
            }
            else
            {

                MoteMaker.MakeStaticMote(cell.ToVector3Shifted(), map, moteDef, scale);
            }
        }   
    }
    
}
