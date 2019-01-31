namespace AdeptusMechanicus
{
    public class ProjectileDef_AbilityExplosive : AbilityUser.ProjectileDef_Ability
    {
        public bool CanStartFire = false;
        public int postFiringDuration = 0;
        public float postFiringFinalIntensity = 0f;
        public float postFiringInitialIntensity = 0f;
        public int preFiringDuration = 0;
        public float preFiringFinalIntensity = 0f;
        public float preFiringInitialIntensity = 0f;
        public float StartFireChance;
        public string warmupGraphicPathSingle = null;
    }
}