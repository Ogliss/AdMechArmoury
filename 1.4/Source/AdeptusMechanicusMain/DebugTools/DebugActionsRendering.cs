using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    [StaticConstructorOnStartup]
	public static class DebugActionsRendering
    {
        [DebugAction("Adeptus Mechanicus", allowedGameStates = AllowedGameStates.Playing)]
        public static void DumpPawnAtlasByDirIndividual()
        {
            List<DebugMenuOption> list = new List<DebugMenuOption>();

            for (int i = 7; i <= 16; i++)
            {
                int value = Mathf.RoundToInt(Mathf.Pow(2, i));
                list.Add(new DebugMenuOption(value.ToString(CultureInfo.CurrentCulture), DebugMenuOptionMode.Action, () =>
                {
                    LongEventHandler.QueueLongEvent(() => DumpAtlasIndividual(value), "Creating Pawn Atlas", false, null);
                }));
            }

            Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
        }

        public static void DumpAtlasIndividual(int atlasSize)
        {
            Rect uvRect = new Rect(0, 0, atlasSize, atlasSize);

            Pawn[] colonistsInOrder = Find.ColonistBar.GetColonistsInOrder().ToArray();

            string path = Application.dataPath + "\\atlasDump_PawnsByDirIndividual";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            else
                foreach (string file in Directory.GetFiles(path))
                    File.Delete(file);

            for (int c = 0; c < colonistsInOrder.Length; c++)
            {
                Pawn pawn = colonistsInOrder[c];

                for (int i = 0; i < 4; i++)
                {
                    Rot4 rot = new Rot4(i);
                    RenderTexture texture = new RenderTexture(atlasSize, atlasSize, 24, RenderTextureFormat.ARGB32, 0)
                    {
                        name = $"{((pawn.Name as NameTriple)?.Nick ?? pawn.Name.ToStringShort).Replace("-", "_")}_{rot.ToStringHuman().ToLower()}"
                    };

                    Find.PawnCacheCamera.rect = uvRect;
                    Find.PawnCacheRenderer.RenderPawn(pawn, texture, Vector3.zero, 1f, 0f, rot);
                    Find.PawnCacheCamera.rect = new Rect(0f, 0f, 1f, 1f);

                    TextureAtlasHelper.WriteDebugPNG(texture, $"{path}\\{texture.name}.png");
                }
            }
        }
    }
}
