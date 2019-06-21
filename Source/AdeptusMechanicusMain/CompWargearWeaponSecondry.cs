using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public class CompProperties_WargearWeaponSecondry : CompProperties
    {
        public CompProperties_WargearWeaponSecondry()
        {
            this.compClass = typeof(CompWargearWeaponSecondry);
        }
        public List<Verb> Verblist = new List<Verb>();
    }

    // Token: 0x02000002 RID: 2
    public class CompWargearWeaponSecondry : CompWargearWeapon
    {
        public CompProperties_WargearWeaponSecondry Props => (CompProperties_WargearWeaponSecondry)props;
        public Verb Castverb;
        public int Vi = 1;
        public List<Verb> Verblist = new List<Verb>();
        public int lastShotTick = 0;

        public CompEquippable CompEq
        {
            get
            {
                return parent.GetComp<CompEquippable>();
            }
        }

        public Verb DefaultVerb
        {
            get
            {
                return CompEq.VerbTracker.PrimaryVerb;
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
        //    Castverb = DefaultVerb;
        }
        // Token: 0x060064D9 RID: 25817 RVA: 0x002D266C File Offset: 0x002D0A6C
        public IEnumerable<Command> GetVerbsCommands(KeyCode hotKey = KeyCode.None)
        {
            CompEquippable ce = parent.GetComp<CompEquippable>();
            if (ce == null)
            {
                yield break;
            }
            Thing ownerThing = ce.parent;
            List<Verb> verbs = ce.AllVerbs;
            for (int i = 1; i < verbs.Count; i++)
            {
                Verb verb = verbs[i];
                Vi = i;
                if (verb.verbProps.hasStandardCommand)
                {

                    yield return this.CreateVerbTargetCommand(ownerThing, verb);
                }
            }
            yield break;
        }

        // Token: 0x060064DA RID: 25818 RVA: 0x002D2690 File Offset: 0x002D0A90
        private Command_VerblikeTarget CreateVerbTargetCommand(Thing ownerThing, Verb verb)
        {

            Command_VerblikeTarget command_VerbTarget = new Command_VerblikeTarget();
            command_VerbTarget.defaultDesc = verb.verbProps.defaultProjectile.description.CapitalizeFirst();
            command_VerbTarget.icon = parent.def.uiIcon;
            command_VerbTarget.iconAngle = parent.def.uiIconAngle;
            command_VerbTarget.iconOffset = parent.def.uiIconOffset;
            command_VerbTarget.tutorTag = "VerbTarget";
            command_VerbTarget.verb = (Verb_ShootOG)verb;
                command_VerbTarget.action = new Action(SwitchFireMode);

            /*
            command_VerbTarget.verb.verbProps = verb.verbProps;
            command_VerbTarget.verb.verbProps.isPrimary = true;
            command_VerbTarget.verb.verbProps.defaultProjectile = verb.verbProps.defaultProjectile;
            command_VerbTarget.verb.verbProps.defaultCooldownTime = verb.verbProps.defaultCooldownTime;
            command_VerbTarget.verb.verbProps.burstShotCount = verb.verbProps.burstShotCount;
            command_VerbTarget.verb.verbProps.verbClass = verb.verbProps.verbClass;
            command_VerbTarget.verb.verbProps.warmupTime = verb.verbProps.warmupTime;
            command_VerbTarget.verb.verbProps.ticksBetweenBurstShots = verb.verbProps.ticksBetweenBurstShots;
            command_VerbTarget.verb.verbProps.soundCast = verb.verbProps.soundCast;
            */
            command_VerbTarget.defaultLabel = "Fire: " + verb.verbProps.label;
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
                else if (GetWearer.stances.curStance.GetType() == typeof(Stance_Warmup))
                {
                    command_VerbTarget.Disable("IsNotDrafted".Translate(verb.CasterPawn.LabelShort, verb.CasterPawn));
                }
            }
            return command_VerbTarget;
        }

        // Token: 0x060064DB RID: 25819 RVA: 0x002D27CC File Offset: 0x002D0BCC
        public Verb GetVerb(VerbCategory category)
        {
            CompEquippable c = parent.GetComp<CompEquippable>();
            List<Verb> allVerbs = c.verbTracker.AllVerbs;
            if (allVerbs != null)
            {
                for (int i = 0; i < allVerbs.Count; i++)
                {
                    if (allVerbs[i].verbProps.category == category)
                    {
                        return allVerbs[i];
                    }
                }
            }
            return null;
        }
        
        public override IEnumerable<Gizmo> EquippedGizmos()
        {
            ThingWithComps owner = IsWorn ? GetWearer : parent;
            bool flag = Find.Selector.SingleSelectedThing == GetWearer;
            if (flag && GetWearer.Drafted && GetWearer.Faction == Faction.OfPlayer)
            {
                IEnumerable<Command> commands = GetVerbsCommands(KeyCode.None);
                foreach (Command c in commands)
                {
                    //Log.Message(string.Format("{0}",c.defaultLabel));
                    yield return c;
                }
            }
            yield break;
        }

        public void SwitchFireMode()
        {
            Pawn p = GetWearer;

         //   p.equipment.Primary != null && p.equipment.Primary.def.IsRangedWeapon && p.equipment.PrimaryEq.PrimaryVerb.HarmsHealth() && !p.equipment.PrimaryEq.PrimaryVerb.UsesExplosiveProjectiles()
        //    Log.Message(string.Format("IsRangedWeapon:{0}, HarmsHealth:{1}, label:{2}, !UsesExplosiveProjectiles:{3}", p.equipment.Primary.def.IsRangedWeapon, p.equipment.PrimaryEq.PrimaryVerb.HarmsHealth(), Castverb.GetProjectile().projectile.damageDef.harmsHealth, !p.equipment.PrimaryEq.PrimaryVerb.UsesExplosiveProjectiles()));
        }

        public virtual void Notify_ProjectileLaunched(int shotsleft)
        {
            Log.Message(string.Format("Notify_ProjectileLaunched shotsleft={0}", shotsleft));
            if (shotsleft==1)
            {
                Log.Message(string.Format("Notify_ProjectileLaunched Last shot", shotsleft));
                lastShotTick = Find.TickManager.TicksGame;
                Castverb = DefaultVerb;
            }
        }
        /*
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
                         // Log.Message(string.Format("selected v fires {0} {1}", v.verbProps.burstShotCount, v.verbProps.defaultProjectile.label));

                        verb = v;
                        Command_VerblikeTarget command_VerbTarget = new Command_VerblikeTarget();
                        command_VerbTarget.defaultDesc = parent.LabelCap + ": " + verb.verbProps.defaultProjectile.description.CapitalizeFirst();
                        command_VerbTarget.icon = parent.def.uiIcon;
                        command_VerbTarget.iconAngle = parent.def.uiIconAngle;
                        command_VerbTarget.iconOffset = parent.def.uiIconOffset;
                        command_VerbTarget.tutorTag = "VerbTarget";
                        command_VerbTarget.verb = verb;
                        command_VerbTarget.verb.verbProps = verb.verbProps;
                        command_VerbTarget.verb.verbProps.isPrimary = true;
                        command_VerbTarget.verb.verbProps.defaultProjectile = verb.verbProps.defaultProjectile;
                        command_VerbTarget.verb.verbProps.defaultCooldownTime = verb.verbProps.defaultCooldownTime;
                        command_VerbTarget.verb.verbProps.burstShotCount = verb.verbProps.burstShotCount;
                        command_VerbTarget.verb.verbProps.verbClass = verb.verbProps.verbClass;
                        command_VerbTarget.verb.verbProps.warmupTime = verb.verbProps.warmupTime;
                        command_VerbTarget.verb.verbProps.ticksBetweenBurstShots = verb.verbProps.ticksBetweenBurstShots;
                        command_VerbTarget.verb.verbProps.soundCast = verb.verbProps.soundCast;
                        command_VerbTarget.defaultLabel = "Fire: " + verb.verbProps.label;
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
                    //    command_Action.action = 
                        command_Action.disabled = true;
                        yield return command_Action;
                    }
                }
            }
            
            yield break;
        }
        */
        public override void CompTick()
        {
            base.CompTick();
            Log.Message(string.Format("CompTick"));
            if (GetWearer!=null)
            {
                Log.Message(string.Format("curStance:{0}, curStance.GetType():{1}", GetWearer.stances.curStance, GetWearer.stances.curStance.GetType()));
                if (GetWearer.stances.curStance.GetType() == typeof(Stance_Cooldown))
                {
                    Castverb = DefaultVerb;
                    Log.Message(string.Format("SwitchFireMode:{0}", DefaultVerb.verbProps.label));
                }
            }
        }
    }
}
