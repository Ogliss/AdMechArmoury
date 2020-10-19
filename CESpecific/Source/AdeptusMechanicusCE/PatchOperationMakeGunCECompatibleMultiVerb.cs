using System;
using System.Xml;
using Verse;

namespace CombatExtended
{
	// Token: 0x0200007C RID: 124 CombatExtended.PatchOperationMakeGunCECompatible
	public class PatchOperationMakeGunCECompatibleMultiVerb : PatchOperation
	{
		// Token: 0x06000286 RID: 646 RVA: 0x00018E44 File Offset: 0x00017044
		protected override bool ApplyWorker(XmlDocument xml)
		{
			bool flag = false;
			bool flag2 = this.defName.NullOrEmpty();
			bool result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				foreach (object obj in xml.SelectNodes("Defs/ThingDef[defName=\"" + this.defName + "\"]"))
				{
					flag = true;
					XmlNode xmlNode = obj as XmlNode;
					XmlContainer xmlContainer = this.statBases;
					bool flag3 = xmlContainer != null && xmlContainer.node.HasChildNodes;
					if (flag3)
					{
						this.AddOrReplaceStatBases(xml, xmlNode);
					}
					XmlContainer xmlContainer2 = this.costList;
					bool flag4 = xmlContainer2 != null && xmlContainer2.node.HasChildNodes;
					if (flag4)
					{
						this.AddOrReplaceCostList(xml, xmlNode);
					}
					bool flag5 = this.Properties != null && this.Properties.node.HasChildNodes;
					if (flag5)
					{
						this.AddOrReplaceVerbPropertiesCE(xml, xmlNode);
					}
					bool flag6 = this.AmmoUser != null || this.FireModes != null;
					if (flag6)
					{
						this.AddOrReplaceCompsCE(xml, xmlNode);
					}
					bool flag7 = this.weaponTags != null && this.weaponTags.node.HasChildNodes;
					if (flag7)
					{
						this.AddOrReplaceWeaponTags(xml, xmlNode);
					}
					bool flag8 = this.researchPrerequisite != null;
					if (flag8)
					{
						this.AddOrReplaceResearchPrereq(xml, xmlNode);
					}
					bool flag9 = ModLister.HasActiveModWithName("RunAndGun") && !this.AllowWithRunAndGun;
					if (flag9)
					{
						this.AddRunAndGunExtension(xml, xmlNode);
					}
				}
				result = flag;
			}
			return result;
		}

		// Token: 0x06000287 RID: 647 RVA: 0x00019004 File Offset: 0x00017204
		private bool GetOrCreateNode(XmlDocument xml, XmlNode xmlNode, string name, out XmlElement output)
		{
			XmlNodeList xmlNodeList = xmlNode.SelectNodes(name);
			bool flag = xmlNodeList.Count == 0;
			bool result;
			if (flag)
			{
				output = xml.CreateElement(name);
				xmlNode.AppendChild(output);
				result = false;
			}
			else
			{
				output = (xmlNodeList[0] as XmlElement);
				result = true;
			}
			return result;
		}

		// Token: 0x06000288 RID: 648 RVA: 0x00019058 File Offset: 0x00017258
		private XmlElement CreateListElementAndPopulate(XmlDocument xml, XmlNode reference, string type = null)
		{
			XmlElement xmlElement = xml.CreateElement("li");
			bool flag = type != null;
			if (flag)
			{
				xmlElement.SetAttribute("Class", type);
			}
			this.Populate(xml, reference, ref xmlElement, false);
			return xmlElement;
		}

		// Token: 0x06000289 RID: 649 RVA: 0x0001909C File Offset: 0x0001729C
		private void Populate(XmlDocument xml, XmlNode reference, ref XmlElement destination, bool overrideExisting = false)
		{
			foreach (object obj in reference)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (overrideExisting)
				{
					XmlNodeList xmlNodeList = destination.SelectNodes(xmlNode.Name);
					bool flag = xmlNodeList != null;
					if (flag)
					{
						foreach (object obj2 in xmlNodeList)
						{
							XmlNode oldChild = (XmlNode)obj2;
							destination.RemoveChild(oldChild);
						}
					}
				}
				destination.AppendChild(xml.ImportNode(xmlNode, true));
			}
		}

		// Token: 0x0600028A RID: 650 RVA: 0x0001917C File Offset: 0x0001737C
		private void AddOrReplaceVerbPropertiesCE(XmlDocument xml, XmlNode xmlNode)
		{
			XmlElement xmlElement;
			bool orCreateNode = this.GetOrCreateNode(xml, xmlNode, "verbs", out xmlElement);
			if (orCreateNode)
			{
				XmlNodeList xmlNodeList = xmlElement.SelectNodes("li[verbClass=\"Verb_Shoot\" or verbClass=\"Verb_ShootOneUse\" or verbClass=\"Verb_LaunchProjectile\"]");
				foreach (object obj in xmlNodeList)
				{
					XmlNode xmlNode2 = obj as XmlNode;
					bool flag = xmlNode2 != null;
					if (flag)
					{
						xmlElement.RemoveChild(xmlNode2);
					}
				}
			}
            foreach (XmlNode item in this.Properties.node.ChildNodes)
			{
				xmlElement.AppendChild(this.CreateListElementAndPopulate(xml, item, "CombatExtended.VerbPropertiesCE"));
			}
		}

		// Token: 0x0600028B RID: 651 RVA: 0x0001922C File Offset: 0x0001742C
		private void AddOrReplaceCompsCE(XmlDocument xml, XmlNode xmlNode)
		{
			XmlElement xmlElement;
			this.GetOrCreateNode(xml, xmlNode, "comps", out xmlElement);
			bool flag = this.AmmoUser != null;
			if (flag)
			{
				xmlElement.AppendChild(this.CreateListElementAndPopulate(xml, this.AmmoUser.node, "CombatExtended.CompProperties_AmmoUser"));
			}
			bool flag2 = this.FireModes != null;
			if (flag2)
			{
				xmlElement.AppendChild(this.CreateListElementAndPopulate(xml, this.FireModes.node, "CombatExtended.CompProperties_FireModes"));
			}
		}

		// Token: 0x0600028C RID: 652 RVA: 0x000192A4 File Offset: 0x000174A4
		private void AddOrReplaceWeaponTags(XmlDocument xml, XmlNode xmlNode)
		{
			XmlElement xmlElement;
			this.GetOrCreateNode(xml, xmlNode, "weaponTags", out xmlElement);
			this.Populate(xml, this.weaponTags.node, ref xmlElement, false);
		}

		// Token: 0x0600028D RID: 653 RVA: 0x000192D8 File Offset: 0x000174D8
		private void AddOrReplaceStatBases(XmlDocument xml, XmlNode xmlNode)
		{
			XmlElement xmlElement;
			this.GetOrCreateNode(xml, xmlNode, "statBases", out xmlElement);
			bool hasChildNodes = xmlElement.HasChildNodes;
			if (hasChildNodes)
			{
				XmlNodeList xmlNodeList = xmlElement.SelectNodes("AccuracyTouch | AccuracyShort | AccuracyMedium | AccuracyLong");
				foreach (object obj in xmlNodeList)
				{
					XmlNode oldChild = (XmlNode)obj;
					xmlElement.RemoveChild(oldChild);
				}
			}
			this.Populate(xml, this.statBases.node, ref xmlElement, true);
		}

		// Token: 0x0600028E RID: 654 RVA: 0x00019378 File Offset: 0x00017578
		private void AddOrReplaceCostList(XmlDocument xml, XmlNode xmlNode)
		{
			XmlElement xmlElement;
			this.GetOrCreateNode(xml, xmlNode, "costList", out xmlElement);
			bool hasChildNodes = xmlElement.HasChildNodes;
			if (hasChildNodes)
			{
				xmlElement.RemoveAll();
			}
			this.Populate(xml, this.costList.node, ref xmlElement, false);
		}

		// Token: 0x0600028F RID: 655 RVA: 0x000193C0 File Offset: 0x000175C0
		private void AddOrReplaceResearchPrereq(XmlDocument xml, XmlNode xmlNode)
		{
			XmlElement xmlElement;
			this.GetOrCreateNode(xml, xmlNode, "recipeMaker", out xmlElement);
			XmlNode xmlNode2 = xmlElement.SelectSingleNode(this.researchPrerequisite.node.Name);
			bool flag = xmlNode2 != null;
			if (flag)
			{
				xmlElement.ReplaceChild(xml.ImportNode(this.researchPrerequisite.node, true), xmlNode2);
			}
			else
			{
				xmlElement.AppendChild(xml.ImportNode(this.researchPrerequisite.node, true));
			}
		}

		// Token: 0x06000290 RID: 656 RVA: 0x00019438 File Offset: 0x00017638
		private void AddRunAndGunExtension(XmlDocument xml, XmlNode xmlNode)
		{
			XmlElement xmlElement;
			this.GetOrCreateNode(xml, xmlNode, "modExtensions", out xmlElement);
			XmlElement xmlElement2 = xml.CreateElement("li");
			xmlElement2.SetAttribute("Class", "RunAndGun.DefModExtension_SettingDefaults");
			xmlElement.AppendChild(xmlElement2);
			XmlElement xmlElement3 = xml.CreateElement("weaponForbidden");
			xmlElement3.InnerText = "true";
			xmlElement2.AppendChild(xmlElement3);
		}

		// Token: 0x040001B1 RID: 433
		public string defName;

		// Token: 0x040001B2 RID: 434
		public bool AllowWithRunAndGun = true;

		// Token: 0x040001B3 RID: 435
		public XmlContainer statBases;

		// Token: 0x040001B4 RID: 436
		public XmlContainer Properties;

		// Token: 0x040001B5 RID: 437
		public XmlContainer AmmoUser;

		// Token: 0x040001B6 RID: 438
		public XmlContainer FireModes;

		// Token: 0x040001B7 RID: 439
		public XmlContainer weaponTags;

		// Token: 0x040001B8 RID: 440
		public XmlContainer costList;

		// Token: 0x040001B9 RID: 441
		public XmlContainer researchPrerequisite;
	}
}
