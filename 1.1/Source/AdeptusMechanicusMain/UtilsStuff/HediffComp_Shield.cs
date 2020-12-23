using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace AdeptusMechanicus
{
    public class HediffCompProperties_Shield : HediffCompProperties
    {
        public HediffCompProperties_Shield()
        {
            this.compClass = typeof(HediffComp_Shield);
        }
        public float EnergyShieldEnergyMax;
        public float EnergyShieldRechargeRate;
        public float DrawX = 1f;
        public float DrawY = 1f;
        public bool allowRanged = false;
        public bool allowMelee = true;
        public bool blockRanged = true;
        public bool blockMelee = false;
        public bool brokenByEMP = true;

    }
    // Token: 0x02000706 RID: 1798
    [StaticConstructorOnStartup]
    public class HediffComp_Shield : HediffComp
    {
        public virtual HediffCompProperties_Shield Props
        {
            get
            {
                return this.props as HediffCompProperties_Shield;
            }
        }
        // Token: 0x170005F5 RID: 1525
        // (get) Token: 0x06002734 RID: 10036 RVA: 0x0012A9AB File Offset: 0x00128DAB
        public virtual float EnergyMax
        {
            get
            {
                return Props.EnergyShieldEnergyMax;
            }
        }

        // Token: 0x170005F6 RID: 1526
        // (get) Token: 0x06002735 RID: 10037 RVA: 0x0012A9B9 File Offset: 0x00128DB9
        public virtual float EnergyGainPerTick
        {
            get
            {
                return Props.EnergyShieldRechargeRate / 60f;
            }
        }

        // Token: 0x170005F7 RID: 1527
        // (get) Token: 0x06002736 RID: 10038 RVA: 0x0012A9CD File Offset: 0x00128DCD
        public float Energy
        {
            get
            {
                return this.energy;
            }
        }

        // Token: 0x170005F8 RID: 1528
        // (get) Token: 0x06002737 RID: 10039 RVA: 0x0012A9D5 File Offset: 0x00128DD5
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

        // Token: 0x170005F9 RID: 1529
        // (get) Token: 0x06002738 RID: 10040 RVA: 0x0012A9E8 File Offset: 0x00128DE8
        public virtual bool ShouldDisplay
        {
            get
            {
                Pawn wearer = Pawn;
            //    return wearer.Spawned && !wearer.Dead && !wearer.Downed;
                return wearer.Spawned && !wearer.Dead && !wearer.Downed && (wearer.InAggroMentalState || wearer.Drafted || (wearer.Faction.HostileTo(Faction.OfPlayer) && !wearer.IsPrisoner) || Find.TickManager.TicksGame < this.lastKeepDisplayTick + this.KeepDisplayingTicks);
            }
        }

        // Token: 0x06002739 RID: 10041 RVA: 0x0012AA7C File Offset: 0x00128E7C
        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look<float>(ref this.energy, "energy", 0f, false);
            Scribe_Values.Look<int>(ref this.ticksToReset, "ticksToReset", -1, false);
            Scribe_Values.Look<int>(ref this.lastKeepDisplayTick, "lastKeepDisplayTick", 0, false);
        }
        // Token: 0x0600273A RID: 10042 RVA: 0x0012AACC File Offset: 0x00128ECC
        public virtual IEnumerable<Gizmo> GetShieldGizmos()
        {
            if (Find.Selector.SingleSelectedThing == Pawn)
            {
                yield return new Gizmo_HediffEnergyShieldStatus
                {
                    shield = this
                };
            }
            yield break;
        }


        // Token: 0x0600273C RID: 10044 RVA: 0x0012AB00 File Offset: 0x00128F00
        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
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

        // Token: 0x0600273D RID: 10045 RVA: 0x0012AB98 File Offset: 0x00128F98
        public virtual bool CheckPreAbsorbDamage(DamageInfo dinfo)
        {
            if (this.ShieldState == null || this.ShieldState != ShieldState.Active)
            {
                return false;
            }
            if (dinfo.Def == DamageDefOf.EMP && Props.brokenByEMP)
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
            return false;
        }

        // Token: 0x0600273E RID: 10046 RVA: 0x0012AC38 File Offset: 0x00129038
        public virtual void KeepDisplaying()
        {
            this.lastKeepDisplayTick = Find.TickManager.TicksGame;
        }

        // Token: 0x0600273F RID: 10047 RVA: 0x0012AC4C File Offset: 0x0012904C
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
                MoteMaker.ThrowDustPuff(loc, Pawn.Map, Rand.Range(0.8f, 1.2f));
                Rand.PopState();
            }
            this.lastAbsorbDamageTick = Find.TickManager.TicksGame;
            this.KeepDisplaying();
        }

        // Token: 0x06002740 RID: 10048 RVA: 0x0012AD44 File Offset: 0x00129144
        public virtual void Break()
        {
            SoundDefOf.EnergyShield_Broken.PlayOneShot(new TargetInfo(Pawn.Position, Pawn.Map, false));
            MoteMaker.MakeStaticMote(Pawn.TrueCenter(), Pawn.Map, ThingDefOf.Mote_ExplosionFlash, 12f);
            for (int i = 0; i < 6; i++)
            {
                Rand.PushState();
                Vector3 loc = Pawn.TrueCenter() + Vector3Utility.HorizontalVectorFromAngle((float)Rand.Range(0, 360)) * Rand.Range(0.3f, 0.6f);
                MoteMaker.ThrowDustPuff(loc, Pawn.Map, Rand.Range(0.8f, 1.2f));
                Rand.PopState();
            }
            this.energy = 0f;
            this.ticksToReset = this.StartingTicksToReset;
        }

        // Token: 0x06002741 RID: 10049 RVA: 0x0012AE24 File Offset: 0x00129224
        public virtual void Reset()
        {
            if (Pawn.Spawned)
            {
                SoundDefOf.EnergyShield_Reset.PlayOneShot(new TargetInfo(Pawn.Position, Pawn.Map, false));
                MoteMaker.ThrowLightningGlow(Pawn.TrueCenter(), Pawn.Map, 3f);
            }
            this.ticksToReset = -1;
            this.energy = this.EnergyOnReset;
        }

        // Token: 0x06002742 RID: 10050 RVA: 0x0012AEA0 File Offset: 0x001292A0
        public virtual void DrawWornExtras()
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
                Vector3 s = new Vector3(num*Props.DrawX, 1f, num * Props.DrawY);
                Matrix4x4 matrix = default(Matrix4x4);
                matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
                Graphics.DrawMesh(MeshPool.plane10, matrix, BubbleMat, 0);
            }
        }

        // Token: 0x06002743 RID: 10051 RVA: 0x0012AF85 File Offset: 0x00129385
        public virtual bool AllowVerbCast(IntVec3 root, Map map, LocalTargetInfo targ, Verb verb)
        {
            return !(verb is Verb_LaunchProjectile) || ReachabilityImmediate.CanReachImmediate(root, targ, map, PathEndMode.Touch, null);
        }

        // Token: 0x04001621 RID: 5665
        public float energy;

        // Token: 0x04001622 RID: 5666
        public int ticksToReset = -1;

        // Token: 0x04001623 RID: 5667
        public int lastKeepDisplayTick = -9999;

        // Token: 0x04001624 RID: 5668
        public Vector3 impactAngleVect;

        // Token: 0x04001625 RID: 5669
        public int lastAbsorbDamageTick = -9999;

        // Token: 0x04001626 RID: 5670
        public const float MinDrawSize = 1.2f;

        // Token: 0x04001627 RID: 5671
        public const float MaxDrawSize = 1.55f;

        // Token: 0x04001628 RID: 5672
        public const float MaxDamagedJitterDist = 0.05f;

        // Token: 0x04001629 RID: 5673
        public const int JitterDurationTicks = 8;

        // Token: 0x0400162A RID: 5674
        public int StartingTicksToReset = 3200;

        // Token: 0x0400162B RID: 5675
        public float EnergyOnReset = 0.2f;

        // Token: 0x0400162C RID: 5676
        public float EnergyLossPerDamage = 0.033f;

        // Token: 0x0400162D RID: 5677
        public int KeepDisplayingTicks = 1000;

        // Token: 0x0400162E RID: 5678
        public float ApparelScorePerEnergyMax = 0.25f;

        // Token: 0x0400162F RID: 5679
        public static readonly Material BubbleMat = MaterialPool.MatFrom("Other/ShieldBubble", ShaderDatabase.Transparent);

        // Token: 0x02000704 RID: 1796
        [StaticConstructorOnStartup]
        public class Gizmo_HediffEnergyShieldStatus : Gizmo
        {
            // Token: 0x0600272F RID: 10031 RVA: 0x0012A758 File Offset: 0x00128B58
            public Gizmo_HediffEnergyShieldStatus()
            {
                this.order = -100f;
            }

            // Token: 0x06002730 RID: 10032 RVA: 0x0012A76B File Offset: 0x00128B6B
            public override float GetWidth(float maxWidth)
            {
                return 140f;
            }

            // Token: 0x06002731 RID: 10033 RVA: 0x0012A774 File Offset: 0x00128B74
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
                    Widgets.FillableBar(rect3, fillPercent, Gizmo_HediffEnergyShieldStatus.FullShieldBarTex, Gizmo_HediffEnergyShieldStatus.EmptyShieldBarTex, false);
                    Text.Font = GameFont.Small;
                    Text.Anchor = TextAnchor.MiddleCenter;
                    Widgets.Label(rect3, (this.shield.Energy * 100f).ToString("F0") + " / " + (this.shield.EnergyMax * 100f).ToString("F0"));
                    Text.Anchor = TextAnchor.UpperLeft;
                }, true, false, 1f);
                return new GizmoResult(GizmoState.Clear);
            }

            // Token: 0x0400161B RID: 5659
            public HediffComp_Shield shield;

            // Token: 0x0400161C RID: 5660
            private static readonly Texture2D FullShieldBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.24f));

            // Token: 0x0400161D RID: 5661
            private static readonly Texture2D EmptyShieldBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);
        }
    }
}
