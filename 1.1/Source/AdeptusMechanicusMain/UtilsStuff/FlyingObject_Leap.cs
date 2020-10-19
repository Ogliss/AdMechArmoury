using System;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x020000DD RID: 221
    [StaticConstructorOnStartup]
    public class FlyingObject_Leap : ThingWithComps
    {
        // Token: 0x1700010A RID: 266
        // (get) Token: 0x060005F2 RID: 1522 RVA: 0x00056D60 File Offset: 0x00054F60
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
                return this.ExactPosition;
            }
        }

        // Token: 0x060005F7 RID: 1527 RVA: 0x00056E74 File Offset: 0x00055074
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
        }

        // Token: 0x060005F8 RID: 1528 RVA: 0x00056F34 File Offset: 0x00055134
        private void Initialize()
        {
            bool flag = this.pawn != null;
            if (flag)
            {
                Rand.PushState();
                MoteMaker.ThrowDustPuff(this.pawn.Position, this.pawn.Map, Rand.Range(1.2f, 1.8f));
                Rand.PopState();
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
            bool spawned = flyingThing.Spawned;
            this.pawn = (launcher as Pawn);
            bool flag = spawned;
            if (flag)
            {
                flyingThing.DeSpawn(DestroyMode.Vanish);
            }
            this.origin = origin;
            this.impactDamage = newDamageInfo;
            this.flyingThing = flyingThing;
            bool flag2 = targ.Thing != null;
            bool flag3 = flag2;
            if (flag3)
            {
                this.assignedTarget = targ.Thing;
            }
            this.destination = targ.Cell.ToVector3Shifted();
            this.ticksToImpact = this.StartingTicksToImpact;
            this.Initialize();
        }

        // Token: 0x060005FC RID: 1532 RVA: 0x00057070 File Offset: 0x00055270
        public override void Tick()
        {
            base.Tick();
            Vector3 exactPosition = this.ExactPosition;
            this.ticksToImpact--;
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
                    Rand.PushState();
                    MoteMaker.ThrowDustPuff(base.Position, base.Map, Rand.Range(0.6f, 0.8f));
                    Rand.PopState();
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
                    Vector3 drawPos = this.DrawPos;
                    bool flag5 = !this.DrawPos.ToIntVec3().IsValid;
                    bool flag6 = flag5;
                    if (flag6)
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
            }
            else
            {
                Graphics.DrawMesh(MeshPool.plane10, this.DrawPos, this.ExactRotation, this.flyingThing.def.DrawMatSingle, 0);
            }
            base.Comps_PostDraw();
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
            }
            try
            {
                SoundDefOf.Ambient_AltitudeWind.sustainFadeoutTime.Equals(30f);
                GenSpawn.Spawn(this.flyingThing, base.Position, base.Map, WipeMode.Vanish);
                Pawn pawn2 = this.flyingThing as Pawn;
                this.Destroy(DestroyMode.Vanish);
            }
            catch
            {
                GenSpawn.Spawn(this.flyingThing, base.Position, base.Map, WipeMode.Vanish);
                Pawn pawn3 = this.flyingThing as Pawn;
                this.Destroy(DestroyMode.Vanish);
            }
        }

        // Token: 0x040005C1 RID: 1473
        protected Vector3 origin;

        // Token: 0x040005C2 RID: 1474
        protected Vector3 destination;

        // Token: 0x040005C3 RID: 1475
        protected float speed = 28f;

        // Token: 0x040005C4 RID: 1476

        // Token: 0x040005C5 RID: 1477
        protected int ticksToImpact;

        // Token: 0x040005C6 RID: 1478
        protected Thing assignedTarget;

        // Token: 0x040005C7 RID: 1479
        protected Thing flyingThing;

        // Token: 0x040005C8 RID: 1480
        public DamageInfo? impactDamage;

        // Token: 0x040005C9 RID: 1481
        public bool damageLaunched = true;

        // Token: 0x040005CA RID: 1482
        public bool explosion = false;

        // Token: 0x040005CB RID: 1483
        public int weaponDmg = 0;

        // Token: 0x040005CC RID: 1484

        // Token: 0x040005CD RID: 1485
        private Pawn pawn;
        
    }
}
