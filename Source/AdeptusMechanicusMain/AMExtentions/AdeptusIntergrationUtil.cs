using DualWield;
using DualWield.Storage;
using Harmony;
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
        public static bool enabled_AstraCore = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.Name.Contains("Astra Militarum") && m.Name.Contains("Imperial Guard Core Mod"));
        public static bool enabled_AstraFactions = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.Name.Contains("Astra Militarum") && m.Name.Contains("Imperial Guard Factions Mod"));
        public static bool enabled_AstraTurrets = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.Name.Contains("Astra Militarum") && m.Name.Contains("Imperial Guard Turret Addon Mod"));

        public static bool enabled_MagosXenobiologis = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.Name.Contains("Adeptus Mechanicus") && m.Name.Contains("Magos Xenobiologis"));
        public static bool enabled_AlienRaces = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.Name == "Humanoid Alien Races 2.0");

        public static bool enabled_XenobiologisOrk = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.Name.Contains("Adeptus Mechanicus") && m.Name.Contains("Ork") && m.Name.Contains("Xenobiologis"));
        public static bool enabled_XenobiologisTau = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.Name.Contains("Adeptus Mechanicus") && m.Name.Contains("Tau") && m.Name.Contains("Xenobiologis"));
        public static bool enabled_XenobiologisEldar = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.Name.Contains("Adeptus Mechanicus") && m.Name.Contains("Eldar") && !m.Name.Contains("Dark") && m.Name.Contains("Xenobiologis"));
        public static bool enabled_XenobiologisDarkEldar = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.Name.Contains("Adeptus Mechanicus") && m.Name.Contains("Eldar") && m.Name.Contains("Dark") && m.Name.Contains("Xenobiologis"));
        public static bool enabled_XenobiologisChaos = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.Name.Contains("Adeptus Mechanicus") && m.Name.Contains("Chaos") && m.Name.Contains("Xenobiologis"));
        public static bool enabled_XenobiologisNecron = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.Name.Contains("Adeptus Mechanicus") && m.Name.Contains("Necron") && m.Name.Contains("Xenobiologis"));
        public static bool enabled_XenobiologisTyranid = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.Name.Contains("Adeptus Mechanicus") && m.Name.Contains("Tyranid") && m.Name.Contains("Xenobiologis"));

        public static bool enabled_CombatExtended = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.Name == "Combat Extended");

        public static bool enabled_rooloDualWield = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.Name == "Dual Wield");
        
        public static bool enabled_RimWorldOfMagic = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.Name == "A RimWorld of Magic");

        private static FactionDef AstraChaosMarine;
        private static FactionDef AstraTraitorGuard;
        private static FactionDef AstraTyranid;

        static AdeptusIntergrationUtil()
        {
            if (enabled_AstraFactions)
            {
                AstraChaosMarine = DefDatabase<FactionDef>.GetNamed("IG_ChaosMarineFaction");
                AstraTraitorGuard = DefDatabase<FactionDef>.GetNamed("TraitorGuardFaction");
                AstraTyranid = DefDatabase<FactionDef>.GetNamed("TyranidFaction");
            }
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
                //    Log.Message(string.Format("{0}", AstraChaosMarine));
                return Find.FactionManager.FirstFactionOfDef(AstraChaosMarine);
            }
        }
        public static Faction OfTraitorGuard
        {
            get
            {
                //    Log.Message(string.Format("{0}", AstraTraitorGuard));
                return Find.FactionManager.FirstFactionOfDef(AstraTraitorGuard);
            }
        }
        public static Faction OfTyranid
        {
            get
            {
                //    Log.Message(string.Format("{0}", AstraTyranid));
                return Find.FactionManager.FirstFactionOfDef(AstraTyranid);
            }
        }
    }

}