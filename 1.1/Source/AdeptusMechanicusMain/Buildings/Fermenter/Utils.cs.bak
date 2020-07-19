using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
	// Token: 0x02000008 RID: 8
	public static class Utils
	{
		// Token: 0x06000035 RID: 53 RVA: 0x00002D9B File Offset: 0x00000F9B
		public static string IngredientFilterSummary(ThingFilter thingFilter)
		{
			return thingFilter.Summary;
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00002DA4 File Offset: 0x00000FA4
		public static string VowelTrim(string str, int limit)
		{
			int num = str.Length - limit;
			int num2 = str.Length - 1;
			while (num2 > 0 && num > 0)
			{
				if (Utils.IsVowel(str[num2]) && str[num2 - 1] != ' ')
				{
					str = str.Remove(num2, 1);
					num--;
				}
				num2--;
			}
			if (str.Length > limit)
			{
				str = str.Remove(limit - 2) + "..";
			}
			return str;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00002E1C File Offset: 0x0000101C
		public static bool IsVowel(char c)
		{
			HashSet<char> hashSet = new HashSet<char>
			{
				'a',
				'e',
				'i',
				'o',
				'u'
			};
			return hashSet.Contains(c);
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00002E64 File Offset: 0x00001064
		public static Texture2D GetIcon(ThingDef thingDef)
		{
			Texture2D texture2D = ContentFinder<Texture2D>.Get(thingDef.graphicData.texPath, false);
			if (texture2D == null)
			{
				texture2D = ContentFinder<Texture2D>.GetAllInFolder(thingDef.graphicData.texPath).ToList<Texture2D>()[0];
				if (texture2D == null)
				{
					texture2D = ContentFinder<Texture2D>.Get("UI/Commands/LaunchReport", true);
					Log.Warning("Fermenter:: No texture at " + thingDef.graphicData.texPath + ".", false);
				}
			}
			return texture2D;
		}
	}
}
