using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus.AirStrikes
{
	// Token: 0x02000040 RID: 64
	[StaticConstructorOnStartup]
	public class FlyingSpaceshipAirStrike : FlyingSpaceship
	{
		// Token: 0x06000139 RID: 313 RVA: 0x0000B3CC File Offset: 0x000095CC
		public void InitializeAirStrikeData(IntVec3 targetPosition, AirStrikeDef airStrikeDef)
		{
			this.targetPosition = targetPosition;
			this.airStrikeDef = airStrikeDef;
			this.ticksBeforeOverflight = this.airStrikeDef.ticksBeforeOverflightInitialValue;
			this.ticksAfterOverflight = 0;
			this.spaceshipKind = SpaceshipKind.Airstrike;
			this.ComputeAirStrikeRotation(this.targetPosition);
			base.ConfigureShipTexture(this.spaceshipKind, airStrikeDef);
			this.spaceshipScale = new Vector3(airStrikeDef.scale.x, 1f, airStrikeDef.scale.y);
			base.Tick();
		}

		private Graphic graphicint;
		public override Graphic Graphic
		{
			get
			{
				if (airStrikeDef.graphicData!=null && graphicint == null)
				{
					graphicint = airStrikeDef.graphicData.GraphicColoredFor(this);
				}
                if (graphicint != null)
                {
					return graphicint;
				}
				return base.Graphic;

			}
		}

		// Token: 0x0600013A RID: 314 RVA: 0x0000B428 File Offset: 0x00009628
		public void ComputeAirStrikeRotation(IntVec3 targetPosition)
		{
			int num = base.Map.Size.x / 2;
			int num2 = base.Map.Size.z / 2;
			bool flag = targetPosition.x <= 50 || base.Map.Size.x - targetPosition.x <= 50 || targetPosition.z <= 50 || base.Map.Size.z - targetPosition.z <= 50;
			if (flag)
			{
				bool flag2 = targetPosition.x <= num && targetPosition.z >= num2;
				if (flag2)
				{
					this.spaceshipExactRotation = (float)Rand.RangeInclusive(280, 350);
				}
				else
				{
					bool flag3 = targetPosition.x >= num && targetPosition.z >= num2;
					if (flag3)
					{
						this.spaceshipExactRotation = (float)Rand.RangeInclusive(10, 80);
					}
					else
					{
						bool flag4 = targetPosition.x >= num && targetPosition.z <= num2;
						if (flag4)
						{
							this.spaceshipExactRotation = (float)Rand.RangeInclusive(100, 170);
						}
						else
						{
							this.spaceshipExactRotation = (float)Rand.RangeInclusive(190, 260);
						}
					}
				}
			}
			else
			{
				this.spaceshipExactRotation = Rand.Range(0f, 360f);
			}
			base.Rotation = new Rot4(Mathf.RoundToInt(this.spaceshipExactRotation) / 90);
		}

		// Token: 0x0600012B RID: 299 RVA: 0x0000AA80 File Offset: 0x00008C80
		public override void Draw()
		{
			this.spaceshipMatrix.SetTRS(this.DrawPos + Altitudes.AltIncVect, this.spaceshipExactRotation.ToQuat(), this.spaceshipScale);
			Graphics.DrawMesh(MeshPool.plane10, this.spaceshipMatrix, this.spaceshipTexture, 0);
			this.spaceshipShadowMatrix.SetTRS(this.ShadowDrawPos + Altitudes.AltIncVect, this.spaceshipExactRotation.ToQuat(), this.spaceshipShadowScale);
			Graphics.DrawMesh(MeshPool.plane10, this.spaceshipShadowMatrix, FadedMaterialPool.FadedVersionOf(this.spaceshipShadowTexture, 0.4f * GenCelestial.CurShadowStrength(base.Map)), 0);
		}

		// Token: 0x0600013B RID: 315 RVA: 0x0000B5A0 File Offset: 0x000097A0
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.ticksBeforeOverflight, "ticksBeforeOverflight", 0, false);
			Scribe_Values.Look<int>(ref this.ticksAfterOverflight, "ticksAfterOverflight", 0, false);
			Scribe_Values.Look<IntVec3>(ref this.targetPosition, "targetPosition", default(IntVec3), false);
			Scribe_Defs.Look<AirStrikeDef>(ref this.airStrikeDef, "airStrikeDef");
			Scribe_Values.Look<float>(ref this.spaceshipExactRotation, "shipRotation", 0f, false);
			Scribe_Collections.Look<int>(ref this.weaponRemainingRounds, "weaponRemainingRounds", LookMode.Undefined, Array.Empty<object>());
			Scribe_Collections.Look<int>(ref this.weaponNextShotTick, "weaponNextShotTick", LookMode.Undefined, Array.Empty<object>());
		}

		// Token: 0x0600013C RID: 316 RVA: 0x0000B64C File Offset: 0x0000984C
		public override void Tick()
		{
			base.Tick();
			bool flag = this.ticksBeforeOverflight == this.airStrikeDef.ticksBeforeOverflightPlaySound;
			if (flag)
			{
				FlyingSpaceshipAirStrike.airStrikeSound.PlayOneShot(new TargetInfo(this.targetPosition, base.Map, false));
			}
			for (int i = 0; i < this.airStrikeDef.weapons.Count; i++)
			{
				this.WeaponTick(i, this.airStrikeDef.weapons[i]);
			}
			bool flag2 = this.ticksBeforeOverflight > 0;
			if (flag2)
			{
				this.ticksBeforeOverflight--;
			}
			else
			{
				this.ticksAfterOverflight++;
				bool flag3 = this.ticksAfterOverflight >= this.airStrikeDef.ticksAfterOverflightFinalValue || !this.spaceshipExactPosition.InBounds(base.Map);
				if (flag3)
				{
					this.Destroy(DestroyMode.Vanish);
				}
			}
		}

		// Token: 0x0600013D RID: 317 RVA: 0x0000B740 File Offset: 0x00009940
		public override void ComputeShipExactPosition()
		{
			Vector3 a = new Vector3(0f, 0f, 1f).RotatedBy(this.spaceshipExactRotation);
			this.spaceshipExactPosition = this.targetPosition.ToVector3ShiftedWithAltitude(AltitudeLayer.Skyfaller);
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

		// Token: 0x0600013E RID: 318 RVA: 0x0000B8E0 File Offset: 0x00009AE0
		public override void ComputeShipShadowExactPosition()
		{
			GenCelestial.LightInfo lightSourceInfo = GenCelestial.GetLightSourceInfo(base.Map, GenCelestial.LightType.Shadow);
			this.spaceshipShadowExactPosition = this.spaceshipExactPosition + 2f * new Vector3(lightSourceInfo.vector.x, -0.1f, lightSourceInfo.vector.y);
		}

		// Token: 0x0600013F RID: 319 RVA: 0x0000B936 File Offset: 0x00009B36
		public override void ComputeShipExactRotation()
		{
		}

		// Token: 0x06000140 RID: 320 RVA: 0x0000B936 File Offset: 0x00009B36
		public override void ComputeShipScale()
		{
		}

		// Token: 0x06000141 RID: 321 RVA: 0x0000B93C File Offset: 0x00009B3C
		public override void SetShipVisibleAboveFog()
		{
			bool flag = base.IsInBoundsAndVisible();
			if (flag)
			{
				base.Position = this.spaceshipExactPosition.ToIntVec3();
			}
			else
			{
				base.Position = this.targetPosition;
			}
		}

		// Token: 0x06000142 RID: 322 RVA: 0x0000B97C File Offset: 0x00009B7C
		public void WeaponTick(int weaponIndex, WeaponDef weaponDef)
		{
			bool flag = weaponDef.ammoDef != null && this.weaponRemainingRounds[weaponIndex] == -1 && this.shipToTargetDistance <= weaponDef.startShootingDistance;
			if (flag)
			{
				this.weaponRemainingRounds[weaponIndex] = weaponDef.ammoQuantity;
				this.weaponNextShotTick[weaponIndex] = Find.TickManager.TicksGame;
				int num = Rand.RangeInclusive(0, 1);
				bool flag2 = num == 1;
				if (flag2)
				{
					this.weaponShootRight[weaponIndex] = true;
				}
				else
				{
					this.weaponShootRight[weaponIndex] = false;
				}
				bool flag3 = weaponDef.disableRainDurationInTicks > 0;
				if (flag3)
				{
					base.Map.weatherDecider.DisableRainFor(weaponDef.disableRainDurationInTicks);
				}
			}
			bool flag4 = this.weaponRemainingRounds[weaponIndex] > 0 && Find.TickManager.TicksGame >= this.weaponNextShotTick[weaponIndex];
			if (flag4)
			{
				bool flag5 = !weaponDef.isTwinGun || this.weaponShootRight[weaponIndex];
				float num2;
				if (flag5)
				{
					num2 = 1f;
				}
				else
				{
					num2 = -1f;
				}
				Vector3 vector = this.spaceshipExactPosition + new Vector3(num2 * weaponDef.horizontalPositionOffset, 0f, weaponDef.verticalPositionOffset).RotatedBy(this.spaceshipExactRotation);
				Vector3 vector2 = vector + new Vector3(0f, 0f, weaponDef.ammoTravelDistance).RotatedBy(this.spaceshipExactRotation);
				bool flag6 = vector.InBounds(base.Map) && vector2.InBounds(base.Map);
				if (flag6)
				{
					Projectile projectile = GenSpawn.Spawn(weaponDef.ammoDef, vector.ToIntVec3(), base.Map, WipeMode.Vanish) as Projectile;
					bool flag7 = weaponDef.soundCastDef != null;
					if (flag7)
					{
						weaponDef.soundCastDef.PlayOneShot(new TargetInfo(vector.ToIntVec3(), base.Map, false));
					}
					MoteMaker.MakeStaticMote(vector, base.Map, ThingDefOf.Mote_ShotFlash, 10f);
					Pawn pawn = null;
					bool flag8 = weaponDef.targetAcquireRange > 0f;
					if (flag8)
					{
						pawn = this.GetRandomeHostilePawnAround(vector2, weaponDef.targetAcquireRange);
					}
					bool flag9 = pawn != null;
					if (flag9)
					{
						projectile.Launch(this, vector, pawn, pawn, ProjectileHitFlags.IntendedTarget, null, null);
					}
					else
					{
						vector2 += new Vector3(Rand.Range(-weaponDef.targetAcquireRange, weaponDef.targetAcquireRange), 0f, 0f).RotatedBy(this.spaceshipExactRotation);
						projectile.Launch(this, vector, vector2.ToIntVec3(), vector2.ToIntVec3(), ProjectileHitFlags.None, null, null);
					}
				}
				List<int> list = this.weaponRemainingRounds;
				int num3 = list[weaponIndex];
				list[weaponIndex] = num3 - 1;
				this.weaponNextShotTick[weaponIndex] = Find.TickManager.TicksGame + weaponDef.ticksBetweenShots;
				this.weaponShootRight[weaponIndex] = !this.weaponShootRight[weaponIndex];
			}
		}

		// Token: 0x06000143 RID: 323 RVA: 0x0000BCAC File Offset: 0x00009EAC
		public Pawn GetRandomeHostilePawnAround(Vector3 center, float radius)
		{
			List<Pawn> list = new List<Pawn>();
			foreach (IntVec3 c in GenRadial.RadialCellsAround(center.ToIntVec3(), radius, true))
			{
				bool flag = !c.InBounds(base.Map);
				if (!flag)
				{
					Pawn firstPawn = c.GetFirstPawn(base.Map);
					bool flag2 = firstPawn != null && firstPawn.HostileTo(Faction.OfPlayer) && !firstPawn.health.Downed;
					if (flag2)
					{
						list.Add(firstPawn);
					}
				}
			}
			bool flag3 = list.Count > 0;
			Pawn result;
			if (flag3)
			{
				result = list.RandomElement<Pawn>();
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x040000A7 RID: 167
		public int ticksBeforeOverflight = 0;

		// Token: 0x040000A8 RID: 168
		public int ticksAfterOverflight = 0;

		// Token: 0x040000A9 RID: 169
		public AirStrikeDef airStrikeDef = null;

		// Token: 0x040000AA RID: 170
		public IntVec3 targetPosition = IntVec3.Invalid;

		// Token: 0x040000AB RID: 171
		public float shipToTargetDistance = 0f;

		// Token: 0x040000AC RID: 172
		public List<int> weaponRemainingRounds = new List<int>(3)
		{
			-1,
			-1,
			-1
		};

		// Token: 0x040000AD RID: 173
		public List<int> weaponNextShotTick = new List<int>(3)
		{
			0,
			0,
			0
		};

		// Token: 0x040000AE RID: 174
		public List<bool> weaponShootRight = new List<bool>(3)
		{
			true,
			true,
			true
		};

		// Token: 0x040000AF RID: 175
		public static readonly SoundDef airStrikeSound = SoundDef.Named("AirStrike");
	}
}
