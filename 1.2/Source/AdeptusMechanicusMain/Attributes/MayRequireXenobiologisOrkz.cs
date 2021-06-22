using System;

namespace RimWorld
{
    [AttributeUsage(AttributeTargets.All)]
    public class MayRequireOrkz : MayRequireAttribute
	{
		public MayRequireOrkz() : base("Ogliss.AdMech.Xenobiologis.Orkz")
		{
		}
	}
}
