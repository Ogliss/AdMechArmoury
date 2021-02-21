using System;
using System.Collections.Generic;
using System.Linq;
using AdeptusMechanicus.settings;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus.ExtensionMethods
{

    public static class VerbExtensions
    {
        public static AdvancedVerbProperties SpecialRules(this Verb verb)
        {
            return verb.verbProps as AdvancedVerbProperties;
        }

        public static bool RapidFire(this Verb verb, float num, out bool InRange, out float modifier)
        {
            modifier = num;
            InRange = false;
            bool result = verb.SpecialRules() != null && verb.SpecialRules().rapidFire && AMSettings.Instance.AllowRapidFire;
            if (result)
            {
                InRange = verb.SpecialRules().rapidFire && verb.CurrentTarget.Cell.InHorDistOf(verb.Caster.Position, verb.verbProps.range * verb.SpecialRules().rapidFireRange);
                if (InRange)
                {
                    float reduction = ((verb.verbProps.burstShotCount - 1) * verb.verbProps.ticksBetweenBurstShots).TicksToSeconds() / 4;
                    reduction += num / 2;
                    modifier -= reduction;
                }
            }
            return result;
        }

        public static bool UserEffect(this Verb verb)
        {
            bool result = verb.SpecialRules() != null && verb.SpecialRules().EffectsUser && AMSettings.Instance.AllowUserEffects;
            return result;
        }
        
        public static bool UserEffect(this Verb verb, out float Chance, out HediffDef Effect, out StatDef ResistStat, out List<string> ImmuneList)
        {
            Chance = 0;
            Effect = null;
            ResistStat = null;
            ImmuneList = new List<string>();
            bool result = verb.SpecialRules() != null && verb.SpecialRules().EffectsUser && AMSettings.Instance.AllowUserEffects;
            if (result)
            {
                Effect = verb.SpecialRules().userEffect;
                ResistStat = verb.SpecialRules().resistEffectStat;
                Chance = verb.SpecialRules().effectsUserChance;
                ImmuneList = verb.SpecialRules().userEffectImmuneList;
                if (verb.SpecialRules().userEffectImmuneList.Contains(verb.caster.def.defName) || verb.caster == null || !verb.CasterIsPawn)
                {
                    return false;
                }

            }
            return result;
        }

        public static bool TwinLinked(this Verb verb)
        {
            bool result = verb.SpecialRules() !=null && verb.SpecialRules().TwinLinked;
            return result;
        }

        public static bool MultiShot(this Verb verb)
        {
            bool result = verb.SpecialRules() != null && verb.SpecialRules().Multishot && AMSettings.Instance.AllowMultiShot;
            return result;
        }
        public static bool MultiShot(this Verb verb, out int Extras)
        {
            Extras = 0;
            bool result = verb.SpecialRules() != null && verb.SpecialRules().Multishot && AMSettings.Instance.AllowMultiShot;
            if (result)
            {
                Extras = verb.SpecialRules().ScattershotCount;
            }
            return result;
        }

        public static bool GetsHot(this Verb verb)
        {
            bool result = verb.SpecialRules() != null && verb.SpecialRules().getsHot;
            return result;
        }

        public static bool GetsHot(this Verb verb,out bool GetsHotCrit, out float GetsHotCritChance, out bool GetsHotCritExplosion, out float GetsHotCritExplosionChance, out bool canDamageWeapon, out float extraWeaponDamage)
        {
            AdvancedVerbProperties entry = verb.SpecialRules();
            bool GetsHot = false;
            if (entry == null || !AMSettings.Instance.AllowGetsHot)
            {
                GetsHotCrit = false;
                GetsHotCritChance = 0f;
                GetsHotCritExplosion = false;
                GetsHotCritExplosionChance = 0f;
                canDamageWeapon = false;
                extraWeaponDamage = 0f;
            //    Log.Message("no SpecialRules detected");
                return GetsHot;
            }
            GetsHot = entry.getsHot;
            GetsHotCrit = entry.GetsHotCrit;
            GetsHotCritChance = entry.getsHotCritChance;
            GetsHotCritExplosion = entry.GetsHotCritExplosion;
            GetsHotCritExplosionChance = entry.getsHotCritExplosionChance;
            canDamageWeapon = entry.HotDamageWeapon;
            extraWeaponDamage = entry.hotDamage;
            return GetsHot;
        }

        public static bool Jams(this Verb verb)
        {
            bool result = verb.SpecialRules() != null && verb.SpecialRules().Jams;
            return result;
        }
        public static bool Jams(this Verb verb, out bool canDamageWeapon, out float extraWeaponDamage)
        {
            AdvancedVerbProperties entry = verb.SpecialRules();
            bool Jams = false;
            if (entry == null || !AMSettings.Instance.AllowJams)
            {
                canDamageWeapon = false;
                extraWeaponDamage = 0f;
           //     Log.Message("no SpecialRules detected");
                return Jams;
            }
            Jams = entry.Jams;
            canDamageWeapon = entry.JamsDamageWeapon;
            extraWeaponDamage = entry.jamDamage;
            return Jams;
        }

        public static bool powerWeapon(this Verb verb)
        {
            return verb.GetDamageDef().powerWeapon();
        }

        public static bool witchbladeWeapon(this Verb verb)
        {
            return verb.GetDamageDef().witchbladeWeapon();
        }

        public static bool forceWeapon(this Verb verb)
        {
            return verb.GetDamageDef().forceWeapon();
        }

        public static bool rendingWeapon(this Verb verb)
        {
            return verb.GetDamageDef().rendingWeapon();
        }

    }

}
