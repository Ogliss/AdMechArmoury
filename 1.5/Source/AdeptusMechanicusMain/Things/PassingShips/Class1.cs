using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AdeptusMechanicus
{
    public class TradeShip : RimWorld.TradeShip
    {
		public string Captain;
		public bool BlocksHostileShips => false;
        public new FloatMenuOption CommFloatMenuOption(Building_CommsConsole console, Pawn negotiator)
		{

			string label = "CallOnRadio".Translate(this.GetCallLabel());
			Action action = null;
			AcceptanceReport canCommunicate = this.CanCommunicateWith(negotiator);
			if (!canCommunicate.Accepted)
			{
				if (!canCommunicate.Reason.NullOrEmpty())
				{
					action = delegate ()
					{
						Messages.Message(canCommunicate.Reason, console, MessageTypeDefOf.RejectInput, false);
					};
				}
			}
			else
			{
				action = delegate ()
				{
					console.GiveUseCommsJob(negotiator, this);
				};
			}
			return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(label, action, MenuOptionPriority.InitiateSocial, null, null, 0f, null, null, true, 0), negotiator, console, "ReservedBy");
		}

		public new void TryOpenComms(Pawn negotiator)
		{
			Dialog_Negotiation dialog_Negotiation = new Dialog_Negotiation(negotiator, this, ShipDialogMaker.CommunicationDialogFor(negotiator, this), true);
			dialog_Negotiation.soundAmbient = SoundDefOf.RadioComms_Ambience;
			Find.WindowStack.Add(dialog_Negotiation);
		}

		public void TryOpenTrade(Pawn negotiator)
		{
			if (!this.CanTradeNow)
			{
				return;
			}
			Find.WindowStack.Add(new Dialog_Trade(negotiator, this, false));
			LessonAutoActivator.TeachOpportunity(ConceptDefOf.BuildOrbitalTradeBeacon, OpportunityType.Critical);
			PawnRelationUtility.Notify_PawnsSeenByPlayer_Letter_Send(this.Goods.OfType<Pawn>(), "LetterRelatedPawnsTradeShip".Translate(Faction.OfPlayer.def.pawnsPlural), LetterDefOf.NeutralEvent, false, true);
			TutorUtility.DoModalDialogIfNotKnown(ConceptDefOf.TradeGoodsMustBeNearBeacon, Array.Empty<string>());
        }
	}

	public static class ShipDialogMaker
    {
		public static DiaNode CommunicationDialogFor(Pawn negotiator, TradeShip ship)
		{
			Map map = negotiator.Map;
			Pawn pawn;
			string value;
			Faction faction = ship.Faction;
			if (faction.leader != null)
			{
				pawn = faction.leader;
				value = faction.leader.Name.ToStringFull.Colorize(ColoredText.NameColor);
			}
			else
			{
				Log.Error("Faction " + faction + " has no leader.");
				pawn = negotiator;
				value = faction.Name;
			}
			DiaNode root;
			if (faction.PlayerRelationKind == FactionRelationKind.Hostile)
			{
				string key = (faction.def.permanentEnemy || !"FactionGreetingHostileAppreciative".CanTranslate()) ? "FactionGreetingHostile" : "FactionGreetingHostileAppreciative";
				root = new DiaNode(key.Translate(value).AdjustedFor(pawn));
			}
			else if (faction.PlayerRelationKind == FactionRelationKind.Neutral)
			{
				root = new DiaNode("FactionGreetingWary".Translate(value, negotiator.LabelShort, negotiator.Named("NEGOTIATOR"), pawn.Named("LEADER")).AdjustedFor(pawn));
			}
			else
			{
				root = new DiaNode("FactionGreetingWarm".Translate(value, negotiator.LabelShort, negotiator.Named("NEGOTIATOR"), pawn.Named("LEADER")).AdjustedFor(pawn));
			}
			if (map != null && map.IsPlayerHome)
			{
				AddAndDecorateOption(RequestTradeOptions(map, ship, negotiator), needsSocial: true);
				AddAndDecorateOption(RequestMilitaryAidOptions(map, ship, negotiator), needsSocial: true);
				Pawn_RoyaltyTracker royalty = negotiator.royalty;
				if (royalty != null && royalty.HasAnyTitleIn(faction))
				{
					foreach (RoyalTitle item in royalty.AllTitlesInEffectForReading)
					{
						if (item.def.permits != null)
						{
							foreach (RoyalTitlePermitDef permit in item.def.permits)
							{
								IEnumerable<DiaOption> factionCommDialogOptions = permit.Worker.GetFactionCommDialogOptions(map, negotiator, faction);
								if (factionCommDialogOptions != null)
								{
									foreach (DiaOption item2 in factionCommDialogOptions)
									{
										AddAndDecorateOption(item2, needsSocial: true);
									}
								}
							}
						}
					}
					if (royalty.GetCurrentTitle(faction).canBeInherited && !negotiator.IsQuestLodger())
					{
						AddAndDecorateOption(FactionDialogMaker.RequestRoyalHeirChangeOption(map, faction, pawn, negotiator), needsSocial: false);
					}
				}
				if (DefDatabase<ResearchProjectDef>.AllDefsListForReading.Any((ResearchProjectDef rp) => rp.HasTag(ResearchProjectTagDefOf.ShipRelated) && rp.IsFinished))
				{
					AddAndDecorateOption(FactionDialogMaker.RequestAICoreQuest(map, faction, negotiator), needsSocial: true);
				}
			}
			if (Prefs.DevMode)
			{
				foreach (DiaOption item3 in FactionDialogMaker.DebugOptions(faction, negotiator))
				{
					AddAndDecorateOption(item3, needsSocial: false);
				}
			}
			AddAndDecorateOption(new DiaOption("(" + "Disconnect".Translate() + ")")
			{
				resolveTree = true
			}, needsSocial: false);
			return root;
			void AddAndDecorateOption(DiaOption opt, bool needsSocial)
			{
				if (needsSocial && negotiator.skills.GetSkill(SkillDefOf.Social).TotallyDisabled)
				{
					opt.Disable("WorkTypeDisablesOption".Translate(SkillDefOf.Social.label));
				}
				root.options.Add(opt);
			}
		}

		private static DiaOption RequestTradeOptions(Map map, TradeShip ship, Pawn negotiator)
		{
			Faction faction = ship.Faction;
			TaggedString taggedString = "RequestTrader".Translate(15);
			if (faction.PlayerRelationKind != FactionRelationKind.Ally)
			{
				DiaOption diaOption = new DiaOption(taggedString);
				diaOption.Disable("MustBeAlly".Translate());
				return diaOption;
			}
			if (!Building_OrbitalTradeBeacon.AllPowered(map).Any<Building_OrbitalTradeBeacon>())
			{
				DiaOption diaOption = new DiaOption(taggedString);
				diaOption.Disable("MessageNeedBeaconToTradeWithShip".Translate());
				return diaOption;

			}
			/*
			int num = faction.lastTraderRequestTick + 240000 - Find.TickManager.TicksGame;
			if (num > 0)
			{
				DiaOption diaOption3 = new DiaOption(taggedString);
				diaOption3.Disable("WaitTime".Translate(num.ToStringTicksToPeriod(true, false, true, true)));
				return diaOption3;
			}
			*/
			DiaOption diaOption4 = new DiaOption(taggedString);
			DiaNode diaNode = new DiaNode("TraderSent".Translate(faction.leader).CapitalizeFirst());
			diaNode.options.Add(FactionDialogMaker.OKToRoot(faction, negotiator));
			DiaNode diaNode2 = new DiaNode("ChooseTraderKind".Translate(faction.leader));
			foreach (TraderKindDef localTk2 in from x in faction.def.caravanTraderKinds
											   where x.requestable
											   select x)
			{
				TraderKindDef localTk = localTk2;
				DiaOption diaOption5 = new DiaOption(localTk.LabelCap);
				if (localTk.TitleRequiredToTrade != null && (negotiator.royalty == null || localTk.TitleRequiredToTrade.seniority > negotiator.GetCurrentTitleSeniorityIn(faction)))
				{
					DiaNode diaNode3 = new DiaNode("TradeCaravanRequestDeniedDueTitle".Translate(negotiator.Named("NEGOTIATOR"), localTk.TitleRequiredToTrade.GetLabelCapFor(negotiator).Named("TITLE"), faction.Named("FACTION")));
					DiaOption diaOption6 = new DiaOption("GoBack".Translate());
					diaNode3.options.Add(diaOption6);
					diaOption5.link = diaNode3;
					diaOption6.link = diaNode2;
				}
				else
				{
					diaOption5.action = delegate ()
					{
						IncidentParms incidentParms = new IncidentParms();
						incidentParms.target = map;
						incidentParms.faction = faction;
						incidentParms.traderKind = localTk;
						incidentParms.forced = true;
						Find.Storyteller.incidentQueue.Add(IncidentDefOf.TraderCaravanArrival, Find.TickManager.TicksGame + 120000, incidentParms, 240000);
						faction.lastTraderRequestTick = Find.TickManager.TicksGame;
						Faction.OfPlayer.TryAffectGoodwillWith(faction, -15, false, true, HistoryEventDefOf.RequestedTrader, null);
					};
					diaOption5.link = diaNode;
				}
				diaNode2.options.Add(diaOption5);
			}
			DiaOption diaOption7 = new DiaOption("GoBack".Translate());
			diaOption7.linkLateBind = FactionDialogMaker.ResetToRoot(faction, negotiator);
			diaNode2.options.Add(diaOption7);
			diaOption4.link = diaNode2;
			return diaOption4;
		}

		private static DiaOption RequestMilitaryAidOptions(Map map, TradeShip ship, Pawn negotiator)
		{
			Faction faction = ship.Faction;
			string text = "RequestMilitaryAid".Translate(25);
			if (faction.PlayerRelationKind != FactionRelationKind.Ally)
			{
				DiaOption diaOption = new DiaOption(text);
				diaOption.Disable("MustBeAlly".Translate());
				return diaOption;
			}
			if (!faction.def.allowedArrivalTemperatureRange.ExpandedBy(-4f).Includes(map.mapTemperature.SeasonalTemp))
			{
				DiaOption diaOption2 = new DiaOption(text);
				diaOption2.Disable("BadTemperature".Translate());
				return diaOption2;
			}
			int num = faction.lastMilitaryAidRequestTick + 60000 - Find.TickManager.TicksGame;
			if (num > 0)
			{
				DiaOption diaOption3 = new DiaOption(text);
				diaOption3.Disable("WaitTime".Translate(num.ToStringTicksToPeriod(true, false, true, true)));
				return diaOption3;
			}
			if (NeutralGroupIncidentUtility.AnyBlockingHostileLord(map, faction))
			{
				DiaOption diaOption4 = new DiaOption(text);
				diaOption4.Disable("HostileVisitorsPresent".Translate());
				return diaOption4;
			}
			DiaOption diaOption5 = new DiaOption(text);
			if (faction.def.techLevel < TechLevel.Industrial)
			{
				diaOption5.link = FactionDialogMaker.CantMakeItInTime(faction, negotiator);
			}
			else
			{
				IEnumerable<Faction> source = (from x in map.attackTargetsCache.TargetsHostileToColony
											   where GenHostility.IsActiveThreatToPlayer(x)
											   select ((Thing)x).Faction into x
											   where x != null && !x.HostileTo(faction)
											   select x).Distinct<Faction>();
				if (source.Any<Faction>())
				{
					DiaNode diaNode = new DiaNode("MilitaryAidConfirmMutualEnemy".Translate(faction.Name, (from fa in source
																										   select fa.Name).ToCommaList(true, false)));
					DiaOption diaOption6 = new DiaOption("CallConfirm".Translate());
					diaOption6.action = delegate ()
					{
						FactionDialogMaker.CallForAid(map, faction);
					};
					diaOption6.link = FactionDialogMaker.FightersSent(faction, negotiator);
					DiaOption diaOption7 = new DiaOption("CallCancel".Translate());
					diaOption7.linkLateBind = FactionDialogMaker.ResetToRoot(faction, negotiator);
					diaNode.options.Add(diaOption6);
					diaNode.options.Add(diaOption7);
					diaOption5.link = diaNode;
				}
				else
				{
					diaOption5.action = delegate ()
					{
						FactionDialogMaker.CallForAid(map, faction);
					};
					diaOption5.link = FactionDialogMaker.FightersSent(faction, negotiator);
				}
			}
			return diaOption5;
		}
	}
}
