using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace AdeptusMechanicus
{
    public class SloatLoadbleFloatMenuPatch : FloatMenuPatch
    {
        public override IEnumerable<KeyValuePair<_Condition, Func<Vector3, Pawn, Thing, List<FloatMenuOption>>>>
            GetFloatMenus()
        {
            var FloatMenus = new List<KeyValuePair<_Condition, Func<Vector3, Pawn, Thing, List<FloatMenuOption>>>>();

            var curCondition = new _Condition(_ConditionType.ThingHasComp, typeof(CompSlottedBonus));

            List<FloatMenuOption> CurFunc(Vector3 clickPos, Pawn pawn, Thing curThing)
            {
                //Log.Message("Patch is loaded");
                var opts = new List<FloatMenuOption>();
                List<IThingHolder> holders = new List<IThingHolder>();
                pawn.GetChildHolders(holders);
                var allThings = new List<Thing>();
                holders.ForEach(x => allThings.AddRange(x.GetDirectlyHeldThings().ToList()));
                foreach (var item in allThings)
                {
                    if (item is ThingWithComps slotLoadable &&
                        slotLoadable.AllComps.FirstOrDefault(x => x is CompSlotLoadable) is CompSlotLoadable
                            compSlotLoadable)
                    {
                        var c = clickPos.ToIntVec3();
                        //var thingList = c.GetThingList(pawn.Map);

                        foreach (var slot in compSlotLoadable.Slots)
                        {
                            var loadableThing = (slot.CanLoad(curThing.def)) ? curThing : null ;
                            if (loadableThing != null)
                            {
                                FloatMenuOption itemSlotLoadable;
                                var labelShort = loadableThing.Label;
                                if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
                                {
                                    itemSlotLoadable = new FloatMenuOption(
                                        "CannotEquip".Translate(labelShort) + " (" + "Incapable".Translate() + ")",
                                        null,
                                        MenuOptionPriority.Default, null, null, 0f, null, null);
                                }
                                else if (!pawn.CanReach(loadableThing, PathEndMode.ClosestTouch, Danger.Deadly))
                                {
                                    itemSlotLoadable = new FloatMenuOption(
                                        "CannotEquip".Translate(labelShort) + " (" + "NoPath".Translate() + ")", null,
                                        MenuOptionPriority.Default, null, null, 0f, null, null);
                                }
                                else if (!pawn.CanReserve(loadableThing, 1))
                                {
                                    itemSlotLoadable = new FloatMenuOption(
                                        "CannotEquip".Translate(labelShort) + " (" +
                                        "ReservedBy".Translate(pawn.Map.physicalInteractionReservationManager
                                            .FirstReserverOf(loadableThing).LabelShort) + ")", null,
                                        MenuOptionPriority.Default, null, null, 0f, null, null);
                                }
                                else
                                {
                                    var text2 = "Equip".Translate(labelShort);
                                    itemSlotLoadable = new FloatMenuOption(text2, delegate
                                    {
                                        loadableThing.SetForbidden(false, true);
                                        pawn.jobs.TryTakeOrderedJob(new Job(
                                            DefDatabase<JobDef>.GetNamed("GatherSlotItem"),
                                            loadableThing));
                                        MoteMaker.MakeStaticMote(loadableThing.DrawPos, loadableThing.Map,
                                            ThingDefOf.Mote_FeedbackEquip, 1f);
                                        //PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.EquippingWeapons, KnowledgeAmount.Total);
                                    }, MenuOptionPriority.High, null, null, 0f, null, null);
                                }
                                opts.Add(itemSlotLoadable);
                            }
                        }
                        return opts;
                    }
                }
                return opts;
            }

            KeyValuePair<_Condition, Func<Vector3, Pawn, Thing, List<FloatMenuOption>>> curSec =
                new KeyValuePair<_Condition, Func<Vector3, Pawn, Thing, List<FloatMenuOption>>>(curCondition, CurFunc);
            FloatMenus.Add(curSec);
            return FloatMenus;
        }
    }


    public enum _ConditionType
    {
        IsType,
        IsTypeStringMatch,
        ThingHasComp,
        HediffHasComp
    }

    public struct _Condition : IEquatable<_Condition>
    {
        public _ConditionType Condition;
        public object Data;

        public _Condition(_ConditionType condition, object data)
        {
            Condition = condition;
            Data = data;
        }

        public override string ToString()
        {
            return "Condition_" + Condition + "_" + Data;
        }

        public bool Equals(_Condition other)
        {
            return Data == other.Data && Condition == other.Condition;
        }

        public bool Passes(object toCheck)
        {
            //Log.Message(toCheck.GetType().ToString());
            //Log.Message(Data.ToString());

            switch (Condition)
            {
                case _ConditionType.IsType:
                    //////////////////////////
                    ///PSYCHOLOGY SPECIAL CASE
                    if (toCheck.GetType().ToString() == "Psychology.PsychologyPawn" && Data.ToString() == "Verse.Pawn")
                        return true;
                    //////////////////////////
                    if (toCheck.GetType() == Data.GetType() || Equals(toCheck.GetType(), Data) ||
                        toCheck.GetType() == Data || toCheck.GetType().ToString() == Data.ToString() ||
                        Data.GetType().IsInstanceOfType(toCheck))
                        return true;
                    break;
                case _ConditionType.IsTypeStringMatch:
                    if (toCheck.GetType().ToString() == (string)toCheck)
                        return true;
                    break;
                case _ConditionType.ThingHasComp:
                    var dataType = Data;
                    if (toCheck is ThingWithComps t && t?.AllComps?.Count > 0 && Enumerable.Any(t.AllComps, comp =>
                            comp?.props?.compClass?.ToString() == dataType?.ToString() ||
                            comp?.props?.compClass?.BaseType?.ToString() == dataType?.ToString()))
                        return true;
                    break;
            }
            return false;
        }
    }

    public abstract class FloatMenuPatch
    {
        public abstract IEnumerable<KeyValuePair<_Condition, Func<Vector3, Pawn, Thing, List<FloatMenuOption>>>>
            GetFloatMenus();
    }
}