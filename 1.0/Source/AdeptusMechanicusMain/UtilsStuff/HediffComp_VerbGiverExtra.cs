using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x02000D75 RID: 3445
    public class HediffCompProperties_VerbGiverExtra : HediffCompProperties_VerbGiver
    {
        // Token: 0x06004C81 RID: 19585 RVA: 0x00239AA6 File Offset: 0x00237EA6
        public HediffCompProperties_VerbGiverExtra()
        {
            this.compClass = typeof(HediffComp_VerbGiverExtra);
        }

        // Token: 0x06004C82 RID: 19586 RVA: 0x00239AC0 File Offset: 0x00237EC0
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

        // Token: 0x06004C83 RID: 19587 RVA: 0x00239B18 File Offset: 0x00237F18
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
        // Token: 0x04003407 RID: 13319
        public List<GunVerbEntry> verbEntrys;

        // Token: 0x04003408 RID: 13320
    //    public List<Tool> tools;
    }
    public class HediffComp_VerbGiverExtra : HediffComp_VerbGiver, IVerbOwner
    {
        public int tickLastUsed = -1;
        // Token: 0x06004C85 RID: 19589 RVA: 0x00239F45 File Offset: 0x00238345
        public HediffComp_VerbGiverExtra()
        {
            this.verbTracker = new VerbTracker(this);
        }
        public IEnumerable<Command> GetVerbsCommands()
        {
            return this.verbTracker.GetVerbsCommands(KeyCode.None);
        }
        // Token: 0x17000C0C RID: 3084
        // (get) Token: 0x06004C86 RID: 19590 RVA: 0x00239F59 File Offset: 0x00238359
        public HediffCompProperties_VerbGiverExtra Props
        {
            get
            {
                return (HediffCompProperties_VerbGiverExtra)this.props;
            }
        }

        // Token: 0x17000C0D RID: 3085
        // (get) Token: 0x06004C87 RID: 19591 RVA: 0x00239F66 File Offset: 0x00238366
        public VerbTracker VerbTracker
        {
            get
            {
                return this.verbTracker;
            }
        }

        // Token: 0x17000C0E RID: 3086
        // (get) Token: 0x06004C88 RID: 19592 RVA: 0x00239F6E File Offset: 0x0023836E
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

        // Token: 0x17000C0F RID: 3087
        // (get) Token: 0x06004C89 RID: 19593 RVA: 0x00239F7B File Offset: 0x0023837B
        public List<Tool> Tools
        {
            get
            {
                return this.Props.tools;
            }
        }

        // Token: 0x17000C0A RID: 3082
        // (get) Token: 0x06004C8A RID: 19594 RVA: 0x00239F88 File Offset: 0x00238388
        Thing IVerbOwner.ConstantCaster
        {
            get
            {
                return base.Pawn;
            }
        }

        // Token: 0x17000C0B RID: 3083
        // (get) Token: 0x06004C8B RID: 19595 RVA: 0x00239F90 File Offset: 0x00238390
        ImplementOwnerTypeDef IVerbOwner.ImplementOwnerTypeDef
        {
            get
            {
                return ImplementOwnerTypeDefOf.Hediff;
            }
        }

        // Token: 0x06004C8C RID: 19596 RVA: 0x00239F98 File Offset: 0x00238398
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

        // Token: 0x06004C8D RID: 19597 RVA: 0x00239FE7 File Offset: 0x002383E7
        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            this.verbTracker.VerbsTick();
        }

        // Token: 0x06004C8E RID: 19598 RVA: 0x00239FFB File Offset: 0x002383FB
        string IVerbOwner.UniqueVerbOwnerID()
        {
            return this.parent.GetUniqueLoadID() + "_" + this.parent.comps.IndexOf(this);
        }

        // Token: 0x06004C8F RID: 19599 RVA: 0x0023A028 File Offset: 0x00238428
        bool IVerbOwner.VerbsStillUsableBy(Pawn p)
        {
            return p.health.hediffSet.hediffs.Contains(this.parent);
        }
        
    }
}
