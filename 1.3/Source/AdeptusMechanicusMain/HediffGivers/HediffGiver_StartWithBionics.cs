using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AdeptusMechanicus
{
    /// <summary>
    /// Adds a hediff to a character after it is generated/spawned.
    /// Perfect for Alien Race bonuses/modifiers.
    /// Chance option to add for some randomization.
    /// </summary>
    /*
    public class HediffGiver_StartWithBionics : HediffGiver
    {
        public float chance = 100.0f;
        public float maleCommonality = 100.0f;
        public float femaleCommonality = 100.0f;
        public List<PawnKindDef> allowedPawnkinds = new List<PawnKindDef>();
        public List<PawnKindDef> disallowedPawnkinds = new List<PawnKindDef>();

        public void GiveHediff(Pawn pawn)
        {
            //If the random number is not within the chance range, exit.
            Rand.PushState();
            bool f = !(chance >= Rand.Range(0.0f, 100.0f));
            Rand.PopState();
            if (f) return;
            //If the gender is male, check the male commonality chance, and if it fails, exit.
            Rand.PushState();
            bool f2 = !(maleCommonality >= Rand.Range(0.0f, 100.0f));
            Rand.PopState();
            if (pawn.gender == Gender.Male && f2)
                return;
            Rand.PushState();
            bool f3 = !(femaleCommonality >= Rand.Range(0.0f, 100.0f));
            Rand.PopState();
            if (pawn.gender == Gender.Female && f3)
                return;
            float cash = pawn.kindDef.techHediffsMoney.RandomInRange;
            if (!allowedPawnkinds.NullOrEmpty() && !allowedPawnkinds.Contains(pawn.kindDef))
                return;
            if (!disallowedPawnkinds.NullOrEmpty() && disallowedPawnkinds.Contains(pawn.kindDef))
                return;

            float partsMoney = pawn.kindDef.techHediffsMoney.RandomInRange;
            IEnumerable<ThingDef> source = from x in DefDatabase<ThingDef>.AllDefs
                                           where x.isTechHediff && x.BaseMarketValue <= partsMoney && x.techHediffsTags != null && pawn.kindDef.techHediffsTags.Any((string tag) => x.techHediffsTags.Contains(tag))
                                           select x;
            if (source.Any<ThingDef>())
            {
                ThingDef partDef = source.RandomElementByWeight((ThingDef w) => w.BaseMarketValue);
                IEnumerable<RecipeDef> source2 = from x in DefDatabase<RecipeDef>.AllDefs
                                                 where x.IsIngredient(partDef) && pawn.def.AllRecipes.Contains(x)
                                                 select x;
                if (source2.Any<RecipeDef>())
                {
                    RecipeDef recipeDef = source2.RandomElement<RecipeDef>();
                    if (recipeDef.Worker.GetPartsToApplyOn(pawn, recipeDef).Any<BodyPartRecord>())
                    {
                        recipeDef.Worker.ApplyOnPawn(pawn, recipeDef.Worker.GetPartsToApplyOn(pawn, recipeDef).RandomElement<BodyPartRecord>(), null, emptyIngredientsList, null);
                    }
                }
            }
     //   HediffGiverUtility.TryApply(pawn, hediff, partsToAffect);
        }
        private static List<Thing> emptyIngredientsList = new List<Thing>();
    }
    */
}