using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public class CompProperties_Leaper : CompProperties
    {
        public float GetLeapChance
        {
            get
            {
                return Mathf.Clamp01(this.leapChance);
            }
        }

        public float GetExplodingLeaperChance
        {
            get
            {
                return Mathf.Clamp01(this.explodingLeaperChance);
            }
        }

        public CompProperties_Leaper()
        {
            this.compClass = typeof(CompLeaper);
        }

        public float leapRangeMax = 8f;
        public float leapRangeMin = 2f;
        public float leapChance = 0.5f;
        public int ticksBetweenLeapChance = 100;
        public bool combatLeap = false;
        public int combatCooldown = 600;
        public bool bouncingLeaper = false;
        public bool explodingLeaper = false;
        public float explodingLeaperChance = 0.2f;
        public float explodingLeaperRadius = 2f;
        public bool textMotes = true;
        public List<string> moteStrings = new List<string>();
        public string leaperDef = "FlyingObject_Leap";
    }

    public class CompLeaper : ThingComp
    {
        public CompProperties_Leaper Props
        {
            get
            {
                return (CompProperties_Leaper)this.props;
            }
        }
        public ThingDef _leaperDef;
        public ThingDef LeaperDef => _leaperDef ??= DefDatabase<ThingDef>.GetNamed(Props.leaperDef);

        private Pawn Pawn
        {
            get
            {
                Pawn pawn = this.parent as Pawn;
                if (pawn == null)
                {
                    Log.Error("CompLeaper pawn is null");
                }
                return pawn;
            }
        }

        public override void CompTick()
        {
            base.CompTick();
            if (this.Pawn.Spawned)
            {
                if (Find.TickManager.TicksGame % 10 == 0)
                {
                    if (this.Pawn.Downed && !this.Pawn.Dead)
                    {
                        Rand.PushState();
                        GenExplosion.DoExplosion(this.Pawn.Position, this.Pawn.Map, Rand.Range(this.explosionRadius * 0.5f, this.explosionRadius * 1.5f), DamageDefOf.Burn, this.Pawn, Rand.Range(6, 10), 0f, null, null, null, null, null, 0f, 1, GasType.Unused,false, null, 0f, 1, 0f, false);
                        Rand.PopState();
                        this.Pawn.Kill(null, null);
                    }
                }
                if (Find.TickManager.TicksGame % this.nextLeap == 0 && !this.Pawn.Downed && !this.Pawn.Dead && Find.TickManager.TicksGame - this.Pawn.mindState.lastAttackTargetTick > Props.combatCooldown)
                {
                    LocalTargetInfo a = null;
                    if (this.Pawn.CurJob != null && this.Pawn.CurJob.targetA != null)
                    {
                        a = this.Pawn.jobs.curJob.targetA.Thing;
                    }
                    if (a != null && a.Thing != null)
                    {
                        Thing thing = a.Thing;
                        if (thing is Pawn && thing.Spawned)
                        {
                            float lengthHorizontal = (thing.Position - this.Pawn.Position).LengthHorizontal;
                            if (lengthHorizontal <= this.Props.leapRangeMax && lengthHorizontal > this.Props.leapRangeMin)
                            {
                                Rand.PushState();
                                bool leap = Rand.Chance(this.Props.GetLeapChance);
                                Rand.PopState();
                                if (leap)
                                {
                                    if (this.CanHitTargetFrom(this.Pawn.Position, thing))
                                    {
                                        this.LeapAttack(thing);
                                    }
                                }
                                else
                                {
                                    if (this.Props.textMotes)
                                    {

                                        Rand.PushState();
                                        if (!this.Props.moteStrings.NullOrEmpty())
                                        {
                                            MoteMaker.ThrowText(this.Pawn.DrawPos, this.Pawn.Map, this.Props.moteStrings.RandomElement(), -1f);
                                        }
                                        else
                                        {
                                            if (Rand.Chance(0.5f))
                                            {
                                                MoteMaker.ThrowText(this.Pawn.DrawPos, this.Pawn.Map, "grrr", -1f);
                                            }
                                            else
                                            {
                                                MoteMaker.ThrowText(this.Pawn.DrawPos, this.Pawn.Map, "hsss", -1f);
                                            }
                                        }
                                        Rand.PopState();
                                    }
                                }
                            }
                            else
                            {
                                if (this.Props.bouncingLeaper)
                                {
                                    Faction faction = null;
                                    if (thing != null && thing.Faction != null)
                                    {
                                        faction = thing.Faction;
                                    }
                                    IEnumerable<IntVec3> enumerable = GenRadial.RadialCellsAround(this.Pawn.Position, this.Props.leapRangeMax, false);
                                    for (int i = 0; i < enumerable.Count<IntVec3>(); i++)
                                    {
                                        Pawn pawn = null;
                                        IntVec3 c = enumerable.ToArray<IntVec3>()[i];
                                        if (c.InBounds(this.Pawn.Map) && c.IsValid)
                                        {
                                            pawn = c.GetFirstPawn(this.Pawn.Map);
                                            if (pawn != null && pawn != thing && !pawn.Downed && !pawn.Dead && pawn.RaceProps != null)
                                            {
                                                if (pawn.Faction != null && pawn.Faction == faction)
                                                {
                                                    Rand.PushState();
                                                    if (Rand.Chance(1f - this.Props.leapChance))
                                                    {
                                                        i = enumerable.Count<IntVec3>();
                                                    }
                                                    else
                                                    {
                                                        pawn = null;
                                                    }
                                                    Rand.PopState();
                                                }
                                                else
                                                {
                                                    pawn = null;
                                                }
                                            }
                                            else
                                            {
                                                pawn = null;
                                            }
                                        }
                                        if (pawn != null)
                                        {
                                            if (this.CanHitTargetFrom(this.Pawn.Position, thing))
                                            {
                                                if (!pawn.Downed && !pawn.Dead)
                                                {
                                                    this.LeapAttack(pawn);
                                                }
                                                this.LeapAttack(pawn);
                                                break;
                                            }
                                        }
                                        enumerable.GetEnumerator().MoveNext();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        public void LeapAttack(LocalTargetInfo target)
        {
            if (target != null && target.Cell != default(IntVec3))
            {
                if (this.Pawn != null && this.Pawn.Position.IsValid && this.Pawn.Spawned && this.Pawn.Map != null && !this.Pawn.Downed && !this.Pawn.Dead && !target.Thing.DestroyedOrNull())
                {
                    this.Pawn.jobs.StopAll(false);
                    FlyingObject_Leap flyingObject_Leap = (FlyingObject_Leap)GenSpawn.Spawn(LeaperDef, this.Pawn.Position, this.Pawn.Map, WipeMode.Vanish);
                    flyingObject_Leap.Launch(this.Pawn, target.Cell, this.Pawn);
                }
            }
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            this.initialized = true;
            Rand.PushState();
            this.nextLeap = Mathf.RoundToInt(Rand.Range(this.Props.ticksBetweenLeapChance * 0.75f, 1.25f * this.Props.ticksBetweenLeapChance));
            this.explosionRadius = this.Props.explodingLeaperRadius * Rand.Range(0.8f, 1.25f);
            Rand.PopState();
        }

        public bool FoughtRecently(int ticks)
        {
            return Find.TickManager.TicksGame - this.Pawn.mindState.lastAttackTargetTick <= ticks;
        }
        private bool CanHitTargetFrom(IntVec3 pawn, LocalTargetInfo target)
        {
            return target.IsValid && target.CenterVector3.InBounds(this.Pawn.Map) && !target.Cell.Fogged(this.Pawn.Map) && target.Cell.Walkable(this.Pawn.Map) && this.TryFindShootLineFromTo(pawn, target, out ShootLine shootLine);
        }

        public bool TryFindShootLineFromTo(IntVec3 root, LocalTargetInfo targ, out ShootLine resultingLine)
        {
            bool result;
            if (targ.HasThing && targ.Thing.Map != this.Pawn.Map)
            {
                resultingLine = default(ShootLine);
                result = false;
            }
            else
            {
                resultingLine = new ShootLine(root, targ.Cell);
                bool flag2 = !GenSight.LineOfSightToEdges(root, targ.Cell, this.Pawn.Map, true, null);
                result = !flag2;
            }
            return result;
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            return base.CompGetGizmosExtra();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<bool>(ref this.initialized, "initialized", true, false);
        }

        private bool initialized = true;
        public float explosionRadius = 2f;
        private int nextLeap = 0;
    }
}
