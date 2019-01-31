using System.Linq;
using CompSlotLoadable;
using Verse;
using RimWorld;
using CompActivatableEffect;

namespace AdeptusMechanicus
{
    public class CompProperties_PowerWeaponActivatableEffect : CompProperties_ActivatableEffect
    {
        public CompProperties_PowerWeaponActivatableEffect() => this.compClass = typeof(CompPowerWeaponActivatableEffect);
    }

    public class CompPowerWeaponActivatableEffect : CompActivatableEffect.CompActivatableEffect
    {
        public override bool CanActivate()
        {
            Pawn p = this.parent.holdingOwner.Owner.ParentHolder as Pawn;
        //    Log.Message(string.Format("{0}", p.Drafted));
            if (p.Drafted)
        //    if (this.parent.SpawnedOrAnyParentSpawned && (p.Drafted || (p.CurJob != null && p.CurJob.def.alwaysShowWeapon) || (p.mindState.duty != null && p.mindState.duty.def.alwaysShowWeapon)))
            {
                return true;
            }
            return false;
        }
        
        public override void Activate()
        {
            base.Activate();
            MakeGlower();
        }

        public void MakeGlower()
        {
            PowerGlow sb;
            Pawn p = this.parent.holdingOwner.Owner.ParentHolder as Pawn;
            p.AllComps.Add(sb = new PowerGlow()
            {
                parent = p,
                props = new CompProperties_Glower()
                {
                    compClass = typeof(PowerGlow),
                    glowRadius = 5f,
                    glowColor = new ColorInt(0, 152, 247, 1),
                    overlightRadius = 5f
                }
            });
            sb.PostSpawnSetup(false);
        }

        public override void Deactivate()
        {
            base.Deactivate();
            if (this.parent is ThingWithComps t && t.holdingOwner is ThingOwner o &&
                o.Owner.ParentHolder is Pawn p && p.GetComp<PowerGlow>() is PowerGlow sb &&
                t?.MapHeld?.glowGrid != null)
            {
                try
                {
                    this.parent.MapHeld.glowGrid.DeRegisterGlower(sb);
                    sb.PostDestroy(DestroyMode.Vanish, this.parent.Map);
                    p.AllComps.Remove(sb);
                }
                catch { }
            }
        }
    }
}