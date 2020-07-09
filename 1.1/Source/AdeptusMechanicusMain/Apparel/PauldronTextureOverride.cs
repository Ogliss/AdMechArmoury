using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    [StaticConstructorOnStartup]
    public class PauldronTextureOverride : IExposable
    {
        public PauldronTextureOption defaultOption = new PauldronTextureOption(null, "Blank");
        public PauldronTextureOption activeOption;
        public List<PauldronTextureOption> Options = new List<PauldronTextureOption>();
        public void ExposeData()
        {
            Scribe_Deep.Look<PauldronTextureOption>(ref this.defaultOption, "defaultOption");
            Scribe_Deep.Look<PauldronTextureOption>(ref this.activeOption, "activeOption", this.defaultOption);
        }
    }

}
