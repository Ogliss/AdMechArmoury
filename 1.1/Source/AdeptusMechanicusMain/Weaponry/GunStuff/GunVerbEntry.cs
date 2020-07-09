using RimWorld;
using System.Collections.Generic;
using Verse;

namespace AdeptusMechanicus
{
    public class GunVerbEntry: IExposable
    {
        public bool RapidFire = false;

        public bool TyranidBurstBodySize = false;

        public Reliability reliability = Reliability.NA;
        public bool GetsHot = false;
        public bool HotDamageWeapon = false;
        public float HotDamage = 0f;
        public bool GetsHotCrit = false;
        public float GetsHotCritChance = 0f;
        public bool GetsHotCritExplosion = false;
        public float GetsHotCritExplosionChance = 0f;

        public bool Jams = false;
        public bool JamsDamageWeapon = false;
        public float JamDamage = 0f;

        public bool TwinLinked = false;
        public bool Multishot = false;

        public bool EffectsUser = false;
        public float EffectsUserChance = 0f;
        public StatDef ResistEffectStat = null;
        public HediffDef UserEffect = null;

        public bool PowerWeapon = false;
        public bool ForceWeapon = false;
        
        public bool Rending = false;
        public float RendingChance = 0.167f;
        
        public DamageDef ForceWeaponEffect = null;
        public HediffDef ForceWeaponHediff = null;
        public float ForceWeaponKillChance = 0f;
        public SoundDef ForceWeaponTriggerSound = null;

        public int ScattershotCount = 0;
        public ResearchProjectDef requiredResearch = null;
        public float originalWarmup = -1;
        public VerbProperties VerbProps;
        public List<string> UserEffectImmuneList = new List<string>();

        public void ExposeData()
        {
            Scribe_Values.Look<float>(ref this.originalWarmup, "originalWarmup", -1f);
        }
    }
}
