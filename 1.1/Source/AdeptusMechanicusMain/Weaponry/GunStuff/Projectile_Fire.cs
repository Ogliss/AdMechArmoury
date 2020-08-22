using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.Projectile_Fire
    public class Projectile_Fire : Projectile_Anim
    {
        protected override void Impact(Thing hitThing)
        {
        //    base.Impact(hitThing);
            Ignite();
        }

        public override void Tick()
        {
            base.Tick();

            distancetotravel = launcher.Position.DistanceTo(usedTarget.Cell);
            distancetraveled = launcher.Position.DistanceTo(this.Position);
            traveled = (distancetraveled / distancetotravel);
            pos = this.Position;
            checked
            {
                this.age++;
                this.TicksforAppearence--;
                bool flag = this.TicksforAppearence == 0 && base.Map != null;
                if (flag)
                {
                    if (pos != IntVec3.Invalid)
                    {
                        if (pos != this.Position)
                        {
                            if (Rand.Chance(0.75f * traveled))
                            {
                                TrySpread();
                            }
                        }
                    }
                    if (Rand.Chance(0.75f * traveled))
                    {
                        ThrowSmoke(this.DrawPos, base.Map, 0.5f * traveled);
                        if (traveled > 0.5f)
                        {
                            if (Rand.Chance(0.25f * traveled))
                            {
                                Ignite();
                            }
                        }
                    }
                    this.TicksforAppearence = 6;
                }
            }
        }

        protected virtual void Ignite()
        {
            Map map = Map;
            Destroy();
            float ignitionChance = def.projectile.explosionChanceToStartFire;
            var radius = def.projectile.explosionRadius;
            var cellsToAffect = SimplePool<List<IntVec3>>.Get();
            cellsToAffect.Clear();
            cellsToAffect.AddRange(def.projectile.damageDef.Worker.ExplosionCellsToHit(Position, map, radius));

            MoteMaker.MakeStaticMote(Position, map, ThingDefOf.Mote_ExplosionFlash, radius * 4f);
            for (int i = 0; i < 4; i++)
            {
                MoteMaker.ThrowSmoke(Position.ToVector3Shifted() + Gen.RandomHorizontalVector(radius * 0.7f), map, radius * 0.6f);
            }

            Rand.PushState();
            if (Rand.Chance(ignitionChance))
                foreach (var vec3 in cellsToAffect)
                {
                    var fireSize = radius - vec3.DistanceTo(Position);
                    if (fireSize > 0.1f)
                    {
                        if (this.def.projectile.damageDef == OGDamageDefOf.OG_Chaos_Deamon_Warpfire)
                        {
                            WarpfireUtility.TryStartWarpfireIn(vec3, map, fireSize);
                        }
                        else
                        {
                            FireUtility.TryStartFireIn(vec3, map, fireSize);
                        }
                    }
                }
            Rand.PopState();

            //Fire explosion should be tiny.
            if (this.def.projectile.explosionEffect != null)
            {
                Effecter effecter = this.def.projectile.explosionEffect.Spawn();
                effecter.Trigger(new TargetInfo(this.Position, map, false), new TargetInfo(this.Position, map, false));
                effecter.Cleanup();
            }
            GenExplosion.DoExplosion(this.Position, map, this.def.projectile.explosionRadius, this.def.projectile.damageDef, this.launcher, this.def.projectile.GetDamageAmount(1, null), this.def.projectile.GetArmorPenetration(1, null), this.def.projectile.soundExplode, this.equipmentDef, this.def, null, this.def.projectile.postExplosionSpawnThingDef, this.def.projectile.postExplosionSpawnChance, this.def.projectile.postExplosionSpawnThingCount, this.def.projectile.applyDamageToExplosionCellsNeighbors, this.def.projectile.preExplosionSpawnThingDef, this.def.projectile.preExplosionSpawnChance, this.def.projectile.preExplosionSpawnThingCount, this.def.projectile.explosionChanceToStartFire, this.def.projectile.explosionDamageFalloff);
        }

        protected void TrySpread()
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

            float chance;

            if (this.def.projectile.damageDef == OGDamageDefOf.OG_Chaos_Deamon_Warpfire)
            {
                chance = WarpfireUtility.ChanceToStartWarpfireIn(intVec, base.Map);
            }
            else
            {
                chance = FireUtility.ChanceToStartFireIn(intVec, base.Map);
            }

            Rand.PushState();
            bool f = Rand.Chance(chance);
            Rand.PopState();
            if (f)
            {
                if (!flag)
                {
                    CellRect startRect = CellRect.SingleCell(base.Position);
                    CellRect endRect = CellRect.SingleCell(intVec);
                    if (!GenSight.LineOfSight(base.Position, intVec, base.Map, startRect, endRect, null))
                    {
                        return;
                    }
                    Spark spark;
                    if (this.def.projectile.damageDef == OGDamageDefOf.OG_Chaos_Deamon_Warpfire)
                    {
                        spark = (Spark)GenSpawn.Spawn(OGThingDefOf.OG_WarpSpark, base.Position, base.Map, WipeMode.Vanish);
                    }
                    else
                    {
                        spark = (Spark)GenSpawn.Spawn(ThingDefOf.Spark, base.Position, base.Map, WipeMode.Vanish);
                    }
                    spark.Launch(this, intVec, intVec, ProjectileHitFlags.All, null);
                }
                else
                {
                    if (this.def.projectile.damageDef == OGDamageDefOf.OG_Chaos_Deamon_Warpfire)
                    {
                        WarpfireUtility.TryStartWarpfireIn(intVec, base.Map, 0.1f);
                    }
                    else
                    {
                        FireUtility.TryStartFireIn(intVec, base.Map, 0.1f);
                    }
                }
            }
        }


        /*
        private void SpawnSmokeParticles()
        {
            if (Fire.fireCount < 15)
            {
                MoteMaker.ThrowSmoke(this.DrawPos, base.Map, this.fireSize);
            }
            if (this.fireSize > 0.5f && this.parent == null)
            {
                MoteMaker.ThrowFireGlow(base.Position, base.Map, this.fireSize);
            }
            float num = this.fireSize / 2f;
            if (num > 1f)
            {
                num = 1f;
            }
            num = 1f - num;
            this.ticksUntilSmoke = Fire.SmokeIntervalRange.Lerped(num) + (int)(10f * Rand.Value);
        }
        */

        public override Quaternion ExactRotation
        {
            get
            {
                var forward = destination - origin;
                forward.y = 0;
                return Quaternion.LookRotation(forward);
            }
        }

        // Token: 0x060026BE RID: 9918 RVA: 0x00126340 File Offset: 0x00124740
        public static void ThrowSmoke(Vector3 loc, Map map, float size)
        {
            if (!loc.ShouldSpawnMotesAt(map) || map.moteCounter.SaturatedLowPriority)
            {
                return;
            }
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Mote_Smoke"), null);
            moteThrown.Scale = Rand.Range(1.5f, 2.5f) * size;
            moteThrown.rotationRate = Rand.Range(-30f, 30f);
            moteThrown.exactPosition = loc;
            moteThrown.SetVelocity((float)Rand.Range(30, 40), Rand.Range(0.5f, 0.7f));
            GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map, WipeMode.Vanish);
        }
        
        public override void Draw()
        {
            string mote = "Mote_FlameGlow";
            if (this.def.projectile.damageDef == OGDamageDefOf.OG_Chaos_Deamon_Warpfire)
            {
                mote = "OG_Mote_WarpFireGlow";
            }
            Mesh mesh = MeshPool.GridPlane(this.def.graphicData.drawSize * traveled);
            Mesh mesh2 = MeshPool.GridPlane(DefDatabase<ThingDef>.GetNamed(mote).graphicData.drawSize * (traveled * 7));
            Graphics.DrawMesh(mesh, this.DrawPos, this.ExactRotation, Graphic.MatSingle, 0);
            Graphics.DrawMesh(mesh2, this.DrawPos, this.ExactRotation, DefDatabase<ThingDef>.GetNamed(mote).graphic.MatSingle, 0);
            base.Comps_PostDraw();
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
        }
        private float distancetotravel = 0;
        private float distancetraveled = 0;
        private float traveled = 0;
        private int TicksforAppearence = 15;
        private int age = 0;
        private IntVec3 pos = IntVec3.Invalid;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref distancetotravel, "distancetotravel");
            Scribe_Values.Look(ref distancetraveled, "distancetraveled");
            Scribe_Values.Look(ref TicksforAppearence, "TicksforAppearence");
            Scribe_Values.Look(ref age, "age");
        }
    }
}
