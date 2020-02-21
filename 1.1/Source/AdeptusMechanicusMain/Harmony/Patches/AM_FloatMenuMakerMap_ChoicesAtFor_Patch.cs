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
using UnityEngine;

namespace AdeptusMechanicus.HarmonyInstance
{
    // Token: 0x020000AB RID: 171
    [HarmonyPatch(typeof(FloatMenuMakerMap), "ChoicesAtFor")]
    internal static class AM_FloatMenuMakerMap_ChoicesAtFor_Patch
    {
        // Token: 0x06000245 RID: 581 RVA: 0x00010E84 File Offset: 0x0000F084
        private static void Postfix(Vector3 clickPos, Pawn pawn, ref List<FloatMenuOption> __result)
        {
            IntVec3 c = clickPos.ToIntVec3();
            Map map = pawn.Map;
            List<Thing> list = map.thingGrid.ThingsListAt(c);
            bool flag = list.Any(x => x is Pawn p && p.IsFreeColonist);
            if (flag)
            {
                List<Thing> list2 = map.thingGrid.ThingsListAt(c).FindAll(x => x is Pawn p && p.IsFreeColonist);
                foreach (var item in list2)
                {
                    Pawn p = (Pawn)item;
                    Log.Message(string.Format("{0}'s free jim....", p.LabelShortCap));
                }
            }
            bool flag1 = list.Any(x => x is Pawn p && p.IsPrisonerOfColony);
            if (flag1)
            {
                List<Thing> list2 = map.thingGrid.ThingsListAt(c).FindAll(x => x is Pawn p && p.IsPrisonerOfColony);
                foreach (var item in list2)
                {
                    Pawn p = (Pawn)item;
                    Log.Message(string.Format("{0}'s captured jim....", p.LabelShortCap));
                }
            }
            bool flag2 = list.Any(x => x is Corpse p);
            if (flag2)
            {
                List<Thing> list2 = map.thingGrid.ThingsListAt(c).FindAll(x => x is Corpse p);
                foreach (var item in list2)
                {
                    Corpse pc = (Corpse)item;
                    Pawn p = pc.InnerPawn;
                    Log.Message(string.Format("{0}'s dead jim....", p.LabelShortCap));
                }
            }
        }
    }
}
