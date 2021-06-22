using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace AdeptusMechanicus
{
	public class PawnThrown : Thing, IThingHolder
	{
		public Pawn ThrownPawn
		{
			get
			{
				if (this.innerContainer.InnerListForReading.Count <= 0)
				{
					return null;
				}
				return this.innerContainer.InnerListForReading[0] as Pawn;
			}
		}

		public ThingOwner GetDirectlyHeldThings()
		{
			return this.innerContainer;
		}

		public PawnThrown()
		{
			this.innerContainer = new ThingOwner<Thing>(this);
		}

		public void GetChildHolders(List<IThingHolder> outChildren)
		{
			ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, this.GetDirectlyHeldThings());
		}

		public Vector3 DestinationPos
		{
			get
			{
				Pawn thrownPawn = this.ThrownPawn;
				return GenThing.TrueCenter(base.Position, thrownPawn.Rotation, thrownPawn.def.size, thrownPawn.def.Altitude);
			}
		}

		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			if (!respawningAfterLoad)
			{
				float num = Mathf.Max(this.throwDistance, 1f) / this.def.pawnFlyer.flightSpeed;
				num = Mathf.Max(num, this.def.pawnFlyer.flightDurationMin);
				this.ticksThrownTime = num.SecondsToTicks();
				this.ticksThrown = 0;
			}
		}

		protected virtual void RespawnPawn()
		{
			Pawn thrownPawn = this.ThrownPawn;
			Thing thing;
			this.innerContainer.TryDrop_NewTmp(thrownPawn, base.Position, thrownPawn.MapHeld, ThingPlaceMode.Direct, out thing, null, null, false);
			if (thrownPawn.drafter != null)
			{
				thrownPawn.drafter.Drafted = this.pawnWasDrafted;
			}
			if (this.pawnWasSelected && Find.CurrentMap == thrownPawn.Map)
			{
				Find.Selector.Unshelve(thrownPawn, false, true);
			}
			if (this.jobQueue != null)
			{
				thrownPawn.jobs.RestoreCapturedJobs(this.jobQueue, true);
			}
		}

		public override void Tick()
		{
			if (this.ticksThrown >= this.ticksThrownTime)
			{
				this.ImpactSomething();
				//	this.Destroy(DestroyMode.Vanish);
			}
			else
			{
				if (this.ticksThrown % 5 == 0)
				{
					this.CheckDestination();
				}
				this.innerContainer.ThingOwnerTick(true);
			}
			this.ticksThrown++;
		}

		private void ImpactSomething()
		{
			if (this.usedTarget != null)
			{
				Pawn pawn = this.usedTarget as Pawn;
				Rand.PushState();
				bool flag2 = pawn != null /* && PawnUtility.GetPosture(pawn) != null */ && GenGeo.MagnitudeHorizontalSquared(this.startVec - this.DestinationPos) >= 20.25f && Rand.Value > 0.2f;
				Rand.PopState();
				if (flag2)
				{
					this.Impact(null);
				}
				else
				{
					this.Impact(this.usedTarget);
				}
			}
			else
			{
				this.Impact(null);
			}
		}


		protected virtual void Impact(Thing hitThing)
		{
			if (hitThing == null)
			{
				Pawn pawn;
				if ((pawn = (GridsUtility.GetThingList(base.Position, base.Map).FirstOrDefault((Thing x) => x == this.usedTarget) as Pawn)) != null)
				{
					hitThing = pawn;
				}
			}
			if (this.impactDamage != null)
			{
				if (this.damageLaunched)
				{
					this.ThrownPawn.TakeDamage(this.impactDamage.Value);
				}
				else
				{
					hitThing.TakeDamage(this.impactDamage.Value);
				}
			}
			this.RespawnPawn();
			if (this.def.projectile.explosionRadius > 0)
			{
				GenExplosion.DoExplosion(base.Position, base.Map, this.def.projectile.explosionRadius, this.def.projectile.damageDef, ThrownPawn, -1, -1f, null, null, null, null, null, 0f, 1, false, null, 0f, 1, 0f, false);
			}
			this.Destroy(0);
		}

		private void CheckDestination()
		{
			if (!PawnThrown.ValidJumpTarget(base.Map, base.Position))
			{
				int num = GenRadial.NumCellsInRadius(3.9f);
				for (int i = 0; i < num; i++)
				{
					IntVec3 intVec = base.Position + GenRadial.RadialPattern[i];
					if (PawnThrown.ValidJumpTarget(base.Map, intVec))
					{
						base.Position = intVec;
						return;
					}
				}
			}
		}
		public static bool ValidJumpTarget(Map map, IntVec3 cell)
		{
			if (!cell.IsValid || !cell.InBounds(map))
			{
				return false;
			}
			if (cell.Impassable(map) || !cell.Walkable(map) || cell.Fogged(map))
			{
				return false;
			}
			Building edifice = cell.GetEdifice(map);
			Building_Door building_Door;
			return edifice == null || (building_Door = (edifice as Building_Door)) == null || building_Door.Open;
		}

		public static PawnThrown MakeThrown(ThingDef thrownDef, Pawn pawn, IntVec3 destCell, DamageInfo? newDamageInfo = null)
		{
			PawnThrown pawnthrown = (PawnThrown)ThingMaker.MakeThing(thrownDef, null);
			if (!pawnthrown.ValidateThrower())
			{
				return null;
			}
			pawnthrown.impactDamage = newDamageInfo;
			pawnthrown.startVec = pawn.TrueCenter();
			pawnthrown.throwDistance = pawn.Position.DistanceTo(destCell);
			pawnthrown.pawnWasDrafted = pawn.Drafted;
			pawnthrown.pawnWasSelected = Find.Selector.IsSelected(pawn);
			if (pawnthrown.pawnWasDrafted)
			{
				Find.Selector.ShelveSelected(pawn);
			}
			pawnthrown.jobQueue = pawn.jobs.CaptureAndClearJobQueue();
			pawn.DeSpawn(DestroyMode.Vanish);
			if (!pawnthrown.innerContainer.TryAdd(pawn, true))
			{
				Log.Error("Could not add " + pawn.ToStringSafe<Pawn>() + " to a thrower.", false);
				pawn.Destroy(DestroyMode.Vanish);
			}
			return pawnthrown;
		}

		protected virtual bool ValidateThrower()
		{
			return true;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Deep.Look<ThingOwner<Thing>>(ref this.innerContainer, "innerContainer", new object[]
			{
				this
			});
			Scribe_Values.Look<Vector3>(ref this.startVec, "startVec", default(Vector3), false);
			Scribe_Values.Look<float>(ref this.throwDistance, "throwDistance", 0f, false);
			Scribe_Values.Look<bool>(ref this.pawnWasDrafted, "pawnWasDrafted", false, false);
			Scribe_Values.Look<bool>(ref this.pawnWasSelected, "pawnWasSelected", false, false);
			Scribe_Values.Look<int>(ref this.ticksThrownTime, "ticksThrownTime", 0, false);
			Scribe_Values.Look<int>(ref this.ticksThrown, "ticksThrown", 0, false);
			Scribe_Deep.Look<JobQueue>(ref this.jobQueue, "jobQueue", Array.Empty<object>());
			Scribe_References.Look<Thing>(ref this.usedTarget, "usedTarget", false);
		}

		public DamageInfo? impactDamage;
		protected Thing usedTarget;
		public bool damageLaunched = true;
		private ThingOwner<Thing> innerContainer;
		protected Vector3 startVec;
		private float throwDistance;
		private bool pawnWasDrafted;
		private bool pawnWasSelected;
		protected int ticksThrownTime = 120;
		protected int ticksThrown;
		private JobQueue jobQueue;
	}
}
