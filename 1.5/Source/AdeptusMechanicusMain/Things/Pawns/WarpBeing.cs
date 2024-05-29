using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    public class WarpBeing : Pawn, IObservedThoughtGiver
    {
        public new HistoryEventDef GiveObservedHistoryEvent(Pawn observer)
        {
            return null;
        }

        public new Thought_Memory GiveObservedThought(Pawn observer)
        {
            Thought_MemoryObservation thought_MemoryObservation;
            ThoughtDef defToImplement = AdeptusThoughtDefOf.OG_ObservedWarpBeing;
            if (defToImplement == null) return null;
            thought_MemoryObservation = (Thought_MemoryObservation)ThoughtMaker.MakeThought(defToImplement);
            thought_MemoryObservation.Target = this;
            return thought_MemoryObservation;
        }
    }
}
