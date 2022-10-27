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
using System.Reflection;
using System.Reflection.Emit;
using AdeptusMechanicus.settings;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Verb), "TryStartCastOn", new Type[] { 
        typeof(LocalTargetInfo), 
        typeof(LocalTargetInfo), 
        typeof(bool),
        typeof(bool),
        typeof(bool),
        typeof(bool) })]
    public static class Verb_TryStartCastOn_warmupTime_Transpiler
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            FieldInfo warmupTime = AccessTools.Field(typeof(VerbProperties), nameof(VerbProperties.warmupTime));

            for (int i = 0; i < instructionsList.Count; i++)
            {
                var instruction = instructionsList[i];
                if (instruction.opcode == OpCodes.Ldfld && instruction.OperandIs(warmupTime))
                {
                    yield return instruction;
                //    Log.Message("Verb TryStartCastOn: Patching at..."+ i + " opcode: " + instruction.opcode + " operand: " + instruction.operand);
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_0);              // Verb
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_1);              // LocalTargetInfo
                    instruction = new CodeInstruction(opcode: OpCodes.Call, operand: typeof(Verb_TryStartCastOn_warmupTime_Transpiler).GetMethod("AdjustedWarmup"));

                }
                yield return instruction;
            }

        }

        public static float AdjustedWarmup(float warmUpTime, Verb verb, LocalTargetInfo castTarg)
        {
            if (AdeptusIntergrationUtility.enabled_CombatExtended)
            {
                if (!CE(verb))
                {
                    return warmUpTime;
                }
            }
            else
            {
                if (!(verb is Verb_Shoot) && !(verb is AbilitesExtended.Verb_ShootEquipment))
                {
                    return warmUpTime;
                }
            }
            float result = warmUpTime;
        //    AdvancedVerbProperties props = verb.verbProps as AdvancedVerbProperties;
            IAdvancedVerb props = verb.verbProps as IAdvancedVerb;
            ThingWithComps gun = verb.EquipmentSource;
            CompEquippable compeq = verb.EquipmentCompSource;
            Thing caster = verb.caster;
            Pawn CasterPawn = verb.CasterPawn;
            if (props != null)
            {
                /*
                if (verb.GetsHot(out bool GetsHotCrit, out float GetsHotCritChance, out bool GetsHotCritExplosion, out float GetsHotCritExplosionChance, out bool canDamageWeapon, out float extraWeaponDamage))
                {

                }
                */
                if (AMAMod.settings.AllowRapidFire && props.RapidFire)
                {
                    //    log.message(string.Format("RapidFire prefix pre-modified Values, Warmup: {0}", gun.def.Verbs[0].warmupTime, cooldown));
                    if (caster.Position.InHorDistOf(castTarg.Cell, verb.verbProps.range * props.RapidFireRange))
                    {
                        float reduction = ((verb.verbProps.burstShotCount - 1) * verb.verbProps.ticksBetweenBurstShots).TicksToSeconds() / 4;
                        reduction += warmUpTime / 2;
                        result -= reduction;
                    }
                    //    log.message(string.Format("RapidFire prefix post-modified Values, Warmup: {0}", gun.def.Verbs[0].warmupTime, cooldown));
                }
                else if (props.HeavyWeapon && verb.CasterIsPawn)
                {
                    CompWeapon_GunSpecialRules GunExt = gun.TryGetCompFast<CompWeapon_GunSpecialRules>();
                    if (GunExt.ticksHere < (props.HeavyWeaponSetupTime.SecondsToTicks()))
                    {
                        float extra = props.HeavyWeaponSetupTime;
                        if (CasterPawn.story?.bodyType == BodyTypeDefOf.Hulk)
                        {
                            extra *= 0.5f;
                        }
                        extra = Math.Max(0, extra - GunExt.ticksHere.TicksToSeconds());
                        //    Log.Message(string.Format("HeavyWeapon prefix pre-modified Values, Warmup: {0} + {1}, last move tick: {2}", __instance.verbProps.warmupTime, extra, GunExt.LastMovedTick));
                        result += extra;
                    }
                    //    log.message(string.Format("HeavyWeapon prefix post-modified Values, Warmup: {0}, last move tick: {1}", gun.def.Verbs[0].warmupTime, GunExt.LastMovedTick));
                }
                if (compeq != null)
                {
                    if (verb.GetProjectile().projectile.Conversion())
                    {
                        float distance = caster.Position.DistanceTo(castTarg.Cell);
                        result = (float)warmUpTime + (distance / 30);
                    }
                }
            }
        //    Log.Message("Testing return result" + result + " warmUpTime: " + warmUpTime + " for verb: " + verb + " casting at " + castTarg);
            return result;
        }

        private static bool CE(Verb __instance)
        {
            if (!(__instance is CombatExtended.Verb_ShootCE) && !(__instance is AbilitesExtended.Verb_ShootEquipment))
            {
                return false;
            }
            return true;
        }
    }
    
}
