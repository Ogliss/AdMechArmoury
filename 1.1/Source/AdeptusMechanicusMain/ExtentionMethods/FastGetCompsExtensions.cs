using Verse;

namespace AdeptusMechanicus.ExtensionMethods
{
    public static class FastGetCompsExtensions
    {
        public static T GetModExtensionFast<T>(this Thing thing) where T : DefModExtension
        {
            return thing.def.GetModExtensionFast<T>();
        }
        public static T GetModExtensionFast<T>(this Def def) where T : DefModExtension
        {
            if (def.modExtensions == null)
            {
                return default(T);
            }

            var type = typeof(T);
            var extensions = def.modExtensions;
            for (int i = 0, count = extensions.Count; i < count; i++)
            {
                var extension = extensions[i];
                if (extension.GetType() == type)
                    return (T)extension;
            }
            return default(T);
        }
        
        // Token: 0x060017F8 RID: 6136 RVA: 0x0008820C File Offset: 0x0008640C
        public static T TryGetCompFast<T>(this Thing thing) where T : ThingComp
        {
            ThingWithComps thingWithComps = thing as ThingWithComps;
            if (thingWithComps == null)
            {
                return default(T);
            }
            return thingWithComps.GetCompFast<T>();
        }
        private static T GetCompFast<T>(this ThingWithComps thing) where T : ThingComp
        {
            var type = typeof(T);
            var comps = thing.AllComps;
            for (int i = 0, count = comps.Count; i < count; i++)
            {
                var comp = comps[i];
                if (comp.GetType() == type)
                    return (T)comp;
            }
            return null;
        }

        public static T TryGetCompFast<T>(this Hediff hd) where T : HediffComp
        {
            HediffWithComps hediffWithComps = hd as HediffWithComps;
            if (hediffWithComps == null)
            {
                return default(T);
            }
            if (hediffWithComps.comps != null)
            {
                var type = typeof(T);
                var comps = hediffWithComps.comps;
                for (int i = 0; i < comps.Count; i++)
                {
                    var comp = comps[i];
                    if (comp.GetType() == type)
                        return (T)comp;
                }
            }
            return default(T);
        }

    }
}
