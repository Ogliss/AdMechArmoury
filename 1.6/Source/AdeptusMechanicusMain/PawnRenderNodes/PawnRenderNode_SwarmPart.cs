using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public class PawnRenderNode_SwarmPart : PawnRenderNode_Spastic
    {
        public PawnRenderNode_SwarmPart(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
        {
        }

        public new bool CheckAndDoSpasm(PawnDrawParms parms, out PawnRenderNode_Spastic.SpasmData dat, out float progress)
        {
            PawnRenderNodeProperties_Spastic pawnRenderNodeProperties_Spastic;
            if (parms.pawn.DeadOrDowned || (pawnRenderNodeProperties_Spastic = (this.props as PawnRenderNodeProperties_Spastic)) == null || parms.Portrait || parms.Cache)
            {
                progress = 0f;
                dat = null;
                return false;
            }
            if (this.spasmData == null)
            {
                this.spasmData = new PawnRenderNode_Spastic.SpasmData();
            }
            if (Find.TickManager.TicksGame >= this.spasmData.nextSpasm)
            {
                this.spasmData.tickStart = Find.TickManager.TicksGame;
                this.spasmData.duration = (float)this.GetNextSpasmDurationTicks();
                this.spasmData.nextSpasm = this.GetNextSpasmTick();
                this.spasmData.rotationStart = this.spasmData.rotationTarget;
                this.spasmData.rotationTarget = pawnRenderNodeProperties_Spastic.rotationRange.RandomInRange;
                this.spasmData.scaleStart = this.spasmData.scaleTarget;
                this.spasmData.scaleTarget = pawnRenderNodeProperties_Spastic.scaleRange.RandomInRange;
                this.spasmData.offsetStart = this.spasmData.offsetTarget;
                this.spasmData.offsetTarget = new Vector3(pawnRenderNodeProperties_Spastic.offsetRangeX.RandomInRange, 0f, pawnRenderNodeProperties_Spastic.offsetRangeZ.RandomInRange);
            }
            progress = (float)(Find.TickManager.TicksGame - this.spasmData.tickStart) / Mathf.Max(this.spasmData.duration, 0.0001f);
            dat = this.spasmData;
            return true;
        }
        public override GraphicMeshSet MeshSetFor(Pawn pawn)
        {
            Graphic graphic = this.GraphicFor(pawn);
            if (graphic != null)
            {
                return MeshPool.GetMeshSetForSize(graphic.drawSize.x, graphic.drawSize.y);
            }
            return null;
            return new GraphicMeshSet(MeshPool.GridPlane(this.props.overrideMeshSize ?? this.props.drawSize));
        }

        public override Graphic GraphicFor(Pawn pawn)
        {
            PawnKindLifeStage curKindLifeStage = pawn.ageTracker.CurKindLifeStage;
            AlternateGraphic alternateGraphic;
            int num;
            Graphic graphic;
            if (TryGetAlternate(pawn, props.baseLayer, out alternateGraphic, out num))
            {
                graphic = alternateGraphic.GetGraphic(curKindLifeStage.bodyGraphicData.Graphic);
            }
            else if (pawn.gender != Gender.Female || curKindLifeStage.femaleGraphicData == null)
            {
                graphic = curKindLifeStage.bodyGraphicData.Graphic;
            }
            else
            {
                graphic = curKindLifeStage.femaleGraphicData.Graphic;
            }
            if ((pawn.Dead || (pawn.IsMutant && pawn.mutant.Def.useCorpseGraphics)) && curKindLifeStage.corpseGraphicData != null)
            {
                if (pawn.gender != Gender.Female || curKindLifeStage.femaleCorpseGraphicData == null)
                {
                    graphic = curKindLifeStage.corpseGraphicData.Graphic.GetColoredVersion(curKindLifeStage.corpseGraphicData.Graphic.Shader, graphic.Color, graphic.ColorTwo);
                }
                else
                {
                    graphic = curKindLifeStage.femaleCorpseGraphicData.Graphic.GetColoredVersion(curKindLifeStage.femaleCorpseGraphicData.Graphic.Shader, graphic.Color, graphic.ColorTwo);
                }
            }
            switch (pawn.Drawer.renderer.CurRotDrawMode)
            {
                case RotDrawMode.Fresh:
                    if (ModsConfig.AnomalyActive && pawn.IsMutant && pawn.mutant.HasTurned)
                    {
                        return graphic.GetColoredVersion(ShaderDatabase.Cutout, MutantUtility.GetMutantSkinColor(pawn, new Color?(graphic.Color).Value), MutantUtility.GetMutantSkinColor(pawn, new Color?(graphic.ColorTwo).Value));
                    }
                    return graphic;
                case RotDrawMode.Rotting:
                    return graphic.GetColoredVersion(ShaderDatabase.Cutout, Verse.PawnRenderUtility.GetRottenColor(graphic.Color), Verse.PawnRenderUtility.GetRottenColor(graphic.ColorTwo));
                case RotDrawMode.Dessicated:
                    if (curKindLifeStage.dessicatedBodyGraphicData != null)
                    {
                        Graphic graphic2;
                        if (pawn.RaceProps.FleshType == FleshTypeDefOf.Insectoid)
                        {
                            Color dessicatedColorInsect = Verse.PawnRenderUtility.DessicatedColorInsect;
                            if (pawn.gender != Gender.Female || curKindLifeStage.femaleDessicatedBodyGraphicData == null)
                            {
                                graphic2 = curKindLifeStage.dessicatedBodyGraphicData.Graphic.GetColoredVersion(ShaderDatabase.Cutout, dessicatedColorInsect, dessicatedColorInsect);
                            }
                            else
                            {
                                graphic2 = curKindLifeStage.femaleDessicatedBodyGraphicData.Graphic.GetColoredVersion(ShaderDatabase.Cutout, dessicatedColorInsect, dessicatedColorInsect);
                            }
                        }
                        else if (pawn.gender != Gender.Female || curKindLifeStage.femaleDessicatedBodyGraphicData == null)
                        {
                            graphic2 = curKindLifeStage.dessicatedBodyGraphicData.GraphicColoredFor(pawn);
                        }
                        else
                        {
                            graphic2 = curKindLifeStage.femaleDessicatedBodyGraphicData.GraphicColoredFor(pawn);
                        }
                        if (pawn.IsMutant)
                        {
                            graphic2.ShadowGraphic = graphic.ShadowGraphic;
                        }
                        if (alternateGraphic != null)
                        {
                            graphic2 = alternateGraphic.GetDessicatedGraphic(graphic2);
                        }
                        return graphic2;
                    }
                    break;
            }
            return null;
        }
        public bool TryGetAlternate(Pawn pawn, float extra, out AlternateGraphic ag, out int index)
        {
            ag = null;
            index = -1;
            Rand.PushState((int)(pawn.thingIDNumber+extra) ^ 46101);
            if (Rand.Chance(pawn.kindDef.alternateGraphicChance))
            {
                if (pawn.kindDef.alternateGraphics.TryRandomElementByWeight((AlternateGraphic x) => x.Weight, out ag))
                {
                    index = pawn.kindDef.alternateGraphics.IndexOf(ag);
                }
            }
            Rand.PopState();
            return ag != null;
        }
    }


    /*
    public class PawnRenderNodeWorker_SwarmPart : PawnRenderNodeWorker
    {
        public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
        {
            PawnRenderNodeProperties_SwarmPart props =node.Props as PawnRenderNodeProperties_SwarmPart;
            return base.CanDrawNow(node, parms) && node.tree.pawn.health.summaryHealth.SummaryHealthPercent >= props.hpThreshold;
        }

        public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
        {
            return base.OffsetFor(node, parms, out pivot);
        }
        public override void AppendDrawRequests(PawnRenderNode node, PawnDrawParms parms, List<PawnGraphicDrawRequest> requests)
        {
            base.AppendDrawRequests(node, parms, requests);
        }
        */
        // change to post draw?, replace root node instead of using multiple subs
        /*
        public override void PostDraw(PawnRenderNode node, PawnDrawParms parms, Mesh mesh, Matrix4x4 matrix)
        {
            Vector3 vector2;
            Vector3 vector = parms.matrix.Position() + this.OffsetFor(node, parms, out vector2);
            Pawn_CarryTracker carryTracker = parms.pawn.carryTracker;
            if (((carryTracker != null) ? carryTracker.CarriedThing : null) != null)
            {
                PawnRenderUtility.DrawCarriedThing(parms.pawn, vector, parms.pawn.carryTracker.CarriedThing);
                return;
            }
            PawnRenderUtility.DrawEquipmentAndApparelExtras(parms.pawn, vector, parms.facing, parms.flags);
        }
        */
        /*
        public override Vector3 ScaleFor(PawnRenderNode node, PawnDrawParms parms)
        {
            Vector3 a = base.ScaleFor(node, parms);
            Vector2 bodyGraphicScale = parms.pawn.story.bodyType.bodyGraphicScale;
            return a * ((bodyGraphicScale.x + bodyGraphicScale.y) / 2f);
        }
    }
    */
}
