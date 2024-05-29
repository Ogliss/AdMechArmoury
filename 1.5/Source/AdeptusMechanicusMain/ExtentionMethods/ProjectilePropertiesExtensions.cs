using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus.ExtensionMethods
{
    public static class ProjectilePropertiesExtensions
    {
        public static bool Melta(this ProjectileProperties projectile)
        {

            DamageDef damage = projectile.damageDef;
            return damage.defName.Contains("OG") && damage.defName.Contains("Melta");
        }

        public static bool Volkite(this ProjectileProperties projectile)
        {
            DamageDef damage = projectile.damageDef;
            return damage.defName.Contains("OG") && damage.defName.Contains("Volkite");
        }

        public static bool Haywire(this ProjectileProperties projectile)
        {
            DamageDef damage = projectile.damageDef;
            return damage.defName.Contains("OG") && damage.defName.Contains("Haywire");
        }

        public static bool Conversion(this ProjectileProperties projectile)
        {
            DamageDef damage = projectile.damageDef;
            return damage.defName.Contains("OG") && damage.defName.Contains("Conversion");
        }

        public static bool Las(this ProjectileProperties projectile)
        {
            DamageDef damage = projectile.damageDef;
            return damage.defName.Contains("OG") && damage.defName.Contains("Las");
        }

        public static bool Bolt(this ProjectileProperties projectile)
        {
            DamageDef damage = projectile.damageDef;
            return damage.defName.Contains("OG") && damage.defName.Contains("Bolt");
        }

        public static bool Plasma(this ProjectileProperties projectile)
        {
            DamageDef damage = projectile.damageDef;
            return damage.defName.Contains("OG") && damage.defName.Contains("Plasma");
        }

        public static bool Distortion(this ProjectileProperties projectile)
        {
            DamageDef damage = projectile.damageDef;
            return damage.defName.Contains("OG") && damage.defName.Contains("Distortion");
        }

        public static bool Gauss(this ProjectileProperties projectile)
        {
            DamageDef damage = projectile.damageDef;
            return damage.defName.Contains("OG") && damage.defName.Contains("Gauss");
        }

        public static bool Tesla(this ProjectileProperties projectile)
        {
            DamageDef damage = projectile.damageDef;
            return damage.defName.Contains("OG") && damage.defName.Contains("Tesla");
        }

    }

}
