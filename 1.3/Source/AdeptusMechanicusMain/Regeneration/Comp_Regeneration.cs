using AdeptusMechanicus.ExtensionMethods;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public class CompProperties_Regeneration : CompProperties
    {
        public CompProperties_Regeneration()
        {
            this.compClass = typeof(Comp_Regeneration);
        }
        public float healFreshChance = 1f;
        public float sealWoundsChance = -1f;
        public float regrowMissingChance = -1f;
        public float healPermenantChance = 1f;
        public float scarRemovalChance = -1f;
        public float scarChance = 1f;
        public float healFactor = 0.005f;
        public float healMinimum = 0.001f;
        public float healFactorOldWound = 0.01f;
        public bool useFood = false;
        public bool useRest = false;
        public bool onlyWhileSleeping = false;
        public int healTickInterval = 3500;
        public int regrowTickInterval = 14000;
        public HediffDef Regenerated_Part;
        public HediffDef Regenerating_Part;
    }

    public class Comp_Regeneration : ThingComp
    {
        public CompProperties_Regeneration Props => this.props as CompProperties_Regeneration;
        public Pawn pawn => this.parent as Pawn;
        public override void PostPostMake()
        {
            base.PostPostMake();
            this.SetNextHealTick();
        }

        public bool CanHeal
        {
            get
            {
                if (parent.Map== null)
                {
                    return false;
                }
                bool a = (Props.useFood && pawn.needs.food.CurCategory > HungerCategory.UrgentlyHungry);
                bool b = (Props.useRest && pawn.needs.rest.CurCategory > RestCategory.VeryTired);
                bool c = (Props.onlyWhileSleeping && pawn.Awake());
                return !(a||b||c);
            }
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<int>(ref this.ticksUntilNextHeal, "ticksUntilNextHeal", 0, false);
            Scribe_Values.Look<int>(ref this.ticksUntilNextGrow, "ticksUntilNextGrow", 0, false);
        }

        public override void CompTick()
        {
            base.CompTick();
            if (Current.Game.tickManager.TicksGame >= this.ticksUntilNextHeal)
            {
                if (Props.healFreshChance > 0f)
                {
                    this.TryHealWounds();
                }
                if (Props.sealWoundsChance > 0f)
                {
                    this.TrySealWounds();
                }
                this.SetNextHealTick();
            }
            if (Current.Game.tickManager.TicksGame >= this.ticksUntilNextGrow)
            {
                if (Props.healPermenantChance > 0f)
                {
                    this.TryHealOldWounds();
                }
                if (Props.regrowMissingChance > 0f)
                {
                    this.TryRegrowBodyparts();
                }
                this.SetNextGrowTick();
            }
        }

        public void TryHealWounds()
        {
            IEnumerable<Hediff> enumerable = from hd in this.pawn.health.hediffSet.hediffs
                                             where !hd.IsPermanent() && hd.def.isBad && hd is Hediff_Injury
                                             select hd;
            if (enumerable != null && CanHeal)
            {
                foreach (Hediff hediff in enumerable)
                {
                    if (hediff is Hediff_Injury Injury)
                    {
                        Rand.PushState();
                        float chance = Rand.Value;
                        Rand.PopState();
                        if (CanHeal && chance < Props.healFreshChance)
                        {
                            float num = Injury.Severity * Props.healFactor;
                            Injury.Heal(Mathf.Max(num, Props.healMinimum));
                            if (Props.useFood)
                            {
                                pawn.needs.food.CurLevel -= num;
                            }
                            if (Props.useRest)
                            {
                                pawn.needs.rest.CurLevel -= num;
                            }
                        }
                    }
                }
            }
        }

        public void TrySealWounds()
        {
            IEnumerable<Hediff> enumerable = from hd in this.pawn.health.hediffSet.hediffs
                                             where hd.Bleeding
                                             select hd;
            if (enumerable != null && CanHeal)
            {
                foreach (Hediff hediff in enumerable)
                {
                    if (hediff is HediffWithComps hediffWithComps)
                    {
                        Rand.PushState();
                        float chance = Rand.Value;
                        Rand.PopState();
                        if (CanHeal && chance < Props.sealWoundsChance)
                        {
                            HediffComp_TendDuration hediffComp_TendDuration = hediffWithComps.TryGetCompFast<HediffComp_TendDuration>();
                            hediffComp_TendDuration.tendQuality = 0f;
                            hediffComp_TendDuration.tendTicksLeft = Find.TickManager.TicksGame;
                            this.pawn.health.Notify_HediffChanged(hediff);
                        }

                    }
                }
            }
        }

        public void TryHealOldWounds()
        {
            IEnumerable<Hediff> enumerable = from hd in this.pawn.health.hediffSet.hediffs
                                             where hd.IsPermanent() && hd.def.isBad && hd.GetType() == typeof(Hediff_Injury)
                                             select hd;
            if (enumerable != null && CanHeal)
            {
                foreach (Hediff hediff in enumerable)
                {
                    if (hediff is Hediff_Injury Injury)
                    {
                        Rand.PushState();
                        float chance = Rand.Value;
                        float chancescar = Rand.Value;
                        Rand.PopState();
                        if (Injury != null && CanHeal && chance < Props.healPermenantChance)
                        {
                            float num = Injury.Severity * Props.healFactorOldWound;
                            if (num > Injury.Severity && Props.scarRemovalChance > 0)
                            {
                                if (chancescar < Props.scarRemovalChance)
                                {
                                    num = Injury.Severity - Props.healFactorOldWound;
                                }
                            }
                            if (Props.useFood)
                            {
                                pawn.needs.food.CurLevel -= num;
                            }
                            if (Props.useRest)
                            {
                                pawn.needs.rest.CurLevel -= num;
                            }
                            Injury.Heal(num);
                        }
                    }
                }
            }
        }
        public void TryRegrowBodyparts()
        {
            using (IEnumerator<BodyPartRecord> enumerator = this.pawn.GetFirstMatchingBodyparts(this.pawn.RaceProps.body.corePart, HediffDefOf.MissingBodyPart, AdeptusHediffDefOf.OG_Hediff_Regenerating_Part, (Hediff hediff) => hediff is Hediff_AddedPart).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    BodyPartRecord part = enumerator.Current;
                    Hediff hediff2 = this.pawn.health.hediffSet.hediffs.First((Hediff hediff) => hediff.Part == part && hediff.def == HediffDefOf.MissingBodyPart);
                    Rand.PushState();
                    float chance = Rand.Value;
                    Rand.PopState();
                    if (hediff2 != null && CanHeal && chance < Props.regrowMissingChance)
                    {
                        float num = hediff2.Part.def.GetMaxHealth(pawn) / 100;
                        this.pawn.health.RemoveHediff(hediff2);
                        this.pawn.health.AddHediff(AdeptusHediffDefOf.OG_Hediff_Regenerating_Part, part, null, null);

                        if (Props.useFood)
                        {
                            pawn.needs.food.CurLevel -= num;
                        }
                        if (Props.useRest)
                        {
                            pawn.needs.rest.CurLevel -= num;
                        }
                        this.pawn.health.hediffSet.DirtyCache();
                    }
                }
            }
        }

        public int ticksUntilNextHeal;
        public void SetNextHealTick()
        {
            this.ticksUntilNextHeal = Current.Game.tickManager.TicksGame + Props.healTickInterval;
        }

        public int ticksUntilNextGrow;
        public void SetNextGrowTick()
        {
            this.ticksUntilNextGrow = Current.Game.tickManager.TicksGame + Props.regrowTickInterval;
        }

    }
}
