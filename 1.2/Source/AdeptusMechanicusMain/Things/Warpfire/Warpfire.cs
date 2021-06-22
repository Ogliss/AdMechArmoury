using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.Warpfire
    public class Warpfire : Fire
    {
        // Token: 0x170005C7 RID: 1479
        // (get) Token: 0x06002643 RID: 9795 RVA: 0x00122E3D File Offset: 0x0012123D
        public override string Label
        {
            get
            {
                if (this.parent != null)
                {
                    return "FireOn".Translate(this.parent.LabelCap, this.parent);
                }
                return this.def.label;
            }
        }

        // Token: 0x170005C8 RID: 1480
        // (get) Token: 0x06002644 RID: 9796 RVA: 0x00122E7C File Offset: 0x0012127C
        public override string InspectStringAddon
        {
            get
            {
                return "Burning".Translate() + " (" + "FireSizeLower".Translate((this.fireSize * 100f).ToString("F0")) + ")";
            }
        }

        // Token: 0x170005C9 RID: 1481
        // (get) Token: 0x06002645 RID: 9797 RVA: 0x00122ECC File Offset: 0x001212CC
        private float SpreadInterval
        {
            get
            {
                float num = 150f - (this.fireSize - 1f) * 40f;
                if (num < 75f)
                {
                    num = 75f;
                }
                return num;
            }
        }

        // Token: 0x06002646 RID: 9798 RVA: 0x00122F04 File Offset: 0x00121304
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.ticksSinceSpawn, "ticksSinceSpawn", 0, false);
            Scribe_Values.Look<float>(ref this.fireSize, "fireSize", 0f, false);
        }

        // Token: 0x06002647 RID: 9799 RVA: 0x00122F34 File Offset: 0x00121334
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.RecalcPathsOnAndAroundMe(map);
            LessonAutoActivator.TeachOpportunity(ConceptDefOf.HomeArea, this, OpportunityType.Important);
            Rand.PushState();
            this.ticksSinceSpread = (int)(this.SpreadInterval * Rand.Value);
            Rand.PopState();
        }

        // Token: 0x06002649 RID: 9801 RVA: 0x00122F6C File Offset: 0x0012136C
        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            if (this.sustainer != null)
            {
                if (this.sustainer.externalParams.sizeAggregator == null)
                {
                    this.sustainer.externalParams.sizeAggregator = new SoundSizeAggregator();
                }
                this.sustainer.externalParams.sizeAggregator.RemoveReporter(this);
            }
            Map map = base.Map;
            base.DeSpawn(mode);
            this.RecalcPathsOnAndAroundMe(map);
        }

        // Token: 0x0600264A RID: 9802 RVA: 0x00122FDC File Offset: 0x001213DC
        private void RecalcPathsOnAndAroundMe(Map map)
        {
            IntVec3[] adjacentCellsAndInside = GenAdj.AdjacentCellsAndInside;
            for (int i = 0; i < adjacentCellsAndInside.Length; i++)
            {
                IntVec3 c = base.Position + adjacentCellsAndInside[i];
                if (c.InBounds(map))
                {
                    map.pathGrid.RecalculatePerceivedPathCostAt(c);
                }
            }
        }

        // Token: 0x0600264B RID: 9803 RVA: 0x00123038 File Offset: 0x00121438
        public override void AttachTo(Thing parent)
        {
            base.AttachTo(parent);
            Pawn pawn = parent as Pawn;
            if (pawn != null)
            {
                TaleRecorder.RecordTale(TaleDefOf.WasOnFire, new object[]
                {
                    pawn
                });
            }
        }

        // Token: 0x0600264C RID: 9804 RVA: 0x00123070 File Offset: 0x00121470
        public override void Tick()
        {
            this.ticksSinceSpawn++;
            if (Warpfire.lastFireCountUpdateTick != Find.TickManager.TicksGame)
            {
                Warpfire.fireCount = base.Map.listerThings.ThingsOfDef(this.def).Count;
                Warpfire.lastFireCountUpdateTick = Find.TickManager.TicksGame;
            }
            if (this.sustainer != null)
            {
                this.sustainer.Maintain();
            }
            else if (!base.Position.Fogged(base.Map))
            {
                SoundInfo info = SoundInfo.InMap(new TargetInfo(base.Position, base.Map, false), MaintenanceType.PerTick);
                this.sustainer = SustainerAggregatorUtility.AggregateOrSpawnSustainerFor(this, SoundDefOf.FireBurning, info);
            }
            this.ticksUntilSmoke--;
            if (this.ticksUntilSmoke <= 0)
            {
                this.SpawnSmokeParticles();
            }
            Rand.PushState();
            if (Warpfire.fireCount < 15 && this.fireSize > 0.7f && Rand.Value < this.fireSize * 0.01f)
            {
                ThrowMicroSparks(this.DrawPos, base.Map);
            }
            Rand.PopState();
            if (this.fireSize > 1f)
            {
                this.ticksSinceSpread++;
                if ((float)this.ticksSinceSpread >= this.SpreadInterval)
                {
                    this.TrySpread();
                    this.ticksSinceSpread = 0;
                }
            }
            if (this.IsHashIntervalTick(150))
            {
                this.DoComplexCalcs();
            }
            if (this.ticksSinceSpawn >= 7500)
            {
                this.TryBurnFloor();
            }
            if (this.IsHashIntervalTick(150))
            {
                Rand.PushState();
                bool despawn = Rand.Chance(0.25f);
                Rand.PopState();
                if (despawn && this.Spawned)
                {
                    this.DeSpawn();
                }
            }
        }

        // Token: 0x0600264D RID: 9805 RVA: 0x001231F8 File Offset: 0x001215F8
        private void SpawnSmokeParticles()
        {
            if (Warpfire.fireCount < 15)
            {
                MoteMaker.ThrowSmoke(this.DrawPos, base.Map, this.fireSize);
            }
            if (this.fireSize > 0.5f && this.parent == null)
            {
                ThrowFireGlow(base.Position, base.Map, this.fireSize);
            }
            float num = this.fireSize / 2f;
            if (num > 1f)
            {
                num = 1f;
            }
            num = 1f - num;
            Rand.PushState();
            this.ticksUntilSmoke = Warpfire.SmokeIntervalRange.Lerped(num) + (int)(10f * Rand.Value);
            Rand.PopState();
        }
        // Token: 0x060026BE RID: 9918 RVA: 0x00126330 File Offset: 0x00124730
        public static void ThrowFireGlow(IntVec3 c, Map map, float size)
        {
            Vector3 vector = c.ToVector3Shifted();
            if (!vector.ShouldSpawnMotesAt(map) || map.moteCounter.SaturatedLowPriority)
            {
                return;
            }
            Rand.PushState();
            vector += size * new Vector3(Rand.Value - 0.5f, 0f, Rand.Value - 0.5f);
            Rand.PopState();
            if (!vector.InBounds(map))
            {
                return;
            }
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(AdeptusThingDefOf.OG_Mote_WarpFireGlow, null);
            Rand.PushState();
            moteThrown.Scale = Rand.Range(4f, 6f) * size;
            moteThrown.rotationRate = Rand.Range(-3f, 3f);
            moteThrown.exactPosition = vector;
            moteThrown.SetVelocity((float)Rand.Range(0, 360), 0.12f);
            Rand.PopState();
            GenSpawn.Spawn(moteThrown, vector.ToIntVec3(), map, WipeMode.Vanish);
        }
        // Token: 0x060026C0 RID: 9920 RVA: 0x001264E0 File Offset: 0x001248E0
        public static void ThrowMicroSparks(Vector3 loc, Map map)
        {
            if (!loc.ShouldSpawnMotesAt(map) || map.moteCounter.SaturatedLowPriority)
            {
                return;
            }
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(AdeptusThingDefOf.OG_Mote_MicroSparksWarp, null);
            Rand.PushState();
            moteThrown.Scale = Rand.Range(0.8f, 1.2f);
            moteThrown.rotationRate = Rand.Range(-12f, 12f);
            moteThrown.exactPosition = loc;
            moteThrown.exactPosition -= new Vector3(0.5f, 0f, 0.5f);
            moteThrown.exactPosition += new Vector3(Rand.Value, 0f, Rand.Value);
            moteThrown.SetVelocity((float)Rand.Range(35, 45), 1.2f);
            Rand.PopState();
            GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map, WipeMode.Vanish);
        }
        // Token: 0x0600264E RID: 9806 RVA: 0x001232A4 File Offset: 0x001216A4
        private void DoComplexCalcs()
        {
            bool flag = false;
            Warpfire.flammableList.Clear();
            this.flammabilityMax = 0f;
            if (!base.Position.GetTerrain(base.Map).extinguishesFire)
            {
                if (this.parent == null)
                {
                    if (base.Position.TerrainFlammableNowWarp(base.Map))
                    {
                        this.flammabilityMax = base.Position.GetTerrain(base.Map).GetStatValueAbstract(StatDefOf.Flammability, null);
                    }
                    List<Thing> list = base.Map.thingGrid.ThingsListAt(base.Position);
                    for (int i = 0; i < list.Count; i++)
                    {
                        Thing thing = list[i];
                        if (thing is Building_Door)
                        {
                            flag = true;
                        }
                        float statValue = thing.GetStatValue(StatDefOf.Flammability, true);
                        if (statValue >= 0.01f)
                        {
                            Warpfire.flammableList.Add(list[i]);
                            if (statValue > this.flammabilityMax)
                            {
                                this.flammabilityMax = statValue;
                            }
                            if (this.parent == null && this.fireSize > 0.4f && list[i].def.category == ThingCategory.Pawn)
                            {
                                list[i].TryAttachWarpfire(this.fireSize * 0.2f);
                            }
                        }
                    }
                }
                else
                {
                    Warpfire.flammableList.Add(this.parent);
                    this.flammabilityMax = this.parent.GetStatValue(StatDefOf.Flammability, true);
                }
            }
            if (this.flammabilityMax < 0.01f)
            {
                this.Destroy(DestroyMode.Vanish);
                return;
            }
            Thing thing2;
            if (this.parent != null)
            {
                thing2 = this.parent;
            }
            else if (Warpfire.flammableList.Count > 0)
            {
                thing2 = Warpfire.flammableList.RandomElement<Thing>();
            }
            else
            {
                thing2 = null;
            }
            if (thing2 != null && (this.fireSize >= 0.4f || thing2 == this.parent || thing2.def.category != ThingCategory.Pawn))
            {
                this.DoWarpfireDamage(thing2);
            }
            if (base.Spawned)
            {
                float num = this.fireSize * 160f;
                if (flag)
                {
                    num *= 0.15f;
                }
                GenTemperature.PushHeat(base.Position, base.Map, num);
                Rand.PushState();
                if (Rand.Value < 0.4f)
                {
                    float radius = this.fireSize * 3f;
                    SnowUtility.AddSnowRadial(base.Position, base.Map, radius, -(this.fireSize * 0.1f));
                }
                Rand.PopState();
                this.fireSize += 0.00055f * this.flammabilityMax * 150f;
                if (this.fireSize > 1.75f)
                {
                    this.fireSize = 1.75f;
                }
                Rand.PushState();
                if (base.Map.weatherManager.RainRate > 0.01f && this.VulnerableToRain() && Rand.Value < 6f)
                {
                    base.TakeDamage(new DamageInfo(DamageDefOf.Extinguish, 10f, 0f, -1f, null, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null));
                }
                Rand.PopState();
            }
        }

        // Token: 0x0600264F RID: 9807 RVA: 0x001235C8 File Offset: 0x001219C8
        private void TryBurnFloor()
        {
            if (this.parent != null || !base.Spawned)
            {
                return;
            }
            if (base.Position.TerrainFlammableNowWarp(base.Map))
            {
                base.Map.terrainGrid.Notify_TerrainBurned(base.Position);
            }
        }

        // Token: 0x06002650 RID: 9808 RVA: 0x00123618 File Offset: 0x00121A18
        private bool VulnerableToRain()
        {
            return false;
        }

        // Token: 0x06002651 RID: 9809 RVA: 0x00123684 File Offset: 0x00121A84
        private void DoWarpfireDamage(Thing targ)
        {
            float num = 0.0125f + 0.0036f * this.fireSize;
            num = Mathf.Clamp(num, 0.0125f, 0.05f);
            int num2 = GenMath.RoundRandom(num * 150f);
            if (num2 < 1)
            {
                num2 = 1;
            }
            Pawn pawn = targ as Pawn;
            if (pawn != null)
            {
                BattleLogEntry_DamageTaken battleLogEntry_DamageTaken = new BattleLogEntry_DamageTaken(pawn, RulePackDefOf.DamageEvent_Fire, null);
                Find.BattleLog.Add(battleLogEntry_DamageTaken);
                DamageInfo dinfo = new DamageInfo(AdeptusDamageDefOf.OG_Chaos_Deamon_Warpfire, (float)num2, 0f, -1f, this, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null);
                dinfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
                targ.TakeDamage(dinfo).AssociateWithLog(battleLogEntry_DamageTaken);
                Apparel apparel;
                if (pawn.apparel != null && pawn.apparel.WornApparel.TryRandomElement(out apparel))
                {
                    apparel.TakeDamage(new DamageInfo(AdeptusDamageDefOf.OG_Chaos_Deamon_Warpfire, (float)num2, 0f, -1f, this, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null));
                }
            }
            else
            {
                targ.TakeDamage(new DamageInfo(AdeptusDamageDefOf.OG_Chaos_Deamon_Warpfire, (float)num2, 0f, -1f, this, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null));
            }
        }

        // Token: 0x06002652 RID: 9810 RVA: 0x00123790 File Offset: 0x00121B90
        protected new void TrySpread()
        {
            IntVec3 intVec = base.Position;
            bool flag;
            Rand.PushState();
            if (Rand.Chance(0.8f))
            {
                intVec = base.Position + GenRadial.ManualRadialPattern[Rand.RangeInclusive(1, 8)];
                flag = true;
            }
            else
            {
                intVec = base.Position + GenRadial.ManualRadialPattern[Rand.RangeInclusive(10, 20)];
                flag = false;
            }
            Rand.PopState();
            if (!intVec.InBounds(base.Map))
            {
                return;
            }
            Rand.PushState();
            bool startfire = Rand.Chance(WarpfireUtility.ChanceToStartWarpfireIn(intVec, base.Map));
            Rand.PopState();
            if (startfire)
            {
                if (!flag)
                {
                    CellRect startRect = CellRect.SingleCell(base.Position);
                    CellRect endRect = CellRect.SingleCell(intVec);
                    if (!GenSight.LineOfSight(base.Position, intVec, base.Map, startRect, endRect, null))
                    {
                        return;
                    }
                    WarpSpark spark = (WarpSpark)GenSpawn.Spawn(AdeptusThingDefOf.OG_WarpSpark, base.Position, base.Map, WipeMode.Vanish);
                    spark.Launch(this, intVec, intVec, ProjectileHitFlags.All, null);
                }
                else
                {
                    WarpfireUtility.TryStartWarpfireIn(intVec, base.Map, 0.1f);
                }
            }
        }

        // Token: 0x04001588 RID: 5512
        private int ticksSinceSpawn;

        // Token: 0x0400158A RID: 5514
        private int ticksSinceSpread;

        // Token: 0x0400158B RID: 5515
        private float flammabilityMax = 0.5f;

        // Token: 0x0400158C RID: 5516
        private int ticksUntilSmoke;

        // Token: 0x0400158D RID: 5517
        private Sustainer sustainer;

        // Token: 0x0400158E RID: 5518
        private static List<Thing> flammableList = new List<Thing>();

        // Token: 0x0400158F RID: 5519
        private static int fireCount;

        // Token: 0x04001590 RID: 5520
        private static int lastFireCountUpdateTick;

        // Token: 0x04001592 RID: 5522
        private const float MinSizeForSpark = 1f;

        // Token: 0x04001593 RID: 5523
        private const float TicksBetweenSparksBase = 150f;

        // Token: 0x04001594 RID: 5524
        private const float TicksBetweenSparksReductionPerFireSize = 40f;

        // Token: 0x04001595 RID: 5525
        private const float MinTicksBetweenSparks = 75f;

        // Token: 0x04001596 RID: 5526
        private const float MinFireSizeToEmitSpark = 1f;

        // Token: 0x04001598 RID: 5528
        private const int TicksToBurnFloor = 7500;

        // Token: 0x04001599 RID: 5529
        private const int ComplexCalcsInterval = 150;

        // Token: 0x0400159A RID: 5530
        private const float CellIgniteChancePerTickPerSize = 0.01f;

        // Token: 0x0400159B RID: 5531
        private const float MinSizeForIgniteMovables = 0.4f;

        // Token: 0x0400159C RID: 5532
        private const float FireBaseGrowthPerTick = 0.00055f;

        // Token: 0x0400159D RID: 5533
        private static readonly IntRange SmokeIntervalRange = new IntRange(130, 200);

        // Token: 0x0400159E RID: 5534
        private const int SmokeIntervalRandomAddon = 10;

        // Token: 0x0400159F RID: 5535
        private const float BaseSkyExtinguishChance = 0.04f;

        // Token: 0x040015A0 RID: 5536
        private const int BaseSkyExtinguishDamage = 10;

        // Token: 0x040015A1 RID: 5537
        private const float HeatPerFireSizePerInterval = 160f;

        // Token: 0x040015A2 RID: 5538
        private const float HeatFactorWhenDoorPresent = 0.15f;

        // Token: 0x040015A3 RID: 5539
        private const float SnowClearRadiusPerFireSize = 3f;

        // Token: 0x040015A4 RID: 5540
        private const float SnowClearDepthFactor = 0.1f;

        // Token: 0x040015A5 RID: 5541
        private const int FireCountParticlesOff = 15;
    }
}
