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
    // AdeptusMechanicus.DeathActionProperties_WarpSpawn
    public class DeathActionProperties_WarpSpawn : DeathActionProperties
    {
        public DeathActionProperties_WarpSpawn()
        {
            this.workerClass = typeof(DeathActionWorker_WarpSpawn);
        }

        public FleckDef fleck;
        public ThingDef mote;
        public int moteCount = 3;
        public FloatRange moteOffsetRange = new FloatRange(0.2f, 0.4f);
        public ThingDef filth;
        public IntRange filthCountRange = IntRange.one;
        public HediffDef injuryCreatedOnDeath;
        public IntRange injuryCount;
        public SoundDef sound;
        public FleshbeastUtility.MeatExplosionSize? meatExplosionSize;
    }
    public class DeathActionWorker_WarpSpawn : DeathActionWorker_NoCorpse
    {
        public DeathActionProperties_WarpSpawn Props => (DeathActionProperties_WarpSpawn)this.props;
        public override void PawnDied(Corpse corpse, Lord prevLord)
        {
            if (!corpse.Spawned)
            {
                return;
            }
            if (Props.mote != null || Props.fleck != null)
            {
                Vector3 drawPos = corpse.DrawPos;
                for (int i = 0; i < Props.moteCount; i++)
                {
                    Vector2 vector = Rand.InsideUnitCircle * Props.moteOffsetRange.RandomInRange * (float)Rand.Sign;
                    Vector3 loc = new Vector3(drawPos.x + vector.x, drawPos.y, drawPos.z + vector.y);
                    if (Props.mote != null)
                    {
                        MoteMaker.MakeStaticMote(loc, corpse.Map, Props.mote, 1f * corpse.InnerPawn.DrawSize.sqrMagnitude / 2, false);
                    }
                    else
                    {
                        FleckMaker.Static(loc, corpse.Map, Props.fleck, 1f * corpse.InnerPawn.DrawSize.sqrMagnitude/2);
                    }
                }
            }
            if (Props.filth != null)
            {
                int filthCount = Props.filthCountRange.RandomInRange;
                FilthMaker.TryMakeFilth(corpse.Position, corpse.Map, Props.filth, filthCount, FilthSourceFlags.None, true);
            }
            if (Props.sound != null)
            {
                Props.sound.PlayOneShot(SoundInfo.InMap(corpse, MaintenanceType.None));
            }
            if (corpse != null)
            {
                FleckMaker.AttachedOverlay(corpse, FleckDefOf.PsycastSkipFlashEntry, Vector3.zero, corpse.DrawSize.sqrMagnitude/2, -1f);
            }
            if (corpse.InnerPawn != null && corpse.InnerPawn.kindDef.destroyGearOnDrop) base.PawnDied(corpse, prevLord);
        }

    }
}