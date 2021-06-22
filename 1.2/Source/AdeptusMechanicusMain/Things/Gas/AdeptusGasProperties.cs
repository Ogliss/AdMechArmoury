using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.AdeptusGasProperties
    public class AdeptusGasProperties : GasProperties
	{
		public HediffDef hediffDef;
		public float hediffAddChance = 1f;
		public float hediffSeverity = 0.05f;
		public int ticksPerApplication = 150;
		public bool onlyAffectLungs = true;
		public bool ignoreAnimals;
		public bool ignoreNormalFlesh;
		public bool ignoreInsectFlesh;
		public bool ignoreMechanoidFlesh;
		public bool ignoreToxicSensitivity;
        public DamageDef damageDef;
        public float damageChance = 1f;
        public int damage = 3;
        public int damageInterval = 100;
        public int damageInitalDelay = 100;
        public bool damageBuildings = true;
        public bool damageThings = true;
        public bool damagePawns = true;
        public bool damagePawnsOnlyMovedRecently = false;
        public bool damagePawnsOnlyMoving = false;
        public bool damageMote = true;
        public bool damagePawnsOnly = false;
        public ThingDef damageMoteDef;
        public int tickUpdateSpeed = 250;
    }
}
