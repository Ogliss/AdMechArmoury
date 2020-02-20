using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using HarmonyLib;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(DropPodUtility), "MakeDropPodAt", null)]
    public class AM_DropPodUtility_MakeDropPodAt_FactionSpecificPods_Patch 
    {
        // Token: 0x060011D5 RID: 4565 RVA: 0x000EC6D0 File Offset: 0x000EA8D0
        public static bool Prefix(IntVec3 c, Map map, ActiveDropPodInfo info)
        {
            bool result = true;
            if (info.innerContainer.Any(x => x.def.thingClass == typeof(Pawn) && (x.Faction != null && x.Faction.def.HasModExtension<FactionDefExtension>())))
            {
                List<Thing> list = info.innerContainer.Where(x => x.def.thingClass == typeof(Pawn) && (x.Faction != null && x.Faction.def.HasModExtension<FactionDefExtension>())).ToList();
                FactionDefExtension extension = list.RandomElement().Faction.def.GetModExtension<FactionDefExtension>();
                if (extension.DropPodOverride == DeepStrikeType.Drop)
                {
                    DeepStrikeUtility.MakeDropPodAt(c, map, info, extension);
                    result = false;
                }
                else if (extension.DropPodOverride == DeepStrikeType.Fly)
                {
                    DeepStrikeUtility.MakeFlyerLandAt(c, map, info, extension);
                    result = false;
                }
                else if (extension.DropPodOverride == DeepStrikeType.Teleport)
                {
                    DeepStrikeUtility.MakeTeleportAt(c, map, info, extension);
                    result = false;
                }
                else if (extension.DropPodOverride == DeepStrikeType.Tunnel)
                {
                    DeepStrikeUtility.MakeTunnelAt(c, map, info, extension);
                    result = false;
                }

            }
            return result;
        }
        
    }
}
