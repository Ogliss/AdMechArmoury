using RimWorld;
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
		}

		// Token: 0x06004D9E RID: 19870 RVA: 0x001A4D6E File Offset: 0x001A2F6E
		public override string Summary(Scenario scen)
		{
			return "ScenPart_StartWithPawns".Translate(this.pawnCount, this.pawnChoiceCount);
		}


		// Token: 0x06004DA0 RID: 19872 RVA: 0x001A4DAC File Offset: 0x001A2FAC
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
					Log.Message("Generating "+item);
					Find.GameInitData.startingAndOptionalPawns.Add(SpecificStartingPawnUtility.NewGeneratedStartingPawn(item));
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
				foreach (var item in kinds)
				{
					Log.Message("Generating " + item);
					Find.GameInitData.startingAndOptionalPawns.Add(SpecificStartingPawnUtility.NewGeneratedStartingPawn(item));
				}
			}
			return;
			goto IL_62;
		}



		public List<PawnSet> Pawns = new List<PawnSet>();
	}
	public class PawnSet
    {
		public PawnKindDef kindDef;
		public int count = 1;
    }
}
