using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus
{

    public class AlternateApparelGraphic
    {
        public float Weight
        {
            get
            {
                return this.weight;
            }
        }

        public Graphic GetGraphic(Graphic other, bool wornGraphic = false, CompColorable colorable = null)
        {
            if (this.graphicData == null)
            {
                this.graphicData = new GraphicData();
            }
            if (other.data != null)
            {
                CopyFrom(other.data);
            }

            if (!this.texPath.NullOrEmpty() && !wornGraphic)
                this.graphicData.texPath = this.texPath;
            else if (!this.wornGraphicPath.NullOrEmpty() && wornGraphic)
                this.graphicData.texPath = this.wornGraphicPath;
            this.graphicData.color = (this.color ?? other.color);
            this.graphicData.colorTwo = (this.colorTwo ?? other.colorTwo);
            if (colorable != null)
            {
                if (colorable is CompColorableTwo twocolor)
                {
                    if (twocolor.Active)
                    {
                        this.graphicData.color = twocolor.Color;
                    }
                    if (twocolor.ActiveTwo)
                    {
                        this.graphicData.colorTwo = twocolor.ColorTwo;
                    }
                }
                if (colorable is CompColorableTwoFaction twoFaction)
                {
                    if (twoFaction.ActiveFaction && this.allowFactionColours)
                    {
                        if (twoFaction.FactionActive)
                        {
                            this.graphicData.color = twoFaction.Color;
                        }
                        if (twoFaction.FactionActiveTwo)
                        {
                            this.graphicData.colorTwo = twoFaction.ColorTwo;
                        }
                    }
                }
            }

            return this.graphicData.Graphic;
        }
        
        public Graphic GetGraphic(GraphicData other, bool wornGraphic = false, CompColorable colorable = null)
        {
            if (this.graphicData == null)
            {
                this.graphicData = new GraphicData();
            }
            CopyFrom(other);
            if (!this.texPath.NullOrEmpty())
            {
                this.graphicData.texPath = this.texPath;
            }
            this.graphicData.color = (this.color ?? other.color);
            this.graphicData.colorTwo = (this.colorTwo ?? other.colorTwo);
            return this.graphicData.Graphic;
        }
        
        public void CopyFrom(GraphicData other)
        {
            Log.Message("CopyFrom");
            this.graphicData.texPath = other.texPath;
            this.graphicData.graphicClass = other.graphicClass;
            this.graphicData.shaderType = other.shaderType;
            this.graphicData.color = other.color;
            this.graphicData.colorTwo = other.colorTwo;
            this.graphicData.drawSize = other.drawSize;
            this.graphicData.drawOffset = other.drawOffset;
            this.graphicData.drawOffsetNorth = other.drawOffsetNorth;
            this.graphicData.drawOffsetEast = other.drawOffsetEast;
            this.graphicData.drawOffsetSouth = other.drawOffsetSouth;
            this.graphicData.drawOffsetWest = other.drawOffsetSouth;
            this.graphicData.onGroundRandomRotateAngle = other.onGroundRandomRotateAngle;
            this.graphicData.drawRotated = other.drawRotated;
            this.graphicData.allowFlip = other.allowFlip;
            this.graphicData.flipExtraRotation = other.flipExtraRotation;
            this.graphicData.shadowData = other.shadowData;
            this.graphicData.damageData = other.damageData;
            this.graphicData.linkType = other.linkType;
            this.graphicData.linkFlags = other.linkFlags;
        }
        // Token: 0x04000503 RID: 1283
        public float weight = 0.5f;

        // Token: 0x04000504 RID: 1284
        public string texPath;
        public string wornGraphicPath;
        public string maskKey;
        public bool allowFactionColours = true;

        // Token: 0x04000505 RID: 1285
        public Color? color;

        public Color? colorTwo;

        public GraphicData graphicData;
        public string label;
        public string saveKey;
    }

    // AdeptusMechanicus.ApparelGraphicExtension
    public class ApparelGraphicExtension : DefModExtension
    {
        public string defaultLabel = "Default";
        public string keyLabel = "Alternate Graphic";
        public List<AlternateApparelGraphic> alternateGraphics = new List<AlternateApparelGraphic>();
        public bool gizmoOnWorn = true;
        public override IEnumerable<string> ConfigErrors()
        {
            foreach (string text in base.ConfigErrors())
            {
                yield return text;
            }


            yield break;
        }
    }

    // AdeptusMechanicus.ApparelComposite
    public class ApparelComposite : Apparel
    {
        private ApparelGraphicExtension altGraphicsExt;
        public ApparelGraphicExtension AltGraphicsExt => altGraphicsExt ??= def.GetModExtensionFast<ApparelGraphicExtension>();
        private List<AlternateApparelGraphic> altGraphics;
        public List<AlternateApparelGraphic> AltGraphics
        {
            get
            {
                if (altGraphics == null)
                {
                    altGraphics = new List<AlternateApparelGraphic>();
                    if (AltGraphicsExt != null)
                    {
                        altGraphics = AltGraphicsExt.alternateGraphics;
                    }
                }
                return altGraphics;
            }
        }

        private AlternateApparelGraphic activeAltGraphic;
        public AlternateApparelGraphic ActiveAltGraphic
        {
            get
            {
                if (activeAltGraphic == null && activeAltInt > -1)
                {
                    if (!activeAltKey.NullOrEmpty()) activeAltGraphic = AltGraphics?.Find(x => x.saveKey == activeAltKey);
                    else activeAltGraphic = AltGraphics?[Math.Min(activeAltInt, AltGraphics.Count)];
                }
                return activeAltGraphic;
            }
            set
            {
                if (activeAltGraphic != value)
                {
                    activeAltGraphic = value;
                    if (value != null)
                    {
                        activeAltKey = value.saveKey;
                        activeAltInt = AltGraphics.IndexOf(value);
                    }
                    else
                    {
                        activeAltKey = null;
                        activeAltInt = -1;
                    }
                    failedgraphic = false;
                    _graphic = null;
                }
            }
        }


        private FactionDef factionColours;
        public FactionDef FactionColours
        {
            get
            {
                if (factionColours == null)
                {
                    if (Wearer?.Faction != null)
                    {
                        factionColours = Wearer?.Faction?.def;
                    }
                }
                return factionColours;
            }
            set
            {
                factionColours = value;
            }
        }
        public FactionDefExtension ColoursExt => FactionColours !=null ? (FactionColours?.GetModExtensionFast<FactionDefExtension>() ?? null) : null;
        bool triedColorable = false;
        public CompColorable colorable;
        public CompColorable Colorable
        {
            get
            {
                if (colorable == null && !triedColorable)
                {
                    colorable = this.TryGetCompFast<CompColorable>();
                    triedColorable = true;
                }
                return colorable;
            }
        }
        public CompColorableTwo ColorableTwo => Colorable as CompColorableTwo;
        public CompColorableTwoFaction ColorableFaction => Colorable as CompColorableTwoFaction;

        private List<CompPauldronDrawer> pauldrons;
        public List<CompPauldronDrawer> Pauldrons
        {
            get
            {
                if (pauldrons.NullOrEmpty())
                {
                    pauldrons = new List<CompPauldronDrawer>();
                    for (int i = 0; i < this.AllComps.Count; i++)
                    {
                        CompPauldronDrawer drawer = this.AllComps[i] as CompPauldronDrawer;
                        if (drawer != null)
                        {
                            pauldrons.Add(drawer);
                        }
                    }
                }
                return pauldrons;
            }
        }
        
        private List<CompApparelExtraPartDrawer> extras;
        public List<CompApparelExtraPartDrawer> Extras
        {
            get
            {
                if (extras.NullOrEmpty())
                {
                    extras = new List<CompApparelExtraPartDrawer>();
                    for (int i = 0; i < this.AllComps.Count; i++)
                    {
                        CompApparelExtraPartDrawer drawer = this.AllComps[i] as CompApparelExtraPartDrawer;
                        if (drawer != null)
                        {
                            extras.Add(drawer);
                        }
                    }
                }
                return extras;
            }
        }


        // add shield comps here, with getter as above
        public override bool CheckPreAbsorbDamage(DamageInfo dinfo)
        {
            return base.CheckPreAbsorbDamage(dinfo);
        }

        public override IEnumerable<Gizmo> GetWornGizmos()
        {
            foreach (var item in base.GetWornGizmos())
            {
                yield return item;
            }
            int num = 700000101;
            if (Find.Selector.SingleSelectedThing == base.Wearer)
            {
                if (AltGraphicsExt != null && AltGraphicsExt.gizmoOnWorn)
                {
                    Command_Action command_Action = new Command_Action()
                    {
                        icon = this.def.uiIcon,
                        defaultLabel = AltGraphicsExt.keyLabel + ": " + (ActiveAltGraphic?.label ?? AltGraphicsExt.defaultLabel),
                        defaultDesc = "Switch." + (ActiveAltGraphic?.texPath ?? def.graphicData.texPath),
                        hotKey = KeyBindingDefOf.Misc10,
                        activateSound = SoundDefOf.Click,
                        action = delegate ()
                        {
                            Find.WindowStack.Add(MakeAlternateGraphicMenu());
                        },
                        groupKey = num + 1
                    };
                    yield return command_Action;
                }

            }
            yield break;
        }




        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var item in base.GetGizmos())
            {
                yield return item;
            }
            int num = 700000101;
            if (AltGraphicsExt != null)
            {
                Command_Action command_Action = new Command_Action()
                {
                    icon = this.def.uiIcon,
                    defaultLabel = AltGraphicsExt.keyLabel + ": " + (ActiveAltGraphic?.label ?? AltGraphicsExt.defaultLabel),
                    defaultDesc = "Switch." + (ActiveAltGraphic?.texPath ?? def.graphicData.texPath),
                    hotKey = KeyBindingDefOf.Misc10,
                    activateSound = SoundDefOf.Click,
                    action = delegate ()
                    {
                        Find.WindowStack.Add(MakeAlternateGraphicMenu());
                    },
                    groupKey = num + 1
                };
                yield return command_Action;
            }
            yield break;
        }

        public FloatMenu MakeAlternateGraphicMenu()
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            if (this.AltGraphics.NullOrEmpty())
            {
                return null;
            }
            if (this.ActiveAltGraphic != null)
            {
                list.Add(new FloatMenuOption(AltGraphicsExt.defaultLabel, delegate ()
                {
                    ActiveAltGraphic = null;
                }, MenuOptionPriority.Default, null, null, 0f, null, null));
            }
            using (List<AlternateApparelGraphic>.Enumerator enumerator = this.AltGraphics.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    AlternateApparelGraphic item = enumerator.Current;
                    if (this.ActiveAltGraphic == null || this.ActiveAltGraphic != item)
                    {
                        list.Add(new FloatMenuOption(item.label, delegate ()
                        {
                            ActiveAltGraphic = item;
                        }, MenuOptionPriority.Default, null, null, 0f, null, null));
                    }
                }
            }
            return new FloatMenu(list);
        }
        public string WornGraphicPath
        {
            get
            {
                string r = this.def.apparel.wornGraphicPath;
                if (ActiveAltGraphic != null)
                {
                    if (!ActiveAltGraphic.wornGraphicPath.NullOrEmpty())
                    {
                        r = ActiveAltGraphic.wornGraphicPath;
                    }
                }
                Log.Message("using " + r);
               return r;
            }
        }
        private Graphic _graphic;
        private bool failedgraphic = false;
        public override Graphic Graphic
        {
            get
            {
                if (!failedgraphic)
                {
                    try
                    {
                        if (ActiveAltGraphic != null)
                        {
                            if (_graphic == null)
                            {
                                _graphic = ActiveAltGraphic.GetGraphic(this.def.graphicData);
                            }
                            if (_graphic != null)
                            {
                                return _graphic;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        Log.Message("shit went tits up");
                        failedgraphic = true;
                    }
                }
                return base.Graphic;
            }
        }

        public override Color DrawColor { get => ColoursExt?.factionColor ?? base.DrawColor; set => base.DrawColor = value; }
        public override Color DrawColorTwo => ColoursExt?.factionColorTwo ?? base.DrawColorTwo;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.activeAltKey, "activeAltGraphicKey", null);
            Scribe_Values.Look(ref this.activeAltInt, "activeAltGraphicInt",-1);
        }
        public string activeAltKey = null;
        public int activeAltInt = -1;
    }
}