using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{

    public class PawnRenderNodeWorker_SwarmPart : PawnRenderNodeWorker
    {
        public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
        {
            return base.CanDrawNow(node, parms) || (node.tree.pawn.Dead && node == node.tree.rootNode.children[0]);
        }
        public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
        {
            Vector3 vector = base.OffsetFor(node, parms, out pivot);
            PawnRenderNode_SwarmPart pawnRenderNode_SwarmPart;
            PawnRenderNode_SwarmPart.SpasmData spasmData;
            float t;
            if ((pawnRenderNode_SwarmPart = (node as PawnRenderNode_SwarmPart)) != null && pawnRenderNode_SwarmPart.CheckAndDoSpasm(parms, out spasmData, out t))
            {
                vector += Vector3.Lerp(spasmData.offsetStart, spasmData.offsetTarget, t);
            }
            return vector;
        }

        public override Quaternion RotationFor(PawnRenderNode node, PawnDrawParms parms)
        {
            Quaternion quaternion = base.RotationFor(node, parms);
            PawnRenderNode_SwarmPart pawnRenderNode_SwarmPart;
            if ((pawnRenderNode_SwarmPart = (node as PawnRenderNode_SwarmPart)) == null)
            {
                return quaternion;
            }
            float num = 0f;
            PawnRenderNodeProperties_SwarmPart pawnRenderNodeProperties_SwarmPart;
            if ((pawnRenderNodeProperties_SwarmPart = (node.Props as PawnRenderNodeProperties_SwarmPart)) != null && pawnRenderNodeProperties_SwarmPart.rotateFacing)
            {
                num += parms.facing.AsAngle;
            }
            PawnRenderNode_SwarmPart.SpasmData spasmData;
            float t;
            if (pawnRenderNode_SwarmPart.CheckAndDoSpasm(parms, out spasmData, out t))
            {
                num += Mathf.Lerp(spasmData.rotationStart, spasmData.rotationTarget, t);
            }
            return quaternion * num.ToQuat();
        }

        public override Vector3 ScaleFor(PawnRenderNode node, PawnDrawParms parms)
        {
            Vector3 vector = base.ScaleFor(node, parms);
            PawnRenderNode_SwarmPart pawnRenderNode_SwarmPart;
            PawnRenderNode_SwarmPart.SpasmData spasmData;
            float t;
            if ((pawnRenderNode_SwarmPart = (node as PawnRenderNode_SwarmPart)) != null && pawnRenderNode_SwarmPart.CheckAndDoSpasm(parms, out spasmData, out t))
            {
                vector *= Mathf.Lerp(spasmData.scaleStart, spasmData.scaleTarget, t);
                vector.y = 1f;
            }
            return vector;
        }
    }

}
