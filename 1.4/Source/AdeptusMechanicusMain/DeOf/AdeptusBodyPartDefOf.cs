using RimWorld;
using System;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class AdeptusBodyPartDefOf
    {
        static AdeptusBodyPartDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AdeptusBodyPartDefOf));
        }
        [MayRequireXenobiologis, MayRequireNecrons]
        public static BodyPartDef OG_Necron_PhasicCapacitor;
        [MayRequireXenobiologis, MayRequireNecrons]
        public static BodyPartDef OG_Necron_ReanimationMatrix;
        [MayRequireXenobiologis, MayRequireNecrons]
        public static BodyPartDef OG_Necron_NecrodermisRegulator;

        // Astartes Bodyparts
        [MayRequireAstartes]
        public static BodyPartDef OG_Zygote_Part_SecondaryHeart;
        [MayRequireAstartes]
        public static BodyPartDef OG_Zygote_Part_Ossmodula;
        [MayRequireAstartes]
        public static BodyPartDef OG_Zygote_Part_Biscopea;
        [MayRequireAstartes]
        public static BodyPartDef OG_Zygote_Part_Haemastamen;
        [MayRequireAstartes]
        public static BodyPartDef OG_Zygote_Part_LarramansOrgan;
        [MayRequireAstartes]
        public static BodyPartDef OG_Zygote_Part_CatalepseanNode;
        [MayRequireAstartes]
        public static BodyPartDef OG_Zygote_Part_Preomnor;
        [MayRequireAstartes]
        public static BodyPartDef OG_Zygote_Part_Omophagea;
        [MayRequireAstartes]
        public static BodyPartDef OG_Zygote_Part_MultiLung;
        [MayRequireAstartes]
        public static BodyPartDef OG_Zygote_Part_Occulobe;
        [MayRequireAstartes]
        public static BodyPartDef OG_Zygote_Part_LymansEar;
        [MayRequireAstartes]
        public static BodyPartDef OG_Zygote_Part_SusanMembrane;
        [MayRequireAstartes]
        public static BodyPartDef OG_Zygote_Part_Melanochrome;
        [MayRequireAstartes]
        public static BodyPartDef OG_Zygote_Part_OoliticKidney;
        [MayRequireAstartes]
        public static BodyPartDef OG_Zygote_Part_Neuroglottis;
        [MayRequireAstartes]
        public static BodyPartDef OG_Zygote_Part_Mucranoid;
        [MayRequireAstartes]
        public static BodyPartDef OG_Zygote_Part_BetchersGland;
        [MayRequireAstartes]
        public static BodyPartDef OG_Zygote_Part_ProgenoidGlandN;
        [MayRequireAstartes]
        public static BodyPartDef OG_Zygote_Part_ProgenoidGlandC;
        [MayRequireAstartes]
        public static BodyPartDef OG_Zygote_Part_TheBlackCarapace;

    }
}
