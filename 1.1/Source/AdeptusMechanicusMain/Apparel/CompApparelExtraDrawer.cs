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
    public class CompProperties_ApparelExtraDrawer : CompProperties
    {
        public List<ExtraPartEntry> ExtrasEntries;
        public float ExtraPartEntryChance = 0.5f;
        public int order = 0;
        public float NorthOffset = 0f;
        public float SouthOffset = 0f;
        public float EastOffset = 0f;
        public float WestOffset = 0f;
        public ApparelLayerDef ApparelLayer = null;
        public CompProperties_ApparelExtraDrawer()
        {
            this.compClass = typeof(CompApparelExtraDrawer);
        }

    }
    [StaticConstructorOnStartup]
    public class CompApparelExtraDrawer : ThingComp
    {
        public CompProperties_ApparelExtraDrawer pprops
        {
            get
            {
                return this.props as CompProperties_ApparelExtraDrawer;
            }
        }

        public const float MinClippingDistance = 0.002f;   // Minimum space between layers to avoid z-fighting
        const float _HeadOffset = 0.02734375f + MinClippingDistance;
        const float _HairOffset = 035f + MinClippingDistance;       // Number must be same as PawnRenderer.YOffset_Head
        const float _BodyOffset = 0.0234375f + MinClippingDistance;   // Number must be same as PawnRenderer.YOffset_Shell
        const float _OffsetFactor = 0.001f;
        const float _SubOffsetFactor = 0.0001f;
        static readonly Dictionary<string, bool> _OnHeadCache = new Dictionary<string, bool>();
        public Shader shader = ShaderDatabase.Cutout;
        public bool ExtraUseBodyOffset; 
        private bool useSecondaryColor;
        //    private bool useFactionTextures = false;
#pragma warning disable IDE0052 // Remove unread private members
        private bool pauldronInitialized = false;
#pragma warning restore IDE0052 // Remove unread private members

        public ExtraPartEntry extraPartEntry;
        public ExtraPartEntry ExtraPartEntry
        {
            get
            {
                if (!pprops.ExtrasEntries.NullOrEmpty())
                {
                    if (extraPartEntry == null)
                    {
                        extraPartEntry = this.pprops.ExtrasEntries.RandomElementByWeight((ExtraPartEntry x) => x.commonality);


                        this.shader = ShaderDatabase.LoadShader(extraPartEntry.shaderType.shaderPath);
                        this.useSecondaryColor = extraPartEntry.UseSecondaryColor;
                        this.ExtraUseBodyOffset = extraPartEntry.UseBodytypeOffsets;
                        pauldronInitialized = true;
                    }
                }
                else
                {
                    extraPartEntry = null;
                }
                return extraPartEntry;
            }
        }

        private Graphic _Graphic;
        public Graphic Graphic
        {
            get
            {
                string path = GraphicPath;
                if (ExtraUseBodyOffset && !onHead)
                {
                    path += "_" + pawn.story.bodyType.ToString();
                }
                this._Graphic = GraphicDatabase.Get<Graphic_Multi>(path, shader, pawn.Graphic.drawSize, this.mainColor, this.secondaryColor);
                return _Graphic;
            }
        }

        public string graphicPath;
        public string GraphicPath
        {
            get
            {
                this.graphicPath = ExtraPartEntry.extraTexPath;
                return graphicPath;
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

        public Color MainColor = Color.white;
        public Color mainColor
        {
            get
            {
                if (ExtraPartEntry != null)
                {
                    if (ExtraPartEntry.PrimaryColor != Color.white)
                    {
                        return ExtraPartEntry.PrimaryColor;
                    }
                    if (ExtraPartEntry.UsePrimaryColor)
                    {
                        return this.parent.DrawColor;
                    }
                    else
                    if (ExtraPartEntry.UseSecondaryColor)
                    {
                        if (ExtraPartEntry.SecondaryColor != Color.white)
                        {
                            return ExtraPartEntry.SecondaryColor;
                        }
                        return this.parent.DrawColorTwo;
                    }
                }
                return this.parent.DrawColor;
            }
        }

        public Color SecondaryColor = Color.white;
        public Color secondaryColor
        {
            get
            {
                if (SecondaryColor == Color.white)
                {
                    SecondaryColor = this.parent.DrawColorTwo;
                    //    Log.Message(string.Format("CompApparelExtaDrawer return {1}'s DrawColorTwo {0}", SecondaryColor, this.parent.def.label));
                    return SecondaryColor;
                }
                if (SecondaryColor != Color.white)
                {
                    return SecondaryColor;
                }
                if (this.useSecondaryColor)
                {
                    return this.parent.DrawColorTwo;
                }
                return mainColor;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<bool>(ref this.useSecondaryColor, "useSecondaryColor", false, false);
            Scribe_Values.Look<string>(ref this.graphicPath, "extragraphicPath", null, false);
            Scribe_Values.Look<bool>(ref this.ExtraUseBodyOffset, "UseBodyOffset", false);
        //    Scribe_Values.Look<ExtraPartEntry>(ref this.extraPartEntry, "ExtraPartEntry", null);
        }
        
        public float GetAltitudeOffset(Rot4 rotation, ExtraPartEntry partEntry)
        {
            float offset = _OffsetFactor * partEntry.order;
            offset = offset + (_SubOffsetFactor * partEntry.sublayer);

            bool flag = Find.Selector.SingleSelectedThing == pawn && Prefs.DevMode && DebugSettings.godMode;
            if (!onHead)
            {
                if (rotation == Rot4.North)
                {
                    offset += _BodyOffset;
                    if (partEntry.northtop)
                    {
                        offset += _HairOffset;
                        offset += NorthOffset(partEntry);
                    }
                    else
                    {
                        offset += NorthOffset(partEntry);
                    }
                }
                else if (rotation == Rot4.West)
                {
                    offset += _BodyOffset;
                    offset += WestOffset(partEntry);
                }
                else if (rotation == Rot4.East)
                {
                    offset += _BodyOffset;
                    offset += EastOffset(partEntry);
                }
                else if (rotation == Rot4.South)
                {
                    offset += _BodyOffset;
                    offset += SouthOffset(partEntry);
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
        
        public float NorthOffset(ExtraPartEntry Entry)
        {
            float result = 0;
            if (Entry.NorthOffset != 0)
            {
                result += Entry.NorthOffset;
                return result;
            }
            result += this.pprops.NorthOffset;
            return this.pprops.NorthOffset;
        }
        
        public float SouthOffset(ExtraPartEntry Entry)
        {
            if (Entry.SouthOffset != 0)
            {
                return Entry.SouthOffset;
            }
            return this.pprops.SouthOffset;
        }

        public float EastOffset(ExtraPartEntry Entry)
        {
            if (Entry.EastOffset != 0)
            {
                return Entry.EastOffset;
            }
            return this.pprops.EastOffset;
        }
        
        public float WestOffset(ExtraPartEntry Entry)
        {
            if (Entry.WestOffset != 0)
            {
                return Entry.WestOffset;
            }
            return this.pprops.WestOffset;
        }
        
        public bool ShouldDrawExtra(Pawn pawn, Apparel curr, Rot4 bodyFacing, out Material extraMaterial)
        {
            extraMaterial = null;
            if (pawn.needs != null && pawn.story != null)
            {
                if (this.Graphic != null)
                {
                    extraMaterial = this.Graphic.GetColoredVersion(shader, this.mainColor, this.secondaryColor).MatAt(bodyFacing);
                    return true;
                }
            }
            return false;

        }
        
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!this.pprops.ExtrasEntries.NullOrEmpty())
            {
                extraPartEntry = this.pprops.ExtrasEntries.RandomElementByWeight((ExtraPartEntry x) => x.commonality);
                this.graphicPath = extraPartEntry.extraTexPath;
                this.shader = ShaderDatabase.LoadShader(extraPartEntry.shaderType.shaderPath);
                this.useSecondaryColor = extraPartEntry.UseSecondaryColor;
                this.ExtraUseBodyOffset = extraPartEntry.UseBodytypeOffsets;
            }
            pauldronInitialized = true;

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
    }
    
    [StaticConstructorOnStartup]
    public class ExtraPartEntry
    {
        public bool OnHead;
        public bool UseBodytypeOffsets;
        public ShaderTypeDef shaderType;
        public string extraTexPath;
        public int commonality;
        public bool UsePrimaryColor;
        public Color PrimaryColor = Color.white;
        public bool UseSecondaryColor;
        public Color SecondaryColor = Color.white;
        public bool UseFactionTextures;
        public int order = 1;
        public int sublayer = 0;
        public bool northtop = false;
        public float NorthOffset = 0f;
        public float SouthOffset = 0f;
        public float EastOffset = 0f;
        public float WestOffset = 0f;
        // Token: 0x0400006D RID: 109
        private bool validated = false;
    }

}
