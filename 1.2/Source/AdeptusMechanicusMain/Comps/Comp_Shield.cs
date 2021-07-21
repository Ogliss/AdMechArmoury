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

    public class CompProperties_Shield : CompProperties
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
        public bool allowRanged = false;
        public bool allowMelee = true;
        public bool blockRanged = true;
        public bool blockMelee = false;
        public bool brokenByEMP = true;

        public Color shieldColor = Color.white;
        public GraphicData graphicData = null;
        public Vector3 offset = new Vector3();
    }

    [StaticConstructorOnStartup]
    public class Comp_Shield : ThingComp
    {
        public virtual CompProperties_Shield Props
        {
            get
            {
                return this.props as CompProperties_Shield;
            }
        }

        public virtual float EnergyMax
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

        public virtual float EnergyGainPerTick
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

        public float Energy
        {
            get
            {
                return this.energy;
            }
        }

        public virtual ShieldState ShieldState
        {
            get
            {
                if (this.ticksToReset > 0)
                {
                    return ShieldState.Resetting;
                }
                return ShieldState.Active;
            }
        }

        public virtual bool ShouldDisplay
        {
            get
            {
                return Pawn != null && Pawn.Spawned && !Pawn.Dead && !Pawn.Downed && (Pawn.InAggroMentalState || Pawn.Drafted || (Pawn.Faction.HostileTo(Faction.OfPlayer) && !Pawn.IsPrisoner) || Find.TickManager.TicksGame < this.lastKeepDisplayTick + this.KeepDisplayingTicks);
            }
        }

        public virtual Pawn Pawn
        {
            get
            {
                if (this.parent is Apparel apparel)
                {
                    return apparel.Wearer ?? null;
                }
                return this.parent as Pawn;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<float>(ref this.energy, "energy", 0f, false);
            Scribe_Values.Look<int>(ref this.ticksToReset, "ticksToReset", -1, false);
            Scribe_Values.Look<int>(ref this.lastKeepDisplayTick, "lastKeepDisplayTick", 0, false);
        }

        private Gizmo_CompEnergyShieldStatus _CompEnergyShieldStatus;
        public virtual Gizmo GetShieldGizmos()
        {
            if (Find.Selector.SingleSelectedThing == Pawn)
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
            if (Pawn == null)
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

        public virtual bool CheckPreAbsorbDamage(DamageInfo dinfo)
        {
            if (this.ShieldState != ShieldState.Active)
            {
                return false;
            }
            if (dinfo.Def == null)
            {
                return false;
            }
            if (Pawn.Map == null)
            {
                return false;
            }
            if (Props != null)
            {
                if (dinfo.Def.externalViolenceForMechanoids && Props.brokenByEMP)
                {
                    this.energy = 0f;
                    this.Break();
                    return false;
                }
                if ((dinfo.Def.isRanged && Props.blockRanged) || (!dinfo.Def.isRanged && Props.blockMelee))
                {
                    this.energy -= dinfo.Amount * this.EnergyLossPerDamage;
                    if (this.energy < 0f)
                    {
                        this.Break();
                    }
                    else
                    {
                        this.AbsorbedDamage(dinfo);
                    }
                    return true;
                }
            }
            return false;
        }

        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.PostPostApplyDamage(dinfo, totalDamageDealt);
        }

        public virtual void KeepDisplaying()
        {
            this.lastKeepDisplayTick = Find.TickManager.TicksGame;
        }

        public virtual void AbsorbedDamage(DamageInfo dinfo)
        {
            SoundDefOf.EnergyShield_AbsorbDamage.PlayOneShot(new TargetInfo(Pawn.Position, Pawn.Map, false));
            this.impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);
            Vector3 loc = Pawn.TrueCenter() + this.impactAngleVect.RotatedBy(180f) * 0.5f;
            float num = Mathf.Min(10f, 2f + dinfo.Amount / 10f);
            MoteMaker.MakeStaticMote(loc, Pawn.Map, ThingDefOf.Mote_ExplosionFlash, num);
            int num2 = (int)num;
            for (int i = 0; i < num2; i++)
            {
                Rand.PushState();
                AdeptusMoteMaker.ThrowDustPuff(loc, Pawn.Map, Rand.Range(0.8f, 1.2f));
                Rand.PopState();
            }
            this.lastAbsorbDamageTick = Find.TickManager.TicksGame;
            this.KeepDisplaying();
        }

        public virtual void Break()
        {
            SoundDefOf.EnergyShield_Broken.PlayOneShot(new TargetInfo(Pawn.Position, Pawn.Map, false));
            MoteMaker.MakeStaticMote(Pawn.TrueCenter(), Pawn.Map, ThingDefOf.Mote_ExplosionFlash, 12);
            for (int i = 0; i < 6; i++)
            {
                Rand.PushState();
                Vector3 loc = Pawn.TrueCenter() + Vector3Utility.HorizontalVectorFromAngle((float)Rand.Range(0, 360)) * Rand.Range(0.3f, 0.6f);
                AdeptusMoteMaker.ThrowDustPuff(loc, Pawn.Map, Rand.Range(0.8f, 1.2f));
                Rand.PopState();
            }
            this.energy = 0f;
            this.ticksToReset = this.StartingTicksToReset;
        }

        public virtual void Reset()
        {
            if (Pawn.Spawned)
            {
                SoundDefOf.EnergyShield_Reset.PlayOneShot(new TargetInfo(Pawn.Position, Pawn.Map, false));
                AdeptusMoteMaker.ThrowLightningGlow(Pawn.TrueCenter(), Pawn.Map, 3f);
            }
            this.ticksToReset = -1;
            this.energy = this.EnergyOnReset;
        }

        public virtual void DrawShield()
        {
            if (this.ShieldState == ShieldState.Active && this.ShouldDisplay)
            {
                float num = Mathf.Lerp(1.2f, 1.55f, this.energy);
                Vector3 vector = Pawn.Drawer.DrawPos;
                vector.y = AltitudeLayer.MoteOverhead.AltitudeFor();
                int num2 = Find.TickManager.TicksGame - this.lastAbsorbDamageTick;
                if (num2 < 8)
                {
                    float num3 = (float)(8 - num2) / 8f * 0.05f;
                    vector += this.impactAngleVect * num3;
                    num -= num3;
                }
                float angle = 0;
                Vector3 s = new Vector3(num * (Pawn.Drawer.renderer.graphics.nakedGraphic.drawSize.x * Props.drawX), 1f, num * (Pawn.Drawer.renderer.graphics.nakedGraphic.drawSize.y * Props.drawY));
                Matrix4x4 matrix = default(Matrix4x4);
                matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
                Graphics.DrawMesh(MeshPool.plane10, matrix, BubbleMat, 0);
            }
        }

        public virtual bool AllowVerbCast(Verb verb)
        {
            return (verb is Verb_LaunchProjectile) ? Props.allowRanged : Props.allowMelee;
        }

        public float energy;
        public int ticksToReset = -1;
        public int lastKeepDisplayTick = -9999;
        public Vector3 impactAngleVect;
        public int lastAbsorbDamageTick = -9999;
        public const float MinDrawSize = 1.2f;
        public const float MaxDrawSize = 1.55f;
        public const float MaxDamagedJitterDist = 0.05f;
        public const int JitterDurationTicks = 8;
        public int StartingTicksToReset = 3200;
        public float EnergyOnReset = 0.2f;
        public float EnergyLossPerDamage = 0.033f;
        public int KeepDisplayingTicks = 1000;
        public float ApparelScorePerEnergyMax = 0.25f;
        protected Material bubbleMat;


        protected virtual Graphic Graphic
        {
            get
            {
                if (graphic is null && Props.graphicData != null && !Props.graphicData.texPath.NullOrEmpty())
                {
                    graphic = Props.graphicData.Graphic;
                }
                return graphic;
            }
        }
        private Graphic graphic = null;

        protected virtual Material BubbleMat
        {
            get
            {
                if (bubbleMat is null)
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

            public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth)
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
