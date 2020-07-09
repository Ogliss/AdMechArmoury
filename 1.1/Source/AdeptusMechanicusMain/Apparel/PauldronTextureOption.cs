using RimWorld;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public class PauldronTextureOption : IExposable
    {
        public PauldronTextureOption()
        {
        }
        public PauldronTextureOption(PauldronTextureOption o)
        {
            faction = o.faction;
            texPath = o.texPath;
            color = o.color;
            colorTwo = o.colorTwo;
        }
        public PauldronTextureOption(string f, string t)
        {
            faction = f;
            texPath = t;
        }
        public PauldronTextureOption(string t)
        {
            faction = null;
            texPath = t;
        }

        private Color? color = null;
        private Color? colorTwo = null;
        private string label;
        public string faction;
        public string texPath;
        public Color? Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }

        public Color? ColorTwo
        {
            get
            {
                return colorTwo;
            }
            set
            {
                colorTwo = value;
            }
        }
        public string Label
        {
            get
            {
                if (!label.NullOrEmpty())
                {
                    return label;
                }
                if (factionDef !=null)
                {
                    label = factionDef.LabelCap;
                }
                else
                {
                    label = texPath;
                }

                return label;
            }
            set
            {
                label = value;
            }
        }
        public string TexPath => texPath.NullOrEmpty() ? string.Empty : Regex.Replace(texPath, " ", "");

        public FactionDef factionDef => !faction.NullOrEmpty() ? DefDatabase<FactionDef>.GetNamedSilentFail(faction) : null;

        public void ExposeData()
        {
            Scribe_Values.Look(ref this.faction, "faction");
            Scribe_Values.Look(ref this.texPath, "TexPath");
            Scribe_Values.Look(ref this.label, "label");
            Scribe_Values.Look(ref this.color, "color");
            Scribe_Values.Look(ref this.colorTwo, "colorTwo");
        }
    }

}
