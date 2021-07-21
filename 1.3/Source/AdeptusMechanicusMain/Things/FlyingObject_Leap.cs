using System;
using System.Collections.Generic;
using System.Linq;
using AdeptusMechanicus.settings;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace AdeptusMechanicus
{
    [StaticConstructorOnStartup]
    public class FlyingObject_Leap : ThingWithComps
    {
        private float ArcHeightFactor
        {
            get
            {
                float num = this.def.projectile.arcHeightFactor;// + heightinc;
                float num2 = (this.destination - this.origin).MagnitudeHorizontalSquared();
                if (num * num > num2 * 0.2f * 0.2f)
                {
                    num = Mathf.Sqrt(num2) * 0.2f;
                }
                return num;
            }
        }
        protected int StartingTicksToImpact
        {
            get
            {
                int num = Mathf.RoundToInt((this.origin - this.destination).magnitude / (this.speed / 100f));
                bool flag = num < 1;
                bool flag2 = flag;
                if (flag2)
                {
                    num = 1;
                }
                return num;
            }
        }

        protected IntVec3 DestinationCell
        {
            get
            {
                return new IntVec3(this.destination);
            }
        }

        public virtual Vector3 ExactPosition
        {
            get
            {
                Vector3 b = (this.destination - this.origin) * (1f - (float)this.ticksToImpact / (float)this.StartingTicksToImpact);
                return this.origin + b + Vector3.up * this.def.Altitude;
            }
        }

        public virtual Quaternion ExactRotation
        {
            get
            {
                return Quaternion.LookRotation(this.destination - this.origin);
            }
        }

        public override Vector3 DrawPos
        {
            get
            {
                float num = this.ArcHeightFactor * GenMath.InverseParabola(this.DistanceCoveredFraction);
                return this.ExactPosition + new Vector3(0f, 0f, 1f) * num;
            }
        }

        protected float DistanceCoveredFraction
        {
            get
            {
                return Mathf.Clamp01(1f - (float)this.ticksToImpact / this.StartingTicksToImpact);
            }
        }
        private List<TrailerProjectileExtension> _trailers;
        public List<TrailerProjectileExtension> Trailers
        {
            get
            {
                if (_trailers == null)
                {
                    _trailers = new List<TrailerProjectileExtension>();
                    if (this.def.HasModExtension<TrailerProjectileExtension>())
                    {
                        for (int i = 0; i < def.modExtensions.Count; i++)
                        {
                            if (def.modExtensions[i] is TrailerProjectileExtension trailer)
                            {
                                _trailers.Add(trailer);
                            }
                        }
                    }
                }
                return _trailers;
            }
        }


        private List<GlowerProjectileExtension> _glower;
        public List<GlowerProjectileExtension> Glower
        {
            get
            {
                if (_glower == null)
                {
                    _glower = new List<GlowerProjectileExtension>();
                    if (this.def.HasModExtension<GlowerProjectileExtension>())
                    {
                        for (int i = 0; i < def.modExtensions.Count; i++)
                        {
                            if (def.modExtensions[i] is GlowerProjectileExtension trailer)
                            {
                                _glower.Add(trailer);
                            }
                        }
                    }
                }
                return _glower;
            }
        }


        private void Initialize()
        {
            if (this.pawn != null)
            {
                Rand.PushState();
                AdeptusFleckMaker.ThrowDustPuff(this.pawn.DrawPos, this.pawn.Map, Rand.Range(1.2f, 1.8f));
                Rand.PopState();
            }
        }

        public void Launch(Thing launcher, LocalTargetInfo targ, Thing flyingThing = null)
        {
            this.Launch(launcher, base.Position.ToVector3Shifted(), targ, flyingThing, null);
        }

        public void Launch(Thing launcher, Vector3 origin, LocalTargetInfo targ, Thing flyer = null, DamageInfo? newDamageInfo = null)
        {
            if (launcher is Pawn p)
            {
                this.pawn = p;
            }
            this.flyingThing = flyer ?? this.pawn;
            if (flyingThing.Spawned)
            {
                flyingThing.DeSpawn(DestroyMode.Vanish);
            }
            this.origin = origin;
            if (newDamageInfo != null)
            {
                this.impactDamage = newDamageInfo;
            }
            if (targ.Thing != null)
            {
                this.assignedTarget = targ.Thing;
            }
            this.drafted = pawn.Drafted;
            this.selected = Find.Selector.IsSelected(pawn);
            if (this.drafted || this.selected)
            {
                Find.Selector.ShelveSelected(pawn);
            }
            if (pawn.RaceProps.Humanlike)
            {
                this.jobQueue = pawn.jobs.CaptureAndClearJobQueue();
            }
            this.destination = targ.Cell.ToVector3Shifted();
            this.ticksToImpact = this.StartingTicksToImpact;
            this.Initialize();
        }

        public override void Tick()
        {
            base.Tick();
            Vector3 exactPosition = this.ExactPosition;
            this.ticksToImpact--;
            if (!this.ExactPosition.InBounds(base.Map))
            {
                this.ticksToImpact++;
                base.Position = this.ExactPosition.ToIntVec3();
                this.Destroy(DestroyMode.Vanish);
            }
            else
            {
                base.Position = this.ExactPosition.ToIntVec3();
                this.TrailTick();
                if (this.ticksToImpact <= 0)
                {
                    if (this.DestinationCell.InBounds(base.Map))
                    {
                        base.Position = this.DestinationCell;
                    }
                    this.ImpactSomething();
                }
            }
        }

        public virtual void TrailTick()
        {
            if (AMAMod.settings.AllowProjectileTrail)
            {
                if (!Trailers.NullOrEmpty())
                {
                    foreach (TrailerProjectileExtension trailer in Trailers)
                    {
                        if (ticksToImpact % trailer.trailerMoteInterval == 0 && (trailer.trailWhenLanded))
                        {
                            for (int ii = 0; ii < trailer.motesThrown; ii++)
                            {
                                //    Trail1Thrower.ThrowSmokeTrail(__instance.Position.ToVector3Shifted(), trailer.trailMoteSize, __instance.Map, trailer.trailMoteDef);

                                //    TrailThrower.ThrowSmokeTrail(__instance.DrawPos, trailer.trailMoteSize * DistanceCoveredFraction(___origin, ___destination, ___ticksToImpact, __instance.def.projectile.SpeedTilesPerTick), __instance.Map, trailer.trailMoteDef, __instance);
                                Color? DC = null;
                                if (trailer.useGraphicColor)
                                {
                                    DC = this.DrawColor;
                                }
                                else
                                if (trailer.useGraphicColorTwo)
                                {
                                    DC = this.DrawColorTwo;
                                }
                                TrailThrower.ThrowSprayTrail(this.DrawPos, this.Map, origin, destination, trailer.trailMoteDef, trailer.trailMoteSize, 240, this.def.projectile.SpeedTilesPerTick, DC);
                            }
                        }
                    }
                }
            }
        }
        public override void Draw()
        {
            float num = this.ArcHeightFactor * GenMath.InverseParabola(this.DistanceCoveredFraction);
            if (this.flyingThing != null)
            {
                if (this.flyingThing is Pawn)
                {
                    if (!this.DrawPos.ToIntVec3().IsValid)
                    {
                        return;
                    }
                    Pawn pawn = this.flyingThing as Pawn;
                    pawn.Drawer.DrawAt(this.DrawPos);
                }
                else
                {
                    Graphics.DrawMesh(MeshPool.plane10, this.DrawPos, this.ExactRotation, this.flyingThing.def.DrawMatSingle, 0);
                }
                if (this.def.projectile.shadowSize > 0f)
                {
                    this.DrawShadow(this.ExactPosition, num);
                }
            }
            else
            {
                Graphics.DrawMesh(MeshPool.plane10, this.DrawPos, this.ExactRotation, this.flyingThing.def.DrawMatSingle, 0);
            }
            drawGlow();
            base.Comps_PostDraw();
        }

        public virtual void drawGlow()
        {
            if (AMAMod.settings.AllowProjectileGlow)
            {
                if (!Glower.NullOrEmpty())
                {
                    foreach (GlowerProjectileExtension glower in Glower)
                    {
                        glower.Glow(this, this.ExactRotation);
                    }
                }
            }
        }
        private void DrawEffects(Vector3 pawnVec, Pawn flyingPawn, int magnitude)
        {
            if (!this.pawn.Dead && !this.pawn.Downed)
            {

            }
        }

        private void ImpactSomething()
        {
            if (this.assignedTarget != null)
            {
                Pawn pawn = this.assignedTarget as Pawn;
                Rand.PushState();
                bool flag = pawn != null && pawn.GetPosture() != PawnPosture.Standing && (this.origin - this.destination).MagnitudeHorizontalSquared() >= 20.25f && Rand.Value > 0.2f;
                Rand.PopState();
                if (flag)
                {
                    this.Impact(null);
                }
                else
                {
                    this.Impact(this.assignedTarget);
                }
            }
            else
            {
                this.Impact(null);
            }
        }

        protected virtual void Impact(Thing hitThing)
        {
            if (hitThing == null)
            {
                Pawn pawn;
                if ((pawn = (base.Position.GetThingList(base.Map).FirstOrDefault((Thing x) => x == this.assignedTarget) as Pawn)) != null)
                {
                    hitThing = pawn;
                }
            }
            if (this.impactDamage != null)
            {
                hitThing.TakeDamage(this.impactDamage.Value);
            }
            try
            {
                SoundDefOf.Ambient_AltitudeWind.sustainFadeoutTime.Equals(30f);
                RespawnPawn();
                this.Destroy(DestroyMode.Vanish);
            }
            catch
            {
                RespawnPawn();
                this.Destroy(DestroyMode.Vanish);
            }
        }

        protected virtual void RespawnPawn()
        {
            GenSpawn.Spawn(this.flyingThing, base.Position, base.Map, WipeMode.Vanish);
            Pawn thrownPawn = this.flyingThing as Pawn;
            if (thrownPawn.drafter != null)
            {
                thrownPawn.drafter.Drafted = this.drafted;
            }
            if (this.selected && Find.CurrentMap == thrownPawn.Map)
            {
                Find.Selector.Unshelve(thrownPawn, false, true);
            }
            if (this.jobQueue != null)
            {
                thrownPawn.jobs.RestoreCapturedJobs(this.jobQueue, true);
            }
        }

        private void DrawShadow(Vector3 drawLoc, float height)
        {
            if (FlyingObject_Leap.shadowMaterial == null)
            {
                return;
            }
            float num = this.def.projectile.shadowSize * Mathf.Lerp(1f, 0.6f, height);
            Vector3 s = new Vector3(num, 1f, num);
            Vector3 b = new Vector3(0f, -0.01f, 0f);
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(drawLoc + b, Quaternion.identity, s);
            Graphics.DrawMesh(MeshPool.plane10, matrix, FlyingObject_Leap.shadowMaterial, 0);
        }
        private static readonly Material shadowMaterial = MaterialPool.MatFrom("Things/Skyfaller/SkyfallerShadowCircle", ShaderDatabase.Transparent);

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<Vector3>(ref this.origin, "origin", default(Vector3), false);
            Scribe_Values.Look<Vector3>(ref this.destination, "destination", default(Vector3), false);
            Scribe_Values.Look<int>(ref this.ticksToImpact, "ticksToImpact", 0, false);
            Scribe_Values.Look<bool>(ref this.drafted, "drafted", false, false);
            Scribe_Values.Look<bool>(ref this.selected, "selected", false, false);
            Scribe_Values.Look<bool>(ref this.damageLaunched, "damageLaunched", true, false);
            Scribe_Values.Look<bool>(ref this.explosion, "explosion", false, false);
            Scribe_References.Look<Thing>(ref this.assignedTarget, "assignedTarget", false);
            Scribe_References.Look<Pawn>(ref this.pawn, "pawn", false);
            Scribe_Deep.Look<Thing>(ref this.flyingThing, "flyingThing", new object[0]);
            Scribe_Deep.Look<JobQueue>(ref this.jobQueue, "jobQueue", Array.Empty<object>());
        }

        private int heightinc = Rand.Range(-1, 3);
        protected Vector3 origin;
        protected Vector3 destination;
        protected float speed = 28f;
        protected int ticksToImpact;
        protected Thing assignedTarget;
        protected Thing flyingThing;
        public DamageInfo? impactDamage;
        public bool damageLaunched = true;
        public bool explosion = false;
        public bool drafted;
        public bool selected;
        public int weaponDmg = 0;
        private Pawn pawn;
        private JobQueue jobQueue;

    }
}
