using System;

namespace RimWorld
{
    [AttributeUsage(AttributeTargets.All)]
    public class MayRequireNecrons : MayRequireAttribute
	{
		public MayRequireNecrons() : base("Ogliss.AdMech.Xenobiologis.Necrons")
		{
		}
	}
}
