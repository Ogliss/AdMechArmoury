using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    [StaticConstructorOnStartup]
    public class CompProperties_PauldronDrawer : CompProperties
    {
        public List<ShoulderPadEntry> PauldronEntries;
        public float PauldronEntryChance = 1f;
        public List<ExtraPartEntry> ExtrasEntries;
        public float ExtraPartEntryChance = 0.5f;
        public int order = 0;
        public float NorthOffset = 0f;
        public float SouthOffset = 0f;
        public float EastOffset = 0f;
        public float WestOffset = 0f;
        public CompProperties_PauldronDrawer()
        {
            this.compClass = typeof(CompPauldronDrawer);
        }

    }
    [StaticConstructorOnStartup]
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
        public bool pauldronInitialized;

        public int entryInd = -1;
        
        public Shader shader
        {
            get
            {
                if (ShoulderPadEntry!=null)
                {
                    return ShoulderPadEntry.shaderType.Shader;
                }
                return ShaderDatabase.Cutout;
            }
        }

        public ShoulderPadEntry shoulderPadEntry;
        public ShoulderPadEntry ShoulderPadEntry
        {
            get
            {
                if (!Props.PauldronEntries.NullOrEmpty() && pauldronInitialized)
                {
                    if (shoulderPadEntry == null)
                    {
                        if (entryInd == -1)
                        {
                            shoulderPadEntry = this.Props.PauldronEntries.RandomElementByWeight((ShoulderPadEntry x) => x.commonality);
                            entryInd = this.Props.PauldronEntries.IndexOf(shoulderPadEntry);
                        }
                        else
                        {
                            shoulderPadEntry = this.Props.PauldronEntries[entryInd];
                        }
                        
                        this.useSecondaryColor = shoulderPadEntry.UseSecondaryColor;
                        this.padType = shoulderPadEntry.shoulderPadType;
                    }
                    else if (entryInd == -1)
                    {
                        entryInd = this.Props.PauldronEntries.IndexOf(shoulderPadEntry);
                    }
                }
                else
                {
                    shoulderPadEntry = null;
                }
                return shoulderPadEntry;
            }
        }

        private Graphic _pauldronGraphic;
        public Graphic PauldronGraphic
        {
            get
            {
                string path = PauldronGraphicPath + "_" + pawn.story.bodyType.ToString();
                this._pauldronGraphic = GraphicDatabase.Get<Graphic_Multi>(path, shader, pawn.Graphic.drawSize, this.mainColor, this.secondaryColor);
                return _pauldronGraphic;
            }
        }

        public string pauldronGraphicPath = null;
        public string PauldronGraphicPath
        {
            get
            {
                if (ShoulderPadEntry!=null)
                {
                    this.pauldronGraphicPath = ShoulderPadEntry.padTexPath;
                }
                return pauldronGraphicPath;
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
        
        public Color mainColor
        {
            get
            {
                if (shoulderPadEntry!=null)
                {
                    if (shoulderPadEntry.PrimaryColor != Color.white)
                    {
                        return shoulderPadEntry.PrimaryColor;
                    }
                    if (shoulderPadEntry.UsePrimaryColor)
                    {
                        return this.parent.DrawColor;
                    }
                    else
                    if (shoulderPadEntry.UseSecondaryColor)
                    {
                        if (shoulderPadEntry.SecondaryColor != Color.white)
                        {
                            return shoulderPadEntry.SecondaryColor;
                        }
                        return this.parent.DrawColorTwo;
                    }
                }
                return this.parent.DrawColor;
            }
        }
        
        public Color secondaryColor
        {
            get
            {
                if (shoulderPadEntry != null)
                {
                    if (shoulderPadEntry.SecondaryColor != Color.white)
                    {
                        return shoulderPadEntry.SecondaryColor;
                    }
                    if (shoulderPadEntry.UseSecondaryColor)
                    {
                        return this.parent.DrawColorTwo;
                    }
                }
                return mainColor;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<string>(ref this.pauldronGraphicPath, "pauldrongraphicPath", null, false);
            Scribe_Values.Look<int>(ref this.entryInd, "entryInd", -1, false);
            Scribe_Values.Look<ShoulderPadType>(ref this.padType, "padType", ShoulderPadType.Both, false);
            //    Scribe_Values.Look<ShoulderPadEntry>(ref this.shoulderPadEntry, "shoulderPadEntry", null); pauldronInitialized
            Scribe_Values.Look<bool>(ref this.pauldronInitialized, "pauldronInitialized", false, false);
            Scribe_Values.Look<bool>(ref this.useSecondaryColor, "useSecondaryColor", false, false);
        }

        public float GetAltitudeOffset(Rot4 rotation)
        {
            float offset = _OffsetFactor * this.ShoulderPadEntry.order;
            offset = offset + (_SubOffsetFactor * this.ShoulderPadEntry.sublayer);

            bool flag = Find.Selector.SingleSelectedThing == pawn && Prefs.DevMode && DebugSettings.godMode;
            if (!onHead)
            {
                if (rotation == Rot4.North)
                {
                    offset += _BodyOffset;
                    if (this.ShoulderPadEntry.northtop)
                    {
                        offset += _HairOffset;
                        offset += NorthOffset(ShoulderPadEntry);
                    }
                    else
                    {
                        offset += NorthOffset(ShoulderPadEntry);
                    }
                }
                else if (rotation == Rot4.West)
                {
                    offset += _BodyOffset;
                    offset += WestOffset(ShoulderPadEntry);
                }
                else if (rotation == Rot4.East)
                {
                    offset += _BodyOffset;
                    offset += EastOffset(ShoulderPadEntry);
                }
                else if (rotation == Rot4.South)
                {
                    offset += _BodyOffset;
                    offset += SouthOffset(ShoulderPadEntry);
                }
                else
                {
                    offset += _BodyOffset;
                }
            }
            else
            {
                if (rotation == Rot4.North)
                {
                    offset += _BodyOffset;
                }
                else
                    offset += _HeadOffset;
            }
            if (flag)
            {
            //    Log.Message(string.Format("{0}'s {1}, {2} offset: {3}, DrawPos.y: {4}", this.pawn.Label, parent.def.label, direction, offset, pawn.Drawer.DrawPos.y));
            }

            return offset;
        }

        public float NorthOffset(ShoulderPadEntry Entry)
        {
            if (Entry.NorthOffset != 0)
            {
                return Entry.NorthOffset;
            }
            return this.Props.NorthOffset;
        }

        public float SouthOffset(ShoulderPadEntry Entry)
        {
            if (Entry.SouthOffset != 0)
            {
                return Entry.SouthOffset;
            }
            return this.Props.SouthOffset;
        }

        public float EastOffset(ShoulderPadEntry Entry)
        {
            if (Entry.EastOffset != 0)
            {
                return Entry.EastOffset;
            }
            return this.Props.EastOffset;
        }

        public float WestOffset(ShoulderPadEntry Entry)
        {
            if (Entry.WestOffset != 0)
            {
                return Entry.WestOffset;
            }
            return this.Props.WestOffset;
        }
        

        public bool ShouldDrawPauldron(Pawn pawn, Rot4 bodyFacing, out Material pauldronMaterial)
        {
            pauldronMaterial = null;
            if (pawn.RaceProps.Humanlike)
            {
                if (this.ShoulderPadEntry != null)
                {
                    if (this.CheckPauldronRotation(pawn, this.padType))
                    {
                        pauldronMaterial = this.PauldronGraphic.GetColoredVersion(shader, this.mainColor, this.secondaryColor).MatAt(bodyFacing);
                        return true;
                    }
                    else
                    {
                    //    Log.Message(string.Format("CheckPauldronRotation false"));
                    }
                }
                else
                {
                //    Log.Message(string.Format("this.PauldronGraphic = null"));
                }
            }
            else
            {
            //    Log.Message(string.Format("pawn.needs = null && pawn.story = null"));
            }
            return false;

        }

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
    }

    public enum ShoulderPadType
    {
        Both,
        Right,
        Left
    }

    [StaticConstructorOnStartup]
    public class ShoulderPadEntry
    {
        public ShoulderPadType shoulderPadType;
        public ShaderTypeDef shaderType;
        public string padTexPath;
        public int commonality;
        public bool northtop = false;
        public bool UseFactionTextures;
        public bool UsePrimaryColor;
        public Color PrimaryColor = Color.white;
        public bool UseSecondaryColor;
        public Color SecondaryColor = Color.white;
        public int order = 1;
        public int sublayer = 0;
        public float NorthOffset = 0f;
        public float SouthOffset = 0f;
        public float EastOffset = 0f;
        public float WestOffset = 0f;
    }
    
}
