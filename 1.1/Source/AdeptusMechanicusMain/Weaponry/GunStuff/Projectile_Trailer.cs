using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x02000003 RID: 3
    public class Projectile_Trailer : Projectile
    {
        // Token: 0x06000003 RID: 3 RVA: 0x000020FC File Offset: 0x000010FC
        public override void Tick()
        {
            base.Tick();
            checked
            {
                this.TicksforAppearence--;
                bool flag = this.TicksforAppearence == 0 && base.Map != null;
                if (flag)
                {
                    TrailThrower.ThrowSmokeTrail(base.Position.ToVector3Shifted(), 0.7f, base.Map, "Mote_Smoke");
                    this.TicksforAppearence = 6;
                }
            }
        }
        // Token: 0x04000001 RID: 1
        private int TicksforAppearence = 3;
    }

    // Token: 0x02000002 RID: 2
    public class TrailThrower
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00001050
        public static void ThrowSmokeTrail(Vector3 loc, float size, Map map, string DefName)
        {
            MoteCounter moteCounter = new MoteCounter();
            bool flag = !loc.ShouldSpawnMotesAt(map) || moteCounter.SaturatedLowPriority;
            if (!flag)
            {
                MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(ThingDef.Named(DefName), null);
                moteThrown.Scale = Rand.Range(2f, 3f) * size;
                moteThrown.exactPosition = loc;
                moteThrown.rotationRate = Rand.Range(-0.5f, 0.5f);
                moteThrown.SetVelocity((float)Rand.Range(30, 40), Rand.Range(0.008f, 0.012f));
                GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map, WipeMode.Vanish);
            }
        }
    }
}
