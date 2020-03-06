using System;

namespace AdeptusMechanicus.settings
{
    // Token: 0x020001D0 RID: 464
    public class ArmourySettingsRef
    {
        // Weapons Special Rules Options
        public bool AllowRapidFire = AMSettings.Instance.AllowRapidFire;
        public bool AllowGetsHot = AMSettings.Instance.AllowGetsHot;
        public bool AllowJams = AMSettings.Instance.AllowJams;
        public bool AllowUserEffects = AMSettings.Instance.AllowUserEffects;
        public bool AllowMultiShot = AMSettings.Instance.AllowMultiShot;
        public bool AllowForceWeaponEffect = AMSettings.Instance.AllowForceWeaponEffect;
        // Faction Weapon Options
        public bool AllowImperial = AMSettings.Instance.AllowImperialWeapons;
        public bool AllowMechanicus = AMSettings.Instance.AllowMechanicusWeapons;
        public bool AllowChaos = AMSettings.Instance.AllowChaosWeapons;
        public bool AllowDarkEldar = AMSettings.Instance.AllowDarkEldarWeapons;
        public bool AllowEldar = AMSettings.Instance.AllowEldarWeapons;
        public bool AllowTau = AMSettings.Instance.AllowTauWeapons;
        public bool AllowOrk = AMSettings.Instance.AllowOrkWeapons;
        public bool AllowNecron = AMSettings.Instance.AllowNecronWeapons;
        public bool AllowTyranid = AMSettings.Instance.AllowTyranidWeapons;
    }
}
