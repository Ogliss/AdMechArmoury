using System;
using Verse;
using Verse.AI;
using RimWorld;
using System.Collections.Generic;

namespace FerroFood
{
    public class JobGiver_GetFood_Ferro : ThinkNode_JobGiver
    {

        private HungerCategory minCategory;

        private float maxLevelPercentage = 1f;

        public bool forceScanWholeMap;

        private Effecter effecter;

        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_GetFood_Ferro jobGiver_GetFood = (JobGiver_GetFood_Ferro)base.DeepCopy(resolve);
            jobGiver_GetFood.minCategory = this.minCategory;
            jobGiver_GetFood.maxLevelPercentage = this.maxLevelPercentage;
            jobGiver_GetFood.forceScanWholeMap = this.forceScanWholeMap;
            return jobGiver_GetFood;
        }

        public override float GetPriority(Pawn pawn)
        {
            Need_Food food = pawn.needs.food;
            if (food == null)
            {
                return 0f;
            }
            if (pawn.needs.food.CurCategory < HungerCategory.Starving && FoodUtility.ShouldBeFedBySomeone(pawn))
            {
                return 0f;
            }
            if (food.CurCategory < this.minCategory)
            {
                return 0f;
            }
            if (food.CurLevelPercentage > this.maxLevelPercentage)
            {
                return 0f;
            }
            if (food.CurLevelPercentage < pawn.RaceProps.FoodLevelPercentageWantEat)
            {
                return 9.5f;
            }
            return 0f;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            Need_Food food = pawn.needs.food;
            if (food == null || food.CurCategory < this.minCategory || food.CurLevelPercentage > this.maxLevelPercentage)
            {
                return null;
            }
            Thing thing = null;
            if (!FerroFoods.Foods.NullOrEmpty())
            {
                foreach (var item in FerroFoods.Foods)
                {
                    thing = FindFerroFood(item, pawn);

                    if (thing != null)
                    {

                        break;
                    }
                }
            }
            else return null;
            if (thing != null && pawn.Map.reservationManager.CanReserve(pawn, thing, 1))
            {
                // you need to set the jobdef here
                Job job3 = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("JOBDEF", true), thing);
                job3.count = 1;// FoodUtility.WillIngestStackCountOf(pawn, thingDef, nutrition);
                return job3;
            }
            else
            {
                if (pawn.Map != null && pawn.needs.food.CurLevelPercentage < pawn.needs.food.PercentageThreshHungry && pawn.Awake())
                {
                    ThingDef newThing = FerroFoods.Foods.RandomElement();
                    Thing newcorpse = null;
                    int count = Rand.Range(1, 10);
                    for (int i = 0; i < count; i++)
                    {
                        newcorpse = GenSpawn.Spawn(newThing, pawn.Position, pawn.Map, WipeMode.Vanish);
                    }

                    if (this.effecter == null)
                    {
                        this.effecter = EffecterDefOf.Mine.Spawn();
                    }
                    this.effecter.Trigger(pawn, newcorpse);
                }
            }
            return null;

        }

        public Thing FindFerroFood(ThingDef thingDef, Pawn pawn)
        {
            ThingRequest thingReq = ThingRequest.ForDef(thingDef);
            bool ignoreEntirelyForbiddenRegions = ForbidUtility.CaresAboutForbidden(pawn, true) && pawn.playerSettings != null && pawn.playerSettings.EffectiveAreaRestrictionInPawnCurrentMap != null;
            Thing thingToEat = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, thingReq, PathEndMode.ClosestTouch, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, null, null, 0, -1, false, RegionType.Set_Passable, ignoreEntirelyForbiddenRegions);
            if (thingToEat != null && thingToEat.Position.InAllowedArea(pawn))
            {
                return thingToEat;
            }
            else
            {
                return null;
            }
        }
    }

    [StaticConstructorOnStartup]
    public static class FerroFoods
    {
        public static List<ThingDef> foods;
        public static List<ThingDef> Foods
        {
            get
            {
                if (foods == null)
                {
                    foods = DefDatabase<ThingDef>.AllDefsListForReading.FindAll(x=> x.stuffProps?.categories != null && x.stuffProps.categories.Contains(StuffCategoryDefOf.Metallic));
                }
                return foods;
            }
        }
    }
}