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
using System.Reflection;
using UnityEngine;
using AdeptusMechanicus.ExtensionMethods;
using AdeptusMechanicus.settings;
using System.Runtime.InteropServices;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Thing), "TakeDamage")]
    public static class Thing_TakeDamage_SpecialRules_Patch
    {
        public static void Prefix(Thing __instance, ref DamageInfo dinfo)
        {
            if (__instance == null || !__instance.Spawned || __instance.Map == null)
            {
                return;
            }
            bool rending = AMSettings.Instance.AllowRendingMeleeEffect && dinfo.Def.rendingWeapon();
            bool power = dinfo.Def.powerWeapon();
            bool force = dinfo.Def.forceWeapon();
            bool witchblade = dinfo.Def.witchbladeWeapon();
            bool act = rending || power || force || witchblade;
            Pawn hitPawn = __instance as Pawn;
            Pawn Attacker = dinfo.Instigator as Pawn ?? null;
            if (__instance != null && act)
            {
                if (dinfo.Instigator != null)
                {
                    if (Attacker == null || hitPawn == null)
                    {
                        return;
                    }
                    if (dinfo.Weapon != null && act)
                    {
                //    //    Log.Message("Thing_TakeDamage_SpecialRules_Patch Prefix " + hitPawn + " hit by " + dinfo.Weapon);
                        Thing Weapon = null;
                        if (dinfo.Weapon.IsWeapon)
                        {
                            foreach (var item in Attacker.equipment.AllEquipmentListForReading)
                            {
                                if (item.def == dinfo.Weapon)
                                {
                                    Weapon = item;
                                }
                            }
                        }
                        else
                        if (dinfo.Weapon.IsApparel)
                        {
                            foreach (var item in Attacker.apparel.WornApparel)
                            {
                                if (item.def == dinfo.Weapon)
                                {
                                    Weapon = item;
                                }
                            }
                        }
                        if (Weapon == null)
                        {
                            Weapon = Attacker.equipment?.Primary;
                        }
                        if (Weapon == null)
                        {
                        //    Log.Warning("Thing_TakeDamage_SpecialRules_Patch Prefix Failed finding Weapon:  " + dinfo.Weapon);
                            return;
                        }
                        OgsCompActivatableEffect.CompActivatableEffect activatableEffect = Weapon.TryGetCompFast<OgsCompActivatableEffect.CompActivatableEffect>();
                        bool? activeEffect = null;
                        if (activatableEffect != null)
                        {
                            activeEffect = activatableEffect.IsActiveNow;
                        }
                        if (!activeEffect.HasValue || activeEffect.Value)
                        {
                        //    Log.Message(dinfo.Weapon.LabelCap + " activeEffect");
                            if (rending)
                            {
                                //    //    Log.Message(dinfo.Weapon.LabelCap + " Is Rending");
                                dinfo = GetRendingDamage(dinfo);
                            }
                            else
                            if (power || force)
                            {
                                dinfo = GetPowerDamage(dinfo);
                                return;
                            }
                            else if (witchblade)
                            {
                                dinfo = GetWitchbladeDamage(dinfo);
                                return;
                            }
                        }
                    }
                }
            }
        }

        public static void Postfix(Thing __instance, ref DamageInfo dinfo)
        {
            bool force = AMSettings.Instance.AllowForceWeaponEffect && dinfo.Def.forceWeapon();
            Pawn Attacker = dinfo.Instigator as Pawn;
            Pawn hitPawn = __instance as Pawn;
            if (__instance != null && force)
            {
                if (Attacker == null || hitPawn == null)
                {
                    return;
                }
                if (dinfo.Weapon != null)
                {
                    if (force)
                    {
                        dinfo = GetForceDamage(dinfo, Attacker, hitPawn);
                    }
                }
            }
            if (hitPawn != null && !hitPawn.RaceProps.Humanlike)
            {
                if (hitPawn.ageTracker.CurKindLifeStage is SwarmKindLifeStage swarm)
                {
                    if (!swarm.subStagesHealth.NullOrEmpty())
                    {
                        int ind = (int)Mathf.Lerp(0, swarm.subStagesHealth.Count, hitPawn.health.summaryHealth.SummaryHealthPercent);
                        Log.Message($"{hitPawn}'s SwarmKindLifeStage using HealthSubStage @ Ind: {ind}");
                        if (ind > 0) hitPawn.Drawer.renderer.graphics.nakedGraphic = swarm.subStagesHealth[ind - 1].bodyGraphicData.Graphic;
                    }
                    
                }
            }
        }

        public static DamageInfo GetPowerDamage(DamageInfo cloneSource)
        {
            float AP = float.MaxValue;
            DamageInfo damage = new DamageInfo(
            cloneSource.Def,
            cloneSource.Amount,
            AP,
            cloneSource.Angle,
            cloneSource.Instigator,
            cloneSource.HitPart,
            cloneSource.Weapon,
            cloneSource.Category,
            cloneSource.IntendedTarget
            );
            return damage;
        }
        

        public static DamageInfo GetRendingDamage(DamageInfo cloneSource)
        {
            Pawn Attacker = cloneSource.Instigator as Pawn;
            float AP = float.MaxValue;
            DamageInfo damage = new DamageInfo(
            cloneSource.Def,
            cloneSource.Amount,
            AP,
            cloneSource.Angle,
            cloneSource.Instigator,
            cloneSource.HitPart,
            cloneSource.Weapon,
            cloneSource.Category,
            cloneSource.IntendedTarget
            );
            return damage;
        }

        public static DamageInfo GetWitchbladeDamage(DamageInfo cloneSource)
        {
            Pawn Caster = cloneSource.Instigator as Pawn;
            if (Caster != null)
            {
                if (Caster.isPsyker(out int Level, out float Mult))
                {
                    float AP = Level * Mult;
                    float Amount = cloneSource.Amount;
                    DamageInfo damage = new DamageInfo(
                    cloneSource.Def,
                    Amount * (Level * Mult),
                    AP,
                    cloneSource.Angle,
                    cloneSource.Instigator,
                    cloneSource.HitPart,
                    cloneSource.Weapon,
                    cloneSource.Category,
                    cloneSource.IntendedTarget
                    );
                    return damage;
                }
            }
            return cloneSource;
        }

        public static BodyPartRecord Head(Pawn hitPawn)
        {
            return hitPawn.RaceProps.body.AllParts.Where(x => x.def.defName.Contains("Head") && !x.def.defName.Contains("Claw") && x.groups.Contains(DefDatabase<BodyPartGroupDef>.GetNamed("HeadAttackTool"))).First();
        }

        public static DamageInfo GetForceDamage(DamageInfo cloneSource, Pawn Caster, Thing target)
        {
            if (Caster != null)
            {
                if (Caster.isPsyker(out int Level, out float Mult))
                {
                    //    log.message(Caster.NameShortColored + " Level " + Level + " Mult " + Mult);
                    if (cloneSource.Weapon != null)
                    {
                        Thing Weapon = null;
                        if (cloneSource.Weapon.IsWeapon)
                        {
                            foreach (var item in Caster.equipment.AllEquipmentListForReading)
                            {
                                if (item.def == cloneSource.Weapon)
                                {
                                    Weapon = item;
                                }
                            }
                        }
                        else
                        if (cloneSource.Weapon.IsApparel)
                        {
                            foreach (var item in Caster.apparel.WornApparel)
                            {
                                if (item.def == cloneSource.Weapon)
                                {
                                    Weapon = item;
                                }
                            }
                        }
                        if (Weapon != null)
                        {

                            Weapon = Caster.equipment.Primary;
                        }
                        CompWeapon_MeleeSpecialRules WeaponRules = Weapon.TryGetCompFast<CompWeapon_MeleeSpecialRules>();
                        CompForceWeaponActivatableEffect compForce = Weapon.TryGetCompFast<CompForceWeaponActivatableEffect>();
                        if (WeaponRules != null || compForce != null)
                        {
                            if (WeaponRules?.ForceWeapon ?? compForce != null)
                            {
                                bool requiresPsyker = WeaponRules?.ForceEffectRequiresPsyker ?? compForce.ForceEffectRequiresPsyker;
                                bool casterPsychiclySensitive = Caster.RaceProps.Humanlike && (Caster.story.traits.HasTrait(TraitDefOf.PsychicSensitivity) || Caster.story.traits.HasTrait(DefDatabase<TraitDef>.GetNamedSilentFail("Psyker")));
                                bool Activate = false;
                                if ((casterPsychiclySensitive || !requiresPsyker) && target.def.category == ThingCategory.Pawn && target is Pawn Victim)
                                {
                                    int casterPsychiclySensitiveDegree = casterPsychiclySensitive ? Caster.story.traits.DegreeOfTrait(TraitDefOf.PsychicSensitivity) : 0;
                                    if ((casterPsychiclySensitiveDegree >= 1 || !requiresPsyker))
                                    {
                                        float? casterPsychicSensitivity = Caster.GetStatValue(StatDefOf.PsychicSensitivity, true) * 100f;
                                        bool targetPsychiclySensitive = Victim.RaceProps.Humanlike && Victim.story.traits.HasTrait(TraitDefOf.PsychicSensitivity);
                                        float? targetPsychicSensitivity = Victim.GetStatValue(StatDefOf.PsychicSensitivity, true) * 100f;
                                        if (targetPsychiclySensitive == true)
                                        {
                                            int targetPsychiclySensitiveDegree = Victim.story.traits.DegreeOfTrait(TraitDefOf.PsychicSensitivity);
                                            if (targetPsychiclySensitiveDegree == -1) { targetPsychicSensitivity = Victim.def.statBases.GetStatValueFromList(StatDefOf.PsychicSensitivity, 1.5f) * 100f; }
                                            else if (targetPsychiclySensitiveDegree == -2) { targetPsychicSensitivity = Victim.def.statBases.GetStatValueFromList(StatDefOf.PsychicSensitivity, 2f) * 100f; }
                                        }
                                        else { /*int targetPsychiclySensitiveDegree = 0;*/ }
                                        {
                                            float CasterMood = Caster.needs.mood.CurLevelPercentage;
                                            float VictimMood = Victim?.needs?.mood != null ? Victim.needs.mood.CurLevelPercentage : 1;
                                            Rand.PushState();
                                            float? casterRoll = Rand.Range(0, (int)casterPsychicSensitivity) * CasterMood;
                                            float? targetRoll = Rand.Range(0, (int)targetPsychicSensitivity) * VictimMood;
                                            Rand.PopState();
                                            casterRoll = (casterRoll - (targetPsychicSensitivity / 2));
                                            Activate = (casterRoll > targetRoll);
                                            //    log.message(string.Format("Caster:{0}, Victim:{1}", casterRoll, targetRoll));
                                            if (Activate)
                                            {
                                                DamageDef damDef = WeaponRules?.ForceWeaponEffect ?? compForce.ForceWeaponEffect;
                                                float damAmount = cloneSource.Amount;
                                                float armorPenetration = cloneSource.ArmorPenetrationInt;
                                                Rand.PushState();
                                                BodyPartRecord bodyPart = Rand.Chance(0.05f) && Victim.RaceProps.body.AllParts.Any(x => x.def.defName.Contains("Brain")) ? Victim.RaceProps.body.AllParts.Find(x => x.def.defName.Contains("Brain")) : null;
                                                Rand.PopState();
                                                BodyPartGroupDef bodyPartGroupDef = null;
                                                HediffDef hediffDef = WeaponRules?.ForceWeaponHediff ?? compForce.ForceWeaponHediff;
                                                damAmount = Rand.Range(damAmount * 0.1f, damAmount * 0.5f);
                                                ThingDef source = cloneSource.Weapon;
                                                Thing caster = Caster;
                                                Vector3 direction = (target.Position - Caster.Position).ToVector3();
                                                float num = damAmount;
                                                DamageInfo mainDinfo = new DamageInfo(damDef, num, 2, -1f, caster, bodyPart, source, DamageInfo.SourceCategory.ThingOrUnknown, null);
                                                mainDinfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
                                                mainDinfo.SetWeaponBodyPartGroup(bodyPartGroupDef);
                                                mainDinfo.SetWeaponHediff(hediffDef);
                                                mainDinfo.SetAngle(direction);
                                                Victim.TakeDamage(mainDinfo);
                                                Map map = Caster.Map;
                                                IntVec3 position = target.Position;
                                                Map map2 = map;
                                                float explosionRadius = 0f;
                                                Thing launcher = Weapon;
                                                SoundDef soundExplode = WeaponRules?.ForceWeaponTriggerSound ?? compForce.ForceWeaponTriggerSound;
                                                Thing thing = target;
                                                GenExplosion.DoExplosion(position, map2, explosionRadius, damDef, launcher, (int)damAmount, armorPenetration, soundExplode, source, null, thing, null, 0f, 0, null, false, null, 0, 0, 0, false);
                                                TaggedString msg = "AdeptusMechanicus.ForceStrike";
                                                MessageTypeDef typeDef = MessageTypeDefOf.NegativeHealthEvent;
                                                float KillChance = WeaponRules?.ForceWeaponKillChance ?? compForce.ForceWeaponKillChance;
                                                if (KillChance != 0)
                                                {
                                                    Rand.PushState();
                                                    float KillRoll = Rand.Range(0, 100);
                                                    if (Rand.Chance(KillChance))
                                                    {
                                                        typeDef = MessageTypeDefOf.PawnDeath;
                                                    }
                                                    Rand.PopState();
                                                }
                                                if (typeDef == MessageTypeDefOf.PawnDeath)
                                                {
                                                    Victim.Kill(mainDinfo);
                                                }
                                                if (Victim.Faction == Faction.OfPlayer) { Messages.Message(string.Format("AdeptusMechanicus.Force_Strike ".Translate(), Victim.Name, typeDef == MessageTypeDefOf.PawnDeath ? "slain" : "blasted"), typeDef); }
                                                return mainDinfo;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return cloneSource;
        }

    }
}