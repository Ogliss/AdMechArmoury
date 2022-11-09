using Verse;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.DeathActionWorker_NoCorpse
    public class DeathActionWorker_NoCorpse : DeathActionWorker
    {
        public override void PawnDied(Corpse corpse)
        {
            if (corpse != null)
            {
                corpse.Destroy();
            }
        }
    }
}
