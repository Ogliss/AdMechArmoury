using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x02000763 RID: 1891
    public class CompProperties_SpawnerLike : CompProperties
    {
        // Token: 0x060029C3 RID: 10691 RVA: 0x0013C6C7 File Offset: 0x0013AAC7
        public CompProperties_SpawnerLike()
        {
            this.compClass = typeof(CompSpawnerLike);
        }

        // Token: 0x0400172B RID: 5931
        public ThingDef thingToSpawn;

        // Token: 0x0400172C RID: 5932
        public int spawnCount = 1;

        // Token: 0x0400172D RID: 5933
        public IntRange spawnIntervalRange = new IntRange(100, 100);

        // Token: 0x0400172E RID: 5934
        public int spawnMaxAdjacent = -1;

        // Token: 0x0400172F RID: 5935
        public bool spawnForbidden;

        // Token: 0x04001730 RID: 5936
        public bool requiresPower;

        // Token: 0x04001731 RID: 5937
        public bool writeTimeLeftToSpawn;

        // Token: 0x04001732 RID: 5938
        public bool showMessageIfOwned;

        // Token: 0x04001733 RID: 5939
        public string saveKeysPrefix;

        // Token: 0x04001734 RID: 5940
        public bool inheritFaction;
    }
    // Token: 0x02000764 RID: 1892
    public class CompSpawnerLike : ThingComp
    {
        // Token: 0x17000675 RID: 1653
        // (get) Token: 0x060029C5 RID: 10693 RVA: 0x0013C704 File Offset: 0x0013AB04
        public CompProperties_SpawnerLike PropsSpawner
        {
            get
            {
                return (CompProperties_SpawnerLike)this.props;
            }
        }

        // Token: 0x17000676 RID: 1654
        // (get) Token: 0x060029C6 RID: 10694 RVA: 0x0013C714 File Offset: 0x0013AB14
        private bool PowerOn
        {
            get
            {
                CompPowerTrader comp = this.parent.GetComp<CompPowerTrader>();
                return comp != null && comp.PowerOn;
            }
        }

        // Token: 0x060029C7 RID: 10695 RVA: 0x0013C73C File Offset: 0x0013AB3C
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if (!respawningAfterLoad)
            {
                this.ResetCountdown();
            }
        }

        // Token: 0x060029C8 RID: 10696 RVA: 0x0013C74A File Offset: 0x0013AB4A
        public override void CompTick()
        {
            this.TickInterval(1);
        }

        // Token: 0x060029C9 RID: 10697 RVA: 0x0013C753 File Offset: 0x0013AB53
        public override void CompTickRare()
        {
            this.TickInterval(250);
        }

        // Token: 0x060029CA RID: 10698 RVA: 0x0013C760 File Offset: 0x0013AB60
        private void TickInterval(int interval)
        {
            if (!this.parent.Spawned)
            {
                return;
            }
            HiveLike hive = this.parent as HiveLike;
            if (hive != null)
            {
                if (!hive.active)
                {
                    return;
                }
            }
            else if (this.parent.Position.Fogged(this.parent.Map))
            {
                return;
            }
            if (this.PropsSpawner.requiresPower && !this.PowerOn)
            {
                return;
            }
            this.ticksUntilSpawn -= interval;
            this.CheckShouldSpawn();
        }

        // Token: 0x060029CB RID: 10699 RVA: 0x0013C7F2 File Offset: 0x0013ABF2
        private void CheckShouldSpawn()
        {
            if (this.ticksUntilSpawn <= 0)
            {
                this.TryDoSpawn();
                this.ResetCountdown();
            }
        }

        // Token: 0x060029CC RID: 10700 RVA: 0x0013C810 File Offset: 0x0013AC10
        public bool TryDoSpawn()
        {
            if (!this.parent.Spawned)
            {
                return false;
            }
            if (this.PropsSpawner.spawnMaxAdjacent >= 0)
            {
                int num = 0;
                for (int i = 0; i < 9; i++)
                {
                    IntVec3 c = this.parent.Position + GenAdj.AdjacentCellsAndInside[i];
                    if (c.InBounds(this.parent.Map))
                    {
                        List<Thing> thingList = c.GetThingList(this.parent.Map);
                        for (int j = 0; j < thingList.Count; j++)
                        {
                            if (thingList[j].def == this.PropsSpawner.thingToSpawn)
                            {
                                num += thingList[j].stackCount;
                                if (num >= this.PropsSpawner.spawnMaxAdjacent)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            IntVec3 center;
            if (this.TryFindSpawnCell(out center))
            {
                Thing thing = ThingMaker.MakeThing(this.PropsSpawner.thingToSpawn, null);
                thing.stackCount = this.PropsSpawner.spawnCount;
                if (this.PropsSpawner.inheritFaction && thing.Faction != this.parent.Faction)
                {
                    thing.SetFaction(this.parent.Faction, null);
                }
                Thing t;
                GenPlace.TryPlaceThing(thing, center, this.parent.Map, ThingPlaceMode.Direct, out t, null, null);
                if (this.PropsSpawner.spawnForbidden)
                {
                    t.SetForbidden(true, true);
                }
                if (this.PropsSpawner.showMessageIfOwned && this.parent.Faction == Faction.OfPlayer)
                {
                    Messages.Message("MessageCompSpawnerSpawnedItem".Translate(this.PropsSpawner.thingToSpawn.LabelCap).CapitalizeFirst(), thing, MessageTypeDefOf.PositiveEvent, true);
                }
                return true;
            }
            return false;
        }

        // Token: 0x060029CD RID: 10701 RVA: 0x0013C9F8 File Offset: 0x0013ADF8
        private bool TryFindSpawnCell(out IntVec3 result)
        {
            foreach (IntVec3 intVec in GenAdj.CellsAdjacent8Way(this.parent).InRandomOrder(null))
            {
                if (intVec.Walkable(this.parent.Map))
                {
                    Building edifice = intVec.GetEdifice(this.parent.Map);
                    if (edifice == null || !this.PropsSpawner.thingToSpawn.IsEdifice())
                    {
                        Building_Door building_Door = edifice as Building_Door;
                        if (building_Door == null || building_Door.FreePassage)
                        {
                            if (this.parent.def.passability == Traversability.Impassable || GenSight.LineOfSight(this.parent.Position, intVec, this.parent.Map, false, null, 0, 0))
                            {
                                bool flag = false;
                                List<Thing> thingList = intVec.GetThingList(this.parent.Map);
                                for (int i = 0; i < thingList.Count; i++)
                                {
                                    Thing thing = thingList[i];
                                    if (thing.def.category == ThingCategory.Item && (thing.def != this.PropsSpawner.thingToSpawn || thing.stackCount > this.PropsSpawner.thingToSpawn.stackLimit - this.PropsSpawner.spawnCount))
                                    {
                                        flag = true;
                                        break;
                                    }
                                }
                                if (!flag)
                                {
                                    result = intVec;
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            result = IntVec3.Invalid;
            return false;
        }

        // Token: 0x060029CE RID: 10702 RVA: 0x0013CBCC File Offset: 0x0013AFCC
        private void ResetCountdown()
        {
            this.ticksUntilSpawn = this.PropsSpawner.spawnIntervalRange.RandomInRange;
        }

        // Token: 0x060029CF RID: 10703 RVA: 0x0013CBE4 File Offset: 0x0013AFE4
        public override void PostExposeData()
        {
            string str = (!this.PropsSpawner.saveKeysPrefix.NullOrEmpty()) ? (this.PropsSpawner.saveKeysPrefix + "_") : null;
            Scribe_Values.Look<int>(ref this.ticksUntilSpawn, str + "ticksUntilSpawn", 0, false);
        }

        // Token: 0x060029D0 RID: 10704 RVA: 0x0013CC3C File Offset: 0x0013B03C
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (Prefs.DevMode)
            {
                yield return new Command_Action
                {
                    defaultLabel = "DEBUG: Spawn " + this.PropsSpawner.thingToSpawn.label,
                    icon = TexCommand.DesirePower,
                    action = delegate ()
                    {
                        this.TryDoSpawn();
                        this.ResetCountdown();
                    }
                };
            }
            yield break;
        }

        // Token: 0x060029D1 RID: 10705 RVA: 0x0013CC60 File Offset: 0x0013B060
        public override string CompInspectStringExtra()
        {
            if (this.PropsSpawner.writeTimeLeftToSpawn && (!this.PropsSpawner.requiresPower || this.PowerOn))
            {
                return "NextSpawnedItemIn".Translate(GenLabel.ThingLabel(this.PropsSpawner.thingToSpawn, null, this.PropsSpawner.spawnCount)) + ": " + this.ticksUntilSpawn.ToStringTicksToPeriod();
            }
            return null;
        }

        // Token: 0x04001735 RID: 5941
        private int ticksUntilSpawn;
    }
}
