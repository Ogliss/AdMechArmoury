using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace AdeptusMechanicus
{
    public class ThingDef_HiveLike : ThingDef
    {
        public FactionDef Faction;
        public ThingDef TunnelDef;
        public ThingDef TunnelDefchild;
        public ThingDef HiveDefchild;
        public List<PawnKindDef> PawnKinds = new List<PawnKindDef>();
        public float maxSpawnPointsPerHive = 550f;
        public float initalSpawnPointsPerHive = 250f;
    }

    public class HiveLike : ThingWithComps
    {
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref this.active, "active", false, false);
            Scribe_Values.Look<int>(ref this.nextPawnSpawnTick, "nextPawnSpawnTick", 0, false);
            Scribe_Collections.Look<Pawn>(ref this.spawnedPawns, "spawnedPawns", LookMode.Reference, new object[0]);
            Scribe_Values.Look<bool>(ref this.caveColony, "caveColony", false, false);
            Scribe_Values.Look<bool>(ref this.canSpawnPawns, "canSpawnPawns", true, false);
            Scribe_References.Look<HiveLike>(ref this.parentHiveLike, "parentHiveLike");
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                this.spawnedPawns.RemoveAll((Pawn x) => x == null);
            }
        }

        public ThingDef_HiveLike Def
        {
            get
            {
                return this.def as ThingDef_HiveLike;
            }
        }

        public Faction OfFaction
        {
            get
            {
                return Find.FactionManager.FirstFactionOfDef(Def.Faction);
            }
        }

        public FactionDef OfFactionDef
        {
            get
            {
                return Def.Faction;
            }
        }

        public ThingDef OfTunnel
        {
            get
            {
                return Def.TunnelDef;
            }
        }

        public List<PawnKindDef> OfPawnKinds
        {
            get
            {
                if (Def.PawnKinds.Count > 0)
                {
                    PawnKinds = Def.PawnKinds;
                }
                else
                {
                    var list = (from def in DefDatabase<PawnKindDef>.AllDefs
                                where ((def.defaultFactionType == OfFaction.def && def.defaultFactionType != null) || (OfFaction.def.pawnGroupMakers.Any(pgm => pgm.options.Any(opt => opt.kind == def) && pgm.kindDef != PawnGroupKindDefOf.Trader && pgm.kindDef != PawnGroupKindDefOf.Peaceful))) && def.isFighter
                                select def).ToList();
                    if (list.Count > 0)
                    {
                        PawnKinds = list;
                    }
                }
                return PawnKinds;
            }
        }
        
        public float MaxSpawnedPawnsPoints
        {
            get
            {
                return Def.maxSpawnPointsPerHive + (Def.maxSpawnPointsPerHive * childHiveLikesCount);
            }
            set
            {
                return;
            }
        }
        
        public float InitialPawnsPoints
        {
            get
            {
                return Def.initalSpawnPointsPerHive + (Def.initalSpawnPointsPerHive * childHiveLikesCount);
            }
        }

        public List<HiveLike> childHiveLikes
        {
            get
            {
                List<HiveLike> blist = new List<HiveLike>();
                foreach (var item in HiveLikeUtility.SpawnedHivelikes(Map, this.OfFactionDef))
                {
                    blist.Add((HiveLike)item);
                }
                return blist.FindAll(x => x.parentHiveLike == this);
            }
        }

        public int childHiveLikesCount
        {
            get
            {
                return childHiveLikes.Count;
            }
        }

        public Lord Lord
        {
            get
            {
                if (parentHiveLike != null)
                {
                    if (parentHiveLike.Lord != null)
                    {
                        return parentHiveLike.Lord;
                    }
                }
                Predicate<Pawn> hasDefendHiveLord = delegate (Pawn x)
                {
                    Lord lord = x.GetLord();
                    return lord != null && lord.LordJob is LordJob_DefendAndExpandHiveLike;
                };
                Pawn foundPawn = this.spawnedPawns.Find(hasDefendHiveLord);
                if (base.Spawned)
                {
                    if (foundPawn == null)
                    {
                        RegionTraverser.BreadthFirstTraverse(this.GetRegion(RegionType.Set_Passable), (Region from, Region to) => true, delegate (Region r)
                        {
                            List<Thing> list = r.ListerThings.ThingsOfDef(Def.TunnelDef);
                            for (int i = 0; i < list.Count; i++)
                            {
                                if (list[i] != this)
                                {
                                    if (list[i].Faction == this.Faction)
                                    {
                                        foundPawn = ((HiveLike)list[i]).spawnedPawns.Find(hasDefendHiveLord);
                                        if (foundPawn != null)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                            return false;
                        }, 20, RegionType.Set_Passable);
                    }
                    if (foundPawn != null)
                    {
                        return foundPawn.GetLord();
                    }
                }
                return null;
            }
        }
        
        public float SpawnedPawnsPoints
        {
            get
            {
                this.FilterOutUnspawnedPawns();
                float num = 0f;
                for (int i = 0; i < this.spawnedPawns.Count; i++)
                {
                    num += this.spawnedPawns[i].kindDef.combatPower;
                }
                return num;
            }
        }
        
        public void ResetStaticData()
        {
            spawnablePawnKinds.Clear();
            if (OfPawnKinds.Count > 0)
            {
                spawnablePawnKinds = OfPawnKinds;
            }
            else
            {
                if (OfFactionDef.basicMemberKind != null)
                {
                    spawnablePawnKinds.Add(OfFactionDef.basicMemberKind);
                }
                else
                {

                }
            }
        }
        
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {

            base.SpawnSetup(map, respawningAfterLoad);
            if (base.Faction == null)
            {
                this.SetFaction(OfFaction, null);

            }
            if (!respawningAfterLoad && this.active)
            {
                this.SpawnInitialPawns();
            }
            else
            {
                spawnablePawnKinds = OfPawnKinds;
            }
        }
        
        public virtual void SpawnInitialPawns()
        {
            this.SpawnPawnsUntilPoints(InitialPawnsPoints);
            this.CalculateNextPawnSpawnTick();
        }

        public void SpawnPawnsUntilPoints(float points)
        {
            spawnablePawnKinds = OfPawnKinds;
            int num = 0;
            while (this.SpawnedPawnsPoints < points)
            {
                num++;
                if (num > 1000)
                {
                    Log.Error("Too many iterations.", false);
                    break;
                }
                if (!this.TrySpawnPawn(out Pawn pawn))
                {
                    break;
                }
            }
            this.CalculateNextPawnSpawnTick();
        }
        
        public override void Tick()
        {
            base.Tick();
            if (base.Spawned)
            {
                this.FilterOutUnspawnedPawns();
                if (!this.active && !base.Position.Fogged(base.Map))
                {
                    this.Activate();
                }
                if (this.active && Find.TickManager.TicksGame >= this.nextPawnSpawnTick)
                {
                    if (this.SpawnedPawnsPoints < MaxSpawnedPawnsPoints)
                    {
                        bool flag = this.TrySpawnPawn(out Pawn pawn);
                        if (flag && pawn.caller != null)
                        {
                            pawn.caller.DoCall();
                        }
                    }
                    this.CalculateNextPawnSpawnTick();
                }
            }
        }
        
        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            Map map = base.Map;
            base.DeSpawn(mode);
            List<Lord> lords = map.lordManager.lords;
            for (int i = 0; i < lords.Count; i++)
            {
                lords[i].ReceiveMemo(HiveLike.MemoDeSpawned);
            }
            HiveLikeUtility.Notify_HiveLikeDespawned(this, map);
        }
        
        public override void PostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            if (dinfo.Def.ExternalViolenceFor(this) && dinfo.Instigator != null && dinfo.Instigator.Faction != null)
            {
                Lord lord = this.Lord;
                if (lord != null)
                {
                    lord.ReceiveMemo(HiveLike.MemoAttackedByEnemy);
                }
            }
            if (dinfo.Def == DamageDefOf.Flame && (float)this.HitPoints < (float)base.MaxHitPoints * 0.3f)
            {
                Lord lord2 = this.Lord;
                if (lord2 != null)
                {
                    lord2.ReceiveMemo(HiveLike.MemoBurnedBadly);
                }
            }
            base.PostApplyDamage(dinfo, totalDamageDealt);
        }
        
        public override void Kill(DamageInfo? dinfo = null, Hediff exactCulprit = null)
        {
            if (base.Spawned && (dinfo == null || dinfo.Value.Category != DamageInfo.SourceCategory.Collapse))
            {
                List<Lord> lords = base.Map.lordManager.lords;
                for (int i = 0; i < lords.Count; i++)
                {
                    lords[i].ReceiveMemo(HiveLike.MemoDestroyedNonRoofCollapse);
                }
            }
            base.Kill(dinfo, exactCulprit);
        }
        
        public void Activate()
        {
            this.active = true;
            this.SpawnInitialPawns();
            this.CalculateNextPawnSpawnTick();
            CompSpawnerHiveLikes comp = base.GetComp<CompSpawnerHiveLikes>();
            if (comp != null)
            {
                comp.CalculateNextHiveLikeSpawnTick();
            }
        }
        
        public void CalculateNextPawnSpawnTick()
        {
            float num = GenMath.LerpDouble(0f, 5f, 1f, 0.5f, (float)this.spawnedPawns.Count);
            this.nextPawnSpawnTick = Find.TickManager.TicksGame + (int)(HiveLike.PawnSpawnIntervalDays.RandomInRange * 60000f / (num * Find.Storyteller.difficulty.enemyReproductionRateFactor));
        }
        
        public void FilterOutUnspawnedPawns()
        {
            for (int i = this.spawnedPawns.Count - 1; i >= 0; i--)
            {
                if (!this.spawnedPawns[i].Spawned)
                {
                    this.spawnedPawns.RemoveAt(i);
                }
            }
        }
        
        public bool TrySpawnPawn(out Pawn pawn)
        {
            if (!this.canSpawnPawns)
            {
                pawn = null;
                return false;
            }
            float curPoints = this.SpawnedPawnsPoints;
            IEnumerable<PawnKindDef> source = from x in spawnablePawnKinds
                                              where curPoints + x.combatPower <= MaxSpawnedPawnsPoints && x.isFighter && !x.factionLeader
                                              select x;
            if (!source.TryRandomElementByWeight((PawnKindDef x) => x.combatPower, out PawnKindDef kindDef))
            {
                pawn = null;
                return false;
            }
            pawn = PawnGenerator.GeneratePawn(kindDef, base.Faction);
            this.spawnedPawns.Add(pawn);
            GenSpawn.Spawn(pawn, CellFinder.RandomClosewalkCellNear(base.Position, base.Map, 2, null), base.Map, WipeMode.Vanish);
            Lord lord = this.Lord;
            if (lord == null)
            {
                lord = this.CreateNewLord();
            }
            lord.AddPawn(pawn);
            SoundDefOf.Hive_Spawn.PlayOneShot(this);
            return true;
        }
        
        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo g in base.GetGizmos())
            {
                yield return g;
            }
            if (Prefs.DevMode)
            {
                float curPoints = this.SpawnedPawnsPoints;
                string Desc = "All Possible Spawns: " + spawnablePawnKinds.Count;
                Desc += "\n";
                Desc += "points remaining: " + (MaxSpawnedPawnsPoints - curPoints);
                Desc += "\n\n";
                foreach (PawnKindDef PKD in spawnablePawnKinds)
                {
                    Desc += PKD.LabelCap + ", Points: " + PKD.combatPower;
                    Desc += "\n";
                }
                IEnumerable<PawnKindDef> source = from x in spawnablePawnKinds
                                                  where curPoints + x.combatPower <= MaxSpawnedPawnsPoints
                                                  select x;

                Desc += "\n";
                Desc += "Affordable Types: " + source.Count();
                Desc += "\n\n";
                foreach (PawnKindDef PKD in source)
                {
                    Desc += PKD.LabelCap + ", Points: " + PKD.combatPower;
                    Desc += "\n";
                }
                yield return new Command_Action
                {
                    defaultLabel = "DEBUG: Spawn pawn",
                    icon = TexCommand.ReleaseAnimals,
                    defaultDesc = Desc,
                    action = delegate ()
                    {
                        this.TrySpawnPawn(out Pawn pawn);
                    }
                };
            }
            yield break;
        }
        
        public override bool PreventPlayerSellingThingsNearby(out string reason)
        {
            if (this.spawnedPawns.Count > 0)
            {
                if (this.spawnedPawns.Any((Pawn p) => !p.Downed))
                {
                    reason = this.def.label;
                    return true;
                }
            }
            reason = null;
            return false;
        }
        
        public Lord CreateNewLord()
        {
            return LordMaker.MakeNewLord(base.Faction, new LordJob_DefendAndExpandHiveLike(!this.caveColony), base.Map, null);
        }
        
        public bool active = true;
        
        public int nextPawnSpawnTick = -1;
        
        public bool caveColony;
        
        public bool canSpawnPawns = true;
        
        public const int PawnSpawnRadius = 2;
        
        public List<PawnKindDef> spawnablePawnKinds = new List<PawnKindDef>();
        
        public static readonly string MemoAttackedByEnemy = "HiveAttacked";
        
        public static readonly string MemoDeSpawned = "HiveDeSpawned";
        
        public static readonly string MemoBurnedBadly = "HiveBurnedBadly";
        
        public static readonly string MemoDestroyedNonRoofCollapse = "HiveDestroyedNonRoofCollapse";

        public List<PawnKindDef> PawnKinds = new List<PawnKindDef>();

        public List<Pawn> spawnedPawns = new List<Pawn>();

        public HiveLike parentHiveLike;

        public static readonly FloatRange PawnSpawnIntervalDays = new FloatRange(0.85f, 1.15f);
    }
}
