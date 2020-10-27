using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class OGSoundDefOf
    {
        static OGSoundDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(OGSoundDefOf));
        }
        public static SoundDef MeleeHit_Metal_RendingWeapon;
        public static SoundDef MeleeHit_Metal_PowerWeapon;
        public static SoundDef MeleeHit_Metal_ForceWeapon;
        public static SoundDef MeleeHit_Metal_ThunderHammer;

    }
}
