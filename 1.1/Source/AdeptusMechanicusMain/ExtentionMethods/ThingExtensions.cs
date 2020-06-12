using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus.ExtensionMethods
{
    public static class ThingExtensions
    {
        public static bool powerWeapon(this Thing thing)
        {
            bool flag1 = thing.TryGetComp<CompEquippable>() != null;
            bool flag2 = thing.def.IsMeleeWeapon;
            bool flag3 = thing.TryGetComp<CompEquippable>().AllVerbs.Any(x => x.powerWeapon());
            return flag1 && flag2 && flag3;
        }

        public static bool witchbladeWeapon(this Thing thing)
        {
            bool flag1 = thing.TryGetComp<CompEquippable>() != null;
            bool flag2 = thing.def.IsMeleeWeapon;
            bool flag3 = thing.TryGetComp<CompEquippable>().AllVerbs.Any(x => x.witchbladeWeapon());
            return flag1 && flag2 && flag3;
        }

        public static bool forceWeapon(this Thing thing)
        {
            bool flag1 = thing.TryGetComp<CompEquippable>() != null;
            bool flag2 = thing.def.IsMeleeWeapon;
            bool flag3 = thing.TryGetComp<CompEquippable>().AllVerbs.Any(x => x.forceWeapon());
            return flag1 && flag2 && flag3;
        }

        public static bool rendingWeapon(this Thing thing)
        {
            bool flag1 = thing.TryGetComp<CompEquippable>() != null;
            bool flag2 = thing.def.IsMeleeWeapon;
            bool flag3 = thing.TryGetComp<CompEquippable>().AllVerbs.Any(x => x.rendingWeapon());
            return flag1 && flag2 && flag3;
        }

    }
}
