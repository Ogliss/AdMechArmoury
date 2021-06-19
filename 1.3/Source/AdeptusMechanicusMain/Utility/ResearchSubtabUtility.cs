using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    static class ResearchSubTabUtility
    {
        public static bool IsSubTabOf(this ResearchTabDef def, ResearchTabDef tabDef)
        {
            if (def is ResearchSubTabDef subTabDef)
            {
                return subTabDef.parentTab == tabDef;
            }
            return false;
        }

        public static IEnumerable<ResearchTabDef> filterSubTabs(IEnumerable<ResearchTabDef> defs)
        {
            foreach (ResearchTabDef def in defs)
            {
                if (def is ResearchSubTabDef)
                {
                    continue;
                }
                yield return def;
            }
        }


        public static bool OnTabOrActiveSubTab(ResearchTabDef defTab, ResearchTabDef CurTab, ResearchProjectDef def)
        {
            if ( CurTab == null || defTab == null)
            {
                return false;
            }
            if (CurTab == AdeptusResearchTabDefOf.OGAMA_RTab)
            {
                if (defTab != CurSubTab)
                {
                    if (def.HasTag(ResearchSubTabUtility.CurSubTabTag))
                    {
                        return true;
                    }
                    if (def.HasTag(AdeptusResearchTagDefOf.OG_Common_Tech))
                    {
                        return true;
                    }
                    return false;
                }
                else return true;

                /*
                    if (defTab is ResearchSubTabDef subTabDef)
                    {
                        Log.Message(defTab + " is subTab of " + subTabDef.parentTab);
                    }
                    if (defTab != CurTab)
                    {

                        //    Log.Message("OGAMA_RTab defTab != CurTab CurSubTabTag: "+ ResearchSubTabUtility.CurSubTabTag);
                        return def != null && (def.HasTag(ResearchSubTabUtility.CurSubTabTag) || def.HasTag(AdeptusResearchTagDefOf.OG_Common_Tech));
                    }
                    else
                    {
                    //    Log.Message("OGAMA_RTab defTab == CurTab");
                        return def != null && (def.HasTag(ResearchSubTabUtility.CurSubTabTag) || def.HasTag(AdeptusResearchTagDefOf.OG_Common_Tech));
                    }
                */
            }
            return defTab == CurTab;
        }
        public static bool OnSubTab(ResearchProjectDef def, ResearchTabDef CurTab)
        {
            if ( CurTab == null || CurTab != AdeptusResearchTabDefOf.OGAMA_RTab)
            {
                return true;
            }
            if (def == null)
            {
                return true;
            }
            if (def.HasTag(ResearchSubTabUtility.CurSubTabTag))
            {;
                return true;
            }
            if (def.HasTag(AdeptusResearchTagDefOf.OG_Common_Tech))
            {
                return true;
            }

            return false;
        }

        public static Rect SubTabMenu(Rect rightOutRect, ResearchTabDef CurTab)
        {
            Rect rect = rightOutRect;
            List<SubTabRecord> subtabs = ResearchSubTabUtility.SubTabs.FindAll(x => x.SubTabOf(CurTab));
            if (!subtabs.NullOrEmpty())
            {
                rect.yMin += 32f;
                Widgets.DrawMenuSection(rect);
                TabDrawer.DrawTabs(rect, subtabs.Cast<TabRecord>().ToList(), 200f);
            }
            /*
            if (CurTab == AdeptusResearchTabDefOf.OGAMA_RTab)
            {
                rect.yMin += 32f;
                Widgets.DrawMenuSection(rect);
                TabDrawer.DrawTabs(rect, ResearchSubTabUtility.SubTabs.Cast<TabRecord>().ToList(), 200f);
            }
            */
            return rect;
        }

        private static float PosX(ResearchProjectDef d)
        {
            return CoordToPixelsX(d.ResearchViewX);
        }

        // Token: 0x06006144 RID: 24900 RVA: 0x0021D3AE File Offset: 0x0021B5AE
        private static float PosY(ResearchProjectDef d)
        {
            return CoordToPixelsY(d.ResearchViewY);
        }
        private static float CoordToPixelsX(float x)
        {
            return x * 190f;
        }

        // Token: 0x06006140 RID: 24896 RVA: 0x0021D385 File Offset: 0x0021B585
        private static float CoordToPixelsY(float y)
        {
            return y * 100f;
        }

        public static bool SameSubTab(ResearchProjectDef defA, ResearchProjectDef defB)
        {
            if (defA.tab == AdeptusResearchTabDefOf.OGAMA_RTab && defB.tab == AdeptusResearchTabDefOf.OGAMA_RTab)
            {
                foreach (var item in ResearchSubTabUtility.SubTabs)
                {
                    //    Log.Message("checking for " + item.label.CapitalizeFirst());
                    if (defA.HasTag(item.subTagDef) && !defB.HasTag(item.subTagDef))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static ResearchSubTabDef CurSubTab
        {
            get
            {
                if (curTabInt == null)
                {
                    curTabInt = AdeptusResearchTabDefOf.OGAMA_RSubTab_Imperial;
                }
                return curTabInt;
            }
            set
            {
                if (value == curTabInt)
                {
                    return;
                }
                curTabInt = value;
                MainTabWindow_Research Research = Find.WindowStack.WindowOfType<MainTabWindow_Research>();
                if (Research != null)
                {
                    Type typ = typeof(MainTabWindow_Research);
                    MethodInfo minfo = typ.GetMethod("ViewSize", BindingFlags.NonPublic | BindingFlags.Instance);
                    FieldInfo rightViewWidth = typ.GetField("rightViewWidth", BindingFlags.NonPublic | BindingFlags.Instance);
                    FieldInfo rightViewHeight = typ.GetField("rightViewHeight", BindingFlags.NonPublic | BindingFlags.Instance);
                    FieldInfo rightScrollPosition = typ.GetField("rightScrollPosition", BindingFlags.NonPublic | BindingFlags.Instance);
                    Vector2 vector = (Vector2)minfo.Invoke(Research, new object[] { CurSubTab });
                    if (rightViewWidth != null)
                    {
                        rightViewWidth.SetValue(Research, vector.x);
                    }
                    if (rightViewHeight != null)
                    {
                        rightViewHeight.SetValue(Research, vector.y);
                    }
                    if (rightScrollPosition != null)
                    {
                        rightScrollPosition.SetValue(Research, Vector2.zero);
                    }

                }
                /*
                Vector2 vector = this.ViewSize(this.CurTab);
                this.rightViewWidth = vector.x;
                this.rightViewHeight = vector.y;
                this.rightScrollPosition = Vector2.zero;
                */
            }
        }
        private static ResearchSubTabDef curTabInt;

        public static ResearchProjectTagDef CurSubTabTag
        {
            get
            {
                if (curTabTagInt == null)
                {
                    curTabTagInt = AdeptusResearchTagDefOf.OG_Imperial_Tech;
                }
                return curTabTagInt;
            }
            set
            {
                if (value == curTabTagInt)
                {
                    return;
                }
                curTabTagInt = value;
                /*
                Vector2 vector = this.ViewSize(this.CurTab);
                this.rightViewWidth = vector.x;
                this.rightViewHeight = vector.y;
                this.rightScrollPosition = Vector2.zero;
                */
            }
        }
        private static ResearchProjectTagDef curTabTagInt;

        public static List<SubTabRecord> SubTabs
        {
            get
            {
                if (subTabs == null)
                {
                    subTabs = new List<SubTabRecord>();
                    List<ResearchSubTabDef> subTabDefs = DefDatabase<ResearchSubTabDef>.AllDefsListForReading;
                    if (subTabDefs.NullOrEmpty())
                    {
                        subTabs = new List<SubTabRecord>()
                        {
                            new SubTabRecord(AdeptusResearchTabDefOf.OGAMA_RSubTab_Imperial,delegate()
                            {
                                CurSubTabTag = AdeptusResearchTagDefOf.OG_Imperial_Tech;
                                CurSubTab = AdeptusResearchTabDefOf.OGAMA_RSubTab_Imperial;
                            }, () => CurSubTabTag == AdeptusResearchTagDefOf.OG_Imperial_Tech&& CurSubTab == AdeptusResearchTabDefOf.OGAMA_RSubTab_Imperial),

                            new SubTabRecord(AdeptusResearchTabDefOf.OGAMA_RSubTab_Aeldari,delegate()
                            {
                                CurSubTabTag = AdeptusResearchTagDefOf.OG_Aeldari_Tech;
                                CurSubTab = AdeptusResearchTabDefOf.OGAMA_RSubTab_Aeldari;
                            },  () => CurSubTabTag == AdeptusResearchTagDefOf.OG_Aeldari_Tech && CurSubTab == AdeptusResearchTabDefOf.OGAMA_RSubTab_Aeldari),

                            new SubTabRecord(AdeptusResearchTabDefOf.OGAMA_RSubTab_Tau,delegate()
                            {
                                CurSubTabTag = AdeptusResearchTagDefOf.OG_Tau_Tech;
                                CurSubTab = AdeptusResearchTabDefOf.OGAMA_RSubTab_Tau;
                            }, () => CurSubTabTag == AdeptusResearchTagDefOf.OG_Tau_Tech&& CurSubTab == AdeptusResearchTabDefOf.OGAMA_RSubTab_Tau),

                            new SubTabRecord(AdeptusResearchTabDefOf.OGAMA_RSubTab_Greenskin,delegate()
                            {
                                CurSubTabTag = AdeptusResearchTagDefOf.OG_Greenskin_Tech;
                                CurSubTab = AdeptusResearchTabDefOf.OGAMA_RSubTab_Greenskin;
                            }, () => CurSubTabTag == AdeptusResearchTagDefOf.OG_Greenskin_Tech&& CurSubTab == AdeptusResearchTabDefOf.OGAMA_RSubTab_Greenskin)
                        };
                    }
                    else
                    {
                        foreach (var item in subTabDefs)
                        {
                            subTabs.Add(new SubTabRecord(item, delegate ()
                            {
                                CurSubTabTag = item.tagdef;
                                CurSubTab = item;
                            }, () => CurSubTabTag == item.tagdef && CurSubTab == item));
                        }
                    }
                }
                return subTabs;
            }
            set
            {
                if (value == subTabs)
                {
                    return;
                }
                subTabs = value;
            }
        }

        private static List<SubTabRecord> subTabs;

    }
}
