using AdeptusMechanicus.settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
	// AdeptusMechanicus.Projectile_Explosive_Thrown
	[StaticConstructorOnStartup]
	class Projectile_Explosive_Thrown : Projectile_Explosive
    {
		private int rotinc = 0;
		private int rotrate = Rand.Range(-3, 3);
		private int heightinc = Rand.Range(-1, 3);

		public override void Draw()
		{
			float num = this.ArcHeightFactor * GenMath.InverseParabola(this.DistanceCoveredFraction);
			Vector3 drawPos = this.DrawPos;

			Vector3 position = drawPos + new Vector3(0f, 0f, 1f) * num;
			if (this.ticksToImpact > 0 && !Find.TickManager.Paused)
			{
				this.rotinc += rotrate;
			}
			if (this.def.projectile.shadowSize > 0f)
			{
				this.DrawShadow(drawPos, num);
			}
			Graphics.DrawMesh(MeshPool.GridPlane(this.def.graphicData.drawSize), position, this.ExactRotation, this.Graphic.MatSingleFor(this), 0);
			base.Comps_PostDraw();
		}
		public override Quaternion ExactRotation
		{
			get
			{
				// Time.deltaTime

				return Quaternion.LookRotation((this.destination - this.origin).Yto0().RotatedBy(rotinc));
			}
		}
		public new float ArcHeightFactor
		{
			get
			{
				float num = this.def.projectile.arcHeightFactor + heightinc;
				float num2 = (this.destination - this.origin).MagnitudeHorizontalSquared();
				if (num * num > num2 * 0.2f * 0.2f)
				{
					num = Mathf.Sqrt(num2) * 0.2f;
				}
				return num;
			}
		}
		public new void DrawShadow(Vector3 drawLoc, float height)
		{
			if (Projectile_Explosive_Thrown.shadowMaterial == null)
			{
				return;
			}
			float num = this.def.projectile.shadowSize * Mathf.Lerp(1f, 0.6f, height);
			Vector3 s = new Vector3(num, 1f, num);
			Vector3 b = new Vector3(0f, -0.01f, 0f);
			Matrix4x4 matrix = default(Matrix4x4);
			matrix.SetTRS(drawLoc + b, Quaternion.identity, s);
			Graphics.DrawMesh(MeshPool.plane10, matrix, Projectile_Explosive_Thrown.shadowMaterial, 0);
		}

		public virtual void TrailTick()
		{
			if (AMAMod.settings.AllowProjectileTrail)
			{
				if (!Trailers.NullOrEmpty())
				{
					foreach (TrailerProjectileExtension trailer in Trailers)
					{
						if (ticksToImpact % trailer.trailerMoteInterval == 0 && (trailer.trailWhenLanded || !this.landed))
						{
							for (int ii = 0; ii < trailer.motesThrown; ii++)
							{
								//    Trail1Thrower.ThrowSmokeTrail(__instance.Position.ToVector3Shifted(), trailer.trailMoteSize, __instance.Map, trailer.trailMoteDef);

								//    TrailThrower.ThrowSmokeTrail(__instance.DrawPos, trailer.trailMoteSize * DistanceCoveredFraction(___origin, ___destination, ___ticksToImpact, __instance.def.projectile.SpeedTilesPerTick), __instance.Map, trailer.trailMoteDef, __instance);
								Color? DC = null;
								if (trailer.useGraphicColor)
								{
									DC = this.DrawColor;
								}
								else
								if (trailer.useGraphicColorTwo)
								{
									DC = this.DrawColorTwo;
								}
								TrailThrower.ThrowSprayTrail(this.DrawPos, this.Map, origin, destination, trailer.trailMoteDef, trailer.trailMoteSize, 240, this.def.projectile.SpeedTilesPerTick, DC);
							}
						}
					}
				}
			}
		}

		private List<TrailerProjectileExtension> _trailers;
		public List<TrailerProjectileExtension> Trailers
		{
			get
			{
				if (_trailers == null)
				{
					_trailers = new List<TrailerProjectileExtension>();
					if (this.def.HasModExtension<TrailerProjectileExtension>())
					{
						for (int i = 0; i < def.modExtensions.Count; i++)
						{
							if (def.modExtensions[i] is TrailerProjectileExtension trailer)
							{
								_trailers.Add(trailer);
							}
						}
					}
				}
				return _trailers;
			}
		}

		public virtual void drawGlow()
		{
			if (AMAMod.settings.AllowProjectileGlow)
			{
				if (!Glower.NullOrEmpty())
				{
					foreach (GlowerProjectileExtension glower in Glower)
					{
						glower.Glow(this, this.ExactRotation);
					}
				}
			}
		}

		private List<GlowerProjectileExtension> _glower;
		public List<GlowerProjectileExtension> Glower
		{
			get
			{
				if (_glower == null)
				{
					_glower = new List<GlowerProjectileExtension>();
					if (this.def.HasModExtension<GlowerProjectileExtension>())
					{
						for (int i = 0; i < def.modExtensions.Count; i++)
						{
							if (def.modExtensions[i] is GlowerProjectileExtension trailer)
							{
								_glower.Add(trailer);
							}
						}
					}
				}
				return _glower;
			}
		}

	}
}
