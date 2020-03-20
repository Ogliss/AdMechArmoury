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
using UnityEngine;
using System.Reflection;

namespace AdeptusMechanicus
{
	/*
    // Token: 0x02000020 RID: 32 AdeptusMechanicus.PsySilencerExt
    public class PsySilencerExt : DefModExtension
    {

    }

    [HarmonyPatch(typeof(ThingRequiringRoyalPermissionUtility), "IsViolatingRulesOfAnyFaction", new Type[] { typeof(Def), typeof(Pawn), typeof(int), typeof(bool) })]
    public static class AMA_ThingRequiringRoyalPermissionUtility_IsViolatingRulesOfAnyFaction_ExtraSilencer_Patch
    {
        [HarmonyPrefix]
        public static bool Post_IsViolatingRulesOfAnyFaction(Def implantOrWeapon, Pawn pawn, int implantLevel, bool ignoreSilencer, ref bool __result)
        {
            //    Log.Message(string.Format("should {0} cast?", implantOrWeapon));
            bool silenced = pawn.health.hediffSet.hediffs.Any(x => x.def.HasModExtension<PsySilencerExt>());
            if (silenced)
            {
                //    Log.Message("ignoreing this cast");
                __result = false;
                return false;
            }
            return true;
        }
        //    public static FieldInfo graphic = typeof(Graphic).GetField("graphicInt", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
    }

    [HarmonyPatch(typeof(AbilityDef), "GetTooltip")]
    public static class AMA_AbilityDef_GetTooltip_ExtraSilencer_Patch
    {
        [HarmonyPostfix]
        public static void Post_GetTooltip(AbilityDef __instance, Pawn pawn, ref string __result)
        {
            if (pawn != null)
            {
                bool silenced = pawn.health.hediffSet.hediffs.Any(x => x.def.HasModExtension<PsySilencerExt>());
                if (silenced)
                {
                    __result = __instance.LabelCap + ((__instance.level > 0) ? ("\n" + "Level".Translate() + " " + __instance.level) : "") + "\n\n" + __instance.description + "\n\n" + __instance.StatSummary.ToLineList(null, false);
                }
            }
        }
    }

	// Token: 0x020003A7 RID: 935
	public class Hediff_QXPsychicAmplifier : Hediff_ImplantWithLevel
	{
		// Token: 0x06001734 RID: 5940 RVA: 0x000166B5 File Offset: 0x000148B5
		public override void PostAdd(DamageInfo? dinfo)
		{
			base.PostAdd(dinfo);
			this.TryGiveAbilityOfLevel(this.level);
		}

		// Token: 0x06001735 RID: 5941 RVA: 0x000D15B4 File Offset: 0x000CF7B4
		public override void ChangeLevel(int levelOffset)
		{
			if (levelOffset > 0)
			{
				float num = Math.Min((float)levelOffset, this.def.maxSeverity - (float)this.level);
				int num2 = 0;
				while ((float)num2 < num)
				{
					int abilityLevel = this.level + 1 + num2;
					this.TryGiveAbilityOfLevel(abilityLevel);
					num2++;
				}
			}
			base.ChangeLevel(levelOffset);
		}

		// Token: 0x06001736 RID: 5942 RVA: 0x000D1608 File Offset: 0x000CF808
		public void TryGiveAbilityOfLevel(int abilityLevel)
		{
			if (!this.pawn.abilities.abilities.Any((Ability a) => a.def.level == abilityLevel))
			{
				AbilityDef abilityDef = (from a in DefDatabase<AbilityDef>.AllDefs
										 where a.level == abilityLevel
										 select a).RandomElement<AbilityDef>();
				this.pawn.abilities.GainAbility(abilityDef);
				if (PawnUtility.ShouldSendNotificationAbout(this.pawn))
				{
					Messages.Message("PsycastLearnedFromImplant".Translate(this.pawn.Named("USER"), abilityLevel, abilityDef.LabelCap), this.pawn, MessageTypeDefOf.PositiveEvent, true);
				}
			}
		}
	}
	*/
}
