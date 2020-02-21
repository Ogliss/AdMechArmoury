using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace AdeptusMechanicus
{
    // Token: 0x0200024E RID: 590
    public class HediffCompProperties_PhaseShifter : HediffCompProperties
    {
        // Token: 0x06000AB1 RID: 2737 RVA: 0x0005598C File Offset: 0x00053D8C
        public HediffCompProperties_PhaseShifter()
        {
            this.compClass = typeof(HediffComp_PhaseShifter);
        }
        public float MinDistance = 5f;
        public float Chance = 0.5f;
        public float minPhasedNotifcation = 5f;
    }
    // Token: 0x02000C69 RID: 3177
    public class HediffComp_PhaseShifter : HediffComp
    {
        public new HediffCompProperties_PhaseShifter Props
        {
            get
            {
                return (HediffCompProperties_PhaseShifter)this.props;
            }
        }
        public int phasedNotifcationTick = 0;
        public List<Projectile> phasedfor = new List<Projectile>();
        TargetScanFlags targetScanFlags = TargetScanFlags.NeedLOSToPawns | TargetScanFlags.NeedReachableIfCantHitFromMyPos | TargetScanFlags.NeedThreat;
        // Token: 0x0600046F RID: 1135 RVA: 0x0002C322 File Offset: 0x0002A722
        protected virtual bool ExtraTargetValidator(Pawn pawn, Thing target)
        {
            return true;
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (phasedNotifcationTick>0)
            {
                phasedNotifcationTick--;
            }
        }

        public bool isPhasedIn
        {
            get
            {
                bool result = true;
                float chance = Props.Chance;
                BodyPartRecord partRecord = (this.parent.Part == null ? null : this.parent.Part);
                if (Pawn.Dead || Pawn.Downed)
                {
                    return true;
                }
                /*
                if (AttackTargetFinder.BestAttackTarget(Pawn, targetScanFlags, (Thing x) => this.ExtraTargetValidator(Pawn, x), 0f, Props.MinDistance, Pawn.Position, Props.MinDistance, false, true)!=null)
                {
                    return true;
                }
                */
                if (partRecord!=null)
                {
                    chance = chance * (Pawn.health.hediffSet.GetPartHealth(partRecord) / partRecord.def.GetMaxHealth(Pawn));
                }
            //    Log.Message(string.Format("Phase chance: {0}", chance));
                if (Rand.Chance(chance))
                {
                    result = false;
                }
                return result;
            }
        }

        // Token: 0x06002743 RID: 10051 RVA: 0x0012AF85 File Offset: 0x00129385
        public bool AllowVerbCast(IntVec3 root, Map map, LocalTargetInfo targ, Verb verb)
        {
            return !(verb is Verb_LaunchProjectile) || ReachabilityImmediate.CanReachImmediate(root, targ, map, PathEndMode.Touch, null);
        }
    }
}
