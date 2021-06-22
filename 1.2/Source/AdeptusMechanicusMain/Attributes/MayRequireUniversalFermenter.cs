using System;

namespace RimWorld
{
    [AttributeUsage(AttributeTargets.All)]
    public class MayRequireUniversalFermenter : MayRequireAttribute
	{
		public MayRequireUniversalFermenter() : base("syrchalis.universalfermenter")
		{
		}
	}
}
