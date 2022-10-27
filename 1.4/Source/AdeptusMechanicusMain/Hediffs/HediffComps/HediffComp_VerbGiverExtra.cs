using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public class HediffCompProperties_VerbGiverExtra : HediffCompProperties_VerbGiver
    {
        public HediffCompProperties_VerbGiverExtra()
        {
            this.compClass = typeof(HediffComp_VerbGiverExtra);
        }

        public override void PostLoad()
        {
            base.PostLoad();
            if (this.tools != null)
            {
                for (int i = 0; i < this.tools.Count; i++)
                {
                    this.tools[i].id = i.ToString();
                }
            }
        }

        public override IEnumerable<string> ConfigErrors(HediffDef parentDef)
        {
            foreach (string err in base.ConfigErrors(parentDef))
            {
                yield return err;
            }
            if (this.tools != null)
            {
                Tool dupeTool = this.tools.SelectMany((Tool lhs) => from rhs in this.tools
                                                                    where lhs != rhs && lhs.id == rhs.id
                                                                    select rhs).FirstOrDefault<Tool>();
                if (dupeTool != null)
                {
                    yield return string.Format("duplicate hediff tool id {0}", dupeTool.id);
                }
                foreach (Tool t in this.tools)
                {
                    foreach (string e in t.ConfigErrors())
                    {
                        yield return e;
                    }
                }
            }
            yield break;
        }
        public int Cooldown = 0;

        public List<GunVerbEntry> verbEntrys;
    }

    public class HediffComp_VerbGiverExtra : HediffComp_VerbGiver, IVerbOwner
    {
        public int tickLastUsed = -1;

        public HediffComp_VerbGiverExtra()
        {
            this.verbTracker = new VerbTracker(this);
        }
        public IEnumerable<Command> GetVerbsCommands()
        {
            return this.verbTracker.GetVerbsCommands(KeyCode.None);
        }

        public new HediffCompProperties_VerbGiverExtra Props
        {
            get
            {
                return (HediffCompProperties_VerbGiverExtra)this.props;
            }
        }

        public new VerbTracker VerbTracker
        {
            get
            {
                return this.verbTracker;
            }
        }

        public new List<VerbProperties> VerbProperties
        {
            get
            {
                List<VerbProperties> VerbProperties = new List<Verse.VerbProperties>();
                if (Props.verbEntrys.NullOrEmpty())
                {
                    return this.Props.verbs;
                }
                foreach (GunVerbEntry Entry in Props.verbEntrys)
                {
                    if (Entry.VerbProps!=null)
                    {
                        VerbProperties.Add(Entry.VerbProps);
                    }
                }
                return VerbProperties;
            }
        }

        public new List<Tool> Tools
        {
            get
            {
                return this.Props.tools;
            }
        }

        Thing IVerbOwner.ConstantCaster
        {
            get
            {
                return base.Pawn;
            }
        }

        ImplementOwnerTypeDef IVerbOwner.ImplementOwnerTypeDef
        {
            get
            {
                return ImplementOwnerTypeDefOf.Hediff;
            }
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            /*
            Scribe_Deep.Look<VerbTracker>(ref this.verbTracker, "verbTracker", new object[]
            {
                this
            });
            if (Scribe.mode == LoadSaveMode.PostLoadInit && this.verbTracker == null)
            {
                this.verbTracker = new VerbTracker(this);
            }
            */
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            this.verbTracker.VerbsTick();
        }

        string IVerbOwner.UniqueVerbOwnerID()
        {
            return this.parent.GetUniqueLoadID() + "_" + this.parent.comps.IndexOf(this);
        }

        bool IVerbOwner.VerbsStillUsableBy(Pawn p)
        {
            return p.health.hediffSet.hediffs.Contains(this.parent);
        }

        public override void Notify_PawnUsedVerb(Verb verb, LocalTargetInfo target)
        {
            base.Notify_PawnUsedVerb(verb, target);
        }
    }
}
