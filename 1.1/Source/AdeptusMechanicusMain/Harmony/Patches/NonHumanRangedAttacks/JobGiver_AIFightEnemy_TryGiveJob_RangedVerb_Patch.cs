using AbilitesExtended;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(JobGiver_AIFightEnemy), "TryGiveJob")]
    public static class JobGiver_AIFightEnemy_TryGiveJob_RangedVerb_Patch
    {
        static bool Prefix(ref JobGiver_AIFightEnemy __instance, ref Job __result, ref Pawn pawn)
        {
            //    Log.Warning(string.Format("Tame animal job detected: {0}", pawn.LabelCap));

            bool player = pawn.Faction == Faction.OfPlayerSilentFail;
            if (pawn.RaceProps.Humanlike)
                return HumanUser(ref __instance, ref __result, ref pawn, player);
            bool hasRangedVerb = false;
            List<Verb> verbList = pawn.verbTracker.AllVerbs;
            List<Verb> rangeList = new List<Verb>();
            for (int i = 0; i < verbList.Count; i++)
            {
                //    Log.Warning("Checkity");
                //    Log.Warning(string.Format("verbList: {0}, Name: {1} RangeMax: {2}", i, verbList[i].verbProps.label, verbList[i].verbProps.range));
                //It corresponds with verbs anyway
                if (verbList[i].verbProps.range > 1.5f)
                {
                    rangeList.Add(verbList[i]);
                    hasRangedVerb = true;
                }
                //Log.Warning("Added Ranged Verb");
            }
            if (pawn.equipment?.PrimaryEq!=null)
            {
                for (int i = 0; i < pawn.equipment.PrimaryEq.AllVerbs.Count; i++)
                {
                    //    Log.Warning("Checkity");
                    //    Log.Warning(string.Format("verbList: {0}, Name: {1} RangeMax: {2}", i, verbList[i].verbProps.label, verbList[i].verbProps.range));
                    //It corresponds with verbs anyway
                    if (pawn.equipment.PrimaryEq.AllVerbs[i].verbProps.range > 1.5f)
                    {
                        if (!rangeList.Contains(pawn.equipment.PrimaryEq.AllVerbs[i]))
                        {
                            rangeList.Add(pawn.equipment.PrimaryEq.AllVerbs[i]);
                            hasRangedVerb = true;
                        }
                    }
                    //Log.Warning("Added Ranged Verb");
                }
            }
            if (pawn.health.hediffSet.hediffs.Any(x => x.TryGetCompFast<HediffComp_VerbGiverExtra>() != null))
            {
                List<Hediff> list = pawn.health.hediffSet.hediffs.FindAll(x => x.TryGetCompFast<HediffComp_VerbGiverExtra>() != null);

                foreach (Hediff item in list)
                {
                    HediffComp_VerbGiverExtra _VerbGiver = item.TryGetCompFast<HediffComp_VerbGiverExtra>();
                    if (_VerbGiver != null)
                    {
                        for (int i = 0; i < _VerbGiver.verbTracker.AllVerbs.Count; i++)
                        {
                            //    Log.Warning("Checkity");
                            //    Log.Warning(string.Format("verbList: {0}, Name: {1} RangeMax: {2}", i, verbList[i].verbProps.label, verbList[i].verbProps.range));
                            //It corresponds with verbs anyway
                            if (_VerbGiver.verbTracker.AllVerbs[i].verbProps.range > 1.5f)
                            {
                                if (!rangeList.Contains(_VerbGiver.verbTracker.AllVerbs[i]))
                                {
                                    rangeList.Add(_VerbGiver.verbTracker.AllVerbs[i]);
                                    hasRangedVerb = true;
                                }
                            }
                            //Log.Warning("Added Ranged Verb");
                        }
                    }
                }
                if (hasRangedVerb == false)
                {
                    Log.Warning("I don't have range verb");
                    return true;
                }
            }
            // this.SetCurMeleeVerb(updatedAvailableVerbsList.RandomElementByWeight((VerbEntry ve) => ve.SelectionWeight).verb);

        //    Log.Warning(string.Format("rangeVerbs: {0}", rangeList.Count));
            Verb rangeVerb;
            if (rangeList.Count>1)
            {
                rangeVerb = rangeList.RandomElementByWeightWithDefault((Verb rangeItem) => rangeItem.verbProps.commonality, 0.5f);
            }
            else if (rangeList.Count == 1)
            {
                rangeVerb = rangeList.First();
            }
            else
            {
                rangeVerb = null;
            }

            if (rangeVerb == null)
            {
            //    Log.Warning("Can't get random range verb");
                return true;
            }
            else
            {

            //    Log.Warning(string.Format("rangeVerb: {0}, Range Max: {1}, Min: {2}, Burst: {3}, Projectile: {4}", rangeVerb.verbProps.label, rangeVerb.verbProps.range, rangeVerb.verbProps.minRange, rangeVerb.verbProps.burstShotCount, rangeVerb.verbProps.defaultProjectile));
               
            }


            Thing enemyTarget = (Thing)AttackTargetFinder.BestAttackTarget((IAttackTargetSearcher)pawn, TargetScanFlags.NeedThreat, (Predicate<Thing>)(x =>
            x is Pawn || x is Building), 0.0f, rangeVerb.verbProps.range, new IntVec3(), float.MaxValue, false);


            if (enemyTarget == null)
            {
            //    Log.Warning("I can't find anything to fight.");
                return true;
            }
            Rand.PushState();
            bool useranged = Rand.Chance(rangeVerb.verbProps.commonality);
            int expiryInterval = Rand.Range(420, 900);
            Rand.PopState();
            //    Log.Warning(string.Format("useranged: {0}", useranged));
            if (!useranged)
            {
                //If adjacent melee attack
                if (enemyTarget.Position.AdjacentTo8Way(pawn.Position))
                {
                    __result = new Job(JobDefOf.AttackMelee, enemyTarget)
                    {
                        maxNumMeleeAttacks = 1,
                        expiryInterval = expiryInterval,
                        attackDoorIfTargetLost = false
                    };
                    return false;
                }
                //Only go if I am to be released. This prevent animal running off.
                if (pawn.CanReach(enemyTarget, PathEndMode.Touch, Danger.Deadly, false))
                {
                //    Log.Warning("Melee Attack");
                    __result = new Job(JobDefOf.AttackMelee, enemyTarget)
                    {
                        maxNumMeleeAttacks = 1,
                        expiryInterval = expiryInterval,
                        attackDoorIfTargetLost = false
                    };
                    return false;
                }
                else
                {
                    return true;
                }
            }

            //Log.Warning("got list of ranged verb");

        //    Log.Warning("Attempting flag");
            bool flag1 = (double)CoverUtility.CalculateOverallBlockChance(pawn.Position, enemyTarget.Position, pawn.Map) > 0.00999999977648258;
            bool flag2 = pawn.Position.Standable(pawn.Map);
            bool flag3 = rangeVerb.CanHitTarget(enemyTarget);
            bool flag4 = (pawn.Position - enemyTarget.Position).LengthHorizontalSquared < 25;



            if (flag1 && flag2 && flag3 || flag4 && flag3)
            {
            //    Log.Warning("Shooting");
                __result = new Job(DefDatabase<JobDef>.GetNamed("AnimalRangeAttack"), enemyTarget, JobGiver_AIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange, true)
                {
                    verbToUse = rangeVerb

                };
                return false;
            }

            bool canShootCondition = false;
            //    Log.Warning("Try casting");

            //Animals with training seek cover
            /*
				if (pawn.training.IsCompleted(TrainableDefOf.Release) && (double)verb.verbProps.range > 7.0)
					Log.Warning("Attempting cover");
				Log.Warning("Try get flag radius :" + Traverse.Create(__instance).Method("GetFlagRadius", pawn).GetValue<float>());
				Log.Warning("Checking cast condition");
				*/

            //Don't find new position if animal not released.


            canShootCondition = CastPositionFinder.TryFindCastPosition(new CastPositionRequest
            {
                caster = pawn,
                target = enemyTarget,
                verb = rangeVerb,
                maxRangeFromTarget = rangeVerb.verbProps.range,
                wantCoverFromTarget = (double)rangeVerb.verbProps.range > 7.0,
                locus = pawn.Position,
                maxRangeFromLocus = Traverse.Create(__instance).Method("GetFlagRadius", pawn).GetValue<float>(),
                maxRegions = 50
            }, out IntVec3 dest);

            if (!canShootCondition)
            {
             //   Log.Warning("I can't move to shooting position");


                return true;
            }

            if (dest == pawn.Position)
            {
             //   Log.Warning("I will stay here and attack");
                __result = new Job(DefDatabase<JobDef>.GetNamed("AnimalRangeAttack"), enemyTarget, JobGiver_AIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange, true)
                {
                    verbToUse = rangeVerb
                };
                return false;
            }
         //   Log.Warning("Going to new place");
            __result = new Job(JobDefOf.Goto, (LocalTargetInfo)dest)
            {
                expiryInterval = JobGiver_AIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange,
                checkOverrideOnExpire = true
            };
            return false;
        }


        public static bool HumanUser(ref JobGiver_AIFightEnemy __instance, ref Job __result, ref Pawn pawn, bool player)
        {
            if (pawn.abilities != null)
            {
                bool hasRangedVerb = false;
                List<Verb> rangeList = new List<Verb>();
                foreach (Ability ability in pawn.abilities.abilities)
                {
                    AbilitesExtended.EquipmentAbility equipmentAbility = ability as AbilitesExtended.EquipmentAbility;
                    if (equipmentAbility != null)
                    {
                        if (equipmentAbility.CanCast && equipmentAbility.verb.verbProps.range < 1.5f)
                        {
                            Log.Message("Adding " + equipmentAbility.def.LabelCap + "to " + pawn + "");
                            rangeList.Add(equipmentAbility.verb);
                            hasRangedVerb = true;
                        }
                    }
                }
                if (!rangeList.NullOrEmpty() && hasRangedVerb)
                {
                    Log.Message("found " + rangeList.Count + " eqabilities for " + pawn);
                }
            }
            return true;
        }
       
        public static bool ToolUser(ref JobGiver_AIFightEnemy __instance, ref Job __result, ref Pawn pawn, bool player)
        {
            bool hasRangedVerb = false;
            List<Verb> verbList = pawn.verbTracker.AllVerbs;
            List<Verb> rangeList = new List<Verb>();
            for (int i = 0; i < verbList.Count; i++)
            {
                //    Log.Warning("Checkity");
                //    Log.Warning(string.Format("verbList: {0}, Name: {1} RangeMax: {2}", i, verbList[i].verbProps.label, verbList[i].verbProps.range));
                //It corresponds with verbs anyway
                if (verbList[i].verbProps.range > 1.5f)
                {
                    rangeList.Add(verbList[i]);
                    hasRangedVerb = true;
                }
                //Log.Warning("Added Ranged Verb");
            }
            if (pawn.equipment?.PrimaryEq != null)
            {
                for (int i = 0; i < pawn.equipment.PrimaryEq.AllVerbs.Count; i++)
                {
                    //    Log.Warning("Checkity");
                    //    Log.Warning(string.Format("verbList: {0}, Name: {1} RangeMax: {2}", i, verbList[i].verbProps.label, verbList[i].verbProps.range));
                    //It corresponds with verbs anyway
                    if (pawn.equipment.PrimaryEq.AllVerbs[i].verbProps.range > 1.5f)
                    {
                        if (!rangeList.Contains(pawn.equipment.PrimaryEq.AllVerbs[i]))
                        {
                            rangeList.Add(pawn.equipment.PrimaryEq.AllVerbs[i]);
                            hasRangedVerb = true;
                        }
                    }
                    //Log.Warning("Added Ranged Verb");
                }
            }
            if (pawn.health.hediffSet.hediffs.Any(x => x.TryGetCompFast<HediffComp_VerbGiverExtra>() != null))
            {
                List<Hediff> list = pawn.health.hediffSet.hediffs.FindAll(x => x.TryGetCompFast<HediffComp_VerbGiverExtra>() != null);

                foreach (Hediff item in list)
                {
                    HediffComp_VerbGiverExtra _VerbGiver = item.TryGetCompFast<HediffComp_VerbGiverExtra>();
                    if (_VerbGiver != null)
                    {
                        for (int i = 0; i < _VerbGiver.verbTracker.AllVerbs.Count; i++)
                        {
                            //    Log.Warning("Checkity");
                            //    Log.Warning(string.Format("verbList: {0}, Name: {1} RangeMax: {2}", i, verbList[i].verbProps.label, verbList[i].verbProps.range));
                            //It corresponds with verbs anyway
                            if (_VerbGiver.verbTracker.AllVerbs[i].verbProps.range > 1.5f)
                            {
                                if (!rangeList.Contains(_VerbGiver.verbTracker.AllVerbs[i]))
                                {
                                    rangeList.Add(_VerbGiver.verbTracker.AllVerbs[i]);
                                    hasRangedVerb = true;
                                }
                            }
                            //Log.Warning("Added Ranged Verb");
                        }
                    }
                }
                if (hasRangedVerb == false)
                {
                    Log.Warning("I don't have range verb");
                    return true;
                }
            }
            // this.SetCurMeleeVerb(updatedAvailableVerbsList.RandomElementByWeight((VerbEntry ve) => ve.SelectionWeight).verb);

            //    Log.Warning(string.Format("rangeVerbs: {0}", rangeList.Count));
            Verb rangeVerb;
            if (rangeList.Count > 1)
            {
                rangeVerb = rangeList.RandomElementByWeightWithDefault((Verb rangeItem) => rangeItem.verbProps.commonality, 0.5f);
            }
            else if (rangeList.Count == 1)
            {
                rangeVerb = rangeList.First();
            }
            else
            {
                rangeVerb = null;
            }

            if (rangeVerb == null)
            {
                //    Log.Warning("Can't get random range verb");
                return true;
            }
            else
            {

                //    Log.Warning(string.Format("rangeVerb: {0}, Range Max: {1}, Min: {2}, Burst: {3}, Projectile: {4}", rangeVerb.verbProps.label, rangeVerb.verbProps.range, rangeVerb.verbProps.minRange, rangeVerb.verbProps.burstShotCount, rangeVerb.verbProps.defaultProjectile));

            }


            Thing enemyTarget = (Thing)AttackTargetFinder.BestAttackTarget((IAttackTargetSearcher)pawn, TargetScanFlags.NeedThreat, (Predicate<Thing>)(x =>
            x is Pawn || x is Building), 0.0f, rangeVerb.verbProps.range, new IntVec3(), float.MaxValue, false);


            if (enemyTarget == null)
            {
                //    Log.Warning("I can't find anything to fight.");
                return true;
            }
            Rand.PushState();
            bool useranged = Rand.Chance(rangeVerb.verbProps.commonality);
            int expiryInterval = Rand.Range(420, 900);
            Rand.PopState();
            //    Log.Warning(string.Format("useranged: {0}", useranged));
            if (!useranged)
            {
                //If adjacent melee attack
                if (enemyTarget.Position.AdjacentTo8Way(pawn.Position))
                {
                    __result = new Job(JobDefOf.AttackMelee, enemyTarget)
                    {
                        maxNumMeleeAttacks = 1,
                        expiryInterval = expiryInterval,
                        attackDoorIfTargetLost = false
                    };
                    return false;
                }
                //Only go if I am to be released. This prevent animal running off.
                if (pawn.CanReach(enemyTarget, PathEndMode.Touch, Danger.Deadly, false))
                {
                    //    Log.Warning("Melee Attack");
                    __result = new Job(JobDefOf.AttackMelee, enemyTarget)
                    {
                        maxNumMeleeAttacks = 1,
                        expiryInterval = expiryInterval,
                        attackDoorIfTargetLost = false
                    };
                    return false;
                }
                else
                {
                    return true;
                }
            }

            //Log.Warning("got list of ranged verb");

            //    Log.Warning("Attempting flag");
            bool flag1 = (double)CoverUtility.CalculateOverallBlockChance(pawn.Position, enemyTarget.Position, pawn.Map) > 0.00999999977648258;
            bool flag2 = pawn.Position.Standable(pawn.Map);
            bool flag3 = rangeVerb.CanHitTarget(enemyTarget);
            bool flag4 = (pawn.Position - enemyTarget.Position).LengthHorizontalSquared < 25;



            if (flag1 && flag2 && flag3 || flag4 && flag3)
            {
                //    Log.Warning("Shooting");
                __result = new Job(DefDatabase<JobDef>.GetNamed("AnimalRangeAttack"), enemyTarget, JobGiver_AIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange, true)
                {
                    verbToUse = rangeVerb

                };
                return false;
            }
            bool canShootCondition = false;
            //    Log.Warning("Try casting");

            //Animals with training seek cover
            /*
				if (pawn.training.IsCompleted(TrainableDefOf.Release) && (double)verb.verbProps.range > 7.0)
					Log.Warning("Attempting cover");
				Log.Warning("Try get flag radius :" + Traverse.Create(__instance).Method("GetFlagRadius", pawn).GetValue<float>());
				Log.Warning("Checking cast condition");
				*/

            //Don't find new position if animal not released.


            canShootCondition = CastPositionFinder.TryFindCastPosition(new CastPositionRequest
            {
                caster = pawn,
                target = enemyTarget,
                verb = rangeVerb,
                maxRangeFromTarget = rangeVerb.verbProps.range,
                wantCoverFromTarget = (double)rangeVerb.verbProps.range > 7.0,
                locus = pawn.Position,
                maxRangeFromLocus = Traverse.Create(__instance).Method("GetFlagRadius", pawn).GetValue<float>(),
                maxRegions = 50
            }, out IntVec3 dest);

            if (!canShootCondition)
            {
                //   Log.Warning("I can't move to shooting position");


                return true;
            }

            if (dest == pawn.Position)
            {
                //   Log.Warning("I will stay here and attack");
                __result = new Job(DefDatabase<JobDef>.GetNamed("AnimalRangeAttack"), enemyTarget, JobGiver_AIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange, true)
                {
                    verbToUse = rangeVerb
                };
                return false;
            }
            //   Log.Warning("Going to new place");
            __result = new Job(JobDefOf.Goto, (LocalTargetInfo)dest)
            {
                expiryInterval = JobGiver_AIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange,
                checkOverrideOnExpire = true
            };
            return false;
        }
        public static bool AnimalUser(ref JobGiver_AIFightEnemy __instance, ref Job __result, ref Pawn pawn, bool player)
        {
            bool hasRangedVerb = false;
            List<Verb> verbList = pawn.verbTracker.AllVerbs;
            List<Verb> rangeList = new List<Verb>();
            for (int i = 0; i < verbList.Count; i++)
            {
                //    Log.Warning("Checkity");
                //    Log.Warning(string.Format("verbList: {0}, Name: {1} RangeMax: {2}", i, verbList[i].verbProps.label, verbList[i].verbProps.range));
                //It corresponds with verbs anyway
                if (verbList[i].verbProps.range > 1.5f)
                {
                    rangeList.Add(verbList[i]);
                    hasRangedVerb = true;
                }
                //Log.Warning("Added Ranged Verb");
            }
            if (pawn.equipment?.PrimaryEq != null)
            {
                for (int i = 0; i < pawn.equipment.PrimaryEq.AllVerbs.Count; i++)
                {
                    //    Log.Warning("Checkity");
                    //    Log.Warning(string.Format("verbList: {0}, Name: {1} RangeMax: {2}", i, verbList[i].verbProps.label, verbList[i].verbProps.range));
                    //It corresponds with verbs anyway
                    if (pawn.equipment.PrimaryEq.AllVerbs[i].verbProps.range > 1.5f)
                    {
                        if (!rangeList.Contains(pawn.equipment.PrimaryEq.AllVerbs[i]))
                        {
                            rangeList.Add(pawn.equipment.PrimaryEq.AllVerbs[i]);
                            hasRangedVerb = true;
                        }
                    }
                    //Log.Warning("Added Ranged Verb");
                }
            }
            if (pawn.health.hediffSet.hediffs.Any(x => x.TryGetCompFast<HediffComp_VerbGiverExtra>() != null))
            {
                List<Hediff> list = pawn.health.hediffSet.hediffs.FindAll(x => x.TryGetCompFast<HediffComp_VerbGiverExtra>() != null);

                foreach (Hediff item in list)
                {
                    HediffComp_VerbGiverExtra _VerbGiver = item.TryGetCompFast<HediffComp_VerbGiverExtra>();
                    if (_VerbGiver != null)
                    {
                        for (int i = 0; i < _VerbGiver.verbTracker.AllVerbs.Count; i++)
                        {
                            //    Log.Warning("Checkity");
                            //    Log.Warning(string.Format("verbList: {0}, Name: {1} RangeMax: {2}", i, verbList[i].verbProps.label, verbList[i].verbProps.range));
                            //It corresponds with verbs anyway
                            if (_VerbGiver.verbTracker.AllVerbs[i].verbProps.range > 1.5f)
                            {
                                if (!rangeList.Contains(_VerbGiver.verbTracker.AllVerbs[i]))
                                {
                                    rangeList.Add(_VerbGiver.verbTracker.AllVerbs[i]);
                                    hasRangedVerb = true;
                                }
                            }
                            //Log.Warning("Added Ranged Verb");
                        }
                    }
                }
                if (hasRangedVerb == false)
                {
                    Log.Warning("I don't have range verb");
                    return true;
                }
            }
            // this.SetCurMeleeVerb(updatedAvailableVerbsList.RandomElementByWeight((VerbEntry ve) => ve.SelectionWeight).verb);

            //    Log.Warning(string.Format("rangeVerbs: {0}", rangeList.Count));
            Verb rangeVerb;
            if (rangeList.Count > 1)
            {
                rangeVerb = rangeList.RandomElementByWeightWithDefault((Verb rangeItem) => rangeItem.verbProps.commonality, 0.5f);
            }
            else if (rangeList.Count == 1)
            {
                rangeVerb = rangeList.First();
            }
            else
            {
                rangeVerb = null;
            }

            if (rangeVerb == null)
            {
                //    Log.Warning("Can't get random range verb");
                return true;
            }
            else
            {

                //    Log.Warning(string.Format("rangeVerb: {0}, Range Max: {1}, Min: {2}, Burst: {3}, Projectile: {4}", rangeVerb.verbProps.label, rangeVerb.verbProps.range, rangeVerb.verbProps.minRange, rangeVerb.verbProps.burstShotCount, rangeVerb.verbProps.defaultProjectile));

            }


            Thing enemyTarget = (Thing)AttackTargetFinder.BestAttackTarget((IAttackTargetSearcher)pawn, TargetScanFlags.NeedThreat, (Predicate<Thing>)(x =>
            x is Pawn || x is Building), 0.0f, rangeVerb.verbProps.range, new IntVec3(), float.MaxValue, false);


            if (enemyTarget == null)
            {
                //    Log.Warning("I can't find anything to fight.");
                return true;
            }
            Rand.PushState();
            bool useranged = Rand.Chance(rangeVerb.verbProps.commonality);
            int expiryInterval = Rand.Range(420, 900);
            Rand.PopState();
            //    Log.Warning(string.Format("useranged: {0}", useranged));
            if (!useranged)
            {
                //If adjacent melee attack
                if (enemyTarget.Position.AdjacentTo8Way(pawn.Position))
                {
                    __result = new Job(JobDefOf.AttackMelee, enemyTarget)
                    {
                        maxNumMeleeAttacks = 1,
                        expiryInterval = expiryInterval,
                        attackDoorIfTargetLost = false
                    };
                    return false;
                }
                //Only go if I am to be released. This prevent animal running off.
                if (pawn.CanReach(enemyTarget, PathEndMode.Touch, Danger.Deadly, false))
                {
                    //    Log.Warning("Melee Attack");
                    __result = new Job(JobDefOf.AttackMelee, enemyTarget)
                    {
                        maxNumMeleeAttacks = 1,
                        expiryInterval = expiryInterval,
                        attackDoorIfTargetLost = false
                    };
                    return false;
                }
                else
                {
                    return true;
                }
            }

            //Log.Warning("got list of ranged verb");

            //    Log.Warning("Attempting flag");
            bool flag1 = (double)CoverUtility.CalculateOverallBlockChance(pawn.Position, enemyTarget.Position, pawn.Map) > 0.00999999977648258;
            bool flag2 = pawn.Position.Standable(pawn.Map);
            bool flag3 = rangeVerb.CanHitTarget(enemyTarget);
            bool flag4 = (pawn.Position - enemyTarget.Position).LengthHorizontalSquared < 25;



            if (flag1 && flag2 && flag3 || flag4 && flag3)
            {
                //    Log.Warning("Shooting");
                __result = new Job(DefDatabase<JobDef>.GetNamed("AnimalRangeAttack"), enemyTarget, JobGiver_AIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange, true)
                {
                    verbToUse = rangeVerb

                };
                return false;
            }
            bool canShootCondition = false;
            //    Log.Warning("Try casting");

            //Animals with training seek cover
            /*
				if (pawn.training.IsCompleted(TrainableDefOf.Release) && (double)verb.verbProps.range > 7.0)
					Log.Warning("Attempting cover");
				Log.Warning("Try get flag radius :" + Traverse.Create(__instance).Method("GetFlagRadius", pawn).GetValue<float>());
				Log.Warning("Checking cast condition");
				*/

            //Don't find new position if animal not released.


            canShootCondition = CastPositionFinder.TryFindCastPosition(new CastPositionRequest
            {
                caster = pawn,
                target = enemyTarget,
                verb = rangeVerb,
                maxRangeFromTarget = rangeVerb.verbProps.range,
                wantCoverFromTarget = (double)rangeVerb.verbProps.range > 7.0,
                locus = pawn.Position,
                maxRangeFromLocus = Traverse.Create(__instance).Method("GetFlagRadius", pawn).GetValue<float>(),
                maxRegions = 50
            }, out IntVec3 dest);

            if (!canShootCondition)
            {
                //   Log.Warning("I can't move to shooting position");


                return true;
            }

            if (dest == pawn.Position)
            {
                //   Log.Warning("I will stay here and attack");
                __result = new Job(DefDatabase<JobDef>.GetNamed("AnimalRangeAttack"), enemyTarget, JobGiver_AIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange, true)
                {
                    verbToUse = rangeVerb
                };
                return false;
            }
            //   Log.Warning("Going to new place");
            __result = new Job(JobDefOf.Goto, (LocalTargetInfo)dest)
            {
                expiryInterval = JobGiver_AIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange,
                checkOverrideOnExpire = true
            };
            return false;
        }
    }
}
