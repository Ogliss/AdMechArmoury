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
        public bool HeavyWeapon = false;
        public float HeavyWeaponSetupTime = 4f;
        public List<GunVerbEntry> VerbEntries;
        public bool TyranidBurstBodySize = false;
    }

    public class CompWeapon_GunSpecialRules : CompWargearWeapon
    {
        public new CompProperties_Weapon_GunSpecialRules Props => (CompProperties_Weapon_GunSpecialRules)props;

        public List<GunVerbEntry> VerbEntries => Props.VerbEntries;
        public List<GunVerbEntry> gunVerbs;
        public List<GunVerbEntry> GunVerbs
        {
            get
            {
                if (this.gunVerbs.NullOrEmpty())
                {
                    this.gunVerbs = new List<GunVerbEntry>();
                }
                foreach (VerbProperties verb in this.parent.def.Verbs)
                {
                    int index = this.parent.def.Verbs.IndexOf(verb);

                    if (VerbEntries[index] == null)
                    {
                        VerbEntries.Add(new GunVerbEntry());
                    }
                    if (VerbEntries[index].VerbProps==null)
                    {
                        VerbEntries[index].VerbProps = verb;
                    }
                    gunVerbs.Add(VerbEntries[index]);
                }
                return gunVerbs;
            }
        }

        public CompToggleFireMode fireMode => this.parent.TryGetComp<CompToggleFireMode>();
        public bool HeavyWeapon => Props.HeavyWeapon;
        public bool TyranidBurstBodySize => Props.TyranidBurstBodySize;
        public bool TwinLinked => fireMode != null ? Props.VerbEntries[fireMode.fireMode].TwinLinked : Props.VerbEntries[0].TwinLinked;
        public bool RapidFire => fireMode != null ? Props.VerbEntries[fireMode.fireMode].RapidFire : Props.VerbEntries[0].RapidFire;
        public bool GetsHot => fireMode != null ? Props.VerbEntries[fireMode.fireMode].GetsHot : Props.VerbEntries[0].GetsHot;
        public bool HotDamageWeapon => fireMode != null ? Props.VerbEntries[fireMode.fireMode].HotDamageWeapon : Props.VerbEntries[0].HotDamageWeapon;
        public bool GetsHotCrit => fireMode != null ? Props.VerbEntries[fireMode.fireMode].GetsHotCrit : Props.VerbEntries[0].GetsHotCrit;
        public bool GetsHotCritExplosion => fireMode != null ? Props.VerbEntries[fireMode.fireMode].GetsHotCritExplosion : Props.VerbEntries[0].GetsHotCritExplosion;
        public bool Jams => fireMode != null ? Props.VerbEntries[fireMode.fireMode].Jams : Props.VerbEntries[0].Jams;
        public bool JamsDamageWeapon => fireMode != null ? Props.VerbEntries[fireMode.fireMode].JamsDamageWeapon : Props.VerbEntries[0].JamsDamageWeapon;
        public bool Multishot => fireMode != null ? Props.VerbEntries[fireMode.fireMode].Multishot : Props.VerbEntries[0].Multishot;
        public bool EffectsUser => fireMode != null ? Props.VerbEntries[fireMode.fireMode].EffectsUser : Props.VerbEntries[0].EffectsUser;
        public bool Rending => fireMode != null ? Props.VerbEntries[fireMode.fireMode].Rending : Props.VerbEntries[0].Rending;
        public Reliability reliability => fireMode != null ? Props.VerbEntries[fireMode.fireMode].reliability : Props.VerbEntries[0].reliability;
        public float HeavyWeaponSetupTime => Props.HeavyWeaponSetupTime;
        public float HotDamage => fireMode != null ? Props.VerbEntries[fireMode.fireMode].HotDamage : Props.VerbEntries[0].HotDamage;
        public float GetsHotCritChance => fireMode != null ? Props.VerbEntries[fireMode.fireMode].GetsHotCritChance : Props.VerbEntries[0].GetsHotCritChance;
        public float GetsHotCritExplosionChance => fireMode != null ? Props.VerbEntries[fireMode.fireMode].GetsHotCritExplosionChance : Props.VerbEntries[0].GetsHotCritExplosionChance;
        public float JamDamage => fireMode != null ? Props.VerbEntries[fireMode.fireMode].JamDamage : Props.VerbEntries[0].JamDamage;
        public float EffectsUserChance => fireMode != null ? Props.VerbEntries[fireMode.fireMode].EffectsUserChance : Props.VerbEntries[0].EffectsUserChance;
        public float RendingChance => fireMode != null ? Props.VerbEntries[fireMode.fireMode].RendingChance : Props.VerbEntries[0].RendingChance;
        public StatDef ResistEffectStat => fireMode != null ? Props.VerbEntries[fireMode.fireMode].ResistEffectStat : Props.VerbEntries[0].ResistEffectStat;
        public HediffDef UserEffect=> fireMode != null ? Props.VerbEntries[fireMode.fireMode].UserEffect : Props.VerbEntries[0].UserEffect;
        public List<string> UserEffectImmuneList => fireMode != null ? Props.VerbEntries[fireMode.fireMode].UserEffectImmuneList : Props.VerbEntries[0].UserEffectImmuneList;
        public ResearchProjectDef requiredResearch => fireMode != null ? Props.VerbEntries[fireMode.fireMode].requiredResearch : Props.VerbEntries[0].requiredResearch;
        public int ScattershotCount => fireMode != null ? Props.VerbEntries[fireMode.fireMode].ScattershotCount : Props.VerbEntries[0].ScattershotCount;
        public bool MeltaWeapon => fireMode != null ? Props.VerbEntries[fireMode.fireMode].VerbProps.defaultProjectile.projectile.Melta() : this.parent.def.Verbs[0].defaultProjectile.projectile.Melta();
        public bool VolkiteWeapon => fireMode != null ? Props.VerbEntries[fireMode.fireMode].VerbProps.defaultProjectile.projectile.Volkite() : this.parent.def.Verbs[0].defaultProjectile.projectile.Volkite();
        public bool GaussWeapon => fireMode != null ? Props.VerbEntries[fireMode.fireMode].VerbProps.defaultProjectile.projectile.Gauss() : this.parent.def.Verbs[0].defaultProjectile.projectile.Gauss();
        public bool HaywireWeapon => fireMode != null ? Props.VerbEntries[fireMode.fireMode].VerbProps.defaultProjectile.projectile.Haywire() : this.parent.def.Verbs[0].defaultProjectile.projectile.Haywire();
        public bool TeslaWeapon => fireMode != null ? Props.VerbEntries[fireMode.fireMode].VerbProps.defaultProjectile.projectile.Tesla() : this.parent.def.Verbs[0].defaultProjectile.projectile.Tesla();
        public bool ConversionWeapon => fireMode != null ? Props.VerbEntries[fireMode.fireMode].VerbProps.defaultProjectile.projectile.Conversion() : this.parent.def.Verbs[0].defaultProjectile.projectile.Conversion();
        public int LastMovedTick
        {
            get
            {

                if (CasterPawn == null)
                {
                    return 0;
                }
                if (!CasterPawn.pather.MovedRecently(HeavyWeaponSetupTime.SecondsToTicks()))
                {
                    return int.MaxValue;
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

        public override void CompTick()
        {
            base.CompTick();
            if (CasterPawn != lastWearer)
            {
                lastWearer = CasterPawn;
            }
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            foreach (GunVerbEntry entry in GunVerbs)
            {
                if (entry.originalWarmup < 0)
                {
                    entry.originalWarmup = entry.VerbProps.warmupTime;
                    Log.Message(parent.LabelShortCap + " setting inital warmup time to " + "( " + entry.originalWarmup + " ) " + entry.VerbProps.warmupTime + " for " + entry.VerbProps.label ?? entry.VerbProps.defaultProjectile.label);
                }
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
            if (fireMode!=null)
            {
                str2 = str2.NullOrEmpty() ? str2 + " Fire Modes" : str2 + ", Fire Modes";
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
            if (fireMode!=null)
            {
                str = str + string.Format("\nFire Modes: ");
                foreach (VerbProperties mode in parent.def.Verbs)
                {
                    if (parent.def.Verbs.IndexOf(mode) == 0)
                    {
                        str = str + string.Format("\n Primary: {0}", mode.label ?? mode.defaultProjectile.label);
                    }
                    if (parent.def.Verbs.IndexOf(mode) != 0)
                    {
                        str = str + string.Format("\n Secondry: {0}", mode.label ?? mode.defaultProjectile.label);
                    }
                }
                str = str + string.Format("\n Current Mode: {0} \n", fireMode.Active.label ?? fireMode.Active.defaultProjectile.label);
            }
            if (RapidFire)
            {
                str = str + string.Format("\n RapidFire: Warmup halved ({0} seconds) and Cooldown halved ({1} seconds) when firing at targets within {2} cells. \n", (this.GunVerbs[0].originalWarmup / 2), compEquippable.VerbTracker.PrimaryVerb.verbProps.defaultCooldownTime / 2, compEquippable.VerbTracker.PrimaryVerb.verbProps.range/2);
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

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
        }

    }

    public class GunVerbEntry: IExposable
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
        public float originalWarmup = -1;
        public VerbProperties VerbProps;
        public List<string> UserEffectImmuneList = new List<string>();

        public void ExposeData()
        {
            Scribe_Values.Look<float>(ref this.originalWarmup, "originalWarmup", -1f);
        }
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
