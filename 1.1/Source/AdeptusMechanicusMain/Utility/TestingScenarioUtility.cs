using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AdeptusMechanicus
{
    class TestingScenarioUtility
    {
        public static void SetUpTestScenarios(List<ScenarioDef> scenariosTesting)
        {

            foreach (ScenarioDef ScenDef in scenariosTesting)
            {
                if (ScenDef.defName.Contains("Imperial"))
                {
                    TryAddWeaponsToScenario(ScenDef, "I");
                    TryAddWeaponsToScenario(ScenDef, "IG");

                }
                else if (ScenDef.defName.Contains("Mechanicus"))
                {
                    TryAddWeaponsToScenario(ScenDef, "AM");
                }
                else if (ScenDef.defName.Contains("Astartes"))
                {
                    TryAddWeaponsToScenario(ScenDef, "AA");
                }
                else if (ScenDef.defName.Contains("Chaos"))
                {
                    TryAddWeaponsToScenario(ScenDef, "C");
                }
                else if (ScenDef.defName.Contains("Eldar"))
                {
                    if (ScenDef.defName.Contains("DarkEldar"))
                    {
                        TryAddWeaponsToScenario(ScenDef, "DE");
                    }
                    else TryAddWeaponsToScenario(ScenDef, "E");
                }
                else if (ScenDef.defName.Contains("Tau"))
                {
                    TryAddWeaponsToScenario(ScenDef, "T");
                    TryAddWeaponsToScenario(ScenDef, "K");
                    TryAddWeaponsToScenario(ScenDef, "V");
                }
                else if (ScenDef.defName.Contains("Ork"))
                {
                    TryAddWeaponsToScenario(ScenDef, "O");
                }
                else if (ScenDef.defName.Contains("Necron"))
                {
                    TryAddWeaponsToScenario(ScenDef, "N");
                }
                else if (ScenDef.defName.Contains("Tyranid"))
                {
                    TryAddWeaponsToScenario(ScenDef, "TY");
                }
            }
        }
        public static void TryAddWeaponsToScenario(ScenarioDef ScenDef, string Tag)
        {
            List<ThingDef> things = DefDatabase<ThingDef>.AllDefsListForReading.FindAll(x => ((x.PlayerAcquirable || Tag == "TY") && (x.IsWeapon || x.IsApparel) && x.defName.Contains("OG" + Tag) && !x.defName.Contains("TOGGLEDEF_S")));
            foreach (ThingDef Weapon in things)
            {
                bool hasweapon = false;
                List<ScenPart> parts = Traverse.Create(ScenDef.scenario).Field("parts").GetValue<List<ScenPart>>();
                foreach (ScenPart scenpart in parts.Where(x => x.def == ScenPartDefOf.StartingThing_Defined))
                {
                    ThingDef td = Traverse.Create(scenpart).Field("thingDef").GetValue<ThingDef>();
                    if (td == Weapon)
                    {
                        hasweapon = true;
                        break;
                    }
                }
                if (hasweapon)
                {
                    continue;
                }
                ScenPart_StartingThing_Define StartingThing = new ScenPart_StartingThing_Define() { def = ScenPartDefOf.StartingThing_Defined };
                if (Weapon.MadeFromStuff)
                {
                    ThingDef stuffdef = DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.IsStuff && Weapon.stuffCategories.Any(y => x.stuffProps.categories.Contains(y))).RandomElement();
                    if (stuffdef != null)
                    {
                        StartingThing.ThingDefStuff = stuffdef;
                    }
                }
                StartingThing.ThingDef = Weapon;
                parts.Add(StartingThing);
            }
        }

    }
}
