using System.Collections.Generic;
using Verse;
using HarmonyLib;
using System.Reflection.Emit;
using RimWorld;

namespace AdeptusMechanicus.HarmonyInstance
{
    //    [HarmonyPatch(typeof(PawnObserver), "ObserveSurroundingThings")]
    public static class PawnObserver_ObserveSurroundingThings_CompilerGenerated_ObserveablePawn_Transpiler
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            
            for (int i = 0; i < instructionsList.Count; i++)
            {
				CodeInstruction instruction = instructionsList[i];
				if (instruction.opcode == OpCodes.Ret)
                {
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Ldarg_1);
					yield return new CodeInstruction(OpCodes.Call, typeof(PawnObserver_ObserveSurroundingThings_CompilerGenerated_ObserveablePawn_Transpiler).GetMethod("ObservePawms"));
				}
				yield return instruction;
			}
            
        }
        public static void ObservePawms(PawnObserver observer, Region reg)
        {
            foreach (Thing thing in reg.ListerThings.ThingsInGroup(ThingRequestGroup.Pawn))
            {
                if (observer.PossibleToObserve(thing))
                {
                    TryCreateObservedThought(observer, thing);
                    TryCreateObservedHistoryEvent(observer, thing);
                }
            }
		}
		private static void TryCreateObservedThought(PawnObserver observer, Thing thing)
		{
			Thought_MemoryObservationTerror item;
			if (TerrorUtility.TryCreateTerrorThought(thing, out item))
			{
				observer.terrorThoughts.Add(item);
			}
			IObservedThoughtGiver observedThoughtGiver;
			if ((observedThoughtGiver = (thing as IObservedThoughtGiver)) != null)
			{
				Thought_Memory thought_Memory = observedThoughtGiver.GiveObservedThought(observer.pawn);
				if (thought_Memory != null)
				{
					observer.pawn.needs.mood.thoughts.memories.TryGainMemory(thought_Memory, null);
				}
			}
		}

		private static void TryCreateObservedHistoryEvent(PawnObserver observer, Thing thing)
		{
			IObservedThoughtGiver observedThoughtGiver;
			if ((observedThoughtGiver = (thing as IObservedThoughtGiver)) != null)
			{
				HistoryEventDef historyEventDef = observedThoughtGiver.GiveObservedHistoryEvent(observer.pawn);
				if (historyEventDef != null)
				{
					HistoryEvent historyEvent = new HistoryEvent(historyEventDef, observer.pawn.Named(HistoryEventArgsNames.Doer), thing.Named(HistoryEventArgsNames.Subject));
					Find.HistoryEventsManager.RecordEvent(historyEvent, true);
				}
			}
		}
    }
    
}
