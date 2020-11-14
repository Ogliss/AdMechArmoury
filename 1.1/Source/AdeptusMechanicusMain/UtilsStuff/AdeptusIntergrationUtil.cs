using DualWield;
using DualWield.Storage;
using HarmonyLib;
using RimWorld;
using System;
using System.Linq;
using System.Reflection;
using Verse;

namespace AdeptusMechanicus
{
    [StaticConstructorOnStartup]
    public static class AdeptusIntergrationUtil
    {
        public static bool enabled_AstraCore = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == ("QX.AstraMilitarum"));
        public static bool enabled_AstraFactions = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == ("QX.AstraMilitarum.Factions"));
        public static bool enabled_AstraTurrets = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == ("QX.AstraMilitarum.Turrets"));

        public static bool enabled_GeneSeed = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == ("Ogliss.AliceTries.GeneSeed"));

        public static bool enabled_MagosXenobiologis = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == ("Ogliss.AdMech.Xenobiologis"));
        public static bool enabled_AlienRaces = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == "erdelf.HumanoidAlienRaces");

        public static bool enabled_AdeptusAstartes = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == ("Ogliss.AdMech.Astartes"));
        public static bool enabled_AdeptusMechanicus = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == ("Ogliss.AdMech.Mechanicus"));
        public static bool enabled_AdeptusMilitarum = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == ("Ogliss.AdMech.Militarum"));
        public static bool enabled_AdeptusSororitas = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == ("Ogliss.AdMech.Sororitas"));
        public static bool enabled_XenobiologisOrk = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == ("Ogliss.AdMech.Xenobiologis.Orkz"));
        public static bool enabled_XenobiologisTau = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == ("Ogliss.AdMech.Xenobiologis.Tau"));
        public static bool enabled_XenobiologisEldar = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == ("Ogliss.AdMech.Xenobiologis.Eldar"));
        public static bool enabled_XenobiologisDarkEldar = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == ("Ogliss.AdMech.Xenobiologis.DarkEldar"));
        public static bool enabled_XenobiologisChaos = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == ("Ogliss.AdMech.Xenobiologis.Chaos"));
        public static bool enabled_XenobiologisNecron = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == ("Ogliss.AdMech.Xenobiologis.Necron"));
        public static bool enabled_XenobiologisTyranid = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == ("Ogliss.AdMech.Xenobiologis.Tyranid"));

        public static bool enabled_CombatExtended = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == "CETeam.CombatExtended");
        public static bool enabled_ResearchPal = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == "notfood.ResearchPal");

        public static bool enabled_rooloDualWield = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == "Roolo.DualWield");
        public static bool enabled_rooloRunAndGun = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == "Roolo.RunAndGun");

        public static bool enabled_RimWorldOfMagic = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == "Torann.ARimworldOfMagic");
        public static bool enabled_SOS2 = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == "kentington.saveourship2");
    //    public static bool enabled_SRTS = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == "Torann.ARimworldOfMagic");



        static AdeptusIntergrationUtil()
        {

        }
        
        // Token: 0x06000021 RID: 33 RVA: 0x000037B0 File Offset: 0x000019B0
        public static void AdMechAddOffHandEquipment(this Pawn_EquipmentTracker instance, ThingWithComps newEq)
        {
            ThingOwner<ThingWithComps> value = Traverse.Create(instance).Field("equipment").GetValue<ThingOwner<ThingWithComps>>();
            ExtendedDataStorage extendedDataStorage = Base.Instance.GetExtendedDataStorage();
            bool flag = extendedDataStorage != null;
            if (flag)
            {
                extendedDataStorage.GetExtendedDataFor(newEq).isOffHand = true;
                LessonAutoActivator.TeachOpportunity(DW_DefOff.DW_Penalties, 0);
                LessonAutoActivator.TeachOpportunity(DW_DefOff.DW_Settings, 0);
                value.TryAdd(newEq, true);
            }
        }
        
        
        // Token: 0x06000022 RID: 34 RVA: 0x00003818 File Offset: 0x00001A18
        public static bool AdMechTryGetOffHandEquipment(this Pawn_EquipmentTracker instance, out ThingWithComps result)
        {
            result = null;
            bool flag = instance.pawn.HasMissingArmOrHand();
            bool result2;
            if (flag)
            {
                result2 = false;
            }
            else
            {
                ExtendedDataStorage extendedDataStorage = Base.Instance.GetExtendedDataStorage();
                foreach (ThingWithComps thingWithComps in instance.AllEquipmentListForReading)
                {
                    ExtendedThingWithCompsData extendedThingWithCompsData;
                    bool flag2 = extendedDataStorage.TryGetExtendedDataFor(thingWithComps, out extendedThingWithCompsData) && extendedThingWithCompsData.isOffHand;
                    if (flag2)
                    {
                        result = thingWithComps;
                        return true;
                    }
                }
                result2 = false;
            }
            return result2;
        }
        
        public static Faction OfChaosMarine
        {
            get
            {
                if (!enabled_AstraFactions)
                {
                    return null;
                }
                //    Log.Message(string.Format("{0}", AstraChaosMarine));
                return Find.FactionManager.FirstFactionOfDef(DefDatabase<FactionDef>.GetNamed("IG_ChaosMarineFaction"));
            }
        }
        public static Faction OfTraitorGuard
        {
            get
            {
                if (!enabled_AstraFactions)
                {
                    return null;
                }
                //    Log.Message(string.Format("{0}", AstraTraitorGuard));
                return Find.FactionManager.FirstFactionOfDef(DefDatabase<FactionDef>.GetNamed("TraitorGuardFaction"));
            }
        }
        public static Faction OfTyranid
        {
            get
            {
                if (!enabled_AstraFactions)
                {
                    return null;
                }
                //    Log.Message(string.Format("{0}", AstraTyranid));
                return Find.FactionManager.FirstFactionOfDef(DefDatabase<FactionDef>.GetNamed("TyranidFaction"));
            }
        }
    }
    static class UtilCE
    {
        public static bool CombatExtended = false;
        public static ModContentPack modContent = null;
        static UtilCE()
        {
            foreach (ModContentPack p in LoadedModManager.RunningMods)
            {
                if (p.PackageIdPlayerFacing == ("CETeam.CombatExtended"))
                {
                    modContent = p;
                    CombatExtended = true;
                }
            }
        }

    }

    static class UtilAvPSynths
    {
        public static bool AvP = false;
        static UtilAvPSynths()
        {
            foreach (ModContentPack p in LoadedModManager.RunningMods)
            {
                if (p.PackageIdPlayerFacing == "Ogliss.AlienVsPredator")
                {
                    AvP = true;
                }
            }
        }

        public static bool isAvPSynth(PawnKindDef pawn)
        {
            bool Result = pawn.RaceProps.FleshType.defName == "RRY_SynthFlesh";

            return Result;
        }
        public static bool isAvPSynth(Pawn pawn)
        {
            bool Result = pawn.def.race.FleshType.defName == "RRY_SynthFlesh";

            return Result;
        }
        public static bool isAvPSynth(ThingDef td)
        {
            bool Result = td.race.FleshType.defName == "RRY_SynthFlesh";

            return Result;
        }
    }

    static class UtilChjAndroids
    {
        public static bool ChjAndroid = false;
        public static ModContentPack modContent = null;
        static UtilChjAndroids()
        {
            foreach (ModContentPack p in LoadedModManager.RunningMods)
            {
                if (p.PackageIdPlayerFacing == ("ChJees.Androids"))
                {
                    modContent = p;
                //    log.message(string.Format("{0}: PackageId: {1}, PackageIdPlayerFacing: {2}", p.Name, p.PackageId, p.PackageIdPlayerFacing));
                    ChjAndroid = true;
                }
            }

        }

        public static bool isChjAndroid(PawnKindDef pawn)
        {
            //    bool Result = pawn.race.comps.Any(x => x.compClass.Name.Contains("Androids.CompProperties_EnergyTracker"));
            bool Result = false;
            if (pawn.race.modContentPack == modContent)
            {
                Result = true;
            }
            return Result;
        }
        public static bool isChjAndroid(Pawn pawn)
        {
            //    bool Result = pawn.def.comps.Any(x => x.compClass.Name.Contains("Androids.CompProperties_EnergyTracker"));
            bool Result = false;
            if (pawn.def.modContentPack == modContent)
            {
                Result = true;
            }
            return Result;
        }
        public static bool isChjAndroid(ThingDef td)
        {
            //    bool Result = td.comps.Any(x => x.compClass.Name.Contains("Androids.CompProperties_EnergyTracker"));
            bool Result = false;
            if (td.modContentPack == modContent)
            {
                Result = true;
            }
            return Result;
        }
    }

    static class UtilTieredAndroids
    {
        public static bool TieredAndroid = false;
        public static ModContentPack modContent = null;
        static UtilTieredAndroids()
        {
            foreach (ModContentPack p in LoadedModManager.RunningMods)
            {
                if (p.PackageIdPlayerFacing == ("Atlas.AndroidTiers"))
                {
                    modContent = p;
                //    log.message(string.Format("{0}: PackageId: {1}, PackageIdPlayerFacing: {2}", p.Name, p.PackageId, p.PackageIdPlayerFacing));
                    TieredAndroid = true;
                }
            }
        }

        public static bool isAtlasAndroid(PawnKindDef pawn)
        {

            bool Result = false;
            if (pawn.race.modContentPack == modContent)
            {
                Result = true;
            }
            return Result;
        }
        public static bool isAtlasAndroid(Pawn pawn)
        {
            bool Result = false;
            if (pawn.def.modContentPack == modContent)
            {
                Result = true;
            }
            return Result;
        }
        public static bool isAtlasAndroid(ThingDef td)
        {
            bool Result = false;
            if (td.modContentPack == modContent)
            {
                Result = true;
            }
            return Result;
        }
    }

    static class UtilDinosauria
    {
        public static bool Dinosauria = false;
        public static ModContentPack modContent = null;
        static UtilDinosauria()
        {
            foreach (ModContentPack p in LoadedModManager.RunningMods)
            {
                if (p.Name.Contains("Dinosauria"))
                {
                    modContent = p;
                    Dinosauria = true;
                }
            }
        }

    }

    static class UtilJurassicRimworld
    {
        public static bool JurassicRimworld = false;
        public static ModContentPack modContent = null;
        static UtilJurassicRimworld()
        {
            foreach (ModContentPack p in LoadedModManager.RunningMods)
            {
                if (p.Name.Contains("Jurassic Rimworld"))
                {
                    modContent = p;
                    JurassicRimworld = true;
                }
            }
        }
    }
}