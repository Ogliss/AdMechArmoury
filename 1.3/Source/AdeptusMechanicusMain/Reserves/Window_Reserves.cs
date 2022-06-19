using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
	public class Window_Reserves : Window
	{
		public override Vector2 InitialSize
		{
			get
			{
				return new Vector2(330f, 300f);
			}
		}

		public Window_Reserves(MapComponent_Reserves mapComp, bool isDraggable, Vector2 pos, bool showPawnList = false)
		{
			this.showPawnList = showPawnList;
			this.ReservesTracker = mapComp;
			this.pos = pos;
			this.forcePause = false;
			this.absorbInputAroundWindow = false;
			this.closeOnCancel = false;
			this.closeOnClickedOutside = false;
			this.doCloseButton = false;
			this.doCloseX = false;
			this.draggable = isDraggable;
			this.drawShadow = false;
			this.preventCameraMotion = false;
			this.resizeable = false;
			this.doWindowBackground = false;
			this.layer = WindowLayer.GameUI;
			this.WaveTip();
		}

		public override void WindowOnGUI()
		{
			bool worldRenderedNow = WorldRendererUtility.WorldRenderedNow;
			if (!worldRenderedNow)
			{
				base.WindowOnGUI();
			}
		}

		public override void PostClose()
		{
			base.PostClose();
		//	this.ReservesTracker.reservesMonitor = null;
		}

		public override void Notify_ResolutionChanged()
		{
			base.Notify_ResolutionChanged();
			this.UpdatePosition();
		}

		public override void SetInitialSizeAndPosition()
		{
			base.SetInitialSizeAndPosition();
			this.UpdatePosition();
		}

		private void UpdatePosition()
		{
			this.windowRect.x = (float)UI.screenWidth - this.pos.x - this.windowRect.width;
			this.windowRect.y = this.pos.y;
			bool flag = this.windowRect.y < 0f;
			if (flag)
			{
				this.windowRect.y = 0f;
			}
			bool flag2 = this.windowRect.x > (float)UI.screenWidth;
			if (flag2)
			{
				this.windowRect.x = (float)UI.screenWidth - this.windowRect.width;
			}
		}

		public void UpdateHeight()
		{
			this.windowRect.height = 225f;
			if (showPawnList)
			{
				this.windowRect.height += (float)this.ReservesTracker.nextReserve.Members.Count * 16f;
			}
		}

		public override void DoWindowContents(Rect inRect)
		{
			bool drawBackground = true;// VESWWMod.settings.drawBackground;
			if (drawBackground)
			{
				Color color = new Color
				{
					r = Widgets.WindowBGFillColor.r,
					g = Widgets.WindowBGFillColor.g,
					b = Widgets.WindowBGFillColor.b,
					a = 0.25f
				};
				Widgets.DrawBoxSolid(inRect, color);
			}
			bool sent = this.ReservesTracker.nextReserve != null && this.ReservesTracker.nextReserve.deployed && !this.ReservesTracker.nextReserve.defeated;
			if (sent)
			{
				this.DoWaveProgressUI(inRect);
			}
			else
			{
				this.DoWavePredictionUI(inRect);
			}
		}
		/*
		private void DoWaveNumberAndModifierUI(Rect rect)
		{
			GameFont font = Text.Font;
			TextAnchor anchor = Text.Anchor;
			Text.Font = GameFont.Medium;
			Text.Anchor = TextAnchor.MiddleCenter;
			float num = rect.height - 10f;
			int i;
			Rect rect2;
			
			for (i = 1; i <= this.DeepStrikeTracker.nextDeepStrike.ModifierCount; i++)
			{
				rect2 = new Rect(rect)
				{
					x = rect.xMax - (float)i * num - (float)((i - 1) * 5),
					width = num,
					height = num
				};
				Rect rect3 = rect2;
				rect3.y += 5f;
				GUI.DrawTexture(rect3, Textures.ModifierBGTex);
				bool mysteryMod = VESWWMod.settings.mysteryMod;
				if (mysteryMod)
				{
					VDefOf.VSEWW_Mystery.DrawCard(rect3);
				}
				else
				{
					this.DeepStrikeTracker.nextDeepStrike.modifiers[i - 1].DrawCard(rect3);
				}
			}
			
			rect2 = new Rect(rect)
			{
				x = rect.xMax - (float)i * num - (float)((i - 1) * 5) - 10f,
				width = num + 10f
			};
			Rect rect4 = rect2;
			GUI.DrawTexture(rect4, Textures.WaveBGTex);
		//	Widgets.DrawTextureFitted(rect4, (this.DeepStrikeTracker.nextDeepStrike.WaveType == 0) ? Textures.NormalTex : Textures.BossTex, 0.8f);
			TooltipHandler.TipRegion(rect4, this.waveTip);
			rect2 = new Rect(rect)
			{
				width = 150f
			};
			Rect r = rect2;
			r.x = rect4.x - 10f - r.width;
			Text.Anchor = TextAnchor.MiddleRight;
		//	Widgets.Label(r.Rounded(), "VESWW.WaveNum".Translate(this.DeepStrikeTracker.nextDeepStrike.waveNum));
			Text.Font = font;
			Text.Anchor = anchor;
		}
		*/
		public void WaveTip()
		{
            if (this.ReservesTracker.nextReserve != null)
			{
				//	string text = (this.DeepStrikeTracker.nextDeepStrike.WaveType == 0) ? "VESWW.NormalWave".Translate() : "VESWW.BossWave".Translate();
				string text2 = "VESWW.PointUsed".Translate(this.ReservesTracker.nextReserve.Members.Select(x => x.kindDef.combatPower).Sum());
				string text3 = "";
				/*
				Dictionary<RewardCategory, int> commonality = RewardCategoryExtension.GetCommonality(this.DeepStrikeTracker.nextDeepStrike.waveNum);
				int num = commonality.Sum((KeyValuePair<RewardCategory, int> v) => v.Value);
				foreach (KeyValuePair<RewardCategory, int> keyValuePair in commonality)
				{
					text3 += string.Format("{0} - {1}\n", keyValuePair.Key, ((keyValuePair.Value > 0) ? ((float)keyValuePair.Value / (float)num) : 0f).ToStringPercent());
				}
				this.waveTip = string.Format("<b>{0}</b>\n\n{1}\n\n{2}\n{3}", new object[]
				{
					text,
					text2,
					"VESWW.RewardChance".Translate(),
					text3
				}).TrimEndNewlines();
				*/
			}
		}

		private void DoWavePredictionUI(Rect rect)
		{
            if (true)
            {

            }
			Rect rect2 = new Rect(rect)
			{
				height = 60f
			};
			Rect rect3 = rect2;
		//	this.DoWaveNumberAndModifierUI(rect3);
			GameFont font = Text.Font;
			TextAnchor anchor = Text.Anchor;
			Text.Font = GameFont.Medium;
			Text.Anchor = TextAnchor.UpperRight;
			rect2 = new Rect(rect)
			{
				y = rect3.yMax + 10f,
				height = 30f
			};
			Rect rect4 = rect2;
			Widgets.Label(rect4, this.ReservesTracker.nextReserve.TimeBeforeDeployment());
			Text.Font = GameFont.Tiny;
			float yMax = rect4.yMax;
			if (showPawnList)
			{
				rect2 = new Rect(rect)
				{
					x = rect3.xMax - 20f,
					y = rect4.yMax + 10f,
					height = 20f,
					width = 20f
				};
				Rect position = rect2;
				GUI.color = this.ReservesTracker.nextReserve.faction.Color;
				GUI.DrawTexture(position, this.ReservesTracker.nextReserve.faction.def.FactionIcon);
				GUI.color = Color.white;
				rect2 = new Rect(rect)
				{
					y = rect4.yMax + 10f,
					height = 20f,
					width = rect.width - position.width
				};
				Rect rect5 = rect2;
				Text.Anchor = TextAnchor.MiddleRight;
				Widgets.Label(rect5, this.ReservesTracker.nextReserve.faction.Name);
				Text.Anchor = TextAnchor.UpperRight;
				rect2 = new Rect(rect)
				{
					y = rect5.yMax + 5f,
					height = (float)this.ReservesTracker.nextReserve.Members.Count * 16f
				};
				Rect rect6 = rect2;
				Widgets.Label(rect6, this.ReservesTracker.nextReserve.Members.Select(x=> x.KindLabel).ToString());
				yMax = rect6.yMax;
			}
			rect2 = new Rect(rect)
			{
				y = yMax + 10f,
				x = rect.x + rect.width / 2f,
				width = rect.width / 2f,
				height = 20f
			};
			Rect rect7 = rect2;
			if (Widgets.ButtonText(rect7, "VESWW.SkipWave".Translate(), true, true, true))
			{
				this.ReservesTracker.nextReserve.delay = 1;
			}
		//	TooltipHandler.TipRegion(rect7, "VESWW.MoreRewardChance".Translate(this.DeepStrikeTracker.nextDeepStrike.FourthRewardChanceNow.ToStringPercent()));
			/*
			if (!VESWWMod.settings.hideToggleDraggable)
			{
				rect2 = new Rect(rect)
				{
					y = rect7.yMax,
					x = rect.x + rect.width / 2f,
					width = rect.width / 2f,
					height = 25f
				};
				Rect rect8 = rect2;
				Widgets.CheckboxLabeled(rect8, "VESWW.Locked".Translate(), ref this.draggable, false, null, null, false);
			}
			*/
			Text.Font = font;
			Text.Anchor = anchor;
		}
		
		private void DoWaveProgressUI(Rect rect)
		{
			Rect rect2 = new Rect(rect)
			{
				height = 60f
			};
			Rect rect3 = rect2;
		//	this.DoWaveNumberAndModifierUI(rect3);
			rect2 = new Rect(rect)
			{
				y = rect3.yMax + 10f,
				width = rect.width,
				height = 25f
			};
			Rect rect4 = rect2;
			bool flag = this.ReservesTracker.nextReserve?.Lords != null;
			if (flag)
			{
				int num = this.ReservesTracker.nextReserve.Members.Count - this.ReservesTracker.nextReserve.Members.Where(x=> x.Spawned && !x.Dead && !x.Downed).Count();
				this.DrawFillableBar(rect4, string.Format("{0}/{1}", num, this.ReservesTracker.nextReserve.Members.Count), (float)num / (float)this.ReservesTracker.nextReserve.Members.Count, true);
				rect2 = new Rect(rect)
				{
					x = rect3.xMax - 20f,
					y = rect4.yMax + 10f,
					height = 20f,
					width = 20f
				};
				Rect position = rect2;
				GUI.color = this.ReservesTracker.nextReserve.faction.Color;
				GUI.DrawTexture(position, this.ReservesTracker.nextReserve.faction.def.FactionIcon);
				GUI.color = Color.white;
				rect2 = new Rect(rect)
				{
					y = rect4.yMax + 10f,
					height = 20f,
					width = rect.width - position.width
				};
				Rect rect5 = rect2;
				Text.Anchor = TextAnchor.MiddleRight;
				Widgets.Label(rect5, this.ReservesTracker.nextReserve.Label);
				Text.Anchor = TextAnchor.UpperRight;
				Text.Anchor = TextAnchor.UpperRight;
				Text.Font = GameFont.Tiny;
				rect2 = new Rect(rect)
				{
					y = rect5.yMax + 10f,
					height = rect.height - rect3.height - rect4.height - 20f
				};
				Rect rect6 = rect2;
				Widgets.Label(rect6, this.ReservesTracker.Reserves.Where(x=> x.deployed && !x.defeated).SelectMany(x=> x.Members).Select(x=> x.KindLabel).ToCommaList());
			}
			Text.Anchor = TextAnchor.UpperLeft;
			Text.Font = GameFont.Small;
		}
		
		private void DrawFillableBar(Rect rect, string label, float percent, bool doBorder = true)
		{
			if (doBorder)
			{
				GUI.DrawTexture(rect, BaseContent.BlackTex);
				rect = rect.ContractedBy(3f);
			}
			GUI.color = Widgets.WindowBGFillColor;
			GUI.DrawTexture(rect, BaseContent.WhiteTex);
			Rect position = new Rect(rect);
			position.width *= percent;
			GUI.color = new Color(0.48f, 0.24f, 0.24f);
			GUI.DrawTexture(position, BaseContent.WhiteTex);
			GUI.color = Color.white;
			TextAnchor anchor = Text.Anchor;
			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(rect, label);
			Text.Anchor = anchor;
		}

		private readonly MapComponent_Reserves ReservesTracker;

		private string waveTip;
		private bool showPawnList;

		internal Vector2 pos;
	}
	/*
	[StaticConstructorOnStartup]
	public static class Textures
	{
		// Token: 0x0400003F RID: 63
		internal static readonly Texture2D WaveBGTex = ContentFinder<Texture2D>.Get("UI/Waves/WaveBG", true);

		// Token: 0x04000040 RID: 64
		internal static readonly Texture2D ModifierBGTex = ContentFinder<Texture2D>.Get("UI/Modifiers/ModifierBG", true);

		// Token: 0x04000041 RID: 65
		internal static readonly Texture2D NormalTex = ContentFinder<Texture2D>.Get("UI/Waves/Wave_Normal", true);

		// Token: 0x04000042 RID: 66
		internal static readonly Texture2D BossTex = ContentFinder<Texture2D>.Get("UI/Waves/Wave_Boss", true);
	}
	*/
}
