using AdeptusMechanicus.settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Verse;

namespace CombatExtended
{
	public class PatchOperationMakeGunCECompatibleAdv : PatchOperation
	{
		public override bool ApplyWorker(XmlDocument xml)
		{
			bool result = false;
			if (this.defName.NullOrEmpty() && this.Name.NullOrEmpty())
			{
				result = false;
			}
			else
			{
				string search = this.defName.NullOrEmpty() ? "@Name" : "defName";
				string s = this.defName.NullOrEmpty() ? this.Name : this.defName;
				XmlNodeList nodeList = xml.SelectNodes("Defs/ThingDef[" + search + "=\"" + s + "\"]");
                if (nodeList.Count == 0)
                {
                    if (AMAMod.Dev && warnOnDefNotFound) 
                    {
						Log.Warning(s + " Not Found");
					}
					result = false;
				}
                else
				{
					IEnumerator enumerator = nodeList.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							result = true;
							XmlNode xmlNode = obj as XmlNode;
							bool? flag = (this.statBases != null) ? new bool?(this.statBases.node.HasChildNodes) : null;
							if (flag != null && flag.Value)
							{
								this.AddOrReplaceStatBases(xml, xmlNode);
							}
							bool? flag2 = (this.costList != null) ? new bool?(this.costList.node.HasChildNodes) : null;
							if (flag2 != null && flag2.Value)
							{
								this.AddOrReplaceCostList(xml, xmlNode);
							}
							if (this.Properties != null && this.Properties.node.HasChildNodes)
							{
								this.AddOrReplaceVerbPropertiesCE(xml, xmlNode);
							}
							if (this.AmmoUser != null || this.FireModes != null)
							{
								this.AddOrReplaceCompsCE(xml, xmlNode);
							}
							if (this.weaponTags != null && this.weaponTags.node.HasChildNodes)
							{
								this.AddOrReplaceWeaponTags(xml, xmlNode);
							}
							if (this.researchPrerequisite != null)
							{
								this.AddOrReplaceResearchPrereq(xml, xmlNode);
							}
							if (ModLister.HasActiveModWithName("RunAndGun") && !this.AllowWithRunAndGun)
							{
								this.AddRunAndGunExtension(xml, xmlNode);
							}
							this.ReplaceCompsOversized(xml, xmlNode);
						}
					}
					finally
					{
						IDisposable disposable;
						if ((disposable = (enumerator as IDisposable)) != null)
						{
							disposable.Dispose();
						}
					}
				}
			}
			return result;
		}

		private bool GetOrCreateNode(XmlDocument xml, XmlNode xmlNode, string name, out XmlElement output)
		{
			XmlNodeList xmlNodeList = xmlNode.SelectNodes(name);
			if (xmlNodeList.Count == 0)
			{
				output = xml.CreateElement(name);
				xmlNode.AppendChild(output);
				return false;
			}
			output = (xmlNodeList[0] as XmlElement);
			return true;
		}

		private XmlElement CreateListElementAndPopulate(XmlDocument xml, XmlNode reference, string type = null)
		{
			XmlElement xmlElement = xml.CreateElement("li");
			if (type != null)
			{
				xmlElement.SetAttribute("Class", type);
			}
			this.Populate(xml, reference, ref xmlElement, false);
			return xmlElement;
		}

		private void Populate(XmlDocument xml, XmlNode reference, ref XmlElement destination, bool overrideExisting = false)
		{
			IEnumerator enumerator = reference.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlNode xmlNode = (XmlNode)obj;
					if (overrideExisting)
					{
						XmlNodeList xmlNodeList = destination.SelectNodes(xmlNode.Name);
						if (xmlNodeList != null)
						{
							IEnumerator enumerator2 = xmlNodeList.GetEnumerator();
							try
							{
								while (enumerator2.MoveNext())
								{
									object obj2 = enumerator2.Current;
									XmlNode oldChild = (XmlNode)obj2;
									destination.RemoveChild(oldChild);
								}
							}
							finally
							{
								IDisposable disposable;
								if ((disposable = (enumerator2 as IDisposable)) != null)
								{
									disposable.Dispose();
								}
							}
						}
					}
					destination.AppendChild(xml.ImportNode(xmlNode, true));
				}
			}
			finally
			{
				IDisposable disposable2;
				if ((disposable2 = (enumerator as IDisposable)) != null)
				{
					disposable2.Dispose();
				}
			}
		}

		private void AddOrReplaceVerbPropertiesCE(XmlDocument xml, XmlNode xmlNode)
		{
			XmlElement xmlElement;
			List<bool> adv = new List<bool>();
			bool multiverb = false;
			bool multiverbCE = this.Properties.node.ChildNodes[0].OuterXml.StartsWith("<li>");
			if (this.GetOrCreateNode(xml, xmlNode, "verbs", out xmlElement))
			{
				int i = 0;
				XmlNodeList xmlNodeList = xmlElement.SelectNodes("li[verbClass=\"Verb_Shoot\" or verbClass=\"Verb_ShootOneUse\" or verbClass=\"Verb_LaunchProjectile\"]");
				multiverb = xmlNodeList.Count > 1;
				IEnumerator enumerator = xmlNodeList.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						XmlNode xmlNode2 = obj as XmlNode;
						if (xmlNode2 != null)
                        {
							bool advanced = xmlNode2.OuterXml.Contains("AdeptusMechanicus.AdvancedVerbProperties");
                            if (advanced)
							{
								//	Log.Message(this.defName+" AdvancedVerb: " + i + " = " + xmlNode2.OuterXml);
							}
							adv.Add(advanced);
							xmlElement.RemoveChild(xmlNode2);
						}
						i++;
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
			}
			if (multiverb)
			{
				string mismatchedVerbCount = "Warning: Multiverb CE patch Mismatched Verb Count for {0}, Patch has {1} verbs, Def has {2} verbs, Only converting Primary Verb. If this is intentional add <warnOnMismatchedVerbCount>false</warnOnMismatchedVerbCount> to its patch";

				if (multiverbCE)
				{
                    if (this.Properties.node.ChildNodes.Count == adv.Count)
					{
						for (int i = 0; i < this.Properties.node.ChildNodes.Count; i++)
						{
							XmlNode item = this.Properties.node.ChildNodes[i];
							string propClass = adv[i] ? "AdeptusMechanicus.AdvancedVerbPropertiesCE" : "CombatExtended.VerbPropertiesCE";
						//	if (adv[i]) Log.Message(this.defName + " AdvancedVerb: " + i + " = " + item.OuterXml);
							xmlElement.AppendChild(this.CreateListElementAndPopulate(xml, item, propClass));
						}
						return;
					}
				}
				if (warnOnMismatchedVerbCount) Log.Warning(string.Format(mismatchedVerbCount, this.defName, adv.Count, multiverbCE ? this.Properties.node.ChildNodes.Count : 1));
			}
			{
				XmlNode item = this.Properties.node;
				string propClass = adv[0] ? "AdeptusMechanicus.AdvancedVerbPropertiesCE" : "CombatExtended.VerbPropertiesCE";
			//	if (adv[0] ) Log.Message(this.defName + " AdvancedVerb: " + 0 + " = " + item.OuterXml);
				xmlElement.AppendChild(this.CreateListElementAndPopulate(xml, item, propClass));
			}
		}

		private void AddOrReplaceCompsCE(XmlDocument xml, XmlNode xmlNode)
		{
			XmlElement xmlElement;
			this.GetOrCreateNode(xml, xmlNode, "comps", out xmlElement);
			if (this.AmmoUser != null)
			{
				xmlElement.AppendChild(this.CreateListElementAndPopulate(xml, this.AmmoUser.node, "CombatExtended.CompProperties_AmmoUser"));
			}
			if (this.FireModes != null)
			{
				xmlElement.AppendChild(this.CreateListElementAndPopulate(xml, this.FireModes.node, "CombatExtended.CompProperties_FireModes"));
			}
		}
		private void ReplaceCompsOversized(XmlDocument xml, XmlNode xmlNode)
		{
			XmlElement xmlElement;
			this.GetOrCreateNode(xml, xmlNode, "comps", out xmlElement);
			XmlNode n = xmlElement.SelectSingleNode("li[@Class=\"OgsCompOversizedWeapon.CompProperties_OversizedWeapon\"]");
			if (n != null)
			{
			//	Log.Message("Oversized found on "+this.defName+"\n"+n.OuterXml);
				XmlElement xmlElement2;
				this.GetOrCreateNode(xml, xmlNode, "graphicData", out xmlElement2);
				XmlNode n2 = xmlElement2.SelectSingleNode("drawSize");
				if (n2 != null)
				{
					var val = n2.InnerText;
				//	Log.Message("with drawSize: "+ val);
					XmlElement xmlElement3;
					this.GetOrCreateNode(xml, xmlNode, "modExtensions", out xmlElement3);
					XmlElement xmlElement4 = xml.CreateElement("li");
					xmlElement4.SetAttribute("Class", "CombatExtended.GunDrawExtension");
					xmlElement3.AppendChild(xmlElement4);
					XmlElement xmlElement5 = xml.CreateElement("DrawSize");
					xmlElement5.InnerText = val+","+ val;
					xmlElement4.AppendChild(xmlElement5);
					/*
					XmlElement xmlElement5 = xml.CreateElement("DrawOffset");
					xmlElement5.InnerText = "true";
					xmlElement3.AppendChild(xmlElement5);
					*/
				}
			}
		}

		private void AddOrReplaceWeaponTags(XmlDocument xml, XmlNode xmlNode)
		{
			XmlElement xmlElement;
			this.GetOrCreateNode(xml, xmlNode, "weaponTags", out xmlElement);
			this.Populate(xml, this.weaponTags.node, ref xmlElement, false);
		}

		private void AddOrReplaceStatBases(XmlDocument xml, XmlNode xmlNode)
		{
			XmlElement xmlElement;
			this.GetOrCreateNode(xml, xmlNode, "statBases", out xmlElement);
			if (xmlElement.HasChildNodes)
			{
				XmlNodeList xmlNodeList = xmlElement.SelectNodes("AccuracyTouch | AccuracyShort | AccuracyMedium | AccuracyLong");
				IEnumerator enumerator = xmlNodeList.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						XmlNode oldChild = (XmlNode)obj;
						xmlElement.RemoveChild(oldChild);
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
			}
			this.Populate(xml, this.statBases.node, ref xmlElement, true);
		}

		private void AddOrReplaceCostList(XmlDocument xml, XmlNode xmlNode)
		{
			XmlElement xmlElement;
			this.GetOrCreateNode(xml, xmlNode, "costList", out xmlElement);
			if (xmlElement.HasChildNodes)
			{
				xmlElement.RemoveAll();
			}
			this.Populate(xml, this.costList.node, ref xmlElement, false);
		}

		private void AddOrReplaceResearchPrereq(XmlDocument xml, XmlNode xmlNode)
		{
			XmlElement xmlElement;
			this.GetOrCreateNode(xml, xmlNode, "recipeMaker", out xmlElement);
			XmlNode xmlNode2 = xmlElement.SelectSingleNode(this.researchPrerequisite.node.Name);
			if (xmlNode2 != null)
			{
				xmlElement.ReplaceChild(xml.ImportNode(this.researchPrerequisite.node, true), xmlNode2);
			}
			else
			{
				xmlElement.AppendChild(xml.ImportNode(this.researchPrerequisite.node, true));
			}
		}

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

		bool warnOnMismatchedVerbCount = true;
		bool warnOnDefNotFound = true;
		public string Name;
		public string defName;
		public string verbPropertiesClass = "CombatExtended.VerbPropertiesCE";
		public bool AllowWithRunAndGun = true;
		public XmlContainer statBases;
		public XmlContainer Properties;
		public XmlContainer AmmoUser;
		public XmlContainer FireModes;
		public XmlContainer weaponTags;
		public XmlContainer costList;
		public XmlContainer researchPrerequisite;
	}
}
