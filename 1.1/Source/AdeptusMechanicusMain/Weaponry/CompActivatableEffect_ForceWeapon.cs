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

        private OgsCompActivatableEffect.CompActivatableEffect.State currentState = OgsCompActivatableEffect.CompActivatableEffect.State.Deactivated;
        
        public CompWeapon_MeleeSpecialRules specialRules
        {
            get
            {
                CompWeapon_MeleeSpecialRules _MeleeSpecialRules = this.parent.TryGetComp<CompWeapon_MeleeSpecialRules>();
                if (_MeleeSpecialRules!=null)
                {
                    return _MeleeSpecialRules;
                }
                return null;
            }
        }

        public CompEquippable Equippable
        {
            get
            {
                return this.parent.TryGetComp<CompEquippable>() ?? null;
            }
        }

        public override bool CanActivate()
        {
            if (Equippable == null)
            {
                return false;
            }
            /*
            if (specialRules == null)
            {
                Log.Warning(parent.LabelCap+ " is a Force weapons without a specialRules comp");
                return false;
            }
            */
            if (Equippable.PrimaryVerb == null)
            {
                return false;
            }
            if (Equippable.PrimaryVerb.CasterPawn == null)
            {
                return false;
            }
            Pawn p = Equippable.PrimaryVerb.CasterPawn;
            //    Log.Message(string.Format("{0} CanActivate IsFighting: {1}, lastGivenWorkType: {2}, alwaysShowWeapon: {3}", GetPawn.LabelShortCap, GetPawn.IsFighting(), GetPawn.mindState.lastGivenWorkType, GetPawn.CurJobDef.alwaysShowWeapon));
            if (!p.Spawned || p.Map == null)
            {
                return false;
            }
            Log.Message("CanActivate 5");
            if (specialRules != null)
            {
                if (specialRules.ForceEffectRequiresPsyker)
                {
                    Log.Message("CanActivate 5 1");
                    if (!p.isPsyker())
                    {
                        return false;
                    }
                }
            }
            Log.Message("CanActivate 6");
            return base.CanActivate();
        }
        

        public override void Initialize()
        {
            base.Initialize();
            if (GetPawn!=null && GetPawn.isPsyker(out int level))
            {
                this.currentState = OgsCompActivatableEffect.CompActivatableEffect.State.Activated;
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