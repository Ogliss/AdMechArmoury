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
    [StaticConstructorOnStartup]
    public class CompProperties_ApparelExtraPartDrawer : CompProperties
    {
        public CompProperties_ApparelExtraPartDrawer()
        {
            this.compClass = typeof(CompApparelExtraPartDrawer);
        }
        public List<ExtraApparelPartProps> ExtrasEntries;
        public float ExtraPartEntryChance = 0.5f;
        public int order = 0;
        public Vector3 NorthOffset = new Vector3();
        public Vector3 SouthOffset = new Vector3();
        public Vector3 EastOffset = new Vector3();
        public Vector3 WestOffset = new Vector3();
        public ApparelLayerDef ApparelLayer = null;
        public bool hidesHair = false;
        public bool hidesHead = false;
        public bool hidesBody = false;
        public bool onHead = false;

    }
    [StaticConstructorOnStartup]
    public class CompApparelExtraPartDrawer : ThingComp
    {
        public CompProperties_ApparelExtraPartDrawer Props
        {
            get
            {
                return this.props as CompProperties_ApparelExtraPartDrawer;
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
        public Shader Shader => ShaderDatabase.Cutout;
        public bool ExtraUseBodyTexture; 
        public bool ExtraUseHeadOffset; 
        private bool useSecondaryColor;
        public bool hidesHair => Props.hidesHair || Props.ExtrasEntries.Any(x=> x.hidesHair);
        public bool hidesHead => Props.hidesHead || Props.ExtrasEntries.Any(x => x.hidesHead);
        public bool hidesBody => Props.hidesBody || Props.ExtrasEntries.Any(x => x.hidesBody);
        public BodyTypeDef forcedBodyType => Props.ExtrasEntries.First(x => x.forcedBodyType!=null)?.forcedBodyType;
        //    private bool useFactionTextures = false;
#pragma warning disable IDE0052 // Remove unread private members
        private bool partInitialized = false;
#pragma warning restore IDE0052 // Remove unread private members

        private ExtraApparelPartProps extraPartEntry;
        private int extraPartEntryint;
        public ExtraApparelPartProps ExtraPartEntry
        {
            get
            {
                StringBuilder msg = new StringBuilder(Apparel.LabelCap);
                if (!Props.ExtrasEntries.NullOrEmpty())
                {
                    msg.AppendLine("ExtrasEntries: " + Props.ExtrasEntries.Count);
                    msg.AppendLine("    extraPartEntryint: " + extraPartEntryint);
                    if (extraPartEntry == null)
                    {
                        msg.AppendLine("        extraPartEntry: Initalize");
                        if (extraPartEntryint >= 0)
                        {
                            msg.AppendLine("        extraPartEntry: Load");
                        }
                        else
                        {
                            msg.AppendLine("        extraPartEntry: Make New");
                        }
                        this.GeneratePart(ref msg);
                        msg.AppendLine("        Initialized: " + partInitialized);
                    }
                    else
                    {
                        msg.AppendLine("    Entry: " + extraPartEntryint);
                    }
                }
                else
                {
                    extraPartEntry = null;
                    msg.AppendLine("ExtraPartEntry: null");
                }
                if (!logged && loging && msg.Length > 0)
                {
                    Log.Message(msg.ToString());
                    logged = true;
                }
                return extraPartEntry;
            }
        }
        bool logged = false;
        bool loging = false;

        public void GeneratePart(ref StringBuilder msg)
        {
            if (extraPartEntryint >= 0)
            {
                extraPartEntry = this.Props.ExtrasEntries[extraPartEntryint];
            }
            else
            {
                List<ExtraApparelPartProps> possibles = new List<ExtraApparelPartProps>();
                if (extraPartEntryint == -1)
                {
                    if (Apparel.TryGetQuality(out QualityCategory quality))
                    {
                        msg.AppendLine("            QualityCategory: " + quality);
                        possibles.AddRange(this.Props.ExtrasEntries.FindAll(x => x.AcceptableForQuality(quality)));
                    }
                    else
                    {
                        msg.AppendLine("            No CompQuality");
                        possibles = this.Props.ExtrasEntries;

                    }
                }
                msg.AppendLine("            possibles: " + possibles.Count);
                if (!possibles.NullOrEmpty())
                {
                    Rand.PushState();
                    extraPartEntry = possibles.RandomElementByWeight((ExtraApparelPartProps x) => x.commonality);
                    Rand.PopState();
                    extraPartEntryint = this.Props.ExtrasEntries.IndexOf(extraPartEntry);

                }
                else extraPartEntryint = -2;

            }
            if (extraPartEntry != null)
            {
                this.shader = ShaderDatabase.LoadShader(extraPartEntry.graphicData.shaderType.shaderPath);
                this.useSecondaryColor = extraPartEntry.useParentSecondaryColor;
                this.ExtraUseBodyTexture = extraPartEntry.UseBodytypeTextures;
                msg.AppendLine("    Using: " + extraPartEntry + " at position: " + extraPartEntryint);
                msg.AppendLine("        shader: " + shader + " useSecondaryColor: " + useSecondaryColor + " ExtraUseBodyTexture: " + ExtraUseBodyTexture);
                partInitialized = true;
            }
            else
            {
                extraPartEntryint = -2;
            }
        }

        private Graphic _Graphic;
        public Graphic Graphic
        {
            get
            {
                if (_Graphic == null)
                {
                    string path = GraphicPath;
                    if (ExtraUseBodyTexture && !onHead && !ExtraPartEntry.OnHead && !Props.onHead)
                    {
                        path += "_" + pawn.story.bodyType.ToString();
                    }
                    this._Graphic = GraphicDatabase.Get<Graphic_Multi>(path, shader, pawn.Graphic.drawSize, this.mainColor, this.secondaryColor);
                }
                return _Graphic;
            }
        }

        public void UpdateGraphic()
        {
            _Graphic = null;
        }

        public string graphicPath;
        public string GraphicPath
        {
            get
            {
                this.graphicPath = ExtraPartEntry.graphicData.texPath;
                return graphicPath;
            }
        }

        public Apparel apparel;
        public Apparel Apparel
        {
            get
            {
                return apparel ??= this.parent as Apparel;
            }
        }

        public Pawn pawn
        {
            get
            {
                return this.Apparel?.Wearer;
            }
        }

        private CompColorable colours;
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

        private CompColorableTwo coloursTwo;
        private bool colorTwotest = false;
        public CompColorableTwo ColoursTwo
        {
            get
            {
                if (colours == null)
                {
                    return null;
                }
                if (coloursTwo == null && !colorTwotest)
                {
                    colorTwotest = true;
                    coloursTwo = Colours as CompColorableTwo;
                }
                return factioncolours;
            }
        }
        private CompColorableTwoFaction factioncolours;
        private bool factioncolortest = false;
        public CompColorableTwoFaction FactionColours
        {
            get
            {
                if (colours == null)
                {
                    return null;
                }
                if (factioncolours == null && !factioncolortest)
                {
                    factioncolortest = true;
                    factioncolours = Colours as CompColorableTwoFaction;
                }
                return factioncolours;
            }
        }

        public Color MainColor = Color.white;
        public Color mainColor
        {
            get
            {
                if (ExtraPartEntry != null)
                {
                    if (ExtraPartEntry.UseFactionColors && factioncolours != null)
                    {
                        if (factioncolours.Active)
                        {
                            //   Log.Message("mainColorFor " + entry.shoulderPadType + " UseFactionColors");
                            return factioncolours.Color;
                        }
                    }
                    if (!ExtraPartEntry.useParentPrimaryColor)
                    {
                        return ExtraPartEntry.graphicData.color;
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
                if (ExtraPartEntry != null)
                {
                    if (ExtraPartEntry.UseFactionColors && factioncolours != null)
                    {
                        if (factioncolours.ActiveTwo)
                        {
                            //   Log.Message("mainColorFor " + entry.shoulderPadType + " UseFactionColors");
                            return factioncolours.ColorTwo;
                        }
                    }
                    if (!ExtraPartEntry.useParentSecondaryColor)
                    {
                        return ExtraPartEntry.graphicData.colorTwo;
                    }
                }
                return this.parent.DrawColorTwo;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<bool>(ref this.partInitialized, "partInitialized", false, false);
            Scribe_Values.Look<bool>(ref this.useSecondaryColor, "useSecondaryColor", false, false);
            Scribe_Values.Look<string>(ref this.graphicPath, "extragraphicPath", null, false);
            Scribe_Values.Look<bool>(ref this.ExtraUseBodyTexture, "UseBodyOffset", false);
            Scribe_Values.Look<int>(ref this.extraPartEntryint, "extraPartEntryint", -1);
            //    Scribe_Values.Look<ExtraPartEntry>(ref this.extraPartEntry, "ExtraPartEntry", null);
        }

        public Vector3 bitPosition = Vector3.zero;
        private bool bitFloatingDown = true;
        private bool bitFloatingRight = true;
        private float bitOffsetZ = 0.0f;
        private float bitOffsetX = 0.0f;
        private float Xlimit = 0.25f;
        private Vector3 AnimateExtraBit(Vector3 origin)
        {
            if (!Find.TickManager.Paused)
            {
                if (bitFloatingDown)
                {
                    if (this.bitOffsetZ < -0.07f)
                    {
                        this.bitFloatingDown = false;
                    }
                    this.bitOffsetZ -= 0.0005f * Find.TickManager.TickRateMultiplier;
                }
                else
                {
                    if (this.bitOffsetZ > 0.12f)
                    {
                        this.bitFloatingDown = true;
                    }
                    this.bitOffsetZ += 0.0005f * Find.TickManager.TickRateMultiplier;
                }
                if (bitFloatingRight)
                {
                    if (this.bitOffsetX > Xlimit || Rand.Chance(Mathf.InverseLerp(0, Xlimit, this.bitOffsetX) * 0.1f))
                    {
                        this.bitFloatingRight = false;
                    }
                    this.bitOffsetX += 0.0005f * Find.TickManager.TickRateMultiplier;
                }
                else
                {
                    if (this.bitOffsetX < -Xlimit || Rand.Chance(Mathf.InverseLerp(0, -Xlimit, this.bitOffsetX) * 0.1f))
                    {
                        this.bitFloatingRight = true;
                    }
                    this.bitOffsetX -= 0.0005f * Find.TickManager.TickRateMultiplier;
                }
            }
            this.bitPosition = origin == default ? pawn.Drawer.DrawPos : origin;
            this.bitPosition.x += this.bitOffsetX;
            this.bitPosition.z += this.bitOffsetZ;
        //    this.bitPosition.y = AltitudeLayer.Blueprint.AltitudeFor();
            return bitPosition;
        }

        public Vector3 GetOffset(Rot4 rotation, ExtraApparelPartProps partEntry)
        {
            Vector3 offset = new Vector3();
            if (!partEntry.hidesBody)
            {
                offset.y = _OffsetFactor * partEntry.order;
                offset.y = offset.y + (_SubOffsetFactor * partEntry.sublayer);
            }

            if (!onHead || !Props.onHead || !ExtraPartEntry.OnHead)
            {
                offset.y += _BodyOffset;
                if (rotation == Rot4.North)
                {
                    if (partEntry.northtop)
                    {
                        offset.y += _HairOffset;
                        offset += NorthOffset(partEntry);
                    }
                    else
                    {
                        offset += NorthOffset(partEntry);
                    }
                }
                else if (rotation == Rot4.West)
                {
                    offset += WestOffset(partEntry);
                }
                else if (rotation == Rot4.East)
                {
                    offset += EastOffset(partEntry);
                }
                else if (rotation == Rot4.South)
                {
                    offset += SouthOffset(partEntry);
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

            if (partEntry.animateExtra)
            {
                offset = AnimateExtraBit(offset);
            }
            return offset;
        }

        public Vector3 NorthOffset(ExtraApparelPartProps Entry)
        {
            if (Entry.NorthOffset != Vector3.zero)
            {
                return Entry.NorthOffset + this.Props.NorthOffset;
            }
            return this.Props.NorthOffset;
        }

        public Vector3 SouthOffset(ExtraApparelPartProps Entry)
        {
            if (Entry.SouthOffset != Vector3.zero)
            {
                return Entry.SouthOffset + this.Props.SouthOffset;
            }
            return this.Props.SouthOffset;
        }

        public Vector3 EastOffset(ExtraApparelPartProps Entry)
        {
            if (Entry.EastOffset != Vector3.zero)
            {
                return Entry.EastOffset + this.Props.EastOffset;
            }
            return this.Props.EastOffset;
        }

        public Vector3 WestOffset(ExtraApparelPartProps Entry)
        {
            if (Entry.WestOffset != Vector3.zero)
            {
                return Entry.WestOffset + this.Props.WestOffset;
            }
            return this.Props.WestOffset;
        }


        public bool ShouldDrawExtra(Pawn pawn, Apparel curr, Rot4 bodyFacing, out Material extraMaterial)
        {
            extraMaterial = null;
            if (pawn.InBed() && !onHead)
            {
                extraMaterial = null;
                return false;
            }
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
            if (!this.Props.ExtrasEntries.NullOrEmpty())
            {
                /*
                extraPartEntry = this.Props.ExtrasEntries.RandomElementByWeight((ExtraApparelPartProps x) => x.commonality);
                this.graphicPath = extraPartEntry.graphicData.texPath;
                this.shader = ShaderDatabase.LoadShader(extraPartEntry.graphicData.shaderType.shaderPath);
                this.useSecondaryColor = extraPartEntry.useParentSecondaryColor;
                this.ExtraUseBodyTexture = extraPartEntry.UseBodytypeTextures;
                */
                StringBuilder msg = new StringBuilder("PostSpawnSetup ");
                this.GeneratePart(ref msg);
            }
            partInitialized = true;

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
                            if (p.groups.Contains(BodyPartGroupDefOf.FullHead) || p.groups.Contains(BodyPartGroupDefOf.UpperHead) || p.groups.Contains(BodyPartGroupDefOf.Eyes) || p.groups.Contains(AdeptusBodyPartGroupDefOf.Mouth) || p.groups.Contains(AdeptusBodyPartGroupDefOf.Neck))
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

}
