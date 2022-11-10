using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;
using System;
using Verse.AI;
using System.Text;
using System.Linq;
using Verse.AI.Group;
using RimWorld.Planet;
using UnityEngine;
using Verse.Sound;

namespace AdeptusMechanicus
{
    public class DeathActionWorker_WarpSpawn : DeathActionWorker_NoCorpse
    {
        public override void PawnDied(Corpse corpse)
        {
            if (!corpse.Spawned)
            {
                return;
            }
            if (this.mote != null || this.fleck != null)
            {
                Vector3 drawPos = corpse.DrawPos;
                for (int i = 0; i < this.moteCount; i++)
                {
                    Vector2 vector = Rand.InsideUnitCircle * this.moteOffsetRange.RandomInRange * (float)Rand.Sign;
                    Vector3 loc = new Vector3(drawPos.x + vector.x, drawPos.y, drawPos.z + vector.y);
                    if (this.mote != null)
                    {
                        MoteMaker.MakeStaticMote(loc, corpse.Map, this.mote, 1f, false);
                    }
                    else
                    {
                        FleckMaker.Static(loc, corpse.Map, this.fleck, 1f);
                    }
                }
            }
            if (this.filth != null)
            {
                FilthMaker.TryMakeFilth(corpse.Position, corpse.Map, this.filth, this.filthCount, FilthSourceFlags.None, true);
            }
            if (this.sound != null)
            {
                this.sound.PlayOneShot(SoundInfo.InMap(corpse, MaintenanceType.None));
            }
            if (corpse != null)
            {
                FleckMaker.AttachedOverlay(corpse, FleckDefOf.PsycastSkipFlashEntry, Vector3.zero, corpse.DrawSize.magnitude, -1f);
            }
            if (corpse.InnerPawn != null && corpse.InnerPawn.kindDef.destroyGearOnDrop) base.PawnDied(corpse);
        }

        public FleckDef fleck = DefDatabase<FleckDef>.GetNamed("OG_Fleck_WarpCloud");
        public ThingDef mote;
        public int moteCount = 3;
        public FloatRange moteOffsetRange = new FloatRange(0.2f, 0.4f);
        public ThingDef filth = DefDatabase<ThingDef>.GetNamed("OG_Filth_WarpSlime");
        public int filthCount = 4;
        public HediffDef injuryCreatedOnDeath;
        public IntRange injuryCount;
        public SoundDef sound = DefDatabase<SoundDef>.GetNamed("DeathAcidifier");
    }
}