using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace AdeptusMechanicus
{
    [StaticConstructorOnStartup]
    public class DropShipLeaving : FlyShipLeaving, IActiveDropPod, IThingHolder
    {
		public new ActiveDropPodInfo Contents
		{
			get
			{
				return ((ActiveDropPod)this.innerContainer[0]).Contents;
			}
			set
			{
				((ActiveDropPod)this.innerContainer[0]).Contents = value;
			}
		}
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref this.alreadyLeft, "alreadyLeft", false, false);
            Scribe_Values.Look<Vector3>(ref this.exactPosition, "exactPosition", default(Vector3), false);
            Scribe_Values.Look<Vector3>(ref this.shadowExactPosition, "shadowExactPosition", default(Vector3), false);
            Scribe_Values.Look<float>(ref this.exactRotation, "exactRotation", 0f, false);
            Scribe_Values.Look<Vector3>(ref this.scale, "scale", default(Vector3), false);
            Scribe_Values.Look<Vector3>(ref this.shadowScale, "shadowScale", default(Vector3), false);
        }

        public override void LeaveMap()
        {
            if (this.groupID < 0 && this.destinationTile < 0)
            {
                this.Destroy(DestroyMode.Vanish);
                return;
            }
            if (this.alreadyLeft)
            {
                base.LeaveMap();
                return;
            }
            if (this.groupID < 0)
            {
                Log.Error("Drop pod left the map, but its group ID is " + this.groupID, false);
                this.Destroy(DestroyMode.Vanish);
                return;
            }
            if (this.destinationTile < 0)
            {
                Log.Error("Drop pod left the map, but its destination tile is " + this.destinationTile, false);
                this.Destroy(DestroyMode.Vanish);
                return;
            }
            Lord lord = TransporterUtility.FindLord(this.groupID, base.Map);
            if (lord != null)
            {
                base.Map.lordManager.RemoveLord(lord);
            }

            DropShipTraveling travelingTransportPods = (DropShipTraveling)WorldObjectMaker.MakeWorldObject(DefDatabase<WorldObjectDef>.GetNamed(Regex.Replace(this.def.defName, "_Leaving", "_Traveling"), true));
            travelingTransportPods.Tile = base.Map.Tile;
            travelingTransportPods.SetFaction(Faction.OfPlayer);
            travelingTransportPods.destinationTile = this.destinationTile;
            travelingTransportPods.arrivalAction = this.arrivalAction;
            Find.WorldObjects.Add(travelingTransportPods);
            DropShipLeaving.tmpActiveDropPods.Clear();
            DropShipLeaving.tmpActiveDropPods.AddRange(base.Map.listerThings.ThingsInGroup(ThingRequestGroup.ActiveDropPod));
            travelingTransportPods.flyingThing = DropShipLeaving.tmpActiveDropPods.Find(delegate (Thing x)
            {
                DropShipLeaving dropshipleaving2 = x as DropShipLeaving;
                int? num = (dropshipleaving2 != null) ? new int?(dropshipleaving2.groupID) : null;
                int num2 = this.groupID;
                return num.GetValueOrDefault() == num2 & num != null;
            });
            for (int i = 0; i < DropShipLeaving.tmpActiveDropPods.Count; i++)
            {
                DropShipLeaving DropshipLeaving = DropShipLeaving.tmpActiveDropPods[i] as DropShipLeaving;
                if (DropshipLeaving != null && DropshipLeaving.groupID == this.groupID)
                {
                    DropshipLeaving.alreadyLeft = true;
                    travelingTransportPods.AddPod(DropshipLeaving.Contents, true);
                    DropshipLeaving.Contents = null;
                    DropshipLeaving.Destroy(DestroyMode.Vanish);
                }
            }
        }
        /*
        public override Vector3 DrawPos
        {
            get
            {
                return this.spaceshipExactPosition;
            }
        }
        */
        public Vector3 ShadowDrawPos
        {
            get
            {
                return this.shadowExactPosition;
            }
        }

        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            Thing thingForGraphic = this.GetThingForGraphic();
            float num = 0f;
            if (this.def.skyfaller.rotateGraphicTowardsDirection)
            {
                num = this.angle;
            }
            if (this.def.skyfaller.angleCurve != null)
            {
                this.angle = this.def.skyfaller.angleCurve.Evaluate(this.TimeInAnimation);
            }
            if (this.def.skyfaller.rotationCurve != null)
            {
                num += this.def.skyfaller.rotationCurve.Evaluate(this.TimeInAnimation);
            }
            if (this.def.skyfaller.xPositionCurve != null)
            {
                drawLoc.x += this.def.skyfaller.xPositionCurve.Evaluate(this.TimeInAnimation);
            }
            if (this.def.skyfaller.zPositionCurve != null)
            {
                drawLoc.z += this.def.skyfaller.zPositionCurve.Evaluate(this.TimeInAnimation);
            }
            this.exactPosition = drawLoc;

            if (this.def.graphicData.drawSize != default(Vector2))
            {
                this.scale = new Vector3(def.graphicData.drawSize.x, 1f, def.graphicData.drawSize.y);
            }
            if (this.def.skyfaller.shadowSize != default(Vector2))
            {
                this.shadowScale = new Vector3(def.skyfaller.shadowSize.x, 1f, def.skyfaller.shadowSize.y);
            }
            this.matrix.SetTRS(drawLoc + Altitudes.AltIncVect, this.angle.ToQuat(), this.scale);
            Graphics.DrawMesh(MeshPool.plane10, this.matrix, this.Graphic.MatAt(flip ? thingForGraphic.Rotation.Opposite : thingForGraphic.Rotation), 0);
        //    this.Graphic.Draw(drawLoc, flip ? thingForGraphic.Rotation.Opposite : thingForGraphic.Rotation, thingForGraphic, num);
            this.DrawDropSpotShadow();
        }

        private Thing GetThingForGraphic()
        {
            if (this.def.graphicData != null || !this.innerContainer.Any)
            {
                return this;
            }
            return this.innerContainer[0];
        }

        protected new void DrawDropSpotShadow()
        {
            Material shadowMaterial = this.ShadowMaterial;
            if (shadowMaterial == null)
            {
                return;
            }
            DrawDropSpotShadow(this.ShadowDrawPos, base.Rotation, shadowMaterial, this.def.skyfaller.shadowSize, this.ticksToImpact);
        }

        public new void DrawDropSpotShadow(Vector3 center, Rot4 rot, Material material, Vector2 shadowSize, int ticksToImpact)
        {
            if (rot.IsHorizontal)
            {
                Gen.Swap<float>(ref shadowSize.x, ref shadowSize.y);
            }
            ticksToImpact = Mathf.Max(ticksToImpact, 0);
            Vector3 pos = center;
            pos.y = AltitudeLayer.Shadows.AltitudeFor();
            float num = 1f + (float)ticksToImpact / 100f;
            Vector3 s = new Vector3(num * shadowSize.x, 1f, num * shadowSize.y);
            Color white = Color.white;
            if (ticksToImpact > 150)
            {
                white.a = Mathf.InverseLerp(200f, 150f, (float)ticksToImpact);
            }
            DropShipLeaving.shadowPropertyBlock.SetColor(ShaderPropertyIDs.Color, white);
            this.shadowMatrix.SetTRS(this.ShadowDrawPos + Altitudes.AltIncVect, this.angle.ToQuat(), this.shadowScale);
            Graphics.DrawMesh(MeshPool.plane10, this.shadowMatrix, FadedMaterialPool.FadedVersionOf(this.ShadowMaterial, 0.4f * GenCelestial.CurShadowStrength(base.Map)), 0);
            Matrix4x4 matrix = default(Matrix4x4);
            /*
            matrix.SetTRS(pos, rot.AsQuat, s);
            Graphics.DrawMesh(MeshPool.plane10Back, matrix, material, 0, null, 0, DropShipLeaving.shadowPropertyBlock);
            */
        }

        private Material ShadowMaterial
        {
            get
            {
                if (this.cachedShadowMaterial == null && !this.def.skyfaller.shadow.NullOrEmpty())
                {
                    this.cachedShadowMaterial = MaterialPool.MatFrom(this.def.skyfaller.shadow, ShaderDatabase.Transparent);
                }
                return this.cachedShadowMaterial;
            }
        }

        public void ComputeDropShipRotation(IntVec3 targetPosition)
        {
            int num = base.Map.Size.x / 2;
            int num2 = base.Map.Size.z / 2;
            bool flag = targetPosition.x <= 50 || base.Map.Size.x - targetPosition.x <= 50 || targetPosition.z <= 50 || base.Map.Size.z - targetPosition.z <= 50;
            if (flag)
            {
                bool flag2 = targetPosition.x <= num && targetPosition.z >= num2;
                if (flag2)
                {
                    this.exactRotation = (float)Rand.RangeInclusive(280, 350);
                }
                else
                {
                    bool flag3 = targetPosition.x >= num && targetPosition.z >= num2;
                    if (flag3)
                    {
                        this.exactRotation = (float)Rand.RangeInclusive(10, 80);
                    }
                    else
                    {
                        bool flag4 = targetPosition.x >= num && targetPosition.z <= num2;
                        if (flag4)
                        {
                            this.exactRotation = (float)Rand.RangeInclusive(100, 170);
                        }
                        else
                        {
                            this.exactRotation = (float)Rand.RangeInclusive(190, 260);
                        }
                    }
                }
            }
            else
            {
                this.exactRotation = Rand.Range(0f, 360f);
            }
            base.Rotation = new Rot4(Mathf.RoundToInt(this.exactRotation) / 90);
        }

        /*
        public void ComputeShipExactPosition()
        {
            Vector3 a = new Vector3(0f, 0f, 1f).RotatedBy(this.exactRotation);
            this.exactPosition = this.targetPosition.ToVector3ShiftedWithAltitude(AltitudeLayer.Skyfaller);
            bool flag = this.ticksBeforeOverflight > 0;
            if (flag)
            {
                bool flag2 = this.ticksBeforeOverflight > this.airStrikeDef.ticksBeforeOverflightReducedSpeed;
                if (flag2)
                {
                    float num = (float)(this.ticksBeforeOverflight - this.airStrikeDef.ticksBeforeOverflightReducedSpeed);
                    float num2 = num * num * 0.01f;
                    this.shipToTargetDistance = (num2 + (float)this.ticksBeforeOverflight) * this.airStrikeDef.cellsTravelledPerTick;
                }
                else
                {
                    this.shipToTargetDistance = (float)this.ticksBeforeOverflight * this.airStrikeDef.cellsTravelledPerTick;
                }
                this.spaceshipExactPosition -= a * this.shipToTargetDistance;
            }
            else
            {
                bool flag3 = this.ticksAfterOverflight > this.airStrikeDef.ticksAfterOverflightReducedSpeed;
                if (flag3)
                {
                    float num3 = (float)(this.ticksAfterOverflight - this.airStrikeDef.ticksAfterOverflightReducedSpeed);
                    float num4 = num3 * num3 * 0.01f;
                    this.shipToTargetDistance = (num4 + (float)this.ticksAfterOverflight) * this.airStrikeDef.cellsTravelledPerTick;
                }
                else
                {
                    this.shipToTargetDistance = (float)this.ticksAfterOverflight * this.airStrikeDef.cellsTravelledPerTick;
                }
                this.spaceshipExactPosition += a * this.shipToTargetDistance;
            }
            this.spaceshipExactPosition += new Vector3(0f, 5.1f, 0f);
        }
        */

        public void ComputeShipShadowExactPosition()
        {
            GenCelestial.LightInfo lightSourceInfo = GenCelestial.GetLightSourceInfo(base.Map, GenCelestial.LightType.Shadow);
            this.shadowExactPosition = this.exactPosition + (Mathf.Max(0.5f, this.def.skyfaller.zPositionCurve.Evaluate(this.TimeInAnimation)) * new Vector3(lightSourceInfo.vector.x, -0.1f, lightSourceInfo.vector.y));
        }

        public override void Tick()
        {
            base.Tick();
            if (this.Map != null)
            {
                this.ComputeShipShadowExactPosition();
            }
        }

        public Vector3 exactPosition = Vector3.zero;
        public Vector3 shadowExactPosition = Vector3.zero;
        public float exactRotation = 0f;
        public Material texture = null;
        public Material shadowTexture = null;
        public Matrix4x4 matrix = default(Matrix4x4);
        public Matrix4x4 shadowMatrix = default(Matrix4x4);

        public Vector3 scale = new Vector3(11f, 1f, 20f);
        public Vector3 shadowScale = new Vector3(11f, 1f, 20f);
        private Material cachedShadowMaterial;
        private static MaterialPropertyBlock shadowPropertyBlock = new MaterialPropertyBlock();
        private bool alreadyLeft;
        private static List<Thing> tmpActiveDropPods = new List<Thing>();
    }
}
