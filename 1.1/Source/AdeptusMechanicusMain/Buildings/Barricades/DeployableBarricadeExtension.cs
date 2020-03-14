using RimWorld;
using Verse;

namespace VanillaSecurityExpanded
{
    // Token: 0x02000020 RID: 32
    public class DeployableBarricadeExtension : DefModExtension
    {
        public int deployedpathCost = 35;
        public float deployedfillPercent = 0.5f;
        public Traversability deployedpassability = Traversability.PassThroughOnly;
        public GraphicData deployedgraphicData = null;
    }
    
}
