using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using HarmonyLib;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(DropPodUtility), "MakeDropPodAt", null)]
    public class DropPodUtility_MakeDropPodAt_FactionSpecificPods_Patch
    {
        // Token: 0x060011D5 RID: 4565 RVA: 0x000EC6D0 File Offset: 0x000EA8D0
        public static bool Prefix(IntVec3 c, Map map, ActiveDropPodInfo info)
        {
            bool result = true;
            List<Thing> list = new List<Thing>();
            for (int i = 0; i < info.innerContainer.Count; i++)
            {
                Thing t = info.innerContainer[i];
                if (t?.Faction?.def.GetModExtension<FactionDefExtension>() != null)
                {
                    list.Add(t);
                }
            }
            if (!list.NullOrEmpty())
            {
                Thing thing = list.RandomElement();
                FactionDefExtension extension = thing.Faction.def.GetModExtension<FactionDefExtension>();
                if (thing.Faction.IsPlayer || extension.DropPodOverride == DeepStrikeType.DropPod && extension.DropPodIncoming == ThingDefOf.DropPodIncoming )
                {
                    return result;
                }
                if (extension.DropPodOverride == DeepStrikeType.DropPod)
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
