using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.Verb_CastAbilityJump
    public class Verb_CastAbilityJump : RimWorld.Verb_CastAbilityJump
    {
        public override float EffectiveRange
        {
            get
            {
                if (this.cachedEffectiveRange < 0f)
                {
                    if (base.EquipmentSource != null)
                    {
                        this.cachedEffectiveRange = base.EquipmentSource.GetStatValue(StatDefOf.JumpRange, true, -1);
                    }
                    else
                    {
                        this.cachedEffectiveRange = this.verbProps.range;
                    }
                    if (this.CasterIsPawn)
                    {
                        this.cachedEffectiveRange += base.CasterPawn.GetStatValue(StatDefOf.JumpRange, true, -1);
                    }
                }
                return this.cachedEffectiveRange;
            }
        }

        public override ThingDef JumpFlyerDef
        {
            get
            {
                if (this.verbProps.defaultProjectile != null)
                {
                    return verbProps.defaultProjectile;
                }
                return base.JumpFlyerDef;
            }
        }
        public new ThingWithComps EquipmentSource
        {
            get
            {
                if (this.EquipmentCompSource != null)
                {
                    return this.EquipmentCompSource.parent;
                }
                if (this.ReloadableCompSource != null)
                {
                    return this.ReloadableCompSource.parent;
                }
                if (this.VerbOwner_ChargedCompSource != null)
                {
                    return this.VerbOwner_ChargedCompSource.parent;
                }
                if (this.CasterIsPawn)
                {
                    return this.CasterPawn;
                }
                return null;
            }
        }
    }
}
