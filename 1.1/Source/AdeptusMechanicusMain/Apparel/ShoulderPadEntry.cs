using AdeptusMechanicus.HarmonyInstance;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
 //   [StaticConstructorOnStartup]
    public class ShoulderPadEntry : IExposable
    {
        public ShoulderPadEntry()
        {
        }

        public ShoulderPadEntry(ShoulderPadEntryProps entry, CompPauldronDrawer drawer)
        {
            this.props = entry;
            this.label = entry.label;
            this.shoulderPadType = entry.shoulderPadType;
            this.shaderType = entry.shaderType;
            this.bodyspecificTextures = entry.bodyspecificTextures;
            this.padTexPath = entry.padTexPath;
            this.label = entry.label;
            this.commonality = entry.commonality;
            this.northtop = entry.northtop;
            this.UseFactionTextures = entry.UseFactionTextures;
            this.UseFactionColors = entry.UseFactionColors;
            this.UseVariableTextures = entry.UseVariableTextures;
            this.UsePrimaryColor = entry.UsePrimaryColor;
            this.overridePrimaryColor = entry.overridePrimaryColor;
            this.UseSecondaryColor = entry.UseSecondaryColor;
            this.overrideSecondaryColor = entry.overrideSecondaryColor;
            this.order = entry.order;
            this.sublayer = entry.sublayer;
            if (this.UseFactionTextures || this.UseVariableTextures)
            {
                this.defaultOption = entry.defaultOption;
                this.activeOption = entry.defaultOption;
            }
            this.drawer = drawer;
            this.apparel = drawer.apparel;
        }

        public Shader Shader => shaderType.Shader;
        public Vector3 NorthOffset => this.Props.NorthOffset;
        public Vector3 SouthOffset => this.Props.SouthOffset;
        public Vector3 EastOffset => this.Props.EastOffset;
        public Vector3 WestOffset => this.Props.WestOffset;
        public Vector3 OffsetFor(Rot4 rot)
        {
            Vector3 vector = new Vector3();
            if (rot == Rot4.North)
            {
                return NorthOffset;
            }
            if (rot == Rot4.South)
            {
                return SouthOffset;
            }
            if (rot == Rot4.East)
            {
                return EastOffset;
            }
            if (rot == Rot4.West)
            {
                return WestOffset;
            }
            return vector;
        }
        public CompPauldronDrawer Drawer
        {
            get
            {
                if (drawer == null)
                {
                    if (apparel != null)
                    {
                        drawer = apparel.TryGetComp<CompPauldronDrawer>();
                    }
                }
                return drawer;
            }
        }
        public PauldronTextureOption DefaultOption
        {
            get
            {
                return props.defaultOption;
            }
        }

        public string Label
        {
            get
            {
                string s = this.shoulderPadType.ToString();
                if (!this.label.NullOrEmpty())
                {
                    s = this.label + " " + s;
                }
                return s;
            }
        }
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

        public ShoulderPadEntryProps Props
        {
            get
            {
                if (props == null)
                {
                    props = new ShoulderPadEntryProps();
                    if (Drawer != null)
                    {
                        for (int i = 0; i < Drawer.Props.PauldronEntries.Count; i++)
                        {
                            ShoulderPadEntryProps e = Drawer.Props.PauldronEntries[i];
                            if ((this.padTexPath == e.padTexPath && this.shoulderPadType == e.shoulderPadType) || (!this.label.NullOrEmpty() && !e.label.NullOrEmpty() && this.label == e.label))
                            {
                                props = e;
                                if (!e.label.NullOrEmpty())
                                {
                                    this.label = e.label;
                                }
                        //        Log.Message("ShoulderPadEntryProps found for " + Label);
                                break;
                            }
                        }
                    }
                }
                return props;
            }
        }
        public void UpdatePadGraphic()
        {
            if (Drawer?.pawn == null)
            {
                Graphic = null;
                return;
            }
            Pawn pawn = Drawer.pawn;
            Shader shader = this.Shader;
            string path = padTexPath;
            if (UseFactionTextures || UseVariableTextures)
            {
                bool notPlayer = pawn.Faction != null && (pawn.Faction != Faction.OfPlayer);
                if (notPlayer)
                {
                    FactionDefExtension ext = pawn.Faction.def.HasModExtension<FactionDefExtension>() ? pawn.Faction.def.GetModExtension<FactionDefExtension>() : null;
                    bool factionTextures = UseFactionTextures && ext?.factionTextureTag != null;
                    if (factionTextures)
                    {
                    //    Log.Message("using factionTextureTag " + ext.factionTextureTag);
                        for (int i = 0; i < Options.Count; i++)
                        {
                            if (Options[i].TexPath == ext.factionTextureTag)
                            {
                                Used = Options[i];
                            //    Log.Message("Found faction VariantTexture " + VariantTextures.Options[i].texPath);
                                break;
                            }
                        }
                    }
                    else
                    {
                        bool foundVar = false;
                        if (!Options.NullOrEmpty())
                        {
                            for (int i = 0; i < Options.Count; i++)
                            {
                                if (pawn.kindDef.apparelTags.Contains(Options[i].TexPath))
                                {
                                    Used = Options[i];
                                    foundVar = true;
                                //    Log.Message("Found KindDef VariantTexture " + VariantTextures.Options[i].texPath);
                                    break;
                                }
                            }
                            if (!foundVar)
                            {
                                Used = Options.RandomElement();
                            }
                        }
                    }
                }
                else
                {
                    if (UseFactionTextures)
                    {
                    //    Log.Message("UseFactionTextures");
                        CompFactionColorableTwo FC = Drawer.Colours as CompFactionColorableTwo;
                        if (FC != null)
                        {
                        //    Log.Message("FC != null");
                            if (FC.FactionDef != null)
                            {
                        //        Log.Message("FactionDef = " + FC.FactionDef.LabelCap);
                                FactionDefExtension e = FC.Extension;
                                if (e != null)
                                {
                                 //   Log.Message("FactionDefExtension != null");
                                    if (!Options.NullOrEmpty())
                                    {
                                    //    Log.Message("Options: " + Options.Count);
                                        for (int i = 0; i < Options.Count; i++)
                                        {
                                        //    Log.Message("checking " + e.factionTextureTag + " Vs " + Options[i].TexPath + " = " + (Options[i].TexPath == e.factionTextureTag));
                                            if (Options[i].TexPath == e.factionTextureTag)
                                            {
                                                Used = Options[i];
                                        //        Log.Message("Found faction VariantTexture " + Options[i].TexPath);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                        //    Log.Message("FC == null");
                            List<FactionDef> factions = DefDatabase<FactionDef>.AllDefsListForReading;
                            for (int i = 0; i < factions.Count; i++)
                            {
                                FactionDef f = factions[i];

                                FactionDefExtension e = f.HasModExtension<FactionDefExtension>() ? f.GetModExtension<FactionDefExtension>() : null;
                                if (e == null)
                                {
                                    continue;
                                }
                                if (e.factionTextureTag.NullOrEmpty())
                                {
                                    continue;
                                }
                                if (Options[i].TexPath == e.factionTextureTag)
                                {
                                //    Used = Options[i];
                            //        Log.Message("Found faction VariantTexture " + Options[i].TexPath);
                                    break;
                                }

                            }
                        }
                    }
                }

                path = padTexPath + "/" + Used.TexPath;
            }
            if (bodyspecificTextures)
            {
                path += "_" + pawn.story.bodyType.ToString();
            }

        //    Log.Message(path + " Shader: " + shader.name + "Colour: " + Drawer.mainColorFor(this) + " Colour: " + Drawer.secondaryColorFor(this));
            Texture2D tex = ContentFinder<Texture2D>.Get(path+"_south", false);
            Graphic graphic;
            if (tex == null && (UseFactionTextures || UseVariableTextures))
            {
                path = padTexPath + "/" + this.DefaultOption.TexPath;
                if (bodyspecificTextures)
                {
                    path += "_" + pawn.story.bodyType.ToString();
                }
            }
            Color color = Drawer.mainColorFor(this);
            Color colorTwo = Drawer.secondaryColorFor(this);
            graphic = GraphicDatabase.Get<Graphic_Multi>(path, shader, size, color, colorTwo);
            CompFactionColorableTwo factionColors = Drawer.Colours as CompFactionColorableTwo;
            if (factionColors != null)
            {
                Texture texture;
                if (factionColors.ActiveFaction)
                {
                    if (!factionColors.Extension.factionMaskTag.NullOrEmpty())
                    {
                        string msk = "m_" + factionColors.Extension.factionMaskTag;
                        texture = ContentFinder<Texture2D>.Get(graphic.path + "_east" + msk, false);
                        if (texture != null)
                        {
                            graphic.MatEast.SetTexture(ShaderPropertyIDs.MaskTex, texture);
                        }
                        graphic.MatEast.SetColor(ShaderPropertyIDs.ColorTwo, colorTwo);

                        texture = ContentFinder<Texture2D>.Get(graphic.path + "_west" + msk, false);
                        if (texture != null)
                        {
                            graphic.MatWest.SetTexture(ShaderPropertyIDs.MaskTex, texture);
                        }
                        graphic.MatWest.SetColor(ShaderPropertyIDs.ColorTwo, colorTwo);

                        texture = ContentFinder<Texture2D>.Get(graphic.path + "_south" + msk, false);
                        if (texture != null)
                        {
                            graphic.MatSouth.SetTexture(ShaderPropertyIDs.MaskTex, texture);
                        }
                        graphic.MatSouth.SetColor(ShaderPropertyIDs.ColorTwo, colorTwo);

                        texture = ContentFinder<Texture2D>.Get(graphic.path + "_north" + msk, false);
                        if (texture != null)
                        {
                            graphic.MatNorth.SetTexture(ShaderPropertyIDs.MaskTex, texture);
                        }
                        graphic.MatNorth.SetColor(ShaderPropertyIDs.ColorTwo, colorTwo);
                    }
                }
            }
            Graphic = graphic;
        }

        public PauldronTextureOption Used
        {
            get
            {
                PauldronTextureOption s = new PauldronTextureOption();
                if (!Options.NullOrEmpty())
                {
                    return activeOption ?? DefaultOption;
                }

                return s;
            }
            
            set
            {
                if (!Options.NullOrEmpty())
                {
                    activeOption = value;
                }
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
                    //    Log.Message("removing space");
                        s = Regex.Replace(Used.TexPath, " ", "");
                    }
                }
                

                return s;
            }
        }

        public List<PauldronTextureOption> Options
        {
            get
            {
                return Props.options;
            }
        }


        Mesh GetPauldronMesh(bool portrait, Pawn pawn, Rot4 facing, bool body)
        {
            return AlienRace.HarmonyPatches.GetPawnMesh(portrait, pawn, facing, body);
        }

        public bool ShouldDrawEntry(bool portrait, Rot4 bodyFacing, Vector2 size, out Graphic pauldronMaterial, out Mesh pauldronMesh, out Vector3 offset)
        {
            this.size = size;
            pauldronMaterial = null;
            offset = OffsetFor(bodyFacing);

            pauldronMesh = this.MeshSet.MeshAt(bodyFacing);
            if (pauldronMesh == null)
            {
                pauldronMesh = !Drawer.onHead ? MeshPool.humanlikeBodySet.MeshAt(bodyFacing) : MeshPool.humanlikeHeadSet.MeshAt(bodyFacing);
                if (AdeptusIntergrationUtil.enabled_AlienRaces)
                {
                    pauldronMesh = GetPauldronMesh(portrait, apparel.Wearer, bodyFacing, !Drawer.onHead);
                }
            }
            if (apparel.Wearer.RaceProps.Humanlike)
            {
                if (this.CheckPauldronRotation(bodyFacing))
                {
                    if (Graphic == null || (Graphic != null && !Graphic.path.Contains(apparel.Wearer.story.bodyType.defName)))
                    {
                //        Log.Message(string.Format("ShouldDrawPauldron UpdatePadGraphic"));
                        UpdatePadGraphic();
                    }
                    pauldronMaterial = Graphic;//.GetColoredVersion(shader, this.mainColorFor(Entry), this.secondaryColorFor(Entry)).MatAt(bodyFacing, this.parent);
                    return true;
                }
                else
                {
                    //    Log.Message(string.Format("CheckPauldronRotation false"));
                }
            }
            else
            {
                //    Log.Message(string.Format("pawn.needs = null && pawn.story = null"));
            }
            return false;

        }
        public bool CheckPauldronRotation(Rot4 bodyFacing)
        {
            if (shoulderPadType == ShoulderPadType.Left && bodyFacing == Rot4.East)
            {
                return false;
            }
            if (shoulderPadType == ShoulderPadType.Right && bodyFacing == Rot4.West)
            {
                return false;
            }
            if (shoulderPadType == ShoulderPadType.SouthOnly && bodyFacing != Rot4.South)
            {
                return false;
            }
            return true;
        }

        public GraphicMeshSet MeshSet
        {
            get
            {
                if (apparel?.Wearer == null)
                {
                    return new GraphicMeshSet(1.5f, 1.5f);
                }
                if (meshSet == null)
                {
                    meshSet = new GraphicMeshSet(size.x, size.y);
                }
                return meshSet;
            }
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref this.shoulderPadType, "shoulderPadType", ShoulderPadType.Both);
            Scribe_Defs.Look(ref this.shaderType, "shaderType");
            Scribe_Values.Look(ref this.bodyspecificTextures, "bodyspecificTextures", true);
            Scribe_Values.Look(ref this.padTexPath, "padTexPath", string.Empty);
            Scribe_Values.Look(ref this.label, "label", string.Empty);
            Scribe_Values.Look(ref this.commonality, "commonality", 1);
            Scribe_Values.Look(ref this.northtop, "northtop", false);
            Scribe_Values.Look(ref this.UseFactionTextures, "UseFactionTextures", false);
            Scribe_Values.Look(ref this.UseFactionColors, "UseFactionColors", false);
            Scribe_Values.Look(ref this.UseVariableTextures, "UseVariableTextures", false);
            Scribe_Values.Look(ref this.UsePrimaryColor, "UsePrimaryColor", true);
            Scribe_Values.Look(ref this.overridePrimaryColor, "PrimaryColor", null);
            Scribe_Values.Look(ref this.UseSecondaryColor, "UseSecondaryColor", true);
            Scribe_Values.Look(ref this.overrideSecondaryColor, "SecondaryColor", null);
            Scribe_Values.Look(ref this.order, "order", 1);
            Scribe_Values.Look(ref this.sublayer, "sublayer", 0);
            Scribe_Values.Look(ref this.size, "size");
            Scribe_Defs.Look(ref this.faction, "faction");
            Scribe_References.Look(ref this.apparel, "apparel");
        //    Scribe_References.Look(ref this.drawer, "drawer");
            Scribe_Deep.Look<PauldronTextureOption>(ref this.activeOption, "activeOption", this.defaultOption);
        //    Scribe_Collections.Look<PauldronTextureOption>(ref this.options, "Options", LookMode.Deep);
        }

        private ShoulderPadEntryProps props;
        public Vector2 size;
        public Apparel apparel;
        public ShoulderPadType shoulderPadType;
        public ShaderTypeDef shaderType;
        public bool bodyspecificTextures = true;
        public string padTexPath;
        private string label;
        public int commonality;
        public bool northtop = false;
        public bool UseFactionTextures = false;
        public bool UseFactionColors = false;
        public bool UseVariableTextures;
        public bool UsePrimaryColor = true;
        public bool UseSecondaryColorAsPrimary = false;
        public Color? overridePrimaryColor;
        public bool UseSecondaryColor = true;
        public bool UsePrimaryColorAsSecondary = false;
        public Color? overrideSecondaryColor;
        public int order = 1;
        public int sublayer = 0;
        public CompPauldronDrawer drawer = null;
        public FactionDef faction;
        private Graphic graphic;
        private GraphicMeshSet meshSet;

        public PauldronTextureOption activeOption;
        protected PauldronTextureOption defaultOption = new PauldronTextureOption(null, "Blank");
    }

}
