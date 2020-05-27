using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using Verse.Sound;
using System.Reflection;
using AdeptusMechanicus.ExtensionMethods;
using UnityEngine;
using AdeptusMechanicus.settings;

namespace AdeptusMechanicus.HarmonyInstance
{
    
    [HarmonyPatch(typeof(AgeInjuryUtility), "RandomHediffsToGainOnBirthday", new Type[] { typeof(Pawn), typeof(int) })]
	public static class AM_AgeInjuryUtility_RandomHediffsToGainOnBirthday_RejuveTreatment_Patch
    {
        [HarmonyPostfix]
        public static IEnumerable<HediffGiver_Birthday> RandomHediffsToGainOnBirthday_RejuveTreatment_Postfix(IEnumerable<HediffGiver_Birthday> __result, Pawn pawn, int age)
		{
			if (pawn != null)
			{
				if (pawn.RaceProps.Humanlike)
				{
					float increase = 0f;
					List<Hediff> rejuvTreatments = pawn.health.hediffSet.hediffs.FindAll(x => x.TryGetComp<HediffComp_RejuvTreatment>() != null);
					if (!rejuvTreatments.NullOrEmpty())
					{
						foreach (Hediff hd in rejuvTreatments)
						{
							HediffComp_RejuvTreatment rejuvTreatment = hd.TryGetComp<HediffComp_RejuvTreatment>();
							if (rejuvTreatment!=null)
							{
								increase += rejuvTreatment.LifeExpectancyIncrease;
							}
						}
						foreach (HediffGiver_Birthday item in RandomHediffsToGainOnBirthday(pawn.def, age, increase))
						{
							yield return item;
						}
						yield break;
					}
				}
			}
			foreach (HediffGiver_Birthday item in __result)
			{
				yield return item;
			}
			yield break;
		}

		// Token: 0x06004157 RID: 16727 RVA: 0x0015C87C File Offset: 0x0015AA7C
		private static IEnumerable<HediffGiver_Birthday> RandomHediffsToGainOnBirthday(ThingDef raceDef, int age, float increase)
		{
			List<HediffGiverSetDef> sets = raceDef.race.hediffGiverSets;
			if (sets == null)
			{
				yield break;
			}
			int num;
			for (int i = 0; i < sets.Count; i = num + 1)
			{
				List<HediffGiver> givers = sets[i].hediffGivers;
				for (int j = 0; j < givers.Count; j = num + 1)
				{
					HediffGiver_Birthday hediffGiver_Birthday = givers[j] as HediffGiver_Birthday;
					if (hediffGiver_Birthday != null)
					{
						float x = (float)age / (raceDef.race.lifeExpectancy + increase);
						if (Rand.Value < hediffGiver_Birthday.ageFractionChanceCurve.Evaluate(x))
						{
							yield return hediffGiver_Birthday;
						}
					}
					num = j;
				}
				givers = null;
				num = i;
			}
			yield break;
		}
	}
    
}