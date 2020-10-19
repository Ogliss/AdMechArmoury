using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Verse;
using RimWorld;

namespace AdeptusMechanicus.settings
{
    public class AMSettings : ModSettings
    {
        // Armoury Settings;
        public bool ShowArmourySettings = true;

        public bool ArmouryGeneralSpecialRules = false;
        public bool AllowDeepStrike = true;
        public bool AllowInfiltrate = true;

        public bool ShowWeaponSpecialRules = false;
        public bool AllowRapidFire = true;
        public bool AllowGetsHot = true;
        public bool AllowJams = true;
        public bool AllowMultiShot = true;
        public bool AllowUserEffects = true;
        public bool AllowForceWeaponEffect = true;
        public bool AllowRendingMeleeEffect = true;
        public bool AllowRendingRangedEffect = true;

        public bool ShowAllowedWeapons = false;
        public bool AllowImperialWeapons = true;
        public bool AllowMechanicusWeapons = true;
        public bool AllowChaosWeapons = true;
        public bool AllowEldarWeapons = true;
        public bool AllowDarkEldarWeapons = true;
        public bool AllowTauWeapons = true;
        public bool AllowOrkWeapons = true;
        public bool AllowNecronWeapons = true;
        public bool AllowTyranidWeapons = false;

        // Xenobiologis Settings

        public bool ShowXenobiologisSettings = true;
        public bool ShowAllowedRaceSettings = true;
        public bool ForceRelations = true;

        // Imperium Settings

        public bool ShowImperium = false;
        public bool ShowAstartes = false;
        public bool ShowMechanicus = false;
        public bool ShowMilitarum = false;
        public bool ShowSororitas = false;
        public bool ShowInquisition = false;
        public bool AllowAdeptusAstartes = false;
        public float AstartePunchingFactor = 1f, AstarteSplitFactor = 1f, AstarteScale = 1f;
        public bool AstarteUseOrgans, AstarteEasyMode, AstartesMaleOnly, AstartesAgeMatters;
        public bool AllowAdeptusMechanicus = true;
        public bool AllowAdeptusMilitarum = true;
        public bool AllowAdeptusSororitas = false;

        // Playable Imperium Settings
        public IntRange activeChapters;
        public Dictionary<string, SettingHandle> AstartesChapterSettings = new Dictionary<string, SettingHandle>();

        // Chaos Settings

        public bool ShowChaos = false;
        public bool AllowChaosMarine = false;
        public bool AllowChaosGuard = false;
        public bool AllowChaosMechanicus = false;
        public bool AllowWarpstorm = true;
        public bool AllowChaosDeamons = true;
        public bool AllowChaosDeamonicIncursion = true;
        public bool AllowChaosDeamonicInfestation = true;

        // Playable Chaos Settings

        // Eldar Settings

        public bool ShowEldar = false;
        public bool AllowEldarCraftworld = true;
        public bool AllowEldarExodite = false;
        public bool AllowEldarHarlequinn = false;
        public bool AllowEldarWraithguard = true;

        // Playable Eldar Settings

        // Dark Eldar Settings

        public bool ShowDarkEldar = false;
        public bool AllowDarkEldar = false;

        // Playable Dark Eldar Settings


        // Tau Settings

        public bool ShowTau = false;
        public bool AllowTau = true;
        public bool AllowKroot = true;
        public bool AllowKrootAuxiliaries = true;
        public bool AllowVespid = false;
        public bool AllowVespidAuxiliaries = false;
        public bool AllowGueVesaAuxiliaries = false;

        // Playable Tau Settings

        // Orkz Settings

        public bool ShowOrk = false;
        public bool AllowOrkTek = true;
        public bool AllowOrkFeral = true;
        public bool AllowOrkRok = true;

        // Playable Orkz Settings

        public float FungusSpawnChance = 0.05f;
        public string FungusSpawnChanceBuffer;
        public float FungusSquigChance = 1f;
        public string FungusSquigChanceBuffer;
        public float FungusSnotChance = 0.35f;
        public string FungusSnotChanceBuffer;
        public float FungusGrotChance = 0.15f;
        public string FungusGrotChanceBuffer;
        public float FungusOrkChance = 0.075f;
        public string FungusOrkChanceBuffer;

        public float CocoonSpawnChance = 0.25f;
        public string CocoonSpawnChanceBuffer;
        public float CocoonSquigChance = 1f;
        public string CocoonSquigChanceBuffer;
        public float CocoonSnotChance = 0.5f;
        public string CocoonSnotChanceBuffer;
        public float CocoonGrotChance = 0.25f;
        public string CocoonGrotChanceBuffer;
        public float CocoonOrkChance = 0.15f;
        public string CocoonOrkChanceBuffer;

        // Necron Settings

        public bool ShowNecron = false;
        public bool AllowNecron = true;
        public bool AllowNecronMonolith = true;
        public bool AllowNecronWellBeBack = true;

        // Tyranid Settings

        public bool ShowTyranid = false;
        public bool AllowTyranid = false;
        public bool AllowTyranidInfestation = false;

        public AMSettings()
        {
            AMSettings.Instance = this;
        }

        public static AMSettings Instance;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.ShowArmourySettings, "AMA_ShowArmourySettings", false);
            Scribe_Values.Look(ref this.ArmouryGeneralSpecialRules, "AMA_ShowSpecialRules", false);
            Scribe_Values.Look(ref this.AllowDeepStrike, "AMA_AllowRapidFire", true);
            Scribe_Values.Look(ref this.AllowInfiltrate, "AMA_AllowGetsHot", true);

            Scribe_Values.Look(ref this.ShowWeaponSpecialRules, "AMA_ShowWeaponSpecialRules", false);
            Scribe_Values.Look(ref this.AllowRapidFire, "AMA_AllowRapidFire", true);
            Scribe_Values.Look(ref this.AllowGetsHot, "AMA_AllowGetsHot", true);
            Scribe_Values.Look(ref this.AllowJams, "AMA_AllowJams", true);
            Scribe_Values.Look(ref this.AllowMultiShot, "AMA_AllowMultiShot", true);
            Scribe_Values.Look(ref this.AllowUserEffects, "AMA_AllowUserEffects", true);
            Scribe_Values.Look(ref this.AllowForceWeaponEffect, "AMA_AllowForceWeaponEffect", true);
            Scribe_Values.Look(ref this.AllowRendingMeleeEffect, "AMA_AllowRendingMeleeEffect", true);
            Scribe_Values.Look(ref this.AllowRendingRangedEffect, "AMA_AllowRendingRangedEffect", true);

            Scribe_Values.Look(ref this.ShowAllowedWeapons, "AMA_ShowAllowedWeapons", false);
            Scribe_Values.Look(ref this.AllowImperialWeapons, "AMA_AllowImperialWeapons", true);
            Scribe_Values.Look(ref this.AllowMechanicusWeapons, "AMA_AllowMechanicusWeapons", true);
            Scribe_Values.Look(ref this.AllowChaosWeapons, "AMA_AllowChaosWeapons", true);
            Scribe_Values.Look(ref this.AllowEldarWeapons, "AMA_AllowEldarWeapons", true);
            Scribe_Values.Look(ref this.AllowDarkEldarWeapons, "AMA_AllowDarkEldarWeapons", true);
            Scribe_Values.Look(ref this.AllowTauWeapons, "AMA_AllowTauWeapons", true);
            Scribe_Values.Look(ref this.AllowOrkWeapons, "AMA_AllowOrkWeapons", true);
            Scribe_Values.Look(ref this.AllowNecronWeapons, "AMA_AllowNecronWeapons", true);
            Scribe_Values.Look(ref this.AllowTyranidWeapons, "AMA_AllowTyranidWeapons", false);
            
            Scribe_Values.Look(ref this.ForceRelations, "AMXB_ForceRelations", true);
            Scribe_Values.Look(ref this.ShowImperium, "AMXB_ShowImperium", false);
            Scribe_Values.Look(ref this.AllowAdeptusAstartes, "AMXB_AllowAdeptusAstartes", false && AMSettings.Instance.AllowImperialWeapons);
            Scribe_Values.Look(ref this.AllowAdeptusMechanicus, "AMXB_AllowAdeptusMechanicus", true && AMSettings.Instance.AllowMechanicusWeapons);
            Scribe_Values.Look(ref this.AllowAdeptusMilitarum, "AMXB_AllowAdeptusMilitarum", true && AMSettings.Instance.AllowImperialWeapons);
            Scribe_Values.Look(ref this.AllowAdeptusSororitas, "AMXB_AllowAdeptusSororitas", false && AMSettings.Instance.AllowImperialWeapons);
            Scribe_Values.Look(ref this.ShowChaos, "AMXB_ShowChaos", false && AMSettings.Instance.AllowChaosWeapons);
            Scribe_Values.Look(ref this.AllowChaosMarine, "AMXB_AllowChaosMarine", false && AMSettings.Instance.AllowChaosWeapons);
            Scribe_Values.Look(ref this.AllowChaosGuard, "AMXB_AllowChaosGuard", false && AMSettings.Instance.AllowChaosWeapons);
            Scribe_Values.Look(ref this.AllowChaosMechanicus, "AMXB_AllowChaosMechanicus", false && AMSettings.Instance.AllowChaosWeapons);
            Scribe_Values.Look(ref this.AllowWarpstorm, "AMXB_AllowWarpstorm", true);
            Scribe_Values.Look(ref this.AllowChaosDeamons, "AMXB_AllowChaosDeamons", true);
            Scribe_Values.Look(ref this.AllowChaosDeamonicIncursion, "AMXB_AllowChaosDeamonicIncursion", true);
            Scribe_Values.Look(ref this.AllowChaosDeamonicInfestation, "AMXB_AllowChaosDeamonicInfestation", true);
            Scribe_Values.Look(ref this.ShowDarkEldar, "AMXB_ShowDarkEldar", false);
            Scribe_Values.Look(ref this.AllowDarkEldar, "AMXB_AllowDarkEldar", false && AMSettings.Instance.AllowDarkEldarWeapons);
            Scribe_Values.Look(ref this.ShowEldar, "AMXB_ShowEldar", false);
            Scribe_Values.Look(ref this.AllowEldarCraftworld, "AMXB_AllowEldarCraftworld", true && AMSettings.Instance.AllowEldarWeapons);
            Scribe_Values.Look(ref this.AllowEldarExodite, "AMXB_AllowEldarExodite", false && AMSettings.Instance.AllowEldarWeapons);
            Scribe_Values.Look(ref this.AllowEldarHarlequinn, "AMXB_AllowEldarHarlequinn", false && AMSettings.Instance.AllowEldarWeapons);
            Scribe_Values.Look(ref this.AllowEldarWraithguard, "AMXB_AllowEldarWraithguard", true && AMSettings.Instance.AllowEldarWeapons);
            Scribe_Values.Look(ref this.ShowTau, "AMXB_ShowTau", false);
            Scribe_Values.Look(ref this.AllowTau, "AMXB_AllowTau", true && AMSettings.Instance.AllowTauWeapons);
            Scribe_Values.Look(ref this.AllowGueVesaAuxiliaries, "AMXB_AllowGueVesaAuxiliaries", true && AMSettings.Instance.AllowTauWeapons);
            Scribe_Values.Look(ref this.AllowKrootAuxiliaries, "AMXB_AllowKrootAuxiliaries", true && AMSettings.Instance.AllowTauWeapons);
            Scribe_Values.Look(ref this.AllowKroot, "AMXB_AllowKroot", true && AMSettings.Instance.AllowTauWeapons);
            Scribe_Values.Look(ref this.AllowVespidAuxiliaries, "AMXB_AllowVespidAuxiliaries", false && AMSettings.Instance.AllowTauWeapons);
            Scribe_Values.Look(ref this.AllowVespid, "AMXB_AllowVespid", false && AMSettings.Instance.AllowTauWeapons);
            Scribe_Values.Look(ref this.ShowNecron, "AMXB_ShowNecron", true);
            Scribe_Values.Look(ref this.AllowNecron, "AMXB_AllowNecron", true && AMSettings.Instance.AllowNecronWeapons);
            Scribe_Values.Look(ref this.AllowNecronMonolith, "AMXB_AllowNecronMonolith", true && AMSettings.Instance.AllowNecronWeapons);
            Scribe_Values.Look(ref this.AllowNecronWellBeBack, "AMXB_AllowNecronWellBeBack", true && AMSettings.Instance.AllowNecronWeapons);
            Scribe_Values.Look(ref this.ShowTyranid, "AMXB_ShowTyranid", false && AMSettings.Instance.AllowTyranidWeapons);
            Scribe_Values.Look(ref this.AllowTyranid, "AMXB_AllowTyranid", false && AMSettings.Instance.AllowTyranidWeapons);
            Scribe_Values.Look(ref this.AllowTyranidInfestation, "AMXB_AllowTyranidInfestation", false && AMSettings.Instance.AllowTyranidWeapons && AMSettings.Instance.AllowTyranid);
            
            // Astartes Data
            Scribe_Values.Look(ref this.AstarteEasyMode, "AMAA_AstarteEasyMode", false);
            Scribe_Values.Look(ref this.AstartesAgeMatters, "AMAA_AstartesAgeMatters", true);
            Scribe_Values.Look(ref this.AstartesMaleOnly, "AMAA_AstartesMaleOnly", true);
            Scribe_Values.Look(ref this.AstarteUseOrgans, "AMAA_AstarteUseOrgans", true);

            // Astartes Playable Race Extras
            Scribe_Values.Look(ref this.ShowAstartes, "AMAA_ShowAstartes", true);
            Scribe_Values.Look(ref this.AstartePunchingFactor, "AMAA_AstartePunchingFactor", 1f);
            Scribe_Values.Look(ref this.AstarteSplitFactor, "AMAA_AstarteSplitFactor", 1f);
            Scribe_Values.Look(ref this.AstarteScale, "AMAA_AstarteScale", 1f);
            Scribe_Values.Look(ref this.AstarteUseOrgans, "AMAA_AstarteUseOrgans", true);
            Scribe_Values.Look(ref this.AstarteEasyMode, "AMAA_Astartesetting2", false);
            Scribe_Values.Look(ref this.activeChapters, "activeChapters", new IntRange(1, 5));
            Scribe_Collections.Look<string, SettingHandle>(ref this.AstartesChapterSettings, "AstartesChapterSettings"/*, LookMode.Def, LookMode.Value, ref RaceKeyWorkingList, ref RaceValueWorkingList*/);

            // Tau Data

            // Tau Playable Race Extras

            // Ork Data
            Scribe_Values.Look(ref this.ShowOrk, "AMXB_AllowOrk", true);
            Scribe_Values.Look(ref this.AllowOrkTek, "AMXB_AllowOrkTek", true && AMSettings.Instance.AllowOrkWeapons);
            Scribe_Values.Look(ref this.AllowOrkFeral, "AMXB_AllowOrkFeral", true && AMSettings.Instance.AllowOrkWeapons);
            Scribe_Values.Look(ref this.AllowOrkRok, "AMXB_AllowOrkRok", true && AMSettings.Instance.AllowOrkWeapons);

            // Orkz Playable Race Extras
            Scribe_Values.Look(ref this.FungusSpawnChance, "AMO_FungusSpawnChance", 0.05f);
            Scribe_Values.Look(ref this.FungusSpawnChanceBuffer, "AMO_FungusSpawnChanceBuffer", string.Empty);
            Scribe_Values.Look(ref this.FungusSnotChance, "AMO_FungusSnotChance", 0.35f);
            Scribe_Values.Look(ref this.FungusSnotChanceBuffer, "AMO_FungusSnotChanceBuffer", string.Empty);
            Scribe_Values.Look(ref this.FungusGrotChance, "AMO_FungusGrotChance", 0.15f);
            Scribe_Values.Look(ref this.FungusGrotChanceBuffer, "AMO_FungusGrotChanceBuffer", string.Empty);
            Scribe_Values.Look(ref this.FungusOrkChance, "AMO_FungusOrkChance", 0.075f);
            Scribe_Values.Look(ref this.FungusOrkChanceBuffer, "AMO_FungusOrkChanceBuffer", string.Empty);

            Scribe_Values.Look(ref this.CocoonSpawnChance, "AMO_CocoonSpawnChance", 0.25f);
            Scribe_Values.Look(ref this.CocoonSpawnChanceBuffer, "AMO_CocoonSpawnChanceBuffer", string.Empty);
            Scribe_Values.Look(ref this.CocoonSnotChance, "AMO_CocoonSnotChance", 0.5f);
            Scribe_Values.Look(ref this.CocoonSnotChanceBuffer, "AMO_CocoonSnotChanceBuffer", string.Empty);
            Scribe_Values.Look(ref this.CocoonGrotChance, "AMO_CocoonGrotChance", 0.25f);
            Scribe_Values.Look(ref this.CocoonGrotChanceBuffer, "AMO_CocoonGrotChanceBuffer", string.Empty);
            Scribe_Values.Look(ref this.CocoonOrkChance, "AMO_CocoonOrkChance", 0.15f);
            Scribe_Values.Look(ref this.CocoonOrkChanceBuffer, "AMO_CocoonOrkChanceBuffer", string.Empty);
        }


    }
    public abstract class SettingHandle : IExposable
    {
        public virtual void ExposeData()
        {

        }
    }
}