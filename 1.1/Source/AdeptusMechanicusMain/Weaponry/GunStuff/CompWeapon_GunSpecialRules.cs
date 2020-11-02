using AdeptusMechanicus.ExtensionMethods;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
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
        public float HeavyWeaponSetupTime;
        public List<GunVerbEntry> VerbEntries;
        public bool TyranidBurstBodySize = false;
        public new bool GizmosOnEquip = true;
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
        public GunVerbEntry ActiveVerbEntry => this.GunVerbs[this.CurMode];
        public VerbProperties ActiveVerbProperties => FireMode?.Active ?? Equipable.PrimaryVerb.verbProps;
        public CompToggleFireMode fireMode;
        public CompToggleFireMode FireMode => fireMode ??= this.parent.TryGetComp<CompToggleFireMode>();
        public CompEquippable equipable; 
        public CompEquippable Equipable => equipable ??= this.parent.TryGetComp<CompEquippable>(); 
        public override bool GizmosOnEquip => FireMode != null;
        public int CurMode => FireMode != null ? FireMode.fireMode : 0;
        public bool HeavyWeapon => Props.HeavyWeapon;
        public float HeavyWeaponSetupTime
        {
            get
            {
                float statValue = this.CasterPawn.GetStatValue(StatDefOf.AimingDelayFactor, true);
                int ticks = (Equipable.PrimaryVerb.verbProps.warmupTime * statValue).SecondsToTicks();
                return Props?.HeavyWeaponSetupTime ?? ticks.TicksToSeconds();
            }
        }
        public bool TyranidBurstBodySize => Props.TyranidBurstBodySize;
        public bool TwinLinked => GunVerbs[CurMode].TwinLinked;
        public bool RapidFire => GunVerbs[CurMode].RapidFire;
        public float RapidFireReductionBase => ((ActiveVerbProperties.burstShotCount - 1) * ActiveVerbProperties.ticksBetweenBurstShots).TicksToSeconds() / 4;
        public float RapidFireWarmupReduction
        {
            get
            {
                float warmup = ActiveVerbProperties.warmupTime;
                float warmupreduction = (warmup / 2) + RapidFireReductionBase;
                return (warmupreduction / warmup) * 100;
            }
        }
        public float RapidFireCooldownReduction
        {
            get
            {
                float cooldown = parent.GetStatValue(StatDefOf.RangedWeapon_Cooldown);
                float cooldownreduction = (cooldown / 2) + RapidFireReductionBase;
                return (cooldownreduction / cooldown) * 100;
            }
        }
        public bool GetsHot => GunVerbs[CurMode].GetsHot;
        public bool HotDamageWeapon => GunVerbs[CurMode].HotDamageWeapon;
        public bool GetsHotCrit => GunVerbs[CurMode].GetsHotCrit;
        public bool GetsHotCritExplosion => GunVerbs[CurMode].GetsHotCritExplosion;
        public bool Jams => GunVerbs[CurMode].Jams;
        public bool JamsDamageWeapon => GunVerbs[CurMode].JamsDamageWeapon;
        public bool EffectsUser => GunVerbs[CurMode].EffectsUser;
        public bool Rending => GunVerbs[CurMode].Rending;
        public Reliability reliability =>  GunVerbs[CurMode].reliability;
        public float HotDamage => GunVerbs[CurMode].HotDamage;
        public float GetsHotCritChance => GunVerbs[CurMode].GetsHotCritChance;
        public float GetsHotCritExplosionChance => GunVerbs[CurMode].GetsHotCritExplosionChance;
        public float JamDamage => GunVerbs[CurMode].JamDamage;
        public float EffectsUserChance => GunVerbs[CurMode].EffectsUserChance;
        public float RendingChance => GunVerbs[CurMode].RendingChance;
        public StatDef ResistEffectStat => GunVerbs[CurMode].ResistEffectStat;
        public HediffDef UserEffect=> GunVerbs[CurMode].UserEffect;
        public List<string> UserEffectImmuneList =>  GunVerbs[CurMode].UserEffectImmuneList;
        public ResearchProjectDef requiredResearch => GunVerbs[CurMode].requiredResearch;
        public bool Multishot => Equipable != null && Equipable.PrimaryVerb.GetProjectile().HasModExtension<ScattershotProjectileExtension>();
        // public bool Multishot => Props.VerbEntries[CurMode].Multishot || Props.VerbEntries[CurMode].VerbProps.defaultProjectile.HasModExtension<ScattershotProjectileExtension>();

        // public int ScattershotCount => GunVerbs[CurMode].VerbProps.defaultProjectile.GetModExtension<ScattershotProjectileExtension>() as ScattershotProjectileExtension is ScattershotProjectileExtension ext ? ext.projectileCount : 0;
        public int ScattershotCount => Multishot && Equipable != null && Equipable.PrimaryVerb.GetProjectile().GetModExtension<ScattershotProjectileExtension>() as ScattershotProjectileExtension is ScattershotProjectileExtension ext ? ext.projectileCount : 0;
        public bool MeltaWeapon => Equipable != null && Equipable.PrimaryVerb.GetProjectile().projectile.Melta();
        public bool VolkiteWeapon => Equipable != null && Equipable.PrimaryVerb.GetProjectile().projectile.Volkite();
        public bool GaussWeapon => Equipable != null && Equipable.PrimaryVerb.GetProjectile().projectile.Gauss();
        public bool HaywireWeapon => Equipable != null && Equipable.PrimaryVerb.GetProjectile().projectile.Haywire();
        public bool TeslaWeapon => Equipable != null && Equipable.PrimaryVerb.GetProjectile().projectile.Tesla();
        public bool ConversionWeapon => Equipable != null && Equipable.PrimaryVerb.GetProjectile().projectile.Conversion();
        public int LastMovedTick
        {
            get
            {
                return lastmovedTick;
            }
            set
            {
                lastmovedTick = value;
            }
        }
        public int ticksHere
        {
            get
            {
                return Find.TickManager.TicksGame - LastMovedTick;
            }
        }
        private int lastmovedTick;

        public override void CompTick()
        {
            base.CompTick();
            if (CasterPawn != lastWearer)
            {
                lastWearer = CasterPawn;
            }
        }


        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref this.lastmovedTick, "WeaponSpecialRuleslastmovedTick", 0);
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
            string str = "AMA_SpecialRules".Translate();
            string str2 = string.Empty;
            if (HeavyWeapon)
            {
                str2 = str2.NullOrEmpty() ? str2 + " " + "AMA_HeavyWeapon".Translate().ToString() : str2 + ", " + "AMA_HeavyWeapon".Translate().ToString();
            }
            if (FireMode!=null)
            {
                string fire = !FireMode.Props.InspectLabelKey.NullOrEmpty() ? FireMode.Props.InspectLabelKey.Translate().ToString() : "Fire Modes";
                str2 = str2.NullOrEmpty() ? str2 + " "+ fire : str2 + ", " + fire;
            }
            if (EffectsUser)
            {
                str2 = str2.NullOrEmpty() ? str2 + " Affects User" : str2 + ", Affects User";
            }
            if (RapidFire)
            {
                str2 = str2.NullOrEmpty() ? str2 + " "+ "AMA_RapidFire".Translate().ToString() : str2 + ", " + "AMA_RapidFire".Translate().ToString();
            }
            if (GetsHot)
            {
                str2 = str2.NullOrEmpty() ? str2 + " " + "AMA_GetsHot".Translate().ToString() : str2 + ", " + "AMA_GetsHot".Translate().ToString();
            }
            else
            if (Jams)
            {
                str2 = str2.NullOrEmpty() ? str2 + " " + "AMA_Jams".Translate().ToString() : str2 + ", " + "AMA_Jams".Translate().ToString();
            }
            if (TwinLinked)
            {
                str2 = str2.NullOrEmpty() ? str2 + " " + "AMA_TwinLinked".Translate().ToString() : str2 + ", " + "AMA_TwinLinked".Translate().ToString();
            }
            if (Multishot)
            {
                str2 = str2.NullOrEmpty() ? str2 + " " + "AMA_Scatter".Translate().ToString() : str2 + ", " + "AMA_Scatter".Translate().ToString();
            }
            if (Rending)
            {
                str2 = str2.NullOrEmpty() ? str2 + " " + "AMA_Rending_Shot".Translate().ToString() : str2 + ", " + "AMA_Rending_Shot".Translate().ToString();
            }
            if (MeltaWeapon)
            {
                str2 = str2.NullOrEmpty() ? str2 + " " + "AMA_Melta".Translate().ToString() : str2 + ", " + "AMA_Melta".Translate().ToString();
            }
            if (VolkiteWeapon)
            {
                str2 = str2.NullOrEmpty() ? str2 + " " + "AMA_Volkite".Translate().ToString() : str2 + ", " + "AMA_Volkite".Translate().ToString();
            }
            if (ConversionWeapon)
            {
                str2 = str2.NullOrEmpty() ? str2 + " " + "AMA_ConversionBeam".Translate().ToString() : str2 + ", " + "AMA_ConversionBeam".Translate().ToString();
            }
            if (HaywireWeapon)
            {
                str2 = str2.NullOrEmpty() ? str2 + " " + "AMA_Haywire".Translate().ToString() : str2 + ", " + "AMA_Haywire".Translate().ToString();
            }
            /*
            if (GaussWeapon)
            {
                str2 = str2.NullOrEmpty() ? str2 + " "+ "AMA_Gauss".Translate().ToString() : str2 + ", "+ "AMA_RapidFire".Translate().ToString();
            }
            */
            if (TeslaWeapon)
            {
                str2 = str2.NullOrEmpty() ? str2 + " " + "AMA_Tesla".Translate().ToString() : str2 + ", " + "AMA_Tesla".Translate().ToString();
            }
            return str2.NullOrEmpty() ? null : str + str2;
        }

        public override string GetDescriptionPart()
        {
            StringBuilder builder = new StringBuilder(base.GetDescriptionPart());
            builder.AppendLine("AMA_SpecialRules".Translate());
            if (FireMode!=null)
            {
                builder.AppendLine();
                builder.AppendLine(string.Format("Fire Modes: "));
                foreach (VerbProperties mode in parent.def.Verbs)
                {
                    if (parent.def.Verbs.IndexOf(mode) == 0)
                    {
                        builder.AppendLine(string.Format("    Primary: {0}", mode.label ?? mode.defaultProjectile.label));
                    }
                    if (parent.def.Verbs.IndexOf(mode) != 0)
                    {
                        builder.AppendLine(string.Format("        Alternate: {0}", mode.label ?? mode.defaultProjectile.label));
                    }
                }
                builder.AppendLine();
                builder.AppendLine(string.Format("Current Mode: {0}", FireMode.Active.label ?? FireMode.Active.defaultProjectile.label));
            }

            if (RapidFire)
            {
                builder.AppendLine();
                float warmup = ActiveVerbProperties.warmupTime;
                float cooldown = parent.GetStatValue(StatDefOf.RangedWeapon_Cooldown);
                float Cycle = cooldown + warmup + (RapidFireReductionBase * 4);
                float warmupreduction = (warmup / 2) + RapidFireReductionBase;
                float cooldownreduction = (cooldown / 2) + RapidFireReductionBase;
                float newCycle = (cooldown - cooldownreduction) + (warmup - warmupreduction) + (RapidFireReductionBase * 4);

                builder.AppendLine("AMA_RapidFire".Translate() + ":");
                builder.AppendLine("AMA_RapidFireDesc".Translate(RapidFireWarmupReduction.ToStringByStyle(ToStringStyle.FloatMaxTwo), warmup - warmupreduction, RapidFireCooldownReduction.ToStringByStyle(ToStringStyle.FloatMaxOne), cooldown - cooldownreduction, ActiveVerbProperties.range / 2, Cycle, newCycle));
            }
            if (GetsHot)
            {
                builder.AppendLine();
                string reliabilityString;
                float failChance;
                StatPart_Reliability.GetReliability(this, out reliabilityString, out failChance);
                builder.AppendLine(string.Format("AMA_GetsHot".Translate() + ": AMA_GetsHotDesc".Translate(parent.Label, reliabilityString, (failChance / 100).ToStringPercent())));
                if (HotDamageWeapon)
                {
                    builder.AppendLine("AMA_GetsHotWeaponDamage".Translate(HotDamage, parent.def.label));
                }
                if (GetsHotCrit)
                {
                    builder.Append(" "+"AMA_GetsHotCrit".Translate((GetsHotCritChance / 100).ToStringPercent()));
                    if (GetsHotCritExplosion)
                    {
                        builder.AppendLine("AMA_GetsHotCritExplosion".Translate((GetsHotCritExplosionChance / 100).ToStringPercent()));
                    }
                }
            }
            if (Jams)
            {
                builder.AppendLine();
                string reliabilityString;
                float failChance;
                StatPart_Reliability.GetReliability(this, out reliabilityString, out failChance);
                builder.AppendLine(string.Format("AMA_Jams".Translate() + ": " + "AMA_JamsDesc".Translate(parent.Label, reliabilityString, (failChance / 100).ToStringPercent())));
                if (JamsDamageWeapon)
                {
                    builder.Append(" "+"AMA_JamsWeaponDamage".Translate(JamDamage, parent.def.label));
                }
            }
            if (TwinLinked)
            {
                builder.AppendLine();
                builder.AppendLine("AMA_TwinLinked".Translate() + ": " + "AMA_TwinLinkedDesc".Translate());
            }
            if (Multishot)
            {
                builder.AppendLine();
                builder.AppendLine(string.Format("AMA_Scatter".Translate() + ": " + "AMA_ScatterDesc".Translate(ScattershotCount)));
            }
            if (Rending)
            {
                builder.AppendLine();
                builder.AppendLine(string.Format("AMA_Rending_Shot".Translate() + ": " + "AMA_Rending_ShotDesc".Translate(RendingChance)));
            }
            if (MeltaWeapon)
            {
                builder.AppendLine();
                builder.AppendLine(string.Format("AMA_Melta".Translate() + ": " + "AMA_MeltaDesc".Translate(compEquippable.VerbTracker.PrimaryVerb.verbProps.range / 2) + " \n"));
            }
            if (VolkiteWeapon)
            {
                builder.AppendLine();
                builder.AppendLine(string.Format("AMA_Volite".Translate() + ": " + "AMA_VoliteDesc".Translate(compEquippable.VerbTracker.PrimaryVerb.verbProps.range / 2)));
            }
            if (ConversionWeapon)
            {
                builder.AppendLine();
                builder.AppendLine(string.Format("AMA_ConversionBeam".Translate() + ": " + "AMA_ConversionBeamDesc".Translate()));
            }
            if (HaywireWeapon)
            {
                builder.AppendLine();
                builder.AppendLine(string.Format("AMA_Haywire".Translate() + ": " + "AMA_HaywireDesc".Translate()));
            }
            /*
            if (GaussWeapon)
            {
                builder.AppendLine();
                builder.AppendLine("Gauss Weapon:");
                builder.AppendLine(string.Format("AMA_Gauss".Translate() + ": " + "AMA_GaussDesc".Translate()));
            }
            */
            if (TeslaWeapon)
            {
                builder.AppendLine();
                builder.AppendLine("Tesla Weapon:");
                builder.AppendLine(string.Format("AMA_Tesla".Translate() + ": " + "AMA_TeslaDesc".Translate()));
            }
            return builder.ToString();
        }

        public void buildRulesString(ref StringBuilder builder, GunVerbEntry mode)
        {
            VerbProperties ActiveVerbProperties = mode.VerbProps;
            if (RapidFire)
            {
                builder.AppendLine();
                float warmup = ActiveVerbProperties.warmupTime;
                float cooldown = parent.GetStatValue(StatDefOf.RangedWeapon_Cooldown);
                float Cycle = cooldown + warmup + (RapidFireReductionBase * 4);
                float warmupreduction = (warmup / 2) + RapidFireReductionBase;
                float cooldownreduction = (cooldown / 2) + RapidFireReductionBase;
                float newCycle = (cooldown - cooldownreduction) + (warmup - warmupreduction) + (RapidFireReductionBase * 4);

                builder.AppendLine("AMA_RapidFire".Translate() + ":");
                builder.AppendLine("AMA_RapidFireDesc".Translate(RapidFireWarmupReduction.ToStringByStyle(ToStringStyle.FloatMaxTwo), warmup - warmupreduction, RapidFireCooldownReduction.ToStringByStyle(ToStringStyle.FloatMaxOne), cooldown - cooldownreduction, ActiveVerbProperties.range / 2, Cycle, newCycle));
            }
            if (GetsHot)
            {
                builder.AppendLine();
                string reliabilityString;
                float failChance;
                StatPart_Reliability.GetReliability(this, out reliabilityString, out failChance);
                builder.AppendLine(string.Format("AMA_GetsHot".Translate() + ": AMA_GetsHotDesc".Translate(parent.Label, reliabilityString, (failChance / 100).ToStringPercent())));
                if (HotDamageWeapon)
                {
                    builder.AppendLine("AMA_GetsHotWeaponDamage".Translate(HotDamage, parent.def.label));
                }
                if (GetsHotCrit)
                {
                    builder.Append(" " + "AMA_GetsHotCrit".Translate((GetsHotCritChance / 100).ToStringPercent()));
                    if (GetsHotCritExplosion)
                    {
                        builder.AppendLine("AMA_GetsHotCritExplosion".Translate((GetsHotCritExplosionChance / 100).ToStringPercent()));
                    }
                }
            }
            if (Jams)
            {
                builder.AppendLine();
                string reliabilityString;
                float failChance;
                StatPart_Reliability.GetReliability(this, out reliabilityString, out failChance);
                builder.AppendLine(string.Format("AMA_Jams".Translate() + ": " + "AMA_JamsDesc".Translate(parent.Label, reliabilityString, (failChance / 100).ToStringPercent())));
                if (JamsDamageWeapon)
                {
                    builder.Append(" " + "AMA_JamsWeaponDamage".Translate(JamDamage, parent.def.label));
                }
            }
            if (TwinLinked)
            {
                builder.AppendLine();
                builder.AppendLine("AMA_TwinLinked".Translate() + ": " + "AMA_TwinLinkedDesc".Translate());
            }
            if (Multishot)
            {
                builder.AppendLine();
                builder.AppendLine(string.Format("AMA_Scatter".Translate() + ": " + "AMA_ScatterDesc".Translate(ScattershotCount)));
            }
            if (Rending)
            {
                builder.AppendLine();
                builder.AppendLine(string.Format("AMA_Rending_Shot".Translate() + ": " + "AMA_Rending_ShotDesc".Translate(RendingChance)));
            }
            if (MeltaWeapon)
            {
                builder.AppendLine();
                builder.AppendLine(string.Format("AMA_Melta".Translate() + ": " + "AMA_MeltaDesc".Translate(compEquippable.VerbTracker.PrimaryVerb.verbProps.range / 2) + " \n"));
            }
            if (VolkiteWeapon)
            {
                builder.AppendLine();
                builder.AppendLine(string.Format("AMA_Volite".Translate() + ": " + "AMA_VoliteDesc".Translate(compEquippable.VerbTracker.PrimaryVerb.verbProps.range / 2)));
            }
            if (ConversionWeapon)
            {
                builder.AppendLine();
                builder.AppendLine(string.Format("AMA_ConversionBeam".Translate() + ": " + "AMA_ConversionBeamDesc".Translate()));
            }
            if (HaywireWeapon)
            {
                builder.AppendLine();
                builder.AppendLine(string.Format("AMA_Haywire".Translate() + ": " + "AMA_HaywireDesc".Translate()));
            }
            /*
            if (GaussWeapon)
            {
                builder.AppendLine();
                builder.AppendLine("Gauss Weapon:");
                builder.AppendLine(string.Format("AMA_Gauss".Translate() + ": " + "AMA_GaussDesc".Translate()));
            }
            */
            if (TeslaWeapon)
            {
                builder.AppendLine();
                builder.AppendLine("Tesla Weapon:");
                builder.AppendLine(string.Format("AMA_Tesla".Translate() + ": " + "AMA_TeslaDesc".Translate()));
            }
        }


        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
        }

        public override IEnumerable<Gizmo> EquippedGizmos()
        {
            if (FireMode!=null)
            {
                foreach (var item in FireMode.EquippedGizmos())
                {
                    yield return item;
                }
            }

        }

        public override void ReceiveCompSignal(string signal)
        {
            switch (signal)
            {
                case HeavyWeaponMovedSignal:
                    this.LastMovedTick = Find.TickManager.TicksGame;
                    break;
                default:
                    base.ReceiveCompSignal(signal);
                    break;
            }
        }

        public const string HeavyWeaponMovedSignal = "OG40K_GunSpecialRules_HeavyWeaponMovedSignal";
    }
}
