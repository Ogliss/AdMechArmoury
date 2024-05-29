using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public class Dialog_FactionSpawningSettlements : Window
    {
        private readonly Action<int, int> spawnCallback;

        private const float newFactionSettlementFactor = 0.7f; // recommendation
        private const float settlementsPer100KTiles = 80; // average

        private int settlementsToSpawn;
        private int settlementsRecommended;
        private int distanceToSpawn;
        private int distanceRecommended;

        public static void OpenDialog(Action<int, int> spawnCallback)
        {
            Find.WindowStack.Add(new Dialog_FactionSpawningSettlements(spawnCallback));
        }

        private Dialog_FactionSpawningSettlements(Action<int, int> spawnCallback)
        {
            doCloseButton = false;
            forcePause = true;
            absorbInputAroundWindow = true;
            this.spawnCallback = spawnCallback;

            settlementsToSpawn = settlementsRecommended = GetSettlementsRecommendation();
            distanceToSpawn = distanceRecommended = SettlementProximityGoodwillUtility.MaxDist;

            // You can enable this again, in case the issue arises that factions always spawn with a single settlement
            //if  (GenTypes.GetTypeInAnyAssembly("FactionControl.Controller", "FactionControl") != null) FactionControlFix();
        }

        private static int GetSettlementsRecommendation()
        {
            int existingFactions = Find.FactionManager.AllFactionsVisible.Count();
            return GenMath.RoundRandom(Find.WorldGrid.TilesCount / 100000f * settlementsPer100KTiles / existingFactions * newFactionSettlementFactor);
        }

        public override void DoWindowContents(Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect.AtZero());

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;

            // Amount
            listing_Standard.Label("AdeptusMechanicus.FactionSettlementsToSpawn".Translate(settlementsRecommended, settlementsToSpawn));

            settlementsToSpawn = Mathf.CeilToInt(listing_Standard.Slider(settlementsToSpawn, 1, Mathf.Max(settlementsRecommended * 4, 1)));

            // Distance from player
            listing_Standard.Label("AdeptusMechanicus.FactionMinDistance".Translate(distanceRecommended, distanceToSpawn));
            distanceToSpawn = Mathf.CeilToInt(listing_Standard.Slider(distanceToSpawn, 1, distanceRecommended * 2));

            if (listing_Standard.ButtonText("AdeptusMechanicus.FactionButtonSpawn".Translate())) Spawn();
            if (listing_Standard.ButtonText("AdeptusMechanicus.FactionButtonCancel".Translate())) Close();

            listing_Standard.End();
        }

        private void Spawn()
        {
            Close();
            spawnCallback(settlementsToSpawn, distanceToSpawn);
        }
    }
}
