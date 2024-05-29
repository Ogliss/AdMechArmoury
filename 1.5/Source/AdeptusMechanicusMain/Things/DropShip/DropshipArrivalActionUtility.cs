using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace AdeptusMechanicus
{
    public static class DropShipArrivalActionUtility
    {

        public static IEnumerable<FloatMenuOption> GetFloatMenuOptions<T>(Func<FloatMenuAcceptanceReport> acceptanceReportGetter, Func<T> arrivalActionGetter, string label, CompDropship representative, int destinationTile, Caravan car) where T : TransportPodsArrivalAction
        {
            FloatMenuAcceptanceReport rep = acceptanceReportGetter();
            if (rep.Accepted || !rep.FailReason.NullOrEmpty() || !rep.FailMessage.NullOrEmpty())
            {
                if (!rep.FailReason.NullOrEmpty())
                {
                    yield return new FloatMenuOption(label + " (" + rep.FailReason + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
                }
                else
                {
                    yield return new FloatMenuOption(label, delegate
                    {
                        FloatMenuAcceptanceReport floatMenuAcceptanceReport = acceptanceReportGetter();
                        if (floatMenuAcceptanceReport.Accepted)
                        {
                            representative.TryLaunch(destinationTile, arrivalActionGetter(), car);
                        }
                        else if (!floatMenuAcceptanceReport.FailMessage.NullOrEmpty())
                        {
                            Messages.Message(floatMenuAcceptanceReport.FailMessage, new GlobalTargetInfo(destinationTile), MessageTypeDefOf.RejectInput, false);
                        }
                    }, MenuOptionPriority.Default, null, null, 0f, null, null);
                }
            }
            yield break;
        }



        public static IEnumerable<FloatMenuOption> GetATKFloatMenuOptions(CompDropship representative, IEnumerable<IThingHolder> pods, Settlement settlement, Caravan car)
        {
            foreach (FloatMenuOption f in DropShipArrivalActionUtility.GetFloatMenuOptions<TransportPodsArrivalAction_AttackSettlement>(() => TransportPodsArrivalAction_AttackSettlement.CanAttack(pods, settlement), () => new TransportPodsArrivalAction_AttackSettlement(settlement, PawnsArrivalModeDefOf.EdgeDrop), "AttackAndDropAtEdge".Translate(settlement.Label), representative, settlement.Tile, car))
            {
                yield return f;
            }
            foreach (FloatMenuOption f2 in DropShipArrivalActionUtility.GetFloatMenuOptions<TransportPodsArrivalAction_AttackSettlement>(() => TransportPodsArrivalAction_AttackSettlement.CanAttack(pods, settlement), () => new TransportPodsArrivalAction_AttackSettlement(settlement, PawnsArrivalModeDefOf.CenterDrop), "AttackAndDropInCenter".Translate(settlement.Label), representative, settlement.Tile, car))
            {
                yield return f2;
            }
            yield break;
        }

        public static IEnumerable<FloatMenuOption> GetGIFTFloatMenuOptions(CompDropship representative, IEnumerable<IThingHolder> pods, Settlement settlement, Caravan car)
        {
            return Enumerable.Empty<FloatMenuOption>();
            /*
            if (settlement.Faction == Faction.OfPlayer)
            {
                return Enumerable.Empty<FloatMenuOption>();
            }
            return DropShipArrivalActionUtility.GetFloatMenuOptions<TransportPodsArrivalAction_GiveGift>(() => TransportPodsArrivalAction_GiveGift.CanGiveGiftTo(pods, settlement),
                () => new TransportPodsArrivalAction_GiveGift(settlement), "GiveGiftViaTransportPods".Translate(settlement.Faction.Name,
                FactionGiftUtility.GetGoodwillChange(pods, settlement).ToStringWithSign()), representative, settlement.Tile, car);
            */
        }

        public static IEnumerable<FloatMenuOption> GetVisitFloatMenuOptions(CompDropship representative, IEnumerable<IThingHolder> pods, Settlement settlement, Caravan car)
        {
            return DropShipArrivalActionUtility.GetFloatMenuOptions<TransportPodsArrivalAction_VisitSettlement>(() => TransportPodsArrivalAction_VisitSettlement.CanVisit(pods, settlement),
                () => new TransportPodsArrivalAction_VisitSettlement(settlement), "VisitSettlement".Translate(settlement.Label), representative, settlement.Tile, car);
        }
    }
}
