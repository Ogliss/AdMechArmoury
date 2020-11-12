using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
	// Token: AdeptusMechanicus.ScenPart_ConfigPage_ConfigureStartingPawnsSpecific
	public class ScenPart_ConfigPage_ConfigureStartingPawnsSpecific : ScenPart_ConfigPage_ConfigureStartingPawns
	{

		// Token: 0x06004D9D RID: 19869 RVA: 0x001A4D42 File Offset: 0x001A2F42
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.pawnCount, "pawnCount", 0, false);
			Scribe_Values.Look<int>(ref this.pawnChoiceCount, "pawnChoiceCount", 0, false);
		}

		// Token: 0x06004D9E RID: 19870 RVA: 0x001A4D6E File Offset: 0x001A2F6E
		public override string Summary(Scenario scen)
		{
			return "ScenPart_StartWithPawns".Translate(this.pawnCount, this.pawnChoiceCount);
		}

		// Token: 0x06004D9F RID: 19871 RVA: 0x001A4D95 File Offset: 0x001A2F95
		public override void Randomize()
		{
			this.pawnCount = Rand.RangeInclusive(1, 6);
			this.pawnChoiceCount = 10;
		}

		// Token: 0x06004DA0 RID: 19872 RVA: 0x001A4DAC File Offset: 0x001A2FAC
		public override void PostWorldGenerate()
		{
            if (this.sets.NullOrEmpty())
            {
				base.PostWorldGenerate();
				return;
            }
            foreach (var item in this.sets)
            {
				this.pawnCount += item.count;
			}
			Find.GameInitData.startingPawnCount = this.pawnCount;
			int num = 0;
			do
			{
				StartingPawnUtility.ClearAllStartingPawns();
                foreach (PawnSet item in this.sets)
				{
					Log.Message("Generating "+item.kindDef);
					for (int i = 0; i < item.count; i++)
					{
						Find.GameInitData.startingAndOptionalPawns.Add(ScenPart_ConfigPage_ConfigureStartingPawnsSpecific.NewGeneratedStartingPawn(item.kindDef));
					}
				}
				num++;
				if (num > 20)
				{
					break;
				}
			}
			while (!StartingPawnUtility.WorkTypeRequirementsSatisfied());
			IL_62:
			while (Find.GameInitData.startingAndOptionalPawns.Count < this.pawnChoiceCount)
			{
				foreach (PawnSet item in this.sets)
				{
					for (int i = 0; i < item.count; i++)
					{
						Find.GameInitData.startingAndOptionalPawns.Add(ScenPart_ConfigPage_ConfigureStartingPawnsSpecific.NewGeneratedStartingPawn(item.kindDef));
					}
				}
			}
			return;
			goto IL_62;
		}

		public static Pawn NewGeneratedStartingPawn(PawnKindDef kindDef)
        {
            if (kindDef == null)
            {
				Log.Error("KindDef NULL");
            }
			PawnGenerationRequest request = new PawnGenerationRequest(kindDef, Faction.OfPlayer, PawnGenerationContext.PlayerStarter, -1, true, false, false, false, true, TutorSystem.TutorialMode, 20f, false, true, true, true, false, false, false, false, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null);
			Pawn pawn = null;
			try
			{
				pawn = PawnGenerator.GeneratePawn(request);
			}
			catch (Exception arg)
			{
				Log.Error("There was an exception thrown by the PawnGenerator during generating a starting pawn. Trying one more time...\nException: " + arg, false);
				pawn = PawnGenerator.GeneratePawn(request);
			}
			pawn.relations.everSeenByPlayer = true;
			PawnComponentsUtility.AddComponentsForSpawn(pawn);
			return pawn;
		}

		public List<PawnSet> sets = new List<PawnSet>();

		// Token: 0x04002C54 RID: 11348
		private string pawnCountBuffer;

		// Token: 0x04002C55 RID: 11349
		private string pawnCountChoiceBuffer;

		// Token: 0x04002C56 RID: 11350
		private const int MaxPawnCount = 10;

		// Token: 0x04002C57 RID: 11351
		private const int MaxPawnChoiceCount = 10;
	}
	public class PawnSet
    {
		public PawnKindDef kindDef;
		public int count;
    }
}
