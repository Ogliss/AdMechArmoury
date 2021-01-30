using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    class Building_DeployableBarricade : Building
    {
        private Graphic graphicDeployedInt;

        public DeployableBarricadeExtension deployed
        {
            get
            {
                return this.def.GetModExtension<DeployableBarricadeExtension>();
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
                    if (this.deployed != null)
                    {
                        this.graphicDeployedInt = this.deployed.deployedgraphicData.GraphicColoredFor(this);
                    }
                }
                return this.graphicDeployedInt;
            }
        }

        public bool Toggled
        {
            get
            {
                return _Flickable.SwitchIsOn;
            }
            set
            {

            }
        }

        public CompFlickable _Flickable
        {
            get
            {
                return this.TryGetComp<CompFlickable>();
            }
        }

        public override ushort PathFindCostFor(Pawn p)
        {
            if (this.Toggled)
            {
                return (ushort)deployed.deployedpathCost;
            }
            return base.PathFindCostFor(p);
        }

        public override ushort PathWalkCostFor(Pawn p)
        {
            if (this.Toggled)
            {
                return (ushort)deployed.deployedpathCost;
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
