using System;
using System.Text;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    // RRYautja.HediffCompProperties_DamageOverTime
    public class HediffCompProperties_DamageOverTime : HediffCompProperties
    {
        // Token: 0x0600012A RID: 298 RVA: 0x0000AF8C File Offset: 0x0000918C
        public HediffCompProperties_DamageOverTime()
        {
            this.compClass = typeof(HediffComp_DamageOverTime);
        }

        // Token: 0x040000CD RID: 205
        public DamageDef cycleDamage = DamageDefOf.Bite;

        // Token: 0x040000CE RID: 206
        public int cycleDamageAmt = 1;

        // Token: 0x040000CF RID: 207
        public int cycleInTicks = 300;

        // Token: 0x040000CE RID: 206
        public int maxCycleAmt = 0;

        // Token: 0x040000D0 RID: 208
        public float spreadChance = 0f;

        // Token: 0x040000D1 RID: 209
        public float armorPenetration = 0f;

        // Token: 0x040000D1 RID: 209
        public float repeatChance = 1f;
    }

    // RRYautja.HediffCompDamageOverTime
    public class HediffComp_DamageOverTime : HediffComp
    {

        // Token: 0x06000128 RID: 296 RVA: 0x0000AF5E File Offset: 0x0000915E
        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look<int>(ref this.ticksUntilDamage, "ticksUntilDamage", -1, true);
            Scribe_Values.Look<int>(ref this.timesRepeated, "timesRepeated", 0, true);
        }

        // Token: 0x040000CC RID: 204
        private int ticksUntilDamage = -1;
        public int timesRepeated = 0;

        public int MaxRepeats
        {
            get { return Props.maxCycleAmt; }
        }

        public float repeatChance
        {
            get { return Props.repeatChance; }
        }
        // Token: 0x17000027 RID: 39
        // (get) Token: 0x06000123 RID: 291 RVA: 0x0000AE54 File Offset: 0x00009054
        public HediffCompProperties_DamageOverTime Props
        {
            get
            {
                return this.props as HediffCompProperties_DamageOverTime;
            }
        }

        // Token: 0x06000124 RID: 292 RVA: 0x0000AE64 File Offset: 0x00009064
        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            bool flag = this.ticksUntilDamage < 0;
            bool flag2 = timesRepeated < MaxRepeats || MaxRepeats == -1;
            if (flag && flag2 && Rand.Chance(repeatChance))
            {
                timesRepeated++;
                this.ticksUntilDamage = this.Props.cycleInTicks;
                this.MakeDamage();
            }
            if (timesRepeated >= MaxRepeats && MaxRepeats != -1)
            {
                this.parent.comps.Remove(this.parent.TryGetComp<HediffComp_DamageOverTime>());
            }
            this.ticksUntilDamage--;
        }

        // Token: 0x06000125 RID: 293 RVA: 0x0000AEB0 File Offset: 0x000090B0
        public DamageInfo GetDamageInfo()
        {
            return new DamageInfo(this.Props.cycleDamage, (float)this.Props.cycleDamageAmt, this.Props.armorPenetration, -1f, this.parent.pawn, this.parent.Part, null, 0, null);
        }

        // Token: 0x06000126 RID: 294 RVA: 0x0000AF07 File Offset: 0x00009107
        public virtual void MakeDamage()
        {
            base.Pawn.TakeDamage(this.GetDamageInfo());
        }

        // Token: 0x06000127 RID: 295 RVA: 0x0000AF1C File Offset: 0x0000911C
        public override string CompDebugString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(base.CompDebugString());
            stringBuilder.AppendLine(this.ticksUntilDamage.ToString());
            return GenText.TrimEndNewlines(stringBuilder.ToString());
        }
    }
}
