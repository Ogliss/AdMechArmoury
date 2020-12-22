using System;
using System.Collections.Generic;
using AdeptusMechanicus.HarmonyInstance;
using AdeptusMechanicus.settings;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
    // Token: 0x02000F94 RID: 3988
    [StaticConstructorOnStartup]
    public class Dialog_CompatabilityPatches : Window
	{
		// Token: 0x1700119E RID: 4510
		// (get) Token: 0x060062AE RID: 25262 RVA: 0x001EDFFD File Offset: 0x001EC1FD
		public override Vector2 InitialSize
		{
			get
			{
				return new Vector2(1024f, (float)UI.screenHeight);
			}
		}

		// Token: 0x060062B7 RID: 25271 RVA: 0x00224580 File Offset: 0x00222780
		public Dialog_CompatabilityPatches()
		{
			this.forcePause = true;
			this.absorbInputAroundWindow = true;
			this.soundAppear = SoundDefOf.CommsWindow_Open;
			this.soundClose = SoundDefOf.CommsWindow_Close;
		}
		
		public Dialog_CompatabilityPatches(List<PatchDescription> patches)
		{
			this.forcePause = true;
			this.absorbInputAroundWindow = true;
			this.soundAppear = SoundDefOf.CommsWindow_Open;
			this.soundClose = SoundDefOf.CommsWindow_Close;
			this.patches = patches;
		}

		private List<PatchDescription> patches = new List<PatchDescription>();

		// Token: 0x060062B8 RID: 25272 RVA: 0x0022462C File Offset: 0x0022282C
		public override void PostOpen()
		{
			base.PostOpen();
		}

		// Token: 0x060062BA RID: 25274 RVA: 0x002248A0 File Offset: 0x00222AA0
		public override void DoWindowContents(Rect inRect)
		{
			GUI.BeginGroup(inRect);
			inRect = inRect.AtZero();
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;
			float num = inRect.width - 590f;
			Rect position = new Rect(num, 0f, inRect.width - num, 58f);
			GUI.BeginGroup(position);
			Text.Font = GameFont.Medium;
			Rect rect = new Rect(0f, 0f, position.width / 2f, position.height);
			Text.Anchor = TextAnchor.UpperLeft;
			Rect rect2 = new Rect(position.width / 2f, 0f, position.width / 2f, position.height);
			Text.Anchor = TextAnchor.UpperRight;
			string text = "AMAA_AstarteChapterSettings".Translate();
			if (Text.CalcSize(text).x > rect2.width)
			{
				Text.Font = GameFont.Small;
				text = text.Truncate(rect2.width, null);
			}
			Widgets.Label(rect2, text);
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperRight;
			Widgets.Label(new Rect(position.width / 2f, 27f, position.width / 2f, position.height / 2f), "AMAA_AstarteAvailableChapters".Translate());
			Text.Anchor = TextAnchor.UpperLeft;
			GUI.EndGroup();

			float num2 = 0f;
			Rect mainRect = new Rect(0f, 58f + num2, inRect.width, inRect.height - 58f - 38f - num2 - 20f);
			this.FillMainRect(mainRect);
			GUI.EndGroup();
		}

		// Token: 0x060062BB RID: 25275 RVA: 0x00224FD6 File Offset: 0x002231D6
		public override void Close(bool doCloseSound = true)
		{
			DragSliderManager.ForceStop();
			base.Close(doCloseSound);
		}

		// Token: 0x060062BC RID: 25276 RVA: 0x00224FE4 File Offset: 0x002231E4
		private void FillMainRect(Rect mainRect)
		{
			Text.Font = GameFont.Small;
			float height = 6f + (float)patches.Count * 30f;
			Rect viewRect = new Rect(0f, 0f, mainRect.width - 16f, height);
			Widgets.BeginScrollView(mainRect, ref this.scrollPosition, viewRect, true);
			float num = 6f;
			float num2 = this.scrollPosition.y - 30f;
			float num3 = this.scrollPosition.y + mainRect.height;
			int num4 = 0;
			for (int i = 0; i < patches.Count; i++)
			{
				if (num > num2 && num < num3)
				{
					Rect rect = new Rect(0f, num, viewRect.width, 30f);
				}
				num += 30f;
				num4++;

			}
			Widgets.EndScrollView();
		}

		public static void CheckboxLabeled(Rect rect, string label, ref bool checkOn, bool disabled = false, Texture2D texChecked = null, Texture2D texUnchecked = null, bool placeCheckboxNearText = false)
		{
			TextAnchor anchor = Text.Anchor;
			Text.Anchor = TextAnchor.MiddleLeft;
			if (placeCheckboxNearText)
			{
				rect.width = Mathf.Min(rect.width, Text.CalcSize(label).x + 24f + 10f);
			}
			Widgets.Label(rect, label);
			if (!disabled && Widgets.ButtonInvisible(rect, true))
			{
				checkOn = !checkOn;
				if (checkOn)
				{
					SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera(null);
				}
				else
				{
					SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera(null);
				}
			}
			CheckboxDraw(rect.x + rect.width - 24f, rect.y, checkOn, disabled, 24f, texChecked, texUnchecked);
			Text.Anchor = anchor;
		}
		private static void CheckboxDraw(float x, float y, bool active, bool disabled, float size = 24f, Texture2D texChecked = null, Texture2D texUnchecked = null)
		{
			Color color = GUI.color;
			if (disabled)
			{
				GUI.color = InactiveColor;
			}
			Texture2D image;
			if (active)
			{
				image = ((texChecked != null) ? texChecked : Widgets.CheckboxOnTex);
			}
			else
			{
				image = ((texUnchecked != null) ? texUnchecked : Widgets.CheckboxOffTex);
			}
			GUI.DrawTexture(new Rect(x, y, size, size), image);
			if (disabled)
			{
				GUI.color = color;
			}
		}
		private static readonly Color InactiveColor = new Color(0.37f, 0.37f, 0.37f, 0.8f);
		public static void TextFieldNumericLabeled<T>(Rect rect, string label, ref T val, ref string buffer, float min = 0f, float max = 1E+09f, string tooltip = null, float textpart = 0.75f, float boxpart = 0.25f) where T : struct
		{
			Rect rect2 = rect.LeftPart(textpart).Rounded();
			Rect rect3 = rect.RightPart(boxpart).Rounded();
			TextAnchor anchor = Text.Anchor;
			Text.Anchor = TextAnchor.MiddleLeft;
			Widgets.Label(rect2, label);
			if (tooltip != null)
			{
				TooltipHandler.TipRegion(rect2, tooltip);
			}
			Text.Anchor = anchor;
			Widgets.TextFieldNumeric(rect3, ref val, ref buffer, min, max);
		}

		public static readonly Texture2D RangeMatch = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/RangeMatch", true);
		// Token: 0x060062BE RID: 25278 RVA: 0x00010451 File Offset: 0x0000E651
		public override bool CausesMessageBackground()
		{
			return true;
		}

		// Token: 0x040035F8 RID: 13816
		private Vector2 scrollPosition = Vector2.zero;

		// Token: 0x040035F9 RID: 13817
		public static float lastCurrencyFlashTime = -100f;



		// Token: 0x04003619 RID: 13849
		protected static readonly Vector2 AcceptButtonSize = new Vector2(160f, 40f);

		// Token: 0x0400361A RID: 13850
		protected static readonly Vector2 OtherBottomButtonSize = new Vector2(160f, 40f);

	}
}
