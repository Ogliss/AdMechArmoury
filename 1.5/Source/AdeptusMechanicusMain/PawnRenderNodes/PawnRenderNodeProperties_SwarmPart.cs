using Verse;

namespace AdeptusMechanicus
{
    public class PawnRenderNodeProperties_SwarmPart : PawnRenderNodeProperties_Spastic
    {
        public PawnRenderNodeProperties_SwarmPart()
        {
            this.nodeClass = typeof(PawnRenderNode_SwarmPart);
            this.workerClass = typeof(PawnRenderNodeWorker_SwarmPart);
            this.rotateFacing = false;
            this.baseLayer = 10f;
            this.offsetRangeX = new FloatRange(-0.5f, 0.5f);
            this.offsetRangeZ = new FloatRange(-0.5f, 0.5f);
        //   this.scaleRange = new FloatRange(0.7f, 1.2f);
            this.rotationRange = new FloatRange(-25f, 25f);
            this.durationTicksRange = new IntRange(10, 35);
            this.nextSpasmTicksRange = new IntRange(0, 20);
        }
    }

}
