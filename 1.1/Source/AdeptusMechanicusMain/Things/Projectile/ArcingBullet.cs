using System;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.ArcingBullet
    [StaticConstructorOnStartup]
    public class ArcingBullet : Bullet_Explosive
    {
        private float TimeInAnimation
        {
            get
            {
                /*
                if (this.def.skyfaller.reversed)
                {
                    return (float)this.ticksToImpact / 220f;
                }
                */
                return 1f - (float)this.ticksToImpact / (float)this.StartingTicksToImpact;
            }
        }

        private float CurrentSpeed
        {
            get
            {
                if (this.def.skyfaller.speedCurve == null)
                {
                    return this.def.skyfaller.speed;
                }
                return this.SpeedArc.Evaluate(this.TimeInAnimation) * this.def.skyfaller.speed;
            }
        }
        protected new int StartingTicksToImpact
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
        protected int UpdateTicksToImpact
        {
            get
            {
                int num = Mathf.RoundToInt((this.ExactPosition - this.destination).magnitude / (this.CurrentSpeed / 100f));
                bool flag = num < 1;
                bool flag2 = flag;
                if (flag2)
                {
                    num = 1;
                }
                return num;
            }
        }

        protected new IntVec3 DestinationCell
        {
            get
            {
                return new IntVec3(this.destination);
            }
        }


        public override Vector3 ExactPosition
        {
            get
            {
                Vector3 b = (this.destination - this.origin) * (1f - (float)this.ticksToImpact / (float)this.StartingTicksToImpact);
                return this.origin + b + Vector3.up * this.def.Altitude;
            }
        }


        public override Quaternion ExactRotation
        {
            get
            {


                if (this.FlightArc != null)
                {
                    Vector3 drawPos = this.origin;
                    Vector3 drawPos2 = this.destination;
                    drawPos.z += this.FlightArc.Evaluate(this.TimeInAnimation);
                    drawPos2.z += this.FlightArc.Evaluate(this.TimeInAnimation + 0.1f);
                    return Quaternion.LookRotation(drawPos2 - drawPos);
                }

                return Quaternion.LookRotation(this.destination - this.origin);
            }
        }

        public override Vector3 DrawPos
        {
            get
            {
                Vector3 drawPos = this.ExactPosition;
                //    return SkyfallerDrawPosUtility.DrawPos_Accelerate(base.DrawPos, this.ticksToImpact, (float)Math.Atan2(origin.y - destination.y, origin.x - destination.x), this.CurrentSpeed);

                /*
                Vector3 drawPos = this.DrawPos;
                bool flag5 = !this.DrawPos.ToIntVec3().IsValid;
                bool flag6 = flag5;
                if (flag6)
                {
                    return;
                }
                Pawn pawn = this.flyingThing as Pawn;
                if (this.def.skyfaller.xPositionCurve != null)
                {
                //    Log.Message("Xpos mod " + this.def.skyfaller.xPositionCurve.Evaluate(this.TimeInAnimation) + " time " + TimeInAnimation);
                    drawPos.x += this.def.skyfaller.xPositionCurve.Evaluate(this.TimeInAnimation);
                }
                if (this.def.skyfaller.zPositionCurve != null)
                {
                //    Log.Message("Zpos mod " + this.def.skyfaller.zPositionCurve.Evaluate(this.TimeInAnimation) + " time " + TimeInAnimation);
                    drawPos.z += this.def.skyfaller.zPositionCurve.Evaluate(this.TimeInAnimation);
                }
                */
                /*
                float num = 0f;

                if (this.def.skyfaller.rotateGraphicTowardsDirection)
                {
                    num = this.angle;
                }
                if (this.def.skyfaller.angleCurve != null)
                {
                    this.angle = this.def.skyfaller.angleCurve.Evaluate(this.TimeInAnimation);
                }
                if (this.def.skyfaller.rotationCurve != null)
                {
                    num += this.def.skyfaller.rotationCurve.Evaluate(this.TimeInAnimation);
                }
                if (this.def.skyfaller.xPositionCurve != null)
                {
                    drawPos.x += this.def.skyfaller.xPositionCurve.Evaluate(this.TimeInAnimation);
                }
                if (this.def.skyfaller.zPositionCurve != null)
                {
                    //    drawPos.z += this.def.skyfaller.zPositionCurve.Evaluate(this.TimeInAnimation);
                }
                */
                if (this.FlightArc != null)
                {
                    drawPos.z += this.FlightArc.Evaluate(this.TimeInAnimation);
                }
                else
                {

                    float num = this.ArcHeightFactor * GenMath.InverseParabola(this.DistanceCoveredFraction);

                    drawPos += new Vector3(0f, 0f, 1f) * num;
                }
                return drawPos;
            //    return this.ExactPosition;
            }
        }

        private int heightinc = Rand.Range(-1, 3);
        private float ArcHeightFactor
        {
            get
            {
                float num = this.def.projectile.arcHeightFactor + heightinc;
                float num2 = (this.destination - this.origin).MagnitudeHorizontalSquared();
                if (num * num > num2 * 0.2f * 0.2f)
                {
                    num = Mathf.Sqrt(num2) * 0.2f;
                }
                return num;
            }
        }
        private Vector2 GetPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            
            float cx = 3 * (p1.x - p0.x);
            float cy = 3 * (p1.y - p0.y);
            float bx = 3 * (p2.x - p1.x) - cx;
            float by = 3 * (p2.y - p1.y) - cy;
            float ax = p3.x - p0.x - cx - bx;
            float ay = p3.y - p0.y - cy - by;
            float Cube = t * t * t;
            float Square = t * t;

            float resY = (ay * Cube) + (by * Square) + (cy * t) + p0.y;
            return new Vector2(t, resY);
        }
        private SimpleCurve flightArc = null;
        public SimpleCurve FlightArc
        {
            get
            {
                if (flightArc == null && this.def.skyfaller?.zPositionCurve != null)
                {
                //    Log.Message("generating flight arc");
                    SimpleCurve arc = new SimpleCurve();
                    for (float t = 0; t <= 1.0f; t += 0.1f)
                    {
                        Vector2 point = GetPoint(t, this.def.skyfaller.zPositionCurve.Points[0], this.def.skyfaller.zPositionCurve.Points[1], this.def.skyfaller.zPositionCurve.Points[2], this.def.skyfaller.zPositionCurve.Points[3]);

                    //    Log.Message("adding " + point);
                        arc.Add(point.x, point.y);
                    }
                    arc.Add(1, 0);
                    flightArc = arc;
                }
                return flightArc;
            }
        }
        private SimpleCurve speedArc = null;
        public SimpleCurve SpeedArc
        {
            get
            {
                if (speedArc == null)
                {
                //    Log.Message("generating speed arc");
                    SimpleCurve arc = new SimpleCurve();
                    float time = 0;
                    for (int i = 0; i < 13; i++)
                    {
                        Vector2 point = GetPoint(time, this.def.skyfaller.speedCurve.Points[0], this.def.skyfaller.speedCurve.Points[1], this.def.skyfaller.speedCurve.Points[2], this.def.skyfaller.speedCurve.Points[3]);

                    //    Log.Message("adding " + point + " to speed arc at time " + time);
                        arc.Add(time, point.y);
                        time += 0.08333333333333333333333333333333f;
                    }
                    speedArc = arc;
                }
                return speedArc;
            }
        }

        public new void Launch(Thing launcher, LocalTargetInfo usedTarget, LocalTargetInfo intendedTarget, ProjectileHitFlags hitFlags, Thing equipment = null)
        {
            this.Launch(launcher, base.Position.ToVector3Shifted(), usedTarget, intendedTarget, hitFlags, equipment, null);
        }

        public new void Launch(Thing launcher, Vector3 origin, LocalTargetInfo usedTarget, LocalTargetInfo intendedTarget, ProjectileHitFlags hitFlags, Thing equipment = null, ThingDef targetCoverDef = null)
        {
            this.launcher = launcher;
            this.origin = origin;
            this.usedTarget = usedTarget;
            this.intendedTarget = intendedTarget;
            this.targetCoverDef = targetCoverDef;
            this.HitFlags = hitFlags;
            if (equipment != null)
            {
                this.equipmentDef = equipment.def;
                this.weaponDamageMultiplier = equipment.GetStatValue(StatDefOf.RangedWeapon_DamageMultiplier, true);
            }
            else
            {
                this.equipmentDef = null;
                this.weaponDamageMultiplier = 1f;
            }
            this.destination = usedTarget.Cell.ToVector3Shifted() + Gen.RandomHorizontalVector(0.3f);
            this.ticksToImpact = Mathf.CeilToInt(this.StartingTicksToImpact);
            if (this.ticksToImpact < 1)
            {
                this.ticksToImpact = 1;
            }
            if (!this.def.projectile.soundAmbient.NullOrUndefined())
            {
                SoundInfo info = SoundInfo.InMap(this, MaintenanceType.PerTick);
            //    this.ambientSustainer = this.def.projectile.soundAmbient.TrySpawnSustainer(info);
            }
        }

        public override void Tick()
        {
            base.Tick();
            Vector3 exactPosition = this.ExactPosition;
        //    this.ticksToImpact = UpdateTicksToImpact;
            this.ticksToImpact--;
            if (this.ticksToImpact == 15)
            {
                this.HitRoof();
            }
            if (this.landed || base.Map==null)
            {
                return;
            }
            bool flag = !this.ExactPosition.InBounds(base.Map);
            bool flag2 = flag;
            if (flag2)
            {
                this.ticksToImpact++;
                base.Position = this.ExactPosition.ToIntVec3();
                this.Destroy(DestroyMode.Vanish);
            }
            else
            {
                base.Position = this.ExactPosition.ToIntVec3();
                bool flag3 = Find.TickManager.TicksGame % 2 == 0;
                if (flag3)
                {
                    if (this.FlightArc != null)
                    {
                        float f = this.FlightArc.Evaluate(this.TimeInAnimation);
                        exactPosition.z += this.FlightArc.Evaluate(this.TimeInAnimation);
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        Rand.PushState();
                        AdeptusMoteMaker.ThrowDustPuff(this.DrawPos, base.Map, Rand.Range(0.3f, 0.6f));
                        Rand.PopState();
                    }
                }
                bool flag4 = this.ticksToImpact <= 0;
                bool flag5 = flag4;
                if (flag5)
                {
                    bool flag6 = this.DestinationCell.InBounds(base.Map);
                    bool flag7 = flag6;
                    if (flag7)
                    {
                        base.Position = this.DestinationCell;
                    }
                    this.ImpactSomething();
                }
            }
        }

        public override void Draw()
        {
            bool flag = this.DefaultGraphic != null;
            bool flag2 = flag;
            if (flag2)
            {
                Graphics.DrawMesh(MeshPool.GridPlane(this.def.graphicData.drawSize), this.DrawPos, this.ExactRotation, this.Graphic.MatSingle, 0);
                this.DrawDropSpotShadow(ExactPosition);
            }
            base.Comps_PostDraw();
        }

        private void DrawDropSpotShadow(Vector3 loc)
        {
            Material shadowMaterial = this.ShadowMaterial;
            if (shadowMaterial == null)
            {
                return;
            }
            ArcingBullet.DrawDropSpotShadow(loc, base.Rotation, shadowMaterial, this.def.skyfaller.shadowSize / this.FlightArc.Evaluate(this.TimeInAnimation), this.ticksToImpact);
        }

        public static void DrawDropSpotShadow(Vector3 center, Rot4 rot, Material material, Vector2 shadowSize, int ticksToImpact)
        {
            if (rot.IsHorizontal)
            {
                Gen.Swap<float>(ref shadowSize.x, ref shadowSize.y);
            }
            ticksToImpact = Mathf.Max(ticksToImpact, 0);
            Vector3 pos = center;
            pos.y = AltitudeLayer.Shadows.AltitudeFor();
            float num = 1f + (float)ticksToImpact / 100f;
            Vector3 s = new Vector3(Math.Min(num * shadowSize.x, 1f), 1f, Math.Min(num * shadowSize.y, 1f));
            Color white = Color.white;
            if (ticksToImpact > 150)
            {
                white.a = Mathf.InverseLerp(200f, 150f, (float)ticksToImpact);
            }
            ArcingBullet.shadowPropertyBlock.SetColor(ShaderPropertyIDs.Color, white);
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(pos, rot.AsQuat, s);
            Graphics.DrawMesh(MeshPool.plane10Back, matrix, material, 0, null, 0, ArcingBullet.shadowPropertyBlock);
        }

        private void ImpactSomething()
        {
            bool flag = this.usedTarget.Thing != null;
            bool flag2 = flag;
            if (flag2)
            {
                Pawn pawn = this.usedTarget.Thing as Pawn;
                Rand.PushState();
                bool flag3 = pawn != null && pawn.GetPosture() != PawnPosture.Standing && (this.origin - this.destination).MagnitudeHorizontalSquared() >= 20.25f && Rand.Value > 0.2f;
                Rand.PopState();
                bool flag4 = flag3;
                if (flag4)
                {
                    this.Impact(null);
                }
                else
                {
                    this.Impact(this.usedTarget.Thing);
                }
            }
            else
            {
                this.Impact(null);
            }
        }

        protected virtual void HitRoof()
        {
            if ((this.def.skyfaller != null && !this.def.skyfaller.hitRoof) || (this.def.projectile != null && !this.def.projectile.flyOverhead))
            {
                return;
            }
            CellRect cr = this.OccupiedRect();
            if (cr.Cells.Any((IntVec3 x) => x.Roofed(this.Map)))
            {
                RoofDef roof = cr.Cells.First((IntVec3 x) => x.Roofed(this.Map)).GetRoof(base.Map);
                if (!roof.soundPunchThrough.NullOrUndefined())
                {
                    roof.soundPunchThrough.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
                }
                RoofCollapserImmediate.DropRoofInCells(cr.ExpandedBy(1).ClipInsideMap(base.Map).Cells.Where(delegate (IntVec3 c)
                {
                    if (!c.InBounds(this.Map))
                    {
                        return false;
                    }
                    if (cr.Contains(c))
                    {
                        return true;
                    }
                    if (c.GetFirstPawn(this.Map) != null)
                    {
                        return false;
                    }
                    Building edifice = c.GetEdifice(this.Map);
                    return edifice == null || !edifice.def.holdsRoof;
                }), base.Map, null);
            }
        }

        private Material ShadowMaterial
        {
            get
            {
                if (this.cachedShadowMaterial == null)
                {
                    if (this.def.skyfaller != null && !this.def.skyfaller.shadow.NullOrEmpty())
                    {
                        this.cachedShadowMaterial = MaterialPool.MatFrom(this.def.skyfaller.shadow, ShaderDatabase.Transparent);
                    }
                }
                return this.cachedShadowMaterial;
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<Vector3>(ref this.origin, "origin", default(Vector3), false);
            Scribe_Values.Look<Vector3>(ref this.destination, "destination", default(Vector3), false);
            Scribe_Values.Look<int>(ref this.ticksToImpact, "ticksToImpact", 0, false);
        }

        private Material cachedShadowMaterial;
        protected float speed => this.def.projectile.speed;



        private static MaterialPropertyBlock shadowPropertyBlock = new MaterialPropertyBlock();

    }
}
