using System.Linq;
using AdeptusMechanicus.settings;
using HarmonyLib;
using RimWorld;
using Verse;

namespace AdeptusMechanicus.HarmonyInstance
{
    public static class Patch_GameComponentUtility
    {
        [HarmonyPatch(typeof(GameComponentUtility), nameof(GameComponentUtility.LoadedGame))]
        public static class LoadedGame
        {
            public static void Postfix()
            {
                LongEventHandler.ExecuteWhenFinished(OnGameLoaded);
            }

            private static void OnGameLoaded()
            {
                if (Current.Game == null) return;

                var enabledFactionEnumerator = DefDatabase<FactionDef>.AllDefs.Where(EnabledFactionValidator).GetEnumerator();
                if (enabledFactionEnumerator.MoveNext())
                {
                    // Only one dialog can be stacked at a time, so give it the list of all factions
                    Dialog_FactionSpawning.OpenDialog(enabledFactionEnumerator);
                }
                /*
                var disabledFactionEnumerator = Find.FactionManager.AllFactions.Where(DisabledFactionValidator).GetEnumerator();
                if (disabledFactionEnumerator.MoveNext())
                {
                    // Only one dialog can be stacked at a time, so give it the list of all factions
                    Dialog_FactionSpawning.OpenDialog(disabledFactionEnumerator);
                }
                */
            }

            private static bool EnabledFactionValidator(FactionDef faction)
            {
                if (faction == null) return false;
                if (faction.isPlayer) return false;
                var count = Find.FactionManager.AllFactions.Count(f => f.def == faction);
                if (count > 0) return false;
                if (!faction.defName.StartsWith("OG_")) return false;
                if (Find.World?.GetComponent<FactionSpawningState>()?.IsIgnored(faction) == true) return false;
                if (FactionSpawningUtility.NeverSpawn(faction)) return false;

                if (faction.defName.Contains("OG_Astartes_"))
                    return SettingsHelper.latest.AllowAdeptusAstartes;
                if (faction.defName.Contains("OG_Mechanicus_"))
                    return SettingsHelper.latest.AllowAdeptusMechanicus;
                if (faction.defName.Contains("OG_Militarum_"))
                    return SettingsHelper.latest.AllowAdeptusMilitarum;
                if (faction.defName.Contains("OG_Sororitas_"))
                    return SettingsHelper.latest.AllowAdeptusSororitas;
                if (faction.defName.Contains("OG_Chaos_"))
                {
                    if (faction.defName.Contains("Deamon"))
                        return SettingsHelper.latest.AllowChaosDeamons;
                    if (faction.defName.Contains("Marine"))
                        return SettingsHelper.latest.AllowChaosMarine;
                    if (faction.defName.Contains("Guard"))
                        return SettingsHelper.latest.AllowChaosGuard;
                    if (faction.defName.Contains("Mechanicus"))
                        return SettingsHelper.latest.AllowChaosMechanicus;
                }
                if (faction.defName.Contains("OG_Eldar_"))
                {
                    if (faction.defName.Contains("Craftworld"))
                        return SettingsHelper.latest.AllowEldarCraftworld;
                    if (faction.defName.Contains("Exodite"))
                        return SettingsHelper.latest.AllowEldarExodite;
                    if (faction.defName.Contains("Harlequin"))
                        return SettingsHelper.latest.AllowEldarHarlequinn;
                }
                if (faction.defName.Contains("OG_Dark_Eldar_"))
                    return SettingsHelper.latest.AllowDarkEldar;
                if (faction.defName.Contains("OG_Kroot_"))
                    return SettingsHelper.latest.AllowKroot;
                if (faction.defName.Contains("OG_Tau_"))
                    return SettingsHelper.latest.AllowTau;
                if (faction.defName.Contains("OG_Necron_"))
                    return SettingsHelper.latest.AllowNecron;
                if (faction.defName.Contains("OG_Ork_"))
                {
                    if (faction.defName.Contains("OG_Ork_Tek_"))
                        return SettingsHelper.latest.AllowOrkTek;
                    if (faction.defName.Contains("OG_Ork_Feral_"))
                        return SettingsHelper.latest.AllowOrkFeral;
                    if (faction.defName.Contains("OG_Ork_Hulk") || faction.defName.Contains("OG_Ork_Rok"))
                        return SettingsHelper.latest.AllowOrkRok;
                }
                if (faction.defName.Contains("OG_Tyranid_") || faction.defName.Contains("OG_Genestealer_Cult"))
                    return SettingsHelper.latest.AllowTyranid;
                if (faction.defName.Contains("OG_Vespid_"))
                    return SettingsHelper.latest.AllowVespid;
                return true;
            }
            
            private static bool DisabledFactionValidator(Faction f)
            {
                FactionDef faction = f.def;
                if (faction == null) return false;
                if (faction.isPlayer) return false;
                var count = Find.FactionManager.AllFactions.Count(f => f.def == faction);
                if (count < 1) return false;
                if (!faction.defName.StartsWith("OG_")) return false;
                if (Find.World?.GetComponent<FactionSpawningState>()?.IsIgnored(faction) == true) return false;
                if (FactionSpawningUtility.NeverSpawn(faction)) return false;

                if (faction.defName.Contains("OG_Astartes_"))
                    return !SettingsHelper.latest.AllowAdeptusAstartes;
                if (faction.defName.Contains("OG_Mechanicus_"))
                    return !SettingsHelper.latest.AllowAdeptusMechanicus;
                if (faction.defName.Contains("OG_Militarum_"))
                    return !SettingsHelper.latest.AllowAdeptusMilitarum;
                if (faction.defName.Contains("OG_Sororitas_"))
                    return !SettingsHelper.latest.AllowAdeptusSororitas;
                if (faction.defName.Contains("OG_Chaos_"))
                {
                    if (faction.defName.Contains("Deamon"))
                        return !SettingsHelper.latest.AllowChaosDeamons;
                    if (faction.defName.Contains("Marine"))
                        return !SettingsHelper.latest.AllowChaosMarine;
                    if (faction.defName.Contains("Guard"))
                        return !SettingsHelper.latest.AllowChaosGuard;
                    if (faction.defName.Contains("Mechanicus"))
                        return !SettingsHelper.latest.AllowChaosMechanicus;
                }
                if (faction.defName.Contains("OG_Eldar_"))
                {
                    if (faction.defName.Contains("Craftworld"))
                        return !SettingsHelper.latest.AllowEldarCraftworld;
                    if (faction.defName.Contains("Exodite"))
                        return !SettingsHelper.latest.AllowEldarExodite;
                    if (faction.defName.Contains("Harlequin"))
                        return !SettingsHelper.latest.AllowEldarHarlequinn;
                }
                if (faction.defName.Contains("OG_Dark_Eldar_"))
                    return !SettingsHelper.latest.AllowDarkEldar;
                if (faction.defName.Contains("OG_Kroot_"))
                    return !SettingsHelper.latest.AllowKroot;
                if (faction.defName.Contains("OG_Tau_"))
                    return !SettingsHelper.latest.AllowTau;
                if (faction.defName.Contains("OG_Necron_"))
                    return !SettingsHelper.latest.AllowNecron;
                if (faction.defName.Contains("OG_Ork_"))
                {
                    if (faction.defName.Contains("OG_Ork_Tek_"))
                        return !SettingsHelper.latest.AllowOrkTek;
                    if (faction.defName.Contains("OG_Ork_Feral_"))
                        return !SettingsHelper.latest.AllowOrkFeral;
                    if (faction.defName.Contains("OG_Ork_Hulk") || faction.defName.Contains("OG_Ork_Rok"))
                        return !SettingsHelper.latest.AllowOrkRok;
                }
                if (faction.defName.Contains("OG_Tyranid_") || faction.defName.Contains("OG_Genestealer_Cult"))
                    return !SettingsHelper.latest.AllowTyranid;
                if (faction.defName.Contains("OG_Vespid_"))
                    return !SettingsHelper.latest.AllowVespid;
                return true;
            }
            
        }
    }
}
