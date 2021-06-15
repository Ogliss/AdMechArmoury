using System;
using Verse;

namespace AdeptusMechanicus
{
	// Token: 0x020005D2 RID: 1490
	public interface IDropShip
	{
		Thing ShipThing { get; set; }
		CompDropship ShipComp { get; set; }
		bool KnownSourcePawn { get; set; }
		float Integrity { get; set; }
		float Stability { get; set; }
		float Mutation { get; set; }

	}

	public enum DropShipState
    {

    }
}
