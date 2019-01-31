using CompSlotLoadable;
using Verse;
using RimWorld;

namespace AdeptusMechanicus
{
    public class CompProperties_RocketSlotLoadable : CompProperties_SlotLoadable
    {
        public CompProperties_RocketSlotLoadable() => this.compClass = typeof(CompRocketSlotLoadable);

    }

    public class CompRocketSlotLoadable : CompSlotLoadable.CompSlotLoadable
    {
        public override void TryEmptySlot(SlotLoadable slot)
        {
            //Log.Message("1");
            if (parent is ThingWithComps compOwner)
            {
                //Log.Message("2"); 

                return;
            }

            base.TryEmptySlot(slot);
        }
    }
}
