using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Verse
{
<<<<<<< HEAD:Source/AdeptusMechanicusMain/Projectile_AbilityRRY.cs
    public class Projectile_AbilityRRY : Projectile_AbilityBase
    {
        public int TicksToImpact => ticksToImpact;
        
        protected override void Impact(Thing hitThing)
        {
            base.Impact(hitThing);
            if (hitThing != null)
            {
                var damageAmountBase = def.projectile.GetDamageAmount(1f);
                var equipmentDef = this.equipmentDef;
                var dinfo = new DamageInfo(def.projectile.damageDef, damageAmountBase, this.def.projectile.GetArmorPenetration(1f), ExactRotation.eulerAngles.y,
                    launcher, null, equipmentDef);
                hitThing.TakeDamage(dinfo);
                PostImpactEffects(hitThing);
            }
        }

        public virtual void PostImpactEffects(Thing hitThing)
        {

        }
=======
    // Token: 0x02000B9E RID: 2974
    public abstract class ResearchMod
    {
        // Token: 0x0600413D RID: 16701
        public abstract void Apply();
    }
>>>>>>> parent of c90b7c9... update computer problems:Source/AdeptusMechanicusMain/Class2.cs

    public class ResearchMod_SetResearchToZero : ResearchMod
    {
        /// <summary>
        /// Defines a specific def. Leave this null for current def.
        /// </summary>
        public ResearchProjectDef def;
        public override void Apply()
        {
            var progress = (Dictionary<ResearchProjectDef, float>)typeof(ResearchManager).GetField("progress", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Find.ResearchManager);
            if (def == null)
                def = Find.ResearchManager.currentProj;

            progress[def] = 0f;
            //For testing and spamming purposes - No real effect on standard gameplay - AND CAUSING BUGS!
            //Find.ResearchManager.currentProj = null;
        }
    }
}
