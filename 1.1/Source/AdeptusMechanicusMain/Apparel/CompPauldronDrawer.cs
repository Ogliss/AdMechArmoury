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
        public List<ShoulderPadEntry> PauldronEntries;
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

    public class CompPauldronDrawer : ThingComp
    {
        public CompProperties_PauldronDrawer Props
        {
            get
            {
                return this.props as CompProperties_PauldronDrawer;
            }
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

                        if (Props.PauldronEntries[i].VariantTextures != null)
                        {
                            entry.VariantTextures.activeOption = Props.PauldronEntries[i].VariantTextures.defaultOption;
                            entry.VariantTextures.Options = Props.PauldronEntries[i].VariantTextures.Options;
                        }

                        activeEntries.Add(entry);
                    }
                }
                else
                {
                    bool backpack = false;
                    bool bothPauldrons = false;
                    bool leftPauldron = false;
                    bool rightPauldron = false;

                    ShoulderPadEntry entry = new ShoulderPadEntry(Props.PauldronEntries.RandomElementByWeight(x=> x.commonality), this);

                    if (entry.VariantTextures != null)
                    {
                        entry.VariantTextures.activeOption = entry.VariantTextures.defaultOption;
                    }
                    activeEntries.Add(entry);
                }
                /*
                if (!activeEntries.EnumerableNullOrEmpty())
                {
                    Log.Message("activeEntries count " + activeEntries.Count);
                }
                */
            }
            else
            {
            //    Log.Message(apparel.LabelShortCap + " pauldrons initialized");
                foreach (ShoulderPadEntry item in activeEntries)
                {

                    //    item.drawer = this;
                    /*
                    if (Props.PauldronEntries.Any(x => x.padTexPath == item.padTexPath && x.shoulderPadType == item.shoulderPadType))
                    {
                        item.EastOffset = Props.PauldronEntries.Find(x => x.padTexPath == item.padTexPath && x.shoulderPadType == item.shoulderPadType).EastOffset;
                        item.WestOffset = Props.PauldronEntries.Find(x => x.padTexPath == item.padTexPath && x.shoulderPadType == item.shoulderPadType).WestOffset;
                        item.NorthOffset = Props.PauldronEntries.Find(x => x.padTexPath == item.padTexPath && x.shoulderPadType == item.shoulderPadType).NorthOffset;
                        item.SouthOffset = Props.PauldronEntries.Find(x => x.padTexPath == item.padTexPath && x.shoulderPadType == item.shoulderPadType).SouthOffset;
                    }
                    */
                    if (item.VariantTextures!=null)
                    {
                        item.VariantTextures.Options = Props.PauldronEntries.Find(x => x.padTexPath == item.padTexPath && x.shoulderPadType == item.shoulderPadType).VariantTextures.Options;
                    }
                    
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

        public Pawn pawn
        {
            get
            {
                return this.apparel.Wearer;
            }
        }
        
        
        public Color mainColorFor(ShoulderPadEntry entry)
        {
            if (entry != null)
            {
                if (entry.VariantTextures!=null)
                {
                    if (entry.VariantTextures.activeOption.Color!=null)
                    {
                        return entry.VariantTextures.activeOption.Color.Value;
                    }
                }
                if (entry.UseFactionColors)
                {
                    if (entry.FactionColours(out Color color, out Color colorTwo))
                    {
                        if (color != Color.white)
                        {
                            return color;
                        }
                    }
                }
                if (entry.PrimaryColor != Color.white)
                {
                    return entry.PrimaryColor;
                }
                if (entry.UsePrimaryColor)
                {
                    return this.parent.DrawColor;
                }
                else
                if (entry.UseSecondaryColor)
                {
                    if (entry.SecondaryColor != Color.white)
                    {
                        return entry.SecondaryColor;
                    }
                    return this.parent.DrawColorTwo;
                }
            }
            return this.parent.DrawColor;
        }
        
        public Color secondaryColorFor(ShoulderPadEntry entry)
        {
            if (entry != null)
            {
                if (entry.VariantTextures != null)
                {
                    if (entry.VariantTextures.activeOption.ColorTwo != null)
                    {
                        return entry.VariantTextures.activeOption.ColorTwo.Value;
                    }
                }
                if (entry.UseFactionColors)
                {
                    if (entry.FactionColours(out Color color, out Color colorTwo))
                    {
                        if (colorTwo != Color.white)
                        {
                            return colorTwo;
                        }
                    }
                }
                if (entry.SecondaryColor != Color.white)
                {
                    return entry.SecondaryColor;
                }
                if (entry.UseSecondaryColor)
                {
                    return this.parent.DrawColorTwo;
                }
            }
            return this.parent.DrawColorTwo;
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
            //    Scribe_Values.Look<ShoulderPadEntry>(ref this.shoulderPadEntry, "shoulderPadEntry", null); pauldronInitialized
            Scribe_Values.Look<bool>(ref this.useSecondaryColor, "useSecondaryColor", false, false);
            Scribe_Collections.Look(ref this.activeEntries, "activeEntries", LookMode.Deep); 
        //    Scribe_Defs.Look(ref this.FactionDef, "FactionDef");
        }

        public Vector3 GetAltitudeOffset(Rot4 rotation, ShoulderPadEntry Entry)
        {
            Vector3 offset = new Vector3();
            offset.y += _OffsetFactor * Entry.order;
            offset.y += offset.y + (_SubOffsetFactor * Entry.sublayer);

            bool flag = Find.Selector.SingleSelectedThing == pawn && Prefs.DevMode && DebugSettings.godMode;
            if (!onHead)
            {
                if (rotation == Rot4.North)
                {
                    offset.y += _BodyOffset;
                    if (Entry.northtop)
                    {
                        offset.y += _HairOffset;
                        offset += NorthOffset(Entry);
                    }
                    else
                    {
                        offset += NorthOffset(Entry);
                    }
                }
                else if (rotation == Rot4.West)
                {
                    offset.y += _BodyOffset;
                    offset += WestOffset(Entry);
                }
                else if (rotation == Rot4.East)
                {
                    offset.y += _BodyOffset;
                    offset += EastOffset(Entry);
                }
                else if (rotation == Rot4.South)
                {
                    offset.y += _BodyOffset;
                    offset += SouthOffset(Entry);
                }
                else
                {
                    offset.y += _BodyOffset;
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
            if (flag)
            {
            //    Log.Message(string.Format("{0}'s {1}, {2} offset: {3}, DrawPos.y: {4}", this.pawn.Label, parent.def.label, direction, offset, pawn.Drawer.DrawPos.y));
            }

            return offset;
        }

        public Vector3 NorthOffset(ShoulderPadEntry Entry)
        {
            if (Entry.NorthOffset != Vector3.zero)
            {
                return Entry.NorthOffset;
            }
            return this.Props.NorthOffset;
        }

        public Vector3 SouthOffset(ShoulderPadEntry Entry)
        {
            if (Entry.SouthOffset != Vector3.zero)
            {
                return Entry.SouthOffset;
            }
            return this.Props.SouthOffset;
        }

        public Vector3 EastOffset(ShoulderPadEntry Entry)
        {
            if (Entry.EastOffset != Vector3.zero)
            {
                return Entry.EastOffset;
            }
            return this.Props.EastOffset;
        }

        public Vector3 WestOffset(ShoulderPadEntry Entry)
        {
            if (Entry.WestOffset != Vector3.zero)
            {
                return Entry.WestOffset;
            }
            return this.Props.WestOffset;
        }
        

        public bool ShouldDrawPauldron( Rot4 bodyFacing, Vector2 size, out Graphic pauldronMaterial, ShoulderPadEntry Entry)
        {
            pauldronMaterial = null;
            this.size = size;
            if (pawn.RaceProps.Humanlike)
            {
                if (Entry != null)
                {
                    if (this.CheckPauldronRotation(pawn, Entry.shoulderPadType))
                    {
                        if (Entry.Graphic==null || (Entry.Graphic != null && !Entry.Graphic.path.Contains(pawn.story.bodyType.defName)))
                        {
                            Entry.UpdatePadGraphic();
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
        public Vector2 size = new Vector2 (1.5f, 1.5f);
        public bool CheckPauldronRotation(Pawn pawn, ShoulderPadType shoulderPadType)
        {
            if (shoulderPadType == ShoulderPadType.Left && pawn.Rotation == Rot4.East)
            {
                return false;
            }
            if (shoulderPadType == ShoulderPadType.Right && pawn.Rotation == Rot4.West)
            {
                return false;
            }
            if (shoulderPadType == ShoulderPadType.SouthOnly && pawn.Rotation != Rot4.South)
            {
                return false;
            }
            return true;
        }
        
        //Utility, return if the apparel is worn on the head/body.        
        protected bool onHead
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

        public override string TransformLabel(string label)
        {
            if (true)
            {

            }

            return base.TransformLabel(label);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            Initialize();
            /*
            foreach (var item in activeEntries)
            {
                if (item.VariantTextures != null)
                {
                    item.VariantTextures.Options = Props.PauldronEntries.Find(x => x.padTexPath == item.padTexPath && x.shoulderPadType == item.shoulderPadType).VariantTextures.Options;
                }
            }
            */
        }
    }

}
