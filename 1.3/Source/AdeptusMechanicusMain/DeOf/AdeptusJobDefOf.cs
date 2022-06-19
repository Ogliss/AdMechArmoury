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
        public static JobDef OGAMXBDeconstruct;
        [MayRequireXenobiologis]
        public static JobDef OGAMXBWaitBuilding;
        [MayRequireXenobiologis]
        public static JobDef OGAMXBWaitCombatBuilding;
        [MayRequireXenobiologis]
        public static JobDef OGAMXBAttackBuilding;

        [MayRequireOrkz]
        public static JobDef OGO_Orkoid_Hunt;
        [MayRequireOrkz]
        public static JobDef OGO_Orkoid_Scrap;

        [MayRequireXenobiologis, MayRequireNecrons]
        public static JobDef OG_XB_Job_Necron_TombSpyderRepair;


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
    }
}
