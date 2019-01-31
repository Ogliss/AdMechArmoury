using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace AdeptusMechanicus
{
	// Token: 0x02000A04 RID: 2564
	public class Verb_MeleeAttackForceweapon : Verb_MeleeAttack
    {
        #region Properties
        public ThingDef_ForceWeapon Def
        {
            get
            {
                return EquipmentSource.def as ThingDef_ForceWeapon;
            }
        }
        #endregion Properties

        // Token: 0x06003974 RID: 14708 RVA: 0x001B3F14 File Offset: 0x001B2314
        private IEnumerable<DamageInfo> DamageInfosToApply(LocalTargetInfo target)
		{
            // og's ForceWeapons attempt
            bool Logging = Def.Logging;
            //    float casterintelligence = (float)base.caster.; 
            Def.forceWeaponEffectActive = false;
            string logmsg = string.Format("");
            string logmsg2 = string.Format("");
            bool casterPsychiclySensitive = this.CasterPawn.story.traits.HasTrait(TraitDefOf.PsychicSensitivity);
            Pawn targetpawn = target.Thing as Pawn;
            logmsg = string.Format("Caster is Psyker: {0}, Target Catergory: {1}", casterPsychiclySensitive, targetpawn.def.category); 
            if (Logging == true) { Log.Message(logmsg); }
            if (casterPsychiclySensitive==true && target.Thing.def.category == ThingCategory.Pawn)
            {
                int casterPsychiclySensitiveDegree = this.CasterPawn.story.traits.DegreeOfTrait(TraitDefOf.PsychicSensitivity);
                if (casterPsychiclySensitiveDegree >= 1)
                {
                    float? casterPsychicSensitivity = this.CasterPawn.GetStatValue(StatDefOf.PsychicSensitivity, true) * 100f;
                 logmsg = string.Format("User Psychic Sensitivity: {0}", casterPsychicSensitivity);

                    bool targetPsychiclySensitive = targetpawn.story.traits.HasTrait(TraitDefOf.PsychicSensitivity);
                    float? targetPsychicSensitivity = targetpawn.GetStatValue(StatDefOf.PsychicSensitivity, true) * 100f;
                 logmsg2 = string.Format("Target Psychic Sensitivity: {0}", targetPsychicSensitivity);
                    if (targetPsychiclySensitive==true)
                    {
                        int targetPsychiclySensitiveDegree = targetpawn.story.traits.DegreeOfTrait(TraitDefOf.PsychicSensitivity);
                        if (targetPsychiclySensitiveDegree == -1) { targetPsychicSensitivity = targetpawn.def.statBases.GetStatValueFromList(StatDefOf.PsychicSensitivity, 1.5f) * 100f; }
                        else if (targetPsychiclySensitiveDegree == -2) { targetPsychicSensitivity = targetpawn.def.statBases.GetStatValueFromList(StatDefOf.PsychicSensitivity, 2f) * 100f; }
                        if (targetPsychicSensitivity != null) { logmsg2 = string.Format("Target Psychic Sensitivity: {0}", targetPsychicSensitivity); }
                    }
                    else { int targetPsychiclySensitiveDegree = targetpawn.story.traits.DegreeOfTrait(TraitDefOf.PsychicSensitivity); }
                    float? casterRoll = Rand.Range(0, (int)casterPsychicSensitivity);
                    float? targetRoll = Rand.Range(0, (int)targetPsychicSensitivity);
                    casterRoll = (casterRoll - (targetPsychicSensitivity / 2));
                    if (casterRoll < 0) { casterRoll = 0; }
                        logmsg = string.Format("User Psychic Sensitivity: {0} rolled {1}", casterPsychicSensitivity, casterRoll);
                        logmsg2 = string.Format("Target Psychic Sensitivity: {0} rolled {1}", targetPsychicSensitivity, targetRoll);
                    if (Logging == true) { Log.Message(logmsg); }
                    if (Logging == true) { Log.Message(logmsg2); }
                    if (casterRoll > targetRoll) { Def.forceWeaponEffectActive = true; }
                    else { Def.forceWeaponEffectActive = false; }
                       logmsg = string.Format("User Psychic Sensitivity:caster rolled {0} target rolled {1} force weapon ability active:{2}", casterRoll, targetRoll, Def.forceWeaponEffectActive);
                    if (Logging == true) { Log.Message(logmsg); }
                }
            }
            DamageDef damDef = null;
            if (Def.forceWeaponEffectActive == true)
            {
                damDef = Def.blastdamageDef;
            }
            else
            {
                damDef = this.verbProps.meleeDamageDef;
            }
                logmsg = string.Format("damDef {0}", damDef);
            if (Logging == true) { Log.Message(logmsg); }
            // end of edit
            float damAmount = this.verbProps.AdjustedMeleeDamageAmount(this, base.CasterPawn);
                logmsg = string.Format("damAmount {0}", damAmount);
            if (Logging == true) { Log.Message(logmsg); }
            float armorPenetration = this.verbProps.AdjustedArmorPenetration(this, base.CasterPawn);
               logmsg = string.Format("armorPenetration {0}", armorPenetration);
            if (Logging == true) { Log.Message(logmsg); }
            BodyPartGroupDef bodyPartGroupDef = null;
               logmsg = string.Format("bodyPartGroupDef {0}", bodyPartGroupDef);
            if (Logging == true) { Log.Message(logmsg); }
            HediffDef hediffDef = null;
            /*
            if (Def.forceWeaponEffectActive == true)
            {
                hediffDef = Def.forceWeaponEffect;
            }
            */
               logmsg = string.Format("hediffDef {0}", hediffDef);
            if (Logging == true) { Log.Message(logmsg); }
            damAmount = Rand.Range(damAmount * 0.8f, damAmount * 1.2f);
               logmsg = string.Format("damAmount {0}", damAmount);
            if (Logging == true) { Log.Message(logmsg); }
            if (base.CasterIsPawn)
			{
				bodyPartGroupDef = this.verbProps.AdjustedLinkedBodyPartsGroup(this.tool);
				if (damAmount >= 1f)
				{
					if (base.HediffCompSource != null)
					{
						hediffDef = base.HediffCompSource.Def;
					}
				}
				else
				{
					damAmount = 1f;
					damDef = DamageDefOf.Blunt;
				}
            }
               logmsg = string.Format("hediffDef {0}", hediffDef);
            if (Logging == true) { Log.Message(logmsg); }
            ThingDef source;
            if (base.EquipmentSource != null)
			{
				source = base.EquipmentSource.def;
			}
			else
			{
				source = base.CasterPawn.def;
            }
               logmsg = string.Format("ThingDef source {0}", source);
            if (Logging == true) { Log.Message(logmsg); }
            Thing caster = this.caster;
               logmsg = string.Format("Thing caster {0}", caster);
            if (Logging == true) { Log.Message(logmsg); }
            Vector3 direction = (target.Thing.Position - base.CasterPawn.Position).ToVector3();
               logmsg = string.Format("Vector3 direction {0}", direction);
            if (Logging == true) { Log.Message(logmsg); }
            DamageDef def = damDef;
               logmsg = string.Format("DamageDef def {0}", def);
            if (Logging == true) { Log.Message(logmsg); }
            float num = damAmount;
               logmsg = string.Format("float num {0}", num);
            if (Logging == true) { Log.Message(logmsg); }
            float num2 = armorPenetration;
               logmsg = string.Format("float num2 {0}", num2);
            if (Logging == true) { Log.Message(logmsg); }
            DamageInfo mainDinfo = new DamageInfo(def, num, num2, -1f, caster, null, source, DamageInfo.SourceCategory.ThingOrUnknown, null);
            mainDinfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
			mainDinfo.SetWeaponBodyPartGroup(bodyPartGroupDef);
			mainDinfo.SetWeaponHediff(hediffDef);
			mainDinfo.SetAngle(direction);
               logmsg = string.Format("DamageInfo mainDinfo {0}", mainDinfo);
            if (Logging == true) { Log.Message(logmsg); }
            Def.mainDinfo = mainDinfo;
            yield return mainDinfo;


            if (this.surpriseAttack && ((this.verbProps.surpriseAttack != null && !this.verbProps.surpriseAttack.extraMeleeDamages.NullOrEmpty<ExtraMeleeDamage>()) || this.tool == null || this.tool.surpriseAttack == null || this.tool.surpriseAttack.extraMeleeDamages.NullOrEmpty<ExtraMeleeDamage>()))
			{
				IEnumerable<ExtraMeleeDamage> extraDamages = Enumerable.Empty<ExtraMeleeDamage>();
				if (this.verbProps.surpriseAttack != null && this.verbProps.surpriseAttack.extraMeleeDamages != null)
				{
					extraDamages = extraDamages.Concat(this.verbProps.surpriseAttack.extraMeleeDamages);
                       logmsg = string.Format("extraDamages {0}", extraDamages);
                    if (Logging == true) { Log.Message(logmsg); }
                }
                if (this.tool != null && this.tool.surpriseAttack != null && !this.tool.surpriseAttack.extraMeleeDamages.NullOrEmpty<ExtraMeleeDamage>())
				{
					extraDamages = extraDamages.Concat(this.tool.surpriseAttack.extraMeleeDamages);
                       logmsg = string.Format("extraDamages {0}", extraDamages);
                    if (Logging == true) { Log.Message(logmsg); }
                }
                foreach (ExtraMeleeDamage extraDamage in extraDamages)
				{
					int extraDamageAmount = GenMath.RoundRandom(extraDamage.AdjustedDamageAmount(this, base.CasterPawn));
                       logmsg = string.Format("int extraDamageAmount {0}", extraDamageAmount);
                    if (Logging == true) { Log.Message(logmsg); }
                    float extraDamageArmorPenetration = extraDamage.AdjustedArmorPenetration(this, base.CasterPawn);
                       logmsg = string.Format("float extraDamageArmorPenetration {0}", extraDamageArmorPenetration);
                    if (Logging == true) { Log.Message(logmsg); }
                    def = extraDamage.def;
                       logmsg = string.Format("def {0}", def);
                    if (Logging == true) { Log.Message(logmsg); }
                    num2 = (float)extraDamageAmount;
                       logmsg = string.Format("num2 {0}", num2);
                    if (Logging == true) { Log.Message(logmsg); }
                    num = extraDamageArmorPenetration;
                       logmsg = string.Format("num {0}", num);
                    if (Logging == true) { Log.Message(logmsg); }
                    caster = this.caster;
                       logmsg = string.Format("caster {0}", caster);
                    if (Logging == true) { Log.Message(logmsg); }
                    DamageInfo extraDinfo = new DamageInfo(def, num2, num, -1f, caster, null, source, DamageInfo.SourceCategory.ThingOrUnknown, null);
                    extraDinfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
					extraDinfo.SetWeaponBodyPartGroup(bodyPartGroupDef);
					extraDinfo.SetWeaponHediff(hediffDef);
					extraDinfo.SetAngle(direction);
                       logmsg = string.Format("DamageInfo extraDinfo {0}", extraDinfo);
                    if (Logging == true) { Log.Message(logmsg); }
                    yield return extraDinfo;
				}
			}

               logmsg = string.Format("yield Break");
            if (Logging == true) { Log.Message(logmsg); }
            yield break;

		}

        // Token: 0x06003974 RID: 14708 RVA: 0x001B3F14 File Offset: 0x001B2314
        private IEnumerable<DamageInfo> DamageInfosToApplyOriginal(LocalTargetInfo target)
        {
            float damAmount = this.verbProps.AdjustedMeleeDamageAmount(this, base.CasterPawn);
            float armorPenetration = this.verbProps.AdjustedArmorPenetration(this, base.CasterPawn);
            DamageDef damDef = this.verbProps.meleeDamageDef;
            BodyPartGroupDef bodyPartGroupDef = null;
            HediffDef hediffDef = null;
            damAmount = Rand.Range(damAmount * 0.8f, damAmount * 1.2f);
            if (base.CasterIsPawn)
            {
                bodyPartGroupDef = this.verbProps.AdjustedLinkedBodyPartsGroup(this.tool);
                if (damAmount >= 1f)
                {
                    if (base.HediffCompSource != null)
                    {
                        hediffDef = base.HediffCompSource.Def;
                    }
                }
                else
                {
                    damAmount = 1f;
                    damDef = DamageDefOf.Blunt;
                }
            }
            ThingDef source;
            if (base.EquipmentSource != null)
            {
                source = base.EquipmentSource.def;
            }
            else
            {
                source = base.CasterPawn.def;
            }
            Vector3 direction = (target.Thing.Position - base.CasterPawn.Position).ToVector3();
            DamageDef def = damDef;
            float num = damAmount;
            float num2 = armorPenetration;
            Thing caster = this.caster;
            DamageInfo mainDinfo = new DamageInfo(def, num, num2, -1f, caster, null, source, DamageInfo.SourceCategory.ThingOrUnknown, null);
            mainDinfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
            mainDinfo.SetWeaponBodyPartGroup(bodyPartGroupDef);
            mainDinfo.SetWeaponHediff(hediffDef);
            mainDinfo.SetAngle(direction);
            yield return mainDinfo;
            if (this.surpriseAttack && ((this.verbProps.surpriseAttack != null && !this.verbProps.surpriseAttack.extraMeleeDamages.NullOrEmpty<ExtraMeleeDamage>()) || this.tool == null || this.tool.surpriseAttack == null || this.tool.surpriseAttack.extraMeleeDamages.NullOrEmpty<ExtraMeleeDamage>()))
            {
                IEnumerable<ExtraMeleeDamage> extraDamages = Enumerable.Empty<ExtraMeleeDamage>();
                if (this.verbProps.surpriseAttack != null && this.verbProps.surpriseAttack.extraMeleeDamages != null)
                {
                    extraDamages = extraDamages.Concat(this.verbProps.surpriseAttack.extraMeleeDamages);
                }
                if (this.tool != null && this.tool.surpriseAttack != null && !this.tool.surpriseAttack.extraMeleeDamages.NullOrEmpty<ExtraMeleeDamage>())
                {
                    extraDamages = extraDamages.Concat(this.tool.surpriseAttack.extraMeleeDamages);
                }
                foreach (ExtraMeleeDamage extraDamage in extraDamages)
                {
                    int extraDamageAmount = GenMath.RoundRandom(extraDamage.AdjustedDamageAmount(this, base.CasterPawn));
                    float extraDamageArmorPenetration = extraDamage.AdjustedArmorPenetration(this, base.CasterPawn);
                    def = extraDamage.def;
                    num2 = (float)extraDamageAmount;
                    num = extraDamageArmorPenetration;
                    caster = this.caster;
                    DamageInfo extraDinfo = new DamageInfo(def, num2, num, -1f, caster, null, source, DamageInfo.SourceCategory.ThingOrUnknown, null);
                    extraDinfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
                    extraDinfo.SetWeaponBodyPartGroup(bodyPartGroupDef);
                    extraDinfo.SetWeaponHediff(hediffDef);
                    extraDinfo.SetAngle(direction);
                    yield return extraDinfo;
                }
            }
            yield break;
        }

        // Token: 0x06003975 RID: 14709 RVA: 0x001B3F40 File Offset: 0x001B2340
        protected override DamageWorker.DamageResult ApplyMeleeDamageToTarget(LocalTargetInfo target)
        {
            string logmsg = string.Format("");
            bool Logging = Def.Logging;
            logmsg = string.Format("Target Catergory: {0}", base.currentTarget.Thing.def.category);
            if (Logging == true) { Log.Message(logmsg); }
            logmsg = string.Format("trying dinfo's");
            if (Logging == true) { Log.Message(logmsg); }
            DamageWorker.DamageResult result = new DamageWorker.DamageResult();
            if (base.currentTarget.Thing.def.category == ThingCategory.Pawn)
            {
                if (target.IsValid)
                {

                    foreach (DamageInfo dinfo in this.DamageInfosToApply(target))
                    {
                        logmsg = string.Format("trying DamageInfo dinfo {0}", dinfo);
                        if (Logging == true) { Log.Message(logmsg); }

                        logmsg = string.Format("trying {0}", dinfo);
                        if (Logging == true) { Log.Message(logmsg); }
                        if (Def.forceWeaponEffectActive == true)
                        {
                            Map map = CasterPawn.Map;
                            IntVec3 position = target.Cell;
                            Map map2 = map;
                            float explosionRadius = Def.blastRadius;
                            DamageDef damageDef = Def.blastdamageDef;
                            Thing launcher = base.EquipmentSource;
                            int damageAmount = Def.blastdamageAmount;
                            float hitdmg = damageAmount;
                            float armorPenetration = Def.blastarmorPenetration;
                            SoundDef soundExplode = Def.blastsoundExplode;
                            ThingDef equipmentDef = base.EquipmentSource.def;
                            Thing thing = target.Thing;
                            GenExplosion.DoExplosion(position, map2, explosionRadius, damageDef, launcher, damageAmount, armorPenetration, soundExplode, equipmentDef, null, thing, null, 0f, 0, false, null, 0, 0, 0, false);
                            DamageInfo daminfo = new DamageInfo(damageDef, hitdmg, armorPenetration, -1f, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, thing);
                            thing.TakeDamage(daminfo);//.AssociateWithLog(battleLogEntry_MeleeCombat);
                            float KillChance = Def.KillChance;
                            if (KillChance != 0)
                            {
                                float KillRoll = Rand.Range(0, 100);
                                logmsg = string.Format("rolled {0} needs less than {1} to detonate", KillRoll, KillChance);
                                if (Logging == true) { Log.Message(logmsg); }
                                if (KillRoll < KillChance)
                                {
                                    string msg = string.Format("{0} was slain by a force strike", target.Thing.LabelCap);
                                    target.Thing.Kill(Def.mainDinfo);
                                    if (target.Thing.Faction == Faction.OfPlayer) { Messages.Message(msg, MessageTypeDefOf.PawnDeath); }
                                }
                            }
                        }

                        if (target.ThingDestroyed || !target.IsValid)
                        {
                            break;
                        }
                        result = target.Thing.TakeDamage(dinfo);
                        logmsg = string.Format("trying DamageInfo dinfo 2 {0}", dinfo);
                        if (Logging == true) { Log.Message(logmsg); }
                    }
                }
                return result;
            }
            else
            {
                foreach (DamageInfo dinfo in this.DamageInfosToApplyOriginal(target))
                {
                    logmsg = string.Format("trying DamageInfo dinfo {0}", dinfo);
                    if (Logging == true) { Log.Message(logmsg); }

                    logmsg = string.Format("trying {0}", dinfo);
                    if (Logging == true) { Log.Message(logmsg); }
                    if (Def.forceWeaponEffectActive == true)
                    {
                        Map map = CasterPawn.Map;
                        IntVec3 position = target.Cell;
                        Map map2 = map;
                        float explosionRadius = Def.blastRadius;
                        DamageDef damageDef = Def.blastdamageDef;
                        Thing launcher = base.EquipmentSource;
                        int damageAmount = Def.blastdamageAmount;
                        float hitdmg = damageAmount;
                        float armorPenetration = Def.blastarmorPenetration;
                        SoundDef soundExplode = Def.blastsoundExplode;
                        ThingDef equipmentDef = base.EquipmentSource.def;
                        Thing thing = target.Thing;
                        GenExplosion.DoExplosion(position, map2, explosionRadius, damageDef, launcher, damageAmount, armorPenetration, soundExplode, equipmentDef, null, thing, null, 0f, 0, false, null, 0, 0, 0, false);
                        DamageInfo daminfo = new DamageInfo(damageDef, hitdmg, armorPenetration, -1f, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, thing);
                        thing.TakeDamage(daminfo);//.AssociateWithLog(battleLogEntry_MeleeCombat);
                        float KillChance = Def.KillChance;
                        float KillRoll = Rand.Range(0, 100);
                        logmsg = string.Format("rolled {0} needs less than {1} to detonate", KillRoll, KillChance);
                        if (Logging == true) { Log.Message(logmsg); }
                        if (KillRoll < KillChance && KillChance != 0)
                        {
                            string msg = string.Format("{0} was slain by a force strike", target.Thing.LabelCap);
                            target.Thing.Kill(Def.mainDinfo);
                            if (target.Thing.Faction == Faction.OfPlayer) { Messages.Message(msg, MessageTypeDefOf.PawnDeath); }
                        }
                    }

                    if (target.ThingDestroyed)
                    {
                        break;
                    }
                    result = target.Thing.TakeDamage(dinfo);
                    logmsg = string.Format("trying DamageInfo dinfo 2 {0}", dinfo);
                    if (Logging == true) { Log.Message(logmsg); }
                }
                return result;
            }
		}

		// Token: 0x0400250E RID: 9486
		private const float MeleeDamageRandomFactorMin = 0.8f;

		// Token: 0x0400250F RID: 9487
		private const float MeleeDamageRandomFactorMax = 1.2f;
    }
}
