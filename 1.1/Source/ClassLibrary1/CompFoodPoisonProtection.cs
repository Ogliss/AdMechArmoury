using Verse;
using RimWorld;
using System.Collections.Generic;

namespace Momu
{
    public class CompProperties_FoodPoisonProtection : CompProperties
    {
        public CompProperties_FoodPoisonProtection()
        {
            this.compClass = typeof(CompFoodPoisonProtection);
        }
        public bool Poisonable = true;
        public List<FoodTypeFlags> FoodTypeFlags = new List<FoodTypeFlags>();
        public List<FoodPoisonCause> FoodPoisonCause = new List<FoodPoisonCause>();
    }

    public class CompFoodPoisonProtection : ThingComp
    {
        public CompProperties_FoodPoisonProtection Props => (CompProperties_FoodPoisonProtection)props;
    }
}
