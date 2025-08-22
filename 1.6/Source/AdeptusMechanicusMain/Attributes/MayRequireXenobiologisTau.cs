using System;

namespace RimWorld
{
    [AttributeUsage(AttributeTargets.All)]
    public class MayRequireTau : MayRequireAttribute
	{
		public MayRequireTau() : base("Ogliss.AdMech.Xenobiologis.Tau")
		{
		}
	}
}
