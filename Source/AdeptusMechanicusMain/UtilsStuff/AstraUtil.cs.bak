using RimWorld;
using System;
using System.Reflection;
using Verse;

namespace AdeptusMechanicus
{
    [StaticConstructorOnStartup]
    static class AstraUtil
    {
        private static bool logging = false;
        private static bool initialized = false;
        public static bool AstraCore = false;
        public static bool AstraFactions = false;
        public static bool AstraTurrets = false;
        private static FactionDef AstraChaosMarine;
        private static FactionDef AstraTraitorGuard;
        private static FactionDef AstraTyranid;
        static AstraUtil()
        {
            foreach (ModContentPack p in LoadedModManager.RunningMods)
            {
                string name;
                if (p.Name.Contains("Astra Militarum"))
                {
                    Log.Message(string.Format("Astra Militarum mods Detected"));
                    if (p.Name.Contains("Astra Militarum Imperial Guard Core Mod"))
                    {
                    //    Log.Message(string.Format("Astra Militarum Imperial Guard Core Mod"));
                        name = p.Name;
                        if (p.Name.IndexOf(name) != -1)
                        {
                            AstraCore = true;
                        }
                    }
                    if (p.Name.Contains("Astra Militarum Imperial Guard Factions Mod"))
                    {
                    //    Log.Message(string.Format("Imperial Guard Factions"));
                        name = p.Name;
                        if (p.Name.IndexOf(name) != -1)
                        {
                            AstraFactions = true;
                            AstraChaosMarine = DefDatabase<FactionDef>.GetNamed("IG_ChaosMarineFaction");
                            AstraTraitorGuard = DefDatabase<FactionDef>.GetNamed("TraitorGuardFaction");
                            AstraTyranid = DefDatabase<FactionDef>.GetNamed("TyranidFaction");
                    //        Log.Message(string.Format("{0}", AstraChaosMarine));
                    //        Log.Message(string.Format("{0}", AstraTraitorGuard));
                    //        Log.Message(string.Format("{0}", AstraTyranid));
                        }
                    }
                    if (p.Name.Contains("Astra Militarum Imperial Guard Turret Addon Mod "))
                    {
                    //    Log.Message(string.Format("Astra Militarum Imperial Guard Turret Addon Mod "));
                        name = p.Name;
                        if (p.Name.IndexOf(name) != -1)
                        {
                            AstraTurrets = true;

                        }
                    }
                }
            }
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