using System;

namespace RimWorld
{
    [AttributeUsage(AttributeTargets.All)]
    public class MayRequireAstartes : MayRequireAttribute
	{
		public MayRequireAstartes() : base("Ogliss.AdMech.Xenobiologis.Astartes")
		{
		}
	}
}
