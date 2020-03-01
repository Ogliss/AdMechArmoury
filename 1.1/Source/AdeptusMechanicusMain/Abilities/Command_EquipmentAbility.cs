using AdeptusMechanicus;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld
{
    // Token: 0x02000A98 RID: 2712
    public class Command_EquipmentAbility : Command_Ability
    {
        // Token: 0x06003F8D RID: 16269 RVA: 0x001516D9 File Offset: 0x0014F8D9
        public Command_EquipmentAbility(EquipmentAbility ability) : base(ability)
        {
        }

        public int curTicks = -1;
        public new EquipmentAbility ability => (EquipmentAbility)base.ability;
        // Token: 0x06003F8E RID: 16270 RVA: 0x001516E4 File Offset: 0x0014F8E4
        /*
        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth)
        {
            EquipmentAbilityDef def = (EquipmentAbilityDef)this.ability.def;
            Pawn pawn = this.ability.pawn;
            this.disabled = false;
            if (def.EntropyGain > 1.401298E-45f)
            {
                Hediff hediff = pawn.health.hediffSet.hediffs.FirstOrDefault((Hediff h) => h.def == HediffDefOf.PsychicAmplifier);
                if (hediff == null || hediff.Severity < (float)def.level)
                {
                    base.DisableWithReason("CommandPsycastHigherLevelAmplifierRequired".Translate(def.level));
                }
                else if (pawn.psychicEntropy.WouldOverflowEntropy(def.EntropyGain + PsycastUtility.TotalEntropyFromQueuedPsycasts(pawn)))
                {
                    base.DisableWithReason("CommandPsycastWouldExceedEntropy".Translate(def.label));
                }
            }
            GizmoResult result = base.GizmoOnGUI(topLeft, maxWidth);
            if (def.EntropyGain > 1.401298E-45f)
            {
                Text.Font = GameFont.Tiny;
                string text = def.EntropyGain.ToString();
                float x = Text.CalcSize(text).x;
                Widgets.Label(new Rect(topLeft.x + this.GetWidth(maxWidth) - x - 5f, topLeft.y + 5f, x, 18f), text);
            }
            return result;
        }
        */

        public override Texture2D BGTexture
        {
            get
            {
                return Command.BGTex;
            }
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth)
        {
        //    this.defaultDesc = this.ability.def.GetTooltip(this.ability.pawn);
            var rect = new Rect(topLeft.x, topLeft.y, this.GetWidth(maxWidth), 75f);
            var isMouseOver = false;
            if (Mouse.IsOver(rect))
            {
                isMouseOver = true;
                GUI.color = GenUI.MouseoverColor;
            }
            var badTex = icon;
            if (badTex == null) badTex = BaseContent.BadTex;

            GUI.DrawTexture(rect, BGTex);
            MouseoverSounds.DoRegion(rect, SoundDefOf.Mouseover_Command);
            GUI.color = IconDrawColor;
            Widgets.DrawTextureFitted(new Rect(rect), badTex, iconDrawScale * 0.85f, iconProportions, iconTexCoords);
            GUI.color = Color.white;
            var isUsed = false;
            //Rect rectFil = new Rect(topLeft.x, topLeft.y, this.Width, this.Width);

            var keyCode = hotKey != null ? hotKey.MainKey : KeyCode.None;
            if (keyCode != KeyCode.None && !GizmoGridDrawer.drawnHotKeys.Contains(keyCode))
            {
                var rect2 = new Rect(rect.x + 5f, rect.y + 5f, rect.width - 10f, 18f);
                Widgets.Label(rect2, keyCode.ToStringReadable());
                GizmoGridDrawer.drawnHotKeys.Add(keyCode);
                if (hotKey.KeyDownEvent)
                {
                    isUsed = true;
                    Event.current.Use();
                }
            }
            if (Widgets.ButtonInvisible(rect, false)) isUsed = true;
            var labelCap = LabelCap;
            if (!labelCap.NullOrEmpty())
            {
                var num = Text.CalcHeight(labelCap, rect.width);
                num -= 2f;
                var rect3 = new Rect(rect.x, rect.yMax - num + 12f, rect.width, num);
                GUI.DrawTexture(rect3, TexUI.GrayTextBG);
                GUI.color = Color.white;
                Text.Anchor = TextAnchor.UpperCenter;
                Widgets.Label(rect3, labelCap);
                Text.Anchor = TextAnchor.UpperLeft;
                GUI.color = Color.white;
            }
            GUI.color = Color.white;
            if (DoTooltip)
            {
                TipSignal tip = Desc;
                if (disabled && !disabledReason.NullOrEmpty())
                    tip.text = tip.text+ StringsToTranslate.AU_DISABLED + ": " + disabledReason + "\n" + ability.CooldownTicksLeft.ToStringSecondsFromTicks();
                TooltipHandler.TipRegion(rect, tip);
            }
            if (ability.CooldownTicksLeft != -1 && ability.CooldownTicksLeft < ability.MaxCastingTicks)
            {
                var math = ability.CooldownTicksLeft / (float)ability.MaxCastingTicks;
                FillableBar(rect, math, AbilityButtons.FullTex, AbilityButtons.EmptyTex, false);
            }
            if (!HighlightTag.NullOrEmpty() && (Find.WindowStack.FloatMenu == null ||
                                                !Find.WindowStack.FloatMenu.windowRect.Overlaps(rect)))
                UIHighlighter.HighlightOpportunity(rect, HighlightTag);
            if (isUsed)
            {
                if (disabled)
                {
                    if (!disabledReason.NullOrEmpty())
                        Messages.Message(disabledReason, MessageTypeDefOf.RejectInput);
                    return new GizmoResult(GizmoState.Mouseover, null);
                }
                if (!TutorSystem.AllowAction(TutorTagSelect))
                    return new GizmoResult(GizmoState.Mouseover, null);
                var result = new GizmoResult(GizmoState.Interacted, Event.current);
                TutorSystem.Notify_Event(TutorTagSelect);
                return result;
            }
            if (isMouseOver) return new GizmoResult(GizmoState.Mouseover, null);
            return new GizmoResult(GizmoState.Clear, null);
        }

        // Token: 0x06001C74 RID: 7284 RVA: 0x000AE41C File Offset: 0x000AC61C
        public static Rect FillableBar(Rect rect, float fillPercent, Texture2D fillTex, Texture2D bgTex, bool doBorder)
        {
            if (doBorder)
            {
                GUI.DrawTexture(rect, BaseContent.BlackTex);
                rect = rect.ContractedBy(3f);
            }
            if (bgTex != null)
            {
                GUI.DrawTexture(rect, bgTex);
            }
            Rect result = rect;
            rect.height *= fillPercent;
            GUI.DrawTexture(rect, fillTex);
            return result;
        }
        //    public new static readonly Texture2D BGTex = ContentFinder<Texture2D>.Get("UI/Widgets/DesButBG", true);
    }
}
