using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    /*
    // AdeptusMechanicus.Building_AdvancedArt
    public class Building_AdvancedArt : Building_Art
    {
        private Graphic offGraphic;
        public CompFlickable flickableComp
        {
            get
            {
                return this.TryGetCompFast<CompFlickable>();
            }
        }

        public CompPowerTrader CompPower
        {
            get
            {
                return this.TryGetCompFast<CompPowerTrader>();
            }
        }

        public override Graphic Graphic
        {
            get
            {
                if (flickableComp != null)
                {
                    if (CompPower != null)
                    {
                        if (flickableComp.SwitchIsOn && CompPower.PowerOn)
                        {
                            return this.DefaultGraphic;
                        }
                        if (this.offGraphic == null)
                        {
                            this.offGraphic = GraphicDatabase.Get(this.def.graphicData.graphicClass, this.def.graphicData.texPath + "_Off", this.def.graphicData.shaderType.Shader, this.def.graphicData.drawSize, this.DrawColor, this.DrawColorTwo);
                        }
                        return this.offGraphic;
                    }
                }
                return base.DefaultGraphic;
            }

        }
    }
    */
}
