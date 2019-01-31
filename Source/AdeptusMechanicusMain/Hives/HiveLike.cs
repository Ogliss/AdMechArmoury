using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace RimWorld
{
    public class ThingDef_HiveLike : ThingDef
    {
        public FactionDef Faction;
        public ThingDef TunnelDef;
        public List<PawnKindDef> PawnKinds = new List<PawnKindDef>();
    }
    // Token: 0x020006EC RID: 1772
    public class HiveLike : ThingWithComps
    {
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
             //   Log.Message(string.Format("Faction: {0}", Find.FactionManager.FirstFactionOfDef(Def.Faction)));
                return Find.FactionManager.FirstFactionOfDef(Def.Faction);
            }
        }

        public FactionDef OfFactionDef
        {
            get
            {
                //Log.Message(string.Format("FactionDef: {0}", Find.FactionManager.FirstFactionOfDef(Def.Faction)));
                return Def.Faction;
            }
        }

        public ThingDef OfTunnel
        {
            get
            {
                //Log.Message(string.Format("HiveLikeDef: {0}", Def.TunnelDef.defName));
                return Def.TunnelDef; 
            }
        }

        public List<PawnKindDef> OfPawnKinds
        {
            get
            {
                if (Def.PawnKinds.Count>0)
                {
                    PawnKinds = Def.PawnKinds;
                }
                else
                {
                    var list = (from def in DefDatabase<PawnKindDef>.AllDefs
                                where ((def.defaultFactionType == OfFaction.def && def.defaultFactionType != null) || (def.defaultFactionType == null && OfFaction.def.pawnGroupMakers.Any(pgm => pgm.options.Any(opt => opt.kind == def) && pgm.kindDef != PawnGroupKindDefOf.Trader && pgm.kindDef != PawnGroupKindDefOf.Peaceful))) && def.isFighter
                                select def).ToList();
                    if (list.Count>0)
                    {
                        PawnKinds = list;
                    }
                }
                //Log.Message(string.Format("PawnKinds: {0}", PawnKinds.ToString()));
                //Log.Message(string.Format("PawnKinds.Count: {0}", PawnKinds.Count));
                return PawnKinds;
            }
        }

        public List<PawnKindDef> PawnKinds = new List<PawnKindDef>();
        // Token: 0x170005CD RID: 1485
        // (get) Token: 0x06002670 RID: 9840 RVA: 0x00123F94 File Offset: 0x00122394
        private Lord Lord
		{
			get
			{
				Predicate<Pawn> hasDefendHiveLord = delegate(Pawn x)
				{
					Lord lord = x.GetLord();
					return lord != null && lord.LordJob is LordJob_DefendAndExpandHiveLike;
				};
				Pawn foundPawn = this.spawnedPawns.Find(hasDefendHiveLord);
				if (base.Spawned)
				{
					if (foundPawn == null)
					{
						RegionTraverser.BreadthFirstTraverse(this.GetRegion(RegionType.Set_Passable), (Region from, Region to) => true, delegate(Region r)
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

		// Token: 0x170005CE RID: 1486
		// (get) Token: 0x06002671 RID: 9841 RVA: 0x00124050 File Offset: 0x00122450
		private float SpawnedPawnsPoints
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

		// Token: 0x06002672 RID: 9842 RVA: 0x0012409F File Offset: 0x0012249F
		public void ResetStaticData()
		{
			spawnablePawnKinds.Clear();
            if (OfPawnKinds.Count > 0)
            {
                spawnablePawnKinds = OfPawnKinds;
            }
            else
            {
                if (OfFactionDef.basicMemberKind!=null)
                {
                    spawnablePawnKinds.Add(OfFactionDef.basicMemberKind);
                }
                else
                {
                    Log.Error(string.Format("COuldnt find any pawnkinds of the {0} faction to spawn for {1}", OfFaction, OfTunnel.defName));
                }
            }
            Log.Error(string.Format("COuldnt find any pawnkinds of the {0} faction to spawn for {1}", OfFaction, OfTunnel.defName));
        }

		// Token: 0x06002673 RID: 9843 RVA: 0x001240D8 File Offset: 0x001224D8
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
		}

		// Token: 0x06002674 RID: 9844 RVA: 0x00124110 File Offset: 0x00122510
		private void SpawnInitialPawns()
		{
			this.SpawnPawnsUntilPoints(200f);
			this.CalculateNextPawnSpawnTick();
		}

		// Token: 0x06002675 RID: 9845 RVA: 0x00124124 File Offset: 0x00122524
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
				Pawn pawn;
				if (!this.TrySpawnPawn(out pawn))
				{
					break;
				}
			}
			this.CalculateNextPawnSpawnTick();
		}

		// Token: 0x06002676 RID: 9846 RVA: 0x0012417C File Offset: 0x0012257C
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
					if (this.SpawnedPawnsPoints < 500f)
					{
						Pawn pawn;
						bool flag = this.TrySpawnPawn(out pawn);
						if (flag && pawn.caller != null)
						{
							pawn.caller.DoCall();
						}
					}
					this.CalculateNextPawnSpawnTick();
				}
			}
		}

		// Token: 0x06002677 RID: 9847 RVA: 0x00124224 File Offset: 0x00122624
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

		// Token: 0x06002678 RID: 9848 RVA: 0x0012427C File Offset: 0x0012267C
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

		// Token: 0x06002679 RID: 9849 RVA: 0x0012431C File Offset: 0x0012271C
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

		// Token: 0x0600267A RID: 9850 RVA: 0x001243A0 File Offset: 0x001227A0
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<bool>(ref this.active, "active", false, false);
			Scribe_Values.Look<int>(ref this.nextPawnSpawnTick, "nextPawnSpawnTick", 0, false);
			Scribe_Collections.Look<Pawn>(ref this.spawnedPawns, "spawnedPawns", LookMode.Reference, new object[0]);
			Scribe_Values.Look<bool>(ref this.caveColony, "caveColony", false, false);
			Scribe_Values.Look<bool>(ref this.canSpawnPawns, "canSpawnPawns", true, false);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				this.spawnedPawns.RemoveAll((Pawn x) => x == null);
			}
		}

		// Token: 0x0600267B RID: 9851 RVA: 0x00124448 File Offset: 0x00122848
		private void Activate()
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

		// Token: 0x0600267C RID: 9852 RVA: 0x0012447C File Offset: 0x0012287C
		private void CalculateNextPawnSpawnTick()
		{
			float num = GenMath.LerpDouble(0f, 5f, 1f, 0.5f, (float)this.spawnedPawns.Count);
			this.nextPawnSpawnTick = Find.TickManager.TicksGame + (int)(HiveLike.PawnSpawnIntervalDays.RandomInRange * 60000f / (num * Find.Storyteller.difficulty.enemyReproductionRateFactor));
		}

		// Token: 0x0600267D RID: 9853 RVA: 0x001244E8 File Offset: 0x001228E8
		private void FilterOutUnspawnedPawns()
		{
			for (int i = this.spawnedPawns.Count - 1; i >= 0; i--)
			{
				if (!this.spawnedPawns[i].Spawned)
				{
					this.spawnedPawns.RemoveAt(i);
				}
			}
		}

		// Token: 0x0600267E RID: 9854 RVA: 0x00124538 File Offset: 0x00122938
		private bool TrySpawnPawn(out Pawn pawn)
		{
			if (!this.canSpawnPawns)
			{
				pawn = null;
				return false;
			}
			float curPoints = this.SpawnedPawnsPoints;
			IEnumerable<PawnKindDef> source = from x in spawnablePawnKinds
			where curPoints + x.combatPower <= 500f
			select x;
			PawnKindDef kindDef;
			if (!source.TryRandomElement(out kindDef))
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

		// Token: 0x0600267F RID: 9855 RVA: 0x001245FC File Offset: 0x001229FC
		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (Gizmo g in base.GetGizmos())
			{
				yield return g;
			}
			if (Prefs.DevMode)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEBUG: Spawn pawn",
					icon = TexCommand.ReleaseAnimals,
					action = delegate()
					{
						Pawn pawn;
						this.TrySpawnPawn(out pawn);
					}
				};
			}
			yield break;
		}

		// Token: 0x06002680 RID: 9856 RVA: 0x00124620 File Offset: 0x00122A20
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

		// Token: 0x06002681 RID: 9857 RVA: 0x0012467E File Offset: 0x00122A7E
		private Lord CreateNewLord()
		{
			return LordMaker.MakeNewLord(base.Faction, new LordJob_DefendAndExpandHiveLike(!this.caveColony), base.Map, null);
		}

		// Token: 0x040015AA RID: 5546
		public bool active = true;

		// Token: 0x040015AB RID: 5547
		public int nextPawnSpawnTick = -1;

		// Token: 0x040015AC RID: 5548
		private List<Pawn> spawnedPawns = new List<Pawn>();

		// Token: 0x040015AD RID: 5549
		public bool caveColony;

		// Token: 0x040015AE RID: 5550
		public bool canSpawnPawns = true;

		// Token: 0x040015AF RID: 5551
		public const int PawnSpawnRadius = 2;

		// Token: 0x040015B0 RID: 5552
		public const float MaxSpawnedPawnsPoints = 500f;

		// Token: 0x040015B1 RID: 5553
		public const float InitialPawnsPoints = 200f;

		// Token: 0x040015B2 RID: 5554
		private static readonly FloatRange PawnSpawnIntervalDays = new FloatRange(0.85f, 1.15f);

		// Token: 0x040015B3 RID: 5555
		public List<PawnKindDef> spawnablePawnKinds = new List<PawnKindDef>();

		// Token: 0x040015B4 RID: 5556
		public static readonly string MemoAttackedByEnemy = "HiveAttacked";

		// Token: 0x040015B5 RID: 5557
		public static readonly string MemoDeSpawned = "HiveDeSpawned";

		// Token: 0x040015B6 RID: 5558
		public static readonly string MemoBurnedBadly = "HiveBurnedBadly";

		// Token: 0x040015B7 RID: 5559
		public static readonly string MemoDestroyedNonRoofCollapse = "HiveDestroyedNonRoofCollapse";
	}
}
