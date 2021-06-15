using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
	// Token: 0x02001598 RID: 5528
	[StaticConstructorOnStartup]
	public class DropShipIncoming : Skyfaller, IActiveDropPod, IThingHolder
	{
		public Rot4 SRTSRotation
		{
			get
			{
				return this.rotation;
			}
			set
			{
				bool flag = this.rotation == value;
				if (!flag)
				{
					this.rotation = value;
				}
			}
		}
		public ActiveDropPodInfo Contents
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
        /*
		protected override void SpawnThings()
		{
			
			if (this.Contents.spawnWipeMode == null)
			{
				base.SpawnThings();
				return;
			}
			for (int i = this.innerContainer.Count - 1; i >= 0; i--)
			{
				GenSpawn.Spawn(this.innerContainer[i], base.Position, base.Map, this.Contents.spawnWipeMode.Value);
			}
			
		}
		*/
        // Token: 0x06007932 RID: 31026 RVA: 0x00239D78 File Offset: 0x00237F78
        public override void Impact()
		{
			for (int i = 0; i < 6; i++)
			{
                AdeptusFleckMaker.ThrowDustPuff(base.Position.ToVector3Shifted() + Gen.RandomHorizontalVector(1f), base.Map, 1.2f);
			}
            AdeptusFleckMaker.ThrowLightningGlow(base.Position.ToVector3Shifted(), base.Map, 2f);
			GenClamor.DoClamor(this, 15f, ClamorDefOf.Impact);
			base.Impact();
        }
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
            this.matrix.SetTRS(drawLoc + Altitudes.AltIncVect, (-num).ToQuat(), this.scale);
            Graphics.DrawMesh(MeshPool.plane10, this.matrix, this.Graphic.MatAt(flip ? thingForGraphic.Rotation.Opposite : thingForGraphic.Rotation), 0);
            //    this.Graphic.Draw(drawLoc, flip ? thingForGraphic.Rotation.Opposite : thingForGraphic.Rotation, thingForGraphic, num);
            this.DrawDropSpotShadow(num, flip);
        }

        private Thing GetThingForGraphic()
        {
            if (this.def.graphicData != null || !this.innerContainer.Any)
            {
                return this;
            }
            return this.innerContainer[0];
        }

        protected void DrawDropSpotShadow(float num, bool flip)
        {
            Material shadowMaterial = this.ShadowMaterial;
            if (shadowMaterial == null)
            {
                return;
            }
            DrawDropSpotShadow(this.ShadowDrawPos, num, shadowMaterial, this.def.skyfaller.shadowSize, this.ticksToImpact, flip);
        }

        public void DrawDropSpotShadow(Vector3 center, float rot, Material material, Vector2 shadowSize, int ticksToImpact, bool flip)
        {
            if (base.Rotation.IsHorizontal)
            {
                Gen.Swap<float>(ref shadowSize.x, ref shadowSize.y);
            }
            ticksToImpact = Mathf.Max(ticksToImpact, 0);
            Vector3 pos = center;
            pos.y = AltitudeLayer.Shadows.AltitudeFor();
            //    float num = 1f + (float)ticksToImpact / 100f;
            float f = !this.def.skyfaller.zPositionCurve.EnumerableNullOrEmpty() ? this.def.skyfaller.zPositionCurve.Evaluate(this.TimeInAnimation) : this.TimeInAnimation;
            float num = 1f + Mathf.Max(0.25f, f) / 100f;
            //    shadow = 1f * (float)ticksToImpact / 100f;
            Vector3 s = new Vector3(num * shadowSize.x, 1f, num * shadowSize.y);
            Color white = Color.white;
            if (ticksToImpact > 150)
            {
                white.a = Mathf.InverseLerp(200f, 150f, (float)ticksToImpact);
            }
            DropShipIncoming.shadowPropertyBlock.SetColor(ShaderPropertyIDs.Color, white);
            this.shadowMatrix.SetTRS(this.ShadowDrawPos + Altitudes.AltIncVect, (-rot).ToQuat(), s);
            Graphics.DrawMesh(MeshPool.plane10, this.shadowMatrix, FadedMaterialPool.FadedVersionOf(material, 0.4f * GenCelestial.CurShadowStrength(base.Map)), 0, null, 0);
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
        public void ComputeShipShadowExactPosition()
        {
            float f = !this.def.skyfaller.zPositionCurve.EnumerableNullOrEmpty() ? this.def.skyfaller.zPositionCurve.Evaluate(this.TimeInAnimation) : this.TimeInAnimation;
            GenCelestial.LightInfo lightSourceInfo = GenCelestial.GetLightSourceInfo(base.Map, GenCelestial.LightType.Shadow);
            this.shadowExactPosition = this.exactPosition + (Mathf.Max(0.25f, f) * new Vector3(lightSourceInfo.vector.x, -0.1f, lightSourceInfo.vector.y));
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

        public override void Tick()
        {
            base.Tick();
            if (this.Map != null)
            {
                this.ComputeShipShadowExactPosition();
            }
        }

        private Rot4 rotation;
        public Vector3 exactPosition = Vector3.zero;
        public Vector3 shadowExactPosition = Vector3.zero;
        public float exactRotation = 0f;
        public Matrix4x4 matrix = default(Matrix4x4);
        public Matrix4x4 shadowMatrix = default(Matrix4x4);
        public Vector3 scale = new Vector3(10f, 1f, 10f);
        public Vector3 shadowScale = new Vector3(10f, 1f, 10f);
        private static MaterialPropertyBlock shadowPropertyBlock = new MaterialPropertyBlock();
        private Material cachedShadowMaterial;
    }
}
