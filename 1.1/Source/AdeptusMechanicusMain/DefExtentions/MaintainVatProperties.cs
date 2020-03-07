using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public class MaintainVatProperties : DefModExtension
    {
        public SkillDef maintainingSkill;
    }
    /// <summary>
    /// Properties for grower derived Buildings.
    /// </summary>
    public class GrowerProperties : DefModExtension
    {
        /// <summary>
        /// Recipes that the Grower have.
        /// </summary>
        public List<GrowerRecipeDef> recipes = new List<GrowerRecipeDef>();

        /// <summary>
        /// If true it requires a pawn to interact with the grower to extract the product.
        /// </summary>
        public bool productRequireManualExtraction = true;
    }

    /// <summary>
    /// Properties specific to the VatGrower.
    /// </summary>
    public class VatGrowerProperties : GrowerProperties
    {
        /// <summary>
        /// Graphic for the "lid".
        /// </summary>
        public GraphicData topGraphic;

        /// <summary>
        /// Graphic for the base.
        /// </summary>
        public GraphicData bottomGraphic;

        /// <summary>
        /// Graphic for the glow.
        /// </summary>
        public GraphicData glowGraphic;

        /// <summary>
        /// Graphic for the top detail.
        /// </summary>
        public GraphicData topDetailGraphic;

        /// <summary>
        /// Offset for where the product is rendered.
        /// </summary>
        public Vector3 productOffset = new Vector3();

        /// <summary>
        /// Scale for the product.
        /// </summary>
        public float productScaleModifier = 1f;

        public override IEnumerable<string> ConfigErrors()
        {
            ResolveAll();
            return base.ConfigErrors();
        }

        public void ResolveAll()
        {
            if (topGraphic != null)
            {
                topGraphic.ResolveReferencesSpecial();
            }

            if (bottomGraphic != null)
            {
                bottomGraphic.ResolveReferencesSpecial();
            }

            if (glowGraphic != null)
            {
                glowGraphic.ResolveReferencesSpecial();
            }
        }
    }
}
