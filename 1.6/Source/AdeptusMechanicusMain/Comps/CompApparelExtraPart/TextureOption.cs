using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public class TextureOption : IExposable
    {
        public TextureOption()
        {
        }
        public TextureOption(TextureOption o)
        {

            faction = o.faction;
            data = o.data;
            color = o.color;
            colorTwo = o.colorTwo;
            label = o.label;
            invertColors = o.invertColors;
        }
        public TextureOption(string f, GraphicData d, string l)
        {
            label = l;
            faction = f;
            data = d;
        }
        public TextureOption(string f, GraphicData d)
        {
            faction = f;
            data = d;
        }
        public TextureOption(GraphicData d)
        {
            faction = null;
            data = d;
        }

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
                if (factionDef != null)
                {
                    label = factionDef.LabelCap;
                }
                else
                {
                    label = data.texPath;
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
                        if (defs[i].defName.Contains(this.data.texPath))
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
        public GraphicData data;
    }

}
