using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AdeptusMechanicus
{
    internal class AutopsyWorker : RecipeWorker
    {
        public override void ConsumeIngredient(Thing ingredient, RecipeDef recipeDef, Map map)
        {
            if (recipeDef.defName.Contains("OG_Recipe_ReclaimBionics") && ingredient is Corpse)
                return;

            base.ConsumeIngredient(ingredient, recipeDef, map);
        }
    }
}
