using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus.ExtensionMethods
{
    public static class Pawn_AbilityTrackerExtensions
    {
        /*
        public static void GainEquipmentAbility(this Pawn_AbilityTracker tracker, EquipmentAbilityDef def, ThingWithComps thing)
        {
            if (!tracker.abilities.Any((Ability a) => a.def == def))
            {
                EquipmentAbility ab = Activator.CreateInstance(def.abilityClass, new object[]
                {
                    tracker.pawn,
                    def
                }) as EquipmentAbility;
                ab.sourceEquipment = thing;
                tracker.abilities.Add(ab);
            }
        }
        */
    }
}
