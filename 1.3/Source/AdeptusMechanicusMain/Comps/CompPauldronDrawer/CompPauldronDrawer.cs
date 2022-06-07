using AdeptusMechanicus.ExtensionMethods;
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
        public List<ShoulderPadProperties> PauldronEntries;
        public float PauldronEntryChance = 1f;
        public int order = 0;
        public Vector3 NorthOffset = new Vector3();
        public Vector3 SouthOffset = new Vector3();
        public Vector3 EastOffset = new Vector3();
        public Vector3 WestOffset = new Vector3();
        public bool drawAll = false;
        public string labelKey = string.Empty;
        public int saveKey = 0;
        public CompProperties_PauldronDrawer()
        {
            this.compClass = typeof(CompPauldronDrawer);
        }
        /*
        public override void ResolveReferences(ThingDef parentDef)
        {
            base.ResolveReferences(parentDef);
            if (!PauldronEntries.NullOrEmpty())
            {
            //    Log.Message("PauldronDrawer ResolveReferences for " + parentDef);
                foreach (var item in PauldronEntries)
                {
                    if (!item.padTexPath.NullOrEmpty())
                    {
                        List<Texture2D> list = (from x in ContentFinder<Texture2D>.GetAllInFolder(item.padTexPath)
                                                where
                                                    !x.name.EndsWith("_m") &&
                                                    !x.name.EndsWith("_Glow") &&
                                                    !x.name.EndsWith("_Glow_m") &&
                                                    !x.name.Contains("_northm") &&
                                                    !x.name.Contains("_southm") &&
                                                    !x.name.Contains("_eastm") &&
                                                    !x.name.Contains("_westm")
                                                orderby x.name
                                                select x).ToList<Texture2D>();
                        if (list.NullOrEmpty<Texture2D>())
                        {
                            Log.Error("PauldronDrawer cannot init "+ item.label + ": No textures found at path " + item.padTexPath, false);
                        }
                        if (!item.options.NullOrEmpty())
                        {
                            foreach (var item2 in item.options)
                            {
                                if (!item2.TexPath.NullOrEmpty())
                                {
                                    list = (from x in ContentFinder<Texture2D>.GetAllInFolder(item2.TexPath)
                                                            where
                                                                !x.name.EndsWith("_m") &&
                                                                !x.name.EndsWith("_Glow") &&
                                                                !x.name.EndsWith("_Glow_m") &&
                                                                !x.name.Contains("_northm") &&
                                                                !x.name.Contains("_southm") &&
                                                                !x.name.Contains("_eastm") &&
                                                                !x.name.Contains("_westm") &&
                                                                x.name.Contains("_" + item2.TexPath)
                                                            orderby x.name
                                                            select x).ToList<Texture2D>();
                                    if (list.NullOrEmpty<Texture2D>())
                                    {
                                        Log.Error("PauldronDrawer cannot init " + item2.Label + ": No textures found at path " + item.padTexPath+ "_" + item2.TexPath, false);
                                    }
                                }
                                else
                                {
                                    Log.Error("PauldronDrawer cannot init option "+ item2 + ": No TexPath found", false);
                                }
                            }
                        }
                    }
                    else
                    {
                        Log.Error("PauldronDrawer cannot init "+ item + ": No padTexPath found", false);
                    }
                }
            }
        }
        */
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
                    item.UpdateProps();
                    item.apparel = this.Apparel;
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
                if (Props.drawAll && Props.PauldronEntries.Count != activeEntries.Count)
                {
                    if (Props.PauldronEntries.Count > activeEntries.Count)
                    {
                        foreach (ShoulderPadProperties props in Props.PauldronEntries)
                        {
                            if (!activeEntries.Any(x=> x.Props == props))
                            {
                                ShoulderPadEntry entry = new ShoulderPadEntry(props, this);

                                activeEntries.Add(entry);
                            }
                        }
                    }

                }
            }
        }

        public Apparel Apparel
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
                    wearer = this.Apparel.Wearer;
                }
                return wearer;
            }
            set
            {
                wearer = value;
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
                    colours = this.Apparel.TryGetCompFast<CompColorable>();
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
                    if (entry.Used.Color.HasValue)
                    {
                    //    Log.Message("mainColorFor "+ entry.shoulderPadType + " activeOption: " + entry.Used.Label + " Color: "+ entry.Used.Color.Value);
                        return entry.Used.Color.Value;
                    }
                }
                if (entry.useFactionColors)
                {
                    CompColorableTwoFaction colours = Colours as CompColorableTwoFaction;
                    if (colours != null)
                    {
                        if (colours.Active)
                        {
                         //   Log.Message("mainColorFor " + entry.shoulderPadType + " UseFactionColors");
                            return colours.Color;
                        }
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
                if (entry.useSecondaryColorAsPrimary)
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
            Color secondary = Apparel.DrawColorTwo;
            string log = (pawn != null ? pawn.NameShortColored.ToString() + "'s " : string.Empty) +parent.LabelCap+ " secondaryColorFor " + entry.shoulderPadType;
            if (entry != null)
            {
                if (entry.usePrimaryColorAsSecondary)
                {
                    log += " UsePrimaryColorAsSecondary";
                }
                if (!entry.Options.NullOrEmpty())
                {
                    if (entry.Used.ColorTwo != null)
                    {
                        secondary = entry.usePrimaryColorAsSecondary ? entry.Used.Color.Value : entry.Used.ColorTwo.Value;
                        log += " activeOption: "+ secondary;
                    //    Log.Message(log);
                        return secondary;
                    }
                }
                if (entry.overrideSecondaryColor.HasValue)
                {
                    //    Log.Message("secondaryColorFor " + entry.shoulderPadType + " overrideSecondaryColor");
                    secondary = entry.overrideSecondaryColor.Value;
                    log += " overrideSecondaryColor: " + secondary;
                //    Log.Message(log);
                    return secondary;
                }
                if (entry.usePrimaryColorAsSecondary)
                {
                    log += " UsePrimaryColorAsSecondary";
                    if (entry.overridePrimaryColor.HasValue)
                    {
                        //    Log.Message("secondaryColorFor " + entry.shoulderPadType + " overridePrimaryColor");
                        secondary = entry.overrideSecondaryColor.Value;
                        log += " overridePrimaryColor: " + secondary;
                    //    Log.Message(log);
                        return secondary;
                    }
                    //    Log.Message("secondaryColorFor " + entry.shoulderPadType + " DrawColor");
                    secondary = this.parent.DrawColor;
                    log += " DrawColor: " + secondary;
                //    Log.Message(log);
                    return this.parent.DrawColor;
                }
                if (entry.useFactionColors)
                {
                    if (Colours is CompColorableTwoFaction colours)
                    {
                        if (colours.FactionActiveTwo)
                        {
                            secondary = entry.usePrimaryColorAsSecondary ? colours.Color : colours.ColorTwo;
                            log += " UseFactionColors: " + secondary;
                        //    Log.Message(log);
                            return secondary;
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
            }
            //    Log.Message("secondaryColorFor " + entry.shoulderPadType + " DrawColorTwo");
            log += " DrawColorTwo: " + secondary;
        //    Log.Message(log);
            return secondary;
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
        /*
        Mesh GetPauldronMesh(bool portrait, Pawn pawn, Rot4 facing, bool body)
        {
            return AlienRace.HarmonyPatches.GetPawnMesh(portrait, pawn, facing, body);
        }
        */
        /*
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
                    //    Log.Message("pawn "+pawn + " Wearer "+ apparel.Wearer);
                        if (Entry.Graphic==null || pawn != apparel.Wearer)
                        {
                            wearer = apparel.Wearer;
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
        */
        public bool CheckPauldronRotation(Rot4 bodyFacing, ApparelAddonType shoulderPadType)
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
            if (shoulderPadType == ApparelAddonType.NorthOnly && bodyFacing != Rot4.North)
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
                _OnHeadCache.TryGetValue(parent.def.defName, out bool ret);  // is there a better way? Dictionary.Item isn't there.  Didn't bother with try/catch as by now it should have the key.
                return ret;
            }
        }
        public string GetDescription(ApparelAddonType type)
        {
            if (type != ApparelAddonType.Backpack && type != ApparelAddonType.NorthOnly && type != ApparelAddonType.SouthOnly)
            {
                if (type == ApparelAddonType.Both) return "Pauldrons";
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

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<int>(ref this.entryInd, "entryInd", -1, false);
            Scribe_Values.Look<ApparelAddonType>(ref this.padType, "padType", ApparelAddonType.Both, false);
            Scribe_Values.Look<bool>(ref this.useSecondaryColor, "useSecondaryColor", false, false);
            Scribe_Collections.Look(ref this.activeEntries, "activeEntries", LookMode.Deep);
            Scribe_References.Look(ref this.wearer, "lastWearingPawn"+ Props.saveKey);
        }

        public const float MinClippingDistance = 0.002f;   // Minimum space between layers to avoid z-fighting
        const float _HeadOffset = PawnRenderer.YOffset_Head + MinClippingDistance;
        const float _HairOffset = 0244897958f + MinClippingDistance;       // Number must be same as PawnRenderer.YOffset_Head
        const float _BodyOffset = PawnRenderer.YOffset_Shell+ MinClippingDistance;   // Number must be same as PawnRenderer.YOffset_Shell
        const float _OffsetFactor = 0.001f;
        const float _SubOffsetFactor = 0.0001f;
        static readonly Dictionary<string, bool> _OnHeadCache = new Dictionary<string, bool>();
        public ApparelAddonType padType;
        public bool ExtraUseBodyOffset;
        public bool useSecondaryColor;
        public bool useFactionTextures = false;
        public bool UseFactionColors = false;
        public bool pauldronInitialized => !activeEntries.EnumerableNullOrEmpty();
        //    public FactionDef FactionDef;
        public int entryInd = -1;

        public Vector3 pos = Vector3.zero;
        public Vector2 size;
        public List<ShoulderPadEntry> activeEntries;
        public static string UpdateString = "UPDATEPADGRAPHICS";
    }

}
