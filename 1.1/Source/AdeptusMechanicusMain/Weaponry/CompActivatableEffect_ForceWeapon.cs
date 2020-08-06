using Verse;
using RimWorld;
using AdeptusMechanicus;
using AdeptusMechanicus.ExtensionMethods;
using System.Collections.Generic;

namespace AdeptusMechanicus
{
    public class CompProperties_ForceWeaponActivatableEffect : CompProperties_AlwaysActivatableEffect
    {
        public CompProperties_ForceWeaponActivatableEffect() => this.compClass = typeof(CompForceWeaponActivatableEffect);
        public bool PowerWeapon = false;
        public bool ForceEffectRequiresPsyker = true;
        public DamageDef ForceWeaponEffect = null;
        public HediffDef ForceWeaponHediff = null;
        public float ForceWeaponKillChance = 0f;
        public SoundDef ForceWeaponTriggerSound = null;

    }

    public class CompForceWeaponActivatableEffect : CompAlwaysActivatableEffect
    {

        private OgsCompActivatableEffect.CompActivatableEffect.State currentState = OgsCompActivatableEffect.CompActivatableEffect.State.Deactivated;
        public new CompProperties_ForceWeaponActivatableEffect Props => this.props as CompProperties_ForceWeaponActivatableEffect;
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

        public bool ForceEffectRequiresPsyker
        {
            get
            {
                return Props.ForceEffectRequiresPsyker;
            }
        }
        public DamageDef ForceWeaponEffect
        {
            get
            {
                return Props.ForceWeaponEffect;
            }
        }
        public HediffDef ForceWeaponHediff
        {
            get
            {
                return Props.ForceWeaponHediff;
            }
        }
        public float ForceWeaponKillChance
        {
            get
            {
                return Props.ForceWeaponKillChance;
            }
        }
        public SoundDef ForceWeaponTriggerSound
        {
            get
            {
                return Props.ForceWeaponTriggerSound;
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
        public override string CompInspectStringExtra()
        {
            string str = "Special Rules:";
            string str2 = string.Empty;
            if (ForceWeapon)
            {
                str2 = str2.NullOrEmpty() ? str + "Force Weapon" : str + ", Force Weapon";
            }
            if (Witchblade)
            {
                str2 = str2.NullOrEmpty() ? str + " Witchblade" : str + ", Witchblade";
            }
            return str2.NullOrEmpty() ? null : str + str2;
        }
        public override string GetDescriptionPart()
        {

            string str = string.Empty;
            CompEquippable c = parent.GetComp<CompEquippable>();
            if (ForceWeapon)
            {
                List<Tool> list = parent.def.tools.FindAll(x => x.capacities.Any(y => y.defName.Contains("OG_ForceWeapon_")));
                List<string> listl = new List<string>();
                list.ForEach(x => listl.Add(x.label));
                str = str + string.Format("\n Force Weapon: Attacks made by the following Tools can cause Force Attacks if the wielder is a Psyker:\n{0}", listl.ToCommaList(), Props.ForceWeaponKillChance);
            }
            if (Witchblade)
            {
                List<Tool> list = parent.def.tools.FindAll(x => x.capacities.Any(y => y.defName.Contains("OG_WitchbladeWeapon_")));
                List<string> listl = new List<string>();
                list.ForEach(x => listl.Add(x.label));
                str = str + string.Format("\n Witchblade: Attacks made by the following Tools can cause increased damage if the wielder is a Psyker:\n{0}", listl.ToCommaList());
            }

            return str;
        }
    }
}