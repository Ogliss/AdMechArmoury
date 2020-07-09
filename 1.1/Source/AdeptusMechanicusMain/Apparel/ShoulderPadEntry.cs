using RimWorld;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    [StaticConstructorOnStartup]
    public class ShoulderPadEntry : IExposable
    {
        public ShoulderPadEntry()
        {
        }

        public ShoulderPadEntry(ShoulderPadEntry entry, CompPauldronDrawer drawer)
        {
            this.shoulderPadType = entry.shoulderPadType;
            this.shaderType = entry.shaderType;
            this.bodyspecificTextures = entry.bodyspecificTextures;
            this.padTexPath = entry.padTexPath;
            this.tagged = entry.tagged;
            this.commonality = entry.commonality;
            this.northtop = entry.northtop;
            this.UseFactionTextures = entry.UseFactionTextures;
            this.UseFactionColors = entry.UseFactionColors;
            this.UseVariableTextures = entry.UseVariableTextures;
            this.UsePrimaryColor = entry.UsePrimaryColor;
            this.VariantTextures = entry.VariantTextures;
            this.PrimaryColor = entry.PrimaryColor;
            this.UseSecondaryColor = entry.UseSecondaryColor;
            this.SecondaryColor = entry.SecondaryColor;
            this.order = entry.order;
            this.sublayer = entry.sublayer;
            this.NorthOffset = entry.NorthOffset;
            this.SouthOffset = entry.SouthOffset;
            this.EastOffset = entry.EastOffset;
            this.WestOffset = entry.WestOffset;
            this.drawer = drawer;
        }

        public ShoulderPadType shoulderPadType;
        public ShaderTypeDef shaderType;
        public bool bodyspecificTextures = true;
        public string padTexPath;
        public string tagged;
        public int commonality;
        public bool northtop = false;
        public bool UseFactionTextures = false;
        public bool UseFactionColors = false;
        public bool UseVariableTextures;
        public bool UsePrimaryColor = true;
        public PauldronTextureOverride VariantTextures;
        public Color PrimaryColor = Color.white;
        public bool UseSecondaryColor = true;
        public Color SecondaryColor = Color.white;
        public int order = 1;
        public int sublayer = 0;
        public Vector3 NorthOffset = new Vector3();
        public Vector3 SouthOffset = new Vector3();
        public Vector3 EastOffset = new Vector3();
        public Vector3 WestOffset = new Vector3();
        public CompPauldronDrawer drawer = null;
        public FactionDef faction;
        private Graphic graphic;
        public Graphic Graphic
        {
            get
            {
                if (graphic == null)
                {
                    UpdatePadGraphic();
                }
                return graphic;
            }
            set
            {
                graphic = value;
            }
        }

        private Vector2 size = new Vector2(1.5f, 1.5f);
        public void UpdatePadGraphic()
        {
            if (drawer?.pawn == null)
            {
                if (drawer == null)
                {
                    Log.Warning("drawer null");
                }
                else
                {
                    Log.Warning("drawer pawn null");
                }
                Graphic = null;
                return;
            }
            Pawn pawn = drawer.pawn;
            Shader shader = this.Shader;
            Vector2 size = drawer.size;
            string path = padTexPath;
            if (UseFactionTextures || UseVariableTextures)
            {
                path = padTexPath + "/" + VariantTextures.activeOption.TexPath;
            }
            if (bodyspecificTextures)
            {
                path += "_" + pawn.story.bodyType.ToString();
            }
            //    Log.Message(path + " Shader: " + shader.name + "Colour: " + mainColorFor(Entry) + " Colour: " + secondaryColorFor(Entry));

            Graphic = GraphicDatabase.Get<Graphic_Multi>(path, shader, size, drawer.mainColorFor(this), drawer.secondaryColorFor(this));
        }

        public Shader Shader => shaderType.Shader;
        public PauldronTextureOption Used
        {
            get
            {
                PauldronTextureOption s = new PauldronTextureOption();
                if (VariantTextures != null)
                {
                    if (VariantTextures != null)
                    {
                        s = VariantTextures.activeOption;
                    }
                }

                return s;
            }
        }

        public string UsedTex
        {
            get
            {
                string s = padTexPath;
                
                if (Used.TexPath != null)
                {
                    s = Used.TexPath;
                    if (s.Contains(" "))
                    {
                        Log.Message("removing space");
                        s = Regex.Replace(Used.TexPath, " ", "");
                    }
                }
                

                return s;
            }
        }

        public bool FactionColours(out Color Color, out Color ColorTwo)
        {
            bool result = false;
            Color = Color.white;
            ColorTwo = Color.white;
            FactionDef factionDef = null;
            FactionDefExtension extension = null;

            if (VariantTextures!=null && VariantTextures.Options.Any(x => x.TexPath == this.UsedTex && !x.faction.NullOrEmpty()))
            {
                factionDef = DefDatabase<FactionDef>.GetNamedSilentFail(VariantTextures.Options.Find(x => x.TexPath == this.UsedTex && !x.faction.NullOrEmpty()).faction);
                if (factionDef != null && factionDef.HasModExtension<FactionDefExtension>())
                {
                    extension = factionDef.GetModExtension<FactionDefExtension>();
                    if (extension.factionColor != null)
                    {
                        Color = extension.factionColor;
                        result = true;
                    }
                    if (extension.factionColorTwo != null)
                    {
                        ColorTwo = extension.factionColorTwo;
                        result = true;
                    }
                }
            }
            return result;
        }
        public void ExposeData()
        {
            Scribe_Values.Look(ref this.shoulderPadType, "shoulderPadType");
            Scribe_Defs.Look(ref this.shaderType, "shaderType");
            Scribe_Values.Look(ref this.bodyspecificTextures, "bodyspecificTextures", true);
            Scribe_Values.Look(ref this.padTexPath, "padTexPath");
            Scribe_Values.Look(ref this.tagged, "tagged");
            Scribe_Values.Look(ref this.commonality, "commonality");
            Scribe_Values.Look(ref this.northtop, "northtop", false);
            Scribe_Values.Look(ref this.UseFactionTextures, "UseFactionTextures", false);
            Scribe_Values.Look(ref this.UseFactionColors, "UseFactionColors", false);
            Scribe_Values.Look(ref this.UseVariableTextures, "UseVariableTextures");
            Scribe_Values.Look(ref this.UsePrimaryColor, "UsePrimaryColor");
            Scribe_Deep.Look(ref this.VariantTextures, "VariantTextures");
            Scribe_Values.Look(ref this.PrimaryColor, "PrimaryColor");
            Scribe_Values.Look(ref this.UseSecondaryColor, "UseSecondaryColor");
            Scribe_Values.Look(ref this.SecondaryColor, "SecondaryColor");
            Scribe_Values.Look(ref this.order, "order");
            Scribe_Values.Look(ref this.sublayer, "sublayer");
            Scribe_Values.Look(ref this.NorthOffset, "NorthOffset");
            Scribe_Values.Look(ref this.SouthOffset, "SouthOffset");
            Scribe_Values.Look(ref this.EastOffset, "EastOffset");
            Scribe_Values.Look(ref this.WestOffset, "WestOffset");
            Scribe_Defs.Look(ref this.faction, "faction");
        }

        public bool FactionTextureFor()
        {
            if (UseFactionTextures)
            {

            }
            return false;
        }
    }

}
