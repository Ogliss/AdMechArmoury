using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace AdeptusMechanicus
{
    public class CompProperties_InvunerableSaveOG : CompProperties
    {
        public CompProperties_InvunerableSaveOG()
        {
            this.compClass = typeof(CompInvunerableSaveOG);
        }
        public float InvunerableSaveChance = 0f;
        public List<DamageDef> BypassingDamageDefs = new List<DamageDef>();
        public string ShieldTexPath;

    }

    // Token: 0x02000002 RID: 2
    public class CompInvunerableSaveOG : ThingComp
    {
        public CompProperties_InvunerableSaveOG Props => (CompProperties_InvunerableSaveOG)props;
        public Pawn pawn;

        public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            //Log.Message(string.Format("Bypassing: {0}", !Props.BypassingDamageDefs.Any(def => dinfo.Def == def)));
            if (dinfo.Def !=null && base.parent is Pawn pawn && pawn != null && Rand.Chance(Props.InvunerableSaveChance) && !Props.BypassingDamageDefs.Any(def => dinfo.Def == def ))
            {
                absorbed = true;
                SoundDefOf.EnergyShield_AbsorbDamage.PlayOneShot(new TargetInfo(base.parent.Position, base.parent.Map, false));
                Vector3 impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);
                Vector3 loc = base.parent.TrueCenter() + impactAngleVect.RotatedBy(180f) * 0.5f;
                float num = Mathf.Min(10f, 2f + (float)dinfo.Amount / 10f);
                MoteMaker.MakeStaticMote(loc, base.parent.Map, ThingDefOf.Mote_ExplosionFlash, num);
                int num2 = (int)num;
                for (int i = 0; i < num2; i++)
                {
                    MoteMaker.ThrowDustPuff(loc, base.parent.Map, Rand.Range(0.8f, 1.2f));


                    float num3 = Mathf.Lerp(1.2f, 1.55f, 2f);
                    Vector3 vector = pawn.Drawer.DrawPos;
                    vector.y = Altitudes.AltitudeFor(AltitudeLayer.MoteOverhead);
                    int num4 = Find.TickManager.TicksGame - this.lastAbsorbDamageTick;
                    if (num4 < 8)
                    {
                        float num5 = (float)(8 - num4) / 8f * 0.05f;
                        vector += impactAngleVect * num5;
                        num3 -= num5;
                    }
                    float angle = (float)Rand.Range(0, 360);
                    Vector3 s = new Vector3(num3, 1f, num3);
                    Matrix4x4 matrix = default(Matrix4x4);
                    matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
                    Graphics.DrawMesh(MeshPool.plane10, matrix, CompInvunerableSaveOGStatic.BubbleMat, 0);
                }
                this.lastAbsorbDamageTick = Find.TickManager.TicksGame;
            }
            else base.PostPreApplyDamage(dinfo, out absorbed);
        }

        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.PostPostApplyDamage(dinfo, totalDamageDealt);
        }

        private int lastAbsorbDamageTick = -9999;
    }
    [StaticConstructorOnStartup]
    public static class CompInvunerableSaveOGStatic
    {
        // Token: 0x0400157E RID: 5502
        public static readonly Material BubbleMat = MaterialPool.MatFrom("Other/InvSaveBubble", ShaderDatabase.Transparent);
    }
}
