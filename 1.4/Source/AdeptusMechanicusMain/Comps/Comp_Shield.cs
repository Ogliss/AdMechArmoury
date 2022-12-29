using AdeptusMechanicus.ExtensionMethods;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace AdeptusMechanicus
{

    public class CompProperties_Shield : RimWorld.CompProperties_Shield
    {
        public CompProperties_Shield()
        {
            this.compClass = typeof(Comp_Shield);
        }
        public float energyShieldEnergyMax;
        public float energyShieldRechargeRate;
        public float drawX = 1f;
        public float drawY = 1f;
        public bool psykerRequired = false;
        public bool allowRangedAttacks = false;
        public bool allowMeleeAttacks = true;
        public bool blocksMeleeWeapons = false;
        public bool brokenByEMP = true;
        public List<DamageDef> bypassingDamage = new List<DamageDef>();

        public string shieldTexPath;
        public Color shieldColor;
        public GraphicData graphicData;
        public Vector3 offset = new Vector3();
    }

    [StaticConstructorOnStartup]
    public class Comp_Shield : CompShield
    {
        public new virtual CompProperties_Shield Props
        {
            get
            {
                return this.props as CompProperties_Shield;
            }
        }

        public new virtual float EnergyMax
        {
            get
            {
                if (this.parent is Apparel apparel)
                {
                    return apparel.GetStatValue(StatDefOf.EnergyShieldEnergyMax, true);
                }
                return Props.energyShieldEnergyMax;
            }
        }

        public new virtual float EnergyGainPerTick
        {
            get
            {
                if (this.parent is Apparel apparel)
                {
                    if (Props.psykerRequired)
                    {
                        if (apparel.Wearer != null && apparel.Wearer.isPsyker(out int Level, out float Mult))
                        {
                            return Math.Max(0, ((apparel.GetStatValue(StatDefOf.EnergyShieldRechargeRate, true) * Mult) / 60f));
                        }
                    }
                    return apparel.GetStatValue(StatDefOf.EnergyShieldRechargeRate, true) / 60f;
                }
                return Props.energyShieldRechargeRate / 60f;
            }
        }

        public new virtual bool ShouldDisplay
        {
            get
            {
                return PawnOwner != null && PawnOwner.Spawned && !PawnOwner.Dead && !PawnOwner.Downed && (PawnOwner.InAggroMentalState || PawnOwner.Drafted || (PawnOwner.Faction.HostileTo(Faction.OfPlayer) && !PawnOwner.IsPrisoner) || Find.TickManager.TicksGame < this.lastKeepDisplayTick + this.KeepDisplayingTicks);
            }
        }


        public override void PostExposeData()
        {
            base.PostExposeData();
        }

        private Gizmo_CompEnergyShieldStatus _CompEnergyShieldStatus;
        public new virtual Gizmo GetGizmos()
        {
            if (Find.Selector.SingleSelectedThing == PawnOwner)
            {
                if (_CompEnergyShieldStatus == null)
                {
                    _CompEnergyShieldStatus = new Gizmo_CompEnergyShieldStatus
                    {
                        shield = this
                    };
                }
            }
            return _CompEnergyShieldStatus;
        }


        public override void CompTick()
        {
            base.CompTick();
            if (PawnOwner == null)
            {
                this.energy = 0f;
                return;
            }
            if (this.ShieldState == ShieldState.Resetting)
            {
                this.ticksToReset--;
                if (this.ticksToReset <= 0)
                {
                    this.Reset();
                }
            }
            else if (this.ShieldState == ShieldState.Active)
            {
                this.energy += this.EnergyGainPerTick;
                if (this.energy > this.EnergyMax)
                {
                    this.energy = this.EnergyMax;
                }
            }
        }

        public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            absorbed = false;
            if (this.ShieldState != ShieldState.Active)
            {
                return;
            }
            if (dinfo.Def == null)
            {
                return;
            }
            if (PawnOwner?.Map == null)
            {
                return;
            }
            if (Props != null)
            {
                if (dinfo.Def == DamageDefOf.SurgicalCut || Props.bypassingDamage.Contains(dinfo.Def))
                {
                    return;
                }
                if (dinfo.Def.externalViolenceForMechanoids && Props.brokenByEMP)
                {
                    this.energy = 0f;
                    this.Break();
                    return;
                }
                if (((dinfo.Def.isRanged || dinfo.Def.isExplosive) && Props.blocksRangedWeapons) || (!dinfo.Def.isRanged && Props.blocksMeleeWeapons))
                {
                    this.energy -= dinfo.Amount * this.Props.energyLossPerDamage;
                    if (this.energy < 0f)
                    {
                        this.Break();
                    }
                    else
                    {
                        this.AbsorbedDamage(dinfo);
                    }
                    absorbed = true;
                    return;
                }
            }
            return;
        }


        public new virtual void AbsorbedDamage(DamageInfo dinfo)
        {
            SoundDefOf.EnergyShield_AbsorbDamage.PlayOneShot(new TargetInfo(PawnOwner.Position, PawnOwner.Map, false));
            this.impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);
            Vector3 loc = PawnOwner.TrueCenter() + this.impactAngleVect.RotatedBy(180f) * 0.5f;
            float num = Mathf.Min(10f, 2f + dinfo.Amount / 10f);
            FleckMaker.Static(loc, PawnOwner.Map, FleckDefOf.ExplosionFlash, num);
            int num2 = (int)num;
            for (int i = 0; i < num2; i++)
            {
                Rand.PushState();
                AdeptusFleckMaker.ThrowDustPuff(loc, PawnOwner.Map, Rand.Range(0.8f, 1.2f));
                Rand.PopState();
            }
            this.lastAbsorbDamageTick = Find.TickManager.TicksGame;
            this.KeepDisplaying();
        }

        public new virtual void Draw()
        {
            if (this.ShieldState == ShieldState.Active && this.ShouldDisplay)
            {
                float num = Mathf.Lerp(this.Props.minDrawSize, this.Props.maxDrawSize, this.energy);
                Vector3 vector = PawnOwner.Drawer.DrawPos;
                vector.y = AltitudeLayer.MoteOverhead.AltitudeFor();
                int num2 = Find.TickManager.TicksGame - this.lastAbsorbDamageTick;
                if (num2 < 8)
                {
                    float num3 = (float)(8 - num2) / 8f * 0.05f;
                    vector += this.impactAngleVect * num3;
                    num -= num3;
                }
                float angle = 0f;
                Vector3 s = new Vector3(num * (PawnOwner.Drawer.renderer.graphics.nakedGraphic.drawSize.x * Props.drawX), 1f, num * (PawnOwner.Drawer.renderer.graphics.nakedGraphic.drawSize.y * Props.drawY));
                Matrix4x4 matrix = default(Matrix4x4);
                matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
                Graphics.DrawMesh(MeshPool.plane10, matrix, BubbleMat, 0);
            }
        }

        public override bool CompAllowVerbCast(Verb verb)
        {
            return (verb is Verb_LaunchProjectile) ? Props.allowRangedAttacks : Props.allowMeleeAttacks;
        }

        public const float MinDrawSize = 1.2f;
        public const float MaxDrawSize = 1.55f;
        public int StartingTicksToReset = 3200;
        public float EnergyOnReset = 0.2f;
        protected Material bubbleMat;


        protected virtual Graphic Graphic
        {
            get
            {
                if (graphic == null && Props.graphicData != null && !Props.graphicData.texPath.NullOrEmpty())
                {
                    graphic = Props.graphicData.Graphic;
                }
                return graphic;
            }
        }
        private Graphic graphic = null;

        protected new virtual Material BubbleMat
        {
            get
            {
                if (bubbleMat == null)
                {
                    if (Graphic != null)
                    {
                        bubbleMat = Graphic.MatSingle;
                    }
                    else
                    {
                        bubbleMat = MaterialPool.MatFrom("Other/ShieldBubble", ShaderDatabase.Transparent, Props.shieldColor);
                    }
                }

                return bubbleMat;
            }
        }


        [StaticConstructorOnStartup]
        public class Gizmo_CompEnergyShieldStatus : Gizmo
        {
            public Gizmo_CompEnergyShieldStatus()
            {
                this.order = -100f;
            }

            public override float GetWidth(float maxWidth)
            {
                return 140f;
            }

            public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
            {
                Rect overRect = new Rect(topLeft.x, topLeft.y, this.GetWidth(maxWidth), 75f);
                Find.WindowStack.ImmediateWindow(984688, overRect, WindowLayer.GameUI, delegate
                {
                    Rect rect = overRect.AtZero().ContractedBy(6f);
                    Rect rect2 = rect;
                    rect2.height = overRect.height / 2f;
                    Text.Font = GameFont.Tiny;
                    Widgets.Label(rect2, this.shield.parent.LabelCap);
                    Rect rect3 = rect;
                    rect3.yMin = overRect.height / 2f;
                    float fillPercent = this.shield.Energy / Mathf.Max(1f, this.shield.EnergyMax);
                    Widgets.FillableBar(rect3, fillPercent, Gizmo_CompEnergyShieldStatus.FullShieldBarTex, Gizmo_CompEnergyShieldStatus.EmptyShieldBarTex, false);
                    Text.Font = GameFont.Small;
                    Text.Anchor = TextAnchor.MiddleCenter;
                    Widgets.Label(rect3, (this.shield.Energy * 100f).ToString("F0") + " / " + (this.shield.EnergyMax * 100f).ToString("F0"));
                    Text.Anchor = TextAnchor.UpperLeft;
                }, true, false, 1f);
                return new GizmoResult(GizmoState.Clear);
            }

            public Comp_Shield shield;
            private static readonly Texture2D FullShieldBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.24f));
            private static readonly Texture2D EmptyShieldBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);
        }
    }
}
