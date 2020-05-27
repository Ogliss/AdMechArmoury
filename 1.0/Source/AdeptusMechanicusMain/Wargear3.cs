using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x02000002 RID: 2
    public class Wargear3 : Apparel
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        public override IEnumerable<Gizmo> GetWornGizmos()
        {
            bool flag = Find.Selector.SingleSelectedThing == base.Wearer;
            string logroll = string.Format("flag {0} base.Wearer {1}", flag, base.Wearer);
            Log.Message(logroll);
            if (flag)
            {
                Thing ownerThing = this;
                List<Verb> verbs = this.Wearer.VerbTracker.AllVerbs;
                logroll = string.Format("ownerThing {0} verbs {1}", ownerThing, verbs);
                Log.Message(logroll);
                for (int i = 0; i < verbs.Count; i++)
			    {
				    Verb verb = verbs[i];
                    logroll = string.Format("verb {0} verbs[i] {1}", verb, verbs[i]);
                    Log.Message(logroll);
                    if (verb.verbProps.hasStandardCommand)
                    {
                        logroll = string.Format("verb {0} verbs[i] {1} verb.verbProps.hasStandardCommand {2}", verb, verbs[i], verb.verbProps.hasStandardCommand);
                        Log.Message(logroll);
                        logroll = string.Format("giving ownerThing {0} verb {1}", ownerThing, verb);
                        Log.Message(logroll);
                        yield return this.CreateVerbTargetCommand(ownerThing, verb);
				    }
			    }
            }
            yield break;
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
