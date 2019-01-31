using RimWorld;
using UnityEngine;
using System.Collections.Generic;
using Verse;
using System.Diagnostics;
using System;
using System.Linq;

namespace AdeptusMechanicus
{

    public class CompProperties_SecondryFire : CompProperties
    {
        // Token: 0x04003403 RID: 13315
        public List<VerbProperties> verbs;

        public Verb VerbClass;

        public bool isPrimary;

        public bool hasStandardCommand;

        public float range;

        public float forcedMissRadius;

        public float warmupTime;

        public float noiseRadius;

        public bool ai_isBuildingDestroyer;

        public SoundDef soundCast;

        public bool canTargetLocations;

        public Projectile defaultProjectile;

        public RulePackDef rangedFireRulepack;


        // Token: 0x04000007 RID: 7
        public int Uses = 20;

        public CompProperties_SecondryFire()
        {
            this.compClass = typeof(CompSecondryFire);
        }
    }

    public class CompSecondryFire : CompWearable
    {

        public CompProperties_SecondryFire Props => (CompProperties_SecondryFire)props;

        // Determine who is wearing this ThingComp. Returns a Pawn or null.
        protected virtual Pawn GetWearer
        {
            get
            {
                if (ParentHolder != null && ParentHolder is Pawn_ApparelTracker)
                {
                    return (Pawn)ParentHolder.ParentHolder;
                }
                else
                {
                    return null;
                }
            }
        }

        // Determine who is wearing this ThingComp. Returns a Pawn or null.
        protected virtual Verb verb
        {
            get
            {
                if (ParentHolder != null && ParentHolder is Pawn_ApparelTracker)
                {
                    return (Verb)GetWearer.VerbTracker.PrimaryVerb;
                   // return GetWearer.equipment.AllEquipmentVerbs.Named();
                    //return GetWearer.apparel.pawn.equipment.AllEquipmentVerbs.Named("throw grenade");
                }
                else
                {
                    return null;
                }
            }
        }

        // Determine if this ThingComp is being worn presently. Returns True/False
        protected virtual bool IsWorn => (GetWearer != null);


        public override IEnumerable<Gizmo> CompGetGizmosWorn()
        {
            yield return this.CreateVerbTargetCommand(GetWearer, verb);
            /*
            yield return new Command_Action
            {
               // action = Detonate,
                defaultLabel = "WearableExplosives_Label".Translate(),
                defaultDesc = "WearableExplosives_Desc".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Buttons/BlastFlame", true)
            };
            */
        }
        private Command_VerbTarget CreateVerbTargetCommand(Thing ownerThing, Verb verb)
        {
            Command_VerbTarget command_VerbTarget = new Command_VerbTarget();
            command_VerbTarget.defaultDesc = ownerThing.LabelCap + ": " + ownerThing.def.description.CapitalizeFirst();
            command_VerbTarget.icon = ownerThing.def.uiIcon;
            command_VerbTarget.iconAngle = ownerThing.def.uiIconAngle;
            command_VerbTarget.iconOffset = ownerThing.def.uiIconOffset;
            command_VerbTarget.tutorTag = "VerbTarget";
            command_VerbTarget.verb = verb;
            if (verb.caster.Faction != Faction.OfPlayer)
            {
                command_VerbTarget.Disable("CannotOrderNonControlled".Translate());
            }
            else if (verb.CasterIsPawn)
            {
                if (verb.CasterPawn.story.WorkTagIsDisabled(WorkTags.Violent))
                {
                    command_VerbTarget.Disable("IsIncapableOfViolence".Translate(verb.CasterPawn.LabelShort, verb.CasterPawn));
                }
                else if (!verb.CasterPawn.drafter.Drafted)
                {
                    command_VerbTarget.Disable("IsNotDrafted".Translate(verb.CasterPawn.LabelShort, verb.CasterPawn));
                }
            }
            return command_VerbTarget;
        }
    }
}