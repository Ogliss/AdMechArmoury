using System;
using System.Collections;
using System.Xml;
using Verse;

namespace CombatExtended
{
    public class PatchOperationMakeProjectileCECompatible : PatchOperation
	{
		protected override bool ApplyWorker(XmlDocument xml)
		{
			bool flag = false;
			bool result;
			if (this.defName.NullOrEmpty() && this.Name.NullOrEmpty())
			{
				result = false;
			}
			else
			{
				string search = this.defName.NullOrEmpty() ? "@Name" : "defName";
				string s = this.defName.NullOrEmpty() ? this.Name : this.defName;
				foreach (object obj in xml.SelectNodes("Defs/ThingDef["+ search +"=\"" + s + "\"]"))
				{
					flag = true;
					XmlNode xmlNode = obj as XmlNode;
				//	Log.Message("Original: "+xmlNode.OuterXml.ToString());
					this.SetAttributes(xmlNode, "Class", defClass);
					if (!thingClass.NullOrEmpty())
					{
						this.AddOrReplaceThingClass(xml, xmlNode);
					}
					XmlContainer xmlContainer = this.projectile;
					bool flag3 = xmlContainer != null && xmlContainer.node.HasChildNodes;
					if (flag3)
					{
						this.ReplaceProjectileProps(xml, xmlNode);
					}
				//	Log.Message("Modified: " + xmlNode.OuterXml.ToString());
				}
				result = flag;
			}
			return result;
		}

        private void SetAttributes(XmlNode xmlNode, string attribute, string value)
		{
			string valOld = "Null";
			if (xmlNode.Attributes[attribute] != null)
			{
				valOld = "Old Value:" + xmlNode.Attributes[attribute].Value;
				xmlNode.Attributes[attribute].Value = value;
			}
			else
			{
				XmlAttribute xmlAttribute = xmlNode.OwnerDocument.CreateAttribute(attribute);
				xmlAttribute.Value = value;
				xmlNode.Attributes.Append(xmlAttribute);
			}
		//	Log.Message(string.Format("{0}: Old: {1}, New: {2}", attribute, valOld, xmlNode.Attributes[attribute].Value));
		}

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

		private void ReplaceProjectileProps(XmlDocument xml, XmlNode xmlNode)
		{
			XmlElement xmlElement;
			this.GetOrCreateNode(xml, xmlNode, "projectile", out xmlElement);
			if (xmlElement.HasChildNodes)
			{
				XmlNodeList xmlNodeList = xmlElement.SelectNodes("armorPenetrationBase");
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
			this.Populate(xml, this.projectile.node, ref xmlElement, true);
			if (xmlElement.Attributes["Class"] == null)
			{
				this.SetAttributes(xmlElement, "Class", projectilePropsClass);
			}
		}
		private void AddOrReplaceThingClass(XmlDocument xml, XmlNode parentNode)
		{
			XmlNode xmlNode = parentNode.SelectSingleNode("thingClass");
			if (xmlNode == null)
			{
				// Add - Add node if not existing
				xmlNode = xml.CreateElement("thingClass");
				parentNode.AppendChild(xmlNode);
			}
			else
			{
				// Replace - Clear existing children
				xmlNode.RemoveAll();
			}
			xmlNode.InnerText = this.thingClass;
		}

		public string Name;
		public string defName;
		public string defClass = "CombatExtended.AmmoDef";
		public string projectilePropsClass = "CombatExtended.ProjectilePropertiesCE";
		public string thingClass;
		public XmlContainer projectile;
	}
}
