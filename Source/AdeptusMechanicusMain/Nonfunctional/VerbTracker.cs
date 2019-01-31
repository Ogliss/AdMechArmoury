using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;

namespace Verse
{
    // Token: 0x02001026 RID: 4134
    public class VerbTracker : IExposable
    {
        // Token: 0x060064C5 RID: 25797 RVA: 0x002D1E03 File Offset: 0x002D0203
        public VerbTracker(IVerbOwner directOwner)
        {
            this.directOwner = directOwner;
        }

        // Token: 0x1700103E RID: 4158
        // (get) Token: 0x060064C6 RID: 25798 RVA: 0x002D1E12 File Offset: 0x002D0212
        public List<Verb> AllVerbs
        {
            get
            {
                if (this.verbs == null)
                {
                    this.InitVerbsFromZero();
                }
                return this.verbs;
            }
        }

        // Token: 0x1700103F RID: 4159
        // (get) Token: 0x060064C7 RID: 25799 RVA: 0x002D1E2C File Offset: 0x002D022C
        public Verb PrimaryVerb
        {
            get
            {
                if (this.verbs == null)
                {
                    this.InitVerbsFromZero();
                }
                for (int i = 0; i < this.verbs.Count; i++)
                {
                    if (this.verbs[i].verbProps.isPrimary)
                    {
                        return this.verbs[i];
                    }
                }
                return null;
            }
        }

        // Token: 0x060064C8 RID: 25800 RVA: 0x002D1E90 File Offset: 0x002D0290
        public void VerbsTick()
        {
            if (this.verbs == null)
            {
                return;
            }
            for (int i = 0; i < this.verbs.Count; i++)
            {
                this.verbs[i].VerbTick();
            }
        }

        // Token: 0x060064C9 RID: 25801 RVA: 0x002D1ED8 File Offset: 0x002D02D8
        public IEnumerable<Command> GetVerbsCommands(KeyCode hotKey = KeyCode.None)
        {
            CompEquippable ce = this.directOwner as CompEquippable;
            if (ce == null)
            {
                yield break;
            }
            Thing ownerThing = ce.parent;
            List<Verb> verbs = this.AllVerbs;
            for (int i = 0; i < verbs.Count; i++)
            {
                Verb verb = verbs[i];
                if (verb.verbProps.hasStandardCommand)
                {
                    yield return this.CreateVerbTargetCommand(ownerThing, verb);
                }
            }
            if (!this.directOwner.Tools.NullOrEmpty<Tool>() && ce != null && ce.parent.def.IsMeleeWeapon)
            {
                yield return this.CreateVerbTargetCommand(ownerThing, (from v in verbs
                                                                       where v.verbProps.IsMeleeAttack
                                                                       select v).FirstOrDefault<Verb>());
            }
            yield break;
        }

        // Token: 0x060064CA RID: 25802 RVA: 0x002D1EFC File Offset: 0x002D02FC
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

        // Token: 0x060064CB RID: 25803 RVA: 0x002D2038 File Offset: 0x002D0438
        public Verb GetVerb(VerbCategory category)
        {
            List<Verb> allVerbs = this.AllVerbs;
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

        // Token: 0x060064CC RID: 25804 RVA: 0x002D208C File Offset: 0x002D048C
        public void ExposeData()
        {
            Scribe_Collections.Look<Verb>(ref this.verbs, "verbs", LookMode.Deep, new object[0]);
            if (Scribe.mode == LoadSaveMode.ResolvingCrossRefs && this.verbs != null)
            {
                if (this.verbs.RemoveAll((Verb x) => x == null) != 0)
                {
                    Log.Error("Some verbs were null after loading. directOwner=" + this.directOwner.ToStringSafe<IVerbOwner>(), false);
                }
                List<Verb> sources = this.verbs;
                this.verbs = new List<Verb>();
                this.InitVerbs(delegate (Type type, string id)
                {
                    Verb verb = sources.FirstOrDefault((Verb v) => v.loadID == id && v.GetType() == type);
                    if (verb == null)
                    {
                        Log.Warning(string.Format("Replaced verb {0}/{1}; may have been changed through a version update or a mod change", type, id), false);
                        verb = (Verb)Activator.CreateInstance(type);
                    }
                    this.verbs.Add(verb);
                    return verb;
                });
            }
        }

        // Token: 0x060064CD RID: 25805 RVA: 0x002D2144 File Offset: 0x002D0544
        private void InitVerbsFromZero()
        {
            this.verbs = new List<Verb>();
            this.InitVerbs(delegate (Type type, string id)
            {
                Verb verb = (Verb)Activator.CreateInstance(type);
                this.verbs.Add(verb);
                return verb;
            });
        }

        // Token: 0x060064CE RID: 25806 RVA: 0x002D2164 File Offset: 0x002D0564
        private void InitVerbs(Func<Type, string, Verb> creator)
        {
            List<VerbProperties> verbProperties = this.directOwner.VerbProperties;
            if (verbProperties != null)
            {
                for (int i = 0; i < verbProperties.Count; i++)
                {
                    try
                    {
                        VerbProperties verbProperties2 = verbProperties[i];
                        string text = Verb.CalculateUniqueLoadID(this.directOwner, i);
                        this.InitVerb(creator(verbProperties2.verbClass, text), verbProperties2, null, null, text);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(string.Concat(new object[]
                        {
                            "Could not instantiate Verb (directOwner=",
                            this.directOwner.ToStringSafe<IVerbOwner>(),
                            "): ",
                            ex
                        }), false);
                    }
                }
            }
            List<Tool> tools = this.directOwner.Tools;
            if (tools != null)
            {
                for (int j = 0; j < tools.Count; j++)
                {
                    Tool tool = tools[j];
                    foreach (ManeuverDef maneuverDef in tool.Maneuvers)
                    {
                        try
                        {
                            VerbProperties verb = maneuverDef.verb;
                            string text2 = Verb.CalculateUniqueLoadID(this.directOwner, tool, maneuverDef);
                            this.InitVerb(creator(verb.verbClass, text2), verb, tool, maneuverDef, text2);
                        }
                        catch (Exception ex2)
                        {
                            Log.Error(string.Concat(new object[]
                            {
                                "Could not instantiate Verb (directOwner=",
                                this.directOwner.ToStringSafe<IVerbOwner>(),
                                "): ",
                                ex2
                            }), false);
                        }
                    }
                }
            }
        }

        // Token: 0x060064CF RID: 25807 RVA: 0x002D231C File Offset: 0x002D071C
        private void InitVerb(Verb verb, VerbProperties properties, Tool tool, ManeuverDef maneuver, string id)
        {
            verb.loadID = id;
            verb.verbProps = properties;
            verb.verbTracker = this;
            verb.tool = tool;
            verb.maneuver = maneuver;
            verb.caster = this.directOwner.ConstantCaster;
        }

        // Token: 0x04004228 RID: 16936
        public IVerbOwner directOwner;

        // Token: 0x04004229 RID: 16937
        private List<Verb> verbs;
    }
}
