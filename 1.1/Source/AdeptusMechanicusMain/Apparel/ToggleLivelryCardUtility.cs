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
    public class ToggleLivelryCardUtility
    {
        // RimWorld.CharacterCardUtility
        public static Vector2 CardSize()
        {
            float width = 395f;
            return new Vector2(width, 536f);
        }

        public static float ButtonSize = 40f;

        public static float ForceButtonSize = 46f;

        public static float ForceButtonPointSize = 24f;

        public static float HeaderSize = 32f;

        public static float TextSize = 22f;

        public static float Padding = 3f;

        public static float SpacingOffset = 15f;

        public static float SectionOffset = 8f;

        public static float ColumnSize = 245f;

        public static float SkillsColumnHeight = 113f;

        public static float SkillsColumnDivider = 114f;

        public static float SkillsTextWidth = 138f;

        public static float SkillsBoxSize = 18f;

        public static float PowersColumnHeight = 195f;

        public static float PowersColumnWidth = 123f;

        public static bool isfirst = true;

        // RimWorld.CharacterCardUtility
        public static void DrawCard(Rect rect, ThingWithComps selectedThing)
        {
            GUI.BeginGroup(rect);

            var Drawer = selectedThing.TryGetComp<CompPauldronDrawer>();

            if (Drawer != null)
            {
                if (!Drawer.pauldronInitialized)
                {
                    Drawer.Initialize();
                }
                if (Drawer.Props.PauldronEntries.Any(x => x.VariantTextures != null))
                {
                    var ts = Text.CalcSize(selectedThing.def.LabelCap).x;
                    var y = rect.y;
                    var rect2 = new Rect(rect.width / 2 - ts + SpacingOffset, y, rect.width, HeaderSize);
                    y += rect2.height;
                    Text.Font = GameFont.Medium;
                    Widgets.Label(rect2, selectedThing.def.LabelCap);
                    Text.Font = GameFont.Small;
                    Widgets.ListSeparator(ref y, rect2.width, "Customiseable Parts:");


                    if (!Drawer.activeEntries.EnumerableNullOrEmpty())
                    {
                        foreach (ShoulderPadEntry entry in Drawer.activeEntries.Where(x => x.VariantTextures != null))
                        {
                            if (entry.drawer == null)
                            {
                                //entry.drawer = Drawer;
                            }
                            if (entry.VariantTextures != null)
                            {
                                if (entry.VariantTextures.Options.Count < 1)
                                {
                                    if (Drawer.Props.PauldronEntries.Any(x => x.padTexPath == entry.padTexPath && x.shoulderPadType == entry.shoulderPadType))
                                    {
                                        entry.VariantTextures.Options = Drawer.Props.PauldronEntries.Find(x => x.padTexPath == entry.padTexPath && x.shoulderPadType == entry.shoulderPadType).VariantTextures.Options;
                                    }
                                }
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
                }

            }

            GUI.EndGroup();
        }

        // Token: 0x060046E9 RID: 18153 RVA: 0x0017FDC8 File Offset: 0x0017DFC8
        public static void DrawVariantButton(Rect rect, CompPauldronDrawer comp, ShoulderPadEntry entry, bool paintable)
        {
            Rect rect1 = rect.LeftHalf().LeftHalf();
            Rect rect2 = rect.LeftHalf().RightHalf();
            rect2.width *= 2;
            entry.drawer = comp;
            Rect rect3 = rect.RightHalf().RightHalf();
            Widgets.Label(rect1, comp.GetDescription(entry.shoulderPadType));
            Widgets.Dropdown<ShoulderPadEntry, PauldronTextureOption>(rect2, entry,
                (ShoulderPadEntry sp) => sp.VariantTextures.activeOption,
                new Func<ShoulderPadEntry, IEnumerable<Widgets.DropdownMenuElement<PauldronTextureOption>>>(DrawVariantButton_GenerateMenu),
                entry.Used.Label, null, null, null, delegate ()
                {

                }, paintable);
        }
        // Token: 0x060046EB RID: 18155 RVA: 0x0017FE99 File Offset: 0x0017E099
        private static IEnumerable<Widgets.DropdownMenuElement<PauldronTextureOption>> DrawVariantButton_GenerateMenu(ShoulderPadEntry e)
        {
            yield return new Widgets.DropdownMenuElement<PauldronTextureOption>
            {
                option = new FloatMenuOption(e.VariantTextures.defaultOption.TexPath, delegate ()
                {
                    /*
                    e.VariantTextures.activeOption = e.VariantTextures.defaultOption;
                    e.UpdatePadGraphic();
                    */

                }, MenuOptionPriority.Default, null, null, 0f, null, null),
                payload = e.VariantTextures.defaultOption
            };
            using (List<PauldronTextureOption>.Enumerator enumerator = e.VariantTextures.Options.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    PauldronTextureOption variant = enumerator.Current;
                    yield return new Widgets.DropdownMenuElement<PauldronTextureOption>
                    {
                        option = new FloatMenuOption(variant.TexPath, delegate ()
                        {
                            e.VariantTextures.activeOption = variant;
                            e.UpdatePadGraphic();

                        }, MenuOptionPriority.Default, null, null, 0f, null, null),
                        payload = variant
                    };

                }
            }
            yield break;
        }


        // Token: 0x060046E9 RID: 18153 RVA: 0x0017FDC8 File Offset: 0x0017DFC8
        public static void DrawFactionButton(Rect rect, CompPauldronDrawer comp, ShoulderPadEntry entry, bool paintable)
        {
            Rect rect1 = rect.LeftHalf().LeftHalf();
            Rect rect2 = rect.LeftHalf().RightHalf();
            rect2.width *= 2;

            Rect rect3 = rect.RightHalf().RightHalf();
            Widgets.Label(rect1, comp.GetDescription(entry.shoulderPadType));
            Widgets.Dropdown<ShoulderPadEntry, PauldronTextureOption>(rect2, entry,
                (ShoulderPadEntry sp) => sp.VariantTextures.activeOption,
                new Func<ShoulderPadEntry, IEnumerable<Widgets.DropdownMenuElement<PauldronTextureOption>>>(DrawFactionButton_GenerateMenu),
                entry.Used.Label, null, null, null, delegate ()
                {
                    entry.drawer = comp;
                }, paintable);
            
        }

        // Token: 0x060046EB RID: 18155 RVA: 0x0017FE99 File Offset: 0x0017E099
        private static IEnumerable<Widgets.DropdownMenuElement<PauldronTextureOption>> DrawFactionButton_GenerateMenu(ShoulderPadEntry e)
        {
            if (e.VariantTextures.activeOption != e.VariantTextures.defaultOption)
            {
                yield return new Widgets.DropdownMenuElement<PauldronTextureOption>
                {
                    option = new FloatMenuOption(e.VariantTextures.defaultOption.TexPath, delegate ()
                    {
                        /*
                        e.VariantTextures.activeOption = e.VariantTextures.defaultOption;
                        e.UpdatePadGraphic();
                        */

                    }, MenuOptionPriority.Default, null, null, 0f, null, null),
                    payload = e.VariantTextures.defaultOption
                };
            }
            using (List<PauldronTextureOption>.Enumerator enumerator = e.VariantTextures.Options.OrderBy(x => x.Label).ToList().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    PauldronTextureOption variant = enumerator.Current;
                    if (e.VariantTextures.activeOption != variant)
                    {
                        yield return new Widgets.DropdownMenuElement<PauldronTextureOption>
                        {
                            option = new FloatMenuOption(variant.Label, delegate ()
                            {
                                Graphic graphic = e.drawer.apparel.Graphic;
                                graphic.path += "_" + variant.TexPath;
                                Color color = graphic.Color;
                                Color colorTwo = graphic.ColorTwo;
                                if (variant.factionDef != null)
                                {
                                    e.faction = variant.factionDef;
                                    if (variant.factionDef.HasModExtension<FactionDefExtension>())
                                    {
                                        FactionDefExtension extension = variant.factionDef.GetModExtension<FactionDefExtension>();
                                        if (extension.factionColor != null)
                                        {
                                            color = extension.factionColor;
                                        }
                                        if (extension.factionColorTwo != null)
                                        {
                                            colorTwo = extension.factionColorTwo;
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
                                e.drawer.apparel.SetColor(color);
                                e.drawer.apparel.SetColorTwo(colorTwo);

                                FieldInfo subgraphic = typeof(Thing).GetField("graphicInt", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
                                Traverse traverse = Traverse.Create(e.drawer.apparel);
                                subgraphic.SetValue(e.drawer.apparel, graphic);

                            //    Log.Message("set active");
                                e.VariantTextures.activeOption = variant;
                            //    Log.Message("Update PadGraphic");
                                e.UpdatePadGraphic();


                            }, MenuOptionPriority.Default, null, null, 0f, null, null),
                            payload = variant
                        };
                    }
                }
            }
            yield break;
        }

    }
}