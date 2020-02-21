using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x02000006 RID: 6
    public class Thing_AddsHediff : Gas
    {
        // Token: 0x17000004 RID: 4
        // (get) Token: 0x06000021 RID: 33 RVA: 0x00002D9F File Offset: 0x00000F9F
        // (set) Token: 0x06000022 RID: 34 RVA: 0x00002DA7 File Offset: 0x00000FA7
        public object cachedLabelMouseover { get; private set; }

        // Token: 0x06000023 RID: 35 RVA: 0x00002DB0 File Offset: 0x00000FB0
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.TickRate = this.Ticks;
            this.Ticks = this.TickRate;
        }

        // Token: 0x06000024 RID: 36 RVA: 0x00002DD4 File Offset: 0x00000FD4
        public override void Tick()
        {
            bool destroyed = base.Destroyed;
            if (!destroyed)
            {
                base.Tick();
                bool flag = this.destroyTick <= Find.TickManager.TicksGame && !base.Destroyed;
                if (flag)
                {
                    this.Destroy(0);
                }
                else
                {
                    this.graphicRotation += this.graphicRotationSpeed;
                    this.Ticks--;
                    bool flag2 = this.Ticks <= 0;
                    if (flag2)
                    {
                        this.TickTack();
                        this.Ticks = this.TickRate;
                    }
                }
            }
        }

        // Token: 0x06000025 RID: 37 RVA: 0x00002E68 File Offset: 0x00001068
        public void TickTack()
        {
            bool destroyed = base.Destroyed;
            if (!destroyed)
            {
                Thing_AddsHediffDef thing_AddsHediffDef = this.def as Thing_AddsHediffDef;
                bool flag = thing_AddsHediffDef.addHediff != null;
                if (flag)
                {
                    List<Thing> thingList = GridsUtility.GetThingList(base.Position, base.Map);
                    for (int i = 0; i < thingList.Count; i++)
                    {
                        Pawn pawn = thingList[i] as Pawn;
                        bool flag2 = pawn != null && !this.touchingPawns.Contains(pawn);
                        if (flag2)
                        {
                            this.touchingPawns.Add(pawn);
                            this.addHediffToPawn(pawn, thing_AddsHediffDef.addHediff, thing_AddsHediffDef.hediffAddChance, thing_AddsHediffDef.hediffSeverity, thing_AddsHediffDef.onlyAffectLungs);
                        }
                    }
                    for (int j = 0; j < this.touchingPawns.Count; j++)
                    {
                        Pawn pawn2 = this.touchingPawns[j];
                        bool flag3 = !pawn2.Spawned || pawn2.Position != base.Position;
                        if (flag3)
                        {
                            this.touchingPawns.Remove(pawn2);
                        }
                    }
                }
                this.cachedLabelMouseover = null;
            }
        }

        // Token: 0x06000026 RID: 38 RVA: 0x00002F9C File Offset: 0x0000119C
        public void damageEntities(Thing e, int amt)
        {
            DamageInfo damageInfo = new DamageInfo(DamageDefOf.Burn, (float)amt, 0f, -1f, null, null, null, 0, null);
            bool flag = e != null;
            if (flag)
            {
                e.TakeDamage(damageInfo);
            }
        }

        // Token: 0x06000027 RID: 39 RVA: 0x00002FDC File Offset: 0x000011DC
        public void damageBuildings(int amt)
        {
            IntVec3 intVec = GenAdj.RandomAdjacentCell8Way(this);
            bool flag = GenGrid.InBounds(intVec, base.Map);
            bool flag2 = flag;
            if (flag2)
            {
                Building firstBuilding = GridsUtility.GetFirstBuilding(intVec, base.Map);
                DamageInfo damageInfo = new DamageInfo(DamageDefOf.Burn, (float)amt, 0f, -1f, null, null, null, 0, null);
                bool flag3 = firstBuilding != null;
                bool flag4 = flag3;
                if (flag4)
                {
                    firstBuilding.TakeDamage(damageInfo);
                }
            }
        }

        // Token: 0x06000028 RID: 40 RVA: 0x00003048 File Offset: 0x00001248
        public void addHediffToPawn(Pawn p, HediffDef _heddiff, float _addhediffChance, float _hediffseverity, bool onlylungs)
        {
            bool EyeProtection = false;
            bool LungProtection = false;
            bool flag = !Rand.Chance(_addhediffChance);
            if (!flag)
            {
                Hediff hediff = HediffMaker.MakeHediff(_heddiff, p, null);
                hediff.Severity = _hediffseverity;
                CompLungProtection clp;
                clp = p.GetComp<CompLungProtection>();
                if (clp != null)
                {
                    LungProtection = true;
                }
                CompEyeProtection cep;
                foreach (var a in p.apparel.WornApparel)
                {
                    if (a.def.apparel.tags.Contains("GasMask"))
                    {

                    }
                }

                cep = p.GetComp<CompEyeProtection>();
                if (cep != null)
                {
                    EyeProtection = true;
                }
                bool flag2 = onlylungs && p.health.capacities.CapableOf(PawnCapacityDefOf.Breathing);
                if (flag2)
                {
                    if (!LungProtection)
                    {
                        List<BodyPartRecord> list = new List<BodyPartRecord>();
                        float num = 0.028758334f;
                        num *= StatExtension.GetStatValue(p, StatDefOf.ToxicSensitivity, true);
                        bool flag3 = num != 0f;
                        if (flag3)
                        {
                            float num2 = Mathf.Lerp(0.85f, 1.15f, Rand.ValueSeeded(p.thingIDNumber ^ 74374237));
                            num *= num2;
                        }
                        float statValue = StatExtension.GetStatValue(p, StatDefOf.ToxicSensitivity, true);
                        hediff.Severity = _hediffseverity * statValue;
                        foreach (BodyPartRecord bodyPartRecord in p.health.hediffSet.GetNotMissingParts(0, BodyPartDepth.Inside, null, null))
                        {
                            bool flag4 = bodyPartRecord.def.tags.Contains(BodyPartTagDefOf.BreathingSource);
                            if (flag4)
                            {
                                list.Add(bodyPartRecord);
                            }
                        }
                        bool flag5 = list.Count > 0;
                        if (flag5)
                        {
                            for (int i = 0; i < list.Count; i++)
                            {
                                Hediff hediff2;
                                if (p == null)
                                {
                                    hediff2 = null;
                                }
                                else
                                {
                                    Pawn_HealthTracker health = p.health;
                                    if (health == null)
                                    {
                                        hediff2 = null;
                                    }
                                    else
                                    {
                                        HediffSet hediffSet = health.hediffSet;
                                        hediff2 = ((hediffSet != null) ? hediffSet.GetFirstHediffOfDef(_heddiff, false) : null);
                                    }
                                }
                                Hediff hediff3 = hediff2;
                                float num3 = Rand.Range(0.1f, 0.2f);
                                bool flag6 = hediff3 != null;
                                if (flag6)
                                {
                                    hediff3.Severity += num3 * statValue;
                                }
                                else
                                {
                                    p.health.AddHediff(hediff, list[i], null, null);
                                }
                            }
                        }
                    }
                }
                else
                {
                    Hediff hediff4;
                    if (p == null)
                    {
                        hediff4 = null;
                    }
                    else
                    {
                        Pawn_HealthTracker health2 = p.health;
                        if (health2 == null)
                        {
                            hediff4 = null;
                        }
                        else
                        {
                            HediffSet hediffSet2 = health2.hediffSet;
                            hediff4 = ((hediffSet2 != null) ? hediffSet2.GetFirstHediffOfDef(_heddiff, false) : null);
                        }
                    }
                    Hediff hediff5 = hediff4;
                    float num4 = Rand.Range(0.1f, 0.2f);
                    float statValue2 = StatExtension.GetStatValue(p, StatDefOf.ToxicSensitivity, true);
                    bool flag7 = hediff5 != null;
                    if (flag7)
                    {
                        hediff5.Severity += num4 * statValue2;
                    }
                    else
                    {
                        hediff.Severity = _hediffseverity * statValue2;
                        p.health.AddHediff(hediff, null, null, null);
                    }
                }
            }
        }

        // Token: 0x04000015 RID: 21
        private List<Pawn> touchingPawns = new List<Pawn>();

        // Token: 0x04000016 RID: 22
        private List<Thing> touchingThings = new List<Thing>();

        // Token: 0x04000017 RID: 23
        private int Ticks = 250;

        // Token: 0x04000018 RID: 24
        private int TickRate = 250;
    }


    // Token: 0x02000004 RID: 4
    public class Thing_AddAcidDamage : Gas
    {
        // Token: 0x17000003 RID: 3
        // (get) Token: 0x06000017 RID: 23 RVA: 0x000027A2 File Offset: 0x000009A2
        // (set) Token: 0x06000018 RID: 24 RVA: 0x000027AA File Offset: 0x000009AA
        public object cachedLabelMouseover { get; private set; }

        // Token: 0x06000019 RID: 25 RVA: 0x000027B3 File Offset: 0x000009B3
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
        }

        // Token: 0x0600001A RID: 26 RVA: 0x000027C0 File Offset: 0x000009C0
        public override void Tick()
        {
            bool destroyed = base.Destroyed;
            if (!destroyed)
            {
                base.Tick();
                bool flag = this.destroyTick <= Find.TickManager.TicksGame && !base.Destroyed;
                if (flag)
                {
                    this.Destroy(0);
                }
                else
                {
                    this.graphicRotation += this.graphicRotationSpeed;
                    this.Ticks--;
                    bool flag2 = this.Ticks <= 0;
                    if (flag2)
                    {
                        this.TickTack();
                        this.Ticks = this.TickRate;
                    }
                }
            }
        }

        // Token: 0x0600001B RID: 27 RVA: 0x00002854 File Offset: 0x00000A54
        public void TickTack()
        {
            bool destroyed = base.Destroyed;
            if (!destroyed)
            {
                List<Thing> thingList = GridsUtility.GetThingList(base.Position, base.Map);
                for (int i = 0; i < thingList.Count; i++)
                {
                    bool flag = thingList[i] != null;
                    if (flag)
                    {
                        Thing thing = thingList[i];
                        Pawn pawn = thingList[i] as Pawn;
                        bool flag2 = thing != null && !this.touchingThings.Contains(thing);
                        if (flag2)
                        {
                            this.touchingThings.Add(thing);
                            this.damageEntities(thing, Mathf.RoundToInt((float)this.AcidDamage * Rand.Range(0.5f, 1.25f)));
                            MoteMaker.ThrowDustPuff(thing.Position, base.Map, 0.2f);
                        }
                        bool flag3 = pawn != null;
                        if (flag3)
                        {
                            this.touchingPawns.Add(pawn);
                            bool flag4 = !pawn.RaceProps.Animal;
                            if (flag4)
                            {
                                this.addAcidDamage(pawn);
                                MoteMaker.ThrowDustPuff(pawn.Position, base.Map, 0.2f);
                            }
                        }
                    }
                }
                for (int j = 0; j < this.touchingPawns.Count; j++)
                {
                    Pawn pawn2 = this.touchingPawns[j];
                    bool flag5 = !pawn2.Spawned || pawn2.Position != base.Position;
                    if (flag5)
                    {
                        this.touchingPawns.Remove(pawn2);
                    }
                    else
                    {
                        bool flag6 = !pawn2.RaceProps.Animal;
                        if (flag6)
                        {
                            this.addAcidDamage(pawn2);
                        }
                    }
                }
                for (int k = 0; k < this.touchingThings.Count; k++)
                {
                    Thing thing2 = this.touchingThings[k];
                    bool flag7 = !thing2.Spawned || thing2.Position != base.Position;
                    if (flag7)
                    {
                        this.touchingThings.Remove(thing2);
                    }
                    else
                    {
                        this.damageEntities(thing2, Mathf.RoundToInt((float)this.AcidDamage * Rand.Range(0.5f, 1.25f)));
                    }
                }
                this.damageBuildings(Mathf.RoundToInt((float)this.AcidDamage * Rand.Range(0.5f, 1.25f)));
                this.cachedLabelMouseover = null;
            }
        }

        // Token: 0x0600001C RID: 28 RVA: 0x00002AD4 File Offset: 0x00000CD4
        public void damageEntities(Thing e, int amt)
        {
            DamageInfo damageInfo = new DamageInfo(DamageDefOf.Burn, (float)amt, 0f, -1f, null, null, null, 0, null);
            bool flag = e != null;
            if (flag)
            {
                e.TakeDamage(damageInfo);
                MoteMaker.ThrowDustPuff(e.Position, base.Map, 0.2f);
            }
        }

        // Token: 0x0600001D RID: 29 RVA: 0x00002B28 File Offset: 0x00000D28
        public void damageBuildings(int amt)
        {
            IntVec3 intVec = GenAdj.RandomAdjacentCell8Way(this);
            bool flag = GenGrid.InBounds(intVec, base.Map);
            bool flag2 = flag;
            if (flag2)
            {
                Building firstBuilding = GridsUtility.GetFirstBuilding(intVec, base.Map);
                DamageInfo damageInfo = new DamageInfo(DamageDefOf.Burn, (float)amt, 0f, -1f, null, null, null, 0, null);
                bool flag3 = firstBuilding != null;
                bool flag4 = flag3;
                if (flag4)
                {
                    firstBuilding.TakeDamage(damageInfo);
                    MoteMaker.ThrowDustPuff(firstBuilding.Position, base.Map, 0.2f);
                }
            }
        }

        // Token: 0x0600001E RID: 30 RVA: 0x00002BAC File Offset: 0x00000DAC
        public void addAcidDamage(Pawn p)
        {
            List<BodyPartRecord> list = new List<BodyPartRecord>();
            List<Apparel> wornApparel = p.apparel.WornApparel;
            int num = Mathf.RoundToInt((float)this.AcidDamage * Rand.Range(0.5f, 1.25f));
            DamageInfo damageInfo = default(DamageInfo);
            MoteMaker.ThrowDustPuff(p.Position, base.Map, 0.2f);
            foreach (BodyPartRecord bodyPartRecord in p.health.hediffSet.GetNotMissingParts(0, BodyPartDepth.Outside, null, null))
            {
                bool flag = wornApparel.Count > 0;
                if (flag)
                {
                    bool flag2 = false;
                    for (int i = 0; i < wornApparel.Count; i++)
                    {
                        bool flag3 = wornApparel[i].def.apparel.CoversBodyPart(bodyPartRecord);
                        if (flag3)
                        {
                            flag2 = true;
                            break;
                        }
                    }
                    bool flag4 = !flag2;
                    if (flag4)
                    {
                        list.Add(bodyPartRecord);
                    }
                }
                else
                {
                    list.Add(bodyPartRecord);
                }
            }
            for (int j = 0; j < wornApparel.Count; j++)
            {
                this.damageEntities(wornApparel[j], num);
            }
            for (int k = 0; k < list.Count; k++)
            {
                damageInfo = new DamageInfo(DamageDefOf.Burn, (float)Mathf.RoundToInt((float)num * list[k].coverage), 0f, -1f, this, list[k], null, 0, null);
                p.TakeDamage(damageInfo);
            }
        }

        // Token: 0x0400000F RID: 15
        private List<Pawn> touchingPawns = new List<Pawn>();

        // Token: 0x04000010 RID: 16
        private List<Thing> touchingThings = new List<Thing>();

        // Token: 0x04000011 RID: 17
        private int Ticks = 100;

        // Token: 0x04000012 RID: 18
        private int TickRate = 100;

        // Token: 0x04000013 RID: 19
        private int AcidDamage = 3;
    }
}
