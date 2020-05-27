using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
	// Token: 0x02000002 RID: 2
	public class Wargear2 : Apparel
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public override IEnumerable<Gizmo> GetWornGizmos()
		{
			bool flag = Find.Selector.SingleSelectedThing == base.Wearer;
			if (flag)
			{
                /*
				yield return new Command_Action
				{
					defaultLabel = this.def.label,
					defaultDesc = "This colonist is equipped with a " + this.def.label,
					hotKey = KeyBindingDefOf.Misc2,
					icon = this.def.uiIcon,
					disabled = true
				};
                */
                yield return Wargear2.GetSquadAttackGizmo(base.Wearer);
            }
			yield break;
		}
        private static Gizmo GetSquadAttackGizmo(Pawn pawn)
        {
            Command_Target command_Target = new Command_Target();
            command_Target.defaultLabel = "CommandSquadAttack".Translate();
            command_Target.defaultDesc = "CommandSquadAttackDesc".Translate();
            command_Target.targetingParams = TargetingParameters.ForAttackAny();
            command_Target.hotKey = KeyBindingDefOf.Misc1;
            command_Target.icon = TexCommand.SquadAttack;
            string str;
            if (FloatMenuUtility.GetAttackAction(pawn, LocalTargetInfo.Invalid, out str) == null)
            {
                command_Target.Disable(str.CapitalizeFirst() + ".");
            }
            command_Target.action = delegate (Thing target)
            {
                IEnumerable<Pawn> enumerable = Find.Selector.SelectedObjects.Where(delegate (object x)
                {
                    Pawn pawn3 = x as Pawn;
                    return pawn3 != null && pawn3.IsColonistPlayerControlled && pawn3.Drafted;
                }).Cast<Pawn>();
                foreach (Pawn pawn2 in enumerable)
                {
                    string text;
                    Action attackAction = FloatMenuUtility.GetAttackAction(pawn2, target, out text);
                    if (attackAction != null)
                    {
                        attackAction();
                    }
                }
            };
            return command_Target;
        }
        /*

                private static Gizmo GetSquadAttackGizmo(Pawn pawn)
                {
                    Command_Target command_Target = new Command_Target();
                    command_Target.defaultLabel = "CommandSquadAttack".Translate();
                    command_Target.defaultDesc = "CommandSquadAttackDesc".Translate();
                    command_Target.targetingParams = TargetingParameters.ForAttackAny();
                    command_Target.hotKey = KeyBindingDefOf.Misc1;
                    command_Target.icon = TexCommand.SquadAttack;
                    string str;
                    if (FloatMenuUtility.GetAttackAction(pawn, LocalTargetInfo.Invalid, out str) == null)
                    {
                        command_Target.Disable(str.CapitalizeFirst() + ".");
                    }
                    command_Target.action = delegate (Thing target)
                    {
                        IEnumerable<Pawn> enumerable = Find.Selector.SelectedObjects.Where(delegate (object x)
                        {
                            Pawn pawn3 = x as Pawn;
                            return pawn3 != null && pawn3.IsColonistPlayerControlled && pawn3.Drafted;
                        }).Cast<Pawn>();
                        foreach (Pawn pawn2 in enumerable)
                        {
                            string text;
                            Action attackAction = FloatMenuUtility.GetAttackAction(pawn2, target, out text);
                            if (attackAction != null)
                            {
                                attackAction();
                            }
                        }
                    };
                    return command_Target;
                }

                */
    }
}
