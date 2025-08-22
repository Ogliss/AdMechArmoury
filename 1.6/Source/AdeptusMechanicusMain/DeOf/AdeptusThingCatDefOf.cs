using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class AdeptusThingCatDefOf
    {
        static AdeptusThingCatDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AdeptusThingCatDefOf));
        }

        #region Imperial
        public static ThingCategoryDef OG_Weapons_Imperial;
        public static ThingCategoryDef OG_Apparel_Imperial;

            #region Assassinorum
            public static ThingCategoryDef OG_Weapons_Assassinorum;
            public static ThingCategoryDef OG_Apparel_Assassinorum;
            #endregion

            #region Astartes
            [MayRequireAstartes]
            public static ThingCategoryDef OG_Weapons_Astartes;
            [MayRequireAstartes]
            public static ThingCategoryDef OG_Apparel_Astartes;
            #endregion

            #region Inquisition
            [MayRequireAstartes]
            public static ThingCategoryDef OG_Weapons_Inquisition;
            [MayRequireAstartes]
            public static ThingCategoryDef OG_Apparel_Inquisition;
            #endregion

            #region Mechanicus
        //    [MayRequireXenobiologis, MayRequireMechanicus]
            public static ThingCategoryDef OG_Weapons_Mechanicus;
            public static ThingCategoryDef OG_Apparel_Mechanicus;
            #endregion
        
            #region Militarum
        //    [MayRequireXenobiologis, MayRequireMechanicus]
            public static ThingCategoryDef OG_Weapons_Militarum;
            public static ThingCategoryDef OG_Apparel_Militarum;
            #endregion
        
            #region Sororitas
        //    [MayRequireXenobiologis, MayRequireMechanicus]
            public static ThingCategoryDef OG_Weapons_Sororitas;
            public static ThingCategoryDef OG_Apparel_Sororitas;
            #endregion
        #endregion

        #region Chaos
        public static ThingCategoryDef OG_Weapons_Chaos;
        [MayRequireChaos]
        public static ThingCategoryDef OG_Apparel_Chaos; 
        #endregion

        #region Aeldari
        [MayRequireDarkEldar, MayRequireEldar]
        public static ThingCategoryDef OG_Weapons_Aeldari;
        [MayRequireDarkEldar, MayRequireEldar]
        public static ThingCategoryDef OG_Apparel_Aeldari;
            #region Asuryani
            [MayRequireXenobiologis, MayRequireEldar]
            public static ThingCategoryDef OG_Weapons_Asuryani;
            [MayRequireEldar]
            public static ThingCategoryDef OG_Apparel_Asuryani;
            #endregion
            #region Drukhari
            [MayRequireXenobiologis, MayRequireDarkEldar]
            public static ThingCategoryDef OG_Weapons_Drukhari;
            [MayRequireDarkEldar]
            public static ThingCategoryDef OG_Apparel_Drukhari;
            #endregion
        #endregion

        #region Greenskins
        [MayRequireOrkz]
        public static ThingCategoryDef OG_Weapons_Ork;
        [MayRequireOrkz]
        public static ThingCategoryDef OG_Apparel_Ork;
        #endregion

        #region Tau
        [MayRequireTau]
        public static ThingCategoryDef OG_Weapons_Tau;
        [MayRequireTau]
        public static ThingCategoryDef OG_Apparel_Tau;
        #endregion

        #region Kroot
        [MayRequireTau]
        public static ThingCategoryDef OG_Weapons_Kroot;
        [MayRequireTau]
        public static ThingCategoryDef OG_Apparel_Kroot;
        #endregion

        #region Vespid
        [MayRequireTau]
        public static ThingCategoryDef OG_Weapons_Vespid;
        [MayRequireTau]
        public static ThingCategoryDef OG_Apparel_Vespid;
        #endregion

        #region Necron
        [MayRequireNecrons]
        public static ThingCategoryDef OG_Weapons_Necron;
        [MayRequireNecrons]
        public static ThingCategoryDef OG_Apparel_Necron;
        #endregion

        #region Tyranids
        [MayRequireTyranids]
        public static ThingCategoryDef OG_Weapons_Tyranid;
        [MayRequireTyranids]
        public static ThingCategoryDef OG_Apparel_Tyranid;
        #endregion
    }
}
