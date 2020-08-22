using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(JobGiver_Manhunter), "TryGiveJob")]
    public static class JobGiver_Manhunter_TryGiveJob_RangedVerb_Patch
    {

        static bool Prefix(ref JobGiver_Manhunter __instance, ref Job __result, ref Pawn pawn)
        {
            //Log.Warning("Detected Animal Attack");

            bool hasRangedVerb = false;



            //Log.Warning("Trying to fire at pawn");

            List<Verb> verbList = pawn.verbTracker.AllVerbs;

            //Log.Warning("Got list of verb");

            List<Verb> rangeList = new List<Verb>();
            for (int i = 0; i < verbList.Count; i++)
            {
                //Log.Warning("Checkity");
                //It corresponds with verbs anyway
                if (verbList[i].verbProps.range > 1.5f)
                {
                    rangeList.Add(verbList[i]);
                    hasRangedVerb = true;
                    //    Log.Warning("Added Ranged Verb");
                }
            }
            //Log.Warning("got list of ranged verb");



            //If there is no ranged verb just return;
            if (hasRangedVerb == false)
                return true;

            Verb rangeVerb = rangeList.RandomElementByWeight((Verb rangeItem) => rangeItem.verbProps.commonality);
            if (rangeVerb == null)
            {
                Log.Warning("Can't get random range verb");
                return true;
            }

            //Log.Warning("Range verb detected");
            Thing target = (Thing)AMA_AttackTargetFinder.BestAttackTarget((IAttackTargetSearcher)pawn, TargetScanFlags.NeedThreat | TargetScanFlags.NeedReachable, (Predicate<Thing>)(x =>
             x is Pawn || x is Building), 0.0f, 9999, new IntVec3(), float.MaxValue, false);

            //Seek thing hiding in embrasure.
            if (target == null)
                target = (Thing)AMA_AttackTargetFinder.BestAttackTarget((IAttackTargetSearcher)pawn, TargetScanFlags.NeedThreat, (Predicate<Thing>)(x =>
            x is Pawn || x is Building), 0.0f, 9999, new IntVec3(), float.MaxValue, false);
            //Use normal manhunter
            //Can't check for target if it doesn't exist duh

            //Log.Warning("CurrentEffectiveVerb " + pawn.CurrentEffectiveVerb);

            /*
			Log.Warning("Effective Range " + pawn.CurrentEffectiveVerb.verbProps.range);
			if (!pawn.CurrentEffectiveVerb.verbProps.MeleeRange)
			{
				Log.Warning("Not melee range");
			}
			*/

            bool targetInSight = false;
            Thing shootable = null;
            if (target == null)
            {
                shootable = (Thing)AMA_AttackTargetFinder.BestShootTargetFromCurrentPosition(pawn, (Predicate<Thing>)(x =>
                 x is Pawn || x is Building), rangeVerb.verbProps.range, rangeVerb.verbProps.minRange, TargetScanFlags.NeedThreat | TargetScanFlags.NeedLOSToPawns | TargetScanFlags.LOSBlockableByGas);

                //Log.Warning("Shootable found, " + shootable);
                if (shootable == null)
                {
                    //Log.Warning("No target in line of site");
                    return true;
                }
                targetInSight = true;
            }
            else if (target.Position.DistanceTo(pawn.Position) < rangeVerb.verbProps.minRange || target.Position.AdjacentTo8Way(pawn.Position))
            {
                //Log.Warning("Target too close, melee!");
                //Core code can't handle animal anymore
                if (pawn.CanReach(target, PathEndMode.Touch, Danger.Deadly, false))
                {
                    //Log.Warning("Melee Attack");
                    __result = new Job(JobDefOf.AttackMelee, target)
                    {
                        maxNumMeleeAttacks = 1,
                        expiryInterval = Rand.Range(420, 900),
                        attackDoorIfTargetLost = false
                    };
                    return false;
                }
                else
                {
                    return true;
                }
            }


            //Log.Warning("Got target");

            //Formally range attack job
            //if (target != null && pawn.CanReach(target, PathEndMode.Touch, Danger.Deadly, false))


            //Log.Warning("Ranged verb selected, range of  " + verb.verbProps.range);
            IntVec3 intVec;

            //Log.Warning("Trying to shoot");
            //Searches for target within range again.
            if (!targetInSight)
            {
                //Log.Warning("No target");
                shootable = (Thing)AMA_AttackTargetFinder.BestShootTargetFromCurrentPosition(pawn, (Predicate<Thing>)(x =>
                 x is Pawn || x is Building), rangeVerb.verbProps.range, rangeVerb.verbProps.minRange, TargetScanFlags.NeedThreat | TargetScanFlags.NeedLOSToPawns | TargetScanFlags.LOSBlockableByGas);
            }

            if (shootable != null)
            {
                //Log.Warning("Got Shootable");
                if (target.Position.DistanceTo(pawn.Position) < rangeVerb.verbProps.minRange || target.Position.AdjacentTo8Way(pawn.Position))
                {
                    //Log.Warning("Target too close, melee!");
                    //Core code can't handle animal anymore
                    if (pawn.CanReach(target, PathEndMode.Touch, Danger.Deadly, false))
                    {
                        //Log.Warning("Melee Attack");
                        __result = new Job(JobDefOf.AttackMelee, target)
                        {
                            maxNumMeleeAttacks = 1,
                            expiryInterval = Rand.Range(420, 900),
                            attackDoorIfTargetLost = false
                        };
                        return false;
                    }
                    else
                    {
                        //Log.Warning("Target too close and I can't melee right away");
                        return true;
                    }
                }

                //Log.Warning("Trying initiate job");
                __result = new Job(DefDatabase<JobDef>.GetNamed("AnimalRangeAttack"), shootable, JobGiver_AIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange, true)
                {
                    verbToUse = rangeVerb
                };
                //Log.Warning("Succesfully created job");
                return false;
            }
            //Target is not null
            if (target != null)
            {
                //Can't shoot at target from current position. Find a new position

                bool canShootCondition = false;
                //Log.Warning("Try casting");
                canShootCondition = CastPositionFinder.TryFindCastPosition(new CastPositionRequest
                {
                    caster = pawn,
                    target = target,
                    verb = rangeVerb,
                    maxRangeFromTarget = 9999,
                    wantCoverFromTarget = false
                }, out intVec);

                if (!canShootCondition)
                {
                    //Log.Warning("Can't find place to shoot at target");
                    return true;
                }
                //Log.Warning("Going");
                //Go to new destination

                //Protection againt not being able to find target.
                if (pawn.Position == intVec)
                {
                    //Log.Warning(pawn + " already at position to shoot, but target not selected to shoot.");
                    __result = new Job(JobDefOf.Wait, 100);
                    return false;
                }

                __result = new Job(JobDefOf.Goto, intVec)
                {

                    expiryInterval = JobGiver_AIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange,
                    checkOverrideOnExpire = true
                };
                return false;
            }
            //Log.Warning("Hit end condition");
            return true;

        }
    }
}
