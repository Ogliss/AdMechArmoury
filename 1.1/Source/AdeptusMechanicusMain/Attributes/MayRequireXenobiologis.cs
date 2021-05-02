using System;
using Verse;

namespace RimWorld
{
    [AttributeUsage(AttributeTargets.All)]
    public class MayRequireXenobiologis : MayRequireAttribute
	{
		public MayRequireXenobiologis() : base("Ogliss.AdMech.Xenobiologis")
		{
		}
	}
}
