using RimWorld;
using System;
using Verse;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.CompProperties_GiveTech
    public class CompProperties_GiveTech : CompProperties_UseEffect
    {
        public ResearchProjectDef TechtoGive;
    }

    // AdeptusMechanicus.CompProperties_GiveTech
    public class CompUseEffect_GiveTechnology : CompUseEffect
    {
        public new CompProperties_GiveTech Props
        {
            get
            {
                return (CompProperties_GiveTech)this.props;
            }
        }

        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);
            ResearchProjectDef currentProj = Props.TechtoGive;
            if (currentProj != null && !currentProj.IsFinished)
            {
                this.FinishInstantly(currentProj, usedBy);
            }
        }

        public override bool SelectedUseOption(Pawn p)
        {
            return base.SelectedUseOption(p);
        }

        public override AcceptanceReport CanBeUsedBy(Pawn p)
        {
            if (Props.TechtoGive == null || Props.TechtoGive.IsFinished)
            {
                return "NoActiveResearchProjectToFinish".Translate();
            }
            return true;
        }
        private void FinishInstantly(ResearchProjectDef proj, Pawn usedBy)
        {
            Find.ResearchManager.FinishProject(proj, false, null);
            Messages.Message("MessageResearchProjectFinishedByItem".Translate(proj.LabelCap), usedBy, MessageTypeDefOf.PositiveEvent, true);
        }
    }
}
