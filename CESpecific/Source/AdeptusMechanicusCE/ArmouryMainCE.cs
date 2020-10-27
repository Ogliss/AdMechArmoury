using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using AdeptusMechanicus;

namespace AdeptusMechanicus
{
    [StaticConstructorOnStartup]
    public class ArmouryMainCE
    {
        static ArmouryMainCE()
        {
            /*
            Log.Message("AppDomain.CurrentDomain.GetAssemblies():\n" + System.AppDomain.CurrentDomain.GetAssemblies().Join(delimiter: "\n"));
            Log.Message("GenTypes.AllActiveAssemblies:\n" + Traverse.Create(typeof(GenTypes)).Property<IEnumerable<System.Reflection.Assembly>>("AllActiveAssemblies").Value.Join(delimiter: "\n"));
            */
             //   Log.Message("ArmouryMainCE ");
            
            if (DefDatabase<ScenarioDef>.AllDefs.Any(x => x.defName.StartsWith("OGAM_TestScenario_")))
            {
                foreach (ScenarioDef ScenDef in DefDatabase<ScenarioDef>.AllDefs.Where(x => x.defName.StartsWith("OGAM_TestScenario_")))
                {
                    TryAddAmmoToScenario(ScenDef);
                }
            }
            if (DefDatabase<ScenarioDef>.AllDefs.Any(x => x.defName.StartsWith("OGAM_Scenario_")))
            {
                foreach (ScenarioDef ScenDef in DefDatabase<ScenarioDef>.AllDefs.Where(x => x.defName.StartsWith("OGAM_Scenario_")))
                {
                    TryAddAmmoToScenario(ScenDef);
                }
            }
            
        }

        public static void TryAddAmmoToScenario(ScenarioDef ScenDef)
        {
            List<ScenPart> parts = Traverse.Create(ScenDef.scenario).Field("parts").GetValue<List<ScenPart>>();
            List<ScenPart> ammoParts = new List<ScenPart>();
            foreach (ScenPart scenpart in parts.Where(x => x.def == ScenPartDefOf.StartingThing_Defined))
            {
                ThingDef td = Traverse.Create(scenpart).Field("thingDef").GetValue<ThingDef>();
                if (td.IsWeapon)
                {
                    if (td.IsWeaponUsingProjectiles || td.IsRangedWeapon)
                    {
                        ThingDef Weapon = td;
                        if (AdeptusIntergrationUtil.enabled_CombatExtended && Weapon.IsRangedWeapon)
                        {
                            if (Weapon.comps.Any(x => x.GetType().ToString().Contains("CompProperties_AmmoUser")))
                            {
                                int count = ScenDef.defName.StartsWith("OGAM_TestScenario_") ? -1 : 100;
                                AddAmmoCE(Weapon, ref ammoParts, count, 1, false); // Prefs.DevMode
                            }
                        }
                    }
                }
            }
            if (!ammoParts.NullOrEmpty())
            {
                parts.AddRange(ammoParts);
            }
        }
        public static void AddAmmoCE(ThingDef Weapon, ref List<ScenPart> parts, int count = -1, int mult = 1, bool logging = false)
        {
            if (logging) Log.Message("Trying to add ammo for " + Weapon);
            CombatExtended.CompProperties_AmmoUser ammo = Weapon.GetCompProperties<CombatExtended.CompProperties_AmmoUser>();
            foreach (var item in ammo.ammoSet.ammoTypes)
            {
                CombatExtended.AmmoDef ammoDef = item.ammo;
                if (count == -1)
                {
                    bool hasammo = false;
                    foreach (ScenPart scenpart in parts.Where(x => x.def == ScenPartDefOf.StartingThing_Defined))
                    {
                        ThingDef td = Traverse.Create(scenpart).Field("thingDef").GetValue<ThingDef>();
                        if (td == ammoDef)
                        {
                            hasammo = true;
                            break;
                        }
                    }
                    if (hasammo)
                    {
                        continue;
                    }
                }
                int c = count == -1 ? item.ammo.stackLimit : Math.Min(count, item.ammo.stackLimit);
                if (logging) Log.Message("Trying to add " + mult + " stacks of " + ammoDef + " X " + c);
                for (int i = 0; i < mult; i++)
                {
                    ScenPart_StartingThing_Define _Defined = new ScenPart_StartingThing_Define() { def = ScenPartDefOf.StartingThing_Defined };
                    _Defined.ThingDef = ammoDef;
                    _Defined.Count = c;
                    if (ammoDef.MadeFromStuff)
                    {
                        ThingDef stuffdef = DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.IsStuff && ammoDef.stuffCategories.Any(y => x.stuffProps.categories.Contains(y))).RandomElement();
                        if (stuffdef != null)
                        {
                            _Defined.ThingDefStuff = stuffdef;
                        }
                    }
                    parts.Add(_Defined);
                    if (count != -1)
                    {
                        break;
                    }
                }
            }
        }

    }
}