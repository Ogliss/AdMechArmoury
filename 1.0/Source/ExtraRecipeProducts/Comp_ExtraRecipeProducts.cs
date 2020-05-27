using RimWorld;
using Verse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtraRecipeProducts
{

    public class CompProperties_ExtraRecipeProducts : CompProperties
    {
        public CompProperties_ExtraRecipeProducts()
        {
            this.compClass = typeof(Comp_ExtraRecipeProducts);
        }

        public List<ExtraProducts> ExtraProducts = new List<ExtraRecipeProducts.ExtraProducts>();
    }

    public class ExtraProducts
    {
        public ThingDef productdef = null;
        public int Min = 1;
        public int Max = 1;
    }

    public class Comp_ExtraRecipeProducts : ThingComp
    {
        public CompProperties_ExtraRecipeProducts Props
        {
            get
            {
                return (CompProperties_ExtraRecipeProducts)this.props;
            }
        }

        public Plant plant
        {
            get
            {
                return base.parent as Plant;
            }
        }
        
        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            foreach (ExtraProducts set in Props.ExtraProducts)
            {
                Thing thing = ThingMaker.MakeThing(set.productdef, null);
                thing.stackCount = Rand.RangeInclusive(set.Min, set.Max);
                GenPlace.TryPlaceThing(thing, base.parent.Position, map, ThingPlaceMode.Near, null, null);
            }
        }

        public PawnKindDef pawnKindDef;

        public Faction faction;

        public PawnGenerationContext generationContext;
    }
 
}
