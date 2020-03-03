using Verse;
using RimWorld;
using AdeptusMechanicus;
using UnityEngine;

namespace AdeptusMechanicus
{
    public class CompProperties_PowerWeaponActivatableEffect : CompProperties_AlwaysActivatableEffect
    {
        public CompProperties_PowerWeaponActivatableEffect() => this.compClass = typeof(CompPowerWeaponActivatableEffect);
    }

    public class CompPowerWeaponActivatableEffect : CompAlwaysActivatableEffect
    {

        private CompActivatableEffect.State currentState = AdeptusMechanicus.CompActivatableEffect.State.Deactivated;
        /*
        public override bool CanActivate()
        {
            
            if (GetPawn is Pawn p && p.Spawned && p.Map != null)
            {
                return true;
            }
            return false;
        }
        */


        public override void Initialize()
        {
            base.Initialize();
            if (!GetPawn.IsColonist)
            {
                this.currentState = AdeptusMechanicus.CompActivatableEffect.State.Activated;
            }
        }

        public override void Activate()
        {
            base.Activate();
        }
        
        public override void Deactivate()
        {
            base.Deactivate();
        }

        public override void PostDraw()
        {
            // base.PostDraw();
        }
    }
}