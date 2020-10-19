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
    public class HediffGiver_StartWithHediff : HediffGiver
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
            if (!allowedPawnkinds.NullOrEmpty() && !allowedPawnkinds.Contains(pawn.kindDef))
                return;
            if (!disallowedPawnkinds.NullOrEmpty() && disallowedPawnkinds.Contains(pawn.kindDef))
                return;
            if (pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, null, null).Any((BodyPartRecord bpr) => partsToAffect.Contains(bpr.def)))
            {
                for (int i = 0; i < this.countToAffect; i++)
                {
                    HediffGiverUtility.TryApply(pawn, hediff, partsToAffect);
                }
            }
        }
    }
}