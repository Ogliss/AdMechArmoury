using AdeptusMechanicus.ExtensionMethods;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    class Building_DeployableBarricade : Building
    {
        private Graphic graphicDeployedInt;

        public DeployableBarricadeExtension Deployed
        {
            get
            {
                return this.def.GetModExtensionFast<DeployableBarricadeExtension>();
            }
        }

        public override Graphic Graphic
        {
            get
            {
                if (this.Toggled)
                {
                    return base.Graphic;
                }
                if (this.graphicDeployedInt == null)
                {
                    if (this.Deployed != null)
                    {
                        this.graphicDeployedInt = this.Deployed.deployedgraphicData.GraphicColoredFor(this);
                    }
                }
                return this.graphicDeployedInt;
            }
        }

        public bool Toggled
        {
            get
            {
                return Flickable.SwitchIsOn;
            }
            set
            {

            }
        }

        public CompFlickable Flickable
        {
            get
            {
                return this.TryGetCompFast<CompFlickable>();
            }
        }

        public override ushort PathFindCostFor(Pawn p)
        {
            if (this.Toggled)
            {
                return (ushort)Deployed.deployedpathCost;
            }
            return base.PathFindCostFor(p);
        }

        public override ushort PathWalkCostFor(Pawn p)
        {
            if (this.Toggled)
            {
                return (ushort)Deployed.deployedpathCost;
            }
            return base.PathWalkCostFor(p);
        }

        public override void Notify_SignalReceived(Signal signal)
        {
            base.Notify_SignalReceived(signal);
            if (signal.tag == "FlickedOn")
            {
                Toggled = true;
            }
            if (signal.tag == "FlickedOff")
            {
                Toggled = false;
            }

        }
        public override string Label
        {
            get
            {
                string status = Toggled ? "DB_Deployed".Translate() : "DB_Retracted".Translate();
                return base.Label + " (" + status + ")";
            }
        }

        private void RecalcPathsOnAndAroundMe(Map map)
        {
            IntVec3[] adjacentCellsAndInside = GenAdj.AdjacentCellsAndInside;
            for (int i = 0; i < adjacentCellsAndInside.Length; i++)
            {
                IntVec3 c = base.Position + adjacentCellsAndInside[i];
                if (c.InBounds(map))
                {
                    map.pathGrid.RecalculatePerceivedPathCostAt(c);
                }
            }
        }
    }
}
