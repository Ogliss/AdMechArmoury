using AdeptusMechanicus.ExtensionMethods;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Verse;
using Verse.AI;

namespace AdeptusMechanicus
{
    public class CompProperties_Weapon_GunSpecialRules : CompProperties_WargearWeapon
    {
        public CompProperties_Weapon_GunSpecialRules()
        {
            this.compClass = typeof(CompWeapon_GunSpecialRules);
        }
        public bool DualFireMode = false;
        public bool HeavyWeapon = false;
        public float HeavyWeaponSetupTime = 4f;
        public List<GunVerbEntry> VerbEntries;
        public bool TyranidBurstBodySize = false;
    }

    public class CompWeapon_GunSpecialRules : CompWargearWeapon
    {
        public CompProperties_Weapon_GunSpecialRules Props => (CompProperties_Weapon_GunSpecialRules)props;
        public CompEquippable compEquippable => parent.GetComp<CompEquippable>();
        public Pawn CasterPawn => compEquippable.PrimaryVerb.CasterPawn;
        public bool DualFireMode => Props.DualFireMode;
        public bool HeavyWeapon => Props.HeavyWeapon;
        public bool TyranidBurstBodySize => Props.TyranidBurstBodySize;
        public bool TwinLinked => Toggled ? Props.VerbEntries[1].TwinLinked : Props.VerbEntries[0].TwinLinked;
        public bool RapidFire => Toggled ? Props.VerbEntries[1].RapidFire : Props.VerbEntries[0].RapidFire;
        public bool GetsHot => Toggled ? Props.VerbEntries[1].GetsHot : Props.VerbEntries[0].GetsHot;
        public bool HotDamageWeapon => Toggled ? Props.VerbEntries[1].HotDamageWeapon : Props.VerbEntries[0].HotDamageWeapon;
        public bool GetsHotCrit => Toggled? Props.VerbEntries[1].GetsHotCrit : Props.VerbEntries[0].GetsHotCrit;
        public bool GetsHotCritExplosion => Toggled ? Props.VerbEntries[1].GetsHotCritExplosion : Props.VerbEntries[0].GetsHotCritExplosion;
        public bool Jams => Toggled ? Props.VerbEntries[1].Jams : Props.VerbEntries[0].Jams;
        public bool JamsDamageWeapon => Toggled ? Props.VerbEntries[1].JamsDamageWeapon : Props.VerbEntries[0].JamsDamageWeapon;
        public bool Multishot => Toggled ? Props.VerbEntries[1].Multishot : Props.VerbEntries[0].Multishot;
        public bool EffectsUser => Toggled ? Props.VerbEntries[1].EffectsUser : Props.VerbEntries[0].EffectsUser;
        public bool Rending => Toggled ? Props.VerbEntries[1].Rending : Props.VerbEntries[0].Rending;
        public Reliability reliability => Toggled ? Props.VerbEntries[1].reliability : Props.VerbEntries[0].reliability;
        public float HeavyWeaponSetupTime => Props.HeavyWeaponSetupTime;
        public float HotDamage => Toggled ? Props.VerbEntries[1].HotDamage : Props.VerbEntries[0].HotDamage;
        public float GetsHotCritChance => Toggled ? Props.VerbEntries[1].GetsHotCritChance : Props.VerbEntries[0].GetsHotCritChance;
        public float GetsHotCritExplosionChance => Toggled ? Props.VerbEntries[1].GetsHotCritExplosionChance : Props.VerbEntries[0].GetsHotCritExplosionChance;
        public float JamDamage => Toggled ? Props.VerbEntries[1].JamDamage : Props.VerbEntries[0].JamDamage;
        public float EffectsUserChance => Toggled ? Props.VerbEntries[1].EffectsUserChance : Props.VerbEntries[0].EffectsUserChance;
        public float RendingChance => Toggled ? Props.VerbEntries[1].RendingChance : Props.VerbEntries[0].RendingChance;
        public StatDef ResistEffectStat => Toggled ? Props.VerbEntries[1].ResistEffectStat : Props.VerbEntries[0].ResistEffectStat;
        public HediffDef UserEffect=> Toggled ? Props.VerbEntries[1].UserEffect : Props.VerbEntries[0].UserEffect;
        public List<string> UserEffectImmuneList => Toggled ? Props.VerbEntries[1].UserEffectImmuneList : Props.VerbEntries[0].UserEffectImmuneList;
        public ResearchProjectDef requiredResearch => Toggled ? Props.VerbEntries[1].requiredResearch : Props.VerbEntries[0].requiredResearch;
        public int ScattershotCount => Toggled ? Props.VerbEntries[1].ScattershotCount : Props.VerbEntries[0].ScattershotCount;
        public bool MeltaWeapon => Toggled ? Props.VerbEntries[1].VerbProps.defaultProjectile.projectile.Melta() : this.parent.def.Verbs[0].defaultProjectile.projectile.Melta();
        public bool VolkiteWeapon => Toggled ? Props.VerbEntries[1].VerbProps.defaultProjectile.projectile.Volkite() : this.parent.def.Verbs[0].defaultProjectile.projectile.Volkite();
        public bool GaussWeapon => Toggled ? Props.VerbEntries[1].VerbProps.defaultProjectile.projectile.Gauss() : this.parent.def.Verbs[0].defaultProjectile.projectile.Gauss();
        public bool HaywireWeapon => Toggled ? Props.VerbEntries[1].VerbProps.defaultProjectile.projectile.Haywire() : this.parent.def.Verbs[0].defaultProjectile.projectile.Haywire();
        public bool TeslaWeapon => Toggled ? Props.VerbEntries[1].VerbProps.defaultProjectile.projectile.Tesla() : this.parent.def.Verbs[0].defaultProjectile.projectile.Tesla();
        public bool ConversionWeapon => Toggled ? Props.VerbEntries[1].VerbProps.defaultProjectile.projectile.Conversion() : this.parent.def.Verbs[0].defaultProjectile.projectile.Conversion();
        public int LastMovedTick
        {
            get
            {

                if (CasterPawn == null)
                {
                    return 0;
                }

                traverse = Traverse.Create(CasterPawn.pather);
                lastmovedTick = (int)lastMovedTick.GetValue(CasterPawn.pather);

                return lastmovedTick;
            }
        }
        public int ticksHere
        {
            get
            {
                return Find.TickManager.TicksGame - LastMovedTick;
            }
        }
        Traverse traverse;
        public int lastmovedTick;
        public static FieldInfo lastMovedTick = typeof(Pawn_PathFollower).GetField("lastMovedTick", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
        public bool Toggled = false;
        public int CurrentMode = 0;
        public int variablegraphic = -1;
        public float OriginalwarmupTime = 0f;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref Toggled, "ToggledFireMode", false);
            Scribe_Values.Look<int>(ref CurrentMode, "CurrentMode", 0, true);
            Scribe_Values.Look<VerbProperties>(ref primaryMode, "primaryMode");
            Scribe_Values.Look<int>(ref variablegraphic, "variablegraphic");
            Scribe_Values.Look<float>(ref this.OriginalwarmupTime, "OriginalCooldown", 0f);
            

        }

        private VerbProperties primaryMode = null;
        protected virtual VerbProperties PrimaryMode
        {
            get
            {
                /*
                if (parent != null && parent is ThingWithComps)
                {
                    if (primaryMode!=null)
                    {
                        return primaryMode;
                    }
                    if (Props.VerbEntries[0].VerbProps!=null)
                    {
                        primaryMode = Props.VerbEntries[0].VerbProps;
                        return primaryMode;
                    }
                    primaryMode = parent.def.Verbs[0];
                    return primaryMode;
                }
                else
                {
                    return null;
                }
                */
                return parent.def.Verbs[0];
            }
        }

        protected virtual VerbProperties SecondaryMode
        {
            get
            {
                if (parent != null && parent is ThingWithComps)
                {
                    return Props.VerbEntries[1].VerbProps;
                }
                else
                {
                    return null;
                }
            }
        }

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
                    defaultLabel = "OG_ToggleFireModeCommandLabel".Translate(Toggled ? PrimaryMode.label : SecondaryMode.label),
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
                    disabledReason = "Busy",
                    hotKey = KeyBindingDefOf.Misc2
                };
            }
            yield break;
        }

        private Texture2D CommandTex
        {
            get
            {
                return Toggled ? SecondaryMode.defaultProjectile.uiIcon : PrimaryMode.defaultProjectile.uiIcon;
            }
        }

        private Texture2D cachedCommandTex;

        public override void CompTick()
        {
            base.CompTick();
            if (GetWearer != lastWearer)
            {
                lastWearer = GetWearer;
            }
        }

        // Token: 0x06000006 RID: 6 RVA: 0x000021F0 File Offset: 0x000003F0
        public void ToggleSecondaryFire()
        {
            compEquippable.VerbTracker.PrimaryVerb.verbProps = SecondaryMode;
        }

        // Token: 0x06000007 RID: 7 RVA: 0x0000227D File Offset: 0x0000047D
        public void TogglePrimaryFire()
        {
            compEquippable.VerbTracker.PrimaryVerb.verbProps = PrimaryMode;
        }

        public void SwitchFireMode()
        {
            switch (Toggled)
            {
                case false:
                    TogglePrimaryFire();
                    break;
                case true:
                    ToggleSecondaryFire();
                    break;
            }
        }
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                SwitchFireMode();
                this.OriginalwarmupTime = this.parent.def.Verbs[0].warmupTime;
            }
        }
        
        public Reliability Reliability
        {
            get
            {
                return this.reliability;
            }
        }
        public override string CompInspectStringExtra()
        {
            string str = "Special Rules:";
            string str2 = string.Empty;
            if (DualFireMode)
            {
                str2 = str2.NullOrEmpty() ? str2 + " Dual Fire Modes" : str2 + ", Dual Fire Modes";
            }
            if (EffectsUser)
            {
                str2 = str2.NullOrEmpty() ? str2 + " Affects User" : str2 + ", Affects User";
            }
            if (RapidFire)
            {
                str2 = str2.NullOrEmpty() ? str2 + " Rapid Fire" : str2 + ", Rapid Fire";
            }
            if (GetsHot)
            {
                str2 = str2.NullOrEmpty() ? str2 + " Gets Hot" : str2 + ", Gets Hot";
            }
            else
            if (Jams)
            {
                str2 = str2.NullOrEmpty() ? str2 + " Jams" : str2 + ", Jams";
            }
            if (TwinLinked)
            {
                str2 = str2.NullOrEmpty() ? str2 + " Twin-Linked" : str2 + ", Twin-Linked";
            }
            if (Multishot)
            {
                str2 = str2.NullOrEmpty() ? str2 + " Scattershot" : str2 + ", Scattershot";
            }
            if (Rending)
            {
                str2 = str2.NullOrEmpty() ? str2 + " Rending Weapon" : str2 + ", Rending Weapon";
            }
            if (MeltaWeapon)
            {
                str2 = str2.NullOrEmpty() ? str2 + " Melta Weapon" : str2 + ", Melta Weapon";
            }
            if (VolkiteWeapon)
            {
                str2 = str2.NullOrEmpty() ? str2 + " Volkite Weapon" : str2 + ", Volkite Weapon";
            }
            if (ConversionWeapon)
            {
                str2 = str2.NullOrEmpty() ? str2 + " Conversion Weapon" : str2 + ", Conversion Weapon";
            }
            if (HaywireWeapon)
            {
                str2 = str2.NullOrEmpty() ? str2 + " Haywire Weapon" : str2 + ", Haywire Weapon";
            }
            /*
            if (GaussWeapon)
            {
                str2 = str2.NullOrEmpty() ? str2 + " Gauss Weapon" : str2 + ", Gauss Weapon";
            }
            */
            if (TeslaWeapon)
            {
                str2 = str2.NullOrEmpty() ? str2 + " Tesla Weapon" : str2 + ", Tesla Weapon";
            }
            return str2.NullOrEmpty() ? null : str + str2;
        }

        public override string GetDescriptionPart()
        {

            string str = string.Empty;
            str = str + "Special Rules:";
            if (DualFireMode)
            {
                str = str + string.Format("\nDual Fire Modes: \n Primary: {0}, Secondary: {1}", parent.def.Verbs[0].defaultProjectile.label, SecondaryMode.defaultProjectile.label);
                str = str + string.Format("\n Current Mode: {0} \n", compEquippable.VerbTracker.PrimaryVerb.verbProps.defaultProjectile.label);
            }
            if (RapidFire)
            {
                str = str + string.Format("\n RapidFire: warmup halved ({0} seconds) and Shots per burst increased to {1} when firing at targets within {2} cells. \n",(OriginalwarmupTime / 2), compEquippable.VerbTracker.PrimaryVerb.verbProps.burstShotCount*2, compEquippable.VerbTracker.PrimaryVerb.verbProps.range/2);
            }
            if (GetsHot)
            {
                string reliabilityString;
                float failChance;
                StatPart_Reliability.GetReliability(this, out reliabilityString, out failChance);
                str = str + string.Format("\n Gets Hot: This {0} is {1} and has a {2} chance to overheat per shot fired.",parent.Label, reliabilityString, (failChance/100).ToStringPercent());
                if (HotDamageWeapon)
                {
                    str = str + string.Format(" Overheats cause {0} damage to the {1}.",HotDamage, parent.def.label);
                }
                if (GetsHotCrit)
                {
                    str = str + string.Format("it has a {0} chance to critically overheat, causing more damage to user and weapon.",(GetsHotCritChance/100).ToStringPercent());
                    if (GetsHotCritExplosion)
                    {
                        str = str + string.Format("Critical overheats have a {0} chance of cuasing the weapon to explode.", (GetsHotCritExplosionChance/100).ToStringPercent());
                    }
                }
                str = str + "\n";
            }
            if (Jams)
            {
                string reliabilityString;
                float failChance;
                StatPart_Reliability.GetReliability(this, out reliabilityString, out failChance);
                str = str + string.Format("\n Jams: This {0} is {1} and has a {2} chance to jam per shot fired.", parent.Label, reliabilityString, (failChance/100).ToStringPercent());
                if (JamsDamageWeapon)
                {
                    str = str + string.Format(" Jamming causes {0} damage to the {1}.", JamDamage, parent.def.label);
                }
                str = str + "\n";
            }
            if (TwinLinked)
            {
                str = str + "\n Twin-Linked: Fires two projectiles per shot";
            }
            if (Multishot)
            {
                str = str + string.Format("\n Scatter-Shot: Fires {0} perjectiles per shot", ScattershotCount);
            }
            if (Rending)
            {
                str = str + string.Format("\n Rending: has a {0} chance to ignore all armour", RendingChance);
            }
            if (MeltaWeapon)
            {
                str = str + string.Format("\n Melta: Damage Vs Buildings and AP doubled when firing at targets within {0} cells. \n", compEquippable.VerbTracker.PrimaryVerb.verbProps.range / 2);
            }
            if (VolkiteWeapon)
            {
                str = str + string.Format("\n Volite: AP begins to drop off when firing at targets over {0} cells. \n", compEquippable.VerbTracker.PrimaryVerb.verbProps.range / 2);
            }
            if (ConversionWeapon)
            {
                str = str + string.Format("\n Conversion: Damage and AP increase the further the target is away. \n");
            }
            if (HaywireWeapon)
            {
                str = str + "\n Haywire Weapon";
                str = str + string.Format("\n Haywire: additional EMP damage. \n");
            }
            /*
            if (GaussWeapon)
            {
                str = str + "\n Gauss Weapon";
                str = str + string.Format("\n Conversion: Damage and AP increase the further the target is away. \n");
            }
            */
            if (TeslaWeapon)
            {
                str = str + "\n Tesla Weapon";
                str = str + string.Format("\n Tesla: Bolt can arc to up to 3 nearby targets. \n");
            }
            return str;
            return base.GetDescriptionPart();
        }
    }

    public class GunVerbEntry
    {
        public bool RapidFire = false;

        public bool TyranidBurstBodySize = false;

        public Reliability reliability = Reliability.NA;
        public bool GetsHot = false;
        public bool HotDamageWeapon = false;
        public float HotDamage = 0f;
        public bool GetsHotCrit = false;
        public float GetsHotCritChance = 0f;
        public bool GetsHotCritExplosion = false;
        public float GetsHotCritExplosionChance = 0f;

        public bool Jams = false;
        public bool JamsDamageWeapon = false;
        public float JamDamage = 0f;

        public bool TwinLinked = false;
        public bool Multishot = false;

        public bool EffectsUser = false;
        public float EffectsUserChance = 0f;
        public StatDef ResistEffectStat = null;
        public HediffDef UserEffect = null;

        public bool PowerWeapon = false;
        public bool ForceWeapon = false;
        
        public bool Rending = false;
        public float RendingChance = 0.167f;
        
        public DamageDef ForceWeaponEffect = null;
        public HediffDef ForceWeaponHediff = null;
        public float ForceWeaponKillChance = 0f;
        public SoundDef ForceWeaponTriggerSound = null;

        public int ScattershotCount = 0;
        public ResearchProjectDef requiredResearch = null;
        public VerbProperties VerbProps;
        public List<string> UserEffectImmuneList = new List<string>();
    }

    public enum Reliability : short
    {
        UR = 80,
        ST = 55,
        VR = 30,
        NA = 0
    }

    public class CompProperties_UpgradeableProjectile : CompProperties
    {
        public CompProperties_UpgradeableProjectile()
        {
            this.compClass = typeof(CompUpgradeableProjectile);
        }
        public List<string> factions;
        public ThingDef projectileDef;
        public string researchDef;
    }

    // Token: 0x02000002 RID: 2
    public class CompUpgradeableProjectile : ThingComp
    {
        public CompProperties_UpgradeableProjectile Props => (CompProperties_UpgradeableProjectile)props;

        public ThingDef projectileDef => Props.projectileDef;
        public ResearchProjectDef researchDef
        {
            get
            {
                ResearchProjectDef Def = DefDatabase<ResearchProjectDef>.GetNamedSilentFail(Props.researchDef);
                if (Def != null)
                {
                    return Def;
                }
                return null;
            }
        }
        public List<FactionDef> factionDefs
        {
            get
            {
                List<FactionDef> list = new List<FactionDef>();
                foreach (var item in Props.factions)
                {
                    FactionDef Def = DefDatabase<FactionDef>.GetNamedSilentFail(item);
                    if (Def != null)
                    {
                        list.Add(Def);
                    }
                }
                return list;
            }
        }
    }
}
