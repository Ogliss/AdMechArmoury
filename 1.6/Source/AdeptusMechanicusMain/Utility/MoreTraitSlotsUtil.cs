using System;
using System.Reflection;
using Verse;

namespace AdeptusMechanicus
{
    public static class MoreTraitSlotsUtil
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
