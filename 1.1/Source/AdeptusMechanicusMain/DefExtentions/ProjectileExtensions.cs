using RimWorld;
using System;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x02000020 RID: 32
    public class TrailerProjectileExtension : DefModExtension
    {
        public string trailMoteDef = "Mote_Smoke";
        public float trailMoteSize = 1f;
        public int trailerMoteInterval = 30;
    }

    public class GlowerProjectileExtension : DefModExtension
    {
        public string GlowMoteDef = string.Empty;
        public float GlowMoteSize = 1f;
        public int GlowMoteInterval = 30;
    }
    public class EffectProjectileExtension : DefModExtension
    {
        public string ImpactMoteDef = string.Empty;
        public float ImpactMoteSize = 1f;
    }
}
