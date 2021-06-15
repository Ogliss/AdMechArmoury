using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using Verse.Sound;
using AdeptusMechanicus;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{
    
//    [HarmonyPatch(typeof(EquipmentUtility), "CanEquip", new Type[]  {typeof(Thing), typeof(Pawn), typeof(string)})]
    public static class EquipmentUtility_CanEquip_Restricted_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Thing thing, Pawn pawn, ref string cantReason, ref bool __result)
        {
            if (__result)
            {
                if (thing.def.IsApparel)
                {
                    if (thing.def.HasModExtension<ApparelRestrictionDefExtension>())
                    {
                        ApparelRestrictionDefExtension defExtension = thing.def.GetModExtensionFast<ApparelRestrictionDefExtension>();
                        if (defExtension != null)
                        {
                            bool gender = defExtension.gender == Gender.None || pawn.gender == defExtension.gender;
                            bool race = defExtension.RaceDefs.NullOrEmpty();
                            bool apparel = defExtension.ApparelDefs.NullOrEmpty();
                            bool hediff = defExtension.HediffDefs.NullOrEmpty();
                            bool trait = defExtension.TraitDefs.NullOrEmpty();
                            bool bodytype = defExtension.BodytypeDefs.NullOrEmpty() || defExtension.forcedBodytype;
                            if (!bodytype)
                            {
                                bodytype = defExtension.BodytypeDefs.Contains(pawn.story.bodyType);
                                if (bodytype && defExtension.Any)
                                {
                                    __result = true;
                                    return;
                                }
                                cantReason = "Wrong Bodytype: ";
                                for (int i = 0; i < defExtension.BodytypeDefs.Count; i++)
                                {
                                    if (i > 0)
                                    {
                                        cantReason += ", ";
                                    }
                                    cantReason += defExtension.BodytypeDefs[i].LabelCap;
                                }
                            }
                            if (!defExtension.RaceDefs.NullOrEmpty())
                            {
                                race = defExtension.RaceDefs.Contains(pawn.def);
                                if (race && defExtension.Any)
                                {
                                    __result = true;
                                    return;
                                }
                                cantReason = "Wrong Race: ";
                                for (int i = 0; i < defExtension.RaceDefs.Count; i++)
                                {
                                    if (i > 0)
                                    {
                                        cantReason += ", ";
                                    }
                                    cantReason += defExtension.RaceDefs[i].LabelCap;
                                }
                            }
                            if (!defExtension.ApparelDefs.NullOrEmpty())
                            {
                                if (pawn.apparel?.WornApparelCount>0)
                                {
                                    foreach (var item in defExtension.ApparelDefs)
                                    {
                                        apparel = pawn.apparel.WornApparel.Any(x=> x.def == item);
                                        if (apparel)
                                        {
                                            break;
                                        }
                                    }
                                    if (apparel && defExtension.Any)
                                    {
                                        __result = true;
                                        return;
                                    }
                                }
                                cantReason = "Missing required Apparel: ";
                                for (int i = 0; i < defExtension.ApparelDefs.Count; i++)
                                {
                                    if (i > 0)
                                    {
                                        cantReason += ", ";
                                    }
                                    cantReason += defExtension.ApparelDefs[i].LabelCap;
                                }
                            }
                            if (!defExtension.HediffDefs.NullOrEmpty())
                            {
                                hediff = pawn.health.hediffSet.hediffs.Any(x => defExtension.HediffDefs.Contains(x.def));
                                if (hediff && defExtension.Any)
                                {
                                    __result = true;
                                    return;
                                }
                                cantReason = "Missing required Implant: ";
                                for (int i = 0; i < defExtension.HediffDefs.Count; i++)
                                {
                                    if (i > 0)
                                    {
                                        cantReason += ", ";
                                    }
                                    cantReason += defExtension.HediffDefs[i].LabelCap;
                                }
                            }
                            if (!defExtension.TraitDefs.NullOrEmpty())
                            {
                                trait = pawn.story.traits.allTraits.Any(x => defExtension.TraitDefs.Contains(x.def));
                                if (trait && defExtension.Any)
                                {
                                    __result = true;
                                    return;
                                }
                                cantReason = "Missing required Trait: ";
                                for (int i = 0; i < defExtension.TraitDefs.Count; i++)
                                {
                                    if (i > 0)
                                    {
                                        cantReason += ", ";
                                    }
                                    cantReason += defExtension.TraitDefs[i].LabelCap;
                                }
                            }
                            __result = gender && (race && hediff && trait && apparel && bodytype);
                            if (!__result)
                            {
                                cantReason = $"{pawn.NameShortColored} "+ cantReason;

                                return;
                            }
                        }
                    }
                }

            }
        }
    }
    
}
