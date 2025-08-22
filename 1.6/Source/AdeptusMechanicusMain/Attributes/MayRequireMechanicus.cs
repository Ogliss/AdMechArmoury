using System;

namespace RimWorld
{
    [AttributeUsage(AttributeTargets.All)]
    public class MayRequireMechanicus : MayRequireAttribute
	{
		public MayRequireMechanicus() : base("Ogliss.AdMech.Xenobiologis.Mechanicus")
		{
		}
	}
}
