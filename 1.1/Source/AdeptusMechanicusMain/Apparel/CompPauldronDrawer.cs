using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public class CompProperties_PauldronDrawer : CompProperties
    {
        public List<ShoulderPadEntryProps> PauldronEntries;
        public float PauldronEntryChance = 1f;
        public int order = 0;
        public Vector3 NorthOffset = new Vector3();
        public Vector3 SouthOffset = new Vector3();
        public Vector3 EastOffset = new Vector3();
        public Vector3 WestOffset = new Vector3();
        public bool drawAll = false;
        public string labelKey = string.Empty;
        public CompProperties_PauldronDrawer()
        {
            this.compClass = typeof(CompPauldronDrawer);
        }
    }

    public class CompPauldronDrawer : ThingComp, ILoadReferenceable
    {
        public CompProperties_PauldronDrawer Props
        {
            get
            {
                return this.props as CompProperties_PauldronDrawer;
            }
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
        //    Initialize();
        }
        public void Initialize()
        {

            if (!pauldronInitialized)
            {
            //    Log.Message(apparel.LabelShortCap + " pauldrons initializing");
                if (activeEntries.EnumerableNullOrEmpty())
                {
                    activeEntries = new List<ShoulderPadEntry>();
                }
                if (Props.drawAll)
                {
                    for (int i = 0; i < Props.PauldronEntries.Count; i++)
                    {
                        ShoulderPadEntry entry = new ShoulderPadEntry(Props.PauldronEntries[i], this);

                        activeEntries.Add(entry);
                    }
                }
                else
                {
#pragma warning disable CS0219 // Variable is assigned but its value is never used
                    bool backpack = false;
                    bool bothPauldrons = false;
                    bool leftPauldron = false;
                    bool rightPauldron = false;
#pragma warning restore CS0219 // Variable is assigned but its value is never used

                    ShoulderPadEntry entry = new ShoulderPadEntry(Props.PauldronEntries.RandomElementByWeight(x=> x.commonality), this);

                    if (!entry.Options.NullOrEmpty())
                    {
                        entry.Used= entry.DefaultOption;
                    }
                    activeEntries.Add(entry);
                }
                /*
                if (!activeEntries.EnumerableNullOrEmpty())
                {
                //    Log.Message("activeEntries count " + activeEntries.Count);
                }
                */
            }
            else
            {
            //    Log.Message(apparel.LabelShortCap + " pauldrons initialized");
                foreach (ShoulderPadEntry item in activeEntries)
                {

                //    item.Drawer = this;
                    item.apparel = this.apparel;
                    /*
                    if (Props.PauldronEntries.Any(x => x.padTexPath == item.padTexPath && x.shoulderPadType == item.shoulderPadType))
                    {
                        item.EastOffset = Props.PauldronEntries.Find(x => x.padTexPath == item.padTexPath && x.shoulderPadType == item.shoulderPadType).EastOffset;
                        item.WestOffset = Props.PauldronEntries.Find(x => x.padTexPath == item.padTexPath && x.shoulderPadType == item.shoulderPadType).WestOffset;
                        item.NorthOffset = Props.PauldronEntries.Find(x => x.padTexPath == item.padTexPath && x.shoulderPadType == item.shoulderPadType).NorthOffset;
                        item.SouthOffset = Props.PauldronEntries.Find(x => x.padTexPath == item.padTexPath && x.shoulderPadType == item.shoulderPadType).SouthOffset;
                    }
                    */
                    
                }
            }
        }

        public Apparel apparel
        {
            get
            {
                return this.parent as Apparel;
            }
        }

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

       
        private  CompColorable colours;
        private bool colortest = false;
        public CompColorable Colours
        {
            get
            {
                if (colours == null && !colortest)
                {
                    colortest = true;
                    colours = this.apparel.TryGetComp<CompColorable>();
                }
                return colours;
            }
        }

        public Color mainColorFor(ShoulderPadEntry entry)
        {
            if (entry != null)
            {
                if (!entry.Options.NullOrEmpty())
                {
                    if (entry.Used.Color!=null)
                    {
                    //   Log.Message("mainColorFor "+ entry.shoulderPadType + " activeOption");
                        return entry.Used.Color.Value;
                    }
                }
                if (entry.UseFactionColors)
                {
                    CompFactionColorableTwo colours = Colours as CompFactionColorableTwo;
                    if (colours != null)
                    {
                        if (colours.Active)
                        {
                            return colours.Color;
                        }
                    //    Log.Message("mainColorFor " + entry.shoulderPadType + " UseFactionColors");
                    }
                    /*
                    else
                    {
                        FactionDef def = pawn?.Faction?.def;
                        if (entry.FactionColours(out Color color, out Color colorTwo, def))
                        {
                            if (color != Color.white)
                            {
                                return color;
                            }
                        }
                    }
                    */
                }
                if (entry.overridePrimaryColor.HasValue)
                {
                //    Log.Message("mainColorFor " + entry.shoulderPadType + " overridePrimaryColor");
                    return entry.overridePrimaryColor.Value;
                }
                if (entry.UseSecondaryColorAsPrimary)
                {
                    if (entry.overrideSecondaryColor.HasValue)
                    {
                    //    Log.Message("mainColorFor " + entry.shoulderPadType + " overrideSecondaryColor");
                        return entry.overrideSecondaryColor.Value;
                    }
                //    Log.Message("mainColorFor " + entry.shoulderPadType + " DrawColorTwo");
                    return this.parent.DrawColorTwo;
                }
            }
        //    Log.Message("mainColorFor " + entry.shoulderPadType + " DrawColor");
            return this.parent.DrawColor;
        }
        
        public Color secondaryColorFor(ShoulderPadEntry entry)
        {
            if (entry != null)
            {
                if (!entry.Options.NullOrEmpty())
                {
                    if (entry.Used.ColorTwo != null)
                    {
                    //    Log.Message("secondaryColorFor " + entry.shoulderPadType + " activeOption");
                        return entry.Used.ColorTwo.Value;
                    }
                }
                if (entry.UseFactionColors)
                {
                    CompFactionColorableTwo colours = Colours as CompFactionColorableTwo;
                    if (colours != null)
                    {
                        //    Log.Message("secondaryColorFor " + entry.shoulderPadType + " UseFactionColors");
                        if (colours.ActiveTwo)
                        {
                            return colours.ColorTwo;
                        }
                    }
                    /*
                    else
                    {
                        FactionDef def = pawn?.Faction?.def;
                        if (entry.FactionColours(out Color color, out Color colorTwo, def))
                        {
                            if (colorTwo != Color.white)
                            {
                                return colorTwo;
                            }
                        }
                    }
                    */
                }
                if (entry.overrideSecondaryColor.HasValue)
                {
                //    Log.Message("secondaryColorFor " + entry.shoulderPadType + " overrideSecondaryColor");
                    return entry.overrideSecondaryColor.Value;
                }
                if (entry.UsePrimaryColorAsSecondary)
                {
                    if (entry.overridePrimaryColor.HasValue)
                    {
                    //    Log.Message("secondaryColorFor " + entry.shoulderPadType + " overridePrimaryColor");
                        return entry.overridePrimaryColor.Value;
                    }
                //    Log.Message("secondaryColorFor " + entry.shoulderPadType + " DrawColor");
                    return this.parent.DrawColor;
                }
            }
        //    Log.Message("secondaryColorFor " + entry.shoulderPadType + " DrawColorTwo");
            return apparel.DrawColorTwo;
        }
        /*
        public override string TransformLabel(string label)
        {
            if (this.activeEntries.Any(x=> x.VariantTextures!=null))
            {
                List<ShoulderPadEntry> variables = this.activeEntries.FindAll(x => x.VariantTextures != null);
                if (variables.Any(x => x.VariantTextures.activeOption.factionDef != null))
                {
                    return (label + (this.activeEntries.First(x => x.VariantTextures != null && (x.VariantTextures.activeOption.factionDef != null)).VariantTextures.activeOption.Label));
                }
            }
            return base.TransformLabel(label);
        }
        */
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<int>(ref this.entryInd, "entryInd", -1, false);
            Scribe_Values.Look<ShoulderPadType>(ref this.padType, "padType", ShoulderPadType.Both, false);
            Scribe_Values.Look<bool>(ref this.useSecondaryColor, "useSecondaryColor", false, false);
            Scribe_Collections.Look(ref this.activeEntries, "activeEntries", LookMode.Deep);
            Scribe_References.Look(ref this.wearer, "lastWearer");
        }

        public Vector3 GetOffsetFor(Rot4 rotation, bool northtop)
        {
            Vector3 offset = new Vector3();
            /*
            offset.y += _OffsetFactor * Entry.order;
            offset.y += offset.y + (_SubOffsetFactor * Entry.sublayer);
            */
        //    bool flag = Find.Selector.SingleSelectedThing == pawn && Prefs.DevMode && DebugSettings.godMode;
            if (!onHead)
            {

                if (rotation == Rot4.North)
                {
                //    offset.y += _BodyOffset;
                    if (northtop)
                    {
                        offset.y += _HairOffset;
                    //    offset += NorthOffset(Entry);
                        offset += Props.NorthOffset;
                    }
                    else
                    {
                    //    offset += NorthOffset(Entry);
                        offset += Props.NorthOffset;
                    }
                }
                else if (rotation == Rot4.West)
                {
                //    offset.y += _BodyOffset;
                //    offset += WestOffset(Entry);
                    offset += Props.WestOffset;
                }
                else if (rotation == Rot4.East)
                {
                //    offset.y += _BodyOffset;
                //    offset += EastOffset(Entry);
                    offset += Props.EastOffset;
                }
                else if (rotation == Rot4.South)
                {
                //    offset.y += _BodyOffset;
                //    offset += SouthOffset(Entry);
                    offset += Props.SouthOffset;
                }
                else
                {
                 //   offset.y += _BodyOffset;
                }
            }
            else
            {
                if (rotation == Rot4.North)
                {
                    offset.y += _BodyOffset;
                }
                else
                    offset.y += _HeadOffset;
            }

            return offset;
        }

        public Vector3 NorthOffset(ShoulderPadEntry Entry)
        {
            return this.Props.NorthOffset + Entry.Props.NorthOffset;
        }

        public Vector3 SouthOffset(ShoulderPadEntry Entry)
        {
            return this.Props.SouthOffset + Entry.Props.SouthOffset;
        }

        public Vector3 EastOffset(ShoulderPadEntry Entry)
        {
            return this.Props.EastOffset + Entry.Props.EastOffset;
        }

        public Vector3 WestOffset(ShoulderPadEntry Entry)
        {
            return this.Props.WestOffset + Entry.Props.WestOffset;
        }

        Mesh GetPauldronMesh(bool portrait, Pawn pawn, Rot4 facing, bool body)
        {
            return AlienRace.HarmonyPatches.GetPawnMesh(portrait, pawn, facing, body);
        }
        public bool ShouldDrawPauldron(bool portrait, Rot4 bodyFacing, Vector2 size, ShoulderPadEntry Entry, out Graphic pauldronMaterial, out Mesh pauldronMesh)
        {
            pauldronMaterial = null;
            pauldronMesh = !onHead ? MeshPool.humanlikeBodySet.MeshAt(bodyFacing) : MeshPool.humanlikeHeadSet.MeshAt(bodyFacing);
            if (AdeptusIntergrationUtility.enabled_AlienRaces)
            {
                pauldronMesh = GetPauldronMesh(portrait, pawn, bodyFacing, !onHead);
            }
            this.size = size;
            if (pawn.RaceProps.Humanlike)
            {
                if (Entry != null)
                {
                    if (this.CheckPauldronRotation(bodyFacing, Entry.shoulderPadType))
                    {
                        if (Entry.Graphic==null || (Entry.Graphic != null && pawn != apparel.Wearer))
                        {
                        //    Log.Message("pawn = Wearer" + (pawn != apparel.Wearer));
                        //    Log.Message(string.Format("ShouldDrawPauldron UpdatePadGraphic"));
                            Entry.UpdateGraphic();
                        }
                        pauldronMaterial = Entry.Graphic;//.GetColoredVersion(shader, this.mainColorFor(Entry), this.secondaryColorFor(Entry)).MatAt(bodyFacing, this.parent);
                        return true;
                    }
                    else
                    {
                    //    Log.Message(string.Format("CheckPauldronRotation false"));
                    }
                }
            }
            else
            {
            //    Log.Message(string.Format("pawn.needs = null && pawn.story = null"));
            }
            return false;

        }
        public Vector2 size;
        public bool CheckPauldronRotation(Rot4 bodyFacing, ShoulderPadType shoulderPadType)
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

        //Utility, return if the apparel is worn on the head/body.        
        public bool onHead
        {
            get
            {
                if (!_OnHeadCache.ContainsKey(parent.def.defName))
                {
                    List<BodyPartRecord> parts = pawn.RaceProps.body.AllParts.Where(parent.def.apparel.CoversBodyPart).ToList();
                    bool gotHit = false;
                    foreach (BodyPartRecord part in parts)
                    {
                        BodyPartRecord p = part;
                        while (p != null)
                        {
                            if (p.groups.Contains(BodyPartGroupDefOf.Torso))
                            {
                                _OnHeadCache.Add(parent.def.defName, false);
                                gotHit = true;
                                break;
                            }
                            if (p.groups.Contains(BodyPartGroupDefOf.FullHead) || p.groups.Contains(BodyPartGroupDefOf.UpperHead))
                            {
                                _OnHeadCache.Add(parent.def.defName, true);
                                gotHit = true;
                                break;
                            }
                            p = p.parent;
                        }
                        if (gotHit)
                            break;
                    }
                    if (!_OnHeadCache.ContainsKey(parent.def.defName))
                    {
                        Log.ErrorOnce(string.Concat("AdeptusMechanicus :: ", this.GetType(), " was unable to determine if body or head on item '", parent.Label,
                                                    "', might the Wearer be non-human?  Assuming apparel is on body."), parent.def.debugRandomId);
                        _OnHeadCache.Add(parent.def.defName, false);
                    }
                }
                bool ret;
                _OnHeadCache.TryGetValue(parent.def.defName, out ret);  // is there a better way? Dictionary.Item isn't there.  Didn't bother with try/catch as by now it should have the key.
                return ret;
            }
        }
        public string GetDescription(ShoulderPadType type)
        {
            if (type != ShoulderPadType.Backpack)
            {
                if (type == ShoulderPadType.Both)
                {
                    return type.ToString() + " Pauldrons";
                }
                return type.ToString() + " Pauldron";
            }
            else
            {
                return type.ToString();
            }
        }
        public override void ReceiveCompSignal(string signal)
        {

            if (!signal.NullOrEmpty())
            {
                if (signal == UpdateString && pawn.Map != null)
                {
                    if (!this.activeEntries.NullOrEmpty())
                    {
                        for (int i = 0; i < this.activeEntries.Count; i++)
                        {
                            //	Log.Message("Entry drawer " + (i2 + 1));
                            this.activeEntries[i].UpdateGraphic();
                        }
                    }
                }
            }
            base.ReceiveCompSignal(signal);
        }
        public override string TransformLabel(string label)
        {

            return base.TransformLabel(label);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            Initialize();
            foreach (var item in activeEntries)
            {
            //    item.Drawer = this;
            }
        }

        public string GetUniqueLoadID()
        {
            return "CompPauldronDrawer_" + parent.thingIDNumber;
        }

        public const float MinClippingDistance = 0.002f;   // Minimum space between layers to avoid z-fighting
        const float _HeadOffset = 0.02734375f + MinClippingDistance;
        const float _HairOffset = 035f + MinClippingDistance;       // Number must be same as PawnRenderer.YOffset_Head
        const float _BodyOffset = 0.0234375f + MinClippingDistance;   // Number must be same as PawnRenderer.YOffset_Shell
        const float _OffsetFactor = 0.001f;
        const float _SubOffsetFactor = 0.0001f;
        static readonly Dictionary<string, bool> _OnHeadCache = new Dictionary<string, bool>();
        public ShoulderPadType padType;
        public bool ExtraUseBodyOffset;
        public bool useSecondaryColor;
        public bool useFactionTextures = false;
        public bool UseFactionColors = false;
        public bool pauldronInitialized => !activeEntries.EnumerableNullOrEmpty();
        //    public FactionDef FactionDef;
        public int entryInd = -1;

        public List<ShoulderPadEntry> activeEntries;
        public static string UpdateString = "UPDATEPADGRAPHICS";
    }

}
