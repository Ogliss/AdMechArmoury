using System;
using System.Collections.Generic;
using System.Linq;
using AdeptusMechanicus;
using RimWorld.Planet;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;

namespace RimWorld
{
    // Token: 0x0200025A RID: 602
    public class CompProperties_PawnSpawnerOnDamaged : CompProperties
    {
        // Token: 0x06000AC8 RID: 2760 RVA: 0x000562D4 File Offset: 0x000546D4
        public CompProperties_PawnSpawnerOnDamaged()
        {
            this.compClass = typeof(CompPawnSpawnerOnDamaged);
        }
        public FactionDef Faction;
        public List<FactionDef> Factions = new List<FactionDef>();
        public List<FactionDef> disallowedFactions = new List<FactionDef>();
        public TechLevel techLevel = TechLevel.Undefined;
        public bool allowHidden = true;
        public bool allowNonHumanlike = false;
        public bool allowDefeated = true;
        public ThingDef skyFaller;
        public string pawnGroupKind = "Combat";
        public List<PawnGenOption> PawnKinds = new List<PawnGenOption>();

        public Faction faction => Find.FactionManager.FirstFactionOfDef(Faction);
    }

    // Token: 0x02000769 RID: 1897
    public class CompPawnSpawnerOnDamaged : ThingComp
    {
        public CompProperties_PawnSpawnerOnDamaged Props => (CompProperties_PawnSpawnerOnDamaged)props;
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
                    //    Log.Message(string.Format("{0}", i.Name));
                }
                return factions;
            }
        }

        public TechLevel techLevel => Props.techLevel;

        public Faction OfFaction
        {
            get
            {
                if (faction == null)
                {
                    if (Props.Faction != null)
                    {
                        factionDef = Props.Faction;
                        faction = Find.FactionManager.FirstFactionOfDef(factionDef);
                    }
                    else if (Props.Factions.Count > 0)
                    {
                        factionDef = Props.Factions.RandomElement<FactionDef>();
                        faction = Find.FactionManager.FirstFactionOfDef(factionDef);
                    }
                    else if (parent.Faction != null)
                    {
                        factionDef = parent.Faction.def;
                        faction = parent.Faction;
                    }
                    else
                    {
                        faction = Find.FactionManager.RandomEnemyFaction(Props.allowHidden, Props.allowDefeated, Props.allowNonHumanlike, techLevel);
                        factionDef = faction.def;
                    }
                    return faction;
                }
                else
                {
                    return faction;
                }
            }
        }
        
        public List<PawnGenOption> PawnKinds
        {
            get
            {
                if (!Props.PawnKinds.NullOrEmpty())
                {
                    return Props.PawnKinds;
                }
                PawnGroupKindDef groupKindDef = DefDatabase<PawnGroupKindDef>.GetNamedSilentFail(Props.pawnGroupKind);
                if (groupKindDef == null)
                {
                    Log.Error("Props.pawnGroupKind.NullOrEmpty");
                }
                if (OfFaction != null)
                {
                    if (OfFaction.def.pawnGroupMakers.Any(x => !x.options.NullOrEmpty() && x.kindDef == groupKindDef))
                    {
                        return OfFaction.def.pawnGroupMakers.Where(x => !x.options.NullOrEmpty() && x.kindDef == groupKindDef).RandomElementByWeight((PawnGroupMaker x) => x.commonality).options;
                    }
                    else
                    {
                        Log.Error(string.Format("pawnGroupMakers.NullOrEmpty for {0} of def {1}", OfFaction.def, groupKindDef));
                    }
                    /*
                    if (DefDatabase<PawnKindDef>.AllDefsListForReading.Any(x=> x.defaultFactionType == OfFaction.def && x.isFighter))
                    {
                        foreach (PawnKindDef item in DefDatabase<PawnKindDef>.AllDefsListForReading.Where(x => x.defaultFactionType == OfFaction.def && x.isFighter))
                        {

                        }
                    }
                    */
                }
                else
                {
                    Log.Error(string.Format("Faction null for {0}",this.parent));
                }
                return new List<PawnGenOption>();
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
            // parent is Building_HiveLike_CrashedShipPart HivelikePart
            if (dinfo.Def.harmsHealth && this.parent.Map!=null)
            {
                if (this.lord != null)
                {
                    this.lord.ReceiveMemo(CompPawnSpawnerOnDamaged.MemoDamaged);
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
            //    Log.Message(string.Format("parent Spawned: {0}", this.parent.Spawned));
            if (this.lord == null)
            {
                IntVec3 invalid;
                if (!CellFinder.TryFindRandomCellNear(this.parent.Position, this.parent.Map, 5, (IntVec3 c) => c.Standable(this.parent.Map) && this.parent.Map.reachability.CanReach(c, this.parent, PathEndMode.Touch, TraverseParms.For(TraverseMode.PassDoors, Danger.Deadly, false)), out invalid, -1))
                {
                    Log.Error("Found no place for Pawns to defend " + this, false);
                    invalid = IntVec3.Invalid;
                }
                LordJob_PawnsDefendShip lordJob = new LordJob_PawnsDefendShip(this.parent, this.parent.Faction, 21f, invalid);
                this.lord = LordMaker.MakeNewLord(OfFaction, lordJob, this.parent.Map, null);
            }
            try
            {
                while (this.pointsLeft > 0f)
                {
                    PawnGenOption kindOption;
                    PawnKindDef kind;
                    if (this.parent is Building_HiveLike_CrashedShipPart _HiveShip)
                    {
                        _HiveShip.spawnablePawnKinds.TryRandomElement(out kind);
                    }
                    else
                    if (!(from opt in PawnKinds
                          where opt.kind.combatPower <= this.pointsLeft
                          select opt).TryRandomElementByWeight((PawnGenOption x) => x.selectionWeight, out kindOption))
                    {
                        if (PawnKinds.NullOrEmpty())
                        {
                            //    Log.Message(string.Format("PawnKinds.NullOrEmpty"));
                        }
                        //    Log.Message(string.Format("try spawn 2a"));
                        break;
                    }
                    else
                    {
                        kind = kindOption.kind;
                    }
                    //    Log.Message(string.Format("try spawn 3"));
                    IntVec3 center;
                    if (!(from cell in GenAdj.CellsAdjacent8Way(this.parent)
                          where this.CanSpawnPawnAt(cell)
                          select cell).TryRandomElement(out center))
                    {
                        break;
                    }
                    //    Log.Message(string.Format("try spawn 4"));
                    PawnGenerationRequest request = new PawnGenerationRequest(kind, faction, PawnGenerationContext.NonPlayer, -1, true, false, false, false, true, false, 1f);
                    //    Log.Message(string.Format("try spawn 5"));
                    Pawn pawn = PawnGenerator.GeneratePawn(request);
                    //    Log.Message(string.Format("try spawn 6"));
                    if (!GenPlace.TryPlaceThing(pawn, center, this.parent.Map, ThingPlaceMode.Near, null, null))
                    {
                        //    Log.Message(string.Format("try spawn 6b"));
                        Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Discard);
                        break;
                    }
                    //    Log.Message(string.Format("try spawn 7"));
                    this.lord.AddPawn(pawn);
                    //    Log.Message(string.Format("pawn: {0} to Lord: {1}", pawn.LabelShortCap, this.lord));
                    this.pointsLeft -= pawn.kindDef.combatPower;
                }
            }
            finally
            {
                //    Log.Message(string.Format("Finally {0} points left", this.pointsLeft));
                this.pointsLeft = 0f;
                //    Log.Message(string.Format("set points left to {0} ", this.pointsLeft));
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
