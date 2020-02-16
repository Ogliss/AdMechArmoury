using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x02000020 RID: 32
    public class FactionDefExtension : DefModExtension
    {
        public List<FactionTraitEntry> ForcedTraits = new List<FactionTraitEntry>();
        public List<HediffGiverSetDef> hediffGivers = new List<HediffGiverSetDef>();
        public ThingDef ActiveDropPod = DefDatabase<ThingDef>.GetNamedSilentFail("ActiveDropPod");
        public ThingDef DropPodIncoming = DefDatabase<ThingDef>.GetNamedSilentFail("DropPodIncoming");
        public DeepStrikeType DropPodOverride = DeepStrikeType.Drop;
        public string TeleportBoltTexPath = "Weather/DefaultBolt";
        public SoundDef raidSound = null;
        public float DeepStrikeChance = 0.1f;
        public FloatRange DeepStrikeDelayMin = new FloatRange(30, 120);
        public FloatRange DeepStrikeDelayMax = new FloatRange(180, 240);
        public float InfiltrateChance = 0.1f;
        public FloatRange InfiltrateDelayMin = new FloatRange(15, 90);
        public FloatRange InfiltrateDelayMax = new FloatRange(90, 180);
    }

    public class FactionTraitEntry
    {
        public TraitDef def = null;
        public int degree = 0;
        public float Chance = 0f;
        public bool replaceiffull = true;
    }

    static class MoreTraitSlotsUtil
    {
        private static bool initialized = false;
        private static FieldInfo settingsFieldInfo = null;
        private static FieldInfo maxFieldInfo = null;

        public static bool TryGetMaxTraitSlots(out int max)
        {
            if (!initialized)
            {
                initialized = true;
                Initialized();
            }

            if (settingsFieldInfo != null && maxFieldInfo != null)
            {
                object settings = settingsFieldInfo.GetValue(null);
                if (settings != null)
                {
                    max = (int)(float)maxFieldInfo.GetValue(settings);
                    return true;
                }
            }
            max = 0;
            return false;
        }

        private static void Initialized()
        {
            foreach (ModContentPack p in LoadedModManager.RunningMods)
            {
                if (p.Name.IndexOf("More Trait Slots") != -1)
                {
                    foreach (Assembly assembly in p.assemblies.loadedAssemblies)
                    {
                        Type type = assembly.GetType("MoreTraitSlots.RMTS");
                        if (type != null)
                        {
                            settingsFieldInfo = type.GetField("Settings", BindingFlags.Public | BindingFlags.Static);
                            if (settingsFieldInfo != null)
                            {
                                Type st = settingsFieldInfo.GetValue(null).GetType();
                                maxFieldInfo = st.GetField("traitsMax", BindingFlags.Public | BindingFlags.Instance);
                            }
                            return;
                        }
                    }
                }
            }
        }
    }
}
