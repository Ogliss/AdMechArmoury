using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;

namespace RimWorld
{
    // Token: 0x0200025A RID: 602
    public class CompProperties_SpawnerPawnsOnDamaged : CompProperties
    {
        // Token: 0x06000AC8 RID: 2760 RVA: 0x000562D4 File Offset: 0x000546D4
        public CompProperties_SpawnerPawnsOnDamaged()
        {
            this.compClass = typeof(CompSpawnerPawnsOnDamaged);
        }
        public FactionDef Faction;
        public Faction faction;
        public List<FactionDef> Factions = new List<FactionDef>();
        public List<FactionDef> disallowedFactions = new List<FactionDef>();
        public String techLevel;
        public bool allowHidden = true;
        public bool allowNonHumanlike = false;
        public bool allowDefeated = true;
        public ThingDef skyFaller;
    }

    // Token: 0x02000769 RID: 1897
    public class CompSpawnerPawnsOnDamaged : ThingComp
    {
        public CompProperties_SpawnerPawnsOnDamaged Props => (CompProperties_SpawnerPawnsOnDamaged)props;
        public FactionDef factionDef;
        public Faction faction = null;
        public List<Faction> AllFactions
        {
            get
            {
                List<Faction> allfactions = Find.FactionManager.AllFactionsListForReading;
                List<Faction> factions = Find.FactionManager.AllFactionsListForReading;
                if (Props.disallowedFactions!=null)
                {
                    foreach (var i in allfactions)
                    {
                        if (!Props.disallowedFactions.Contains(i.def))
                        {
                            factions.Remove(i);
                        }
                    }
                }
                foreach (var i in factions)
                {
                    Log.Message(string.Format("{0}", i.Name));
                }
                return factions;
            }
        }

        public TechLevel techLevel
        {
            get
            {
                if (Props.techLevel!=null)
                {
                    if (Props.techLevel == "Animal")
                    {
                        return TechLevel.Animal;
                    }
                    if (Props.techLevel == "Archotech")
                    {
                        return TechLevel.Archotech;
                    }
                    if (Props.techLevel == "Industrial")
                    {
                        return TechLevel.Industrial;
                    }
                    if (Props.techLevel == "Medieval")
                    {
                        return TechLevel.Medieval;
                    }
                    if (Props.techLevel == "Neolithic")
                    {
                        return TechLevel.Neolithic;
                    }
                    if (Props.techLevel == "Spacer")
                    {
                        return TechLevel.Spacer;
                    }
                    if (Props.techLevel == "Ultra")
                    {
                        return TechLevel.Ultra;
                    }
                    else return TechLevel.Undefined;
                }
                else return TechLevel.Undefined;
            }
        }

        public Faction OfFaction
        {
            get
            {
                if (faction == null)
                {
                    if (Props.Faction != null)
                    {
                    //    Log.Message(string.Format("Loading Faction Def from CompProps"));
                        factionDef = Props.Faction;
                        faction = Find.FactionManager.FirstFactionOfDef(factionDef);
                        Props.faction = faction;
                    //    Log.Message(string.Format("Owner: {0} Def of:{1}", Find.FactionManager.FirstFactionOfDef(factionDef), factionDef));
                        return faction;
                    }
                    else if (Props.Factions.Count > 0)
                    {
                    //    Log.Message(string.Format("Loading Faction List from CompProps"));
                        factionDef = Props.Factions.RandomElement<FactionDef>();
                        Props.faction = faction;
                        faction = Find.FactionManager.FirstFactionOfDef(factionDef);
                    //    Log.Message(string.Format("Owner: {0} Def of:{1}", Find.FactionManager.FirstFactionOfDef(factionDef), factionDef));
                        return faction;
                    }
                    else
                    {
                    //    Log.Message(string.Format("Getting Random Enemy Faction"));
                        faction = Find.FactionManager.RandomEnemyFaction(Props.allowHidden, Props.allowDefeated, Props.allowNonHumanlike, techLevel);
                        factionDef = faction.def;
                        Props.faction = faction;
                    //    Log.Message(string.Format("Owner: {0} Def of:{1}", faction.Name, factionDef));
                        return faction;
                    }
                }
                else
                {
                    return faction;
                }
            }
        }

        // Token: 0x060029EB RID: 10731 RVA: 0x0013D92F File Offset: 0x0013BD2F
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_References.Look<Lord>(ref this.lord, "defenseLord", false);
            Scribe_References.Look<Faction>(ref this.faction, "defenseFaction", false);
            Scribe_Values.Look<float>(ref this.pointsLeft, "PawnPointsLeft", 0f, false);
        }

        // Token: 0x060029EC RID: 10732 RVA: 0x0013D960 File Offset: 0x0013BD60
        public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            base.PostPreApplyDamage(dinfo, out absorbed);
            if (absorbed)
            {
                return;
            }
            if (dinfo.Def.harmsHealth)
            {
                if (this.lord != null)
                {
                    this.lord.ReceiveMemo(CompSpawnerPawnsOnDamaged.MemoDamaged);
                }
                float num = (float)this.parent.HitPoints - dinfo.Amount;
                if ((num < (float)this.parent.MaxHitPoints * 0.98f && dinfo.Instigator != null && dinfo.Instigator.Faction != null) || num < (float)this.parent.MaxHitPoints * 0.9f)
                {
                    this.TrySpawnPawns();
                }
            }
            absorbed = false;
        }

        // Token: 0x060029ED RID: 10733 RVA: 0x0013DA14 File Offset: 0x0013BE14
        public void Notify_BlueprintReplacedWithSolidThingNearby(Pawn by)
        {
            if (by.Faction != OfFaction)
            {
                this.TrySpawnPawns();
            }
        }

        // Token: 0x060029EE RID: 10734 RVA: 0x0013DA2C File Offset: 0x0013BE2C
        private void TrySpawnPawns()
        {
            if (this.pointsLeft <= 0f)
            {
                return;
            }
            if (!this.parent.Spawned)
            {
                return;
            }
            if (this.lord == null)
            {
                IntVec3 invalid;
                if (!CellFinder.TryFindRandomCellNear(this.parent.Position, this.parent.Map, 5, (IntVec3 c) => c.Standable(this.parent.Map) && this.parent.Map.reachability.CanReach(c, this.parent, PathEndMode.Touch, TraverseParms.For(TraverseMode.PassDoors, Danger.Deadly, false)), out invalid, -1))
                {
                //    Log.Error("Found no place for Pawns to defend " + this, false);
                    invalid = IntVec3.Invalid;
                }
                LordJob_PawnsDefendShip lordJob = new LordJob_PawnsDefendShip(this.parent, this.parent.Faction, 21f, invalid);
                this.lord = LordMaker.MakeNewLord(OfFaction, lordJob, this.parent.Map, null);
            }
            try
            {
                while (this.pointsLeft > 0f)
                {
                    PawnKindDef kind;
                    if (!(from def in DefDatabase<PawnKindDef>.AllDefs
                          where ((def.defaultFactionType == faction.def && def.defaultFactionType != null) || (def.defaultFactionType == null && faction.def.pawnGroupMakers.Any(pgm => pgm.options.Any(opt => opt.kind == def) && pgm.kindDef != PawnGroupKindDefOf.Trader && pgm.kindDef != PawnGroupKindDefOf.Peaceful))) && def.isFighter && def.combatPower <= this.pointsLeft
                          //where ((def.defaultFactionType == faction.def && def.defaultFactionType != null) || (!faction.def.pawnGroupMakers.All(pgm => pgm.options.Any(opt => opt.kind == def)) && def.defaultFactionType == null)) && def.isFighter && def.combatPower <= this.pointsLeft
                          select def).TryRandomElement(out kind))
                    {
                    //    Log.Message(string.Format("kindDef: {0}", kind));
                        break;
                    }
                    IntVec3 center;
                    if (!(from cell in GenAdj.CellsAdjacent8Way(this.parent)
                          where this.CanSpawnPawnAt(cell)
                          select cell).TryRandomElement(out center))
                    {
                        break;
                    }
                //    Log.Message(string.Format("kindDef: {0}", kind));
                    PawnGenerationRequest request = new PawnGenerationRequest(kind, faction, PawnGenerationContext.NonPlayer, -1, true, false, false, false, true, false, 1f, false, true, true, false, false, false, false, null, null, null, null, null, null, null, null);
                    Pawn pawn = PawnGenerator.GeneratePawn(request);
                    if (!GenPlace.TryPlaceThing(pawn, center, this.parent.Map, ThingPlaceMode.Near, null, null))
                    {
                        Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Discard);
                        break;
                    }
                    this.lord.AddPawn(pawn);
                    this.pointsLeft -= pawn.kindDef.combatPower;
                }
            }
            finally
            {
                this.pointsLeft = 0f;
            }
            SoundDefOf.PsychicPulseGlobal.PlayOneShotOnCamera(this.parent.Map);
        }

        // Token: 0x060029EF RID: 10735 RVA: 0x0013DC44 File Offset: 0x0013C044
        private bool CanSpawnPawnAt(IntVec3 c)
        {
            return c.Walkable(this.parent.Map);
        }

        // Token: 0x04001746 RID: 5958
        public float pointsLeft;

        // Token: 0x04001747 RID: 5959
        private Lord lord;

        // Token: 0x04001748 RID: 5960
        private const float PawnsDefendRadius = 21f;

        // Token: 0x04001749 RID: 5961
        public static readonly string MemoDamaged = "ShipPartDamaged";

        // Token: 0x04000FB7 RID: 4023
        private List<Faction> allFactions = new List<Faction>();
    }
}
