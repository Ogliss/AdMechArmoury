using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x02000006 RID: 6
    public class EffectGas : Gas
    {
        public AdeptusGasProperties GasProps => this.def.gas as AdeptusGasProperties;
        public object cachedLabelMouseover { get; private set; }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.TickRate = this.Ticks;
            this.Ticks = this.TickRate;
        }

        // Token: 0x06000024 RID: 36 RVA: 0x00002DD4 File Offset: 0x00000FD4
        public override void Tick()
        {
            if (!base.Destroyed)
            {
                base.Tick();
                if (this.destroyTick <= Find.TickManager.TicksGame && !base.Destroyed)
                {
                    this.Destroy(0);
                }
                else
                {
                    this.graphicRotation += this.graphicRotationSpeed;
                    this.Ticks--;
                    if (this.Ticks <= 0)
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
            if (!base.Destroyed)
            {
                if (GasProps?.addHediff != null)
                {
                    List<Thing> thingList = GridsUtility.GetThingList(base.Position, base.Map);
                    for (int i = 0; i < thingList.Count; i++)
                    {
                        Pawn pawn = thingList[i] as Pawn;
                        if (pawn != null && !this.touchingPawns.Contains(pawn))
                        {
                            this.touchingPawns.Add(pawn);
                            this.AddHediffToPawn(pawn, GasProps.addHediff, GasProps.hediffAddChance, GasProps.hediffSeverity, GasProps.onlyAffectLungs);
                        }
                    }
                    for (int j = 0; j < this.touchingPawns.Count; j++)
                    {
                        Pawn pawn2 = this.touchingPawns[j];
                        if (!pawn2.Spawned || pawn2.Position != base.Position)
                        {
                            this.touchingPawns.Remove(pawn2);
                        }
                    }
                }
                this.cachedLabelMouseover = null;
            }
        }

        // Token: 0x06000026 RID: 38 RVA: 0x00002F9C File Offset: 0x0000119C
        public void DamageEntities(Thing e, int amt)
        {
            DamageInfo damageInfo = new DamageInfo(DamageDefOf.Burn, (float)amt, 0f, -1f, null, null, null, 0, null);
            if (e != null)
            {
                e.TakeDamage(damageInfo);
            }
        }

        // Token: 0x06000027 RID: 39 RVA: 0x00002FDC File Offset: 0x000011DC
        public void DamageBuildings(int amt)
        {
            IntVec3 intVec = GenAdj.RandomAdjacentCell8Way(this);
            if (GenGrid.InBounds(intVec, base.Map))
            {
                Building firstBuilding = GridsUtility.GetFirstBuilding(intVec, base.Map);
                DamageInfo damageInfo = new DamageInfo(DamageDefOf.Burn, (float)amt, 0f, -1f, null, null, null, 0, null);

                if (firstBuilding != null)
                {
                    firstBuilding.TakeDamage(damageInfo);
                }
            }
        }

        // Token: 0x06000028 RID: 40 RVA: 0x00003048 File Offset: 0x00001248
        public void AddHediffToPawn(Pawn p, HediffDef _heddiff, float _addhediffChance, float _hediffseverity, bool onlylungs)
        {
            bool EyeProtection = false;
            bool LungProtection = false;
            Rand.PushState();
            bool flag = !Rand.Chance(_addhediffChance);
            Rand.PopState();
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
                            Rand.PushState();
                            float num2 = Mathf.Lerp(0.85f, 1.15f, Rand.ValueSeeded(p.thingIDNumber ^ 74374237));
                            Rand.PopState();
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
                                        hediff2 = (hediffSet?.GetFirstHediffOfDef(_heddiff, false));
                                    }
                                }
                                Hediff hediff3 = hediff2;
                                Rand.PushState();
                                float num3 = Rand.Range(0.1f, 0.2f);
                                Rand.PopState();
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
                            hediff4 = (hediffSet2?.GetFirstHediffOfDef(_heddiff, false));
                        }
                    }
                    Hediff hediff5 = hediff4;
                    Rand.PushState();
                    float num4 = Rand.Range(0.1f, 0.2f);
                    Rand.PopState();
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
}
