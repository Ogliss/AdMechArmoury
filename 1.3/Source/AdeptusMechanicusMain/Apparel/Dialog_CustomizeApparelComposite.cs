using System;
using System.Collections.Generic;
using System.Linq;
using AdeptusMechanicus.ExtensionMethods;
using AdeptusMechanicus.HarmonyInstance;
using AdeptusMechanicus.settings;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
	[StaticConstructorOnStartup]
    public class Dialog_CustomizeApparelComposite : Window
	{
		public override Vector2 InitialSize
		{
			get
			{
				return new Vector2(1024f, (float)UI.screenHeight * 0.75f);
			}
		}

		public Dialog_CustomizeApparelComposite(Thing thing)
		{
			if (Things == null) Things = new List<Thing>();
			if (thing is Apparel apparel && apparel.Wearer is Pawn pawn)
			{
				foreach (Apparel item in pawn.apparel.WornApparel)
				{
				//	Log.Message(pawn + ": wearing " + item);
					if (!Things.Contains(item) && AdeptusApparelUtility.CanCustomizeApparel(item))
					{
					//	Log.Message(item + ": Customizeable");
						Things.Add(item);
					}
				}
			}
			this.thing = thing;
			this.thingInd = Things.IndexOf(thing);
			this.forcePause = true;
			this.doCloseButton = true;
			this.absorbInputAroundWindow = true;
			this.soundAppear = SoundDefOf.CommsWindow_Open;
			this.soundClose = SoundDefOf.CommsWindow_Close;
		}
		private int thingInd = 0;

        private readonly List<Thing> Things;
		public override void PostOpen()
		{
			base.PostOpen();
		}


		public override void DoWindowContents(Rect inRect)
		{
			GUI.BeginGroup(inRect);
			inRect = inRect.AtZero();
			Text.Font = GameFont.Medium;
			Text.Anchor = TextAnchor.UpperLeft;
			Widgets.Label(new Rect(0f, 17f, inRect.width / 3f, 40), thing.def.LabelCap);
			float num = inRect.width - 590f;
			Rect position = new Rect(num, 0f, inRect.width - num, 58f);
			GUI.BeginGroup(position);
			Text.Font = GameFont.Medium;
			Rect rect = new Rect(0f, 0f, position.width / 2f, position.height);
			Text.Anchor = TextAnchor.UpperLeft;
			Rect rect2 = new Rect(position.width / 2f, 0f, position.width / 2f, position.height);
			Text.Anchor = TextAnchor.UpperRight;
			string text = "AdeptusMechanicus.Customization".Translate();
			if (Text.CalcSize(text).x > rect2.width)
			{
				Text.Font = GameFont.Small;
				text = text.Truncate(rect2.width, null);
			}
			Widgets.Label(rect2, text);
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperRight;
			Widgets.Label(new Rect(position.width / 2f, 27f, position.width / 2f, position.height / 2f), "AdeptusMechanicus.ApparelCustomizeationOptionsDesc".Translate());
			Text.Anchor = TextAnchor.UpperLeft;
			GUI.EndGroup();

			float num2 = 0f;
			Rect mainRect = new Rect(0f, 58f + num2, inRect.width, inRect.height - 58f - 38f - num2 - 20f);
			this.FillMainRect(mainRect);
			GUI.EndGroup();
			Rect rect3 = this.windowRect.AtZero();


            if (Things.Count > 1)
			{
				if (BackButtonFor(new Rect((rect3.width / 2f - Dialog_CustomizeApparelComposite.CloseButSize.x / 2f) - Dialog_CustomizeApparelComposite.CloseButSize.x - 20f, rect3.height - 74f, Dialog_CustomizeApparelComposite.CloseButSize.x * 0.9f, Dialog_CustomizeApparelComposite.CloseButSize.y * 0.9f)))
				{
					this.Thing = this.Things[thingInd == 0 ? Things.Count -1 : thingInd - 1];
				}
				if (NextButtonFor(new Rect((rect3.width / 2f - Dialog_CustomizeApparelComposite.CloseButSize.x / 2f) + Dialog_CustomizeApparelComposite.CloseButSize.x - 5f, rect3.height - 74f, Dialog_CustomizeApparelComposite.CloseButSize.x * 0.9f, Dialog_CustomizeApparelComposite.CloseButSize.y * 0.9f)))
				{
					this.Thing = this.Things[thingInd == Things.Count -1 ? 0 : thingInd + 1];
				}
			}

		}

		public static bool BackButtonFor(Rect rectToBack)
		{
			return Widgets.ButtonText(rectToBack, "Back".Translate(), true, true, true);
		}
		public static bool NextButtonFor(Rect rectToBack)
		{
			return Widgets.ButtonText(rectToBack, "Next".Translate(), true, true, true);
		}
		public override void Close(bool doCloseSound = true)
		{
			DragSliderManager.ForceStop();
			base.Close(doCloseSound);
		}

		private static readonly Vector2 PawnPortraitSize = new Vector2(256f, 256f) * 1.5f;

		private void FillMainRect(Rect mainRect)
		{
			Apparel apparel = this.thing as Apparel;
			Text.Font = GameFont.Small;
			float height = 6f + 1 * 30f;
			Rect scrollRect = mainRect.LeftHalf();
			Rect previewRect = mainRect.RightHalf().TopPart(0.75f);
			Rect scrollViewRect = new Rect(0f, 0f, scrollRect.width - 16f, height);
            if (apparel?.Wearer is Pawn Wearer)
			{
				/*
				Rect north = previewRect.TopHalf().RightHalf(); // new Rect(previewRect.center.x - PawnPortraitSize.x / 2f, previewRect.yMin - 24f, PawnPortraitSize.x, PawnPortraitSize.y)
				Rect South = previewRect.TopHalf().LeftHalf();
				Rect east = previewRect.BottomHalf().RightHalf();
				Rect west = previewRect.BottomHalf().LeftHalf();
				*/
				GUI.DrawTexture(previewRect.TopHalf().LeftHalf(), PortraitsCache.Get(Wearer, PawnPortraitSize * 0.75f, Rot4.South, default(Vector2), 1f, true, true));
				GUI.DrawTexture(previewRect.TopHalf().RightHalf(), PortraitsCache.Get(Wearer, PawnPortraitSize * 0.75f, Rot4.North, default(Vector2), 1f, true, true));
				GUI.DrawTexture(previewRect.BottomHalf().RightHalf(), PortraitsCache.Get(Wearer, PawnPortraitSize * 0.75f, Rot4.East, default(Vector2), 1f, true, true));
				GUI.DrawTexture(previewRect.BottomHalf().LeftHalf(), PortraitsCache.Get(Wearer, PawnPortraitSize * 0.75f, Rot4.West, default(Vector2), 1f, true, true));
			}
            else
			{
				Widgets.ThingIcon(new Rect(previewRect.center.x - PawnPortraitSize.x / 2f, previewRect.yMin - 24f, PawnPortraitSize.x, PawnPortraitSize.y), this.thing, 1f);
			}
			Widgets.BeginScrollView(scrollRect, ref this.scrollPosition, scrollViewRect, true);
			float num = 6f;
			float num2 = this.scrollPosition.y - 30f;
			float num3 = this.scrollPosition.y + scrollRect.height;
			int num4 = 0;
            if (this.thing is ApparelComposite composite)
            {
                if (composite.ColorableTwo != null)
				{
                    if (composite.ColorableFaction != null)
					{
						if (num > num2 && num < num3)
						{
							Rect rect = new Rect(0f, num, scrollViewRect.width, 30f);
							AdeptusApparelUtility.DrawFactionColorsButton(rect, composite.ColorableFaction, false);
						}
						num += 30f;
						num4++;
					}
					float spacer = 120f;
					if (num > num2 && num < num3)
					{
						Rect rect = new Rect(0f, num, scrollViewRect.width, spacer);
						AdeptusApparelUtility.DrawBaseColourOptions(rect, "Colours", composite);
					}
					num += spacer;
					num4++;
				}
                if (!composite.AltGraphics.NullOrEmpty())
				{
					if (num > num2 && num < num3)
					{
						Rect rect = new Rect(0f, num, scrollViewRect.width, 30f);
						AdeptusApparelUtility.DrawBaseTextureOptions(rect, "Main Texture:", composite);
					}
					num += 30f;
					num4++;
				}
                if (!composite.Pauldrons.NullOrEmpty())
                {
                    for (int i = 0; i < composite.Pauldrons.Count; i++)
                    {
						CompPauldronDrawer Drawer = composite.Pauldrons[i];
						if (!Drawer.activeEntries.EnumerableNullOrEmpty())
						{
							foreach (ShoulderPadEntry entry in Drawer.activeEntries)
							{
								if (entry.Options.NullOrEmpty() && (entry.useVariableTextures || entry.useFactionTextures))
								{
									if (entry.Options.NullOrEmpty())
									{
										if (Prefs.DevMode) Log.Message(entry.Label + " no options");
										continue;
									}
								}
								if (entry.Drawer == null)
								{
									//entry.drawer = Drawer;
								}
								if (entry.useFactionTextures)
								{
									Rect rect = new Rect(0f, num, scrollViewRect.width, 30f);
									AdeptusApparelUtility.DrawFactionButton(rect, Drawer, entry, false);
									num += 30f;
									num4++;
								}
								else
								if (entry.useVariableTextures)
								{
									Rect rect = new Rect(0f, num, scrollViewRect.width, 30f);
									AdeptusApparelUtility.DrawVariantButton(rect, Drawer, entry, false);
									num += 30f;
									num4++;
								}
							}
						}

					}
                }
				/*
                if (!composite.Extras.NullOrEmpty())
				{
					for (int i = 0; i < composite.Extras.Count; i++)
					{
						CompApparelExtraPartDrawer Drawer = composite.Extras[i];
						ExtraApparelPartProps entry = Drawer.ExtraPartEntry;
						if (entry.Options.NullOrEmpty() && (entry.UseVariableTextures || entry.UseFactionTextures))
						{
							if (entry.Options.NullOrEmpty())
							{
								if (Prefs.DevMode) Log.Message(entry.Label + " no options");
								continue;
							}
						}
						if (entry.Drawer == null)
						{
							//entry.drawer = Drawer;
						}
						if (entry.UseFactionTextures)
						{
							Rect rect = new Rect(0f, num, scrollViewRect.width, 30f);
							ITab_ToggleLivelry.DrawFactionButton(rect, Drawer, entry, false);
							num += 30f;
							num4++;
						}
						if (entry.UseVariableTextures)
						{
							Rect rect = new Rect(0f, num, scrollViewRect.width, 30f);
							ITab_ToggleLivelry.DrawVariantButton(rect, Drawer, entry, false);
							num += 30f;
							num4++;
						}

					}
				}
				*/
			}
			/*
			for (int i = 0; i < 10; i++)
			{
				if (num > num2 && num < num3)
				{
					Rect rect = new Rect(0f, num, scrollViewRect.width, 30f);
					DrawSettings(rect, "box");
				}
				num += 30f;
				num4++;
			}
			*/
			Widgets.EndScrollView();
		}

		public void DrawSettings(Rect rect, string label)
		{
			float w = rect.width / 5;
			float w2 = (rect.width - w) / 4;
			float y = rect.y;
			float x = rect.x;
			Rect rectLabel = new Rect(x, y, w, 30f);
			Widgets.Label(rectLabel, label);
			x += w;

		}

		public override bool CausesMessageBackground()
		{
			return true;
		}

		protected Thing Thing
        {
            get
            {
				return thing;
            }
            set
            {
                if (value == thing)
                {
					return;
                }
				thing = value; 
				thingInd = Things.IndexOf(this.thing);
			}
        }

        public override void PostClose()
		{
			Apparel apparel = this.thing as Apparel;
            if (apparel?.Wearer != null)
			{
				AdeptusApparelUtility.UpdateApparelGraphicsFor(apparel.Wearer);

				apparel.Wearer.Drawer.renderer.graphics.SetAllGraphicsDirty();
				PortraitsCache.SetDirty(apparel.Wearer);
			}
			base.PostClose();
        }

        private Thing thing = null;
		private Vector2 scrollPosition = Vector2.zero;
		public static float lastCurrencyFlashTime = -100f;
		protected static readonly Vector2 AcceptButtonSize = new Vector2(160f, 40f);
		protected static readonly Vector2 OtherBottomButtonSize = new Vector2(160f, 40f);
		protected static readonly Vector2 BottomButSize = new Vector2(150f, 38f);
		public const float BottomButHeight = 38f;

	}
}
