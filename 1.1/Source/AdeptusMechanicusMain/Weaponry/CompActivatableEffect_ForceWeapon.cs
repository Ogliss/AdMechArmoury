using Verse;
using RimWorld;
using AdeptusMechanicus;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus
{
    public class CompProperties_ForceWeaponActivatableEffect : CompProperties_AlwaysActivatableEffect
    {
        public CompProperties_ForceWeaponActivatableEffect() => this.compClass = typeof(CompForceWeaponActivatableEffect);
    }

    public class CompForceWeaponActivatableEffect : CompAlwaysActivatableEffect
    {

        private CompActivatableEffect.State currentState = CompActivatableEffect.State.Deactivated;
        
        public CompWeapon_MeleeSpecialRules specialRules
        {
            get
            {
                return this.parent.TryGetComp<CompWeapon_MeleeSpecialRules>();
            }
        }

        public override bool CanActivate()
        {
        //    Log.Message(string.Format("{0} CanActivate IsFighting: {1}, lastGivenWorkType: {2}, alwaysShowWeapon: {3}", GetPawn.LabelShortCap, GetPawn.IsFighting(), GetPawn.mindState.lastGivenWorkType, GetPawn.CurJobDef.alwaysShowWeapon));
            if (GetPawn is Pawn p && p.Spawned && p.Map != null)
            {
                return base.CanActivate() &&  (GetPawn.isPsyker() || !specialRules.ForceEffectRequiresPsyker);
            }
            return false;
        }
        

        public override void Initialize()
        {
            base.Initialize();
            if (GetPawn!=null && GetPawn.isPsyker(out int level))
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
    }
}