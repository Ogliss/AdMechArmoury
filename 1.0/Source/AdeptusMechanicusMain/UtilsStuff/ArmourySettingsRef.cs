using System;

namespace AdeptusMechanicus.settings
{
    // Token: 0x020001D0 RID: 464
    public class ArmourySettingsRef
    {
        // Weapons Special Rules Options
        public bool AllowRapidFire = AMASettings.Instance.AllowRapidFire;
        public bool AllowGetsHot = AMASettings.Instance.AllowGetsHot;
        public bool AllowJams = AMASettings.Instance.AllowJams;
        public bool AllowUserEffects = AMASettings.Instance.AllowUserEffects;
        public bool AllowMultiShot = AMASettings.Instance.AllowMultiShot;
        public bool AllowForceWeaponEffect = AMASettings.Instance.AllowForceWeaponEffect;
        // Faction Weapon Options
        public bool AllowImperial = AMASettings.Instance.AllowImperialWeapons;
        public bool AllowMechanicus = AMASettings.Instance.AllowMechanicusWeapons;
        public bool AllowChaos = AMASettings.Instance.AllowChaosWeapons;
        public bool AllowDarkEldar = AMASettings.Instance.AllowDarkEldarWeapons;
        public bool AllowEldar = AMASettings.Instance.AllowEldarWeapons;
        public bool AllowTau = AMASettings.Instance.AllowTauWeapons;
        public bool AllowOrk = AMASettings.Instance.AllowOrkWeapons;
        public bool AllowNecron = AMASettings.Instance.AllowNecronWeapons;
        public bool AllowTyranid = AMASettings.Instance.AllowTyranidWeapons;
    }
}
