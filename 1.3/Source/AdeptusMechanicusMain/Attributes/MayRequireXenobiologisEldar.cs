using System;

namespace RimWorld
{
    [AttributeUsage(AttributeTargets.All)]
    public class MayRequireEldar : MayRequireAttribute
	{
		public MayRequireEldar() : base("Ogliss.AdMech.Xenobiologis.Eldar")
		{
		}
	}
}
