using AdeptusMechanicus.ExtensionMethods;
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
                        labelKey = "AdeptusMechanicus.Customize".Translate(); // defined by the Comp
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
                return SelThing.TryGetCompFast<CompPauldronDrawer>();
            }
        }

        private CompApparelExtraPartDrawer Extra
        {
            get
            {
                return SelThing.TryGetCompFast<CompApparelExtraPartDrawer>();
            }
        }
        private CompColorableTwoFaction Colorable
        {
            get
            {
                return SelThing.TryGetCompFast<CompColorableTwoFaction>();
            }
        }

        public override void FillTab()
        {
            var selected = Find.Selector.SingleSelectedThing as ThingWithComps;
            if (Drawer == null)
            {
                if (Colorable == null) Log.Warning("selected thing has no CompPauldronDrawer for ITab_ToggleLivelry"); 
                labelKey = "AdeptusMechanicus.Customize";
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
                                AdeptusApparelUtility.DrawFactionButton(rect3, Drawer, entry, false);
                                y += rect2.height;
                            }
                            if (entry.UseVariableTextures)
                            {
                                var rect3 = new Rect(0f, y, rect.width, 20f);
                                AdeptusApparelUtility.DrawVariantButton(rect3, Drawer, entry, false);
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
                Widgets.ListSeparator(ref y, rect2.width, "Colours: ");
                if (!Colorable.Props.Key.NullOrEmpty())
                {

                    List<FactionDef> factions = new List<FactionDef>();
                    for (int i = 0; i < DefDatabase<FactionDef>.AllDefsListForReading.Count; i++)
                    {
                        FactionDef f = DefDatabase<FactionDef>.AllDefsListForReading[i];
                        FactionDefExtension extension = f.GetModExtensionFast<FactionDefExtension>();
                        if (extension != null && (extension.factionColourTag.Contains(Colorable.Props.Key) || f.defName.Contains(Colorable.Props.Key)))
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
                        AdeptusApparelUtility.DrawFactionColorsButton(rect3, Colorable, false);
                        y += rect2.height;
                    }
                    if (!factions.NullOrEmpty())
                    {
                        var rect3 = new Rect(0f, y, rect.width, 20f);
                        var rect3R = rect3.RightHalf();
                        var rect3L = rect3.LeftHalf();

                        y += rect2.height;
                    }
                }

            }
            GUI.EndGroup();
        }


        public static float HeaderSize = 32f;

        public static float SpacingOffset = 15f;
    }
}