using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld
{
    // Token: 0x0200031C RID: 796
    public class GameCondition_Warpstorm : GameCondition
    {
        // Token: 0x06000D8E RID: 3470 RVA: 0x00066BFC File Offset: 0x00064FFC
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<IntVec2>(ref this.centerLocation, "centerLocation", default(IntVec2), false);
            Scribe_Values.Look<int>(ref this.areaRadius, "areaRadius", 0, false);
            Scribe_Values.Look<int>(ref this.nextLightningTicks, "nextLightningTicks", 0, false);
        }

        // Token: 0x06000D8F RID: 3471 RVA: 0x00066C50 File Offset: 0x00065050
        public override void Init()
        {
            base.Init();
            this.areaRadius = GameCondition_Warpstorm.AreaRadiusRange.RandomInRange;
            this.FindGoodCenterLocation();
        }

        // Token: 0x06000D90 RID: 3472 RVA: 0x00066C7C File Offset: 0x0006507C
        public override void GameConditionTick()
        {
            if (Find.TickManager.TicksGame > this.nextLightningTicks)
            {
                Rand.PushState();
                Vector2 vector = Rand.UnitVector2 * Rand.Range(0f, (float)this.areaRadius);
                Rand.PopState();
                IntVec3 intVec = new IntVec3((int)Math.Round((double)vector.x) + this.centerLocation.x, 0, (int)Math.Round((double)vector.y) + this.centerLocation.z);
                if (this.IsGoodLocationForStrike(intVec))
                {
                    base.SingleMap.weatherManager.eventHandler.AddEvent(new WeatherEvent_WarpLightningStrike(base.SingleMap, intVec));
                    this.nextLightningTicks = Find.TickManager.TicksGame + GameCondition_Warpstorm.TicksBetweenStrikes.RandomInRange;
                }
            }
        }

        // Token: 0x06000D91 RID: 3473 RVA: 0x00066D41 File Offset: 0x00065141
        public override void End()
        {
            base.SingleMap.weatherDecider.DisableRainFor(30000);
            base.End();
        }

        // Token: 0x06000D92 RID: 3474 RVA: 0x00066D60 File Offset: 0x00065160
        private void FindGoodCenterLocation()
        {
            if (base.SingleMap.Size.x <= 16 || base.SingleMap.Size.z <= 16)
            {
                throw new Exception("Map too small for flashstorm.");
            }
            for (int i = 0; i < 10; i++)
            {
                Rand.PushState();
                this.centerLocation = new IntVec2(Rand.Range(8, base.SingleMap.Size.x - 8), Rand.Range(8, base.SingleMap.Size.z - 8));
                Rand.PopState();
                if (this.IsGoodCenterLocation(this.centerLocation))
                {
                    break;
                }
            }
        }

        // Token: 0x06000D93 RID: 3475 RVA: 0x00066E17 File Offset: 0x00065217
        private bool IsGoodLocationForStrike(IntVec3 loc)
        {
            return loc.InBounds(base.SingleMap) && !loc.Roofed(base.SingleMap) && loc.Standable(base.SingleMap);
        }

        // Token: 0x06000D94 RID: 3476 RVA: 0x00066E4C File Offset: 0x0006524C
        private bool IsGoodCenterLocation(IntVec2 loc)
        {
            int num = 0;
            int num2 = (int)(3.14159274f * (float)this.areaRadius * (float)this.areaRadius / 2f);
            foreach (IntVec3 loc2 in this.GetPotentiallyAffectedCells(loc))
            {
                if (this.IsGoodLocationForStrike(loc2))
                {
                    num++;
                }
                if (num >= num2)
                {
                    break;
                }
            }
            return num >= num2;
        }

        // Token: 0x06000D95 RID: 3477 RVA: 0x00066EE4 File Offset: 0x000652E4
        private IEnumerable<IntVec3> GetPotentiallyAffectedCells(IntVec2 center)
        {
            for (int x = center.x - this.areaRadius; x <= center.x + this.areaRadius; x++)
            {
                for (int z = center.z - this.areaRadius; z <= center.z + this.areaRadius; z++)
                {
                    if ((center.x - x) * (center.x - x) + (center.z - z) * (center.z - z) <= this.areaRadius * this.areaRadius)
                    {
                        yield return new IntVec3(x, 0, z);
                    }
                }
            }
            yield break;
        }

        // Token: 0x040008CB RID: 2251
        private static readonly IntRange AreaRadiusRange = new IntRange(45, 60);

        // Token: 0x040008CC RID: 2252
        private static readonly IntRange TicksBetweenStrikes = new IntRange(50, 400);

        // Token: 0x040008CD RID: 2253
        private const int RainDisableTicksAfterConditionEnds = 30000;

        // Token: 0x040008CE RID: 2254
        public IntVec2 centerLocation;

        // Token: 0x040008CF RID: 2255
        private int areaRadius;

        // Token: 0x040008D0 RID: 2256
        private int nextLightningTicks;
    }
}
