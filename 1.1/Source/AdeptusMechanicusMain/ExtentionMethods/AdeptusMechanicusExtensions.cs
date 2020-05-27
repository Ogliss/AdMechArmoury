using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus.ExtensionMethods
{

    public static class AdeptusMechanicusExtensions
    {
        public static void GainEquipmentAbility(this Pawn_AbilityTracker tracker, EquipmentAbilityDef def, ThingWithComps thing)
        {
            if (!tracker.abilities.Any((Ability a) => a.def == def))
            {
                EquipmentAbility ab = Activator.CreateInstance(def.abilityClass, new object[]
                {
                    tracker.pawn,
                    def
                }) as EquipmentAbility;
                ab.sourceEquipment = thing;
                tracker.abilities.Add(ab);
            }
        }

        public static bool ButtonTextLine(this Listing_Standard L, string label, string highlightTag = null)
        {
            Rect rect = L.GetRect(Text.LineHeight);
            bool result = Widgets.ButtonText(rect, label, true, true, true);
            if (highlightTag != null)
            {
                UIHighlighter.HighlightOpportunity(rect, highlightTag);
            }
            L.Gap(L.verticalSpacing);
            return result;
        }

        // Token: 0x06001B7B RID: 7035 RVA: 0x000A80E6 File Offset: 0x000A62E6
        public static void TextFieldNumericLabeled<T>(this Listing_Standard L, string label, ref T val, ref string buffer, float min = 0f, float max = 1E+09f, string tooltip = null, float textpart = 0.75f, float boxpart = 0.25f) where T : struct
        {
            TextFieldNumericLabeled<T>(L.GetRect(Text.LineHeight), label, ref val, ref buffer, min, max, tooltip, textpart, boxpart);
            L.Gap(L.verticalSpacing);
        }
        public static void TextFieldNumericLabeled<T>(Rect rect, string label, ref T val, ref string buffer, float min = 0f, float max = 1E+09f, string tooltip = null, float textpart = 0.75f, float boxpart = 0.25f) where T : struct
        {
            Rect rect2 = rect.LeftPart(textpart).Rounded();
            Rect rect3 = rect.RightPart(boxpart).Rounded();
            TextAnchor anchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect2, label);
            if (tooltip != null)
            {
                TooltipHandler.TipRegion(rect2, tooltip);
            }
            Text.Anchor = anchor;
            Widgets.TextFieldNumeric(rect3, ref val, ref buffer, min, max);
        }

        // Token: 0x06001B47 RID: 6983 RVA: 0x000A6A5C File Offset: 0x000A4C5C
        public static Listing_Standard BeginSection(this Listing_Standard L, float height, bool hidesection = false, int type = 0)
        {
            Rect rect = L.GetRect(height + 8f);
            if (!hidesection) 
            {
                switch (type)
                {
                    case 1:
                        Widgets.DrawWindowBackground(rect);
                        break;
                    case 2:
                        Widgets.DrawWindowBackgroundTutor(rect);
                        break;
                    case 3:
                        Widgets.DrawOptionUnselected(rect);
                        break;
                    case 4:
                        Widgets.DrawOptionSelected(rect);
                        break;
                    default:
                        Widgets.DrawMenuSection(rect);
                        break;
                }
            }
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(rect.ContractedBy(4f));
            return listing_Standard;
        }

        public static void CheckboxLabeled(this Listing_Standard listing_Standard, string label, ref bool checkOn, string tooltip = null, bool disabled = false, bool highlight = false)
        {
            float lineHeight = Text.LineHeight;
            Rect rect = listing_Standard.GetRect(lineHeight);
            if (!tooltip.NullOrEmpty() || highlight)
            {
                if (Mouse.IsOver(rect))
                {
                    Widgets.DrawHighlight(rect);
                }
                if (!tooltip.NullOrEmpty()) TooltipHandler.TipRegion(rect, tooltip);
            }
            Widgets.CheckboxLabeled(rect, label, ref checkOn, disabled, null, null, false);
            listing_Standard.Gap(listing_Standard.verticalSpacing);
        }
        /*
        // Token: 0x060051F6 RID: 20982 RVA: 0x0025FD78 File Offset: 0x0025E178
        public static Graphic ExtractInnerGraphicFor(this Graphic outerGraphic, Thing thing)
        {
            Graphic_RandomRotated graphic_RandomRotated = outerGraphic as Graphic_RandomRotated;
            if (graphic_RandomRotated != null)
            {
                Graphic_Random graphic_Random = outerGraphic as Graphic_Random;
                Log.Message("graphic_Random SubGraphicFor");
                return graphic_Random.SubGraphicFor(thing);
            }
            Log.Message("outerGraphic");
            return outerGraphic;
        }
        public static Graphic ExtractRandomGraphicFor(this Graphic outerGraphic, Thing thing)
        {
            Log.Message("ExtractRandomGraphicFor");
            Log.Message(string.Format("{0}", outerGraphic.ExtractInnerGraphicFor(thing)));
            Graphic_RandomRotated graphic_RandomRotated = outerGraphic as Graphic_RandomRotated;
            if (graphic_RandomRotated != null)
            {
                Log.Message("ExtractRandomGraphicFor graphic_RandomRotated not null");
                if (graphic_RandomRotated.ExtractInnerGraphicFor(thing)!=null)
                {
                    Log.Message("ExtractRandomGraphicFor graphic_RandomRotated ExtractInnerGraphicFor not null");
                    Log.Message(string.Format("{0}", graphic_RandomRotated.ExtractInnerGraphicFor(thing)));
                    Graphic_Random graphic_RandomItem = graphic_RandomRotated.ExtractInnerGraphicFor(thing) as Graphic_Random;
                    if (graphic_RandomItem!=null)
                    {
                        Log.Message("ExtractRandomGraphicFor graphic_RandomItem not null");
                        if (thing.def.thingClass == typeof(ThingWithCompsRandomGraphic))
                        {
                            Log.Message("ExtractRandomGraphicFor ThingWithCompsRandomGraphic");
                            ThingWithCompsRandomGraphic randomGraphic = (ThingWithCompsRandomGraphic)thing;
                            if (randomGraphic != null)
                            {
                                Log.Message("ExtractRandomGraphicFor randomGraphic not null");
                                if (randomGraphic.gfxint == -1)
                                {
                                    Log.Message("ExtractRandomGraphicFor randomize gfxint");
                                    randomGraphic.gfxint = Rand.Range(0, graphic_RandomItem.subGraphics.Count());
                                }
                            return graphic_RandomItem.subGraphics[randomGraphic.gfxint];
                            }
                        }
                    }
                }
            }
            return outerGraphic;
        }
        */
        public static bool isAdult(this Pawn pawn)
        {
            return pawn.RaceProps.lifeStageAges.Any(x => x.def.reproductive) && pawn.ageTracker.AgeBiologicalYearsFloat >= pawn.RaceProps.lifeStageAges.First(x => x.def.reproductive).minAge;
        }

        public static bool Melta(this ProjectileProperties projectile)
        {

            DamageDef damage = projectile.damageDef;
            return damage.defName.Contains("OG") && damage.defName.Contains("Melta");
        }
        
        public static bool Volkite(this ProjectileProperties projectile)
        {
            DamageDef damage = projectile.damageDef;
            return damage.defName.Contains("OG") && damage.defName.Contains("Volkite");
        }
        
        public static bool Haywire(this ProjectileProperties projectile)
        {
            DamageDef damage = projectile.damageDef;
            return damage.defName.Contains("OG") && damage.defName.Contains("Haywire");
        }

        public static bool Conversion(this ProjectileProperties projectile)
        {
            DamageDef damage = projectile.damageDef;
            return damage.defName.Contains("OG") && damage.defName.Contains("Conversion");
        }
        
        public static bool Las(this ProjectileProperties projectile)
        {
            DamageDef damage = projectile.damageDef;
            return damage.defName.Contains("OG") && damage.defName.Contains("Las");
        }
        
        public static bool Bolt(this ProjectileProperties projectile)
        {
            DamageDef damage = projectile.damageDef;
            return damage.defName.Contains("OG") && damage.defName.Contains("Bolt");
        }
        
        public static bool Plasma(this ProjectileProperties projectile)
        {
            DamageDef damage = projectile.damageDef;
            return damage.defName.Contains("OG") && damage.defName.Contains("Plasma");
        }

        public static bool Distortion(this ProjectileProperties projectile)
        {
            DamageDef damage = projectile.damageDef;
            return damage.defName.Contains("OG") && damage.defName.Contains("Distortion");
        }
        
        public static bool Gauss(this ProjectileProperties projectile)
        {
            DamageDef damage = projectile.damageDef;
            return damage.defName.Contains("OG") && damage.defName.Contains("Gauss");
        }
        
        public static bool Tesla(this ProjectileProperties projectile)
        {
            DamageDef damage = projectile.damageDef;
            return damage.defName.Contains("OG") && damage.defName.Contains("Tesla");
        }


        public static object call(this object o, string methodName, params object[] args)
        {
            var mi = o.GetType().GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (mi != null)
            {
                return mi.Invoke(o, args);
            }
            return null;
        }

        public static MapComponent_DeepStrike DeepStrike(this Map map)
        {
            return map.GetComponent<MapComponent_DeepStrike>();
        }
        
        public static MapComponent_Infiltrate Infiltrate(this Map map)
        {
            return map.GetComponent<MapComponent_Infiltrate>();
        }
        
        public static bool canDeepStrike(this Pawn pawn)
        {
            bool kindDefFlag = pawn.kindDef.HasModExtension<DeepStrikeExtension>();
            bool apparelFlag = pawn.apparel != null && pawn.apparel.WornApparel != null && pawn.apparel.WornApparel.Any(x => x.def.HasModExtension<DeepStrikeExtension>());
            bool hediffFlag = pawn.health.hediffSet.hediffs.Any(x => x.def.HasModExtension<DeepStrikeExtension>());
            bool raceDefFlag = pawn.def.HasModExtension<DeepStrikeExtension>();
            return kindDefFlag || apparelFlag || hediffFlag || raceDefFlag;
        }

        public static bool canDeepStrike(this Pawn pawn, out DeepStrikeExtension Extension)
        {
            Extension = null;
            bool kindDefFlag = pawn.kindDef.HasModExtension<DeepStrikeExtension>();
            bool apparelFlag = pawn.apparel != null && pawn.apparel.WornApparel != null && pawn.apparel.WornApparel.Any(x => x.def.HasModExtension<DeepStrikeExtension>());
            bool hediffFlag = pawn.health.hediffSet.hediffs.Any(x => x.def.HasModExtension<DeepStrikeExtension>());
            bool raceDefFlag = pawn.def.HasModExtension<DeepStrikeExtension>();

            if (kindDefFlag)
            {
                Extension = pawn.kindDef.GetModExtension<DeepStrikeExtension>();
            }
            if (raceDefFlag)
            {
                Extension = pawn.def.GetModExtension<DeepStrikeExtension>();
            }
            if (hediffFlag)
            {
                Extension = pawn.health.hediffSet.hediffs.Where(x => x.def.HasModExtension<DeepStrikeExtension>()).Single().def.GetModExtension<DeepStrikeExtension>();
            }
            if (apparelFlag)
            {
                Extension = pawn.apparel.WornApparel.Where(x => x.def.HasModExtension<DeepStrikeExtension>()).Single().def.GetModExtension<DeepStrikeExtension>();
            }
            return kindDefFlag || apparelFlag || hediffFlag || raceDefFlag;
        }

        public static DeepStrikeExtension DeepStrike(this Pawn pawn)
        {
            DeepStrikeExtension Extension = null;
            bool kindDefFlag = pawn.kindDef.HasModExtension<DeepStrikeExtension>();
            bool apparelFlag = pawn.apparel != null && pawn.apparel.WornApparel != null && pawn.apparel.WornApparel.Any(x => x.def.HasModExtension<DeepStrikeExtension>());
            bool hediffFlag = pawn.health.hediffSet.hediffs.Any(x => x.def.HasModExtension<DeepStrikeExtension>());
            bool raceDefFlag = pawn.def.HasModExtension<DeepStrikeExtension>();

            if (kindDefFlag)
            {
                Extension = pawn.kindDef.GetModExtension<DeepStrikeExtension>();
            }
            if (raceDefFlag)
            {
                Extension = pawn.def.GetModExtension<DeepStrikeExtension>();
            }
            if (hediffFlag)
            {
                Extension = pawn.health.hediffSet.hediffs.Where(x => x.def.HasModExtension<DeepStrikeExtension>()).Single().def.GetModExtension<DeepStrikeExtension>();
            }
            if (apparelFlag)
            {
                Extension = pawn.apparel.WornApparel.Where(x => x.def.HasModExtension<DeepStrikeExtension>()).Single().def.GetModExtension<DeepStrikeExtension>();
            }
            return Extension;
        }

        public static float chanceDeepStrike(this Pawn pawn)
        {
            float f = 0f;
            if (pawn.canDeepStrike(out DeepStrikeExtension extension))
            {
                f = extension.DeepStrikeChance;
            }
            return f;
        }

        public static bool canInfiltrate(this Pawn pawn)
        {
            bool kindDefFlag = pawn.kindDef.HasModExtension<InfiltratorExtension>();
            bool apparelFlag = pawn.apparel != null && pawn.apparel.WornApparel != null && pawn.apparel.WornApparel.Any(x => x.def.HasModExtension<InfiltratorExtension>());
            bool hediffFlag = pawn.health.hediffSet.hediffs.Any(x => x.def.HasModExtension<InfiltratorExtension>());
            bool raceDefFlag = pawn.def.HasModExtension<InfiltratorExtension>();
            return kindDefFlag || apparelFlag || hediffFlag || raceDefFlag;
        }

        public static bool canInfiltrate(this Pawn pawn, out InfiltratorExtension Extension)
        {
            Extension = null;
            bool kindDefFlag = pawn.kindDef.HasModExtension<InfiltratorExtension>();
            bool apparelFlag = pawn.apparel != null && pawn.apparel.WornApparel != null && pawn.apparel.WornApparel.Any(x => x.def.HasModExtension<InfiltratorExtension>());
            bool hediffFlag = pawn.health.hediffSet.hediffs.Any(x => x.def.HasModExtension<InfiltratorExtension>());
            bool raceDefFlag = pawn.def.HasModExtension<InfiltratorExtension>();

            if (kindDefFlag)
            {
                Extension = pawn.kindDef.GetModExtension<InfiltratorExtension>();
            }
            if (raceDefFlag)
            {
                Extension = pawn.def.GetModExtension<InfiltratorExtension>();
            }
            if (hediffFlag)
            {
                Extension = pawn.health.hediffSet.hediffs.Where(x => x.def.HasModExtension<InfiltratorExtension>()).Single().def.GetModExtension<InfiltratorExtension>();
            }
            if (apparelFlag)
            {
                Extension = pawn.apparel.WornApparel.Where(x => x.def.HasModExtension<InfiltratorExtension>()).Single().def.GetModExtension<InfiltratorExtension>();
            }
            return kindDefFlag || apparelFlag || hediffFlag || raceDefFlag;
        }

        public static InfiltratorExtension Infiltrate(this Pawn pawn)
        {
            InfiltratorExtension Extension = null;
            bool kindDefFlag = pawn.kindDef.HasModExtension<InfiltratorExtension>();
            bool apparelFlag = pawn.apparel != null && pawn.apparel.WornApparel != null && pawn.apparel.WornApparel.Any(x => x.def.HasModExtension<InfiltratorExtension>());
            bool hediffFlag = pawn.health.hediffSet.hediffs.Any(x => x.def.HasModExtension<InfiltratorExtension>());
            bool raceDefFlag = pawn.def.HasModExtension<InfiltratorExtension>();
            if (kindDefFlag)
            {
                Extension = pawn.kindDef.GetModExtension<InfiltratorExtension>();
            }
            if (raceDefFlag)
            {
                Extension = pawn.def.GetModExtension<InfiltratorExtension>();
            }
            if (hediffFlag)
            {
                Extension = pawn.health.hediffSet.hediffs.Where(x => x.def.HasModExtension<InfiltratorExtension>()).Single().def.GetModExtension<InfiltratorExtension>();
            }
            if (apparelFlag)
            {
                Extension = pawn.apparel.WornApparel.Where(x => x.def.HasModExtension<InfiltratorExtension>()).Single().def.GetModExtension<InfiltratorExtension>();
            }
            return Extension;
        }

        public static float chanceInfiltrate(this Pawn pawn)
        {
            float f = 0f;
            if (pawn.canInfiltrate(out InfiltratorExtension extension))
            {
                f = extension.InfiltrateChance;
            }
            return f;
        }
        /*
        public static bool abilityWeapon(this Pawn pawn)
        {
            bool flag1 = pawn.equipment != null;
            bool flag2 = pawn.equipment.Primary != null;
            bool flag3 = pawn.equipment.Primary.TryGetComp<CompAbilityItem>() != null;
            return flag1 && flag2 && flag3;
        }

        public static bool abilityEquipment(this Pawn pawn)
        {
            bool flag1 = pawn.apparel != null;
            bool flag2 = pawn.apparel.WornApparel != null;
            bool flag3 = pawn.apparel.WornApparel.Any(x=> x.TryGetComp<CompAbilityItem>() != null);
            return flag1 && flag2 && flag3;
        }

        public static bool abilityImplant(this Pawn pawn)
        {
            return false;
        }
        */
        public static bool isPsyker(this Pawn pawn)
        {
            return pawn.isPsyker(out int Level);
        }

        public static bool isPsyker(this Pawn pawn, out int Level)
        {
            return pawn.isPsyker(out Level, out float Mult);
        }

        public static bool isPsyker(this Pawn pawn, out int Level, out float Mult)
        {
            bool result = false;
            Mult = 0f;
            Level = 0;

            if (pawn.RaceProps.Humanlike)
            {
                if (pawn.health.hediffSet.hediffs.Any(x => x.GetType() == typeof(Hediff_ImplantWithLevel)))
                {
                    Level = (pawn.health.hediffSet.hediffs.First(x => x.GetType() == typeof(Hediff_ImplantWithLevel)) as Hediff_ImplantWithLevel).level;
                    result = pawn.story.traits.DegreeOfTrait(TraitDefOf.PsychicSensitivity) > 0;
                }
                else
                if (pawn.story.traits.HasTrait(TraitDefOf.PsychicSensitivity))
                {
                    result = pawn.story.traits.DegreeOfTrait(TraitDefOf.PsychicSensitivity) > 0;
                    Level = pawn.story.traits.DegreeOfTrait(TraitDefOf.PsychicSensitivity);
                }
                Mult = pawn.GetStatValue(StatDefOf.PsychicSensitivity) * (pawn.needs.mood.CurInstantLevelPercentage - pawn.health.hediffSet.PainTotal);
            }
            else
            {
                ToolUserPskyerDefExtension extension = null;
                if (pawn.def.HasModExtension<ToolUserPskyerDefExtension>())
                {
                    extension = pawn.def.GetModExtension<ToolUserPskyerDefExtension>();
                }
                else
                if (pawn.kindDef.HasModExtension<ToolUserPskyerDefExtension>())
                {
                    extension = pawn.kindDef.GetModExtension<ToolUserPskyerDefExtension>();
                }
                if (extension != null)
                {
                    Level = extension.Level;
                }
                if (pawn.needs!=null && pawn.needs.mood!=null)
                {
                    Mult = pawn.GetStatValue(StatDefOf.PsychicSensitivity) * (pawn.needs.mood.CurInstantLevelPercentage - pawn.health.hediffSet.PainTotal);
                }
                else
                {
                    Mult = pawn.GetStatValue(StatDefOf.PsychicSensitivity) * (1 - pawn.health.hediffSet.PainTotal);
                }
            }

            return result;
        }


        public static bool powerWeapon(this Thing thing)
        {
            bool flag1 = thing.TryGetComp<CompEquippable>() != null;
            bool flag2 = thing.def.IsMeleeWeapon;
            bool flag3 = thing.TryGetComp<CompEquippable>().AllVerbs.Any(x => x.powerWeapon());
            return flag1 && flag2 && flag3;
        }

        public static bool powerWeapon(this Verb verb)
        {
            return verb.GetDamageDef().powerWeapon();
        }

        public static bool powerWeapon(this DamageDef damage)
        {
            return damage.defName.Contains("OG_Power_");
        }

        public static bool witchbladeWeapon(this Thing thing)
        {
            bool flag1 = thing.TryGetComp<CompEquippable>() != null;
            bool flag2 = thing.def.IsMeleeWeapon;
            bool flag3 = thing.TryGetComp<CompEquippable>().AllVerbs.Any(x => x.witchbladeWeapon());
            return flag1 && flag2 && flag3;
        }

        public static bool witchbladeWeapon(this Verb verb)
        {
            return verb.GetDamageDef().witchbladeWeapon();
        }

        public static bool witchbladeWeapon(this DamageDef damage)
        {
            return damage.defName.Contains("OG_Witchblade_");
        }

        public static bool forceWeapon(this Thing thing)
        {
            bool flag1 = thing.TryGetComp<CompEquippable>() != null;
            bool flag2 = thing.def.IsMeleeWeapon;
            bool flag3 = thing.TryGetComp<CompEquippable>().AllVerbs.Any(x => x.forceWeapon());
            return flag1 && flag2 && flag3;
        }

        public static bool forceWeapon(this Verb verb)
        {
            return verb.GetDamageDef().forceWeapon();
        }

        public static bool forceWeapon(this DamageDef damage)
        {
            return damage.defName.Contains("OG_Force_");
        }
        
        public static bool rendingWeapon(this Thing thing)
        {
            bool flag1 = thing.TryGetComp<CompEquippable>() != null;
            bool flag2 = thing.def.IsMeleeWeapon;
            bool flag3 = thing.TryGetComp<CompEquippable>().AllVerbs.Any(x => x.rendingWeapon());
            return flag1 && flag2 && flag3;
        }

        public static bool rendingWeapon(this Verb verb)
        {
            return verb.GetDamageDef().rendingWeapon();
        }

        public static bool rendingWeapon(this DamageDef damage)
        {
            return damage.defName.Contains("OG_Rending_");
        }

    }

}
