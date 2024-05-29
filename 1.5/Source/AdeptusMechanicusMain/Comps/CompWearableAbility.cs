using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.CompProperties_WearableAbility
    public class CompProperties_WearableAbility : CompProperties
    {
        public CompProperties_WearableAbility()
        {
            this.compClass = typeof(CompWearableAbility);
        }

        public AbilityDef abilityDef;
        public List<AbilityDef> abilityDefs;
    }
    public class CompWearableAbility : CompWearable
    {
        public CompProperties_WearableAbility Props
        {
            get
            {
                return this.props as CompProperties_WearableAbility;
            }
        }

        public Ability AbilityForReading
        {
            get
            {
                if (this.ability == null)
                {
                    this.ability = AbilityUtility.MakeAbility(this.Props.abilityDef, base.Wearer);
                }
                return this.ability;
            }
        }

        public List<Ability> AllAbilitiesForReading
        {
            get
            {
                if (this.abilities == null)
                {
                    this.abilities = new List<Ability>();
                    foreach (AbilityDef abilityDef in this.Props.abilityDefs)
                    {
                        this.abilities.Add(AbilityUtility.MakeAbility(abilityDef, this.Wearer));
                    }
                }
                return this.abilities;
            }
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            if (base.Wearer != null)
            {
                if (Props.abilityDef != null)
                {
                    this.AbilityForReading.pawn = base.Wearer;
                    this.AbilityForReading.verb.caster = base.Wearer;
                }
                if (!Props.abilityDefs.NullOrEmpty())
                {
                    foreach (var item in AllAbilitiesForReading)
                    {
                        item.pawn = base.Wearer;
                        item.verb.caster = base.Wearer;
                    }
                }
            }
        }

        public virtual void UsedOnce()
        {
        }

        public override void Notify_Equipped(Pawn pawn)
        {
            Log.Message($"{this.parent.LabelCap} equipped by {pawn.Name}");
            if (Props.abilityDef != null)
            {
                this.AbilityForReading.pawn = base.Wearer;
                this.AbilityForReading.verb.caster = base.Wearer;
            }
            if (!Props.abilityDefs.NullOrEmpty())
            {
                foreach (var item in AllAbilitiesForReading)
                {
                    item.pawn = base.Wearer;
                    item.verb.caster = base.Wearer;
                }
            }
            pawn.abilities.Notify_TemporaryAbilitiesChanged();
        }

        public override void Notify_Unequipped(Pawn pawn)
        {
            Log.Message($"{this.parent.LabelCap} unequipped by {pawn.Name}");
            pawn.abilities.Notify_TemporaryAbilitiesChanged();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Deep.Look<Ability>(ref this.ability, "ability", Array.Empty<object>());
            if (Scribe.mode == LoadSaveMode.PostLoadInit && base.Wearer != null)
            {
                this.AbilityForReading.pawn = base.Wearer;
                this.AbilityForReading.verb.caster = base.Wearer;
            }
        }

        private List<Ability> abilities;
        private Ability ability;
    }
}
