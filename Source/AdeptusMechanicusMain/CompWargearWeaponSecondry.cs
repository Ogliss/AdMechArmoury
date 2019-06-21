using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    public class CompProperties_WargearWeaponSecondry : CompProperties
    {
        public CompProperties_WargearWeaponSecondry()
        {
            this.compClass = typeof(CompWargearWeaponSecondry);
        }
    }

    // Token: 0x02000002 RID: 2
    public class CompWargearWeaponSecondry : CompWargearWeapon
    {
        public CompProperties_WargearWeaponSecondry Props => (CompProperties_WargearWeaponSecondry)props;

        public Verb verb;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
        }


        public override IEnumerable<Gizmo> EquippedGizmos()
        {
            ThingWithComps owner = IsWorn ? GetWearer : parent;
            bool flag = Find.Selector.SingleSelectedThing == GetWearer;
         //   Log.Message(string.Format("0"));
            if (flag)
            {
                CompEquippable c = parent.GetComp<CompEquippable>();
                //   Log.Message(string.Format("there are {0} verbs", c.verbTracker.AllVerbs.Count));
                // Verb verbA = parent.def.Verbs;
                foreach (Verb v in c.verbTracker.AllVerbs)
                {
                 //   Log.Message(string.Format("v fires {0} {1}", v.verbProps.burstShotCount, v.verbProps.defaultProjectile.label));
                    if (v!=v.verbTracker.PrimaryVerb&&!v.IsMeleeAttack)
                    {
                        //   Log.Message(string.Format("selected v fires {0} {1}", v.verbProps.burstShotCount, v.verbProps.defaultProjectile.label));

                        verb = v;
                        Command_VerbTarget command_VerbTarget = new Command_VerbTarget();
                        command_VerbTarget.defaultDesc = parent.LabelCap + ": " + parent.def.description.CapitalizeFirst();
                        command_VerbTarget.icon = parent.def.uiIcon;
                        command_VerbTarget.iconAngle = parent.def.uiIconAngle;
                        command_VerbTarget.iconOffset = parent.def.uiIconOffset;
                        command_VerbTarget.tutorTag = "VerbTarget";
                        command_VerbTarget.verb = verb;
                        command_VerbTarget.verb.verbProps = verb.verbProps;
                        command_VerbTarget.verb.verbProps.defaultProjectile = verb.verbProps.defaultProjectile;
                        command_VerbTarget.verb.verbProps.defaultCooldownTime = verb.verbProps.defaultCooldownTime;
                        command_VerbTarget.verb.verbProps.burstShotCount = verb.verbProps.burstShotCount;
                        command_VerbTarget.verb.verbProps.verbClass = verb.verbProps.verbClass;
                        command_VerbTarget.verb.verbProps.warmupTime = verb.verbProps.warmupTime;
                        command_VerbTarget.verb.verbProps.ticksBetweenBurstShots = verb.verbProps.ticksBetweenBurstShots;
                        command_VerbTarget.verb.verbProps.soundCast = verb.verbProps.soundCast;
                        command_VerbTarget.defaultLabel = "Fire: " + parent.def.label;
                        command_VerbTarget.activateSound = SoundDef.Named("Click");
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
                        yield return command_VerbTarget;
                    }
                    else
                    {
                        Command_Action command_Action = new Command_Action();
                        command_Action.defaultLabel = parent.def.label;
                        command_Action.defaultDesc = "This colonist is equipped with a " + parent.def.label;
                        command_Action.hotKey = KeyBindingDefOf.Misc2;
                        command_Action.icon = parent.def.uiIcon;
                        command_Action.disabled = true;
                        yield return command_Action;
                    }
                }
            }
            yield break;
        }

        public override void CompTick()
        {
            base.CompTick();
            if (GetWearer != lastWearer)
            {
                lastWearer = GetWearer;
            }
        }
    }
}
