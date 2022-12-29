using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using AdeptusMechanicus.ExtensionMethods;
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
		static MethodInfo drawColor = AccessTools.Property(typeof(Apparel), "DrawColor").GetGetMethod();
		static MethodInfo drawColorTwo = AccessTools.Property(typeof(Thing), "DrawColorTwo").GetGetMethod();
		static MethodInfo getGraphicOneColor = AccessTools.GetDeclaredMethods(typeof(GraphicDatabase)).First((MethodInfo mi) => mi.Name == "Get" && mi.GetParameters().Length == 4 && mi.GetParameters().Last().ParameterType == typeof(Color));
		static MethodInfo getGraphicTwoColor = AccessTools.GetDeclaredMethods(typeof(GraphicDatabase)).First((MethodInfo mi) => mi.Name == "Get" && mi.GetParameters().Length == 5 && mi.GetParameters().Last().ParameterType == typeof(Color));
		/*
		static MethodInfo getGraphicTwoColor = AccessTools.Method(typeof(GraphicDatabase), "Get", new Type[]
			{
				typeof(string),
				typeof(Shader),
				typeof(float),
				typeof(Color),
				typeof(Color)
			});
		*/
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{

			var instructionsList = new List<CodeInstruction>(instructions);

            for (int i = 0; i < instructionsList.Count; i++)
            {
				CodeInstruction instruction = instructionsList[i];
                if (instruction.opcode == OpCodes.Call && instruction.OperandIs(getGraphicOneColor.MakeGenericMethod(typeof(Graphic_Multi))))
				{
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Callvirt, drawColorTwo);
				//    Log.Message($"{i}  opcode: {instruction.opcode} operand: {instruction.operand}");
					instruction = new CodeInstruction(OpCodes.Call, getGraphicTwoColor.MakeGenericMethod(typeof(Graphic_Multi)));
				}
				yield return instruction;
			}

		}

		// Token: 0x060008F6 RID: 2294 RVA: 0x0004A904 File Offset: 0x00048B04
		[HarmonyPostfix]
		public static void Postfix(ref Apparel apparel, BodyTypeDef bodyType, ref ApparelGraphicRecord rec)
		{
			bool onHead = apparel.def.apparel.LastLayer == ApparelLayerDefOf.Overhead || PawnRenderer.RenderAsPack(apparel) || apparel.def.apparel.wornGraphicPath == BaseContent.PlaceholderImagePath;
			/*
			bool Pauldron = apparel.TryGetCompFast<CompPauldronDrawer>() != null;
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
			CompColorableTwo compColorable = apparel.TryGetCompFast<CompColorableTwo>();
			if (compColorable!=null)
			{
			//	Log.Message("CompColorableTwo "+ apparel);
				string comptype = compColorable.GetType().Name;
				string msg = string.Empty;
				string mskFaction = string.Empty;
				CompColorableTwoFaction factionColors = compColorable as CompColorableTwoFaction;
				Color colorOne = compColorable.Color;
				Color colorTwo = compColorable.ColorTwo;
				
				if (factionColors != null)
				{
                    if (apparel.Wearer?.Faction != null)
					{
                        if (apparel.Wearer.Faction != Faction.OfPlayer)
						{
							factionColors.FactionDef = apparel.Wearer.Faction?.def;
							msg += " entry for Non Player Pawn using FactionDef " + factionColors.FactionDef;
						}
                        else
						{
							if (factionColors.FactionDef != null)
							{
								msg += " entry for Player Pawn using FactionDef " + factionColors.FactionDef;
							}
                            else
							{
								/*
								CompPauldronDrawer pauldrons = apparel.TryGetCompFast<CompPauldronDrawer>();
								if (pauldrons != null)
								{
									for (int i = 0; i < pauldrons.activeEntries.Count; i++)
									{
										ShoulderPadEntry entry = pauldrons.activeEntries[i];
										if (entry.faction != null && entry.UseFactionTextures || entry.UseFactionColors)
										{
											factionColors.FactionDef = entry.faction;
											msg += " entry for Player Pawn using FactionDef " + factionColors.FactionDef;
											break;
										}

									}
								}
								else
								{
									//	Log.Message("CompFactionColorableTwo Player Pawn no CompPauldronDrawer");
								}
								*/
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
								mskFaction = "_" + factionColors.Extension.factionMaskTag;
							//	Log.Message("factionMaskTag: "+msk);
							}
						}
					}
					// msg
                    if (factionColors.ActiveFaction)
                    {
						comptype += "FactionActive: " + factionColors.FactionActive + ", FactionActiveTwo: " + factionColors.FactionActiveTwo;
					}
                    else
					{
						comptype += "Active: " + factionColors.Active + ", ActiveTwo: " + factionColors.ActiveTwo;
					}
				}
				else
				{
					comptype += "Active: " + compColorable.Active + ", ActiveTwo: " + compColorable.ActiveTwo;
				}

				//	Log.Message(comptype + msg + " present on " + apparel.Wearer +"'s "+ apparel + " colorOne: " + colorOne + ", colorTwo: " + colorTwo);
				//	Log.Message("New graphic for "+rec.sourceApparel.LabelCap+" worn by "+rec.sourceApparel.Wearer.NameShortColored+ " colorOne: "+colorOne+", colorTwo"+ colorTwo);
				bool altGraphic = false;
				bool altGraphicMask = false;
                if (rec.graphic != null)
				{
					string mskVariant = "";
					if (apparel is ApparelComposite composite)
					{
						if (!composite.AltGraphics.NullOrEmpty() && composite.ActiveAltGraphic != null)
						{
							rec.graphic = composite.ActiveAltGraphic.GetGraphic(rec.graphic, true);

							Graphic graphic = GraphicDatabase.Get<Graphic_Multi>(composite.WornGraphicPath, apparel.def.apparel.useWornGraphicMask ? ShaderDatabase.CutoutComplex : ShaderDatabase.Cutout, apparel.def.graphicData.drawSize, composite.DrawColor, composite.DrawColorTwo);
							if (!composite.ActiveAltGraphic.maskKey.NullOrEmpty())
							{
								mskVariant = "_" + composite.ActiveAltGraphic.maskKey;
								altGraphicMask = true;

                            }
							rec = new ApparelGraphicRecord(graphic, apparel);
							altGraphic = true;

                        }
					}
					Graphic newgraphic = rec.graphic.GetColoredVersion(rec.graphic.Shader, colorOne, colorTwo);
					bool replaced = false;
					if (!apparel.def.apparel.wornGraphicPath.NullOrEmpty())
					{
						Graphic replace = AdeptusApparelUtility.ApplyMask(newgraphic, apparel, colorOne, colorTwo, mskVariant, mskFaction);
						replaced = replace != null;
						if (replaced)
						{
							newgraphic = replace;
							   rec = new ApparelGraphicRecord(newgraphic, apparel);
						}
					}
					if (!rec.graphic.path.NullOrEmpty())
					{
						Texture texture = ContentFinder<Texture2D>.Get(rec.graphic.path + mskFaction, false);
						if (texture != null)
						{
							newgraphic.MatSingle.SetTexture(ShaderPropertyIDs.MaskTex, texture);
						}
						newgraphic.MatEast.SetColor(ShaderPropertyIDs.ColorTwo, colorTwo);
                        rec = new ApparelGraphicRecord(newgraphic, apparel);
                    }
				}
			//	Log.Message(comptype + msg + " present on " + apparel.Wearer +"'s "+ apparel + " colorOne: " + colorOne + ", colorTwo: " + colorTwo);
			}
			if (!apparel.def.apparel.wornGraphicPath.NullOrEmpty())
			{
				if (apparel.def.GetModExtensionFast<ApparelRestrictionDefExtension>() is ApparelRestrictionDefExtension apparelExt)
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
							if (RaceDef == apparel.Wearer?.def)
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
									Graphic graphic = GraphicDatabase.Get<Graphic_Multi>(path, apparel.def.apparel.useWornGraphicMask ? ShaderDatabase.CutoutComplex : ShaderDatabase.Cutout, apparel.def.graphicData.drawSize, rec.graphic.color, rec.graphic.colorTwo);
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
