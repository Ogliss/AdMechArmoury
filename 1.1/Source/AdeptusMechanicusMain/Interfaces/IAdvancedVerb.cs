using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public interface IAdvancedVerb
    {
        public bool RapidFire { get; }
        public float RapidFireRange { get; }
        public Reliability Reliability { get; }
        public int ScattershotCount { get; }
        public bool Jams { get; }
        public bool JamsDamageWeapon { get; }
        public float JamDamage { get; }
        public bool GetsHot { get; }
        public bool HotDamageWeapon { get; }
        public float HotDamage { get; }
        public bool GetsHotCrit { get; }
        public float GetsHotCritChance { get; }
        public bool GetsHotCritExplosion { get; }
        public float GetsHotCritExplosionChance { get; }
        public bool Rending { get; }
        public float RendingChance { get; }
        public bool EffectsUser { get; }
        public float EffectsUserChance { get; }
        public HediffDef UserEffect { get; }
        public StatDef ResistEffectStat { get; }
        public List<string> UserEffectImmuneList { get; }
        public bool TwinLinked { get; }
        public bool Multishot { get; }
        public bool HeavyWeapon { get; }
        public float HeavyWeaponSetupTime { get; }
        public ResearchProjectDef RequiredResearch { get; }
        public int ticksHere { get; }
        public int LastMovedTick { get; set; }

        public float AdjustedCooldown(Verb ownerVerb, Pawn attacker);
        public void DrawExtraRadiusRings(IntVec3 center);
        public void DrawRapidFireRadiusRing(IntVec3 center, Func<IntVec3, bool> predicate = null);
        public float GetHitChanceFactor(Thing equipment, float dist);
        public float AdjustedAccuracy(RangeCategory cat, Thing equipment);

        float BarrelLength { get; }
        float BulletOffset { get; }
        float BarrelOffset { get; }
        public float MuzzleSmokeSize { get; }
        public float MuzzleFlareSize { get; }
        ThingDef MuzzleFlareDef { get; }
        public bool MuzzleFlareRotates { get; }
        public Color MuzzleFlareColor { get; }
        public Color MuzzleFlareColorTwo { get; }
        ThingDef MuzzleSmokeDef { get; }
    }

}
