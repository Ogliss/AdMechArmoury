using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace AdeptusMechanicus
{
    // Token: 0x02000004 RID: 4
    public class GrenadePack : Apparel
    {
        // Token: 0x17000001 RID: 1
        // (get) Token: 0x06000007 RID: 7 RVA: 0x00002149 File Offset: 0x00000349
        public CompGrenadePack kitComp
        {
            get
            {
                return base.GetComp<CompGrenadePack>();
            }
        }

        // Token: 0x06000008 RID: 8 RVA: 0x00002151 File Offset: 0x00000351
        public override void PostMake()
        {
            base.PostMake();
            this.uses = base.GetComp<CompGrenadePack>().Props.Uses;
        }

        // Token: 0x06000009 RID: 9 RVA: 0x00002174 File Offset: 0x00000374
        public void UseKit()
        {
            bool flag = this.uses > 1;
            if (flag)
            {
                this.uses--;
            }
            else
            {
                bool flag2 = !base.Destroyed;
                if (flag2)
                {
                    this.Destroy(DestroyMode.Vanish);
                }
            }
        }

        // Token: 0x0600000A RID: 10 RVA: 0x000021B8 File Offset: 0x000003B8
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.uses, "uses", 0, false);
        }

        // Token: 0x17000002 RID: 2
        // (get) Token: 0x0600000B RID: 11 RVA: 0x000021D8 File Offset: 0x000003D8
        public TargetingParameters TargetingParams
        {
            get
            {
                return new TargetingParameters
                {
                    canTargetLocations = true,
                    canTargetPawns = true,
                    canTargetBuildings = true,
                    canTargetItems = true,
                    canTargetSelf = false
                };
            }
        }

        // Token: 0x0600000C RID: 12 RVA: 0x00002204 File Offset: 0x00000404
        public override IEnumerable<Gizmo> GetWornGizmos()
        {
            bool flag = Find.Selector.SingleSelectedThing == base.Wearer;
            if (flag)
            {
                yield return new Gizmo_GrenadePackStatus
                {
                    grenades = this
                };
                foreach (Gizmo item in base.GetWornGizmos())
                {
                    yield return item;
                    //item = null;
                }
                IEnumerator<Gizmo> enumerator = null;
                yield return new Command_Action
                {
                    action = delegate ()
                    {
                        Find.Targeter.BeginTargeting(this.TargetingParams, delegate (LocalTargetInfo t)
                        {
                            SoundDefOf.Click.PlayOneShotOnCamera(null);
                            GetSquadAttackGizmo(base.Wearer);
                        }, null, null, this.def.uiIcon);
                    },
                    defaultLabel = "TendTarget".Translate(),
                    defaultDesc = "TendTargetDesc".Translate(),
                    hotKey = KeyBindingDefOf.Misc4,
                    icon = this.def.uiIcon
                };
            }
            yield break;
        }

        // Token: 0x0600000D RID: 13 RVA: 0x00002214 File Offset: 0x00000414
        public override IEnumerable<Gizmo> GetGizmos()
        {
            yield return new Gizmo_GrenadePackStatus
            {
                grenades = this
            };
            foreach (Gizmo item in base.GetGizmos())
            {
                yield return item;
                //item = null;
            }
            IEnumerator<Gizmo> enumerator = null;
            yield break;
        }

        // Token: 0x04000006 RID: 6
        public int uses = 0;

        private static Gizmo GetSquadAttackGizmo(Pawn pawn)
        {
            Command_Target command_Target = new Command_Target();
            command_Target.defaultLabel = "CommandSquadAttack".Translate();
            command_Target.defaultDesc = "CommandSquadAttackDesc".Translate();
            command_Target.targetingParams = TargetingParameters.ForAttackAny();
            command_Target.hotKey = KeyBindingDefOf.Misc4;
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

    }
}
