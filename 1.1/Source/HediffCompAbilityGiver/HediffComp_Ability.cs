using RimWorld;
using System.Collections.Generic;
using Verse;

namespace HediffCompAbilityGiver
{
    public class HediffCompProperties_Ability : HediffCompProperties
    {
        public HediffCompProperties_Ability()
        {
            this.compClass = typeof(HediffComp_Ability);
        }
        public List<AbilityDef> abilities = new List<AbilityDef>();
    }
    // Token: 0x02000706 RID: 1798
    [StaticConstructorOnStartup]
    public class HediffComp_Ability : HediffComp
    {
        public virtual HediffCompProperties_Ability Props
        {
            get
            {
                return this.props as HediffCompProperties_Ability;
            }
        }

        public override void CompPostMake()
        {
            base.CompPostMake();
            foreach (AbilityDef ab in Props.abilities)
            {
                if (!Pawn.abilities.abilities.Any(x=> x.def == ab))
                {
                    Pawn.abilities.GainAbility(ab);
                }
            }
        }

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            foreach (AbilityDef ab in Props.abilities)
            {
                if (!Pawn.abilities.abilities.Any(x => x.def == ab))
                {
                    Pawn.abilities.GainAbility(ab);
                }
            }
        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            foreach (Ability ab in Pawn.abilities.abilities)
            {
                if (Props.abilities.Any(x => x == ab.def))
                {
                    Pawn.abilities.abilities.Remove(ab);
                }
            }
        }
    }
}
