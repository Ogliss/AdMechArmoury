using RimWorld;
using System.Collections.Generic;
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
            padTexPathOverride = o.padTexPathOverride;
            color = o.color;
            colorTwo = o.colorTwo;
            label = o.label ?? o.texPath;
            invertColors = o.invertColors;
        }
        public PauldronTextureOption(string f, string t, string Override = null)
        {
            faction = f;
            texPath = t;
            padTexPathOverride = Override;
        }
        public PauldronTextureOption(string t)
        {
            faction = null;
            texPath = t;
        }

        public string TexPath => texPath.NullOrEmpty() ? string.Empty : Regex.Replace(texPath, " ", "");
        public Color? Color
        {
            get
            {

                return invertColors ? colorTwo : color;
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
                return invertColors ? color : colorTwo;
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
                    label = factionDef.fixedName ?? factionDef.LabelCap;
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

        public FactionDef factionDef
        {
            get
            {
                if (faction.NullOrEmpty())
                {
                    List<FactionDef> defs = DefDatabase<FactionDef>.AllDefsListForReading;
                    for (int i = 0; i < defs.Count; i++)
                    {
                        if (defs[i].defName.Contains(this.TexPath))
                        {
                            faction = defs[i].defName;
                            break;
                        }
                    }
                }
                return !faction.NullOrEmpty() ? DefDatabase<FactionDef>.GetNamedSilentFail(faction) : null;
            }
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref this.faction, "faction");
            Scribe_Values.Look(ref this.texPath, "TexPath");
            Scribe_Values.Look(ref this.padTexPathOverride, "PadTexPathOverride");
            Scribe_Values.Look(ref this.label, "label");
            Scribe_Values.Look(ref this.color, "color");
            Scribe_Values.Look(ref this.invertColors, "invertColors");
            Scribe_Values.Look(ref this.colorTwo, "colorTwo");
        }
        private Color? color = null;
        private Color? colorTwo = null;
        private string label;
        private bool invertColors = false;
        public string faction;
        public string texPath = null;
        public string padTexPathOverride = null;
    }

}
