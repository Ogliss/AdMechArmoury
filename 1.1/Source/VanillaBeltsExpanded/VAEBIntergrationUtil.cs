using Harmony;
using RimWorld;
using System;
using System.Linq;
using System.Reflection;
using Verse;

namespace VanillaApparelExpandedBelts
{
    [StaticConstructorOnStartup]
    public static class VAEBIntergrationUtil
    {
        public static bool enabled_AlienRaces = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.Name == "Humanoid Alien Races 2.0");
        
        static VAEBIntergrationUtil()
        {

        }
        
    }

}