using RimWorld;
using Verse;
using HarmonyLib;
using System;

namespace AdeptusMechanicus.HarmonyInstance
{
    //helicopter incoming, Edge Code thanks to SmashPhil and Neceros of SRTS-Expanded!
    [HarmonyPatch(typeof(DropPodUtility), "MakeDropPodAt", new Type[] { typeof(IntVec3), typeof(Map), typeof(ActiveDropPodInfo) })]
    public static class DropPodUtility_MakeDropPodAt_Dropship_Patch
    {
        public static bool Prefix(IntVec3 c, Map map, ActiveDropPodInfo info)
        {
            Thing dropship = null;
            CompDropship cargo = null;
            //    CompTransporter comp2 = null;
            for (int index = 0; index < info.innerContainer.Count; index++)
            {
                if (info.innerContainer[index].TryGetComp<CompDropship>() != null)
                {
                    dropship = info.innerContainer[index];
                    string defName = dropship.def.defName;
                    ActiveDropPod activeDropPod = (ActiveDropPod)ThingMaker.MakeThing(ThingDef.Named(defName + "_Active"), null);

                    activeDropPod.Contents = info;
                    EnsureInBounds(ref c, info.innerContainer[index].def, map);
                    info.innerContainer.Remove(dropship);
                    cargo = dropship.TryGetComp<CompDropship>();
                    cargo.Transporter.innerContainer = info.innerContainer;
                    SkyfallerMaker.SpawnSkyfaller(ThingDef.Named(defName + "_Incoming"), dropship, c, map);
                    return false;
                }
            }

            return true;
        }

        private static void EnsureInBounds(ref IntVec3 c, ThingDef dropship, Map map)
        {

            int x = (int)9;
            int y = (int)9;
            int offset = x > y ? x : y;

            if (c.x < offset)
            {
                c.x = offset;
            }
            else if (c.x >= (map.Size.x - offset))
            {
                c.x = (map.Size.x - offset);
            }
            if (c.z < offset)
            {
                c.z = offset;
            }
            else if (c.z > (map.Size.z - offset))
            {
                c.z = (map.Size.z - offset);
            }
        }
    }

}