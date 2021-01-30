using System;
using Verse;

namespace RimWorld
{
    // Token: 0x0200025F RID: 607
    public class CompProperties_GiveTech : CompProperties_UseEffect
    {
        // Token: 0x040004DB RID: 1243
        public ResearchProjectDef TechtoGive;
    }

    // Token: 0x0200078F RID: 1935
    public class CompUseEffect_GiveTechnology : CompUseEffect
    {
        public CompProperties_GiveTech Props
        {
            get
            {
                return (CompProperties_GiveTech)this.props;
            }
        }
        // Token: 0x06002AC8 RID: 10952 RVA: 0x001428E4 File Offset: 0x00140CE4
        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);
            ResearchProjectDef currentProj = Props.TechtoGive;
            if (currentProj != null)
            {
                this.FinishInstantly(currentProj, usedBy);
            }
        }

        // Token: 0x06002AC9 RID: 10953 RVA: 0x00142911 File Offset: 0x00140D11
        public override bool CanBeUsedBy(Pawn p, out string failReason)
        {
            if (Props.TechtoGive == null)
            {
                failReason = "NoActiveResearchProjectToFinish".Translate();
                return false;
            }
            failReason = null;
            return true;
        }

        // Token: 0x06002ACA RID: 10954 RVA: 0x00142934 File Offset: 0x00140D34
        private void FinishInstantly(ResearchProjectDef proj, Pawn usedBy)
        {
            Find.ResearchManager.FinishProject(proj, false, null);
            Messages.Message("MessageResearchProjectFinishedByItem".Translate(proj.LabelCap), usedBy, MessageTypeDefOf.PositiveEvent, true);
        }
    }
}
