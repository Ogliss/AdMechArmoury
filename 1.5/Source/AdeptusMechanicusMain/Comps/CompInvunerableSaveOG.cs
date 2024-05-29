using System;
using System.Collections.Generic;
using AlienRace;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace AdeptusMechanicus
{
    public class CompProperties_InvunerableSave : CompProperties
    {
        public CompProperties_InvunerableSave()
        {
            this.compClass = typeof(CompInvunerableSave);
        }
        public float InvunerableSaveChance = 0f;
        public List<DamageDef> BypassingDamageDefs = new List<DamageDef>();
        public List<DamageArmorCategoryDef> BypassingArmorCategoryDamageDefs = new List<DamageArmorCategoryDef>();
        public string ShieldTexPath;
        public int keepDisplayingTicks = 60;
    }

    public class CompInvunerableSave : ThingComp
    {
        public CompProperties_InvunerableSave Props => (CompProperties_InvunerableSave)props;
        public bool MitiagatesDamage(DamageDef def)
        {
            if (def != null)
            {
                if (!Props.BypassingDamageDefs.NullOrEmpty())
                {
                    for (int i = 0; i < Props.BypassingDamageDefs.Count; i++)
                    {
                        if (def == Props.BypassingDamageDefs[i]) return false;
                    }
                }
                if (!Props.BypassingArmorCategoryDamageDefs.NullOrEmpty())
                {
                    for (int i = 0; i < Props.BypassingArmorCategoryDamageDefs.Count; i++)
                    {
                        if (def.armorCategory == Props.BypassingArmorCategoryDamageDefs[i]) return false;
                    }
                }
            }
            return true;
        }


        public override void PostPreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
        {
            //Log.Message(string.Format("Bypassing: {0}", !Props.BypassingDamageDefs.Any(def => dinfo.Def == def)));

            Rand.PushState();
            bool invSave = Rand.Chance(Props.InvunerableSaveChance);
            Rand.PopState();
            DamageDef damageDef = dinfo.Def;
            if (damageDef != null && base.parent is Pawn pawn && pawn != null && invSave && !Props.BypassingDamageDefs.Any(def => damageDef == def ))
            {
                absorbed = true;
                AbsorbedDamage(dinfo);
            }
            else base.PostPreApplyDamage(ref dinfo, out absorbed);
        }

        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.PostPostApplyDamage(dinfo, totalDamageDealt);
        }

        protected Pawn PawnOwner
        {
            get
            {
                Apparel apparel;
                if ((apparel = (this.parent as Apparel)) != null)
                {
                    return apparel.Wearer;
                }
                Pawn result;
                if ((result = (this.parent as Pawn)) != null)
                {
                    return result;
                }
                return null;
            }
        }
        protected bool ShouldDisplay
        {
            get
            {
                Pawn pawnOwner = this.PawnOwner;
                return pawnOwner.Spawned && !pawnOwner.Dead && !pawnOwner.Downed && Find.TickManager.TicksGame < this.lastKeepDisplayTick + this.Props.keepDisplayingTicks;
            }
        }

        public bool IsApparel
        {
            get
            {
                return this.parent is Apparel;
            }
        }

        private bool IsBuiltIn
        {
            get
            {
                return !this.IsApparel;
            }
        }
        
        private void AbsorbedDamage(DamageInfo dinfo)
        {
            SoundDefOf.EnergyShield_AbsorbDamage.PlayOneShot(new TargetInfo(this.PawnOwner.Position, this.PawnOwner.Map, false));
            this.impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);
            Vector3 loc = this.PawnOwner.TrueCenter() + this.impactAngleVect.RotatedBy(180f) * 0.5f;
            float num = Mathf.Min(10f, 2f + dinfo.Amount / 10f);
            FleckMaker.Static(loc, this.PawnOwner.Map, FleckDefOf.ExplosionFlash, num);
            int num2 = (int)num;
            for (int i = 0; i < num2; i++)
            {
                FleckMaker.ThrowDustPuff(loc, this.PawnOwner.Map, Rand.Range(0.8f, 1.2f));
            }
            this.lastAbsorbDamageTick = Find.TickManager.TicksGame;
            this.KeepDisplaying();
        }
        public void KeepDisplaying()
        {
            this.lastKeepDisplayTick = Find.TickManager.TicksGame;
        }

        public override void CompDrawWornExtras()
        {
            base.CompDrawWornExtras();
            if (this.IsApparel)
            {
                this.Draw();
            }
        }

        public override void PostDraw()
        {
            base.PostDraw();
            if (this.IsBuiltIn)
            {
                this.Draw();
            }
        }

        private void Draw()
        {
            if (this.ShouldDisplay)
            {
                float num = (this.PawnOwner.DrawSize.y + this.PawnOwner.DrawSize.x);
                Vector3 vector = this.PawnOwner.Drawer.DrawPos;
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
                Graphics.DrawMesh(MeshPool.plane10, matrix, CompInvunerableSaveOGStatic.BubbleMat, 0);
            }
        }
        protected int lastKeepDisplayTick = -9999;
        private int lastAbsorbDamageTick = -9999;
        private Vector3 impactAngleVect;
    }
    [StaticConstructorOnStartup]
    public static class CompInvunerableSaveOGStatic
    {
        // Token: 0x0400157E RID: 5502
        public static readonly Material BubbleMat = MaterialPool.MatFrom("Other/InvSaveBubble", ShaderDatabase.Transparent);
    }
}
