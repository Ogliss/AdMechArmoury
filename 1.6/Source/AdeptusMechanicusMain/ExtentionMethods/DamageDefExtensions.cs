using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus.ExtensionMethods
{

    public static class DamageDefExtensions
    {
        public static bool powerWeapon(this DamageDef damage)
        {
            return damage.defName.Contains("OG_Power_");
        }

        public static bool witchbladeWeapon(this DamageDef damage)
        {
            return damage.defName.Contains("OG_Witchblade_");
        }

        public static bool forceWeapon(this DamageDef damage)
        {
            return damage.defName.Contains("OG_Force_");
        }

        public static bool rendingWeapon(this DamageDef damage)
        {
            return damage.defName.Contains("OG_Rending_");
        }

    }

}
