using AdeptusMechanicus.ExtensionMethods;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace AdeptusMechanicus
{
    public class CompProperties_UpgradeableProjectile : CompProperties
    {
        public CompProperties_UpgradeableProjectile()
        {
            this.compClass = typeof(CompUpgradeableProjectile);
        }
        public List<string> factions;
        public ThingDef projectileDef;
        public string researchDef;
    }

    public class CompUpgradeableProjectile : ThingComp
    {
        public CompProperties_UpgradeableProjectile Props => (CompProperties_UpgradeableProjectile)props;
        public CompEquippable equippable;
        public CompEquippable Equippable
        {
            get => equippable ??= this.parent.TryGetCompFast<CompEquippable>();
        }
        public ThingDef ProjectileDef => Props.projectileDef;
        public bool Active => ResearchDef != null && ResearchDef.IsFinished;
        private bool findResearch = true;
        private ResearchProjectDef researchProjectDef = null;
        public ResearchProjectDef ResearchDef
        {
            get
            {
                if (researchProjectDef == null && findResearch)
                {
                    researchProjectDef = DefDatabase<ResearchProjectDef>.GetNamedSilentFail(Props.researchDef);
                    findResearch = false;
                }
                return researchProjectDef;
            }
        }
        private List<FactionDef> factionDefs = null;
        public List<FactionDef> FactionDefs
        {
            get
            {
                if (factionDefs == null)
                {
                    factionDefs = new List<FactionDef>();
                    foreach (var item in Props.factions)
                    {
                        FactionDef Def = DefDatabase<FactionDef>.GetNamedSilentFail(item);
                        if (Def != null)
                        {
                            factionDefs.Add(Def);
                        }
                    }
                }
                return factionDefs;
            }
        }
        public override void Notify_UsedWeapon(Pawn pawn)
        {
            if (Active) Equippable.PrimaryVerb.verbProps.defaultProjectile = this.ProjectileDef;
            base.Notify_UsedWeapon(pawn);
        }
        public override void Notify_Equipped(Pawn pawn)
        {
            if (Active) Equippable.PrimaryVerb.verbProps.defaultProjectile = this.ProjectileDef;
            base.Notify_Equipped(pawn);
        } 
    }
}
