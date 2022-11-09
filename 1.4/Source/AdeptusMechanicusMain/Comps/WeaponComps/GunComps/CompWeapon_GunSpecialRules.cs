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
        public Verb ActiveVerb => FireMode?.ActiveVerb ?? Equipable.PrimaryVerb;
        public VerbProperties ActiveVerbProperties => FireMode?.ActiveProps ?? Equipable.PrimaryVerb.verbProps;
        public IAdvancedVerb AdvancedVerb => ActiveVerbProperties as IAdvancedVerb;
        public CompToggleFireMode fireMode;
        public CompToggleFireMode FireMode => fireMode ??= this.parent.TryGetCompFast<CompToggleFireMode>();
        public CompEquippable equipable; 
        public CompEquippable Equipable => equipable ??= this.parent.TryGetCompFast<CompEquippable>(); 
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
        public bool TwinLinked => AdvancedVerb?.ScattershotCount == 1f;
        public bool RapidFire => AdvancedVerb?.RapidFire ?? false;
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
        public bool GetsHot => AdvancedVerb?.GetsHot ?? false;
        public bool HotDamageWeapon => AdvancedVerb?.HotDamageWeapon ?? false;
        public bool GetsHotCrit => AdvancedVerb?.GetsHotCrit ?? false;
        public bool GetsHotCritExplosion => AdvancedVerb?.GetsHotCritExplosion ?? false;
        public bool Jams => AdvancedVerb?.Jams ?? false;
        public bool JamsDamageWeapon => AdvancedVerb?.JamsDamageWeapon ?? false;
        public bool EffectsUser => AdvancedVerb?.EffectsUser ?? false;
        public bool Rending => AdvancedVerb?.Rending ?? false;
        public Reliability Reliability => AdvancedVerb?.Reliability ?? Reliability.NA;
        public float HotDamage => AdvancedVerb?.HotDamage ?? 0f;
        public float GetsHotCritChance => AdvancedVerb?.GetsHotCritChance ?? 0f;
        public float GetsHotCritExplosionChance => AdvancedVerb?.GetsHotCritExplosionChance ?? 0f;
        public float JamDamage => AdvancedVerb?.JamDamage ?? 0f;
        public float EffectsUserChance => AdvancedVerb?.EffectsUserChance ?? 0f;
        public float RendingChance => AdvancedVerb?.RendingChance ?? 0f;
        public StatDef ResistEffectStat => AdvancedVerb?.ResistEffectStat;
        public HediffDef UserEffect=> AdvancedVerb?.UserEffect;
        public List<string> UserEffectImmuneList => AdvancedVerb?.UserEffectImmuneList;
        public ResearchProjectDef RequiredResearch => AdvancedVerb?.RequiredResearch;
        public bool Multishot => (ActiveVerb?.GetProjectile()?.HasModExtension<ScattershotProjectileExtension>() ?? false) || (AdvancedVerb?.Multishot ?? false);
        // public bool Multishot => Props.VerbEntries[CurMode].Multishot || Props.VerbEntries[CurMode].VerbProps.defaultProjectile.HasModExtension<ScattershotProjectileExtension>();

        // public int ScattershotCount => GunVerbs[CurMode].VerbProps.defaultProjectile.GetModExtensionFast<ScattershotProjectileExtension>() as ScattershotProjectileExtension is ScattershotProjectileExtension ext ? ext.projectileCount : 0;
        public int ScattershotCount => Multishot && ActiveVerb?.GetProjectile()?.GetModExtensionFast<ScattershotProjectileExtension>() as ScattershotProjectileExtension is ScattershotProjectileExtension ext && ext.projectileCount.HasValue ? ext.projectileCount.Value : 0;
        public bool MeltaWeapon => ActiveVerb?.GetProjectile()?.projectile.Melta() ?? false;
        public bool VolkiteWeapon => ActiveVerb?.GetProjectile()?.projectile.Volkite() ?? false;
        public bool GaussWeapon => ActiveVerb?.GetProjectile()?.projectile.Gauss() ?? false;
        public bool HaywireWeapon => ActiveVerb?.GetProjectile()?.projectile.Haywire() ?? false;
        public bool TeslaWeapon => ActiveVerb?.GetProjectile()?.projectile.Tesla() ?? false;
        public bool ConversionWeapon => ActiveVerb?.GetProjectile()?.projectile.Conversion() ?? false;
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

        public override string CompInspectStringExtra()
        {
            string str = "AdeptusMechanicus.SpecialRules".Translate();
            string str2 = string.Empty;
            if (HeavyWeapon)
            {
                str2 = str2.NullOrEmpty() ? str2 + " " + "AdeptusMechanicus.HeavyWeapon".Translate().ToString() : str2 + ", " + "AdeptusMechanicus.HeavyWeapon".Translate().ToString();
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
                str2 = str2.NullOrEmpty() ? str2 + " "+ "AdeptusMechanicus.RapidFire".Translate().ToString() : str2 + ", " + "AdeptusMechanicus.RapidFire".Translate().ToString();
            }
            if (GetsHot)
            {
                str2 = str2.NullOrEmpty() ? str2 + " " + "AdeptusMechanicus.GetsHot".Translate().ToString() : str2 + ", " + "AdeptusMechanicus.GetsHot".Translate().ToString();
            }
            else
            if (Jams)
            {
                str2 = str2.NullOrEmpty() ? str2 + " " + "AdeptusMechanicus.Jams".Translate().ToString() : str2 + ", " + "AdeptusMechanicus.Jams".Translate().ToString();
            }
            if (TwinLinked)
            {
                str2 = str2.NullOrEmpty() ? str2 + " " + "AdeptusMechanicus.TwinLinked".Translate().ToString() : str2 + ", " + "AdeptusMechanicus.TwinLinked".Translate().ToString();
            }
            if (Multishot)
            {
                str2 = str2.NullOrEmpty() ? str2 + " " + "AdeptusMechanicus.Scatter".Translate().ToString() : str2 + ", " + "AdeptusMechanicus.Scatter".Translate().ToString();
            }
            if (Rending)
            {
                str2 = str2.NullOrEmpty() ? str2 + " " + "AdeptusMechanicus.Rending_Shot".Translate().ToString() : str2 + ", " + "AdeptusMechanicus.Rending_Shot".Translate().ToString();
            }
            if (MeltaWeapon)
            {
                str2 = str2.NullOrEmpty() ? str2 + " " + "AdeptusMechanicus.Melta".Translate().ToString() : str2 + ", " + "AdeptusMechanicus.Melta".Translate().ToString();
            }
            if (VolkiteWeapon)
            {
                str2 = str2.NullOrEmpty() ? str2 + " " + "AdeptusMechanicus.Volkite".Translate().ToString() : str2 + ", " + "AdeptusMechanicus.Volkite".Translate().ToString();
            }
            if (ConversionWeapon)
            {
                str2 = str2.NullOrEmpty() ? str2 + " " + "AdeptusMechanicus.ConversionBeam".Translate().ToString() : str2 + ", " + "AdeptusMechanicus.ConversionBeam".Translate().ToString();
            }
            if (HaywireWeapon)
            {
                str2 = str2.NullOrEmpty() ? str2 + " " + "AdeptusMechanicus.Haywire".Translate().ToString() : str2 + ", " + "AdeptusMechanicus.Haywire".Translate().ToString();
            }
            if (GaussWeapon)
            {
                str2 = str2.NullOrEmpty() ? str2 + " "+ "AdeptusMechanicus.Gauss".Translate().ToString() : str2 + ", "+ "AdeptusMechanicus.RapidFire".Translate().ToString();
            }
            if (TeslaWeapon)
            {
                str2 = str2.NullOrEmpty() ? str2 + " " + "AdeptusMechanicus.Tesla".Translate().ToString() : str2 + ", " + "AdeptusMechanicus.Tesla".Translate().ToString();
            }
            return str2.NullOrEmpty() ? null : str + str2;
        }

        public override string GetDescriptionPart()
        {
            StringBuilder builder = new StringBuilder(base.GetDescriptionPart());
            builder.AppendLine("AdeptusMechanicus.SpecialRules".Translate());
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
                builder.AppendLine(string.Format("Current Mode: {0}", FireMode.ActiveProps.label ?? FireMode.ActiveProps.defaultProjectile.label));
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

                builder.AppendLine("AdeptusMechanicus.RapidFire".Translate() + ":");
                builder.AppendLine("AdeptusMechanicus.RapidFireDesc".Translate(RapidFireWarmupReduction.ToStringByStyle(ToStringStyle.FloatMaxTwo), warmup - warmupreduction, RapidFireCooldownReduction.ToStringByStyle(ToStringStyle.FloatMaxOne), cooldown - cooldownreduction, ActiveVerbProperties.range / 2, Cycle, newCycle));
            }
            if (GetsHot)
            {
                builder.AppendLine();
                StatPart_Reliability.GetReliability(this, out string reliabilityString, out float failChance);
                builder.AppendLine(string.Format("AdeptusMechanicus.GetsHot".Translate() + ": AMA_GetsHotDesc".Translate(parent.Label, reliabilityString, (failChance / 100).ToStringPercent())));
                if (HotDamageWeapon)
                {
                    builder.AppendLine("AdeptusMechanicus.GetsHotWeaponDamage".Translate(HotDamage, parent.def.label));
                }
                if (GetsHotCrit)
                {
                    builder.Append(" "+"AdeptusMechanicus.GetsHotCrit".Translate((GetsHotCritChance / 100).ToStringPercent()));
                    if (GetsHotCritExplosion)
                    {
                        builder.AppendLine("AdeptusMechanicus.GetsHotCritExplosion".Translate((GetsHotCritExplosionChance / 100).ToStringPercent()));
                    }
                }
            }
            if (Jams)
            {
                builder.AppendLine();
                StatPart_Reliability.GetReliability(this, out string reliabilityString, out float failChance);
                builder.AppendLine(string.Format("AdeptusMechanicus.Jams".Translate() + ": " + "AdeptusMechanicus.JamsDesc".Translate(parent.Label, reliabilityString, (failChance / 100).ToStringPercent())));
                if (JamsDamageWeapon)
                {
                    builder.Append(" "+"AdeptusMechanicus.JamsWeaponDamage".Translate(JamDamage, parent.def.label));
                }
            }
            if (TwinLinked)
            {
                builder.AppendLine();
                builder.AppendLine("AdeptusMechanicus.TwinLinked".Translate() + ": " + "AdeptusMechanicus.TwinLinkedDesc".Translate());
            }
            if (Multishot)
            {
                builder.AppendLine();
                builder.AppendLine(string.Format("AdeptusMechanicus.Scatter".Translate() + ": " + "AdeptusMechanicus.ScatterDesc".Translate(ScattershotCount)));
            }
            if (Rending)
            {
                builder.AppendLine();
                if (GaussWeapon)
                {
                    builder.AppendLine("Gauss Weapon:");
                //    builder.AppendLine(string.Format("AdeptusMechanicus.Gauss".Translate() + ": " + "AdeptusMechanicus.GaussDesc".Translate()));
                }
                builder.AppendLine(string.Format("AdeptusMechanicus.Rending_Shot".Translate() + ": " + "AdeptusMechanicus.Rending_ShotDesc".Translate(RendingChance)));
            }
            if (MeltaWeapon)
            {
                builder.AppendLine();
                builder.AppendLine(string.Format("AdeptusMechanicus.Melta".Translate() + ": " + "AdeptusMechanicus.MeltaDesc".Translate(compEquippable.VerbTracker.PrimaryVerb.verbProps.range / 2) + " \n"));
            }
            if (VolkiteWeapon)
            {
                builder.AppendLine();
                builder.AppendLine(string.Format("AdeptusMechanicus.Volkite".Translate() + ": " + "AdeptusMechanicus.VolkiteDesc".Translate(compEquippable.VerbTracker.PrimaryVerb.verbProps.range / 2)));
            }
            if (ConversionWeapon)
            {
                builder.AppendLine();
                builder.AppendLine(string.Format("AdeptusMechanicus.ConversionBeam".Translate() + ": " + "AdeptusMechanicus.ConversionBeamDesc".Translate()));
            }
            if (HaywireWeapon)
            {
                builder.AppendLine();
                builder.AppendLine(string.Format("AdeptusMechanicus.Haywire".Translate() + ": " + "AdeptusMechanicus.HaywireDesc".Translate()));
            }
            if (TeslaWeapon)
            {
                builder.AppendLine();
                builder.AppendLine("Tesla Weapon:");
                builder.AppendLine(string.Format("AdeptusMechanicus.Tesla".Translate() + ": " + "AdeptusMechanicus.TeslaDesc".Translate()));
            }
            return builder.ToString();
        }

        public void BuildRulesString(ref StringBuilder builder, GunVerbEntry mode)
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

                builder.AppendLine("AdeptusMechanicus.RapidFire".Translate() + ":");
                builder.AppendLine("AdeptusMechanicus.RapidFireDesc".Translate(RapidFireWarmupReduction.ToStringByStyle(ToStringStyle.FloatMaxTwo), warmup - warmupreduction, RapidFireCooldownReduction.ToStringByStyle(ToStringStyle.FloatMaxOne), cooldown - cooldownreduction, ActiveVerbProperties.range / 2, Cycle, newCycle));
            }
            if (GetsHot)
            {
                builder.AppendLine();
                StatPart_Reliability.GetReliability(this, out string reliabilityString, out float failChance);
                builder.AppendLine(string.Format("AdeptusMechanicus.GetsHot".Translate() + ": AMA_GetsHotDesc".Translate(parent.Label, reliabilityString, (failChance / 100).ToStringPercent())));
                if (HotDamageWeapon)
                {
                    builder.AppendLine("AdeptusMechanicus.GetsHotWeaponDamage".Translate(HotDamage, parent.def.label));
                }
                if (GetsHotCrit)
                {
                    builder.Append(" " + "AdeptusMechanicus.GetsHotCrit".Translate((GetsHotCritChance / 100).ToStringPercent()));
                    if (GetsHotCritExplosion)
                    {
                        builder.AppendLine("AdeptusMechanicus.GetsHotCritExplosion".Translate((GetsHotCritExplosionChance / 100).ToStringPercent()));
                    }
                }
            }
            if (Jams)
            {
                builder.AppendLine();
                StatPart_Reliability.GetReliability(this, out string reliabilityString, out float failChance);
                builder.AppendLine(string.Format("AdeptusMechanicus.Jams".Translate() + ": " + "AdeptusMechanicus.JamsDesc".Translate(parent.Label, reliabilityString, (failChance / 100).ToStringPercent())));
                if (JamsDamageWeapon)
                {
                    builder.Append(" " + "AdeptusMechanicus.JamsWeaponDamage".Translate(JamDamage, parent.def.label));
                }
            }
            if (TwinLinked)
            {
                builder.AppendLine();
                builder.AppendLine("AdeptusMechanicus.TwinLinked".Translate() + ": " + "AdeptusMechanicus.TwinLinkedDesc".Translate());
            }
            if (Multishot)
            {
                builder.AppendLine();
                builder.AppendLine(string.Format("AdeptusMechanicus.Scatter".Translate() + ": " + "AdeptusMechanicus.ScatterDesc".Translate(ScattershotCount)));
            }
            if (Rending)
            {
                builder.AppendLine();
                builder.AppendLine(string.Format("AdeptusMechanicus.Rending_Shot".Translate() + ": " + "AdeptusMechanicus.Rending_ShotDesc".Translate(RendingChance)));
            }
            if (MeltaWeapon)
            {
                builder.AppendLine();
                builder.AppendLine(string.Format("AdeptusMechanicus.Melta".Translate() + ": " + "AdeptusMechanicus.MeltaDesc".Translate(compEquippable.VerbTracker.PrimaryVerb.verbProps.range / 2) + " \n"));
            }
            if (VolkiteWeapon)
            {
                builder.AppendLine();
                builder.AppendLine(string.Format("AdeptusMechanicus.Volkite".Translate() + ": " + "AdeptusMechanicus.VolkiteDesc".Translate(compEquippable.VerbTracker.PrimaryVerb.verbProps.range / 2)));
            }
            if (ConversionWeapon)
            {
                builder.AppendLine();
                builder.AppendLine(string.Format("AdeptusMechanicus.ConversionBeam".Translate() + ": " + "AdeptusMechanicus.ConversionBeamDesc".Translate()));
            }
            if (HaywireWeapon)
            {
                builder.AppendLine();
                builder.AppendLine(string.Format("AdeptusMechanicus.Haywire".Translate() + ": " + "AdeptusMechanicus.HaywireDesc".Translate()));
            }
            /*
            if (GaussWeapon)
            {
                builder.AppendLine();
                builder.AppendLine("Gauss Weapon:");
                builder.AppendLine(string.Format("AdeptusMechanicus.Gauss".Translate() + ": " + "AdeptusMechanicus.GaussDesc".Translate()));
            }
            */
            if (TeslaWeapon)
            {
                builder.AppendLine();
                builder.AppendLine("Tesla Weapon:");
                builder.AppendLine(string.Format("AdeptusMechanicus.Tesla".Translate() + ": " + "AdeptusMechanicus.TeslaDesc".Translate()));
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
