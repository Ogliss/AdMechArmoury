using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
    // Token: 0x020000DD RID: 221
    [StaticConstructorOnStartup]
    public class FlyingObject_JumpPack : ThingWithComps, IThingHolder
    {
        // Token: 0x17000E05 RID: 3589
        // (get) Token: 0x06004F1D RID: 20253 RVA: 0x001AA3A8 File Offset: 0x001A85A8
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

        // Token: 0x17000E06 RID: 3590
        // (get) Token: 0x06004F1E RID: 20254 RVA: 0x001AA3E0 File Offset: 0x001A85E0
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
        protected int UpdateTicksToImpact
        {
            get
            {
                int num = Mathf.RoundToInt((this.ExactPosition - this.destination).magnitude / (this.speed / 100f));
                bool flag = num < 1;
                bool flag2 = flag;
                if (flag2)
                {
                    num = 1;
                }
                return num;
            }
        }

        // Token: 0x1700010B RID: 267
        // (get) Token: 0x060005F3 RID: 1523 RVA: 0x00056DB0 File Offset: 0x00054FB0
        protected IntVec3 DestinationCell
        {
            get
            {
                return new IntVec3(this.destination);
            }
        }

        // Token: 0x1700010C RID: 268
        // (get) Token: 0x060005F4 RID: 1524 RVA: 0x00056DD0 File Offset: 0x00054FD0
        public virtual Vector3 ExactPosition
        {
            get
            {
                Vector3 b = (this.destination - this.origin) * (1f - (float)this.ticksToImpact / (float)this.StartingTicksToImpact);
                return this.origin + b + Vector3.up * this.def.Altitude;
            }
        }

        // Token: 0x1700010D RID: 269
        // (get) Token: 0x060005F5 RID: 1525 RVA: 0x00056E34 File Offset: 0x00055034
        public virtual Quaternion ExactRotation
        {
            get
            {
                return Quaternion.LookRotation(this.destination - this.origin);
            }
        }

        // Token: 0x1700010E RID: 270
        // (get) Token: 0x060005F6 RID: 1526 RVA: 0x00056E5C File Offset: 0x0005505C
        public override Vector3 DrawPos
        {
            get
            {
            //    return SkyfallerDrawPosUtility.DrawPos_Accelerate(base.DrawPos, this.ticksToImpact, (float)Math.Atan2(origin.y - destination.y, origin.x - destination.x), this.CurrentSpeed);
                return this.ExactPosition;
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

            float resX = (ax * Cube) + (bx * Square) + (cx * t) + p0.x;
            float resY = (ay * Cube) + (by * Square) + (cy * t) + p0.y;
            
        //    float resY = (p0.y + t * (p1.y - p0.y))  / p0.y;
            return new Vector2(resX, resY);
        }
        private SimpleCurve flightArc = null;
        public SimpleCurve FlightArc
        {
            get
            {
                if (flightArc == null)
                {
                    Log.Message("generating flight arc");
                    SimpleCurve arc = new SimpleCurve();
                    for (float t = 0; t <= 1.0f; t += 0.1f)
                    {
                        Vector2 point = GetPoint(t, this.def.skyfaller.zPositionCurve.Points[0], this.def.skyfaller.zPositionCurve.Points[1], this.def.skyfaller.zPositionCurve.Points[2], this.def.skyfaller.zPositionCurve.Points[3]);

                        Log.Message("adding " + point);
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
                    Log.Message("generating speed arc");
                    SimpleCurve arc = new SimpleCurve();
                    float time = 0;
                    for (int i = 0; i < 13; i++)
                    {
                        Vector2 point = GetPoint(time, this.def.skyfaller.speedCurve.Points[0], this.def.skyfaller.speedCurve.Points[1], this.def.skyfaller.speedCurve.Points[2], this.def.skyfaller.speedCurve.Points[3]);

                        Log.Message("adding " + point + " to speed arc at time " + time);
                        arc.Add(time, point.y);
                        time += 0.08333333333333333333333333333333f;
                    }
                    speedArc = arc;
                }
                return speedArc;
            }
        }

        private void Initialize()
        {
            bool flag = this.pawn != null;
            if (flag)
            {
                MoteMaker.ThrowDustPuff(this.pawn.Position, this.pawn.Map, Rand.Range(1.2f, 1.8f));
            }
        }

        // Token: 0x060005F9 RID: 1529 RVA: 0x00056F84 File Offset: 0x00055184
        public void Launch(Thing launcher, LocalTargetInfo targ, Thing flyingThing, DamageInfo? impactDamage)
        {
            this.Launch(launcher, base.Position.ToVector3Shifted(), targ, flyingThing, impactDamage);
        }

        // Token: 0x060005FA RID: 1530 RVA: 0x00056FAC File Offset: 0x000551AC
        public void Launch(Thing launcher, LocalTargetInfo targ, Thing flyingThing)
        {
            this.Launch(launcher, base.Position.ToVector3Shifted(), targ, flyingThing, null);
        }

        // Token: 0x060005FB RID: 1531 RVA: 0x00056FDC File Offset: 0x000551DC
        public void Launch(Thing launcher, Vector3 origin, LocalTargetInfo targ, Thing flyingThing, DamageInfo? newDamageInfo = null)
        {
            this.innerContainer = new ThingOwner<Thing>(this, false, LookMode.Deep);
            bool spawned = flyingThing.Spawned;
            this.pawn = (launcher as Pawn);
            bool flag = spawned;
            if (flag)
            {

                FieldInfo shadowgraphic = typeof(PawnRenderer).GetField("shadowGraphic", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
                //    FieldInfo c2 = typeof(Thing).GetField("graphicInt", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
                Traverse traverse = Traverse.Create(pawn.Drawer.renderer);
                ShadowData data = new ShadowData();
                data.volume = Vector3.zero;
                Graphic_Shadow graphic = new Graphic_Shadow(data);
                shadowgraphic.SetValue(pawn.Drawer.renderer, graphic);

            }
            this.origin = origin;
            this.impactDamage = newDamageInfo;
            this.flyingThing = flyingThing;
            bool flag2 = targ.Thing != null;
            bool flag3 = flag2;
            if (flag3)
            {
                Log.Message("Has Target aat launch");
                this.assignedTarget = targ.Thing;
            }
            this.destination = targ.Cell.ToVector3Shifted();
            this.ticksToImpact = this.StartingTicksToImpact;
            this.Initialize();
        }

        int delay = 5;
        public override void Tick()
        {
            base.Tick();
        //    this.innerContainer.ThingOwnerTick(true);
            Vector3 exactPosition = this.ExactPosition;
            if (assignedTarget!=null)
            {
                Log.Message("Has Target");
                if (assignedTarget.Position!=this.DestinationCell)
                {
                    Log.Message("Target moved");
                    this.destination = assignedTarget.Position.ToVector3Shifted();
                //    this.ticksToImpact = UpdateTicksToImpact;
                }
            }
            if (delay>0)
            {
                delay--;
            }
            else
            {
                this.ticksToImpact--;
                if (this.ticksToImpact == 15)
                {
                    this.HitRoof();
                }
                //    this.ticksToImpact-= (int)CurrentSpeed;
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
                        MoteMaker.ThrowDustPuff(exactPosition, base.Map, Rand.Range(0.3f, 0.6f));
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

        // Token: 0x060005FD RID: 1533 RVA: 0x00057178 File Offset: 0x00055378
        public override void Draw()
        {
            bool flag = this.flyingThing != null;
            bool flag2 = flag;
            if (flag2)
            {
                bool flag3 = this.flyingThing is Pawn;
                bool flag4 = flag3;
                if (flag4)
                {
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
                        Log.Message("Xpos mod " + this.def.skyfaller.xPositionCurve.Evaluate(this.TimeInAnimation) + " time " + TimeInAnimation);
                        drawPos.x += this.def.skyfaller.xPositionCurve.Evaluate(this.TimeInAnimation);
                    }
                    if (this.def.skyfaller.zPositionCurve != null)
                    {
                        Log.Message("Zpos mod " + this.def.skyfaller.zPositionCurve.Evaluate(this.TimeInAnimation) + " time " + TimeInAnimation);
                        drawPos.z += this.def.skyfaller.zPositionCurve.Evaluate(this.TimeInAnimation);
                    }
                    */
                    Vector3 drawPos = this.DrawPos;
                    Pawn thingForGraphic = this.flyingThing as Pawn;
                    float num = 0f;

                    /*
                    if (this.def.skyfaller.rotateGraphicTowardsDirection)
                    {
                        num = this.angle;
                    }
                    if (this.def.skyfaller.angleCurve != null)
                    {
                        this.angle = this.def.skyfaller.angleCurve.Evaluate(this.TimeInAnimation);
                    }
                    */
                    if (this.def.skyfaller.rotationCurve != null)
                    {
                        num += this.def.skyfaller.rotationCurve.Evaluate(this.TimeInAnimation);
                    }
                    if (this.def.skyfaller.xPositionCurve != null)
                    {
                        drawPos.x += this.def.skyfaller.xPositionCurve.Evaluate(this.TimeInAnimation);
                    }
                    /*
                    if (this.def.skyfaller.zPositionCurve != null)
                    {
                        drawPos.z += this.def.skyfaller.zPositionCurve.Evaluate(this.TimeInAnimation);
                    }
                    */
                    
                    if (this.FlightArc != null)
                    {
                        drawPos.z += this.FlightArc.Evaluate(this.TimeInAnimation);
                    }
                    
                    pawn.Drawer.DrawAt(drawPos);
                    this.DrawDropSpotShadow(ExactPosition);
                }
                else
                {
                    Graphics.DrawMesh(MeshPool.plane10, this.DrawPos, this.ExactRotation, this.flyingThing.def.DrawMatSingle, 0);
                }
            }
            else
            {
                Graphics.DrawMesh(MeshPool.plane10, this.DrawPos, this.ExactRotation, this.flyingThing.def.DrawMatSingle, 0);
            }
            base.Comps_PostDraw();
        }
        // Token: 0x06004F2E RID: 20270 RVA: 0x001AAD18 File Offset: 0x001A8F18
        private void DrawDropSpotShadow(Vector3 loc)
        {
            Material shadowMaterial = this.ShadowMaterial;
            if (shadowMaterial == null)
            {
                return;
            }
            Skyfaller.DrawDropSpotShadow(loc, base.Rotation, shadowMaterial, this.def.skyfaller.shadowSize / this.FlightArc.Evaluate(this.TimeInAnimation), this.ticksToImpact);
        }

        // Token: 0x06004F2F RID: 20271 RVA: 0x001AAD60 File Offset: 0x001A8F60
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
            FlyingObject_JumpPack.shadowPropertyBlock.SetColor(ShaderPropertyIDs.Color, white);
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(pos, rot.AsQuat, s);
            Graphics.DrawMesh(MeshPool.plane10Back, matrix, material, 0, null, 0, FlyingObject_JumpPack.shadowPropertyBlock);
        }
        // Token: 0x060005FE RID: 1534 RVA: 0x00057260 File Offset: 0x00055460
        private void DrawEffects(Vector3 pawnVec, Pawn flyingPawn, int magnitude)
        {
            bool flag = !this.pawn.Dead && !this.pawn.Downed;
            bool flag2 = flag;
            if (flag2)
            {
            }
        }

        // Token: 0x060005FF RID: 1535 RVA: 0x00057294 File Offset: 0x00055494
        private void ImpactSomething()
        {
            bool flag = this.assignedTarget != null;
            bool flag2 = flag;
            if (flag2)
            {
                Pawn pawn = this.assignedTarget as Pawn;
                bool flag3 = pawn != null && pawn.GetPosture() != PawnPosture.Standing && (this.origin - this.destination).MagnitudeHorizontalSquared() >= 20.25f && Rand.Value > 0.2f;
                bool flag4 = flag3;
                if (flag4)
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

        // Token: 0x06000600 RID: 1536 RVA: 0x00057328 File Offset: 0x00055528
        protected virtual void Impact(Thing hitThing)
        {
            bool flag = hitThing == null;
            bool flag2 = flag;
            if (flag2)
            {
                Pawn pawn;
                bool flag3 = (pawn = (base.Position.GetThingList(base.Map).FirstOrDefault((Thing x) => x == this.assignedTarget) as Pawn)) != null;
                bool flag4 = flag3;
                if (flag4)
                {
                    hitThing = pawn;
                }
            }
            bool flag5 = this.impactDamage != null;
            bool flag6 = flag5;
            if (flag6)
            {
                hitThing.TakeDamage(this.impactDamage.Value);
                if (this.def.projectile.explosionRadius>0 && hitThing.Faction!=flyingThing.Faction)
                {
                    GenExplosion.DoExplosion(this.Position, this.Map, this.def.projectile.explosionRadius, this.def.projectile.damageDef, this.flyingThing);
                }
            }
            try
            {
                SoundDefOf.Ambient_AltitudeWind.sustainFadeoutTime.Equals(30f);
                GenSpawn.Spawn(this.flyingThing, base.Position, base.Map, WipeMode.Vanish);
                Pawn pawn2 = this.flyingThing as Pawn;
                pawn2.jobs.StopAll(false);
                pawn2.drafter.Drafted = true;
                Find.Selector.Select(pawn2);
                FieldInfo shadowgraphic = typeof(PawnRenderer).GetField("shadowGraphic", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
                Traverse traverse = Traverse.Create(pawn.Drawer.renderer);
                shadowgraphic.SetValue(pawn.Drawer.renderer, null);
                this.Destroy(DestroyMode.Vanish);
            }
            catch
            {
                GenSpawn.Spawn(this.flyingThing, base.Position, base.Map, WipeMode.Vanish);
                Pawn pawn3 = this.flyingThing as Pawn;
                pawn3.jobs.StopAll(false);
                pawn3.drafter.Drafted = true;
                Find.Selector.Select(pawn3);
                FieldInfo shadowgraphic = typeof(PawnRenderer).GetField("shadowGraphic", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
                Traverse traverse = Traverse.Create(pawn.Drawer.renderer);
                shadowgraphic.SetValue(pawn.Drawer.renderer, null);
                this.Destroy(DestroyMode.Vanish);
            }
            if (this.flyingThing.Faction == Faction.OfPlayer)
            {
                Log.Message("Player flying");
                if (this.flyingThing.Position.Fogged(this.flyingThing.Map))
                {
                    Log.Message("Fogged Landing");
                    FloodUnfogAdjacent(this.flyingThing.Position, this.flyingThing.Map);
                }
            }
        }

        private void FloodUnfogAdjacent(IntVec3 c, Map map)
        {
            map.fogGrid.Unfog(c);
            bool flag = false;
            FloodUnfogResult floodUnfogResult = default(FloodUnfogResult);
            for (int i = 0; i < 4; i++)
            {
                IntVec3 intVec = c + GenAdj.CardinalDirections[i];
                if (intVec.InBounds(map) && intVec.Fogged(map))
                {
                    Building edifice = intVec.GetEdifice(map);
                    if (edifice == null || !edifice.def.MakeFog)
                    {
                        flag = true;
                        floodUnfogResult = FloodFillerFog.FloodUnfog(intVec, map);
                    }
                    else
                    {
                        map.fogGrid.Unfog(intVec);
                    }
                }
            }
            for (int j = 0; j < 8; j++)
            {
                IntVec3 c2 = c + GenAdj.AdjacentCells[j];
                if (c2.InBounds(map))
                {
                    Building edifice2 = c2.GetEdifice(map);
                    if (edifice2 != null && edifice2.def.MakeFog)
                    {
                        map.fogGrid.Unfog(c2);
                    }
                }
            }
            if (flag)
            {
                if (floodUnfogResult.mechanoidFound)
                {
                    Find.LetterStack.ReceiveLetter("LetterLabelAreaRevealed".Translate(), "AreaRevealedWithMechanoids".Translate(), LetterDefOf.ThreatBig, new TargetInfo(c, map, false), null, null, null, null);
                    return;
                }
                if (!floodUnfogResult.allOnScreen || floodUnfogResult.cellsUnfogged >= 600)
                {
                    Find.LetterStack.ReceiveLetter("LetterLabelAreaRevealed".Translate(), "AreaRevealed".Translate(), LetterDefOf.NeutralEvent, new TargetInfo(c, map, false), null, null, null, null);
                }
            }
        }
        protected virtual void HitRoof()
        {
            if (!this.def.skyfaller.hitRoof)
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
                if (this.cachedShadowMaterial == null && !this.def.skyfaller.shadow.NullOrEmpty())
                {
                    this.cachedShadowMaterial = MaterialPool.MatFrom(this.def.skyfaller.shadow, ShaderDatabase.Transparent);
                }
                return this.cachedShadowMaterial;
            }
        }

        // Token: 0x060024F3 RID: 9459 RVA: 0x00116CE3 File Offset: 0x001150E3
        public ThingOwner GetDirectlyHeldThings()
        {
            return this.innerContainer;
        }

        // Token: 0x060024F4 RID: 9460 RVA: 0x00116CEB File Offset: 0x001150EB
        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, this.GetDirectlyHeldThings());
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<Vector3>(ref this.origin, "origin", default(Vector3), false);
            Scribe_Values.Look<Vector3>(ref this.destination, "destination", default(Vector3), false);
            Scribe_Values.Look<int>(ref this.ticksToImpact, "ticksToImpact", 0, false);
            Scribe_Values.Look<bool>(ref this.damageLaunched, "damageLaunched", true, false);
            Scribe_Values.Look<bool>(ref this.explosion, "explosion", false, false);
            Scribe_References.Look<Thing>(ref this.assignedTarget, "assignedTarget", false);
            Scribe_References.Look<Pawn>(ref this.pawn, "pawn", false);
            Scribe_Deep.Look<Thing>(ref this.flyingThing, "flyingThing", new object[0]);
            Scribe_Deep.Look<ThingOwner>(ref this.innerContainer, "innerContainer", new object[]
            {
                this
            });
        }
        public ThingOwner innerContainer;

        private Material cachedShadowMaterial;
        protected Vector3 origin;

        protected Vector3 destination;

        protected float speed => this.def.projectile.speed;

        protected int ticksToImpact;

        protected Thing assignedTarget;

        protected Thing flyingThing;

        public DamageInfo? impactDamage;

        public bool damageLaunched = true;

        public bool explosion = false;

        public int weaponDmg = 0;

        private static MaterialPropertyBlock shadowPropertyBlock = new MaterialPropertyBlock();
        private Pawn pawn;

    }
}
