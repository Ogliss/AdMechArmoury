using System;
using System.Collections.Generic;
using AdeptusMechanicus.Harmony;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    public class CompProperties_PowerArmour : CompProperties
    {
        public CompProperties_PowerArmour()
        {
            this.compClass = typeof(CompPowerArmour);
        }
    }

    // Token: 0x02000002 RID: 2
    public class CompPowerArmour : ThingComp
    {
        public CompProperties_PowerArmour Props => (CompProperties_PowerArmour)props;
        private Pawn lastWearer;
        private BodyTypeDef bodyTypeDef;

        // Determine who is wearing this ThingComp. Returns a Pawn or null.
        protected virtual Pawn GetWearer
        {
            get
            {
                if (ParentHolder != null && ParentHolder is Pawn_ApparelTracker)
                {
                    return (Pawn)ParentHolder.ParentHolder;
                }
                else
                {
                    return null;
                }
            }
        }

        // Determine if this ThingComp is being worn presently. Returns True/False
        protected virtual bool IsWorn => (GetWearer != null);

        public void Remove(Pawn pawn)
        {
            Log.Message(string.Format("Off"));
            if (pawn != null)
            {
               HarmonyPatchesOG.Patch.ChangeBodyType(pawn, bodyTypeDef);
            }
        }

        public bool Add(Pawn pawn)
        {
            Log.Message(string.Format("On"));
            // Sanity test; if our pawn doesn't exist, don't even bother.
            if (pawn == null) return false;
            bodyTypeDef = pawn.story.bodyType;
            HarmonyPatchesOG.Patch.ChangeBodyType(pawn, RimWorld.BodyTypeDefOf.Hulk);
            return true;
        }

        public override void CompTick()
        {
            base.CompTick();

            // We know our parent is an Apparel; cast it as such so we can access its Wearer member.
            Apparel parent = base.parent as Apparel;
            Log.Message(string.Format("tick"));
            // We only need to do something if our wearer has changed.
            if (GetWearer != lastWearer)
            {
                Log.Message(string.Format("tock"));
                // It has, so remove our effects from the last wearer and apply them to the new one.
                Remove(lastWearer);
                Add(GetWearer);
                // Update our wearer so we don't run code too often.
                lastWearer = GetWearer;
                // Set our last recorded durability to some impossible value to force an update.
            }
            
        }
    }
}
