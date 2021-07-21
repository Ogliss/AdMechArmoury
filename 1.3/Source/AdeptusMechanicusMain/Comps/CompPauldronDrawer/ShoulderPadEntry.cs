using AdeptusMechanicus.ExtensionMethods;
using AdeptusMechanicus.HarmonyInstance;
using HarmonyLib;
using RimWorld;
using System;
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

        public ShoulderPadEntry(ShoulderPadProperties entry, CompPauldronDrawer drawer)
        {
            UpdateProps(entry);
            this.drawer = drawer;
            this.apparel = drawer.Apparel;
        }

        public void UpdateProps(ShoulderPadProperties entry)
        {
            this.props = entry;
            this.label = entry.label;
            this.shoulderPadType = entry.shoulderPadType;
            this.shaderType = entry.shaderType;
            this.bodyspecificTextures = entry.bodyspecificTextures;
            this.padTexPath = entry.padTexPath;
            if (!entry.label.NullOrEmpty())
            {
                this.label = entry.label;
            }
            this.commonality = entry.commonality;
            this.northalt = entry.northtop;
            this.southalt = entry.southtop;
            this.eastalt = entry.easttop;
            this.westalt = entry.westtop;
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
                if (activeOption == null)
                {
                    this.activeOption = entry.defaultOption;
                }
            }
        }

        public void UpdateProps()
        {
            ShoulderPadProperties newprops = null;
            for (int i = 0; i < Drawer.Props.PauldronEntries.Count; i++)
            {
                ShoulderPadProperties e = Drawer.Props.PauldronEntries[i];
                if ((this.padTexPath == e.padTexPath && this.shoulderPadType == e.shoulderPadType) || (!this.label.NullOrEmpty() && !e.label.NullOrEmpty() && this.label == e.label))
                {
                    newprops = e;
                    break;
                }
            }
            if (newprops == null && Drawer.Props.PauldronEntries.Any(x=> x.shoulderPadType == this.shoulderPadType))
            {
                newprops = Drawer.Props.PauldronEntries.Find(x => x.shoulderPadType == this.shoulderPadType);
            }
            if (newprops != null)
            {
                UpdateProps(newprops);
            }
        }

        public const float MinClippingDistance = 0.0015f;   // Minimum space between layers to avoid z-fighting
        private const float YOffset_Utility_South = PawnRenderer.YOffset_Utility_South;
        private const float YOffset_Shell = PawnRenderer.YOffset_Shell + MinClippingDistance;
        private const float YOffset_Head = PawnRenderer.YOffset_Head + MinClippingDistance;
        private const float YOffset_Utility = PawnRenderer.YOffset_Utility + MinClippingDistance;
        private const float YOffset_OnHead = PawnRenderer.YOffset_OnHead + MinClippingDistance;
        private const float YOffset_PostHead = PawnRenderer.YOffset_PostHead + MinClippingDistance;
        private const float YOffset_CarriedThing = PawnRenderer.YOffset_CarriedThing - MinClippingDistance;
        public Shader Shader => shaderType.Shader;
        public Vector3 NorthOffset => this.Props.NorthOffset;
        public Vector3 SouthOffset => this.Props.SouthOffset;
        public Vector3 EastOffset => this.Props.EastOffset;
        public Vector3 WestOffset => this.Props.WestOffset;
        public float AltOffet(string alt)
        {
            switch (alt.CapitalizeFirst())
            {
                case "Utility_South":
                    return YOffset_Utility_South;
                case "Shell":
                    return YOffset_Shell;
                case "Head":
                    return YOffset_Head;
                case "Utility":
                    return YOffset_Utility;
                case "OnHead":
                    return YOffset_OnHead;
                case "PostHead":
                    return YOffset_PostHead;
                case "CarriedThing":
                    return YOffset_CarriedThing;
                default:
                    if (alt.NullOrEmpty())
                    {
                        return 0f;
                    }
                    return YOffset_Shell;
            }
        }
        public Vector3 OffsetFor(Rot4 rot)
        {
            Vector3 vector = new Vector3();
            string alt = string.Empty;
            float altOffset = 0f;
            if (rot == Rot4.North)
            {
                vector = NorthOffset;
                altOffset = AltOffet(northalt);
                alt = northalt;
            }
            else
            if (rot == Rot4.South)
            {
                vector = SouthOffset;
                altOffset = AltOffet(southalt);
                alt = northalt;
            }
            else
            if (rot == Rot4.East)
            {
                vector = EastOffset;
                altOffset = AltOffet(eastalt);
                alt = northalt;
            }
            else
            if (rot == Rot4.West)
            {
                vector = WestOffset;
                altOffset = AltOffet(westalt);
                alt = northalt;
            }
            vector.y += Math.Min(altOffset, YOffset_CarriedThing);
            // vector.y = Math.Min(Math.Min(vector.y, altOffset), YOffset_CarriedThing);
            //   Log.Message("Offset for " + rot.ToStringHuman() +" at alt: " + alt + ": " + vector);
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
                        drawer = apparel.TryGetCompFast<CompPauldronDrawer>();
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
                //    Log.Message("Graphic UpdatieGraphic");
                    UpdateGraphic();
                }
                return graphic;
            }
            set
            {
                graphic = value;
            }
        }

        public ShoulderPadProperties Props
        {
            get
            {
                if (props == null)
                {
                    props = new ShoulderPadProperties();
                    if (Drawer != null)
                    {
                        UpdateProps();
                    }
                }
                return props;
            }
        }

        public string bodyTypeString(BodyTypeDef def)
        {
            string body = string.Empty;
            if (bodyspecificTextures)
            {
                body = "_";
                if (def.ToString().Contains("Female"))
                {
                    body += "Female";
                }
                else
                if (def.ToString().Contains("Male"))
                {
                    body += "Male";
                }
                else
                if (def.ToString().Contains("Fat"))
                {
                    body += "Fat";
                }
                else
                if (def.ToString().Contains("Thin"))
                {
                    body += "Thin";
                }
                else
                if (def.ToString().Contains("Hulk"))
                {
                    body += "Hulk";
                }
                else body += def.ToString();
                //    Log.Message("bodyspecificTextures: "+ path + "_" + body);
            }
            return body;
        }

        public bool ValidatePat()
        {

            return true;
        }

        public void UpdateGraphic()
        {
            if (Drawer?.pawn == null)
            {
                Graphic = null;
                return;
            }
            if (apparel.Wearer == null)
            {
                if (Drawer != null)
                {
                    Drawer.pawn = null;
                }
                Graphic = null;
                return;
            }
            if (Drawer.pawn != apparel.Wearer)
            {
            //    Log.Message("Old Wearer: "+ Drawer.pawn + "new Wearer: "+ apparel.Wearer);
                Drawer.pawn = apparel.Wearer;
            }
            Pawn pawn = Drawer.pawn;
            Shader shader = this.Shader;
            string path = padTexPath;
            if (UseFactionTextures || UseVariableTextures)
            {
                bool notPlayer = pawn.Faction != null && (pawn.Faction != Faction.OfPlayer);
                if (notPlayer)
                {
                    FactionDefExtension ext = pawn.Faction.def.HasModExtension<FactionDefExtension>() ? pawn.Faction.def.GetModExtensionFast<FactionDefExtension>() : null;
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
                    //    CompColorableTwoFaction FC = Drawer.Colours as CompColorableTwoFaction;
                        if (Drawer.Colours is CompColorableTwoFaction FC)
                        {
                            //    Log.Message("FC != null");
                            if (FC.FactionDef != null)
                            {
                                //    //    Log.Message("FactionDef = " + FC.FactionDef.LabelCap);
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
                                        //    //    Log.Message("Found faction VariantTexture " + Options[i].TexPath);
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
                        //    Log.Message("UpdateGraphic() 4 1 B 1 1 B 2 factions: "+ factions.Count);
                            for (int i = 0; i < factions.Count; i++)
                            {
                            //    Log.Message("UpdateGraphic() 4 1 B 1 1 B 2 faction: " + i);
                                FactionDef f = factions[i];

                            //    Log.Message("UpdateGraphic() 4 1 B 1 1 B 2 faction: " + f);
                                FactionDefExtension e = f.HasModExtension<FactionDefExtension>() ? f.GetModExtensionFast<FactionDefExtension>() : null;
                            //    Log.Message("UpdateGraphic() 4 1 B 1 1 B 2 faction: " + (e == null));
                                if (e == null)
                                {
                                //    Log.Message("e == null");
                                    continue;
                                }
                                if (e.factionTextureTag.NullOrEmpty())
                                {
                                //    Log.Message("factionTextureTag == null");
                                    continue;
                                }

                                if (!Options.NullOrEmpty())
                                {
                                //    Log.Message("UpdateGraphic() 4 1 B 1 1 A 1 1 1");
                                    //    Log.Message("Options: " + Options.Count);
                                    for (int ii = 0; ii < Options.Count; ii++)
                                    {
                                        //    Log.Message("checking " + e.factionTextureTag + " Vs " + Options[i].TexPath + " = " + (Options[i].TexPath == e.factionTextureTag));
                                        if (Options[ii].TexPath == e.factionTextureTag)
                                        {
                                            faction = f;
                                            //    //    Log.Message("Found faction VariantTexture " + Options[i].TexPath);
                                            break;
                                        }
                                    }
                                }

                            }
                        }
                    }
                }

                path = padTexPath + "/" + Used.TexPath;
            }

            string body = bodyTypeString(pawn.story.bodyType);
            string testRot = "_";
            if (CheckPauldronRotation(Rot4.South))
            {
                testRot += "south";
            }
            else
            if (CheckPauldronRotation(Rot4.North))
            {
                testRot += "north";
            }
            else
            if (CheckPauldronRotation(Rot4.East))
            {
                testRot += "east";
            }
            else
            if (CheckPauldronRotation(Rot4.West))
            {
                testRot += "west";
            }
            else
            {
                testRot = "";
            }
            string testpath = path + body + testRot;
            Texture2D tex = ContentFinder<Texture2D>.Get(testpath, false);
            Graphic graphic;
            if (tex == null)
            {
                this.UpdateProps();
                path = padTexPath;
                if (UseFactionTextures || UseVariableTextures)
                {
                    path = padTexPath + "/" + this.DefaultOption.TexPath;
                }
            }
            path += body;
            Color color = Drawer.mainColorFor(this);
            Color colorTwo = Drawer.secondaryColorFor(this);
            graphic = GraphicDatabase.Get<Graphic_Multi>(path, shader, size, color, colorTwo);
            if (Drawer.Colours is CompColorableTwoFaction factionColors)
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
            //    Log.Message(this.Label + " " + graphic.path + " Shader: " + graphic.Shader.name + "Colour: " + graphic.Color + " Colour: " + graphic.ColorTwo);
            /*
            if (!this.Drawer.apparel.def.apparel.wornGraphicPath.NullOrEmpty())
            {
                SetApparelColours();
            }
            */
            Graphic = graphic;
        }

        public void SetApparelColours()
        {

            Graphic graphic = this.Drawer.Apparel.DefaultGraphic;
            Color color = this.Drawer.Apparel.DrawColor;
            Color colorTwo = this.Drawer.Apparel.DrawColorTwo;

            graphic = graphic.GetColoredVersion(graphic.Shader, color, colorTwo);
            //    this.Drawer.apparel.SetColors(color, colorTwo, true, null, graphic);
            if (this.activeOption != null)
            {
                if (activeOption.Color.HasValue)
                {
                    color = activeOption.Color.Value;
                }
                if (activeOption.ColorTwo.HasValue)
                {
                    colorTwo = activeOption.ColorTwo.Value;
                }
                graphic = graphic.GetColoredVersion(graphic.Shader, color, colorTwo);
                this.Drawer.Apparel.SetColors(color, colorTwo, true, activeOption?.factionDef ?? null, graphic);
            }
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

        public bool ShouldDrawEntry(bool portrait, Rot4 bodyFacing, Vector2 size, bool renderBody, out Graphic pauldronMaterial, out Mesh pauldronMesh, out Vector3 offset)
        {
            this.size = size;
            pauldronMaterial = null;
            offset = OffsetFor(bodyFacing);
            if (!renderBody)
            {
                if (!Drawer.onHead || (Props.drawInBed.HasValue && Props.drawInBed.Value == false))
                {
                    pauldronMesh = null;
                    return false;
                }
            }
            pauldronMesh = this.MeshSet.MeshAt(bodyFacing);
            if (pauldronMesh == null)
            {
                pauldronMesh = !Drawer.onHead ? MeshPool.humanlikeBodySet.MeshAt(bodyFacing) : MeshPool.humanlikeHeadSet.MeshAt(bodyFacing);
                if (AdeptusIntergrationUtility.enabled_AlienRaces)
                {
                    pauldronMesh = GetPauldronMesh(portrait, apparel.Wearer, bodyFacing, !Drawer.onHead);
                }
            }
            if (apparel.Wearer.RaceProps.Humanlike)
            {
                if (this.CheckPauldronRotation(bodyFacing))
                {
                    if (Graphic == null || (Graphic != null && Drawer.pawn != apparel.Wearer))
                    {
                //    //    Log.Message(string.Format("ShouldDrawPauldron UpdatePadGraphic"));
                        UpdateGraphic();
                    }
                    pauldronMaterial = Graphic;//.GetColoredVersion(shader, this.mainColorFor(Entry), this.secondaryColorFor(Entry)).MatAt(bodyFacing, this.parent);
                    return true;
                }
            }
            return false;

        }

        public bool CheckPauldronRotation(Rot4 bodyFacing)
        {
            if (shoulderPadType == ApparelAddonType.Left && bodyFacing == Rot4.East)
            {
                return false;
            }
            if (shoulderPadType == ApparelAddonType.Right && bodyFacing == Rot4.West)
            {
                return false;
            }
            if (shoulderPadType == ApparelAddonType.SouthOnly && bodyFacing != Rot4.South)
            {
                return false;
            }
            if (shoulderPadType == ApparelAddonType.NotSouth && bodyFacing == Rot4.South)
            {
                return false;
            }
            if (shoulderPadType == ApparelAddonType.NorthOnly && bodyFacing != Rot4.North)
            {
                return false;
            }
            if (shoulderPadType == ApparelAddonType.NotNorth && bodyFacing == Rot4.North)
            {
                return false;
            }
            if (shoulderPadType == ApparelAddonType.NorthSouth && (bodyFacing != Rot4.North && bodyFacing != Rot4.South))
            {
                return false;
            }
            if (shoulderPadType == ApparelAddonType.EastOnly && bodyFacing != Rot4.East)
            {
                return false;
            }
            if (shoulderPadType == ApparelAddonType.NotEast && bodyFacing == Rot4.East)
            {
                return false;
            }
            if (shoulderPadType == ApparelAddonType.WestOnly && bodyFacing != Rot4.West)
            {
                return false;
            }
            if (shoulderPadType == ApparelAddonType.NotWest && bodyFacing == Rot4.West)
            {
                return false;
            }
            if (shoulderPadType == ApparelAddonType.EastWest && (bodyFacing != Rot4.East && bodyFacing != Rot4.West))
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
            Scribe_Values.Look(ref this.shoulderPadType, "shoulderPadType", ApparelAddonType.Both);
            Scribe_Defs.Look(ref this.shaderType, "shaderType");
            Scribe_Values.Look(ref this.bodyspecificTextures, "bodyspecificTextures", true);
            Scribe_Values.Look(ref this.padTexPath, "padTexPath", string.Empty);
            Scribe_Values.Look(ref this.label, "label", string.Empty);
            Scribe_Values.Look(ref this.commonality, "commonality", 1);
            Scribe_Values.Look(ref this.northalt, "northalt", "PostHead");
            Scribe_Values.Look(ref this.southalt, "southalt", "Shell");
            Scribe_Values.Look(ref this.eastalt, "eastalt", "Shell");
            Scribe_Values.Look(ref this.westalt, "westalt", "Shell");
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
        //    Scribe_References.Look(ref this.wearer, "lastWearer");
            Scribe_Deep.Look<PauldronTextureOption>(ref this.activeOption, "activeOption", this.defaultOption);
        //    Scribe_Collections.Look<PauldronTextureOption>(ref this.options, "Options", LookMode.Deep);
        }
        /*
        private Pawn wearer;
        public Pawn pawn
        {
            get
            {
                if (wearer == null)
                {
                    wearer = this.apparel.Wearer;
                }
                return wearer;
            }
        }
        */
        private ShoulderPadProperties props;
        public Vector2 size;
        public Apparel apparel;
        public ApparelAddonType shoulderPadType;
        public ShaderTypeDef shaderType;
        public bool bodyspecificTextures = true;
        public string padTexPath;
        private string label;
        public int commonality;
        public string northalt = "Shell";
        public string southalt = "Shell";
        public string eastalt = "Shell";
        public string westalt = "Shell";
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
