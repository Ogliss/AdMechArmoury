using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class AdeptusHediffDefOf
    {
        static AdeptusHediffDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AdeptusHediffDefOf));
        }
        public static HediffDef MissingBodyPart;
        public static HediffDef OG_Hediff_Regenerated_Part;
        public static HediffDef OG_Hediff_Regenerating_Part;
        public static HediffDef OG_Hediff_PlasmaBurn;
        public static HediffDef OG_Hediff_RadiationPoisioning;
        public static HediffDef OG_Hediff_FWPsychicShock;
        [MayRequireXenobiologis]
        public static HediffDef OG_Regenerating;
        [MayRequireXenobiologis]
        public static HediffDef OG_Regenerated;
        [MayRequireXenobiologis]
        public static HediffDef OG_Necron_Upgrade_RessurectionOrb;
        [MayRequireXenobiologis]
        public static HediffDef OG_Necron_Upgrade_Phylactery;
        [MayRequireXenobiologis]
        public static HediffDef OG_Necron_Upgrade_VeilOfDarkness;
        [MayRequireXenobiologis]
        public static HediffDef OG_Necron_Upgrade_PhaseShifter;


        // Astartes HeDiffDefs
        // Astartes Implants
        [MayRequireAstartes]
        public static HediffDef OG_Zygote_Hediff_SecondaryHeart;
        [MayRequireAstartes]
        public static HediffDef OG_Zygote_Hediff_Ossmodula;
        [MayRequireAstartes]
        public static HediffDef OG_Zygote_Hediff_Biscopea;
        [MayRequireAstartes]
        public static HediffDef OG_Zygote_Hediff_Haemastamen;
        [MayRequireAstartes]
        public static HediffDef OG_Zygote_Hediff_LarramansOrgan;
        [MayRequireAstartes]
        public static HediffDef OG_Zygote_Hediff_Catalepsean;
        [MayRequireAstartes]
        public static HediffDef OG_Zygote_Hediff_Preomnor;
        [MayRequireAstartes]
        public static HediffDef OG_Zygote_Hediff_Omophagea;
        [MayRequireAstartes]
        public static HediffDef OG_Zygote_Hediff_MultiLung;
        [MayRequireAstartes]
        public static HediffDef OG_Zygote_Hediff_Occulobe;
        [MayRequireAstartes]
        public static HediffDef OG_Zygote_Hediff_LymanEar;
        [MayRequireAstartes]
        public static HediffDef OG_Zygote_Hediff_SusanMembrane;
        [MayRequireAstartes]
        public static HediffDef OG_Zygote_Hediff_Melanochrome;
        [MayRequireAstartes]
        public static HediffDef OG_Zygote_Hediff_OoliticKidney;
        [MayRequireAstartes]
        public static HediffDef OG_Zygote_Hediff_Neuroglottis;
        [MayRequireAstartes]
        public static HediffDef OG_Zygote_Hediff_Mucranoid;
        [MayRequireAstartes]
        public static HediffDef OG_Zygote_Hediff_BetchersGland;
        [MayRequireAstartes]
        public static HediffDef OG_Zygote_Hediff_ProgenoidGland;
        [MayRequireAstartes]
        public static HediffDef OG_Zygote_Hediff_BlackCarapace;

        // Organ Effects
        [MayRequireAstartes]
        public static HediffDef OG_Hediff_Geneseed;
        [MayRequireAstartes]
        public static HediffDef OG_Astartes_Oolitic_Coma;
        [MayRequireAstartes]
        public static HediffDef OG_Astartes_SusAn_Hibernation;
    }
}
