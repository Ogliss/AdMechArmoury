using AdeptusMechanicus.ExtensionMethods;
using AdeptusMechanicus.settings;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Color = UnityEngine.Color;

namespace AdeptusMechanicus.Utility
{
    public static class ApparelAddonGraphicRecordGetter
    {
        // ApparelAddonGraphicRecordGetter.TryGetGraphicApparelAddon
        public static bool TryGetGraphicApparelAddon(ShoulderPadEntry entry, Pawn pawn, out ApparelGraphicRecord rec)
        {
            BodyTypeDef bodyType = pawn.story.bodyType;
            if (bodyType == null)
            {
                Log.Error("Getting apparel graphic with undefined body type.");
                bodyType = BodyTypeDefOf.Male;
            }
            if (entry.padTexPath.NullOrEmpty())
            {
                rec = new ApparelGraphicRecord(null, null);
                return false;
            }
            string path = entry.padTexPath;
            if (entry.useFactionTextures || entry.useVariableTextures)
            {
                bool notPlayer = pawn.Faction != null && pawn.Faction != Faction.OfPlayer;

                if (notPlayer)
                {
                    FactionDefExtension ext = pawn.Faction.def.HasModExtension<FactionDefExtension>() ? pawn.Faction.def.GetModExtensionFast<FactionDefExtension>() : null;
                    bool factionTextures = entry.useFactionTextures && ext?.factionTextureTag != null;
                    if (factionTextures)
                    {
                        //    Log.Message("using factionTextureTag " + ext.factionTextureTag);
                        for (int i = 0; i < entry.Options.Count; i++)
                        {
                            if (entry.Options[i].TexPath == ext.factionTextureTag)
                            {
                                entry.Used = entry.Options[i];
                                //    Log.Message("Found faction VariantTexture " + VariantTextures.Options[i].texPath);
                                break;
                            }
                        }
                    }
                    else
                    {
                        bool foundVar = false;
                        if (!entry.Options.NullOrEmpty())
                        {
                            for (int i = 0; i < entry.Options.Count; i++)
                            {
                                if (pawn.kindDef.apparelTags.Contains(entry.Options[i].TexPath))
                                {
                                    entry.Used = entry.Options[i];
                                    foundVar = true;
                                    //    Log.Message("Found KindDef VariantTexture " + VariantTextures.Options[i].texPath);
                                    break;
                                }
                            }
                            if (!foundVar)
                            {
                                entry.Used = entry.Options.RandomElement();
                            }
                        }
                    }
                }
                else
                {
                    if (entry.useFactionTextures)
                    {
                        //    Log.Message("UseFactionTextures");
                        //    CompColorableTwoFaction FC = Drawer.Colours as CompColorableTwoFaction;
                        if (entry.Drawer.Colours is CompColorableTwoFaction FC)
                        {
                            //    Log.Message("FC != null");
                            if (FC.FactionDef != null)
                            {
                                //    //    Log.Message("FactionDef = " + FC.FactionDef.LabelCap);
                                FactionDefExtension e = FC.Extension;
                                if (e != null)
                                {
                                    //   Log.Message("FactionDefExtension != null");
                                    if (!entry.Options.NullOrEmpty())
                                    {
                                        //    Log.Message("Options: " + Options.Count);
                                        for (int i = 0; i < entry.Options.Count; i++)
                                        {
                                            //    Log.Message("checking " + e.factionTextureTag + " Vs " + Options[i].TexPath + " = " + (Options[i].TexPath == e.factionTextureTag));
                                            if (entry.Options[i].TexPath == e.factionTextureTag)
                                            {
                                                entry.Used = entry.Options[i];
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

                                if (!entry.Options.NullOrEmpty())
                                {
                                    //    Log.Message("UpdateGraphic() 4 1 B 1 1 A 1 1 1");
                                    //    Log.Message("Options: " + Options.Count);
                                    for (int ii = 0; ii < entry.Options.Count; ii++)
                                    {
                                        //    Log.Message("checking " + e.factionTextureTag + " Vs " + Options[i].TexPath + " = " + (Options[i].TexPath == e.factionTextureTag));
                                        if (entry.Options[ii].TexPath == e.factionTextureTag)
                                        {
                                            entry.faction = f;
                                            //    //    Log.Message("Found faction VariantTexture " + Options[i].TexPath);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                path = (notPlayer && !entry.Used.padTexPathOverride.NullOrEmpty() ? entry.Used.padTexPathOverride : entry.padTexPath) + "/" + entry.Used.TexPath;
            }

            bool onHead = entry.apparel.def.apparel.LastLayer == ApparelLayerDefOf.Overhead || entry.apparel.def.apparel.LastLayer == ApparelLayerDefOf.EyeCover || entry.apparel.RenderAsPack() || entry.apparel.WornGraphicPath == BaseContent.PlaceholderImagePath || entry.apparel.WornGraphicPath == BaseContent.PlaceholderGearImagePath;
            string body = bodyTypeString(pawn.story.bodyType, !onHead && entry.bodyspecificTextures);
            string testRot = "_";
            if (CheckPauldronRotation(entry.shoulderPadType, Rot4.South))
            {
                testRot += "south";
            }
            else
            if (CheckPauldronRotation(entry.shoulderPadType, Rot4.North))
            {
                testRot += "north";
            }
            else
            if (CheckPauldronRotation(entry.shoulderPadType, Rot4.East))
            {
                testRot += "east";
            }
            else
            if (CheckPauldronRotation(entry.shoulderPadType, Rot4.West))
            {
                testRot += "west";
            }
            else
            {
                testRot = "";
            }
            string testpath = path + body + testRot;
            Texture2D tex = ContentFinder<Texture2D>.Get(testpath, false);
            if (tex == null)
            {
                //    this.UpdateProps();
                path = entry.padTexPath;
                if (entry.useFactionTextures || entry.useVariableTextures)
                {
                    path = entry.padTexPath + "/" + entry.DefaultOption.TexPath;
                }
            }

            path += body;
            Color color = entry.Drawer.mainColorFor(entry);
            Color colorTwo = entry.Drawer.secondaryColorFor(entry);
            Shader shader = ShaderDatabase.Cutout;
            ThingStyleDef styleDef = entry.apparel.StyleDef;
            if ((styleDef != null ? styleDef.graphicData.shaderType : null) != null)
            {
                shader = entry.apparel.StyleDef.graphicData.shaderType.Shader;
            }
            else if (entry.apparel.StyleDef == null && entry.apparel.def.apparel.useWornGraphicMask || entry.apparel.StyleDef != null && entry.apparel.StyleDef.UseWornGraphicMask)
            {
                shader = ShaderDatabase.CutoutComplex;
            }
            Graphic graphic = GraphicDatabase.Get<Graphic_Multi>(path, shader, entry.apparel.def.graphicData.drawSize, entry.apparel.DrawColor, entry.apparel.DrawColorTwo);

        //    if (AMAMod.Dev) Log.Message($"Graphic for {entry.apparel} {entry.Label}: {path}");
            if (entry.Drawer.Colours is CompColorableTwoFaction factionColors)
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
            rec = new ApparelGraphicRecord(graphic, entry.apparel);
            return true;
        }

        public static bool CheckPauldronRotation(ApparelAddonType shoulderPadType, Rot4 bodyFacing)
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
            if (shoulderPadType == ApparelAddonType.NorthSouth && bodyFacing != Rot4.North && bodyFacing != Rot4.South)
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
            if (shoulderPadType == ApparelAddonType.EastWest && bodyFacing != Rot4.East && bodyFacing != Rot4.West)
            {
                return false;
            }
            return true;
        }

        public static string bodyTypeString(BodyTypeDef def, bool bodyspecificTextures)
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

    }
}
