using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public class CompProperties_HatcherHorde : CompProperties
    {
        public CompProperties_HatcherHorde()
        {
            this.compClass = typeof(CompHatcherHorde);
		}
		public float hatcherDaystoHatch = 1f;
        public int minForHorde = 20;
		public int hordeSpawns = 20;
		public float hordeChance = 1f;
		public PawnKindDef hatcherPawn;
		public PawnKindDef hordePawn;


	}

    public class CompHatcherHorde : ThingComp
    {
        public CompProperties_HatcherHorde Props => (CompProperties_HatcherHorde)this.props;

		private CompTemperatureRuinable FreezerComp
		{
			get
			{
				return this.parent.GetComp<CompTemperatureRuinable>();
			}
		}

		public bool TemperatureDamaged
		{
			get
			{
				return this.FreezerComp != null && this.FreezerComp.Ruined;
			}
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look<float>(ref this.gestateProgress, "gestateProgress", 0f, false);
			Scribe_References.Look<Pawn>(ref this.hatcheeParent, "hatcheeParent", false);
			Scribe_References.Look<Pawn>(ref this.otherParent, "otherParent", false);
			Scribe_References.Look<Faction>(ref this.hatcheeFaction, "hatcheeFaction", false);
		}

		public override void CompTick()
		{
			if (!this.TemperatureDamaged)
			{
				float num = 1f / (this.Props.hatcherDaystoHatch * 60000f);
				this.gestateProgress += num;
				if (this.gestateProgress > 1f)
				{
					this.Hatch();
				}
			}
		}

		public void Hatch()
		{
			try
			{
				int present = this.parent.Map?.mapPawns.AllPawns.Where(x => x.def == this.Props.hatcherPawn.race).Count() ?? 0;
				int spawnCount = this.parent.stackCount;
				PawnKindDef spawnKind = this.Props.hatcherPawn;
				if (present > this.Props.minForHorde)
				{
					Rand.PushState();
					if (Rand.Chance(this.Props.hordeChance))
					{
						spawnCount += this.Props.hordeSpawns;
						spawnKind = this.Props.hordePawn;
					}
					Rand.PopState();

				}
				PawnGenerationRequest request = new PawnGenerationRequest(spawnKind, this.hatcheeFaction, PawnGenerationContext.NonPlayer, -1, false, false, false, true, false, 1f, false, true, true, true, false, false, false, false, false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null);

				for (int i = 0; i < spawnCount; i++)
				{
				//	Log.Message("Spawning pawn "+(i+1)+ " of " +spawnCount);
					Pawn pawn = PawnGenerator.GeneratePawn(request);
					if (PawnUtility.TrySpawnHatchedOrBornPawn(pawn, this.parent))
					{
						if (pawn != null)
						{
							if (this.hatcheeParent != null)
							{
								if (pawn.playerSettings != null && this.hatcheeParent.playerSettings != null && this.hatcheeParent.Faction == this.hatcheeFaction)
								{
									pawn.playerSettings.AreaRestriction = this.hatcheeParent.playerSettings.AreaRestriction;
								}
								if (pawn.RaceProps.IsFlesh)
								{
									pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, this.hatcheeParent);
								}
							}
							if (this.otherParent != null && (this.hatcheeParent == null || this.hatcheeParent.gender != this.otherParent.gender) && pawn.RaceProps.IsFlesh)
							{
								pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, this.otherParent);
							}
						}
						if (this.parent.Spawned)
						{
							FilthMaker.TryMakeFilth(this.parent.Position, this.parent.Map, ThingDefOf.Filth_AmnioticFluid, 1, FilthSourceFlags.None);
						}
					}
					else
					{
						Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Discard);
					}
				}
			}
			finally
			{
				this.parent.Destroy(DestroyMode.Vanish);
			}
		}

		public override void PreAbsorbStack(Thing otherStack, int count)
		{
			float t = (float)count / (float)(this.parent.stackCount + count);
			float b = ((ThingWithComps)otherStack).GetComp<CompHatcherHorde>().gestateProgress;
			this.gestateProgress = Mathf.Lerp(this.gestateProgress, b, t);
		}

		public override void PostSplitOff(Thing piece)
		{
			CompHatcherHorde comp = ((ThingWithComps)piece).GetComp<CompHatcherHorde>();
			comp.gestateProgress = this.gestateProgress;
			comp.hatcheeParent = this.hatcheeParent;
			comp.otherParent = this.otherParent;
			comp.hatcheeFaction = this.hatcheeFaction;
		}

		public override void PrePreTraded(TradeAction action, Pawn playerNegotiator, ITrader trader)
		{
			base.PrePreTraded(action, playerNegotiator, trader);
			if (action == TradeAction.PlayerBuys)
			{
				this.hatcheeFaction = Faction.OfPlayer;
				return;
			}
			if (action == TradeAction.PlayerSells)
			{
				this.hatcheeFaction = trader.Faction;
			}
		}

		public override void PostPostGeneratedForTrader(TraderKindDef trader, int forTile, Faction forFaction)
		{
			base.PostPostGeneratedForTrader(trader, forTile, forFaction);
			this.hatcheeFaction = forFaction;
		}

		public override string CompInspectStringExtra()
		{
			if (!this.TemperatureDamaged)
			{
				return "EggProgress".Translate() + ": " + this.gestateProgress.ToStringPercent();
			}
			return null;
		}

		private float gestateProgress;
		public Pawn hatcheeParent;
		public Pawn otherParent;
		public Faction hatcheeFaction;
	}
}
