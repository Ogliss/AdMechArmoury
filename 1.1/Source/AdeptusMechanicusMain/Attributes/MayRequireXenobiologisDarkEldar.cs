using System;

namespace RimWorld
{
    [AttributeUsage(AttributeTargets.All)]
    public class MayRequireDarkEldar : MayRequireAttribute
	{
		public MayRequireDarkEldar() : base("Ogliss.AdMech.Xenobiologis.DarkEldar")
		{
		}
	}
}
