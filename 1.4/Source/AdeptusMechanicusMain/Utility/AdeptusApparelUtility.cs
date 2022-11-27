using AdeptusMechanicus.ExtensionMethods;
using AdeptusMechanicus.settings;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
    [StaticConstructorOnStartup]
    public static class AdeptusApparelUtility
    {
        static AdeptusApparelUtility()
        {
            foreach (var item in DefDatabase<ThingDef>.AllDefsListForReading.Where(x=> x.HasComp(typeof(CompPauldronDrawer))))
            {
                item.GetCompProperties<CompProperties_PauldronDrawer>().initalizeGraphics(item);
            }
        }

        public static bool CanCustomizeApparel(Thing thing)
        {
            if (thing is ApparelComposite apparel)
            {
                if (!apparel.Pauldrons.NullOrEmpty() || !apparel.AltGraphics.NullOrEmpty() | apparel.ColorableFaction != null)
                {
                    return true;
                }
            }
            else
            {
                if (PauldronApparel.Contains(thing.def))
                {
                    return true;
                }
                if (thing.def.HasComp(typeof(CompColorableTwoFaction)))
                {
                    return true;
                }
            }
            return false;
        }
        public static Graphic ApplyMask(Graphic Original, Apparel apparel, Color32 colorOne, Color32 colorTwo, string mskVariant, string mskFaction)
        {

            Graphic newgraphic = Original.GetColoredVersion(Original.Shader, colorOne, colorTwo);
            if (!apparel.def.apparel.wornGraphicPath.NullOrEmpty())
            {
                string varMaskPath = Original.path + mskVariant + mskFaction + "{0}" + "m";
                bool factionMask = !mskFaction.NullOrEmpty();
                bool variantMask = !mskFaction.NullOrEmpty();
                bool defaultMask = !variantMask && !factionMask;
                StringBuilder s = new StringBuilder("Setting Variable Masks for " + apparel.def.LabelCap);
                if (factionMask)
                {
                    s.AppendLine("Faction Mask Active");

                }
                if (variantMask)
                {
                    s.AppendLine("Variant Mask Active");

                }
                if (defaultMask)
                {
                    s.AppendLine("Default Mask Active");
                }
                s.AppendLine("Using Mask: " + string.Format(varMaskPath, " with Mask: "));

                if (newgraphic.MatSouth.mainTexture.name.Contains("_south") )
                {
                    string used = string.Format(varMaskPath, "_south");
                    if (newgraphic.MatSouth.GetTexture(ShaderPropertyIDs.MaskTex)?.name != used)
                    {
                        Texture texture = ContentFinder<Texture2D>.Get(used, false);
                        s.AppendLine("Checking North texture: " + (texture != null) + "\n" + used);
                        if (texture != null)
                        {
                            newgraphic.MatSouth.SetTexture(ShaderPropertyIDs.MaskTex, texture);
                        }
                    }
                    newgraphic.MatSouth.SetColor(ShaderPropertyIDs.ColorTwo, colorTwo);
                }

                if (newgraphic.MatNorth.mainTexture.name.Contains("_north"))
                {
                    string used = string.Format(varMaskPath, "_north");
                    if (newgraphic.MatSouth.GetTexture(ShaderPropertyIDs.MaskTex)?.name != used)
                    {
                        Texture texture = ContentFinder<Texture2D>.Get(used, false);
                        s.AppendLine("Checking North texture: " + (texture != null) + "\n" + used);
                        if (texture != null)
                        {
                            newgraphic.MatNorth.SetTexture(ShaderPropertyIDs.MaskTex, texture);
                        }
                    }
                    newgraphic.MatNorth.SetColor(ShaderPropertyIDs.ColorTwo, colorTwo);
                }
                if (newgraphic.MatEast.mainTexture.name.Contains("_east"))
                {
                    string used = string.Format(varMaskPath, "_east");
                    if (newgraphic.MatSouth.GetTexture(ShaderPropertyIDs.MaskTex)?.name != used)
                    {
                        Texture texture = ContentFinder<Texture2D>.Get(used, false);
                        s.AppendLine("Checking East texture: " + (texture != null) + "\n" + used);
                        if (texture != null)
                        {
                            newgraphic.MatEast.SetTexture(ShaderPropertyIDs.MaskTex, texture);
                        }
                    }
                    newgraphic.MatEast.SetColor(ShaderPropertyIDs.ColorTwo, colorTwo);
                }
                if (newgraphic.MatWest.mainTexture.name.Contains("_west"))
                {
                    string used = string.Format(varMaskPath, "_west");
                    if (newgraphic.MatSouth.GetTexture(ShaderPropertyIDs.MaskTex)?.name != used)
                    {
                        Texture texture = ContentFinder<Texture2D>.Get(used, false);
                        s.AppendLine("Checking West texture: " + (texture != null) + "\n" + used);
                        if (texture != null)
                        {
                            newgraphic.MatWest.SetTexture(ShaderPropertyIDs.MaskTex, texture);
                        }
                    }
                    newgraphic.MatWest.SetColor(ShaderPropertyIDs.ColorTwo, colorTwo);
                }
                /*
                ExtendedGraphicData newdata = new ExtendedGraphicData();
                newdata.graphicClass = typeof(Graphic_MultiMask);
            //    Log.Message("ExtendedGraphicData");
                newdata.texPath = rec.graphic.path;
                newdata.MaskSelector = msk;
                newdata.maskKey = olddata.maskKey;
                newgraphic = GraphicDatabase.Get<Graphic_MultiMask>(rec.graphic.path, rec.graphic.Shader, apparel.def.graphicData.drawSize, colorOne, colorTwo, newdata);
                */
            //    Log.Message(s.ToString());
            }
            return newgraphic;
        }

        public static void SetApparelColours(ShoulderPadEntry e, PauldronTextureOption variant)
        {

            Graphic graphic = e.Drawer.Apparel.DefaultGraphic;
            Color color = graphic.Color;
            Color colorTwo = graphic.ColorTwo;

            graphic = graphic.GetColoredVersion(graphic.Shader, color, colorTwo);
            e.Drawer.Apparel.SetColors(color, colorTwo, true, null, graphic);
            {
                if (variant.Color.HasValue)
                {
                    color = variant.Color.Value;
                }
                if (variant.ColorTwo.HasValue)
                {
                    colorTwo = variant.ColorTwo.Value;
                }
            }

            graphic = graphic.GetColoredVersion(graphic.Shader, color, colorTwo);

            e.Drawer.Apparel.SetColors(color, colorTwo, true, variant.factionDef, graphic);
            if (e.Drawer.pawn != null)
            {
                UpdateApparelGraphicsFor(e.Drawer.pawn);
            }
        }

        public static void UpdateApparelGraphicsFor(Pawn pawn)
        {
            if (pawn != null)
            {
                pawn.Drawer.renderer.graphics.ResolveApparelGraphics();
                PortraitsCache.SetDirty(pawn);
                PortraitsCache.Clear();
                PortraitsCache.PortraitsCacheUpdate();
            }
        }

        public static void DrawVariantButton(Rect rect, CompPauldronDrawer comp, ShoulderPadEntry entry, bool paintable = false)
        {
            Rect rect1 = rect.LeftHalf().LeftHalf();
            Rect rect2 = rect.LeftHalf().RightHalf();
            rect2.width *= 2;
            Rect rect3 = rect.RightHalf().RightHalf();
            Widgets.Label(rect1, comp.GetDescription(entry.shoulderPadType));
            Widgets.Dropdown<ShoulderPadEntry, PauldronTextureOption>
                (
                    rect2, 
                    entry,
                    (ShoulderPadEntry p) => p.Used, 
                    new Func<ShoulderPadEntry, IEnumerable<Widgets.DropdownMenuElement<PauldronTextureOption>>>(DrawVariantButton_GenerateMenu), 
                    entry.Used.Label, 
                    null, 
                    null, 
                    null,
                    delegate ()
                    {
                    },
                    paintable
                );
            Rect rect4 = rect3.LeftHalf().ContractedBy(2);
            Rect rect5 = rect3.RightHalf().ContractedBy(2);
            Widgets.DrawBoxSolid(rect4, entry.overridePrimaryColor ?? (entry.useSecondaryColorAsPrimary ? entry.apparel.DrawColorTwo : entry.apparel.DrawColor));
            Widgets.DrawBoxSolid(rect5, entry.overrideSecondaryColor ?? (entry.usePrimaryColorAsSecondary ? entry.apparel.DrawColor : entry.apparel.DrawColorTwo));
        }

        public static void DrawFactionButton(Rect rect, CompPauldronDrawer comp, ShoulderPadEntry entry, bool paintable = false)
        {
            Rect rect1 = rect.LeftHalf().LeftHalf();
            Rect rect2 = rect.LeftHalf().RightHalf();
            rect2.width *= 2;
            Rect rect3 = rect.RightHalf().RightHalf();
            Widgets.Label(rect1, comp.GetDescription(entry.shoulderPadType));
            Widgets.Dropdown<ShoulderPadEntry, PauldronTextureOption>
                (
                    rect2,
                    entry,
                    (ShoulderPadEntry sp) => entry.Used,
                    new Func<ShoulderPadEntry, IEnumerable<Widgets.DropdownMenuElement<PauldronTextureOption>>>(DrawFactionButton_GenerateMenu),
                    entry.Used.Label, 
                    null, 
                    null, 
                    null, 
                    delegate ()
                    {
                    }, 
                    paintable
                );
            Rect rect4 = rect3.LeftHalf().ContractedBy(2);
            Rect rect5 = rect3.RightHalf().ContractedBy(2);
            Widgets.DrawBoxSolid(rect4, entry.overridePrimaryColor ?? (entry.useSecondaryColorAsPrimary ? entry.apparel.DrawColorTwo : entry.apparel.DrawColor));
            Widgets.DrawBoxSolid(rect5, entry.overrideSecondaryColor ?? (entry.usePrimaryColorAsSecondary ? entry.apparel.DrawColor : entry.apparel.DrawColorTwo));
        }
        private static List<Color> allColors = null;
        public static List<Color> AllColors(Pawn pawn = null)
        {
            if (allColors == null)
            {
                allColors = (from x in DefDatabase<ColorDef>.AllDefsListForReading
            //                 where !x.hairOnly
                             select x into ic
                             select ic.color).ToList<Color>();
                foreach (var item in DefDatabase<FactionDef>.AllDefsListForReading)
                {
                    if (item.GetModExtensionFast<FactionDefExtension>() is FactionDefExtension ext && (ext.factionColor.HasValue || ext.factionColorTwo.HasValue))
                    {
                        if (ext.factionColor.HasValue && !allColors.Any(c => ext.factionColor.Value.IndistinguishableFrom(c)))
                        {
                            allColors.Add(ext.factionColor.Value);
                        }
                        if (ext.factionColorTwo.HasValue && !allColors.Any(c => ext.factionColorTwo.Value.IndistinguishableFrom(c)))
                        {
                            allColors.Add(ext.factionColorTwo.Value);
                        }
                    }
                }
                allColors.SortByColor((Color x) => x);
            }
            List<Color> cs = allColors;
            return cs;
        }

        public static void DrawBaseColourOptions_new(Rect rect, CompPauldronDrawer comp, ShoulderPadEntry entry, bool paintable = false)
        {
            Rect rect1 = rect.LeftHalf().LeftHalf();
            Rect rect2 = rect.LeftHalf().RightHalf();
            rect2.width *= 2;
            Rect rect3 = rect.RightHalf().RightHalf();
            Widgets.Label(rect1, comp.GetDescription(entry.shoulderPadType));
            Widgets.Dropdown<ShoulderPadEntry, PauldronTextureOption>
                (
                    rect2,
                    entry,
                    (ShoulderPadEntry sp) => entry.Used,
                    new Func<ShoulderPadEntry, IEnumerable<Widgets.DropdownMenuElement<PauldronTextureOption>>>(DrawFactionButton_GenerateMenu),
                    entry.Used.Label,
                    null,
                    null,
                    null,
                    delegate ()
                    {
                    },
                    paintable
                );
            Rect rect4 = rect3.LeftHalf().ContractedBy(2);
            Rect rect5 = rect3.RightHalf().ContractedBy(2);
            Widgets.DrawBoxSolid(rect4, entry.overridePrimaryColor ?? (entry.useSecondaryColorAsPrimary ? entry.apparel.DrawColorTwo : entry.apparel.DrawColor));
            Widgets.DrawBoxSolid(rect5, entry.overrideSecondaryColor ?? (entry.usePrimaryColorAsSecondary ? entry.apparel.DrawColor : entry.apparel.DrawColorTwo));
        }
        public static float DrawBaseColourOptions(Rect rect, string label, ApparelComposite composite, bool paintable = false)
        {
            float w = rect.width / 5;
            float w2 = (rect.width - w) / 2;
            float y = rect.y;
            float x = rect.x;
            Rect rect1 = new Rect(x, y, w, 30f);
            Widgets.Label(rect1.LeftHalf(), $"Primary {label}");
            Widgets.Label(rect1.RightHalf(), $"Secondry {label}");
            y += 30f;
            x += w;
            Rect colours = new Rect(x, y, w2, rect.height);
            Rect rect2 = colours.LeftHalf();
            Pawn_StoryTracker story = composite.Wearer.story;
            if (!composite.Wearer.apparel.IsLocked(composite))
            {
                if (composite.Wearer.Ideo != null && !Find.IdeoManager.classicMode)
                {
                    Rect ideoColour = new Rect(rect.x, y, rect.width - 10f, 24f);
                    if (Widgets.ButtonText(composite.ColorableTwo != null ? ideoColour.LeftHalf() : ideoColour, "SetIdeoColor".Translate(), true, true, true))
                    {
                        composite.colorable.color = composite.Wearer.Ideo.ApparelColor;
                        if (composite?.Wearer != null) AdeptusApparelUtility.UpdateApparelGraphicsFor(composite?.Wearer);
                        SoundDefOf.Tick_Low.PlayOneShotOnCamera(null);
                    }
                    if (composite.ColorableTwo != null && Widgets.ButtonText(ideoColour.RightHalf(), "SetIdeoColor".Translate(), true, true, true))
                    {
                        composite.ColorableTwo.ColorTwo = composite.Wearer.Ideo.ApparelColor;
                        if (composite?.Wearer != null) AdeptusApparelUtility.UpdateApparelGraphicsFor(composite?.Wearer);
                        SoundDefOf.Tick_Low.PlayOneShotOnCamera(null);
                    }
                    y += 24f;
                }
                if (story != null && story.favoriteColor != null)
                {
                    Rect favoriteColour = new Rect(rect.x, y, rect.width - 10f, 24f);
                    if (Widgets.ButtonText(composite.ColorableTwo != null ? favoriteColour.LeftHalf() : favoriteColour, "SetFavoriteColor".Translate(), true, true, true))
                    {
                        composite.colorable.color = composite.Wearer.story.favoriteColor.Value;
                        if (composite?.Wearer != null) AdeptusApparelUtility.UpdateApparelGraphicsFor(composite?.Wearer);
                        SoundDefOf.Tick_Low.PlayOneShotOnCamera(null);
                    }
                    if (composite.ColorableTwo != null && Widgets.ButtonText(favoriteColour.RightHalf(), "SetFavoriteColor".Translate(), true, true, true))
                    {
                        composite.ColorableTwo.colorTwo = composite.Wearer.story.favoriteColor.Value;
                        if (composite?.Wearer != null) AdeptusApparelUtility.UpdateApparelGraphicsFor(composite?.Wearer);
                        SoundDefOf.Tick_Low.PlayOneShotOnCamera(null);
                    }
                    y += 24f;
                }
                int entries = AllColors(composite.Wearer).Count;
                int lines = (int)((entries / 8f) + (entries % 8f == 0 ? 0 : 1));
                float height = lines * 30f;


                Rect baseColours = new Rect(rect.x, y, rect.width, height);
                if (composite.ColorableFaction != null)
                {
                    if (AdeptusWidgets.ColorSelector(baseColours.LeftHalf().ContractedBy(6), ref composite.ColorableFaction.color, AllColors(composite.Wearer), out height, null, 22, 2, composite))
                    {
                        composite.ColorableFaction.Active = true;
                    }

                    if (AdeptusWidgets.ColorSelector(baseColours.RightHalf().ContractedBy(6), ref composite.ColorableFaction.colorTwo, AllColors(composite.Wearer), out height, null, 22, 2, composite))
                    {
                        composite.ColorableFaction.ActiveTwo = true;
                    }
                    baseColours.height = height;
                }
                else
                {
                    AdeptusWidgets.ColorSelector(baseColours.LeftHalf(), ref composite.colorable.color, AllColors(composite.Wearer), out height, null, 22, 2, composite);
                    if (composite.ColorableTwo != null)
                    {
                        if (AdeptusWidgets.ColorSelector(baseColours.RightHalf(), ref composite.ColorableTwo.colorTwo, AllColors(composite.Wearer), out height, null, 22, 2, composite))
                        {
                            composite.ColorableTwo.ActiveTwo = true;
                        }
                    }
                    baseColours.height = height;
                }
                y += baseColours.height;


            }
            else
            {
                Widgets.ColorSelectorIcon(new Rect(rect2.x, rect2.y, 88f, 88f), composite.def.uiIcon, composite.colorable.color, true);
                Text.Anchor = TextAnchor.MiddleLeft;
                Rect rect3 = rect2;
                rect3.x += 100f;
                Widgets.Label(rect3, "ApparelLockedCannotRecolor".Translate(composite.Wearer.Named("PAWN"), composite.Named("APPAREL")).Colorize(ColorLibrary.RedReadable));
                Text.Anchor = TextAnchor.UpperLeft;
            }
            x += w;
            if (composite.ColorableTwo is CompColorableTwo twoColour)
            {

            }
            return y;
        }

        private static Vector2 coloursScrollPos = Vector2.zero;

        public static void DrawBaseTextureOptions(Rect rect, string label, ApparelComposite composite, bool paintable = false)
        {
            float w = rect.width / 5;
            float w2 = (rect.width - w) / 4;
            float y = rect.y;
            float x = rect.x;
            Rect rect1 = new Rect(x, y, w, 30f);
            Widgets.Label(rect1, label);
            x += w;
            Rect rect2 = new Rect(x, y, w2, 30f);
            Widgets.Dropdown<ApparelComposite, AlternateApparelGraphic>
                (
                    rect2,
                    composite,
                    (ApparelComposite p) => p.ActiveAltGraphic,
                    new Func<ApparelComposite, IEnumerable<Widgets.DropdownMenuElement<AlternateApparelGraphic>>>(DrawMainTexVariantButton_GenerateMenu),
                    composite.ActiveAltGraphic?.label.CapitalizeFirst() ?? composite.AltGraphicsExt.defaultLabel.CapitalizeFirst(),
                    null,
                    null,
                    null,
                    delegate ()
                    {
                    },
                    paintable
                );
            /*
            if (comp.FactionDef != null)
            {
                Rect rect4 = rect3.LeftHalf().ContractedBy(2);
                Rect rect5 = rect3.RightHalf().ContractedBy(2);
                Widgets.DrawBoxSolid(rect4, composite.DrawColor);
                Widgets.DrawBoxSolid(rect5, composite.DrawColorTwo);
            }
            */
            x += w;
        }

        public static void DrawFactionColorsButton(Rect rect, CompColorableTwoFaction comp, bool paintable = false)
        {
            Rect rect1 = rect.LeftHalf().LeftHalf();
            Widgets.Label(rect1, "Faction Presets: ");
            Rect rect2 = rect.LeftHalf().RightHalf();
            rect2.width *= 2;
            Rect rect3 = rect.RightHalf().RightHalf();
            Widgets.Dropdown<CompColorableTwoFaction, FactionDef>
                (
                    rect2, 
                    comp,
                    (CompColorableTwoFaction sp) => comp.FactionDef,
                    new Func<CompColorableTwoFaction, IEnumerable<Widgets.DropdownMenuElement<FactionDef>>>(DrawFactionColorsButton_GenerateMenu),
                    comp.FactionDef != null ? ((string)comp.FactionDef.fixedName ?? comp.FactionDef.LabelCap) : "None",
                    comp.FactionDef != null ? comp.FactionDef.FactionIcon : null,
                    null,
                    null,
                    delegate ()
                    {

                    }, 
                    paintable
                );
            if (comp.FactionDef != null)
            {
                Rect rect4 = rect3.LeftHalf().ContractedBy(2);
                Rect rect5 = rect3.RightHalf().ContractedBy(2);
                Widgets.DrawBoxSolid(rect4, comp.Color);
                Widgets.DrawBoxSolid(rect5, comp.ColorTwo);
            }
        }

        private static IEnumerable<Widgets.DropdownMenuElement<PauldronTextureOption>> DrawVariantButton_GenerateMenu(ShoulderPadEntry e)
        {
            if (e.Used != e.DefaultOption)
            {
                yield return new Widgets.DropdownMenuElement<PauldronTextureOption>
                {
                    option = new FloatMenuOption(e.DefaultOption.TexPath, delegate ()
                    {

                        e.activeOption = e.DefaultOption;
                        /*
                        if (e.UseFactionColors)
                        {
                            SetApparelColours(e, e.DefaultOption);
                        }
                        */
                        e.UpdateGraphic();
                        if (e.Drawer.pawn != null)
                        {
                            UpdateApparelGraphicsFor(e.Drawer.pawn);
                        }



                    }, MenuOptionPriority.Default, null, null, 0f, null, null),
                    payload = e.DefaultOption
                };
            }
            using (List<PauldronTextureOption>.Enumerator enumerator = e.Options.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    PauldronTextureOption variant = enumerator.Current;
                    if (e.Used != variant)
                    {
                        yield return new Widgets.DropdownMenuElement<PauldronTextureOption>
                        {
                            option = new FloatMenuOption(variant.Label.CapitalizeFirst() ?? variant.TexPath, delegate ()
                            {
                                e.Used = variant;
                                /*
                                if (e.UseFactionColors)
                                {
                                    SetApparelColours(e, variant);
                                }
                                */
                                e.UpdateGraphic();
                                if (e.Drawer.pawn != null)
                                {
                                    UpdateApparelGraphicsFor(e.Drawer.pawn);
                                }


                            }, MenuOptionPriority.Default, null, null, 0f, null, null),
                            payload = variant
                        };
                    }
                }
            }
            yield break;
        }

        private static IEnumerable<Widgets.DropdownMenuElement<PauldronTextureOption>> DrawFactionButton_GenerateMenu(ShoulderPadEntry e)
        {
            if (e.Used != e.DefaultOption)
            {
                yield return new Widgets.DropdownMenuElement<PauldronTextureOption>
                {
                    option = new FloatMenuOption(e.DefaultOption.TexPath, delegate ()
                    {
                        Graphic graphic = e.Drawer.Apparel.DefaultGraphic;
                        Color color = graphic.Color;
                        Color colorTwo = graphic.ColorTwo;

                        graphic = graphic.GetColoredVersion(graphic.Shader, color, colorTwo);
                        /*
                        e.Drawer.apparel.SetColors(color, colorTwo, true, null, graphic);
                        e.drawer.apparel.SetColorOne(color);
                        e.drawer.apparel.SetColorTwo(colorTwo);
                        FieldInfo subgraphic = typeof(Thing).GetField("graphicInt", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
                        Traverse traverse = Traverse.Create(e.drawer.apparel);
                        subgraphic.SetValue(e.drawer.apparel, graphic);
                        */
                        e.Used = e.DefaultOption;
                        e.faction = e.DefaultOption.factionDef;
                        e.UpdateGraphic();
                        if (e.Drawer.pawn != null)
                        {
                            UpdateApparelGraphicsFor(e.Drawer.pawn);
                        }

                    }, MenuOptionPriority.Default, null, null, 0f, null, null),
                    payload = e.DefaultOption
                };
            }
            using (List<PauldronTextureOption>.Enumerator enumerator = e.Options.OrderBy(x => x.Label).ToList().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    PauldronTextureOption variant = enumerator.Current;
                    if (e.Used != variant)
                    {
                        yield return new Widgets.DropdownMenuElement<PauldronTextureOption>
                        {
                            option = new FloatMenuOption(variant.Label.CapitalizeFirst() ?? variant.TexPath, delegate ()
                            {
                                Graphic graphic = e.Drawer.Apparel.Graphic;
                                if (!variant.padTexPathOverride.NullOrEmpty())
                                {
                                    graphic.path = variant.padTexPathOverride;
                                }
                                graphic.path += "_" + variant.TexPath;
                                Color color = graphic.Color;
                                Color colorTwo = graphic.ColorTwo;
                                if (variant.factionDef != null)
                                {
                                    //    e.faction = variant.factionDef;
                                    if (variant.factionDef.HasModExtension<FactionDefExtension>())
                                    {
                                        FactionDefExtension extension = variant.factionDef.GetModExtensionFast<FactionDefExtension>();
                                        if (extension.factionColor != null)
                                        {
                                            color = extension.factionColor.Value;
                                        }
                                        if (extension.factionColorTwo != null)
                                        {
                                            colorTwo = extension.factionColorTwo.Value;
                                        }
                                    }
                                }
                                else
                                {
                                    if (e.useFactionColors)
                                    {
                                        if (variant.Color.HasValue)
                                        {
                                            color = variant.Color.Value;
                                        }
                                        if (variant.ColorTwo.HasValue)
                                        {
                                            colorTwo = variant.ColorTwo.Value;
                                        }
                                    }
                                }

                                graphic = graphic.GetColoredVersion(graphic.Shader, color, colorTwo);

                                /*
                                e.Drawer.apparel.SetColors(color, colorTwo, true, variant.factionDef, graphic);
                                e.drawer.apparel.SetColorOne(color);
                                e.drawer.apparel.SetColorTwo(colorTwo);

                                FieldInfo subgraphic = typeof(Thing).GetField("graphicInt", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
                                Traverse traverse = Traverse.Create(e.drawer.apparel);
                                subgraphic.SetValue(e.drawer.apparel, graphic);
                                */
                                //    Log.Message("set active");

                                e.faction = variant.factionDef;
                                e.Used = variant;
                                //    Log.Message("Update PadGraphic");
                                e.UpdateGraphic();
                                if (e.Drawer.pawn != null)
                                {
                                    UpdateApparelGraphicsFor(e.Drawer.pawn);
                                }


                            }, MenuOptionPriority.Default, null, null, 0f, null, null),
                            payload = variant
                        };
                    }
                }
            }
            yield break;
        }

		private static IEnumerable<Widgets.DropdownMenuElement<AlternateApparelGraphic>> DrawMainTexVariantButton_GenerateMenu(ApparelComposite composite)
		{
			if (composite.ActiveAltGraphic != null)
			{
				yield return new Widgets.DropdownMenuElement<AlternateApparelGraphic>
				{
					option = new FloatMenuOption(composite.AltGraphicsExt.defaultLabel.CapitalizeFirst(), delegate ()
					{

						composite.ActiveAltGraphic = null;
                        if (composite.Wearer != null)
                        {
                            UpdateApparelGraphicsFor(composite.Wearer);
                        }


                    },composite.def, null, false, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0),
					payload = null
				};
			}
			using (List<AlternateApparelGraphic>.Enumerator enumerator = composite.AltGraphics.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					AlternateApparelGraphic variant = enumerator.Current;
					if (composite.ActiveAltGraphic != variant)
					{
						yield return new Widgets.DropdownMenuElement<AlternateApparelGraphic>
						{
							option = new FloatMenuOption(variant.label.CapitalizeFirst() ?? variant.texPath.CapitalizeFirst(), delegate ()
							{
								composite.ActiveAltGraphic = variant;
                                if (composite.Wearer != null)
                                {
                                    UpdateApparelGraphicsFor(composite.Wearer);
                                }

                            }, MenuOptionPriority.Default, null, null, 0f, null, null),
							payload = variant
						};
					}
				}
			}
			yield break;
		}
        
		private static IEnumerable<Widgets.DropdownMenuElement<FactionDef>> DrawFactionColorsButton_GenerateMenu(CompColorableTwoFaction e)
		{
			if (e.FactionDef != null)
			{
				yield return new Widgets.DropdownMenuElement<FactionDef>
				{
					option = new FloatMenuOption("None", delegate ()
					{
						/*
                        Graphic graphic = e.parent.DefaultGraphic;
                        Color color = graphic.Color;
                        Color colorTwo = graphic.ColorTwo;

                        graphic = graphic.GetColoredVersion(graphic.Shader, color, colorTwo);
                        e.parent.SetColors(color, colorTwo, true, null, graphic);
                        */
						/*
                        e.drawer.apparel.SetColorOne(color);
                        e.drawer.apparel.SetColorTwo(colorTwo);
                        FieldInfo subgraphic = typeof(Thing).GetField("graphicInt", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
                        Traverse traverse = Traverse.Create(e.drawer.apparel);
                        subgraphic.SetValue(e.drawer.apparel, graphic);
                        */
						e.FactionDef = null;
						Graphic graphic = e.parent.DefaultGraphic;
						Color color = e.Color;
						Color colorTwo = e.ColorTwo;

						graphic = graphic.GetColoredVersion(graphic.Shader, color, colorTwo);

						e.parent.SetColors(color, colorTwo, true, e.FactionDef, graphic);
                        if (e.parent is Apparel apparel)
                        {
                            if (apparel.Wearer != null)
                            {
                                UpdateApparelGraphicsFor(apparel.Wearer);
                            }
                        }


                    }, MenuOptionPriority.Default, null, null, 0f, null, null),
					payload = null
				};
			}
			string key = e.Props.Key;
			using (List<FactionDef>.Enumerator enumerator = e.ColouredDefs.OrderBy(x => x.label).ToList().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FactionDef variant = enumerator.Current;
					FactionDefExtension ext = enumerator.Current.GetModExtensionFast<FactionDefExtension>();

                    if (ext == null)
                    {
                        continue;
                    }
                    if (e.FactionDef != variant && (variant.defName.Contains(key) || ext.factionColourTag.Contains(key)))
					{
						if (ext.factionColor == null && ext.factionColorTwo == null && ext.factionMaskTag.NullOrEmpty() && ext.factionTextureTag.NullOrEmpty())
						{
							continue;
						}
						yield return new Widgets.DropdownMenuElement<FactionDef>
						{
							option = new FloatMenuOption((string)variant.fixedName ?? variant.LabelCap, delegate ()
							{
								/*
                                Graphic graphic = e.parent.Graphic;
                            //    graphic.path += "_" + variant.TexPath;
                                Color color = graphic.Color;
                                Color colorTwo = graphic.ColorTwo;
                                if (ext.factionColor != null)
                                {
                                    color = ext.factionColor.Value;
                                }
                                if (ext.factionColorTwo != null)
                                {
                                    colorTwo = ext.factionColorTwo.Value;
                                }
                                graphic = graphic.GetColoredVersion(graphic.Shader, color, colorTwo);

                                e.parent.SetColors(color, colorTwo, true, variant, graphic);
                                Texture texture;
                                if (!ext.factionMaskTag.NullOrEmpty())
                                {
                                    string msk = "m_" + ext.factionMaskTag;
                                    texture = ContentFinder<Texture2D>.Get(graphic.path + msk, false);
                                    if (texture != null)
                                    {
                                        graphic.MatEast.SetTexture(ShaderPropertyIDs.MaskTex, texture);
                                    }
                                    graphic.MatEast.SetColor(ShaderPropertyIDs.ColorTwo, colorTwo);
                                }
                                */

								/*
                                e.drawer.apparel.SetColorOne(color);
                                e.drawer.apparel.SetColorTwo(colorTwo);

                                FieldInfo subgraphic = typeof(Thing).GetField("graphicInt", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
                                Traverse traverse = Traverse.Create(e.drawer.apparel);
                                subgraphic.SetValue(e.drawer.apparel, graphic);
                                */
								//    Log.Message("set active");

								e.FactionDef = variant;
								Graphic graphic = e.parent.DefaultGraphic;
								Color color = e.Color;
								Color colorTwo = e.ColorTwo;

								graphic = graphic.GetColoredVersion(graphic.Shader, color, colorTwo);

								e.parent.SetColors(color, colorTwo, true, e.FactionDef, graphic);
                                if (e.parent is Apparel apparel)
                                {
                                    if (apparel.Wearer != null)
                                    {
                                        UpdateApparelGraphicsFor(apparel.Wearer);
                                    }
                                }


                            }, MenuOptionPriority.Default, null, null, 0f, null, null),
							payload = variant
						};
					}
				}
			}
			yield break;
        }
        
        private static List<ThingDef> pauldronApparel;
        public static List<ThingDef> PauldronApparel
        {
            get
            {
                if (pauldronApparel == null)
                {
                    pauldronApparel = new List<ThingDef>();
                    pauldronApparel = DefDatabase<ThingDef>.AllDefsListForReading.FindAll(x => x.HasComp(typeof(CompPauldronDrawer)));
                    if (AMAMod.Dev)
                    {
                    //    Log.Message("Generated PauldronApparel list: " + pauldronApparel.Count);
                    }
                }
                return pauldronApparel;
            }
        }
        private static List<ThingDef> pauldronApparelCustomizeable;
        public static List<ThingDef> PauldronApparelCustomizeable
        {
            get
            {
                if (pauldronApparelCustomizeable == null)
                {
                    pauldronApparelCustomizeable = new List<ThingDef>();
                    foreach (var item in pauldronApparel)
                    {
                        CompProperties_PauldronDrawer drawer = item.GetCompProperties<CompProperties_PauldronDrawer>();
                        if (drawer != null)
                        {
                            if (drawer.PauldronEntries.Any(x=> x.UseFactionTextures || x.UseVariableTextures))
                            {
                                pauldronApparelCustomizeable.Add(item);
                            }
                        }
                    }
                    if (AMAMod.Dev)
                    {
                    //    Log.Message("Generated PauldronApparel list: " + pauldronApparelCustomizeable.Count);
                    }
                }
                return pauldronApparelCustomizeable;
            }
        }

        private static List<ThingDef> extraPartApparel;
        public static List<ThingDef> ExtraPartApparel
        {
            get
            {
                if (extraPartApparel == null)
                {
                    extraPartApparel = new List<ThingDef>();
                    extraPartApparel = DefDatabase<ThingDef>.AllDefsListForReading.FindAll(x => x.HasComp(typeof(CompApparelExtraPartDrawer)));
                    if (AMAMod.Dev)
                    {
                    //    Log.Message("Generated ExtraPartApparel list: " + extraPartApparel.Count);
                    }
                }
                return extraPartApparel;
            }
        }

        private static List<HediffDef> graphicHediffs;
        public static List<HediffDef> GraphicHediffs
        {
            get
            {

                if (graphicHediffs == null)
                {

                    graphicHediffs = new List<HediffDef>();
                    graphicHediffs = DefDatabase<HediffDef>.AllDefsListForReading.FindAll(x => x.HasComp(typeof(HediffComp_DrawImplant_AdMech)));
                    if (AMAMod.Dev)
                    {
                    //    Log.Message("Generated GraphicHediffs list: " + graphicHediffs.Count);
                    }
                }
                return graphicHediffs;
            }
        }

    }

}
