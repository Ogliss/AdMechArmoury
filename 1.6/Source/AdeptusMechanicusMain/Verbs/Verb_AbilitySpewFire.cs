using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.Verb_AbilitySpewFire
    public class Verb_AbilitySpewFire : Verb_SpewFire, IAbilityVerb
    {
        public Ability Ability
        {
            get
            {
                return this.ability;
            }
            set
            {
                this.ability = value;
            }
        }

        public override bool TryCastShot()
        {
            bool flag = base.TryCastShot();
            if (flag)
            {
                this.ability.StartCooldown(this.ability.def.cooldownTicksRange.RandomInRange);
            }
            return flag;
        }

        public override void ExposeData()
        {
            Scribe_References.Look<Ability>(ref this.ability, "ability", false);
            base.ExposeData();
        }

        private Ability ability;
    }
}
