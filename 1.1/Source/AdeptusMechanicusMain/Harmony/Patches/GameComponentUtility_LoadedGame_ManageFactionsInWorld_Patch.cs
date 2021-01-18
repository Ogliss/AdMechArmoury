using System.Linq;
using AdeptusMechanicus.settings;
using HarmonyLib;
using RimWorld;
using Verse;

namespace AdeptusMechanicus.HarmonyInstance
{
    public static class GameComponentUtility_LoadedGame_ManageFactionsInWorld_Patch
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
                {
                    if (!AMAMod.settings.AllowAdeptusAstartes)
                    {
                        return false;
                    }
                    else
                    {
                        if (true)
                        {
                            return false;
                        }
                    }
                }
                if (faction.defName.Contains("OG_Mechanicus_"))
                    return AMAMod.settings.AllowAdeptusMechanicus;
                if (faction.defName.Contains("OG_Militarum_"))
                    return AMAMod.settings.AllowAdeptusMilitarum;
                if (faction.defName.Contains("OG_Sororitas_"))
                    return AMAMod.settings.AllowAdeptusSororitas;
                if (faction.defName.Contains("OG_Chaos_"))
                {
                    if (faction.defName.Contains("Deamon"))
                        return AMAMod.settings.AllowChaosDeamons;
                    if (faction.defName.Contains("Marine"))
                        return AMAMod.settings.AllowChaosMarine;
                    if (faction.defName.Contains("Guard"))
                        return AMAMod.settings.AllowChaosGuard;
                    if (faction.defName.Contains("Mechanicus"))
                        return AMAMod.settings.AllowChaosMechanicus;
                }
                if (faction.defName.Contains("OG_Eldar_"))
                {
                    if (faction.defName.Contains("Craftworld"))
                        return AMAMod.settings.AllowEldarCraftworld;
                    if (faction.defName.Contains("Exodite"))
                        return AMAMod.settings.AllowEldarExodite;
                    if (faction.defName.Contains("Harlequin"))
                        return AMAMod.settings.AllowEldarHarlequinn;
                }
                if (faction.defName.Contains("OG_DarkEldar_"))
                    return AMAMod.settings.AllowDarkEldar;
                if (faction.defName.Contains("OG_Kroot_"))
                    return AMAMod.settings.AllowKroot;
                if (faction.defName.Contains("OG_Tau_"))
                    return AMAMod.settings.AllowTau;
                if (faction.defName.Contains("OG_Necron_"))
                    return AMAMod.settings.AllowNecron;
                if (faction.defName.Contains("OG_Ork_"))
                {
                    if (faction.defName.Contains("OG_Ork_Tek_"))
                        return AMAMod.settings.AllowOrkTek;
                    if (faction.defName.Contains("OG_Ork_Feral_"))
                        return AMAMod.settings.AllowOrkFeral;
                    if (faction.defName.Contains("OG_Ork_Hulk") || faction.defName.Contains("OG_Ork_Rok"))
                        return AMAMod.settings.AllowOrkRok;
                }
                if (faction.defName.Contains("OG_Tyranid_") || faction.defName.Contains("OG_Genestealer_Cult"))
                    return AMAMod.settings.AllowTyranid;
                if (faction.defName.Contains("OG_Vespid_"))
                    return AMAMod.settings.AllowVespid;
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
                    return !AMAMod.settings.AllowAdeptusAstartes;
                if (faction.defName.Contains("OG_Mechanicus_"))
                    return !AMAMod.settings.AllowAdeptusMechanicus;
                if (faction.defName.Contains("OG_Militarum_"))
                    return !AMAMod.settings.AllowAdeptusMilitarum;
                if (faction.defName.Contains("OG_Sororitas_"))
                    return !AMAMod.settings.AllowAdeptusSororitas;
                if (faction.defName.Contains("OG_Chaos_"))
                {
                    if (faction.defName.Contains("Deamon"))
                        return !AMAMod.settings.AllowChaosDeamons;
                    if (faction.defName.Contains("Marine"))
                        return !AMAMod.settings.AllowChaosMarine;
                    if (faction.defName.Contains("Guard"))
                        return !AMAMod.settings.AllowChaosGuard;
                    if (faction.defName.Contains("Mechanicus"))
                        return !AMAMod.settings.AllowChaosMechanicus;
                }
                if (faction.defName.Contains("OG_Eldar_"))
                {
                    if (faction.defName.Contains("Craftworld"))
                        return !AMAMod.settings.AllowEldarCraftworld;
                    if (faction.defName.Contains("Exodite"))
                        return !AMAMod.settings.AllowEldarExodite;
                    if (faction.defName.Contains("Harlequin"))
                        return !AMAMod.settings.AllowEldarHarlequinn;
                }
                if (faction.defName.Contains("OG_DarkEldar_"))
                    return !AMAMod.settings.AllowDarkEldar;
                if (faction.defName.Contains("OG_Kroot_"))
                    return !AMAMod.settings.AllowKroot;
                if (faction.defName.Contains("OG_Tau_"))
                    return !AMAMod.settings.AllowTau;
                if (faction.defName.Contains("OG_Necron_"))
                    return !AMAMod.settings.AllowNecron;
                if (faction.defName.Contains("OG_Ork_"))
                {
                    if (faction.defName.Contains("OG_Ork_Tek_"))
                        return !AMAMod.settings.AllowOrkTek;
                    if (faction.defName.Contains("OG_Ork_Feral_"))
                        return !AMAMod.settings.AllowOrkFeral;
                    if (faction.defName.Contains("OG_Ork_Hulk") || faction.defName.Contains("OG_Ork_Rok"))
                        return !AMAMod.settings.AllowOrkRok;
                }
                if (faction.defName.Contains("OG_Tyranid_") || faction.defName.Contains("OG_Genestealer_Cult"))
                    return !AMAMod.settings.AllowTyranid;
                if (faction.defName.Contains("OG_Vespid_"))
                    return !AMAMod.settings.AllowVespid;
                return true;
            }
            
        }
    }
}
