using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.DamageGas
    public class DamageGas : Gas
	{
		public AdeptusGasProperties Def => this.def.gas as AdeptusGasProperties;
		private DamageDef DamageDef => Def.damageDef ?? DamageDefOf.Burn;
		public object cachedLabelMouseover { get; private set; }
		private int Damage
		{
			get
			{
				int dmg = Def.damage;
				if (Def.damageDef != null && Def.damageDef.defaultDamage > 0)
				{
					dmg = Def.damageDef.defaultDamage;

				}
				return dmg;
			}
		}
		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
		}

		public override void Tick()
		{
			if (base.Destroyed)
			{
				return;
			}
			base.Tick();
			if (this.destroyTick <= Find.TickManager.TicksGame && !base.Destroyed)
			{
				this.Destroy(DestroyMode.Vanish);
				return;
			}
			this.graphicRotation += this.graphicRotationSpeed;
			this.delay--;
			if (this.delay <= 0)
			{
				this.EffectTick();
				this.delay = this.interval;
			}
		}

		public void EffectTick()
		{
			if (base.Destroyed)
			{
				return;
			}
			List<Thing> thingList = base.Position.GetThingList(base.Map);
			for (int i = 0; i < thingList.Count; i++)
			{
				if (thingList[i] != null)
				{
					Thing thing = thingList[i];
					Pawn pawn = thingList[i] as Pawn;
					if (thing != null && !this.touchingThings.Contains(thing) && thing.def.useHitPoints && !Def.damagePawnsOnly)
					{
					//	if (Find.Selector.SelectedObjects.Contains(thing)) Log.Message("Damaging thing " + thing);
						this.touchingThings.Add(thing);
						this.DamageEntities(thing, Mathf.RoundToInt((float)this.Damage * Rand.Range(0.5f, 1.25f)));
						if (Def.damageMote) FleckMaker.ThrowDustPuff(thing.Position, base.Map, 0.2f);
					}
					if (pawn != null)
					{
						this.touchingPawns.Add(pawn);
					//	if (Find.Selector.SelectedObjects.Contains(pawn)) Log.Message("Damaging pawn " + pawn);
						this.DamagePawn(pawn);
						if (Def.damageMote) FleckMaker.ThrowDustPuff(pawn.Position, base.Map, 0.2f);
					}
				}
			}
			for (int j = 0; j < this.touchingPawns.Count; j++)
			{
				Pawn pawn2 = this.touchingPawns[j];
				if (!pawn2.Spawned || pawn2.Position != base.Position)
				{
					this.touchingPawns.Remove(pawn2);
				}
				else/* if (!pawn2.RaceProps.Animal)*/
				{
				//	if (Find.Selector.SelectedObjects.Contains(pawn2)) Log.Message("Damaging pawn2 " + pawn2);
					this.DamagePawn(pawn2);
				}
			}
			for (int k = 0; k < this.touchingThings.Count; k++)
			{
				Thing thing2 = this.touchingThings[k];
				if (!thing2.Spawned || thing2.Position != base.Position || Def.damagePawnsOnly)
				{
					this.touchingThings.Remove(thing2);
				}
				else
				{
				//	if (Find.Selector.SelectedObjects.Contains(thing2)) Log.Message("Damaging thing2 " + thing2);
					this.DamageEntities(thing2, Mathf.RoundToInt((float)this.Damage * Rand.Range(0.5f, 1.25f)));
				}
			}
			if (Def.damageBuildings && !Def.damagePawnsOnly)
			{
				this.DamageBuildings(Mathf.RoundToInt((float)this.Damage * Rand.Range(0.5f, 1.25f)));
			}
			this.cachedLabelMouseover = null;
		}

		public void DamageEntities(Thing thingToDamage, int amt)
		{
			DamageInfo dinfo = new DamageInfo(DamageDef, (float)amt, 0f, -1f, null, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null);
			if (thingToDamage != null)
			{
				thingToDamage.TakeDamage(dinfo);
				if (Def.damageMote) FleckMaker.ThrowDustPuff(thingToDamage.Position, base.Map, 0.2f);
			}
		}

		public void DamageBuildings(int amt)
		{
			IntVec3 c = this.RandomAdjacentCell8Way();
			if (c.InBounds(base.Map))
			{
				Building firstBuilding = c.GetFirstBuilding(base.Map);
				DamageInfo dinfo = new DamageInfo(DamageDef, (float)amt, 0f, -1f, null, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null);
				if (firstBuilding != null)
				{
				//	if (Find.Selector.SelectedObjects.Contains(firstBuilding)) Log.Message("Damaging firstBuilding " + firstBuilding);
					firstBuilding.TakeDamage(dinfo);
					if (Def.damageMote) FleckMaker.ThrowDustPuff(firstBuilding.Position, base.Map, 0.2f);
				}
			}
		}

		public void DamagePawn(Pawn pawn)
		{
			if (!this.PawnCanBeAffected(pawn))
			{
				return;
			}
			List<BodyPartRecord> list = new List<BodyPartRecord>();
			int num = Mathf.RoundToInt((float)this.Damage * Rand.Range(0.5f, 1.25f));
			DamageInfo dinfo = default(DamageInfo);
			if (Def.damageMote) FleckMaker.ThrowDustPuff(pawn.Position, base.Map, 0.2f);

			List<Apparel> wornApparel = new List<Apparel>();
			if (pawn.apparel != null)
			{
				if (pawn.apparel.WornApparel != null)
				{
					wornApparel = pawn.apparel.WornApparel;
				}
			}

			foreach (BodyPartRecord bodyPartRecord in pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Outside, null, null))
			{
				if (wornApparel.Count > 0)
				{
					bool flag = false;
					for (int i = 0; i < wornApparel.Count; i++)
					{
						if (wornApparel[i].def.apparel.CoversBodyPart(bodyPartRecord))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						list.Add(bodyPartRecord);
					}
				}
				else
				{
					list.Add(bodyPartRecord);
				}
			}
			for (int j = 0; j < wornApparel.Count; j++)
			{
				this.DamageEntities(wornApparel[j], num);
			}
			for (int k = 0; k < list.Count; k++)
			{
				dinfo = new DamageInfo(DamageDef, (float)Mathf.RoundToInt((float)num * list[k].coverage), 0f, -1f, this, list[k], this.def, DamageInfo.SourceCategory.ThingOrUnknown, null);
				pawn.TakeDamage(dinfo);
			}
		}
		/*
		private bool ThingCanBeAffected(Thing thing)
		{
			return (!this.Def.ignoreAnimals || !thing.RaceProps.Animal) && (!this.Def.ignoreInsectFlesh || thing.RaceProps.FleshType != FleshTypeDefOf.Insectoid) && (!this.Def.ignoreMechanoidFlesh || thing.RaceProps.FleshType != FleshTypeDefOf.Mechanoid) && (!this.Def.ignoreNormalFlesh || thing.RaceProps.FleshType != FleshTypeDefOf.Normal);
		}
		*/
		private bool PawnCanBeAffected(Pawn pawn)
		{
			return (!this.Def.ignoreAnimals || !pawn.RaceProps.Animal) && (!this.Def.ignoreInsectFlesh || pawn.RaceProps.FleshType != FleshTypeDefOf.Insectoid) && (!this.Def.ignoreMechanoidFlesh || pawn.RaceProps.FleshType != FleshTypeDefOf.Mechanoid) && (!this.Def.ignoreNormalFlesh || pawn.RaceProps.FleshType != FleshTypeDefOf.Normal) && (!this.Def.damagePawnsOnlyMoving || !pawn.pather.MovingNow) && (!this.Def.damagePawnsOnlyMovedRecently || !pawn.pather.MovedRecently(interval));
		}

		private List<Pawn> touchingPawns = new List<Pawn>();

		private List<Thing> touchingThings = new List<Thing>();

		private int delay = 100;
		private int interval = 100;
	}
}
