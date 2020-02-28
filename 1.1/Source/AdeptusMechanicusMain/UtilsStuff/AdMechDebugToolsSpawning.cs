using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using Verse.AI.Group;

namespace Verse
{
    // Token: 0x02000330 RID: 816
    public static class AdMechDebugToolsSpawning
    {
        // Token: 0x0600186F RID: 6255 RVA: 0x0008C8C8 File Offset: 0x0008AAC8
        [DebugAction("Spawning", null, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void SpawnDropship()
        {
            List<DebugMenuOption> list = new List<DebugMenuOption>();
            list.Add(new DebugMenuOption("Incoming", DebugMenuOptionMode.Tool, delegate ()
            {
                GenPlace.TryPlaceThing(SkyfallerMaker.MakeSkyfaller(ThingDefOf.ShuttleIncoming, ThingMaker.MakeThing(ThingDefOf.Shuttle, null)), UI.MouseCell(), Find.CurrentMap, ThingPlaceMode.Near, null, null, default(Rot4));
            }));
            list.Add(new DebugMenuOption("Stationary", DebugMenuOptionMode.Tool, delegate ()
            {
                GenPlace.TryPlaceThing(ThingMaker.MakeThing(ThingDefOf.Shuttle, null), UI.MouseCell(), Find.CurrentMap, ThingPlaceMode.Near, null, null, default(Rot4));
            }));
            List<DebugMenuOption> options = list;
            Find.WindowStack.Add(new Dialog_DebugOptionListLister(options));
        }
    }
}
