using RimWorld;
using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
    // Token: 0x02000991 RID: 2449
    public class GameCondition_PlanetConquest : GameCondition
    {
        // Token: 0x17000A76 RID: 2678
        // (get) Token: 0x060039AD RID: 14765 RVA: 0x0012EE14 File Offset: 0x0012D014
        public override string TooltipString
        {
            get
            {
                Vector2 location;
                if (Find.CurrentMap != null)
                {
                    location = Find.WorldGrid.LongLatOf(Find.CurrentMap.Tile);
                }
                else
                {
                    location = default(Vector2);
                }
                return this.def.LabelCap + "\n" + "\n" + this.Description + ("\n" + "ImpactDate".Translate().CapitalizeFirst() + ": " + GenDate.DateFullStringAt((long)GenDate.TickGameToAbs(this.startTick + base.Duration), location)) + ("\n" + "TimeLeft".Translate().CapitalizeFirst() + ": " + base.TicksLeft.ToStringTicksToPeriod(true, false, true, true));
            }
        }

        // Token: 0x060039AE RID: 14766 RVA: 0x0012EF04 File Offset: 0x0012D104
        public override void GameConditionTick()
        {
            base.GameConditionTick();
            if (base.TicksLeft <= 179)
            {
                Find.ActiveLesson.Deactivate();
                if (base.TicksLeft == 179)
                {
                    SoundDefOf.PlanetkillerImpact.PlayOneShotOnCamera(null);
                }
                if (base.TicksLeft == 90)
                {
                    ScreenFader.StartFade(GameCondition_PlanetConquest.FadeColor, 1f);
                }
            }
        }

        // Token: 0x060039AF RID: 14767 RVA: 0x0012EF5F File Offset: 0x0012D15F
        public override void End()
        {
            base.End();
            this.Impact();
        }

        // Token: 0x060039B0 RID: 14768 RVA: 0x0012EF6D File Offset: 0x0012D16D
        private void Impact()
        {
            ScreenFader.SetColor(Color.clear);
            GenGameEnd.EndGameDialogMessage("GameOverPlanetkillerImpact".Translate(Find.World.info.name), false, GameCondition_PlanetConquest.FadeColor);
        }

        // Token: 0x04002211 RID: 8721
        private const int SoundDuration = 179;

        // Token: 0x04002212 RID: 8722
        private const int FadeDuration = 90;

        // Token: 0x04002213 RID: 8723
        private static readonly Color FadeColor = Color.white;
    }
}
