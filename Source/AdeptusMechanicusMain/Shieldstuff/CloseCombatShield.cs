using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace AdeptusMechanicus
{
    // Token: 0x02000706 RID: 1798
    [StaticConstructorOnStartup]
    public class CloseCombatShield : Apparel
    {
        // Token: 0x170005F4 RID: 1524
        // (get) Token: 0x06002733 RID: 10035 RVA: 0x0012A8FF File Offset: 0x00128CFF
        private float EnergyMax
        {
            get
            {
                return this.GetStatValue(StatDefOf.EnergyShieldEnergyMax, true);
            }
        }

        // Token: 0x170005F5 RID: 1525
        // (get) Token: 0x06002734 RID: 10036 RVA: 0x0012A90D File Offset: 0x00128D0D
        private float EnergyGainPerTick
        {
            get
            {
                return this.GetStatValue(StatDefOf.EnergyShieldRechargeRate, true) / 60f;
            }
        }

        // Token: 0x170005F6 RID: 1526
        // (get) Token: 0x06002735 RID: 10037 RVA: 0x0012A921 File Offset: 0x00128D21
        public float Energy
        {
            get
            {
                return this.energy;
            }
        }

        // Token: 0x170005F7 RID: 1527
        // (get) Token: 0x06002736 RID: 10038 RVA: 0x0012A929 File Offset: 0x00128D29
        public ShieldState ShieldState
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

        // Token: 0x170005F8 RID: 1528
        // (get) Token: 0x06002737 RID: 10039 RVA: 0x0012A93C File Offset: 0x00128D3C
        private bool ShouldDisplay
        {
            get
            {
                Pawn wearer = base.Wearer;
                return wearer.Spawned && !wearer.Dead && !wearer.Downed && (wearer.InAggroMentalState || wearer.Drafted || (wearer.Faction.HostileTo(Faction.OfPlayer) && !wearer.IsPrisoner) || Find.TickManager.TicksGame < this.lastKeepDisplayTick + this.KeepDisplayingTicks);
            }
        }

        // Token: 0x06002738 RID: 10040 RVA: 0x0012A9D0 File Offset: 0x00128DD0
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref this.energy, "energy", 0f, false);
            Scribe_Values.Look<int>(ref this.ticksToReset, "ticksToReset", -1, false);
            Scribe_Values.Look<int>(ref this.lastKeepDisplayTick, "lastKeepDisplayTick", 0, false);
        }

        // Token: 0x06002739 RID: 10041 RVA: 0x0012AA20 File Offset: 0x00128E20
        public override IEnumerable<Gizmo> GetWornGizmos()
        {
            if (Find.Selector.SingleSelectedThing == base.Wearer)
            {
                yield return new Gizmo_CloseCombatShieldStatus
                {
                    shield = this
                };
            }
            yield break;
        }

        // Token: 0x0600273A RID: 10042 RVA: 0x0012AA43 File Offset: 0x00128E43
        public override float GetSpecialApparelScoreOffset()
        {
            return this.EnergyMax * this.ApparelScorePerEnergyMax;
        }

        // Token: 0x0600273B RID: 10043 RVA: 0x0012AA54 File Offset: 0x00128E54
        public override void Tick()
        {
            base.Tick();
            if (base.Wearer == null)
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

        // Token: 0x0600273C RID: 10044 RVA: 0x0012AAEC File Offset: 0x00128EEC
        public override bool CheckPreAbsorbDamage(DamageInfo dinfo)
        {
            if (this.ShieldState != ShieldState.Active)
            {
                return false;
            }
            if (dinfo.Def == DamageDefOf.EMP)
            {
                this.energy = 0f;
                this.Break();
                return false;
            }
            if (!dinfo.Def.isRanged && !dinfo.Def.isExplosive)
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

        // Token: 0x0600273D RID: 10045 RVA: 0x0012AB8C File Offset: 0x00128F8C
        public void KeepDisplaying()
        {
            this.lastKeepDisplayTick = Find.TickManager.TicksGame;
        }

        // Token: 0x0600273E RID: 10046 RVA: 0x0012ABA0 File Offset: 0x00128FA0
        private void AbsorbedDamage(DamageInfo dinfo)
        {
            SoundDefOf.EnergyShield_AbsorbDamage.PlayOneShot(new TargetInfo(base.Wearer.Position, base.Wearer.Map, false));
            this.impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);
            Vector3 loc = base.Wearer.TrueCenter() + this.impactAngleVect.RotatedBy(180f) * 0.5f;
            float num = Mathf.Min(10f, 2f + dinfo.Amount / 10f);
            MoteMaker.MakeStaticMote(loc, base.Wearer.Map, ThingDefOf.Mote_ExplosionFlash, num);
            int num2 = (int)num;
            for (int i = 0; i < num2; i++)
            {
                MoteMaker.ThrowDustPuff(loc, base.Wearer.Map, Rand.Range(0.8f, 1.2f));
            }
            this.lastAbsorbDamageTick = Find.TickManager.TicksGame;
            this.KeepDisplaying();
        }

        // Token: 0x0600273F RID: 10047 RVA: 0x0012AC98 File Offset: 0x00129098
        private void Break()
        {
            SoundDefOf.EnergyShield_Broken.PlayOneShot(new TargetInfo(base.Wearer.Position, base.Wearer.Map, false));
            MoteMaker.MakeStaticMote(base.Wearer.TrueCenter(), base.Wearer.Map, ThingDefOf.Mote_ExplosionFlash, 12f);
            for (int i = 0; i < 6; i++)
            {
                Vector3 loc = base.Wearer.TrueCenter() + Vector3Utility.HorizontalVectorFromAngle((float)Rand.Range(0, 360)) * Rand.Range(0.3f, 0.6f);
                MoteMaker.ThrowDustPuff(loc, base.Wearer.Map, Rand.Range(0.8f, 1.2f));
            }
            this.energy = 0f;
            this.ticksToReset = this.StartingTicksToReset;
        }

        // Token: 0x06002740 RID: 10048 RVA: 0x0012AD78 File Offset: 0x00129178
        private void Reset()
        {
            if (base.Wearer.Spawned)
            {
                SoundDefOf.EnergyShield_Reset.PlayOneShot(new TargetInfo(base.Wearer.Position, base.Wearer.Map, false));
                MoteMaker.ThrowLightningGlow(base.Wearer.TrueCenter(), base.Wearer.Map, 3f);
            }
            this.ticksToReset = -1;
            this.energy = this.EnergyOnReset;
        }

        // Token: 0x06002741 RID: 10049 RVA: 0x0012ADF4 File Offset: 0x001291F4
        public override void DrawWornExtras()
        {
            if (this.ShieldState == ShieldState.Active && this.ShouldDisplay)
            {
                float num = Mathf.Lerp(1.2f, 1.55f, this.energy);
                Vector3 vector = base.Wearer.Drawer.DrawPos;
                vector.y = AltitudeLayer.MoteOverhead.AltitudeFor();
                int num2 = Find.TickManager.TicksGame - this.lastAbsorbDamageTick;
                if (num2 < 8)
                {
                    float num3 = (float)(8 - num2) / 8f * 0.05f;
                    vector += this.impactAngleVect * num3;
                    num -= num3;
                }
                float angle = (float)Rand.Range(0, 360);
                Vector3 s = new Vector3(num, 1f, num);
                Matrix4x4 matrix = default(Matrix4x4);
                matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
                Graphics.DrawMesh(MeshPool.plane10, matrix, CloseCombatShield.BubbleMat, 0);
            }
        }

        // Token: 0x06002742 RID: 10050 RVA: 0x0012AED9 File Offset: 0x001292D9
        public override bool AllowVerbCast(IntVec3 root, Map map, LocalTargetInfo targ, Verb verb)
        {
            return true;
        }

        // Token: 0x04001621 RID: 5665
        private float energy;

        // Token: 0x04001622 RID: 5666
        private int ticksToReset = -1;

        // Token: 0x04001623 RID: 5667
        private int lastKeepDisplayTick = -9999;

        // Token: 0x04001624 RID: 5668
        private Vector3 impactAngleVect;

        // Token: 0x04001625 RID: 5669
        private int lastAbsorbDamageTick = -9999;

        // Token: 0x04001626 RID: 5670
        private const float MinDrawSize = 1.2f;

        // Token: 0x04001627 RID: 5671
        private const float MaxDrawSize = 1.55f;

        // Token: 0x04001628 RID: 5672
        private const float MaxDamagedJitterDist = 0.05f;

        // Token: 0x04001629 RID: 5673
        private const int JitterDurationTicks = 8;

        // Token: 0x0400162A RID: 5674
        private int StartingTicksToReset = 3200;

        // Token: 0x0400162B RID: 5675
        private float EnergyOnReset = 0.2f;

        // Token: 0x0400162C RID: 5676
        private float EnergyLossPerDamage = 0.033f;

        // Token: 0x0400162D RID: 5677
        private int KeepDisplayingTicks = 1000;

        // Token: 0x0400162E RID: 5678
        private float ApparelScorePerEnergyMax = 0.25f;

        // Token: 0x0400162F RID: 5679
        private static readonly Material BubbleMat = MaterialPool.MatFrom("Other/CloseCombatShieldBubble", ShaderDatabase.Transparent);
    }
}
