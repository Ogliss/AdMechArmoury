using System;
using System.Collections.Generic;
using AdeptusMechanicus.ExtensionMethods;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public class EffectGas : Gas
    {
        public AdeptusGasProperties Def => this.def.gas as AdeptusGasProperties;

		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			if (this.Def.ticksPerApplication > 0)
			{
				this.interval = this.Def.ticksPerApplication;
			}
			this.delay = this.interval;
		}

		public override void Tick()
		{
			if (base.Destroyed)
			{
				return;
			}
			base.Tick();
			this.delay--;
			if (this.delay <= 0)
			{
				this.ApplyHediff();
				this.delay = this.interval;
			}
		}

		public void ApplyHediff()
		{
			if (this.Def.hediffDef == null)
			{
				return;
			}
			List<Thing> thingList = base.Position.GetThingList(base.Map);
			if (thingList.Count == 0 || thingList == null)
			{
				return;
			}
			for (int i = 0; i < thingList.Count; i++)
			{
				Pawn pawn = thingList[i] as Pawn;
				if (pawn != null && pawn.Spawned)
				{
					this.AddHediffToPawn(pawn, this.Def.hediffDef, this.Def.hediffAddChance, this.Def.hediffSeverity);
				}
			}
		}

		public void AddHediffToPawn(Pawn pawn, HediffDef hediffToAdd, float chanceToAddHediff, float severityToAdd)
		{
			if (!this.PawnCanBeAffected(pawn))
			{
				return;
			}
			Rand.PushState();
			bool exit = !Rand.Chance(chanceToAddHediff);
			Rand.PopState();
			if (exit || severityToAdd <= 0f)
			{
				return;
			}
			float statValue = pawn.GetStatValue(StatDefOf.ToxicResistance, true);
			Hediff hediff = HediffMaker.MakeHediff(hediffToAdd, pawn, null);
			if (!this.Def.ignoreToxicResistance)
			{
				hediff.Severity = severityToAdd * statValue;
			}
			else
			{
				hediff.Severity = severityToAdd;
			}
			if (this.Def.onlyAffectLungs)
			{
				if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Breathing))
				{
					return;
				}
				List<BodyPartRecord> list = new List<BodyPartRecord>();
				foreach (BodyPartRecord bodyPartRecord in pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Inside, null, null))
				{
					if (bodyPartRecord.def.tags.Contains(BodyPartTagDefOf.BreathingSource))
					{
						list.Add(bodyPartRecord);
					}
				}
				if (list.Count > 0)
				{
					hediff.Severity /= (float)list.Count;
					for (int i = 0; i < list.Count; i++)
					{
						BodyPartRecord bodyPartRecord2 = list[i];
						if (pawn.health.hediffSet.HasHediff(hediffToAdd, bodyPartRecord2, false))
						{
							for (int j = 0; j < pawn.health.hediffSet.hediffs.Count; j++)
							{
								if (pawn.health.hediffSet.hediffs[j].Part == bodyPartRecord2)
								{
									pawn.health.hediffSet.hediffs[j].Severity += hediff.Severity;
									break;
								}
							}
						}
						else
						{
							pawn.health.AddHediff(hediff, bodyPartRecord2, null, null);
						}
					}
					return;
				}
			}
			else
			{
				if (pawn.health.hediffSet.HasHediff(hediffToAdd, false))
				{
					pawn.health.hediffSet.GetFirstHediffOfDef(hediffToAdd, false).Severity += hediff.Severity;
					return;
				}
				pawn.health.AddHediff(hediff, null, null, null);
			}
		}

		private bool PawnCanBeAffected(Pawn pawn)
		{
			return (!this.Def.ignoreAnimals || !pawn.RaceProps.Animal) && (!this.Def.ignoreInsectFlesh || pawn.RaceProps.FleshType != FleshTypeDefOf.Insectoid) && (!this.Def.ignoreMechanoidFlesh || pawn.RaceProps.FleshType != FleshTypeDefOf.Mechanoid) && (!this.Def.ignoreNormalFlesh || pawn.RaceProps.FleshType != FleshTypeDefOf.Normal);
		}

		private int delay = 30;
		private int interval = 30;
	}
}
