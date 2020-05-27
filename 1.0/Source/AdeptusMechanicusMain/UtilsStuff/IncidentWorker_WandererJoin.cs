﻿using RimWorld;
using System;
using System.Linq;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x02000353 RID: 851
    public class IncidentWorker_WandererJoin : IncidentWorker
    {
        // Token: 0x06000EB4 RID: 3764 RVA: 0x0006D7D8 File Offset: 0x0006BBD8
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!base.CanFireNowSub(parms))
            {
                return false;
            }
            Map map = (Map)parms.target;
            IntVec3 intVec;
            return this.TryFindEntryCell(map, out intVec);
        }

        // Token: 0x06000EB5 RID: 3765 RVA: 0x0006D808 File Offset: 0x0006BC08
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            IntVec3 loc;
            if (!this.TryFindEntryCell(map, out loc))
            {
                return false;
            }
            Gender? gender = null;
            if (this.def.pawnFixedGender != Gender.None)
            {
                gender = new Gender?(this.def.pawnFixedGender);
            }
            PawnKindDef pawnKind = this.def.pawnKind;
            Faction ofPlayer = Faction.OfPlayer;

        //        Log.Message(string.Format("{0}", ofPlayer.def.defName));
                var list = (from def in DefDatabase<PawnKindDef>.AllDefs
                            where ((def.race == ofPlayer.def.basicMemberKind.race) && (def.defName.Contains("StrangerInBlack")))
                            select def).ToList();
                if (list.Count > 0)
                {
                    pawnKind = list.RandomElement<PawnKindDef>();
                    pawnKind.defaultFactionType = ofPlayer.def;
                }

    //        Log.Message(string.Format("{0}", pawnKind.defName));
            bool pawnMustBeCapableOfViolence = this.def.pawnMustBeCapableOfViolence;
            Gender? fixedGender = gender;
            PawnGenerationRequest request = new PawnGenerationRequest(pawnKind, ofPlayer, PawnGenerationContext.NonPlayer, -1, true, false, false, false, true, pawnMustBeCapableOfViolence, 20f, false, true, true, false, false, false, false, null, null, null, null, null, fixedGender, null, null);
            Pawn pawn = PawnGenerator.GeneratePawn(request);
            GenSpawn.Spawn(pawn, loc, map, WipeMode.Vanish);
            string text = this.def.letterText.Formatted(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN");
            string label = this.def.letterLabel.Formatted(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN");
            PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref text, ref label, pawn);
            Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.PositiveEvent, pawn, null, null);
            return true;
        }

        // Token: 0x06000EB6 RID: 3766 RVA: 0x0006D95C File Offset: 0x0006BD5C
        private bool TryFindEntryCell(Map map, out IntVec3 cell)
        {
            return CellFinder.TryFindRandomEdgeCellWith((IntVec3 c) => map.reachability.CanReachColony(c) && !c.Fogged(map), map, CellFinder.EdgeRoadChance_Neutral, out cell);
        }

        // Token: 0x04000951 RID: 2385
        private const float RelationWithColonistWeight = 20f;
    }
}
