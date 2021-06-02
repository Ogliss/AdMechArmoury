using AdeptusMechanicus.ExtensionMethods;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public class CompProperties_Weapon_MeleeSpecialRules : CompProperties_WargearWeapon
    {
        public CompProperties_Weapon_MeleeSpecialRules()
        {
            this.compClass = typeof(CompWeapon_MeleeSpecialRules);
        }
        
        public float RendingChance = 0.167f;
        public bool RendingWeapon = false;
        public bool PowerWeapon = false;
        public bool ForceWeapon = false;
        public bool ForceEffectRequiresPsyker = true;
        public DamageDef ForceWeaponEffect = null;
        public HediffDef ForceWeaponHediff = null;
        public float ForceWeaponKillChance = 0f;
        public SoundDef ForceWeaponTriggerSound = null;

        public ResearchProjectDef requiredResearch = null;
        public VerbProperties VerbProps;
        public List<string> UserEffectImmuneList = new List<string>();
        public bool EffectsUser = false;
        public float EffectsUserChance = 0f;
        public StatDef ResistEffectStat = null;
        public HediffDef UserEffect = null;
        public new bool GizmosOnEquip = true;
    }

    public class CompWeapon_MeleeSpecialRules : CompWargearWeapon
    {
        public new CompProperties_Weapon_MeleeSpecialRules Props => (CompProperties_Weapon_MeleeSpecialRules)props;

        public bool EffectsUser
        {
            get
            {
                return Props.EffectsUser;
            }
        }
        public float EffectsUserChance
        {
            get
            {
                return Props.EffectsUserChance;
            }
        }
        public StatDef ResistEffectStat
        {
            get
            {
                return Props.ResistEffectStat;
            }
        }
        public HediffDef UserEffect
        {
            get
            {
                return Props.UserEffect;
            }
        }
        public List<string> UserEffectImmuneList
        {
            get
            {
                return Props.UserEffectImmuneList;
            }
        }
        public bool PowerWeapon
        {
            get
            {
                return Props.PowerWeapon || parent.def.tools.Any(x => x.capacities.Any(y => y.defName.Contains("OG_PowerWeapon_")));
            }
        }
        public bool RendingWeapon
        {
            get
            {
                return Props.PowerWeapon || parent.def.tools.Any(x => x.capacities.Any(y => y.defName.Contains("OG_RendingWeapon_")));
            }
        }

        public float RendingChance
        {
            get
            {
                return Props.RendingChance;
            }
        }
        public bool ForceWeapon
        {
            get
            {
                return Props.ForceWeapon || parent.def.tools.Any(x=> x.capacities.Any(y=> y.defName.Contains("OG_ForceWeapon_")));
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

        public ResearchProjectDef requiredResearch
        {
            get
            {
                return Props.requiredResearch;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();;
        }

        /*
        public override IEnumerable<Gizmo> EquippedGizmos()
        {
            ThingWithComps owner = IsWorn ? GetWearer : parent;
            bool flag = Find.Selector.SingleSelectedThing == GetWearer;
            if (flag && GetWearer.Drafted && GetWearer.IsColonist && DualFireMode)
            {
                int num = 700000101;
                yield return new Command_Toggle
                {
                    icon = this.CommandTex,
                    defaultLabel = "AdeptusMechanicus.ToggleFireModeCommandLabel".Translate(Toggled ? PrimaryMode.defaultProjectile.label : SecondaryMode.defaultProjectile.label),
                    defaultDesc = Toggled ? PrimaryMode.defaultProjectile.description : SecondaryMode.defaultProjectile.description,
                    isActive = (() => Toggled),
                    toggleAction = delegate ()
                    {
                        Toggled = !Toggled;
                        SwitchFireMode();
                    },
                    activateSound = SoundDef.Named("Click"),
                    groupKey = num,
                    disabled = GetWearer.stances.curStance.StanceBusy,
                    disabledReason = "Busy"
                };
            }
            yield break;
        }
        */

        public override IEnumerable<Gizmo> EquippedGizmos()
        {
            ThingWithComps owner = IsWorn ? CasterPawn : parent;
            bool flag = IsWorn ? Find.Selector.SingleSelectedThing == CasterPawn : false ;
            if (flag)
            {
                string desc = string.Empty;
                string desc2 = string.Empty;
                if (ForceWeapon)
                {
                    int level = 0;
                    bool psyker = IsWorn ? CasterPawn.isPsyker(out level) : false;
                    desc = "Force Weapon";
                    if (psyker)
                    {
                        if (level == 1)
                        {
                            desc = desc + " and posses minor latent psyhic ability";
                        }
                        if (level == 2)
                        {
                            desc = desc + " and posses potent latent psyhic ability";
                        }
                        desc = desc + " \nThey may use this weapons special effects";
                    }
                    else if (!psyker && !ForceEffectRequiresPsyker)
                    {

                        desc = desc + "utilizeable by Non Psykers";
                        desc = desc + " \nThey may use this weapons special effects";
                    }
                    yield return new Command_Toggle
                    {
                        icon = parent.def.uiIcon,
                        defaultLabel = parent.def.label,
                        defaultDesc = "Equipped with a " + desc,
                        isActive = (() => psyker),
                        toggleAction = delegate ()
                        {

                        },
                        disabled = true,
                        disabledReason = !psyker && ForceEffectRequiresPsyker? "User posses no psyhic ability\nThey may use this weapon, however it will not have any special effect": null,
                    };
                }
                else if (PowerWeapon)
                {
                    desc = "Power Weapon";

                    Command_Action command_Action = new Command_Action();
                    command_Action.defaultLabel = parent.def.label;
                    command_Action.defaultDesc = "Equipped with a " + desc;
                    //    command_Action.hotKey = KeyBindingDefOf.Misc2;
                    command_Action.icon = parent.def.uiIcon;
                    command_Action.disabled = true;
                    yield return command_Action;
                }
            }
            yield break;
        }

        public override void CompTick()
        {
            base.CompTick();
            if (CasterPawn != lastWearer)
            {
                lastWearer = CasterPawn;
            }
        }
        
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
        }
        /*
        public override string CompInspectStringExtra()
        {
            string str = "Special Rules:";
            if (RendingWeapon)
            {
                str = str + "\n Rending";
            }
            if (PowerWeapon)
            {
                str = str + "\n Power Weapon";
            }
            if (ForceWeapon)
            {
                str = str + "\n Force Weapon";
            }
            return str;
        }

        public override string GetDescriptionPart()
        {

            string str = string.Empty;
            CompEquippable c = parent.GetComp<CompEquippable>();
            if (RendingWeapon)
            {
                str = str + string.Format("\n Rending: has a {0} chance to ignore all armour", RendingChance);
            }
            if (PowerWeapon)
            {
                List<Tool> list = parent.def.tools.FindAll(x => x.capacities.Any(y => y.defName.Contains("OG_PowerWeapon_")));
                List<string> listl = new List<string>();
                list.ForEach(x => listl.Add(x.label));
                str = str + string.Format("\n Power Weapon: Attacks by {0} can cause Force Attacks if the wielder is a Psyker", listl.ToCommaList());
            }
            if (ForceWeapon)
            {
                bool psyker = IsWorn ? GetWearer.isPsyker() : false;
                List<Tool> list = parent.def.tools.FindAll(x => x.capacities.Any(y => y.defName.Contains("OG_ForceWeapon_")));
                List<string> listl = new List<string>();
                list.ForEach(x => listl.Add(x.label));
                str = str + string.Format("\n Force Weapon: Attacks by {0} can cause Force Attacks if the wielder is a Psyker", listl.ToCommaList(), ForceWeaponKillChance);
            }
            return str;
        }
        */
    }
    
}
