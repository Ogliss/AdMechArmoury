using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus.AirStrikes
{
	// Many thanks to Rikki for allowing me to utilize their code
	[StaticConstructorOnStartup]
    public abstract class FlyingShip : ThingWithComps
	{
		// Token: 0x17000042 RID: 66
		// (get) Token: 0x0600011F RID: 287 RVA: 0x0000A794 File Offset: 0x00008994
		public override Vector3 DrawPos
		{
			get
			{
				return this.spaceshipExactPosition;
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06000120 RID: 288 RVA: 0x0000A7AC File Offset: 0x000089AC
		public Vector3 ShadowDrawPos
		{
			get
			{
				return this.spaceshipShadowExactPosition;
			}
		}

		// Token: 0x06000121 RID: 289 RVA: 0x0000A7C4 File Offset: 0x000089C4
		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			if (respawningAfterLoad)
			{
				AirStrikeIncoming airStrike = this as AirStrikeIncoming;
				if (airStrike != null)
				{
					this.ConfigureShipTexture(this.spaceshipKind, airStrike.airStrikeDef);
				}
                else
				{
					this.ConfigureShipTexture(this.spaceshipKind);
				}
			}
		}

		// Token: 0x06000122 RID: 290 RVA: 0x0000A7F0 File Offset: 0x000089F0
		public void ConfigureShipTexture(ShipKind spaceshipKind, AirStrikeDef strikeDef = null)
		{
			switch (spaceshipKind)
			{
				/*
				case SpaceshipKind.CargoPeriodic:
				case SpaceshipKind.CargoRequested:
				case SpaceshipKind.Damaged:
					this.spaceshipTexture = FlyingSpaceship.supplySpaceshipTexture;
					this.spaceshipShadowTexture = FlyingSpaceship.supplySpaceshipShadowTexture;
					this.baseSpaceshipScale = FlyingSpaceship.supplySpaceshipScale;
					break;
				case SpaceshipKind.DispatcherDrop:
				case SpaceshipKind.DispatcherPick:
					this.spaceshipTexture = FlyingSpaceship.dispatcherTexture;
					this.spaceshipShadowTexture = FlyingSpaceship.supplySpaceshipShadowTexture;
					this.baseSpaceshipScale = FlyingSpaceship.supplySpaceshipScale;
					break;
				case SpaceshipKind.Medical:
					this.spaceshipTexture = FlyingSpaceship.medicalSpaceshipTexture;
					this.spaceshipShadowTexture = FlyingSpaceship.medicalSpaceshipShadowTexture;
					this.baseSpaceshipScale = FlyingSpaceship.medicalSpaceshipScale;
					break;
					*/
				case ShipKind.Airstrike:
					this.spaceshipTexture = FlyingShip.strikeshipTexture;
					this.spaceshipShadowTexture = FlyingShip.strikeshipShadowTexture;
					this.baseSpaceshipScale = FlyingShip.supplySpaceshipScale;
					break;
				default:
					Log.ErrorOnce("Adeptus Airstrike: unhandled ShipKind (" + this.spaceshipKind.ToString() + ").", 123456784);
					break;
			}
            if (strikeDef != null && spaceshipKind == ShipKind.Airstrike)
            {
                if (strikeDef.graphicData != null)
				{
					this.spaceshipTexture = strikeDef.graphicData.GraphicColoredFor(this).MatSingle;
				}
                if (strikeDef.shadowData != null)
				{
					this.spaceshipShadowTexture = strikeDef.shadowData.GraphicColoredFor(this).MatSingle;
				}
                if (strikeDef.scale != default(Vector2))
				{
					this.spaceshipScale = new Vector3(strikeDef.scale.x, 1f, strikeDef.scale.y);
					this.spaceshipShadowScale = new Vector3(strikeDef.scale.x, 1f, strikeDef.scale.y);
				}
			}
		}

        // Token: 0x06000123 RID: 291 RVA: 0x0000A8E8 File Offset: 0x00008AE8
        public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<Vector3>(ref this.spaceshipExactPosition, "spaceshipExactPosition", default(Vector3), false);
			Scribe_Values.Look<Vector3>(ref this.spaceshipShadowExactPosition, "spaceshipShadowExactPosition", default(Vector3), false);
			Scribe_Values.Look<float>(ref this.spaceshipExactRotation, "spaceshipExactRotation", 0f, false);
			Scribe_Values.Look<ShipKind>(ref this.spaceshipKind, "spaceshipKind", ShipKind.CargoPeriodic, false);
			Scribe_Values.Look<Vector3>(ref this.spaceshipScale, "spaceshipScale", default(Vector3), false);
			Scribe_Values.Look<Vector3>(ref this.spaceshipShadowScale, "spaceshipShadowScale", default(Vector3), false);
		}

		// Token: 0x06000124 RID: 292 RVA: 0x0000A993 File Offset: 0x00008B93
		public override void Tick()
		{
			this.ComputeShipExactPosition();
			this.ComputeShipShadowExactPosition();
			this.ComputeShipExactRotation();
			this.ComputeShipScale();
			this.SetShipVisibleAboveFog();
			if (this.AllComps != null)
			{
				int i = 0;
				int count = this.AllComps.Count;
				while (i < count)
				{
					this.AllComps[i].CompTick();
					i++;
				}
			}
		}

		// Token: 0x06000125 RID: 293
		public abstract void ComputeShipExactPosition();

		// Token: 0x06000126 RID: 294
		public abstract void ComputeShipShadowExactPosition();

		// Token: 0x06000127 RID: 295
		public abstract void ComputeShipExactRotation();

		// Token: 0x06000128 RID: 296
		public abstract void ComputeShipScale();

		// Token: 0x06000129 RID: 297
		public abstract void SetShipVisibleAboveFog();

		// Token: 0x0600012A RID: 298 RVA: 0x0000A9BC File Offset: 0x00008BBC
		public bool IsInBoundsAndVisible()
		{
			return this.DrawPos.ToIntVec3().InBounds(base.Map) && !base.Map.fogGrid.IsFogged(this.DrawPos.ToIntVec3()) && this.DrawPos.ToIntVec3().x >= 10 && this.DrawPos.ToIntVec3().x < base.Map.Size.x - 10 && this.DrawPos.ToIntVec3().z >= 10 && this.DrawPos.ToIntVec3().z < base.Map.Size.z - 10;
		}

		// Token: 0x0600012B RID: 299 RVA: 0x0000AA80 File Offset: 0x00008C80
		public override void Draw()
		{
			this.spaceshipMatrix.SetTRS(this.DrawPos + Altitudes.AltIncVect, this.spaceshipExactRotation.ToQuat(), this.spaceshipScale);
			Graphics.DrawMesh(MeshPool.plane10, this.spaceshipMatrix, this.spaceshipTexture, 0);

			this.spaceshipShadowMatrix.SetTRS(this.ShadowDrawPos + Altitudes.AltIncVect, this.spaceshipExactRotation.ToQuat(), this.spaceshipShadowScale);
			Graphics.DrawMesh(MeshPool.plane10, this.spaceshipShadowMatrix, FadedMaterialPool.FadedVersionOf(this.spaceshipShadowTexture, 0.4f * GenCelestial.CurShadowStrength(base.Map)), 0);
		}


        // Token: 0x04000091 RID: 145
        public Vector3 spaceshipExactPosition = Vector3.zero;

		// Token: 0x04000092 RID: 146
		public Vector3 spaceshipShadowExactPosition = Vector3.zero;

		// Token: 0x04000093 RID: 147
		public float spaceshipExactRotation = 0f;

		// Token: 0x04000094 RID: 148
		public ShipKind spaceshipKind = ShipKind.Airstrike;

		// Token: 0x04000095 RID: 149
		public static Vector3 supplySpaceshipScale = new Vector3(11f, 1f, 20f);

		// Token: 0x04000096 RID: 150
		public static Material strikeshipTexture = MaterialPool.MatFrom("Things/Ships/Strike/StrikeShip");
		public static Material strikeshipShadowTexture = MaterialPool.MatFrom("Things/Ships/Strike/StrikeShip_Shadow");
		public static Vector3 medicalSpaceshipScale = new Vector3(7f, 1f, 11f);
		/*
		// Token: 0x04000097 RID: 151
		public static Material supplySpaceshipTexture = MaterialPool.MatFrom("Things/SupplySpaceship/SupplySpaceship");

		// Token: 0x04000098 RID: 152
		public static Material dispatcherTexture = MaterialPool.MatFrom("Things/Dispatcher/DispatcherFlying");

		// Token: 0x04000099 RID: 153
		public static Material medicalSpaceshipTexture = MaterialPool.MatFrom("Things/MedicalSpaceship/MedicalSpaceship");

		// Token: 0x0400009A RID: 154

		// Token: 0x0400009B RID: 155
		public static Material supplySpaceshipShadowTexture = MaterialPool.MatFrom("Things/SupplySpaceship/SupplySpaceshipShadow", ShaderDatabase.Transparent);

		// Token: 0x0400009C RID: 156
		public static Material medicalSpaceshipShadowTexture = MaterialPool.MatFrom("Things/MedicalSpaceship/MedicalSpaceshipShadow", ShaderDatabase.Transparent);
		*/
		// Token: 0x0400009D RID: 157
		public Material spaceshipTexture = null;

		// Token: 0x0400009E RID: 158
		public Material spaceshipShadowTexture = null;

		// Token: 0x0400009F RID: 159
		public Matrix4x4 spaceshipMatrix = default(Matrix4x4);

		// Token: 0x040000A0 RID: 160
		public Matrix4x4 spaceshipShadowMatrix = default(Matrix4x4);

		// Token: 0x040000A1 RID: 161
		public Vector3 baseSpaceshipScale = new Vector3(1f, 1f, 1f);

		// Token: 0x040000A2 RID: 162
		public Vector3 spaceshipScale = new Vector3(11f, 1f, 20f);

		// Token: 0x040000A3 RID: 163
		public Vector3 spaceshipShadowScale = new Vector3(11f, 1f, 20f);
	}
}
