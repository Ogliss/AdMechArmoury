using Verse;
using Verse.AI.Group;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.DeathActionWorker_NoCorpse
    public class DeathActionWorker_NoCorpse : DeathActionWorker
    {
        public override void PawnDied(Corpse corpse, Lord prevLord)
        {
            if (corpse != null)
            {
                corpse.Destroy();
            }
        }
    }
}
