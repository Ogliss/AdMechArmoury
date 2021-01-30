using System;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus.OrbitalStrikes
{
    // AdeptusMechanicus.OrbitalStrikes.OrbitalStrike
    public class OrbitalStrike : ThingWithComps
	{
		protected int TicksPassed
		{
			get
			{
				return Find.TickManager.TicksGame - this.startTick;
			}
		}

		protected int TicksLeft
		{
			get
			{
				return this.duration - this.TicksPassed;
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_References.Look<Thing>(ref this.instigator, "instigator", false);
			Scribe_Defs.Look<ThingDef>(ref this.weaponDef, "weaponDef");
			Scribe_Defs.Look<OrbitalStrikeDef>(ref this.strikeDef, "strikeDef");
			Scribe_Values.Look<int>(ref this.duration, "duration", 0, false);
			Scribe_Values.Look<float>(ref this.angle, "angle", 0f, false);
			Scribe_Values.Look<int>(ref this.startTick, "startTick", 0, false);
			Scribe_Values.Look<IntVec3>(ref this.targetLoc, "targetLoc", default(IntVec3), false);
		}

		public override void Draw()
		{
			base.Comps_PostDraw();
		}

		public virtual void StartStrike()
		{
			if (!base.Spawned)
			{
				Log.Error("Called StartStrike() on unspawned thing.", false);
				return;
			}
			this.angle = OrbitalStrike.AngleRange.RandomInRange;
			this.startTick = Find.TickManager.TicksGame;
			CompAffectsSky comp = base.GetComp<CompAffectsSky>();
			if (comp != null)
			{
				comp.StartFadeInHoldFadeOut(30, this.duration - 30 - 15, 15, 1f);
			}
			CompOrbitalBeam comp2 = base.GetComp<CompOrbitalBeam>();
			if (comp2 != null)
			{
				comp2.StartAnimation(this.duration, 10, this.angle);
			}
		}

		public virtual bool HitRoof(IntVec3 c)
        {
			RoofDef roofDef = base.Map.roofGrid.RoofAt(c);
			if (roofDef != null)
			{
				this.ThrowDebugText("hit-roofed-Cell", c);
				if (roofDef.isThickRoof)
				{
					this.ThrowDebugText("hit-thick-roof", c);
					if (Rand.Chance(strikeDef.roofThickCollapseChance))
					{
						this.ThrowDebugText("collapse-thick-roof", c);
						if (c.GetEdifice(base.Map) == null || c.GetEdifice(base.Map).def.Fillage != FillCategory.Full)
						{
							RoofCollapserImmediate.DropRoofInCells(c, base.Map, null);
						}
					}
                    else
					{
						this.ThrowDebugText("stopped-thick-roof", c);
						if (this.weaponDef?.projectile?.soundHitThickRoof != null)
						{
							this.weaponDef.projectile.soundHitThickRoof.PlayOneShot(new TargetInfo(c, base.Map, false));
						}
						return true;
					}
				}
				else
				{
					this.ThrowDebugText("hit-thin-roof", c);
					if (Rand.Chance(strikeDef.roofThinCollapseChance))
					{
						this.ThrowDebugText("collapse-thin-roof", c);
						if (c.GetEdifice(base.Map) == null || c.GetEdifice(base.Map).def.Fillage != FillCategory.Full)
						{
							RoofCollapserImmediate.DropRoofInCells(c, base.Map, null);
						}
					}
					else
					{
						this.ThrowDebugText("stopped-thin-roof", c);
                        if (this.weaponDef?.projectile?.soundHitThickRoof != null)
						{
							this.weaponDef.projectile.soundHitThickRoof.PlayOneShot(new TargetInfo(c, base.Map, false));
						}
						return true;
					}
				}
			}
			return false;
		}

		private void ThrowDebugText(string text, IntVec3 c)
		{
			if (DebugViewSettings.drawShooting)
			{
				MoteMaker.ThrowText(c.ToVector3Shifted(), base.Map, text, -1f);
				Log.Message("loc: "+c.ToVector3Shifted() +" "+ text);
			}
		}
		public override void Tick()
		{
			base.Tick();
			if (this.TicksPassed >= this.duration)
			{
				this.Destroy(DestroyMode.Vanish);
			}
		}

		public float impactAreaRadius = 15f;
		public FloatRange explosionRadiusRange = new FloatRange(6f, 8f);
		public int randomFireRadius = 25;
		public int bombIntervalTicks = 18;
		public int warmupTicks = 60;
		public int explosionCount = 30;
		public IntVec3 targetLoc;
		public int duration;
		public Thing instigator;
		public ThingDef weaponDef;
		public OrbitalStrikeDef strikeDef;
		private float angle;
		private int startTick;
		private static readonly FloatRange AngleRange = new FloatRange(-12f, 12f);
		private const int SkyColorFadeInTicks = 30;
		private const int SkyColorFadeOutTicks = 15;
		private const int OrbitalBeamFadeOutTicks = 10;
	}
}
