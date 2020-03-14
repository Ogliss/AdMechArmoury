using RimWorld;
using System;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class OGHediffDefOf
    {
        static OGHediffDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(OGHediffDefOf));
        }
        public static HediffDef Regenerated_Part_OG;
        public static HediffDef Regenerating_Part_OG;
        public static HediffDef PlasmaBurn;
        public static HediffDef RadiationPoisioning;
        public static HediffDef FWPsychicShock;
        public static DamageDef OGForceStrike;

    }
    [DefOf]
    public static class OGTaleDefOf
    {
        static OGTaleDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(OGTaleDefOf));
        }
        public static TaleDef OG_EmergedFromTunnel;

    }
    [DefOf]
    public static class OGJobDefOf
    {
        static OGJobDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(OGJobDefOf));
        }
        public static JobDef OG_Job_ChangeLaserColor;

    }
    [DefOf]
    public static class OGPawnsArrivalModeDefOf
    {
        static OGPawnsArrivalModeDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(OGPawnsArrivalModeDefOf));
        }
        public static PawnsArrivalModeDef OG_DeepStrike_DropPod_TargetPower;
        public static PawnsArrivalModeDef OG_DeepStrike_DropPod_TargetEnemies;
        public static PawnsArrivalModeDef OG_DeepStrike_Teleport_TargetPower;
        public static PawnsArrivalModeDef OG_DeepStrike_Teleport_TargetEnemies;
        public static PawnsArrivalModeDef OG_DeepStrike_Tunnel_TargetPower;
        public static PawnsArrivalModeDef OG_DeepStrike_Tunnel_TargetEnemies;
        public static PawnsArrivalModeDef OG_DeepStrike_Fly_TargetPower;
        public static PawnsArrivalModeDef OG_DeepStrike_Fly_TargetEnemies;

    }
    [DefOf]
    public static class OGDamageDefOf
    {
        static OGDamageDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(OGDamageDefOf));
        }
        public static DamageDef OG_WarpStormStrike;

        public static DamageDef OG_E_Distortion_Damage;
        public static DamageDef OG_E_Distortion_Damage_Blast;
        public static DamageDef OG_N_Gauss_Damage;

        public static DamageDef OG_PowerWeapon_Cut;
        public static DamageDef OG_PowerWeapon_Stab;
        public static DamageDef OG_PowerWeapon_Blunt;
        public static DamageArmorCategoryDef OG_PowerWeapon;

        public static DamageDef OGForceStrike;
        public static DamageDef OG_ForceWeapon_Cut;
        public static DamageDef OG_ForceWeapon_Stab;
        public static DamageDef OG_ForceWeapon_Blunt;
        public static DamageDef OG_Chaos_Deamon_Warpfire;


    }
    [DefOf]
    public static class OGReseachDefOf
    {
        static OGReseachDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(OGReseachDefOf));
        }
        /*
        public static ResearchProjectDef OG_Weapons_Lasgun_Hotshot;
        public static ResearchProjectDef OG_Weapons_BolterDragonfire_Imperial;
        public static ResearchProjectDef OG_Weapons_BolterHellfire_Imperial;
        public static ResearchProjectDef OG_Weapons_BolterKraken_Imperial;
        public static ResearchProjectDef OG_Weapons_BolterVengance_Imperial;
        public static ResearchProjectDef OG_Weapons_Bolter_Imperial;
        */
    }
    [DefOf]
    public static class OGThingDefOf
    {
        static OGThingDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(OGThingDefOf));
        }
        //    public static ThingDef HiveLike; RimlaserPrism
        public static ThingDef OG_AMA_Tunneler;
        public static ThingDef OG_AMA_Teleporter;
        public static ThingDef OG_Warpfire;
        public static ThingDef OG_WarpSpark;
        public static ThingDef OG_Mote_MicroSparksWarp;
        public static ThingDef OG_Mote_WarpFireGlow;

    }
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
    [DefOf]
    public static class OGGameConditionDefOf
    {
        // Token: 0x06003776 RID: 14198 RVA: 0x001A84F4 File Offset: 0x001A68F4
        static OGGameConditionDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(OGGameConditionDefOf));
        }
        // Token: 0x04001FCD RID: 8141
        public static GameConditionDef OG_Warpstorm;
    }
    [DefOf]
    public static class OGStatDefOf
    {
        public static readonly StatDef reliability = StatDef.Named("reliability"); // for gun reliability

    }
}
