using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x02000020 RID: 32
    public class DeployableBarricadeExtension : DefModExtension
    {
        public int deployedpathCost = 35;
        public float deployedfillPercent = 0.5f;
        public Traversability deployedpassability = Traversability.PassThroughOnly;
        public GraphicData deployedgraphicData = null;
    }


    class Building_DeployableBarricade : Building
    {
        /*
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref Toggled, "ToggledFireMode", false, true);
        }
        */
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

        /*
        public override IEnumerable<Gizmo> GetGizmos()
        {
            int num = 700000101;

            yield return new Command_Toggle
            {
                icon = this.def.uiIcon,
                defaultLabel = "DB_ToggleModeCommandLabel".Translate(),
                defaultDesc = "DB_ToggleModeCommandDesc".Translate(),
                isActive = (() => Toggled),
                toggleAction = delegate ()
                {
                    Toggled = !Toggled;
                    SwitchMode();
                },
                activateSound = SoundDef.Named("Click"),
                groupKey = num
            };
            foreach (var item in base.GetGizmos())
            {
                yield return item;
            }
        }
        */
        

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
                return base.Label + " ("+status+")";
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

        public void SwitchMode()
        {
            switch (Toggled)
            {
                case false:
                //    this.def.passability = Traversability.Standable;
                //    this.def.castEdgeShadows
                    break;
                case true:
                //    this.def.passability = Traversability.PassThroughOnly;
                    break;
            }
            this.RecalcPathsOnAndAroundMe(this.Map);
        }
        
    }
}
