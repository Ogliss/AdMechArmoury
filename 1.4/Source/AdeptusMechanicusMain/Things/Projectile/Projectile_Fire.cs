using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.Projectile_Fire
    [StaticConstructorOnStartup]
    public class Projectile_Fire : Projectile_Anim
    {
        public override void Impact(Thing hitThing, bool blockedByShield = false)
        {
        //    base.Impact(hitThing);
            Ignite();
        }

        public override void Tick()
        {
            base.Tick();

            this.realPosition = new Vector2(base.ExactPosition.x, base.ExactPosition.z);
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
                        TrailThrower.ThrowSprayTrail(DrawPos, Map, origin, destination, null, 1.5f * traveled, 240, def.projectile.SpeedTilesPerTick);
                        if (pos != this.Position)
                        {
                            Rand.PushState();
                            if (Rand.Chance(0.75f * traveled))
                            {
                                TrySpread();
                            }
                            Rand.PopState();
                        }
                    }
                    Rand.PushState();
                    if (Rand.Chance(0.75f * traveled))
                    {
                        if (traveled > 0.5f)
                        {
                            if (Rand.Chance(0.25f * traveled))
                            {
                                Ignite();
                            }
                        }
                    }
                    Rand.PopState();
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

            FleckMaker.Static(ExactPosition, map, FleckDefOf.ExplosionFlash, radius * 4f);
            for (int i = 0; i < 4; i++)
            {
                FleckMaker.ThrowSmoke(Position.ToVector3Shifted() + Gen.RandomHorizontalVector(radius * 0.7f), map, radius * 0.6f);
            }

            Rand.PushState();
            if (Rand.Chance(ignitionChance))
                foreach (var vec3 in cellsToAffect)
                {
                    var fireSize = radius - vec3.DistanceTo(Position);
                    if (fireSize > 0.1f)
                    {
                        if (this.def.projectile.damageDef == AdeptusDamageDefOf.OG_Chaos_Deamon_Warpfire)
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
            GenExplosion.DoExplosion(this.Position, map, this.def.projectile.explosionRadius, this.def.projectile.damageDef, this.launcher, this.def.projectile.GetDamageAmount(1, null), this.def.projectile.GetArmorPenetration(1, null), this.def.projectile.soundExplode, this.equipmentDef, this.def, null, this.def.projectile.postExplosionSpawnThingDef, this.def.projectile.postExplosionSpawnChance, this.def.projectile.postExplosionSpawnThingCount, null, this.def.projectile.applyDamageToExplosionCellsNeighbors, this.def.projectile.preExplosionSpawnThingDef, this.def.projectile.preExplosionSpawnChance, this.def.projectile.preExplosionSpawnThingCount, this.def.projectile.explosionChanceToStartFire, this.def.projectile.explosionDamageFalloff);
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

            if (this.def.projectile.damageDef == AdeptusDamageDefOf.OG_Chaos_Deamon_Warpfire)
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
                    if (this.def.projectile.damageDef == AdeptusDamageDefOf.OG_Chaos_Deamon_Warpfire)
                    {
                        spark = (Spark)GenSpawn.Spawn(AdeptusThingDefOf.OG_WarpSpark, base.Position, base.Map, WipeMode.Vanish);
                    }
                    else
                    {
                        spark = (Spark)GenSpawn.Spawn(ThingDefOf.Spark, base.Position, base.Map, WipeMode.Vanish);
                    }
                    spark.Launch(this, intVec, intVec, ProjectileHitFlags.All, false, null);
                }
                else
                {
                    if (this.def.projectile.damageDef == AdeptusDamageDefOf.OG_Chaos_Deamon_Warpfire)
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

        public override Quaternion ExactRotation
        {
            get
            {
                var forward = destination - origin;
                forward.y = 0;
                return Quaternion.LookRotation(forward);
            }
        }

        public override void Draw()
        {
            string mote = "OG_Mote_FlameGlow";
            if (this.def.projectile.damageDef == AdeptusDamageDefOf.OG_Chaos_Deamon_Warpfire)
            {
                mote = "OG_Mote_WarpFireGlow";
            }
            ThingDef moteDef = DefDatabase<ThingDef>.GetNamed(mote);
            Graphic glow = moteDef.graphic;
            Mesh mesh = MeshPool.GridPlane(this.def.graphicData.drawSize * traveled);
            Mesh mesh2 = MeshPool.GridPlane(moteDef.graphicData.drawSize * (traveled * 7));
            Graphics.DrawMesh(mesh, this.DrawPos, this.ExactRotation, Graphic.MatSingle, 0);
            Graphics.DrawMesh(mesh2, this.DrawPos, this.ExactRotation, glow.MatSingle, 0);
            /*
			Rand.PushState();
			Rand.Seed = this.thingIDNumber;
			for (int i = 0; i < 180; i++)
			{
                

                this.DrawPart(Rand.Range(0f, distancetraveled), Rand.Range(0f, 9f), Rand.Range(0.9f, 1.1f), Rand.Range(0.52f, 0.88f));
			}
			Rand.PopState();
			*/
            base.Comps_PostDraw();
        }

        
		private void DrawPart(float distanceFromCenter, float initialAngle, float speedMultiplier, float colorMultiplier)
		{
			int ticksGame = Find.TickManager.TicksGame;
			float num = 1f / distanceFromCenter;
			float num2 = 25f * speedMultiplier * num;
			float num3 = (initialAngle + (float)ticksGame * num2) % 360f;
			Vector2 vector = this.realPosition.Moved(num3, this.AdjustedDistanceFromCenter(distanceFromCenter));

            //	vector.y += distanceFromCenter * 4f;
            //	vector.y += Projectile_Fire.ZOffsetBias;
            Rand.PushState();
            Vector3 a = new Vector3(vector.x, AltitudeLayer.Weather.AltitudeFor() + 0.042857144f * Rand.Range(0f, 1f), vector.y);
            Rand.PopState();
            float num4 = distanceFromCenter / 3f;
			float num5 = 1f;
			
			if (num3 > 270f)
			{
				num5 = GenMath.LerpDouble(270f, 360f, 0f, 1f, num3);
			}
			else if (num3 > 180f)
			{
				num5 = GenMath.LerpDouble(180f, 270f, 1f, 0f, num3);
			}
			
			float num6 = Mathf.Min(distanceFromCenter / (Projectile_Fire.PartsDistanceFromCenter.max + 2f), 1f);
			float d = Mathf.InverseLerp(0.18f, 0.4f, num6);
			Vector3 a2 = new Vector3(Mathf.Sin((float)ticksGame / 1000f + (float)(this.thingIDNumber * 10)) * 2f, 0f, 0f);
			Vector3 pos = a + a2 * d;
			float a3 = Mathf.Max(1f - num6, 0f) * num5 * this.FadeInOutFactor;
			Color value = new Color(this.Graphic.color.r, this.Graphic.color.g, this.Graphic.color.b, a3);
            Projectile_Fire.matPropertyBlock.SetColor(ShaderPropertyIDs.Color, value);
			Matrix4x4 matrix = Matrix4x4.TRS(pos, ExactRotation, new Vector3(num4, 1f, num4));
			Graphics.DrawMesh(MeshPool.plane10, matrix, Graphic.MatSingle, 0, null, 0, Projectile_Fire.matPropertyBlock);
		}

        private float FadeInOutFactor
        {
            get
            {
                float a = Mathf.Clamp01((float)(Find.TickManager.TicksGame - this.spawnTick) / 120f);
                float b = (this.leftFadeOutTicks < 0) ? 1f : Mathf.Min((float)this.leftFadeOutTicks / 120f, 1f);
                return Mathf.Min(a, b);
            }
        }
        private float AdjustedDistanceFromCenter(float distanceFromCenter)
        {
            float num = Mathf.Min(distanceFromCenter / 4f, 1f);
        //    num *= num;
            return distanceFromCenter * num;
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                this.realPosition = new Vector2(base.ExactPosition.x, base.ExactPosition.z);
                this.spawnTick = Find.TickManager.TicksGame;
                this.leftFadeOutTicks = -1;
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<Vector2>(ref this.realPosition, "realPosition", default(Vector2), false);
            Scribe_Values.Look<int>(ref this.spawnTick, "spawnTick", 0, false);
            Scribe_Values.Look<int>(ref this.leftFadeOutTicks, "leftFadeOutTicks", 0, false);
            Scribe_Values.Look<int>(ref this.ticksLeftToDisappear, "ticksLeftToDisappear", 0, false);
            Scribe_Values.Look(ref distancetotravel, "distancetotravel");
            Scribe_Values.Look(ref distancetraveled, "distancetraveled");
            Scribe_Values.Look(ref TicksforAppearence, "TicksforAppearence");
            Scribe_Values.Look(ref age, "age");
        }
        private static readonly FloatRange PartsDistanceFromCenter = new FloatRange(1f, 10f);
        private static readonly float ZOffsetBias = -4f * Projectile_Fire.PartsDistanceFromCenter.min;
        private int spawnTick;
        private int leftFadeOutTicks = -1;
        private int ticksLeftToDisappear = -1;
        private const int FadeInTicks = 120;
        private const int FadeOutTicks = 120;
        private const float MaxMidOffset = 2f;
        private static MaterialPropertyBlock matPropertyBlock = new MaterialPropertyBlock();
        private static readonly Material TornadoMaterial = MaterialPool.MatFrom("Things/Ethereal/Tornado", ShaderDatabase.Transparent, MapMaterialRenderQueues.Tornado);
        private Vector2 realPosition;
        private float distancetotravel = 0;
        private float distancetraveled = 0;
        private float traveled = 0;
        private int TicksforAppearence = 15;
        private int age = 0;
        private IntVec3 pos = IntVec3.Invalid;
    }
}
