using System;
using System.Collections.Generic;
using System.Linq;
using AdeptusMechanicus.ExtensionMethods;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace AdeptusMechanicus
{
    public class MapComponent_Reserves : MapComponent
    {
        public MapComponent_Reserves(Map map) : base(map)
        {
            this.map = map;
            this.reserves = new List<ReserveForce>();

        }
        public ReserveForce nextReserve => reserves.OrderBy(x=> x.delay).FirstOrFallback(x=> !x.defeated && !x.deployed);
        public override void MapComponentTick()
        {
            //    Log.Message($"Reserve Comp Tick");
            base.MapComponentTick();
            if (Reserves.Count > 0)
            {
                //    Log.Message($"Reserve Comp Tick Reserves {Reserves.Count}");
                foreach (ReserveForce reserve in Reserves)
                {
                    if (!reserve.defeated)
                    {
                    //    Log.Message($"DeepStrike Comp Tick TICKING {reserve.Label}");
                        reserve.Tick();
                    //    Log.Message($"DeepStrike Comp Tick POST: {reserve.Label}");
                    }
                }
                Reserves.RemoveAll(x => x.defeated);
                /*
                if (this.nextReserve != null)
                {
                    if (this.reservesMonitor == null && Find.CurrentMap == this.map)
                    {
                        this.reservesMonitor = new Window_Reserves(this, this.monitorDraggable, this.monitorPos);
                        Find.WindowStack.Add(this.reservesMonitor);
                        this.reservesMonitor.UpdateHeight();
                        this.reservesMonitor.WaveTip();
                    }
                    else
                    {
                        if (this.reservesMonitor != null && Find.CurrentMap != this.map)
                        {
                            this.RemoveMonitor();
                        }
                    }
                }
                */
            }
        }

        public void AddDeepStrike(ReserveForce strike,int delay)
        {
            if (!this.Reserves.Contains(strike))
            {
                Reserves.Add(strike);
            }
        }

        public void Notify_PawnLostViolently(Lord lord, DamageInfo? dinfo = null)
        {
            if (Reserves.Where(x=> x.faction == lord.faction && x.map == lord.Map) is IEnumerable<ReserveForce> forces)
            {
                foreach (var item in forces)
                {
                    Rand.PushState();
                    if (Rand.Chance(0.45f))
                    {
                        item.delay = 1;
                        if (dinfo?.Instigator != null)
                        {
                            item.target = dinfo?.Instigator.Position ?? IntVec3.Invalid;
                        }
                        break;
                    }
                    Rand.PopState();
                }
            }
        }
        public void Notify_PawnDamaged(Lord lord, DamageInfo dinfo)
        {
            if (Reserves.Where(x=> x.faction == lord.faction) is IEnumerable<ReserveForce> forces)
            {
                foreach (var item in forces)
                {
                    Rand.PushState();
                    if (Rand.Chance(0.1f))
                    {
                        item.delay = 1;
                        if (dinfo.Instigator != null)
                        {
                            item.target = dinfo.Instigator.Position;
                        }
                        break;
                    }
                    Rand.PopState();
                }
            }
        }

        internal bool monitorDraggable = true;

        internal Vector2 monitorPos;
        /*
        internal void RemoveMonitor()
        {
            this.monitorPos = new Vector2((float)UI.screenWidth - this.reservesMonitor.windowRect.xMax, this.reservesMonitor.windowRect.y);
            this.monitorDraggable = this.reservesMonitor.draggable;
            this.reservesMonitor.Close(true);
            this.reservesMonitor = null;
        }
        */
        public override void ExposeData()
        {

            base.ExposeData();
            Scribe_Collections.Look(ref this.reserves, "StrikesGroups", LookMode.Deep);
        }

        public List<ReserveForce> Reserves => reserves;
        private List<ReserveForce> reserves;
    //    public Window_Reserves reservesMonitor;
        private readonly IntRange ResultSpawnDelay = new IntRange(500, 1000);
    }


}
