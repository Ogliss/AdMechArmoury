using AlienRace;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static AlienRace.AlienPartGenerator;
// AdeptusMechanicus.LinkedBodyAddon
namespace AdeptusMechanicus
{
    public class LinkedBodyAddon : BodyAddon
    {
        public bool useDefautZeroOffset = true;
        public bool linkLifeStageDrawSize = false;
        public List<List<int>> allowPairWith = null;


        public override Graphic GetPath(Pawn pawn, ref int sharedIndex, int? savedIndex = null)
        {
			string empty = string.Empty;
			int num = 0;
			using (List<BodyAddonPrioritization>.Enumerator enumerator = Prioritization.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					switch (enumerator.Current)
					{
						case BodyAddonPrioritization.Backstory:
							{
								BodyAddonBackstoryGraphic bodyAddonBackstoryGraphic = backstoryGraphics?.FirstOrDefault((BodyAddonBackstoryGraphic babgs) => pawn.story.AllBackstories.Any((Backstory bs) => bs.identifier == babgs.backstory));
								if (bodyAddonBackstoryGraphic != null)
								{
									empty = bodyAddonBackstoryGraphic.path;
									num = bodyAddonBackstoryGraphic.variantCount;
								}
								break;
							}
						case BodyAddonPrioritization.Hediff:
							if (!hediffGraphics.NullOrEmpty())
							{
								foreach (BodyAddonHediffGraphic bahg in hediffGraphics)
								{
									using (IEnumerator<Hediff> enumerator3 = pawn.health.hediffSet.hediffs.Where((Hediff h) => h.def == bahg.hediff && (h.Part == null || bodyPart.NullOrEmpty() || h.Part.untranslatedCustomLabel == bodyPart || h.Part.def.defName == bodyPart)).GetEnumerator())
									{
										if (enumerator3.MoveNext())
										{
											Hediff current = enumerator3.Current;
											empty = bahg.path;
											num = bahg.variantCount;
											if (!bahg.severity.NullOrEmpty())
											{
												foreach (BodyAddonHediffSeverityGraphic item in bahg.severity)
												{
													if (current.Severity >= item.severity)
													{
														empty = item.path;
														num = item.variantCount;
														break;
													}
												}
											}
										}
									}
								}
							}
							break;
						default:
							throw new ArrayTypeMismatchException();
					}
					if (!empty.NullOrEmpty())
					{
						break;
					}
				}
			}
			if (empty.NullOrEmpty())
			{
				empty = path;
				num = variantCount;
			}
			if (num <= 0)
			{
				num = 1;
			}
			ExposableValueTuple<Color, Color> channel = pawn.GetComp<AlienComp>().GetChannel(ColorChannel);
			if (empty.NullOrEmpty())
			{
				return null;
			}
			int num2;
			Rand.PushState();
			string varPath = (((num2 = (savedIndex.HasValue ? (sharedIndex = savedIndex.Value % num) : (linkVariantIndexWithPrevious ? (sharedIndex % num) : (sharedIndex = Rand.Range(0, num))))) == 0) ? "" : num2.ToString());
			Rand.PopState();
			if (!allowPairWith.NullOrEmpty())
			{
                foreach (List<int> item in allowPairWith)
                {
                    if (item.Contains(num2))
                    {
						sharedIndex = item.RandomElement();
					//	Log.Message("setting shared index to " + sharedIndex);
						break;
					}
                }
			}
			return GraphicDatabase.Get<Graphic_Multi_RotationFromData>(empty += varPath , (ContentFinder<Texture2D>.Get(empty + "_northm", reportFailure: false) == null) ? ShaderType.Shader : ShaderDatabase.CutoutComplex, drawSize * 1.5f, channel.first, channel.second, new GraphicData
			{
				drawRotated = !drawRotated
			});
		}
    }
}
