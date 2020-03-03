using System.Collections.Generic;
using Verse;

namespace AdeptusMechanicus
{
    public class CompProperties_SlotLoadable : CompProperties
    {
        public bool gizmosOnEquip = true;

        public List<SlotLoadableDef> slots = new List<SlotLoadableDef>();

        public CompProperties_SlotLoadable()
        {
            compClass = typeof(CompSlotLoadable);
        }
    }
}