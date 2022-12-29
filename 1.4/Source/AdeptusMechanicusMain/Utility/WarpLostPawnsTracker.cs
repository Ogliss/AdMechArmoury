using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace AdeptusMechanicus
{
    public class WarpLostPawnsTracker : WorldComponent
    {
        public WarpLostPawnsTracker(World world) : base(world)
        {
            this.world = world;
        }

        public List<Pawn> WarpLostPawnsListForReading
        {
            get
            {
                return this.warpLostPawns;
            }
        }


        public override void ExposeData()
        {
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                this.warpLostPawns.RemoveAll((Pawn x) => x.Destroyed);
            }
            Scribe_Collections.Look<Pawn>(ref this.warpLostPawns, "warpLostPawns", LookMode.Reference, Array.Empty<object>());
        }

        public void Kidnap(Pawn pawn, Pawn kidnapper)
        {
            if (this.warpLostPawns.Contains(pawn))
            {
                Log.Error("The Warp tried to take an already Warp Lost pawn " + pawn);
                return;
            }
            this.PreLost(pawn, kidnapper);
            pawn.DeSpawnOrDeselect(DestroyMode.Vanish);
            this.warpLostPawns.Add(pawn);
            if (!Find.WorldPawns.Contains(pawn))
            {
                Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Decide);
                if (!Find.WorldPawns.Contains(pawn))
                {
                    Log.Error("WorldPawns discarded warp lost pawn.");
                    this.warpLostPawns.Remove(pawn);
                }
            }
            if (pawn.Faction == Faction.OfPlayer)
            {
                PawnDiedOrDownedThoughtsUtility.TryGiveThoughts(pawn, null, PawnDiedOrDownedThoughtsKind.Lost);
                BillUtility.Notify_ColonistUnavailable(pawn);
                if (kidnapper != null)
                {
                    Find.LetterStack.ReceiveLetter("LetterLabelPawnsKidnapped".Translate(pawn.Named("PAWN")), "LetterPawnsKidnapped".Translate(pawn.Named("PAWN"), kidnapper.Faction.Named("FACTION")), LetterDefOf.NegativeEvent, null);
                }
            }
            QuestUtility.SendQuestTargetSignals(pawn.questTags, "Kidnapped", pawn.Named("SUBJECT"), kidnapper.Named("KIDNAPPER"));
            Find.GameEnder.CheckOrUpdateGameOver();
        }

        public void PreLost(Pawn pawn, Pawn kidnapper)
        {
            Find.Storyteller.Notify_PawnEvent(pawn, AdaptationEvent.Kidnapped, null);
            if (pawn.IsColonist && kidnapper != null)
            {
                TaleRecorder.RecordTale(TaleDefOf.KidnappedColonist, new object[]
                {
                    kidnapper,
                    this
                });
            }
            if (pawn.ownership != null)
            {
                pawn.ownership.UnclaimAll();
            }
            if (pawn.guest != null && !pawn.guest.IsSlave)
            {
                pawn.guest.SetGuestStatus(null, RimWorld.GuestStatus.Guest);
            }
            if (pawn.RaceProps.IsFlesh)
            {
                pawn.relations.Notify_PawnKidnapped();
            }
            pawn.ClearMind(false, false, true);
        }
        public void RemoveWarpLostPawn(Pawn pawn)
        {
            if (this.warpLostPawns.Remove(pawn))
            {
                if (pawn.Faction == Faction.OfPlayer)
                {
                    PawnDiedOrDownedThoughtsUtility.RemoveLostThoughts(pawn);
                    return;
                }
            }
            else
            {
                Log.Warning("Tried to remove Warp Lost pawn " + pawn + " but he's not here.");
            }
        }

        public void LogWarpLostPawns()
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < this.warpLostPawns.Count; i++)
            {
                stringBuilder.AppendLine(this.warpLostPawns[i].Name.ToStringFull+": Lost to the Warp");
            }
            Log.Message(stringBuilder.ToString());
        }

        public override void WorldComponentTick()
        {
            base.WorldComponentTick();
            for (int i = this.warpLostPawns.Count - 1; i >= 0; i--)
            {
                if (this.warpLostPawns[i].DestroyedOrNull())
                {
                    this.warpLostPawns.RemoveAt(i);
                }
            }
            if (Find.TickManager.TicksGame % 15051 == 0)
            {
                for (int j = this.warpLostPawns.Count - 1; j >= 0; j--)
                {
                    if (Rand.MTBEventOccurs(30f, 60000f, 15051f))
                    {
                        this.warpLostPawns.RemoveAt(j);
                    }
                }
            }
        }

        private List<Pawn> warpLostPawns = new List<Pawn>();
        private const int TryRecruitInterval = 15051;
        private const float RecruitMTBDays = 30f;
    }

}
