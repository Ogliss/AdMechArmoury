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
        // Token: 0x06000AC8 RID: 2760 RVA: 0x000562D4 File Offset: 0x000546D4
        public CompProperties_Regeneration()
        {
            this.compClass = typeof(Comp_Regeneration);
        }
        public bool healFreshWounds = true;
        public float healFreshChance = 1f;
        public bool allowSealWounds = false;
        public float sealWoundsChance = 1f;
        public bool regrowMissingParts = false;
        public float regrowMissingChance = 1f;
        public bool healPermenantWounds = true;
        public float healPermenantChance = 1f;
        public bool allowScarRemoval = false;
        public float scarRemovalChance = 1f;
        public bool useFood = false;
        public bool useRest = false;
        public bool onlyWhileSleeping = false;
        public int ticksUntilNextHeal = 3500;
        public int ticksUntilNextGrow = 14000;
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

        public bool canHeal
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
                if (Props.healFreshWounds)
                {
                    this.TryHealWounds();
                }
                if (Props.allowSealWounds)
                {
                    this.TrySealWounds();
                }
                this.SetNextHealTick();
            }
            if (Current.Game.tickManager.TicksGame >= this.ticksUntilNextGrow)
            {
                if (Props.healPermenantWounds)
                {
                    this.TryHealOldWounds();
                }
                if (Props.regrowMissingParts)
                {
                    this.TryRegrowBodyparts();
                }
                this.SetNextGrowTick();
            }
        }

        public void TryHealWounds()
        {
            IEnumerable<Hediff> enumerable = from hd in this.pawn.health.hediffSet.hediffs
                                             where !hd.IsPermanent() && hd.def.isBad && hd.GetType() == typeof(Hediff_Injury) 
                                             select hd;
            bool flag = enumerable != null;
            if (flag && canHeal)
            {
                foreach (Hediff hediff in enumerable)
                {
                    Hediff_Injury Injury = hediff as Hediff_Injury;
                    bool flag2 = Injury != null;
                    Rand.PushState();
                    float chance = Rand.Value;
                    Rand.PopState();
                    if (flag2 && canHeal && chance < Props.healFreshChance)
                    {
                        float num = Injury.Severity * 0.001f;
                        Injury.Heal(num);
                    //    Log.Message(string.Format("num = {0}",num));
                        if (Props.useFood)
                        {
                            pawn.needs.food.CurLevel -= num;
                        }
                        if (Props.useRest)
                        {
                            pawn.needs.rest.CurLevel -= num;
                        }
                        //    Traverse.Create(hediff_Injury).Property(name: "BleedRate").SetValue(hediff_Injury.BleedRate*0.95);
                    }
                }
            }
        }

        public void TrySealWounds()
        {
            IEnumerable<Hediff> enumerable = from hd in this.pawn.health.hediffSet.hediffs
                                             where hd.Bleeding
                                             select hd;
            bool flag = enumerable != null;
            if (flag && canHeal)
            {
                foreach (Hediff hediff in enumerable)
                {
                    HediffWithComps hediffWithComps = hediff as HediffWithComps;
                    bool flag2 = hediffWithComps != null;
                    Rand.PushState();
                    float chance = Rand.Value;
                    Rand.PopState();
                    if (flag2 && canHeal && chance < Props.sealWoundsChance)
                    {
                        HediffComp_TendDuration hediffComp_TendDuration = hediffWithComps.TryGetCompFast<HediffComp_TendDuration>();
                        hediffComp_TendDuration.tendQuality = 0f;
                        hediffComp_TendDuration.tendTicksLeft = Find.TickManager.TicksGame;
                        this.pawn.health.Notify_HediffChanged(hediff);
                    }
                }
            }
        }

        public void TryHealOldWounds()
        {
            IEnumerable<Hediff> enumerable = from hd in this.pawn.health.hediffSet.hediffs
                                             where hd.IsPermanent() && hd.def.isBad && hd.GetType() == typeof(Hediff_Injury)
                                             select hd;
            bool flag = enumerable != null;
            if (flag && canHeal)
            {
                foreach (Hediff hediff in enumerable)
                {
                    Hediff_Injury Injury = hediff as Hediff_Injury;
                    bool flag2 = Injury != null;
                    Rand.PushState();
                    float chance = Rand.Value;
                    Rand.PopState();
                    if (flag2 && canHeal && chance < Props.healPermenantChance)
                    {
                        float num = Injury.Severity * 0.01f;
                        if (num > Injury.Severity && !Props.allowScarRemoval)
                        {
                            Rand.PushState();
                            chance = Rand.Value;
                            Rand.PopState();
                            if (flag2 && canHeal && chance < Props.scarRemovalChance)
                                num = Injury.Severity - 0.01f;
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
                        //    Traverse.Create(hediff_Injury).Property(name: "BleedRate").SetValue(hediff_Injury.BleedRate*0.95);
                    }
                }
            }
        }
        public void TryRegrowBodyparts()
        {
            using (IEnumerator<BodyPartRecord> enumerator = this.pawn.GetFirstMatchingBodyparts(this.pawn.RaceProps.body.corePart, HediffDefOf.MissingBodyPart, OGHediffDefOf.OG_Hediff_Regenerating_Part, (Hediff hediff) => hediff is Hediff_AddedPart).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    BodyPartRecord part = enumerator.Current;
                    Hediff hediff2 = this.pawn.health.hediffSet.hediffs.First((Hediff hediff) => hediff.Part == part && hediff.def == HediffDefOf.MissingBodyPart);
                    bool flag = hediff2 != null;
                    Rand.PushState();
                    float chance = Rand.Value;
                    Rand.PopState();
                    if (flag && canHeal && chance < Props.regrowMissingChance)
                    {
                        float num = hediff2.Part.def.GetMaxHealth(pawn) / 100;
                        this.pawn.health.RemoveHediff(hediff2);
                        this.pawn.health.AddHediff(OGHediffDefOf.OG_Hediff_Regenerating_Part, part, null, null);

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
            this.ticksUntilNextHeal = Current.Game.tickManager.TicksGame + Props.ticksUntilNextHeal;
        }

        public int ticksUntilNextGrow;
        public void SetNextGrowTick()
        {
            this.ticksUntilNextGrow = Current.Game.tickManager.TicksGame + Props.ticksUntilNextGrow;
        }

    }
}
