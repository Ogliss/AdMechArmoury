using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AbilityUser
{
    public class CompProperties_AbilityItem : CompProperties
    {
        public List<AbilityDef> Abilities = new List<AbilityDef>();
        
        public CompProperties_AbilityItem()
        {
            compClass = typeof(CompAbilityItem);
        }
    }
    public class CompAbilityItem : ThingComp
    {
        public CompProperties_AbilityItem Props => (CompProperties_AbilityItem)this.props;
    }

    [StaticConstructorOnStartup]
    public class AbilityButtons
    {
        public static readonly Texture2D EmptyTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);
        public static readonly Texture2D FullTex = SolidColorMaterials.NewSolidColorTexture(0.5f, 0.5f, 0.5f, 0.6f);
    }

    public static class StringsToTranslate
    {
        //
        public static readonly string AU_AoEProperties = "Area of Effect Properties";

        public static readonly string AU_TargetClass = "Targets: ";
        public static readonly string AU_AoECharacters = "Characters";
        public static readonly string AU_AoEFriendlyFire = "Friendly Fire: ";
        public static readonly string AU_AoEMaxTargets = "Max Targets: ";
        public static readonly string AU_AoEStartsFromCaster = "Starts from caster: ";
        public static readonly string AU_Cooldown = "Cooldown: ";
        public static readonly string AU_Type = "Type: ";
        public static readonly string AU_TargetAoE = "Area of Effect";
        public static readonly string AU_TargetSelf = "Targets Self";
        public static readonly string AU_TargetThing = "Targets Other";
        public static readonly string AU_TargetLocation = "Targets Location";
        public static readonly string AU_Extra = "Extra";
        public static readonly string AU_MentalStateChance = "Mental State Chance";
        public static readonly string AU_EffectChance = "Effect Chance";
        public static readonly string AU_BurstShotCount = "Burst Count:";
        public static readonly string AU_CastSuccess = "Cast Success";
        public static readonly string AU_CastFailure = "Cast Failed";
        public static readonly string AU_DISABLED = "DISABLED";
    }
}