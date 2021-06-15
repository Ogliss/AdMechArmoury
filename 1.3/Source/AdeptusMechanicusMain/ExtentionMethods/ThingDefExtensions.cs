using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus.ExtensionMethods
{
    public static class ThingDefExtensions
    {

        public static bool powerWeapon(this ThingDef def)
        {
            return def.tools.Any(x => x.capacities.Any(z => z.Maneuvers.Any(y => y.verb.meleeDamageDef.powerWeapon())));
        }

        public static bool witchbladeWeapon(this ThingDef def)
        {
            return def.tools.Any(x => x.capacities.Any(z => z.Maneuvers.Any(y => y.verb.meleeDamageDef.witchbladeWeapon())));
        }

        public static bool forceWeapon(this ThingDef def)
        {
            return def.tools.Any(x => x.capacities.Any(z => z.Maneuvers.Any(y => y.verb.meleeDamageDef.forceWeapon())));
        }

        public static bool rendingWeapon(this ThingDef def)
        {
            return def.tools.Any(x=> x.capacities.Any(z=> z.Maneuvers.Any(y=> y.verb.meleeDamageDef.rendingWeapon())));
        }

    }
}
