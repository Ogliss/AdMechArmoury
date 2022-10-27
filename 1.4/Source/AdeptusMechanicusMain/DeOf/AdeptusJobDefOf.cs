using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class AdeptusJobDefOf
    {
        static AdeptusJobDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AdeptusJobDefOf));
        }
        public static JobDef OG_Job_ChangeLaserColor;

        [MayRequireXenobiologis]
        public static JobDef OG_AMXB_Deconstruct;
        [MayRequireXenobiologis]
        public static JobDef OG_AMXB_WaitBuilding;
        [MayRequireXenobiologis]
        public static JobDef OG_AMXB_WaitCombatBuilding;
        [MayRequireXenobiologis]
        public static JobDef OG_AMXB_AttackBuilding;

        [MayRequireOrkz]
        public static JobDef OGO_Orkoid_Hunt;
        [MayRequireOrkz]
        public static JobDef OGO_Orkoid_Scrap;

        [MayRequireXenobiologis, MayRequireNecrons]
        public static JobDef OG_Necron_TombSpyder_Repair;


        [MayRequireAstartes]
        public static JobDef OG_DepositIntoGrowerJob;
        [MayRequireAstartes]
        public static JobDef OG_ExtractFromGrowerJob;
        [MayRequireAstartes]
        public static JobDef OG_MaintainGrowerJob_Intellectual;
        [MayRequireAstartes]
        public static JobDef OG_MaintainGrowerJob_Medicine;

        [MayRequireAstartes]
        public static JobDef OG_Astartes_OmophageaIngest;
        [MayRequireAstartes]
        public static JobDef OG_Astartes_NeuroglottisInspect;
        [MayRequireAstartes]
        public static JobDef OG_Astartes_EnterSusAn;
        [MayRequireAstartes]
        public static JobDef OG_Astartes_WakeSusAn;
        [MayRequireAstartes]
        public static JobDef OG_Astartes_WakeSusAnOther;
    }
}
