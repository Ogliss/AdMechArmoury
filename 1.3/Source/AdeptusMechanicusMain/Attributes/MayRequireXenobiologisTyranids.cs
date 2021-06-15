using System;

namespace RimWorld
{
    [AttributeUsage(AttributeTargets.All)]
    public class MayRequireTyranids : MayRequireAttribute
	{
		public MayRequireTyranids() : base("Ogliss.AdMech.Xenobiologis.Tyranids")
		{
		}
	}
}
