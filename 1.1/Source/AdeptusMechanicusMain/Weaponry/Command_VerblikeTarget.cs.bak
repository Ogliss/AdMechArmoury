using System;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
    // Token: 0x0200004B RID: 75
    public class Command_VerblikeTarget : Command
    {
        // Token: 0x1700002C RID: 44
        // (get) Token: 0x06000143 RID: 323 RVA: 0x0000D7C0 File Offset: 0x0000B9C0
        public override Color IconDrawColor
        {
            get
            {
                bool flag = this.verb.EquipmentSource != null;
                Color result;
                if (flag)
                {
                    result = this.verb.EquipmentSource.DrawColor;
                }
                else
                {
                    result = base.IconDrawColor;
                }
                return result;
            }
        }

        // Token: 0x06000144 RID: 324 RVA: 0x0000D800 File Offset: 0x0000BA00
        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            if (action!=null)
            {
                this.verb.EquipmentSource.GetComp<CompWargearWeaponSecondry>().Castverb = verb;
                this.action();
            }
            //Log.Message(string.Format("Command_Targetlike trying to cast {0} {1}", this.verb.EquipmentSource.GetComp<CompWargearWeaponSecondry>().Castverb, verb.verbProps.defaultProjectile.label));
            SoundStarter.PlayOneShotOnCamera(SoundDefOf.Tick_Tiny, null);
            Targeter targeter = Find.Targeter;
            bool flag = this.verb.CasterIsPawn && targeter.targetingVerb != null && targeter.targetingVerb.verbProps == this.verb.verbProps;
            if (flag)
            {
                Pawn casterPawn = this.verb.CasterPawn;
                bool flag2 = !targeter.IsPawnTargeting(casterPawn);
                if (flag2)
                {
                    targeter.targetingVerbAdditionalPawns.Add(casterPawn);
                }
            }
            else
            {

            //    Log.Message(string.Format("Command_Targetlike selected v fires {0} {1}", verb.verbProps.burstShotCount, verb.verbProps.defaultProjectile.label));
                Find.Targeter.BeginTargeting(this.verb);
            }
        }
        /*
        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth)
        {
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
                    tip.text = tip.text + "\n" + StringsToTranslate.AU_DISABLED + ": " + disabledReason;
                TooltipHandler.TipRegion(rect, tip);
            }
            if (pawnAbility.CooldownTicksLeft != -1 && pawnAbility.CooldownTicksLeft < pawnAbility.MaxCastingTicks)
            {
                var math = curTicks / (float)pawnAbility.MaxCastingTicks;
                Widgets.FillableBar(rect, math, AbilityButtons.FullTex, AbilityButtons.EmptyTex, false);
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
        */
        public void FillableBarBottom(Rect rect, float fillPercent, Texture2D fillTex, Texture2D bgTex, bool doBorder)
        {
            if (doBorder)
            {
                GUI.DrawTexture(rect, BaseContent.BlackTex);
                rect = rect.ContractedBy(3f);
            }
            if (fillTex != null)
                GUI.DrawTexture(rect, fillTex);
            rect.height *= fillPercent;
            GUI.DrawTexture(rect, bgTex);
        }
        //   public CompAbilityUser compAbilityUser;

        // Token: 0x04000026 RID: 38
        public int curTicks = -1;

        // Token: 0x04000027 RID: 39
     //   public PawnAbility pawnAbility;

        // Token: 0x04000028 RID: 40
        public Verb_ShootOG verb = null;
        // Token: 0x04000118 RID: 280
        public Action action;
    }
}
