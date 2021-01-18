using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public class ITab_ToggleLivelry : ITab
    {
        public ITab_ToggleLivelry()
        {
            size = ITab_ToggleLivelry.CardSize() + new Vector2(17f, 17f) * 2f;
        }

        public override bool IsVisible
        {
            get
            {
#pragma warning disable IDE0019 // Use pattern matching
                var selected = SelThing as ThingWithComps;
#pragma warning restore IDE0019 // Use pattern matching
                if (selected != null)
                {
                    if (Drawer != null)
                    {
                        //Log.Message("ITab_isvisible");
                        labelKey = Drawer.Props.labelKey; // defined by the Comp
                        return true;
                    }
                    else if (Colorable != null)
                    {
                        labelKey = "OG_Customize".Translate(); // defined by the Comp
                        return true;
                    }
                }
                return false;
            }
        }

        private CompPauldronDrawer Drawer
        {
            get
            {
                return SelThing.TryGetComp<CompPauldronDrawer>();
            }
        }

        private CompApparelExtraPartDrawer Extra
        {
            get
            {
                return SelThing.TryGetComp<CompApparelExtraPartDrawer>();
            }
        }
        private CompColorableTwoFaction Colorable
        {
            get
            {
                return SelThing.TryGetComp<CompColorableTwoFaction>();
            }
        }

        protected override void FillTab()
        {
            var selected = Find.Selector.SingleSelectedThing as ThingWithComps;
            if (Drawer == null)
            {
                if (Colorable == null) Log.Warning("selected thing has no CompPauldronDrawer for ITab_ToggleLivelry"); 
                labelKey = "OG_Customize";
            }
            else labelKey = Drawer.Props.labelKey; //"UM_TabToggleDef";//.Translate();
            var rect = new Rect(17f, 17f, ITab_ToggleLivelry.CardSize().x, ITab_ToggleLivelry.CardSize().y);

            var ts = Text.CalcSize(selected.def.LabelCap).x;
            var y = rect.y;
            var rect2 = new Rect(rect.width / 2 - ts + SpacingOffset, y, rect.width, HeaderSize);
            y += rect2.height;
            Text.Font = GameFont.Medium;
            Widgets.Label(rect2, selected.def.LabelCap);

            if (Colorable != null)
            {
                Rect r = Colorable == null ? rect.ContractedBy(4) : rect.TopPart(0.25f).ContractedBy(4);
                this.DrawCard(r, selected, Colorable);
            }
            
            if (Drawer != null)
            {
                Rect r = Colorable == null ? rect.ContractedBy(4) : rect.BottomPart(0.75f).ContractedBy(4);
                this.DrawCard(r, selected, Drawer);
            }
            
        }

        // RimWorld.CharacterCardUtility
        public static Vector2 CardSize()
        {
            float width = 395f;
            return new Vector2(width, 536f);
        }


        // RimWorld.CharacterCardUtility
        public void DrawCard(Rect rect, ThingWithComps selectedThing, CompPauldronDrawer Drawer)
        {
            GUI.BeginGroup(rect);

            if (Drawer != null)
            {
                if (!Drawer.pauldronInitialized)
                {
                    Drawer.Initialize();
                }
                if (Drawer.Props.PauldronEntries.Any(x => !x.options.NullOrEmpty()))
                {
                    var ts = Text.CalcSize(selectedThing.def.LabelCap).x;
                    var y = rect.y;
                    var rect2 = new Rect(rect.width / 2 - ts + SpacingOffset, y, rect.width, HeaderSize);
                    Text.Font = GameFont.Small;
                    int parts = Drawer.activeEntries.Where(x => !x.Options.NullOrEmpty()).Count();
                    Widgets.ListSeparator(ref y, rect2.width, "Customiseable Parts: "+ (string)(!Drawer.activeEntries.EnumerableNullOrEmpty() ? ""+ parts : ""));


                    if (!Drawer.activeEntries.EnumerableNullOrEmpty())
                    {
                        foreach (ShoulderPadEntry entry in Drawer.activeEntries)
                        {
                            if (entry.Options.NullOrEmpty() && (entry.UseVariableTextures || entry.UseFactionTextures))
                            {
                                if (entry.Options.NullOrEmpty())
                                {
                                    if (Prefs.DevMode) Log.Message(entry.Label + " no options");
                                    continue;
                                }
                            }
                            if (entry.Drawer == null)
                            {
                                //entry.drawer = Drawer;
                            }
                            if (entry.UseFactionTextures)
                            {
                                var rect3 = new Rect(0f, y, rect.width, 20f);
                                DrawFactionButton(rect3, Drawer, entry, false);
                                y += rect2.height;
                            }
                            if (entry.UseVariableTextures)
                            {
                                var rect3 = new Rect(0f, y, rect.width, 20f);
                                DrawVariantButton(rect3, Drawer, entry, false);
                                y += rect2.height;
                            }
                        }
                    }
                    else
                    {
                        Widgets.ListSeparator(ref y, rect2.width, "No Customiseable Parts:");
                    }
                }

            }

            GUI.EndGroup();
        }
        
        // RimWorld.CharacterCardUtility
        public void DrawCard(Rect rect, ThingWithComps selectedThing, CompColorableTwoFaction Colorable)
        {
            GUI.BeginGroup(rect);

            if (Colorable != null && Colorable.Props != null)
            {
                var ts = Text.CalcSize(selectedThing.def.LabelCap).x;
                var y = rect.y;
                var rect2 = new Rect(rect.width / 2 - ts + SpacingOffset, y, rect.width, HeaderSize);
            //    y += rect2.height;
                Text.Font = GameFont.Small;
                Widgets.ListSeparator(ref y, rect2.width, "Faction Colors: ");
                if (!Colorable.Props.Key.NullOrEmpty())
                {

                    List<FactionDef> factions = new List<FactionDef>();
                    for (int i = 0; i < DefDatabase<FactionDef>.AllDefsListForReading.Count; i++)
                    {
                        FactionDef f = DefDatabase<FactionDef>.AllDefsListForReading[i];
                        FactionDefExtension extension = f.GetModExtension<FactionDefExtension>();
                        if (f.defName.Contains(Colorable.Props.Key) && extension != null)
                        {
                            if (extension.factionColor.HasValue)
                            {
                                factions.Add(f);
                            }
                        }
                    }
                    if (!factions.NullOrEmpty())
                    {
                        var rect3 = new Rect(0f, y, rect.width, 20f);
                        DrawFactionColorsButton(rect3, Colorable, false);
                        y += rect2.height;
                    }
                }

            }
            GUI.EndGroup();
        }

        public void DrawVariantButton(Rect rect, CompPauldronDrawer comp, ShoulderPadEntry entry, bool paintable)
        {
            Rect rect1 = rect.LeftHalf().LeftHalf();
            Rect rect2 = rect.LeftHalf().RightHalf();
            rect2.width *= 2;
         //   entry.Drawer = comp;
            Rect rect3 = rect.RightHalf().RightHalf();
            Widgets.Label(rect1, comp.GetDescription(entry.shoulderPadType));


            Widgets.Dropdown<ShoulderPadEntry, PauldronTextureOption>(rect2, entry, (ShoulderPadEntry p) => p.Used, new Func<ShoulderPadEntry, IEnumerable<Widgets.DropdownMenuElement<PauldronTextureOption>>>(this.DrawVariantButton_GenerateMenu), entry.Used.Label, null, null, null, null, true);
            /*
            Widgets.Dropdown<ShoulderPadEntry, PauldronTextureOption>(rect2, entry,
                (ShoulderPadEntry sp) => sp.Used,
                new Func<ShoulderPadEntry, IEnumerable<Widgets.DropdownMenuElement<PauldronTextureOption>>>(DrawVariantButton_GenerateMenu),
                entry.VariantTextures.activeOption.Label, null, null, null, delegate ()
                {

                }, paintable);
            */
        }

        private IEnumerable<Widgets.DropdownMenuElement<PauldronTextureOption>> DrawVariantButton_GenerateMenu(ShoulderPadEntry e)
        {
            if (e.Used != e.DefaultOption)
            {
                yield return new Widgets.DropdownMenuElement<PauldronTextureOption>
                {
                    option = new FloatMenuOption(e.DefaultOption.TexPath, delegate ()
                    {
                        
                        e.activeOption = e.DefaultOption;
                        if (e.UseFactionColors)
                        {
                            SetApparelColours(e, e.DefaultOption);
                        }
                        e.UpdateGraphic();
                        

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
                                if (e.UseFactionColors)
                                {
                                    SetApparelColours(e, variant);
                                }
                                e.UpdateGraphic();

                            }, MenuOptionPriority.Default, null, null, 0f, null, null),
                            payload = variant
                        };
                    }
                }
            }
            yield break;
        }


        public void SetApparelColours(ShoulderPadEntry e, PauldronTextureOption variant)
        {

            Graphic graphic = e.Drawer.apparel.DefaultGraphic;
            Color color = graphic.Color;
            Color colorTwo = graphic.ColorTwo;

            graphic = graphic.GetColoredVersion(graphic.Shader, color, colorTwo);
            e.Drawer.apparel.SetColors(color, colorTwo, true, null, graphic);
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

            e.Drawer.apparel.SetColors(color, colorTwo, true, variant.factionDef, graphic);
        }

        public void DrawFactionButton(Rect rect, CompPauldronDrawer comp, ShoulderPadEntry entry, bool paintable)
        {
            Rect rect1 = rect.LeftHalf().LeftHalf();
            Rect rect2 = rect.LeftHalf().RightHalf();
            rect2.width *= 2;

            Rect rect3 = rect.RightHalf().RightHalf();
            Widgets.Label(rect1, comp.GetDescription(entry.shoulderPadType));
            Widgets.Dropdown<ShoulderPadEntry, PauldronTextureOption>(rect2, entry,
                (ShoulderPadEntry sp) => entry.Used,
                new Func<ShoulderPadEntry, IEnumerable<Widgets.DropdownMenuElement<PauldronTextureOption>>>(DrawFactionButton_GenerateMenu),
                entry.Used.Label, null, null, null, delegate ()
                {
                //    entry.Drawer = comp;
                }, paintable);

        }

        private IEnumerable<Widgets.DropdownMenuElement<PauldronTextureOption>> DrawFactionButton_GenerateMenu(ShoulderPadEntry e)
        {
            if (e.Used != e.DefaultOption)
            {
                yield return new Widgets.DropdownMenuElement<PauldronTextureOption>
                {
                    option = new FloatMenuOption(e.DefaultOption.TexPath, delegate ()
                    {
                        Graphic graphic = e.Drawer.apparel.DefaultGraphic;
                        Color color = graphic.Color;
                        Color colorTwo = graphic.ColorTwo;

                        graphic = graphic.GetColoredVersion(graphic.Shader, color, colorTwo);
                        e.Drawer.apparel.SetColors(color, colorTwo, true, null, graphic);
                        /*
                        e.drawer.apparel.SetColorOne(color);
                        e.drawer.apparel.SetColorTwo(colorTwo);
                        FieldInfo subgraphic = typeof(Thing).GetField("graphicInt", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
                        Traverse traverse = Traverse.Create(e.drawer.apparel);
                        subgraphic.SetValue(e.drawer.apparel, graphic);
                        */
                        e.Used = e.DefaultOption;
                        e.faction = e.DefaultOption.factionDef;
                        e.UpdateGraphic();

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
                                Graphic graphic = e.Drawer.apparel.Graphic;
                                graphic.path += "_" + variant.TexPath;
                                Color color = graphic.Color;
                                Color colorTwo = graphic.ColorTwo;
                                if (variant.factionDef != null)
                                {
                                //    e.faction = variant.factionDef;
                                    if (variant.factionDef.HasModExtension<FactionDefExtension>())
                                    {
                                        FactionDefExtension extension = variant.factionDef.GetModExtension<FactionDefExtension>();
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
                                    if (e.UseFactionColors)
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

                                e.Drawer.apparel.SetColors(color, colorTwo, true, variant.factionDef, graphic);
                                /*
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


                            }, MenuOptionPriority.Default, null, null, 0f, null, null),
                            payload = variant
                        };
                    }
                }
            }
            yield break;
        }
        
        public void DrawFactionColorsButton(Rect rect, CompColorableTwoFaction comp, bool paintable)
        {
            Rect rect1 = rect.LeftHalf().LeftHalf();
            Rect rect2 = rect.LeftHalf().RightHalf();
            rect2.width *= 2;

            Rect rect3 = rect.RightHalf().RightHalf();
            Widgets.Dropdown<CompColorableTwoFaction, FactionDef>(rect2, comp,
                (CompColorableTwoFaction sp) => comp.FactionDef,
                new Func<CompColorableTwoFaction, IEnumerable<Widgets.DropdownMenuElement<FactionDef>>>(DrawFactionColorsButton_GenerateMenu),
                comp.FactionDef != null ? ((string)comp.FactionDef.LabelCap ?? comp.FactionDef.fixedName) : "None", null, null, null, delegate ()
                {
                //    entry.Drawer = comp;
                }, paintable);

        }

        // Token: 0x060046EB RID: 18155 RVA: 0x0017FE99 File Offset: 0x0017E099
        private IEnumerable<Widgets.DropdownMenuElement<FactionDef>> DrawFactionColorsButton_GenerateMenu(CompColorableTwoFaction e)
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
                    FactionDefExtension ext = enumerator.Current.GetModExtension<FactionDefExtension>();

                    if (e.FactionDef != variant && variant.defName.Contains(key))
                    {
                        if (ext == null)
                        {
                            continue;
                        }
                        if (ext.factionColor == null && ext.factionColorTwo == null && ext.factionMaskTag.NullOrEmpty() && ext.factionTextureTag.NullOrEmpty())
                        {
                            continue;
                        }
                        yield return new Widgets.DropdownMenuElement<FactionDef>
                        {
                            option = new FloatMenuOption((string)variant.LabelCap ?? variant.fixedName, delegate ()
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


                            }, MenuOptionPriority.Default, null, null, 0f, null, null),
                            payload = variant
                        };
                    }
                }
            }
            yield break;
        }

        public static float HeaderSize = 32f;

        public static float SpacingOffset = 15f;
    }
}