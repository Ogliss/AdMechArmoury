using AdeptusMechanicus.ExtensionMethods;
using AdeptusMechanicus.Lasers;
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
                else if (ScenDef.defName.Contains("SpecialMelee"))
                {
                    TryAddRendingWeaponsToScenario(ScenDef);
                    TryAddPowerWeaponsToScenario(ScenDef);
                    TryAddForceWeaponsToScenario(ScenDef);
                }
                else if (ScenDef.defName.Contains("Laser"))
                {
                    List<string> tags = new List<string>()
                    {
                        "Las",
                        "Lance",
                        "lance",
                        "Volkite"
                    };
                    TryAddTestingWeaponsToScenario(ScenDef, tags);
                }
                else if (ScenDef.defName.Contains("Bolt"))
                {
                    List<string> tags = new List<string>()
                    {
                        "Bolt"
                    };
                    TryAddTestingWeaponsToScenario(ScenDef, tags);
                }
                else if (ScenDef.defName.Contains("Auto"))
                {
                    List<string> tags = new List<string>()
                    {
                        "Auto",
                        "Stubber"
                    };
                    TryAddTestingWeaponsToScenario(ScenDef, tags);
                }
                else if (ScenDef.defName.Contains("Melta"))
                {
                    List<string> tags = new List<string>() 
                    {
                        "Melta",
                        "Fusion"
                    };
                    TryAddTestingWeaponsToScenario(ScenDef, tags);
                }
                else if (ScenDef.defName.Contains("Flamer"))
                {
                    List<string> tags = new List<string>()
                    {
                        "Flamer",
                        "Phosphur"
                    };
                    TryAddTestingWeaponsToScenario(ScenDef, tags);
                }
                else if (ScenDef.defName.Contains("Plasma"))
                {
                    List<string> tags = new List<string>()
                    {
                        "Plasma"
                    };
                    TryAddTestingWeaponsToScenario(ScenDef, tags);
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
        
        public static void TryAddPowerWeaponsToScenario(ScenarioDef ScenDef)
        {
            List<ThingDef> things = DefDatabase<ThingDef>.AllDefsListForReading.FindAll(x => x.IsWeapon && x.powerWeapon());
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
        
        public static void TryAddForceWeaponsToScenario(ScenarioDef ScenDef)
        {
            List<ThingDef> things = DefDatabase<ThingDef>.AllDefsListForReading.FindAll(x => x.IsWeapon && (x.forceWeapon() || x.witchbladeWeapon()));
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
        
        public static void TryAddRendingWeaponsToScenario(ScenarioDef ScenDef)
        {
            List<ThingDef> things = DefDatabase<ThingDef>.AllDefsListForReading.FindAll(x => x.IsWeapon && x.rendingWeapon());
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
        
        public static void TryAddLaserWeaponsToScenario(ScenarioDef ScenDef)
        {
            List<ThingDef> things = DefDatabase<ThingDef>.AllDefsListForReading.FindAll(x => x.IsRangedWeapon && x.Verbs.Any(z=> z.defaultProjectile as LaserBeamDef != null));
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
        
        public static void TryAddTestingWeaponsToScenario(ScenarioDef ScenDef, List<string> tags)
        {
            List<ThingDef> things = DefDatabase<ThingDef>.AllDefsListForReading.FindAll(x => x.IsRangedWeapon && x.defName.StartsWith("OG") && (tags.Any(y => x.weaponTags.ToCommaList().Contains(y)) || tags.Any(y=> x.defName.Contains(y))));
            List<ScenPart> parts = Traverse.Create(ScenDef.scenario).Field("parts").GetValue<List<ScenPart>>();
            foreach (ThingDef Weapon in things)
            {
                bool hasweapon = false;
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
