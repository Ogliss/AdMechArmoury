using RimWorld;
using Verse.AI;
using Verse;
using System.Collections.Generic;
using System.Security.Cryptography;
using RimWorld.Utility;
using UnityEngine;
using Verse.Sound;

namespace AdeptusMechanicus
{
    public class CompAbilityEffect_Teleport : CompAbilityEffect
    {
        public new CompProperties_AbilityTeleport Props
        {
            get
            {
                return (CompProperties_AbilityTeleport)this.props;
            }
        }
        /*
        public override IEnumerable<PreCastAction> GetPreCastActions()
        {
            yield return new PreCastAction
            {
                action = delegate (LocalTargetInfo t, LocalTargetInfo d)
                {
                    if (!this.parent.def.HasAreaOfEffect)
                    {
                        Pawn pawn = t.Pawn;
                        if (pawn != null)
                        {
                            FleckCreationData dataAttachedOverlay = FleckMaker.GetDataAttachedOverlay(pawn, FleckDefOf.PsycastSkipFlashEntry, new Vector3(-0.5f, 0f, -0.5f), 1f, -1f);
                            dataAttachedOverlay.link.detachAfterTicks = 5;
                            pawn.Map.flecks.CreateFleck(dataAttachedOverlay);
                        }
                        else
                        {
                            FleckMaker.Static(t.CenterVector3, this.parent.pawn.Map, FleckDefOf.PsycastSkipFlashEntry, 1f);
                        }
                        FleckMaker.Static(d.Cell, this.parent.pawn.Map, FleckDefOf.PsycastSkipInnerExit, 1f);
                    }
                    if (this.Props.destination != AbilityEffectDestination.RandomInRange)
                    {
                        FleckMaker.Static(d.Cell, this.parent.pawn.Map, FleckDefOf.PsycastSkipOuterRingExit, 1f);
                    }
                    if (!this.parent.def.HasAreaOfEffect)
                    {
                        SoundDefOf.Psycast_Skip_Entry.PlayOneShot(new TargetInfo(t.Cell, this.parent.pawn.Map, false));
                        SoundDefOf.Psycast_Skip_Exit.PlayOneShot(new TargetInfo(d.Cell, this.parent.pawn.Map, false));
                    }
                },
                ticksAwayFromCast = 5
            };
            yield break;
        }
        */

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (target.IsValid)
            {
            //    base.Apply(target, dest);
            //    LocalTargetInfo destination = base.GetDestination(dest.IsValid ? dest : target);
            /*
                if (destination.IsValid)
                {
                }
                */
                Pawn pawn = this.parent.pawn;
                if (!this.parent.def.HasAreaOfEffect)
                {
                    this.parent.AddEffecterToMaintain(EffecterDefOf.Skip_Entry.Spawn(pawn, pawn.Map, 1f), pawn.Position, 60, null);
                }
                else
                {
                    this.parent.AddEffecterToMaintain(EffecterDefOf.Skip_EntryNoDelay.Spawn(pawn, pawn.Map, 1f), pawn.Position, 60, null);
                }
                if (this.Props.destination == AbilityEffectDestination.Selected)
                {
                    this.parent.AddEffecterToMaintain(EffecterDefOf.Skip_Exit.Spawn(target.Cell, pawn.Map, 1f), target.Cell, 60, null);
                }
                else
                {
                    this.parent.AddEffecterToMaintain(EffecterDefOf.Skip_ExitNoDelay.Spawn(target.Cell, pawn.Map, 1f), target.Cell, 60, null);
                }
                pawn.Position = target.Cell;
                if ((pawn.Faction == Faction.OfPlayer || pawn.IsPlayerControlled) && pawn.Position.Fogged(pawn.Map))
                {
                    FloodFillerFog.FloodUnfog(pawn.Position, pawn.Map);
                }
                pawn.stances.stunner.StunFor(this.Props.stunTicks.RandomInRange, this.parent.pawn, false, false, false);
                pawn.Notify_Teleported(true, true);
                CompAbilityEffect_Teleport.SendSkipUsedSignal(pawn.Position, pawn);
                if (this.Props.destClamorType != null)
                {
                    GenClamor.DoClamor(pawn, target.Cell, (float)this.Props.destClamorRadius, this.Props.destClamorType);
                }
            }
        }
        /*
        public override bool CanHitTarget(LocalTargetInfo target)
        {
            return base.CanPlaceSelectedTargetAt(target) && base.CanHitTarget(target);
        }
        */
        /*
        public override bool Valid(LocalTargetInfo target, bool showMessages = true)
        {
            AcceptanceReport report = this.CanSkipTarget(target);
            if (!report)
            {
                Pawn pawn;
                if (showMessages && !report.Reason.NullOrEmpty() && (pawn = (target.Thing as Pawn)) != null)
                {
                    Messages.Message("CannotSkipTarget".Translate(pawn.Named("PAWN")) + ": " + report.Reason, pawn, MessageTypeDefOf.RejectInput, false);
                }
                return false;
            }
            return base.Valid(target, showMessages);
        }
        */
        private AcceptanceReport CanSkipTarget(LocalTargetInfo target)
        {
            Pawn pawn;
            if ((pawn = (target.Thing as Pawn)) != null)
            {
                if (pawn.BodySize > this.Props.maxBodySize)
                {
                    return "CannotSkipTargetTooLarge".Translate();
                }
                if (pawn.kindDef.skipResistant)
                {
                    return "CannotSkipTargetPsychicResistant".Translate();
                }
            }
            return true;
        }

        public override string ExtraLabelMouseAttachment(LocalTargetInfo target)
        {
            return this.CanSkipTarget(target).Reason;
        }

        public static void SendSkipUsedSignal(LocalTargetInfo target, Thing initiator)
        {
            Find.SignalManager.SendSignal(new Signal(CompAbilityEffect_Teleport.SkipUsedSignalTag, target.Named("POSITION"), initiator.Named("SUBJECT")));
        }

        public static string SkipUsedSignalTag = "CompAbilityEffect.SkipUsed";
    }

    // AdeptusMechanicus.JobGiver_AIJumpToShootingPosition
    public class JobGiver_AIJumpToShootingPosition : ThinkNode_JobGiver
    {
        public override Job TryGiveJob(Pawn pawn)
        {
            Thing enemyTarget = pawn.mindState.enemyTarget;
            if (enemyTarget == null)
            {
                return null;
            }
            Pawn_AbilityTracker abilities = pawn.abilities;
            Ability ability = (abilities != null) ? abilities.GetAbility(this.ability, false) : null;
            if (ability == null || !ability.CanCast)
                return null;
            Job curJob = pawn.CurJob;
            if (curJob == null || curJob.def == this.ability.jobDef)
                return null;
            Verb attackVerb = pawn.VerbTracker.PrimaryVerb;
            bool attackVerbLOS = attackVerb.verbProps.requireLineOfSight;
            LocalTargetInfo target = curJob.GetTarget(this.targetIndex);
            if (!this.CanJumpToTarget(pawn, target))
                return null;
            IntVec3 cell = target.Cell;
            float num = pawn.Position.DistanceTo(cell);
            if (num < attackVerb.verbProps.range && (!attackVerbLOS || GenSight.LineOfSight(pawn.Position, cell, pawn.Map))) return null; 
            VerbProperties verbProps = ability.verb.verbProps;
            bool abilityLOS = verbProps.requireLineOfSight;
            if (num < verbProps.minRange || num > verbProps.range + attackVerb.verbProps.range || (abilityLOS && !GenSight.LineOfSight(pawn.Position, cell, pawn.Map)))
                return null;
            if (target.HasThing && !this.TryFindShootingPosition(pawn, target.Thing, out cell, attackVerb))
                return null;
            num = pawn.Position.DistanceTo(cell);
            if (num < verbProps.minRange || num > verbProps.range || (abilityLOS && !GenSight.LineOfSight(pawn.Position, cell, pawn.Map)))
                return null;
            if (ability.verb.ValidateTarget(target, false))
            {
                Job job = ability.GetJob(cell, cell);
                pawn.jobs.StartJob(job, JobCondition.InterruptForced, null, true, true, null, null, false, false, null, false, true, false);
                FleckMaker.Static(cell, pawn.Map, FleckDefOf.FeedbackGoto, 1f);
            }
            return null;
        }

        public virtual bool TryFindShootingPosition(Pawn pawn, Thing enemyTarget, out IntVec3 dest, Verb verbToUse = null)
		{
		//	Thing enemyTarget = pawn.mindState.enemyTarget;
			bool allowManualCastWeapons = !pawn.IsColonist && !pawn.IsColonyMutant;
			Verb verb = verbToUse ?? pawn.TryGetAttackVerb(enemyTarget, allowManualCastWeapons);
			if (verb == null)
			{
				dest = IntVec3.Invalid;
				return false;
			}
			return CastPositionFinder.TryFindCastPosition(new CastPositionRequest
			{
				caster = pawn,
				target = enemyTarget,
				verb = verb,
				maxRangeFromTarget = verb.verbProps.range,
				wantCoverFromTarget = (verb.verbProps.range > 5f)
			}, out dest);
		}

        public virtual bool CanJumpToTarget(Pawn pawn, LocalTargetInfo target)
        {
            return true;
        }

        public AbilityDef ability;

        public TargetIndex targetIndex = TargetIndex.A;
    }

    // AdeptusMechanicus.JobGiver_AIMultiAbilityFight
    class JobGiver_AIMultiAbilityFight : JobGiver_AIFightEnemy
    {
        public override bool OnlyUseAbilityVerbs
        {
            get
            {
                return true;
            }
        }

        public override bool OnlyUseRangedSearch
        {
            get
            {
                return true;
            }
        }

        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_AIMultiAbilityFight jobGiver_AIAbilityFight = (JobGiver_AIMultiAbilityFight)base.DeepCopy(resolve);
            jobGiver_AIAbilityFight.abilities = this.abilities;
            jobGiver_AIAbilityFight.skipIfCantTargetNow = this.skipIfCantTargetNow;
            jobGiver_AIAbilityFight.includeTemporary = this.includeTemporary;
            return jobGiver_AIAbilityFight;
        }

        public override bool TryFindShootingPosition(Pawn pawn, out IntVec3 dest, Verb verbToUse = null)
        {
            dest = IntVec3.Invalid;
            Thing enemyTarget = pawn.mindState.enemyTarget;
            if (pawn.abilities == null) return false;
            Ability ability = pawn.abilities.GetAbility(this.ability, false);
            return CastPositionFinder.TryFindCastPosition(new CastPositionRequest
            {
                caster = pawn,
                target = enemyTarget,
                verb = ability.verb,
                maxRangeFromTarget = ability.verb.verbProps.range,
                wantCoverFromTarget = false,
                preferredCastPosition = new IntVec3?(pawn.Position)
            }, out dest);
        }

        public override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.abilities == null) return null;
            bool anyReady = false;
            foreach (var item in abilities)
            {
                if ((bool)!pawn.abilities?.GetAbility(item, includeTemporary)?.OnCooldown) anyReady = true;
            }
            if (!anyReady && this.skipIfCantTargetNow) return null;
            return base.TryGiveJob(pawn);
        }

        public override bool ShouldLoseTarget(Pawn pawn)
        {
            return base.ShouldLoseTarget(pawn) || !this.CanTarget(pawn, pawn.mindState.enemyTarget);
        }

        public override bool ExtraTargetValidator(Pawn pawn, Thing target)
        {
            return base.ExtraTargetValidator(pawn, target) && this.CanTarget(pawn, target);
        }

        private bool CanTarget(Pawn pawn, Thing target)
        {
            if (pawn.abilities == null) return false;
            bool anyReady = false;

            List<AbilityDef> ready = new List<AbilityDef>();
            foreach (var item in abilities)
            {
                if (item.verbProperties.targetParams.CanTarget(target, null))
                {
                    ready.Add(item);    
                    anyReady = true;
                }
            }
            if (!anyReady)
            {
                return false;
            }
            this.ability = ready.RandomElementByWeight(x => x.verbProperties.commonality + x.verbProperties.range);
            Ability ability = pawn.abilities.GetAbility(this.ability, includeTemporary);
            return ability.CanApplyOn((LocalTargetInfo)target) && (!this.skipIfCantTargetNow || ability.AICanTargetNow(target));
        }

        private AbilityDef ability;
        private List<AbilityDef> abilities;

        private bool skipIfCantTargetNow = true;
        private bool includeTemporary = false;
    }
}