using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AdeptusMechanicus.Comps
{
    public class CompPropererties_FireStarter : CompProperties
    {
        public CompPropererties_FireStarter()
        {
            this.compClass = typeof(CompFireStarter);
        }
        public float SpreadInterval = 6000f;



    }

    public class CompFireStarter : ThingComp
    {
        public CompPropererties_FireStarter Props => this.props as CompPropererties_FireStarter;
        public Pawn pawn => this.parent as Pawn;
        public override void CompTick()
        {
            base.CompTick();
            {


                //this.ticksUntilSmoke--;
                //if (this.ticksUntilSmoke <= 0)
                //{
                //    this.SpawnSmokeParticles();
                //}

                if (pawn != null)
                {
                    float fireSize = pawn.BodySize;
                    if (/*fireSize > 0.7f && */Rand.Value < fireSize * 0.01f)
                    {
                        FleckMaker.ThrowMicroSparks(this.pawn.DrawPos, this.pawn.Map);
                        this.ticksSinceSpread++;
                        if ((float)this.ticksSinceSpread >= this.Props.SpreadInterval)
                        {
                            this.TrySpread();
                            this.ticksSinceSpread = 0;
                        }
                    }
                }
                /*
                if (this.IsHashIntervalTick(150))
                {
                    this.DoComplexCalcs();
                }
                if (this.ticksSinceSpawn >= 7500)
                {
                    this.TryBurnFloor();
                }
                */
            }

        }

        protected void TrySpread()
        {
            IntVec3 intVec = pawn.Position;
            bool flag;

            Rand.PushState();
            if (Rand.Chance(0.8f))
            {
                intVec = pawn.Position + GenRadial.ManualRadialPattern[Rand.RangeInclusive(1, 8)];
                flag = true;
            }
            else
            {
                intVec = pawn.Position + GenRadial.ManualRadialPattern[Rand.RangeInclusive(10, 20)];
                flag = false;
            }
            Rand.PopState();
            if (!intVec.InBounds(pawn.Map))
            {
                return;
            }
            if (Rand.Chance(FireUtility.ChanceToStartFireIn(intVec, pawn.Map)))
            {
                if (!flag)
                {
                    CellRect startRect = CellRect.SingleCell(pawn.Position);
                    CellRect endRect = CellRect.SingleCell(intVec);
                    if (!GenSight.LineOfSight(pawn.Position, intVec, pawn.Map, startRect, endRect, null))
                    {
                        return;
                    }
                    ((Spark)GenSpawn.Spawn(ThingDefOf.Spark, pawn.Position, pawn.Map, WipeMode.Vanish)).Launch(pawn, intVec, intVec, ProjectileHitFlags.All, false, null);
                    return;
                }
                else
                {
                    FireUtility.TryStartFireIn(intVec, pawn.Map, 0.1f);
                }
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref this.ticksSinceSpread, "ticksSinceSpread", -1);
        }
        private int ticksSinceSpread;
    }
}
