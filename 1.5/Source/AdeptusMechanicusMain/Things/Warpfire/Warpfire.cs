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

        public override string InspectStringAddon
        {
            get
            {
                return "Burning".Translate() + " (" + "FireSizeLower".Translate((this.fireSize * 100f).ToString("F0")) + ")";
            }
        }

        public new float SpreadInterval
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

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.RecalcPathsOnAndAroundMe(map);
            LessonAutoActivator.TeachOpportunity(ConceptDefOf.HomeArea, this, OpportunityType.Important);
            Rand.PushState();
            this.ticksSinceSpread = (int)(this.SpreadInterval * Rand.Value);
            Rand.PopState();
        }

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

        public new void RecalcPathsOnAndAroundMe(Map map)
        {
            IntVec3[] adj = GenAdj.AdjacentCellsAndInside;
            for (int i = 0; i < adj.Length; i++)
            {
                IntVec3 c = base.Position + adj[i];
                bool flag = !c.InBounds(map);
                if (!flag)
                {
                    map.pathing.RecalculatePerceivedPathCostAt(c);
                }
            }
        }

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

        public new void SpawnSmokeParticles()
        {
            if (Warpfire.fireCount < 15)
            {
                FleckMaker.ThrowSmoke(this.DrawPos, base.Map, this.fireSize);
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

        public new void DoComplexCalcs()
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

        public new bool VulnerableToRain()
        {
            return false;
        }

        public void DoWarpfireDamage(Thing targ)
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
                    spark.Launch(this, intVec, intVec, ProjectileHitFlags.All, false, null);
                }
                else
                {
                    WarpfireUtility.TryStartWarpfireIn(intVec, base.Map, 0.1f);
                }
            }
        }
    }
}
