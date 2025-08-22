using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // Token: AdeptusMechanicus.ScenPart_ConfigPage_ConfigureStartingPawnsSpecific
    public class ScenPart_ConfigPage_ConfigureStartingPawnsSpecific : ScenPart_ConfigPage_ConfigureStartingPawns
	{

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref this.pawnCount, "pawnses");
		}

		public override string Summary(Scenario scen)
		{
			return "ScenPart_StartWithPawns".Translate(this.pawnCount, this.pawnChoiceCount);
		}

		public override void PostIdeoChosen()
		{
			if (this.Pawns.NullOrEmpty())
			{
			//	Log.Message("Pawns NullOrEmpty");
				base.PostIdeoChosen();
				return;
			}
		//	Log.Message($"Pawns ({Pawns})");
			List<PawnKindDef> kinds = new List<PawnKindDef>();
			foreach (var item in this.Pawns)
			{
				for (int i = 0; i < item.count; i++)
				{
					kinds.Add(item.kindDef);
				}
			}
			this.pawnCount = kinds.Count;
			Find.GameInitData.startingPawnCount = this.pawnCount;
			if (ModsConfig.IdeologyActive)
			{
				Faction ofPlayerSilentFail = Faction.OfPlayerSilentFail;
				bool flag;
				if (ofPlayerSilentFail == null)
				{
					flag = (null != null);
				}
				else
				{
					FactionIdeosTracker ideos = ofPlayerSilentFail.ideos;
					flag = (((ideos != null) ? ideos.PrimaryIdeo : null) != null);
				}
				if (flag)
				{
					foreach (Precept precept in Faction.OfPlayerSilentFail.ideos.PrimaryIdeo.PreceptsListForReading)
					{
						if (precept.def.defaultDrugPolicyOverride != null)
						{
							Current.Game.drugPolicyDatabase.MakePolicyDefault(precept.def.defaultDrugPolicyOverride);
						}
					}
				}
			}
			int num = 0;
			do
			{
				StartingPawnUtility.ClearAllStartingPawns();
				foreach (var item in kinds)
				{
					//	Log.Message("Generating "+item);
					Find.GameInitData.startingAndOptionalPawns.Add(SpecificStartingPawnUtility.NewGeneratedStartingPawn(item));
				}
				num++;
				if (num > 20)
				{
					break;
				}
			}
			while (!StartingPawnUtility.WorkTypeRequirementsSatisfied());
			while (Find.GameInitData.startingAndOptionalPawns.Count < this.pawnChoiceCount)
			{
				foreach (var item in kinds)
				{
					//	Log.Message("Generating " + item);
					Find.GameInitData.startingAndOptionalPawns.Add(SpecificStartingPawnUtility.NewGeneratedStartingPawn(item));
				}
			}
			return;
		}

		/*
		public override void PostWorldGenerate()
		{
			if (this.Pawns.NullOrEmpty())
			{
				base.PostWorldGenerate();
				return;
			}
			List<PawnKindDef> kinds = new List<PawnKindDef>();
			foreach (var item in this.Pawns)
			{
				for (int i = 0; i < item.count; i++)
				{
					kinds.Add(item.kindDef);
				}
			}
			this.pawnCount = kinds.Count;
			Find.GameInitData.startingPawnCount = this.pawnCount;
			int num = 0;
			do
			{
				StartingPawnUtility.ClearAllStartingPawns();
                foreach (var item in kinds)
				{
				//	Log.Message("Generating "+item);
					Find.GameInitData.startingAndOptionalPawns.Add(SpecificStartingPawnUtility.NewGeneratedStartingPawn(item));
				}
				num++;
				if (num > 20)
				{
					break;
				}
			}
			while (!StartingPawnUtility.WorkTypeRequirementsSatisfied());
			while (Find.GameInitData.startingAndOptionalPawns.Count < this.pawnChoiceCount)
			{
				foreach (var item in kinds)
				{
				//	Log.Message("Generating " + item);
					Find.GameInitData.startingAndOptionalPawns.Add(SpecificStartingPawnUtility.NewGeneratedStartingPawn(item));
				}
			}
			return;
		}
		*/


		public List<PawnSet> Pawns = new List<PawnSet>();
	}

	public class PawnSet
    {
		public PawnKindDef kindDef;
		public int count = 1;
    }
}
