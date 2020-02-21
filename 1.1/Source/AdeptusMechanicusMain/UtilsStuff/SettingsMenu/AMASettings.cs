using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Verse;
using RimWorld;

namespace AdeptusMechanicus.settings
{
    public class AMASettings : ModSettings
    {
        public bool 
            ShowSpecialRules = false,
            AllowDeepStrike = true,
            AllowInfiltrate = true
            ;

        public bool 
            ShowWeaponSpecialRules = false, 
            AllowRapidFire = true, 
            AllowGetsHot = true, 
            AllowJams = true, 
            AllowMultiShot = true, 
            AllowUserEffects = true, 
            AllowForceWeaponEffect = true, 
            AllowRendingMeleeEffect = true,
            AllowRendingRangedEffect = true;

        public bool 
            ShowAllowedWeapons = false, 
            AllowImperialWeapons = true, 
            AllowMechanicusWeapons = true, 
            AllowChaosWeapons = true, 
            AllowEldarWeapons = true, 
            AllowDarkEldarWeapons = true, 
            AllowTauWeapons = true, 
            AllowOrkWeapons = true, 
            AllowNecronWeapons = true, 
            AllowTyranidWeapons = false;
        
        public AMASettings()
        {
            AMASettings.Instance = this;
        }

        public static AMASettings Instance;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.ShowSpecialRules, "AMA_ShowSpecialRules", false);
            Scribe_Values.Look(ref this.AllowDeepStrike, "AMA_AllowRapidFire", true);
            Scribe_Values.Look(ref this.AllowInfiltrate, "AMA_AllowGetsHot", true);

            Scribe_Values.Look(ref this.ShowWeaponSpecialRules, "AMA_ShowWeaponSpecialRules", false);
            Scribe_Values.Look(ref this.AllowRapidFire, "AMA_AllowRapidFire", true);
            Scribe_Values.Look(ref this.AllowGetsHot, "AMA_AllowGetsHot", true);
            Scribe_Values.Look(ref this.AllowJams, "AMA_AllowJams", true);
            Scribe_Values.Look(ref this.AllowMultiShot, "AMA_AllowMultiShot", true);
            Scribe_Values.Look(ref this.AllowUserEffects, "AMA_AllowUserEffects", true);
            Scribe_Values.Look(ref this.AllowForceWeaponEffect, "AMA_AllowForceWeaponEffect", true);
            Scribe_Values.Look(ref this.AllowRendingMeleeEffect, "AMA_AllowRendingMeleeEffect", true);
            Scribe_Values.Look(ref this.AllowRendingRangedEffect, "AMA_AllowRendingRangedEffect", true);

            Scribe_Values.Look(ref this.ShowAllowedWeapons, "AMA_ShowAllowedWeapons", false);
            Scribe_Values.Look(ref this.AllowImperialWeapons, "AMA_AllowImperialWeapons", true);
            Scribe_Values.Look(ref this.AllowMechanicusWeapons, "AMA_AllowMechanicusWeapons", true);
            Scribe_Values.Look(ref this.AllowChaosWeapons, "AMA_AllowChaosWeapons", true);
            Scribe_Values.Look(ref this.AllowEldarWeapons, "AMA_AllowEldarWeapons", true);
            Scribe_Values.Look(ref this.AllowDarkEldarWeapons, "AMA_AllowDarkEldarWeapons", true);
            Scribe_Values.Look(ref this.AllowTauWeapons, "AMA_AllowTauWeapons", true);
            Scribe_Values.Look(ref this.AllowOrkWeapons, "AMA_AllowOrkWeapons", true);
            Scribe_Values.Look(ref this.AllowNecronWeapons, "AMA_AllowNecronWeapons", true);
            Scribe_Values.Look(ref this.AllowTyranidWeapons, "AMA_AllowTyranidWeapons", false);
        }


    }
}