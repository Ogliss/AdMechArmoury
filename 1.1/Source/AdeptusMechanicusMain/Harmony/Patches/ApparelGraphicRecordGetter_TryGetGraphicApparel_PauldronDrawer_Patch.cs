using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus.HarmonyInstance
{
	// Token: 0x020001AF RID: 431
	[HarmonyPatch(typeof(ApparelGraphicRecordGetter), "TryGetGraphicApparel")]
	public static class ApparelGraphicRecordGetter_TryGetGraphicApparel_PauldronDrawer_Patch
	{

		// Token: 0x060008F6 RID: 2294 RVA: 0x0004A904 File Offset: 0x00048B04
		[HarmonyPostfix]
		public static void Postfix(Apparel apparel, BodyTypeDef bodyType, ref ApparelGraphicRecord rec)
		{
			/*
			bool Pauldron = apparel.TryGetComp<CompPauldronDrawer>() != null;
			if (Pauldron)
			{
			//	Log.Message("Updating pad graphics for "+apparel.LabelShortCap);
				for (int i = 0; i < apparel.GetComps<CompPauldronDrawer>().Count(); i++)
				{
				//	Log.Message("Pauldron drawer "+(i+1));
					CompPauldronDrawer comp = apparel.GetComps<CompPauldronDrawer>().ElementAt(i);
					if (!comp.activeEntries.NullOrEmpty())
					{
						for (int i2 = 0; i2 < comp.activeEntries.Count; i2++)
						{
						//	Log.Message("Entry drawer " + (i2 + 1));
							comp.activeEntries[i2].UpdatePadGraphic();
						}
					}
				}
			}
			*/
			CompColorableTwo compColorable = apparel.TryGetComp<CompColorableTwo>();
			if (compColorable!=null)
			{
				string comptype = "CompColorableTwo Active: " + compColorable.Active + ", ActiveTwo: " + compColorable.ActiveTwo;
				string msg = string.Empty;
				string msk = "m";
				CompFactionColorableTwo factionColors = compColorable as CompFactionColorableTwo;
				Color colorOne = compColorable.Color;
				Color colorTwo = compColorable.ColorTwo;
				if (factionColors != null)
				{
                    if (apparel.Wearer.Faction != null)
					{
                        if (apparel.Wearer.Faction != Faction.OfPlayer)
						{
							factionColors.FactionDef = apparel.Wearer.Faction?.def;
							msg += (" entry for Non Player Pawn using FactionDef " + factionColors.FactionDef);
						}
                        else
						{
							CompPauldronDrawer pauldrons = apparel.TryGetComp<CompPauldronDrawer>();
                            if (pauldrons != null)
                            {
                                for (int i = 0; i < pauldrons.activeEntries.Count; i++)
                                {
									ShoulderPadEntry entry = pauldrons.activeEntries[i];
                                    if (entry.faction !=null && entry.UseFactionTextures || entry.UseFactionColors)
									{
										factionColors.FactionDef = entry.faction;
										msg += (" entry for Non Player Pawn using FactionDef " + factionColors.FactionDef);
										break;
									}

								}
                            }
                            else
                            {
							//	Log.Message("CompFactionColorableTwo Player Pawn no CompPauldronDrawer");
                            }
						}
					}
                    if (factionColors.Active)
					{
					//	Log.Message("factionColors.Active");
						colorOne = factionColors.Color;
						apparel.SetColorOne(colorOne);
					}
                    if (factionColors.ActiveTwo)
					{
					//	Log.Message("factionColors.ActiveTwo");
						colorTwo = factionColors.ColorTwo;
						apparel.SetColorTwo(colorTwo);
					}
                    if (factionColors.Extension != null)
					{
					//	Log.Message("factionColors.Extension != null");
						if (factionColors.ActiveTwo || factionColors.Active)
						{
                            if (!factionColors.Extension.factionMaskTag.NullOrEmpty())
							{
							//	Log.Message("factionColors.factionMaskTag");
								msk = msk +"_" + factionColors.Extension.factionMaskTag;
							//	Log.Message("factionMaskTag: "+msk);
							}
						}
					}
					// msg
					comptype = "CompFactionColorableTwo Active: " + factionColors.Active + ", ActiveTwo: " + factionColors.ActiveTwo;
				}
				Graphic newgraphic = rec.graphic.GetColoredVersion(rec.graphic.Shader, colorOne, colorTwo);
				Texture texture;
				if (!apparel.def.apparel.wornGraphicPath.NullOrEmpty())
				{
					if (!msk.NullOrEmpty())
					{
						texture = ContentFinder<Texture2D>.Get(rec.graphic.path + "_east" + msk, false);
						if (texture != null)
						{
							newgraphic.MatEast.SetTexture(ShaderPropertyIDs.MaskTex, texture);
						}
						newgraphic.MatEast.SetColor(ShaderPropertyIDs.ColorTwo, colorTwo);

						texture = ContentFinder<Texture2D>.Get(rec.graphic.path + "_west" + msk, false);
						if (texture != null)
						{
							newgraphic.MatWest.SetTexture(ShaderPropertyIDs.MaskTex, texture);
						}
						newgraphic.MatWest.SetColor(ShaderPropertyIDs.ColorTwo, colorTwo);

						texture = ContentFinder<Texture2D>.Get(rec.graphic.path + "_south" + msk, false);
						if (texture != null)
						{
							newgraphic.MatSouth.SetTexture(ShaderPropertyIDs.MaskTex, texture);
						}
						newgraphic.MatSouth.SetColor(ShaderPropertyIDs.ColorTwo, colorTwo);

						texture = ContentFinder<Texture2D>.Get(rec.graphic.path + "_north" + msk, false);
                        if (texture != null)
						{
							newgraphic.MatNorth.SetTexture(ShaderPropertyIDs.MaskTex, texture);
						}
						newgraphic.MatNorth.SetColor(ShaderPropertyIDs.ColorTwo, colorTwo);
						/*
						ExtendedGraphicData newdata = new ExtendedGraphicData();
						newdata.graphicClass = typeof(Graphic_MultiMask);
						Log.Message("ExtendedGraphicData");
						newdata.texPath = rec.graphic.path;
						newdata.MaskSelector = msk;
						newdata.maskKey = olddata.maskKey;
						newgraphic = GraphicDatabase.Get<Graphic_MultiMask>(rec.graphic.path, rec.graphic.Shader, apparel.def.graphicData.drawSize, colorOne, colorTwo, newdata);
						*/
					}
					rec = new ApparelGraphicRecord(newgraphic, apparel);
				}
				texture = ContentFinder<Texture2D>.Get(rec.graphic.path +msk, false);
				if (texture != null)
				{
					newgraphic.MatSingle.SetTexture(ShaderPropertyIDs.MaskTex, texture);
				}
				newgraphic.MatEast.SetColor(ShaderPropertyIDs.ColorTwo, colorTwo);

				//	Log.Message(comptype + msg + " present on " + apparel.Wearer +"'s "+ apparel + " colorOne: " + colorOne + ", colorTwo: " + colorTwo);
			}
			if (!apparel.def.apparel.wornGraphicPath.NullOrEmpty())
			{
				if (apparel.def.GetModExtension<ApparelRestrictionDefExtension>() is ApparelRestrictionDefExtension apparelExt)
				{
					if (!apparelExt.raceSpecifics.NullOrEmpty())
					{
						foreach (var item in apparelExt.raceSpecifics)
						{
							ThingDef RaceDef = DefDatabase<ThingDef>.GetNamedSilentFail(item.raceDef);
                            if (RaceDef == null)
                            {
								continue;
                            }
							if (RaceDef == apparel.Wearer.def)
							{
								if (!item.texPath.NullOrEmpty())
								{
									string path;
									if (apparel.def.apparel.LastLayer == ApparelLayerDefOf.Overhead || PawnRenderer.RenderAsPack(apparel) || apparel.def.apparel.wornGraphicPath == BaseContent.PlaceholderImagePath)
									{
										path = apparel.def.apparel.wornGraphicPath + "_" + item.texPath;
									}
									else
									{
										path = apparel.def.apparel.wornGraphicPath + "_" + item.texPath + "_" + bodyType.defName;
									}
									Shader shader = ShaderDatabase.Cutout;
									if (apparel.def.apparel.useWornGraphicMask)
									{
										shader = ShaderDatabase.CutoutComplex;
									}
									Graphic graphic = GraphicDatabase.Get<Graphic_Multi>(path, shader, apparel.def.graphicData.drawSize, rec.graphic.color, rec.graphic.colorTwo);
									rec = new ApparelGraphicRecord(graphic, apparel);

								}
								break;
							}
						}
					}
				}
			}
			/*
            for (int i = 0; i < apparel.AllComps.Count; i++)
            {
				CompPauldronDrawer drawer = apparel.AllComps[i] as CompPauldronDrawer;
                if (drawer != null)
                {
					drawer.
                }

			}
			*/
			apparel.BroadcastCompSignal(CompPauldronDrawer.UpdateString);
		}

	}
}
