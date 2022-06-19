using System;
using Verse;

namespace AdeptusMechanicus.Ordnance
{
	public class WeaponDef : Def
	{
		public ThingDef ammoDef = null;
		public SoundDef soundCastDef = null;
		public int ammoQuantity = 0;
		public int ticksBetweenShots = 0;
		public float startShootingDistance = 0f;
		public float ammoTravelDistance = 0f;
		public float ammoDispersion = 0f;
		public float targetAcquireRange = 0f;
		public bool isTwinGun = true;
		public float horizontalPositionOffset = 0f;
		public float verticalPositionOffset = 0f;
		public int disableRainDurationInTicks = 0;
	}
}
