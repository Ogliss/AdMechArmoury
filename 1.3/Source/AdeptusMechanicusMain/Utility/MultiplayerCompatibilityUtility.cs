using Verse;
using Multiplayer.API;
namespace AdeptusMechanicus
{
    [StaticConstructorOnStartup]
    public static class MultiplayerCompatibilityUtility
    {
        static MultiplayerCompatibilityUtility()
        {
            // Multiplayer compatibility
            if (MP.enabled)
            {
                MP.RegisterAll();
            //    MP.RegisterSyncMethod(typeof(RimWorld.CompAssignableToPawn), nameof(RimWorld.CompAssignableToPawn.TryAssignPawn)).CancelIfAnyArgNull();
            }

        }
    }
}
