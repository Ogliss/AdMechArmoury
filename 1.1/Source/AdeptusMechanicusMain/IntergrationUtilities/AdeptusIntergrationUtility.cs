using DualWield;
using DualWield.Storage;
using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;

namespace AdeptusMechanicus
{
    [StaticConstructorOnStartup]
    public static class AdeptusIntergrationUtility
    {
        /*
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

        public static bool enabled_GasTrap = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == "com.distman.gastrap");

        public static bool enabled_AvP = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == "Ogliss.AlienVsPredator");
        public static bool enabled_ChjAndroids = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == "ChJees.Androids");
        public static bool enabled_AndroidTiers = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == "Atlas.AndroidTiers");
        */

        public static bool enabled_AstraCore;
        public static bool enabled_AstraServoSkulls;
        public static bool enabled_AstraFactions;
        public static bool enabled_AstraTurrets;

        public static bool enabled_GeneSeed;

        public static bool enabled_MagosXenobiologis;
        public static bool enabled_AlienRaces;

        public static bool enabled_AdeptusAstartes;
        public static bool enabled_AdeptusMechanicus;
        public static bool enabled_AdeptusMilitarum;
        public static bool enabled_AdeptusSororitas;
        public static bool enabled_XenobiologisOrk;
        public static bool enabled_XenobiologisTau;
        public static bool enabled_XenobiologisEldar;
        public static bool enabled_XenobiologisDarkEldar;
        public static bool enabled_XenobiologisChaos;
        public static bool enabled_XenobiologisNecron;
        public static bool enabled_XenobiologisTyranid;

        public static bool enabled_CombatExtended;
        public static bool enabled_ResearchPal;

        public static bool enabled_rooloDualWield;
        public static bool enabled_rooloRunAndGun;

        public static bool enabled_RimWorldOfMagic;
        public static bool enabled_RimAtomics;
        public static bool enabled_SOS2;
        public static bool enabled_SRTS;

        public static bool enabled_GasTrap;

        public static bool enabled_AvP;
        public static bool enabled_ChjAndroids;
        public static bool enabled_AndroidTiers;

        public static bool enabled_EndTimesWithGuns;


        static AdeptusIntergrationUtility()
        {
            enabled_AstraCore = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == ("QX.AstraMilitarum"));
            enabled_AstraFactions = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == ("QX.AstraMilitarum.Factions"));
            enabled_AstraTurrets = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == ("QX.AstraMilitarum.Turrets"));
            enabled_AstraServoSkulls = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == ("QX.AstraMilitarum.ServoSkull"));

            enabled_GeneSeed = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == ("Ogliss.AliceTries.GeneSeed"));

            enabled_MagosXenobiologis = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == ("Ogliss.AdMech.Xenobiologis"));
            enabled_AlienRaces = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == "erdelf.HumanoidAlienRaces");

            enabled_AdeptusAstartes = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == ("Ogliss.AdMech.Astartes"));
            enabled_AdeptusMechanicus = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == ("Ogliss.AdMech.Mechanicus"));
            enabled_AdeptusMilitarum = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == ("Ogliss.AdMech.Militarum"));
            enabled_AdeptusSororitas = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == ("Ogliss.AdMech.Sororitas"));
            enabled_XenobiologisOrk = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == ("Ogliss.AdMech.Xenobiologis.Orkz"));
            enabled_XenobiologisTau = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == ("Ogliss.AdMech.Xenobiologis.Tau"));
            enabled_XenobiologisEldar = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == ("Ogliss.AdMech.Xenobiologis.Eldar"));
            enabled_XenobiologisDarkEldar = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == ("Ogliss.AdMech.Xenobiologis.DarkEldar"));
            enabled_XenobiologisChaos = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == ("Ogliss.AdMech.Xenobiologis.Chaos"));
            enabled_XenobiologisNecron = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == ("Ogliss.AdMech.Xenobiologis.Necron"));
            enabled_XenobiologisTyranid = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == ("Ogliss.AdMech.Xenobiologis.Tyranid"));

            enabled_CombatExtended = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == "CETeam.CombatExtended");
            enabled_ResearchPal = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == "notfood.ResearchPal");

            enabled_rooloDualWield = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == "Roolo.DualWield");
            enabled_rooloRunAndGun = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == "Roolo.RunAndGun");

            enabled_RimWorldOfMagic = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == "Torann.ARimworldOfMagic");
            enabled_RimWorldOfMagic = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == "Dubwise.Rimatomics");
            enabled_SOS2 = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == "kentington.saveourship2");
            //    enabled_SRTS = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == "Torann.ARimworldOfMagic");

            enabled_GasTrap = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == "com.distman.gastrap");

            enabled_AvP = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == "Ogliss.AlienVsPredator");
            enabled_ChjAndroids = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == "ChJees.Androids");
            enabled_AndroidTiers = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == "Atlas.AndroidTiers");
            enabled_EndTimesWithGuns = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == "SickBoyWi.TheEndTimes.WithGuns");
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

        public static bool isChjAndroid(PawnKindDef pawn)
        {
            //    bool Result = pawn.race.comps.Any(x => x.compClass.Name.Contains("Androids.CompProperties_EnergyTracker"));
            foreach (var item in pawn.race.comps)
            {
                if (item.compClass.Name.Contains("Androids.CompProperties_EnergyTracker"))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool isChjAndroid(Pawn pawn)
        {
            //    bool Result = pawn.def.comps.Any(x => x.compClass.Name.Contains("Androids.CompProperties_EnergyTracker"));
            foreach (var item in pawn.def.comps)
            {
                if (item.compClass.Name.Contains("Androids.CompProperties_EnergyTracker"))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool isChjAndroid(ThingDef td)
        {
            //    bool Result = td.comps.Any(x => x.compClass.Name.Contains("Androids.CompProperties_EnergyTracker"));
            foreach (var item in td.comps)
            {
                if (item.compClass.Name.Contains("Androids.CompProperties_EnergyTracker"))
                {
                    return true;
                }
            }
            return false;
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
}