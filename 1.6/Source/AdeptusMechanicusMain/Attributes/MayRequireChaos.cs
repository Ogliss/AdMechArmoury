using System;

namespace RimWorld
{
    [AttributeUsage(AttributeTargets.Field)]
    public class MayRequireChaos : MayRequireAttribute
	{
		public MayRequireChaos() : base("Ogliss.AdMech.Chaos")
		{
		}
	}
}
