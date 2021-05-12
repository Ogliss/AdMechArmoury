using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class AdeptusSoundDefOf
    {
        static AdeptusSoundDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AdeptusSoundDefOf));
        }
        public static SoundDef Interact_Research;
        public static SoundDef EmpDisabled;
        public static SoundDef MeleeHit_Metal_RendingWeapon;
        public static SoundDef MeleeHit_Metal_PowerWeapon;
        public static SoundDef MeleeHit_Metal_ForceWeapon;
        public static SoundDef MeleeHit_Metal_ThunderHammer;

    }
}
