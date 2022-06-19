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

    // AdeptusMechanicus.ApparelProperties
    /*
    public class ApparelProperties : RimWorld.ApparelProperties
    {
        public List<string> wornGraphicMasks = new List<string>();

    }
    */

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
                        if (Quality != null && AltGraphicsExt.QualityControled)
                        {
                            altGraphics.RemoveAll(x => !x.SuitableQuality(Quality.Quality));
                        }

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
                if (activeAltGraphic == null && activeAltInt > -1 && AltGraphicsExt != null)
                {
                    if (!activeAltKey.NullOrEmpty()) activeAltGraphic = AltGraphics?.Find(x => x.saveKey == activeAltKey);
                    else
                    {
                        List<AlternateApparelGraphic> useable = AltGraphics;
                        int used = activeAltInt;
                        /*
                        if (Quality != null && AltGraphicsExt.QualityControled)
                        {
                            useable.RemoveAll(x => !x.SuitableQuality(Quality.Quality));
                        }
                        */
                        if (AltGraphicsExt.randomizeInital)
                        {
                            used = Rand.Range(0, useable.Count - 1);
                        }
                        activeAltGraphic = useable?[Math.Min(used, useable.Count - 1)];
                    }
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
        bool triedQuality = false;
        public CompQuality quality;
        public CompQuality Quality
        {
            get
            {
                if (quality == null && !triedQuality)
                {
                    quality = this.TryGetCompFast<CompQuality>();
                    triedQuality = true;
                }
                return quality;
            }
        }
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
                if (pauldrons == null)
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
                if (extras == null)
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

        public override void DrawWornExtras()
        {
            base.DrawWornExtras();
            if (!Pauldrons.NullOrEmpty())
            {
                PawnRenderFlags flags = Wearer.Drawer.renderer.GetDefaultRenderFlags(Wearer);
                Quaternion quat = Quaternion.AngleAxis(0, Vector3.up);
                Vector3 vector = Wearer.Drawer.renderer.GetBodyPos(Wearer.DrawPos, out bool renderBody);
                Rot4 headfacing = Wearer.Rotation;
                Rot4 bodyFacing = Wearer.Rotation;
                bool portrait = flags.FlagSet(PawnRenderFlags.Portrait);
                Vector2 size = (portrait ? MeshPool.humanlikeBodySet.MeshAt(bodyFacing).bounds.size : Wearer.Drawer.renderer.graphics.nakedGraphic.MeshAt(bodyFacing).bounds.size);
                if (AdeptusIntergrationUtility.enabled_AlienRaces)
                {
                    PawnRenderUtility.AlienRacesPatch(Wearer, bodyFacing, out size, portrait);
                }
                else
                {
                    size = new Vector2(1.5f, 1.5f);
                }
                foreach (CompPauldronDrawer Pauldron in Pauldrons)
                {
                    Vector3 center = vector + (quat * Pauldron.GetOffsetFor(bodyFacing, false));
                    if (Pauldron.activeEntries.NullOrEmpty())
                    {
                        Pauldron.Initialize();
                    }
                    foreach (ShoulderPadEntry entry in Pauldron.activeEntries)
                    {
                        //    entry.Drawer = Pauldron;
                        if (entry.apparel == null)
                        {
                            entry.apparel = this;
                        }
                        if (entry.Drawer == null)
                        {
                            Log.Warning("Warning! Drawer null");
                        }
                        if (!entry.forceDynamicDraw)
                        {
                            continue;
                        }

                        if (entry.ShouldDrawEntry(flags, bodyFacing, size, renderBody, out Graphic pauldronMat, out Mesh pauldronMesh, out Vector3 offset))
                        {
                            if (Pauldron.onHead || renderBody)
                            {
                                Vector3 drawAt = Wearer.Drawer.DrawPos + (quat * Pauldron.GetOffsetFor(bodyFacing, false));
                                /*
                                drawAt.y += 0.009687258f;
                                drawAt.y += 0.022166021f;
                                drawAt.y += 0.028957527f;
                                drawAt.y += 0.97f;
                                */
                                GenDraw.DrawMeshNowOrLater
                                    (
                                        // pauldronMesh,
                                        PawnRenderUtility.GetPawnMesh(portrait, Wearer, entry.Props.flipWest && bodyFacing == Rot4.West ? bodyFacing.Opposite : bodyFacing, !Pauldron.onHead),
                                        drawAt + (quat * offset),
                                        quat,
                                        PawnRenderUtility.OverrideMaterialIfNeeded(pauldronMat.MatAt(bodyFacing), Wearer),
                                        flags.FlagSet(PawnRenderFlags.DrawNow)
                                    );
                            }
                        }
                    }
                }
            }
            if (!Extras.NullOrEmpty())
            {
                PawnRenderFlags flags = Wearer.Drawer.renderer.GetDefaultRenderFlags(Wearer);
                Quaternion quat = Quaternion.AngleAxis(0, Vector3.up);
                Vector3 vector = Wearer.DrawPos;
                Rot4 headfacing = Wearer.Rotation;
                Rot4 bodyFacing = Wearer.Rotation;
                bool portrait = flags.FlagSet(PawnRenderFlags.Portrait);
                Vector2 size = (portrait ? MeshPool.humanlikeBodySet.MeshAt(bodyFacing).bounds.size : Wearer.Drawer.renderer.graphics.nakedGraphic.MeshAt(bodyFacing).bounds.size);
                if (AdeptusIntergrationUtility.enabled_AlienRaces)
                {
                    PawnRenderUtility.AlienRacesPatch(Wearer, bodyFacing, out size, portrait);
                }
                else
                {
                    size = new Vector2(1.5f, 1.5f);
                }
                foreach (CompApparelExtraPartDrawer ExtraDrawer in Extras)
                {
                    Vector3 drawAt = Wearer.DrawPos;
                    if (!ExtraDrawer.Props.ExtrasEntries.NullOrEmpty())
                    {
                        bool onHead = ExtraDrawer.onHead || ExtraDrawer.ExtraPartEntry.OnHead || ExtraDrawer.Props.onHead;
                        Rot4 facing = onHead ? headfacing : bodyFacing;
                        if (!ExtraDrawer.ExtraPartEntry.DynamicDraw)
                        {
                            continue;
                        }
                        if (ExtraDrawer.ShouldDrawExtra(Wearer, this, facing, out Material extraMat))
                        {
                            if (onHead || !PawnRenderFlagsExtension.FlagSet(flags, PawnRenderFlags.Invisible))
                            {
                                if (onHead)
                                {
                                    Vector3 v = vector + quat * Wearer.Drawer.renderer.BaseHeadOffsetAt(headfacing);
                                    drawAt = v + quat * new Vector3(ExtraDrawer.GetOffset(bodyFacing, ExtraDrawer.ExtraPartEntry).x * size.x, ExtraDrawer.GetOffset(bodyFacing, ExtraDrawer.ExtraPartEntry).y, ExtraDrawer.GetOffset(bodyFacing, ExtraDrawer.ExtraPartEntry).z * size.y);

                                }
                                else
                                {
                                    drawAt = vector + (quat * new Vector3(ExtraDrawer.GetOffset(bodyFacing, ExtraDrawer.ExtraPartEntry).x * size.x, ExtraDrawer.GetOffset(bodyFacing, ExtraDrawer.ExtraPartEntry).y, ExtraDrawer.GetOffset(bodyFacing, ExtraDrawer.ExtraPartEntry).z * size.y));
                                }
                                /*
                                GenDraw.DrawMeshNowOrLater
                                    (
                                        // pauldronMesh,
                                        PawnRenderUtility.GetPawnMesh(portrait, Wearer, facing, !onHead),
                                        drawAt,
                                        quat,
                                        PawnRenderUtility.OverrideMaterialIfNeeded(extraMat, Wearer),
                                        ExtraDrawer.ExtraPartEntry.animateExtra
                                    );
                                */
                                Graphics.DrawMesh(
                                    PawnRenderUtility.GetPawnMesh(portrait, Wearer, facing, !onHead),
                                    drawAt,
                                    quat,
                                    PawnRenderUtility.OverrideMaterialIfNeeded(extraMat, Wearer),
                                    0);
                            }
                            //    vector.y += CompApparelExtaDrawer.MinClippingDistance;
                        }
                    }
                }
            }
            
            if (!Shields.NullOrEmpty())
            {
                foreach (var item in Shields)
                {
                    item.DrawShield();
                }
            }
            
        }

        private List<Comp_Shield> shields;
        public List<Comp_Shield> Shields
        {
            get
            {
                if (shields == null)
                {
                //    Log.Message("generating shieldlist");
                    shields = new List<Comp_Shield>();
                    for (int i = 0; i < this.AllComps.Count; i++)
                    {
                        if (this.AllComps[i] is Comp_Shield shield)
                        {
                        //    Log.Message("adding shield to shieldlist");
                            shields.Add(shield);
                        }
                    }
                //    Log.Message("generated shieldlist: " + shields.Count);
                }
                return shields;
            }
        }

        public override bool CheckPreAbsorbDamage(DamageInfo dinfo)
        {
            foreach (Comp_Shield shield in Shields)
            {
                if (shield.CheckPreAbsorbDamage(dinfo))
                {
                    return true;
                }
            }
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
                /*
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
                */
                if (!Shields.NullOrEmpty())
                {
                    foreach (var item in Shields)
                    {
                        yield return item.GetShieldGizmos();
                    }
                }

            }
            yield break;
        }



        /*
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
        */

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
        public new string WornGraphicPath
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
            //    Log.Message("using " + r);
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
                    //    Log.Message("shit went tits up");
                        failedgraphic = true;
                    }
                }
                return base.Graphic;
            }
        }

        public override Color DrawColor { get => ColoursExt?.factionColor ?? base.DrawColor; set => base.DrawColor = value; }
        public override Color DrawColorTwo => ColoursExt?.factionColorTwo ?? base.DrawColorTwo;

        public override bool AllowVerbCast(Verb verb)
        {
            if (!Shields.NullOrEmpty())
            {
                foreach (var item in Shields)
                {
                    if (!item.AllowVerbCast(verb))
                    {
                        return false;
                    }
                }
            }
            return base.AllowVerbCast(verb);
        }

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