using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;
using AdeptusMechanicus.HarmonyInstance;

namespace AdeptusMechanicus.ExtensionMethods
{
    public static class ThingExtensions
    {
        public static bool powerWeapon(this Thing thing)
        {
            bool flag1 = thing.TryGetCompFast<CompEquippable>() != null;
            bool flag2 = thing.def.IsMeleeWeapon;
            bool flag3 = thing.TryGetCompFast<CompEquippable>().AllVerbs.Any(x => x.powerWeapon());
            return flag1 && flag2 && flag3;
        }

        public static bool witchbladeWeapon(this Thing thing)
        {
            bool flag1 = thing.TryGetCompFast<CompEquippable>() != null;
            bool flag2 = thing.def.IsMeleeWeapon;
            bool flag3 = thing.TryGetCompFast<CompEquippable>().AllVerbs.Any(x => x.witchbladeWeapon());
            return flag1 && flag2 && flag3;
        }

        public static bool forceWeapon(this Thing thing)
        {
            bool flag1 = thing.TryGetCompFast<CompEquippable>() != null;
            bool flag2 = thing.def.IsMeleeWeapon;
            bool flag3 = thing.TryGetCompFast<CompEquippable>().AllVerbs.Any(x => x.forceWeapon());
            return flag1 && flag2 && flag3;
        }

        public static bool rendingWeapon(this Thing thing)
        {
            bool flag1 = thing.TryGetCompFast<CompEquippable>() != null;
            bool flag2 = thing.def.IsMeleeWeapon;
            bool flag3 = thing.TryGetCompFast<CompEquippable>().AllVerbs.Any(x => x.rendingWeapon());
            return flag1 && flag2 && flag3;
        }

		public static float Ingested(this Thing thing, Pawn ingester, float nutritionWanted, BodyPartRecord targetPart)
		{
			if (thing.Destroyed)
			{
				Log.Error(ingester + " ingested destroyed thing " + thing, false);
				return 0f;
			}
			if (!thing.IngestibleNow)
			{
				Log.Error(ingester + " ingested IngestibleNow=false thing " + thing, false);
				return 0f;
			}
			Corpse corpse = thing as Corpse;
			if (corpse == null)
			{
				Log.Error(ingester + " ingested NonCorpse thing " + thing, false);
				return 0f;
			}
			ingester.mindState.lastIngestTick = Find.TickManager.TicksGame;
			if (ingester.needs.mood != null)
			{
				List<ThoughtDef> list = FoodUtility.ThoughtsFromIngesting(ingester, thing, thing.def);
				for (int i = 0; i < list.Count; i++)
				{
					ingester.needs.mood.thoughts.memories.TryGainMemory(list[i], null);
				}
			}
			if (ingester.needs.drugsDesire != null)
			{
				ingester.needs.drugsDesire.Notify_IngestedDrug(thing);
			}
			if (ingester.IsColonist && FoodUtility.IsHumanlikeMeatOrHumanlikeCorpse(thing))
			{
				TaleRecorder.RecordTale(TaleDefOf.AteRawHumanlikeMeat, new object[]
				{
					ingester
				});
			}
			int num;
			float result;
			corpse.IngestedCalculateAmounts(ingester, targetPart, out num, out result);
			/*
			MethodInfo dynMethod = thing.GetType().GetMethod("IngestedCalculateAmounts",
			BindingFlags.NonPublic | BindingFlags.Instance);
			object[] parameters = new object[] { ingester, nutritionWanted, null, null };
			dynMethod.Invoke(thing, parameters);
			Log.Message(thing+" eaten by "+ingester+" nutritionWanted: "+ nutritionWanted + " num: " + parameters[2] + " result: " + parameters[3]);
			num = (int)parameters[2];
			result = (float)parameters[3];
			//	thing.IngestedCalculateAmounts(ingester, nutritionWanted, out num, out result);
			*/
			if (!ingester.Dead && ingester.needs.joy != null && Mathf.Abs(thing.def.ingestible.joy) > 0.0001f && num > 0)
			{
				JoyKindDef joyKind = (thing.def.ingestible.joyKind != null) ? thing.def.ingestible.joyKind : JoyKindDefOf.Gluttonous;
				ingester.needs.joy.GainJoy((float)num * thing.def.ingestible.joy, joyKind);
			}
			if (ingester.RaceProps.Humanlike && Rand.Chance(thing.GetStatValue(StatDefOf.FoodPoisonChanceFixedHuman, true) * FoodUtility.GetFoodPoisonChanceFactor(ingester)))
			{
				FoodUtility.AddFoodPoisoningHediff(ingester, thing, FoodPoisonCause.DangerousFoodType);
			}
			bool flag = false;
			if (num > 0)
			{
				if (thing.stackCount == 0)
				{
					Log.Error(thing + " stack count is 0.", false);
				}
				if (num == thing.stackCount)
				{
					flag = true;
				}
				else
				{
					thing.SplitOff(num);
				}
			}
			MethodInfo dynMethod2 = thing.GetType().GetMethod("PrePostIngested",
			BindingFlags.NonPublic | BindingFlags.Instance);
			dynMethod2.Invoke(thing, new object[] { ingester });
		//	thing.PrePostIngested(ingester);
			if (flag)
			{
				ingester.carryTracker.innerContainer.Remove(thing);
			}
			if (thing.def.ingestible.outcomeDoers != null)
			{
				for (int j = 0; j < thing.def.ingestible.outcomeDoers.Count; j++)
				{
					thing.def.ingestible.outcomeDoers[j].DoIngestionOutcome(ingester, thing);
				}
			}
			if (flag)
			{
				thing.Destroy(DestroyMode.Vanish);
			}
			MethodInfo dynMethod3 = thing.GetType().GetMethod("PostIngested",
			BindingFlags.NonPublic | BindingFlags.Instance);
			dynMethod3.Invoke(thing, new object[] { ingester });
		//	thing.PostIngested(ingester);
			return result;
		}

		
		public static void IngestedCalculateAmounts(this Corpse corpse, Pawn ingester, BodyPartRecord bodyPartRecord, out int numTaken, out float nutritionIngested)
		{
			if (bodyPartRecord == null)
			{
				Log.Error(string.Concat(new object[]
				{
					ingester,
					" ate ",
					corpse,
					" but no body part was found. Replacing with core part."
				}), false);
				bodyPartRecord = corpse.InnerPawn.RaceProps.body.corePart;
			}
			float bodyPartNutrition = FoodUtility.GetBodyPartNutrition(corpse, bodyPartRecord);
			if (bodyPartRecord == corpse.InnerPawn.RaceProps.body.corePart)
			{
				if (PawnUtility.ShouldSendNotificationAbout(corpse.InnerPawn) && corpse.InnerPawn.RaceProps.Humanlike)
				{
					Messages.Message("MessageEatenByPredator".Translate(corpse.InnerPawn.LabelShort, ingester.Named("PREDATOR"), corpse.InnerPawn.Named("EATEN")).CapitalizeFirst(), ingester, MessageTypeDefOf.NegativeEvent, true);
				}
				numTaken = 1;
			}
			else
			{
				Hediff_MissingPart hediff_MissingPart = (Hediff_MissingPart)HediffMaker.MakeHediff(HediffDefOf.MissingBodyPart, corpse.InnerPawn, bodyPartRecord);
				hediff_MissingPart.lastInjury = HediffDefOf.Bite;
				hediff_MissingPart.IsFresh = true;
				corpse.InnerPawn.health.AddHediff(hediff_MissingPart, null, null, null);
				numTaken = 0;
			}
			nutritionIngested = bodyPartNutrition;
		}
		
	}
}
