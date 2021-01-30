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

        public ThingDef projectileDef => Props.projectileDef;
        public ResearchProjectDef researchDef
        {
            get
            {
                ResearchProjectDef Def = DefDatabase<ResearchProjectDef>.GetNamedSilentFail(Props.researchDef);
                if (Def != null)
                {
                    return Def;
                }
                return null;
            }
        }
        public List<FactionDef> factionDefs
        {
            get
            {
                List<FactionDef> list = new List<FactionDef>();
                foreach (var item in Props.factions)
                {
                    FactionDef Def = DefDatabase<FactionDef>.GetNamedSilentFail(item);
                    if (Def != null)
                    {
                        list.Add(Def);
                    }
                }
                return list;
            }
        }
    }
}
