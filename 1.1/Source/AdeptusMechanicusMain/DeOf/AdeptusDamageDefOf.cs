using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class AdeptusDamageDefOf
    {
        static AdeptusDamageDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AdeptusDamageDefOf));
        }
        public static DamageDef OG_WarpStormStrike;

        public static DamageDef OG_E_Distortion_Damage;
        public static DamageDef OG_E_Distortion_Damage_Blast;
        public static DamageDef OG_N_Gauss_Damage;

        public static DamageDef OG_Power_Cut;
        public static DamageDef OG_Power_Stab;
        public static DamageDef OG_Power_Blunt;
        public static DamageArmorCategoryDef OG_PowerWeapon;

        public static DamageDef OG_ForceStrike;
        public static DamageDef OG_Force_Cut;
        public static DamageDef OG_Force_Stab;
        public static DamageDef OG_Force_Blunt;
        public static DamageDef OG_Chaos_Deamon_Warpfire;


    }
}
