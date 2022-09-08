using AdeptusMechanicus.ExtensionMethods;
using AdeptusMechanicus.settings;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI.Group;

namespace AdeptusMechanicus
{
    public enum StrikeTargetType
    {
        Aggressor,
        Turrets,
        Power,
        HighValue,
        Altar
    }

    public class ReserveForce : IExposable
    {
        public ReserveForce()
        {
        }

        public ReserveForce(Faction faction = null, int delay = 0, Map map = null, List<Pawn> members = null)
        {
            this.members = members ?? new List<Pawn>();
            this.faction = faction;
            this.map = map;
            this.delay = delay;
            this.target = IntVec3.Invalid;
        }
        public ReserveForce(IntVec3? target = null, Faction faction = null, int delay = 0, Map map = null, List<Pawn> members = null)
        {
            this.members = members ?? new List<Pawn>();
            this.faction = faction;
            this.map = map;
            this.delay = delay;
            if (target.HasValue) this.target = target.Value;
        }
        public string Label
        {
            get
            {
                return $"{faction.NameColored} ({faction.def.LabelCap}) Reserves";
            }
        }
        public void AddPawns(List<Pawn> pawns)
        {
            foreach (Pawn p in pawns)
            {
                AddPawn(p);
            }
        }

        public void AddPawn(Pawn p)
        {

            if (!this.members.Contains(p))
            {
                if (p.Spawned)
                {
                    p.DeSpawn(DestroyMode.Vanish);
                }
                SortReserve(p);
                this.members.Add(p);
            }
        }
        
        public void SortReserves()
        {
            foreach (var p in Members)
            {
                SortReserve(p);
            }
        }

        public void SortReserve(Pawn p)
        {
            ReserveDeploymentType type = p.ReserveDeployment()?.pawnsArrivalMode ?? ReserveDeploymentType.DropPod;
            switch (type)
            {
                /*
                case DeepStrikeType.DropPara:
                    if (!this.paradroppers.Contains(p)) paradroppers.Add(p);
                    break;
                case DeepStrikeType.DropShip:
                    if (!this.dropshipers.Contains(p)) dropshipers.Add(p);
                    break;
                    */
                case ReserveDeploymentType.Fly:
                    if (!this.flyers.Contains(p)) flyers.Add(p);
                    break;
                case ReserveDeploymentType.Teleport:
                    if (!this.teleporters.Contains(p)) teleporters.Add(p);
                    break;
                case ReserveDeploymentType.Tunnel:
                    if (!this.tunnellers.Contains(p)) tunnellers.Add(p);
                    break;
                case ReserveDeploymentType.Infiltrate:
                    if (!this.infiltrators.Contains(p)) infiltrators.Add(p);
                    break;
                default:
                    if (!this.droppers.Contains(p)) droppers.Add(p);
                    break;
            }
        }

        public void RemovePawns(List<Pawn> pawns)
        {
            foreach (Pawn p in pawns)
            {
                RemovePawn(p);
            }
        }

        public void RemovePawn(Pawn p)
        {
            if (this.members.Contains(p))
            {
                this.members.Remove(p);
            }
        }

        public string TimeBeforeDeployment()
        {
            bool useRimworldTime = AMAMod.settings.rimTime;
            string result;
            if (useRimworldTime)
            {
                result = delay.ToStringTicksToPeriod(true, false, true, true);
            }
            else
            {
                result = delay.TicksToSeconds().ToString();
            }
            return result;
        }

        public void Tick()
        {
            if (this.delay>-1)
            {
                this.delay--;
            }
            if (DeployNow)
            {
                SortReserves();
                DeployReserve();
            }
            if (deployed && !defeated)
            {
                if (!this.members.Any(x=> !(x.Dead || x.Downed)))
                {
                    Log.Message($"Reserve force {Label} defeated");
                    defeated = true;
                }
            }
        }

        public void Notify_PawnLostViolently(Lord lord, out bool reacting, DamageInfo? dinfo = null)
        {
            Rand.PushState();
            reacting = Rand.Chance(0.35f);
            Rand.PopState();
            if (reacting)
            {
                if (dinfo?.Instigator != null)
                    target = dinfo?.Instigator.Position ?? IntVec3.Invalid;
                delay = 1;
            }

        }
        public void Notify_PawnDamaged(Lord lord, DamageInfo dinfo, out bool reacting)
        {
            Rand.PushState();
            reacting = Rand.Chance(0.35f);
            Rand.PopState();
            if (reacting)
            {
                if (dinfo.Instigator != null)
                    target = dinfo.Instigator.Position;
                delay = 1;
            }
        }

        public void DeployReserve()
        {
            Log.Message($"DeployReserve with {Members.Count} pawns");
            string str = string.Empty;
            List<Pawn> pawns = new List<Pawn>();
            bool infilOnly = false;
            Log.Message($"DeployReserve PRE: Infiltrators: {infiltrators.Count}");
            if (!infiltrators.NullOrEmpty())
            {
                str += str == string.Empty ? DeepStrikeUtility.DeepstrikeArrivalmode(ReserveDeploymentType.Infiltrate) : ", " + DeepStrikeUtility.DeepstrikeArrivalmode(ReserveDeploymentType.Infiltrate);
                pawns.AddRange(infiltrators);
                Arrive(infiltrators, ReserveDeploymentType.Infiltrate);
                Log.Message($"DeployReserve POST: Infiltrators");
                infilOnly = true;
            }
            Log.Message($"DeployReserve PRE: Droppers: {droppers.Count}");
            if (!droppers.NullOrEmpty())
            {
                str += str == string.Empty ? DeepStrikeUtility.DeepstrikeArrivalmode(ReserveDeploymentType.DropPod) : ", " + DeepStrikeUtility.DeepstrikeArrivalmode(ReserveDeploymentType.DropPod);
                pawns.AddRange(droppers);
                Arrive(droppers, ReserveDeploymentType.DropPod);
                Log.Message($"DeployReserve POST: Droppers");
                infilOnly = false;
            }
            Log.Message($"DeployReserve PRE: Flyers: {flyers.Count}");
            if (!flyers.NullOrEmpty())
            {
                str += str == string.Empty ? DeepStrikeUtility.DeepstrikeArrivalmode(ReserveDeploymentType.Fly) : ", " + DeepStrikeUtility.DeepstrikeArrivalmode(ReserveDeploymentType.Fly);
                pawns.AddRange(flyers);
                Arrive(flyers, ReserveDeploymentType.Fly);
                Log.Message($"DeployReserve POST: Flyers");
                infilOnly = false;
            }
            Log.Message($"DeployReserve PRE: Teleporters: {teleporters.Count}");
            if (!teleporters.NullOrEmpty())
            {
                str += str == string.Empty ? DeepStrikeUtility.DeepstrikeArrivalmode(ReserveDeploymentType.Teleport) : ", " + DeepStrikeUtility.DeepstrikeArrivalmode(ReserveDeploymentType.Teleport);
                pawns.AddRange(teleporters);
                Arrive(teleporters, ReserveDeploymentType.Teleport);
                Log.Message($"DeployReserve POST: Teleporters");
                infilOnly = false;
            }
            Log.Message($"DeployReserve PRE: Tunnellers: {tunnellers.Count}");
            if (!tunnellers.NullOrEmpty())
            {
                str += str == string.Empty ? DeepStrikeUtility.DeepstrikeArrivalmode(ReserveDeploymentType.Tunnel) : ", " + DeepStrikeUtility.DeepstrikeArrivalmode(ReserveDeploymentType.Tunnel);
                pawns.AddRange(tunnellers);
                Arrive(tunnellers, ReserveDeploymentType.Tunnel);
                Log.Message($"DeployReserve POST: Tunnellers");
                infilOnly = false;
            }
            if (infilOnly) Find.LetterStack.ReceiveLetter("AdeptusMechanicus.Infiltrators_Revealed".Translate(faction.def.pawnSingular), "AdeptusMechanicus.Infiltrators_Revealed_Letter".Translate(faction.def.pawnsPlural), LetterDefOf.ThreatBig, infiltrators, faction, null);
            else Find.LetterStack.ReceiveLetter("AdeptusMechanicus.DeepStrike_Incomming".Translate(faction.def.pawnSingular), "AdeptusMechanicus.DeepStrike_Incomming_Letter".Translate(faction.def.pawnsPlural, str), LetterDefOf.ThreatBig, Members, faction, null);

            Log.Message($"DoStrike Post ReceiveLetter");
            deployed = true;
        }
        
        public void Arrive(List<Pawn> pawns, ReserveDeploymentType strikeType = ReserveDeploymentType.DropPod)
        {
            for (int i = 0; i < pawns.Count; i++)
            {
                //    IntVec3 dropCenter = map.listerBuildings.allBuildingsColonist.FindAll(x => x.TryGetCompFast<CompPowerPlant>() != null).RandomElement().Position;
                IntVec3 dropCenter;
                if (strikeType != ReserveDeploymentType.Infiltrate)
                {
                    if (DropCellFinder.TryFindRaidDropCenterClose(out dropCenter, map))
                    {
                        DeepStrikeUtility.DropThingsNear(dropCenter, map, Gen.YieldSingle<Thing>(pawns[i]), 50, false, false, true, strikeType);
                    }
                }
                else
                {
                    if (DropCellFinder.TryFindRaidDropCenterClose(out dropCenter, map))
                    {
                        if (RCellFinder.TryFindRandomSpotJustOutsideColony(dropCenter, map, out dropCenter))
                        {
                            InfiltrateUtility.PlaceInfiltratorsNear(dropCenter, map, Gen.YieldSingle<Thing>(pawns[i]), 50, true, false, true);
                        }
                        else
                        {
                            InfiltrateUtility.PlaceInfiltratorsNear(dropCenter, map, Gen.YieldSingle<Thing>(pawns[i]), 50, true, false, true);
                        }
                    }
                    else
                    {
                        dropCenter = DropCellFinder.FindRaidDropCenterDistant(map, true);
                        if (RCellFinder.TryFindRandomSpotJustOutsideColony(dropCenter, map, out dropCenter))
                        {
                            InfiltrateUtility.PlaceInfiltratorsNear(dropCenter, map, Gen.YieldSingle<Thing>(pawns[i]), 50, true, false, true);
                        }
                        else
                        {
                            InfiltrateUtility.PlaceInfiltratorsNear(dropCenter, map, Gen.YieldSingle<Thing>(pawns[i]), 50, true, false, true);
                        }
                    }
                }
            }

        }

        public MapComponent_Reserves DeepStrikeController => deepStrikeController != null ? deepStrikeController : deepStrikeController = map.GetComponent<MapComponent_Reserves>();
        public List<Lord> Lords
        {
            get
            {
                List<Lord> result;
                if (!this.lords.NullOrEmpty<Lord>())
                {
                    result = this.lords;
                }
                else
                {
                    if (map.lordManager != null && !map.lordManager.lords.NullOrEmpty<Lord>())
                    {
                        this.lords = map.lordManager.lords.FindAll((Lord l) => l.faction != null && l.faction == this.faction && !l.ownedPawns.NullOrEmpty<Pawn>() && l.AnyActivePawn);
                    }
                    result = (this.lords ?? null);
                }
                return result;
            }
        }
        public virtual bool DeployNow => this.delay <= 0 && !this.deployed;
        public List<Pawn> Members => members;

        public virtual void ExposeData()
        {
            Scribe_References.Look(ref faction, "faction");
            Scribe_References.Look(ref map, "map");
            Scribe_Values.Look(ref delay, "delay", -1);
            Scribe_Values.Look(ref deployed, "deployed", false);
            Scribe_Values.Look(ref defeated, "defeated", false);
            Scribe_Values.Look(ref target, "target", IntVec3.Invalid);
            Scribe_Collections.Look(ref members, "members", LookMode.Deep);
            Scribe_Collections.Look(ref droppers, "droppers", LookMode.Reference);
            Scribe_Collections.Look(ref dropshipers, "dropshipers", LookMode.Reference);
            Scribe_Collections.Look(ref flyers, "flyers", LookMode.Reference);
            Scribe_Collections.Look(ref infiltrators, "infiltrators", LookMode.Reference);
            Scribe_Collections.Look(ref paradroppers, "paradroppers", LookMode.Reference);
            Scribe_Collections.Look(ref teleporters, "teleporters", LookMode.Reference);
            Scribe_Collections.Look(ref tunnellers, "tunnellers", LookMode.Reference);
        }

        public Faction faction;
        public Map map;
        public int delay;
        public IntVec3 target;
        public bool defeated = false;
        public bool deployed = false;
        public List<Pawn> droppers = new List<Pawn>();
        public List<Pawn> paradroppers = new List<Pawn>();
        public List<Pawn> dropshipers = new List<Pawn>();
        public List<Pawn> flyers = new List<Pawn>();
        public List<Pawn> infiltrators = new List<Pawn>();
        public List<Pawn> teleporters = new List<Pawn>();
        public List<Pawn> tunnellers = new List<Pawn>();

        private MapComponent_Reserves deepStrikeController;
        private List<Pawn> members;
        private List<Lord> lords;
    }
}
