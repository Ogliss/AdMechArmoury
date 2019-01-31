using System;
using System.Xml;
using Verse;

namespace CombatExtended
{
    // Token: 0x0200003A RID: 58
    public class PatchOperationMakeGunCECompatible : PatchOperation
    {
        // Token: 0x060000F2 RID: 242 RVA: 0x0000979C File Offset: 0x0000799C
        protected override bool ApplyWorker(XmlDocument xml)
        {
            bool result = false;
            if (GenText.NullOrEmpty(this.defName))
            {
                return false;
            }
            foreach (object obj in xml.SelectNodes("*/ThingDef[defName=\"" + this.defName + "\"]"))
            {
                result = true;
                XmlNode xmlNode = obj as XmlNode;
                XmlContainer xmlContainer = this.statBases;
                if (xmlContainer != null && xmlContainer.node.HasChildNodes)
                {
                    this.AddOrReplaceStatBases(xml, xmlNode);
                }
                XmlContainer xmlContainer2 = this.costList;
                if (xmlContainer2 != null && xmlContainer2.node.HasChildNodes)
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
            }
            return result;
        }

        // Token: 0x060000F3 RID: 243 RVA: 0x000098CC File Offset: 0x00007ACC
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

        // Token: 0x060000F4 RID: 244 RVA: 0x00009910 File Offset: 0x00007B10
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

        // Token: 0x060000F5 RID: 245 RVA: 0x00009944 File Offset: 0x00007B44
        private void Populate(XmlDocument xml, XmlNode reference, ref XmlElement destination, bool overrideExisting = false)
        {
            foreach (object obj in reference)
            {
                XmlNode xmlNode = (XmlNode)obj;
                if (overrideExisting)
                {
                    XmlNodeList xmlNodeList = destination.SelectNodes(xmlNode.Name);
                    if (xmlNodeList != null)
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

        // Token: 0x060000F6 RID: 246 RVA: 0x00009A04 File Offset: 0x00007C04
        private void AddOrReplaceVerbPropertiesCE(XmlDocument xml, XmlNode xmlNode)
        {
            XmlElement xmlElement;
            if (this.GetOrCreateNode(xml, xmlNode, "verbs", out xmlElement))
            {
                foreach (object obj in xmlElement.SelectNodes("li[verbClass=\"Verb_Shoot\" or verbClass=\"Verb_ShootOneUse\" or verbClass=\"Verb_LaunchProjectile\"]"))
                {
                    XmlNode xmlNode2 = obj as XmlNode;
                    if (xmlNode2 != null)
                    {
                        xmlElement.RemoveChild(xmlNode2);
                    }
                }
            }
            xmlElement.AppendChild(this.CreateListElementAndPopulate(xml, this.Properties.node, "CombatExtended.VerbPropertiesCE"));
        }

        // Token: 0x060000F7 RID: 247 RVA: 0x00009A98 File Offset: 0x00007C98
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

        // Token: 0x060000F8 RID: 248 RVA: 0x00009B04 File Offset: 0x00007D04
        private void AddOrReplaceWeaponTags(XmlDocument xml, XmlNode xmlNode)
        {
            XmlElement xmlElement;
            this.GetOrCreateNode(xml, xmlNode, "weaponTags", out xmlElement);
            this.Populate(xml, this.weaponTags.node, ref xmlElement, false);
        }

        // Token: 0x060000F9 RID: 249 RVA: 0x00009B38 File Offset: 0x00007D38
        private void AddOrReplaceStatBases(XmlDocument xml, XmlNode xmlNode)
        {
            XmlElement xmlElement;
            this.GetOrCreateNode(xml, xmlNode, "statBases", out xmlElement);
            if (xmlElement.HasChildNodes)
            {
                foreach (object obj in xmlElement.SelectNodes("AccuracyTouch | AccuracyShort | AccuracyMedium | AccuracyLong"))
                {
                    XmlNode oldChild = (XmlNode)obj;
                    xmlElement.RemoveChild(oldChild);
                }
            }
            this.Populate(xml, this.statBases.node, ref xmlElement, true);
        }

        // Token: 0x060000FA RID: 250 RVA: 0x00009BC4 File Offset: 0x00007DC4
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

        // Token: 0x060000FB RID: 251 RVA: 0x00009C04 File Offset: 0x00007E04
        private void AddOrReplaceResearchPrereq(XmlDocument xml, XmlNode xmlNode)
        {
            XmlElement xmlElement;
            this.GetOrCreateNode(xml, xmlNode, "recipeMaker", out xmlElement);
            XmlNode xmlNode2 = xmlElement.SelectSingleNode(this.researchPrerequisite.node.Name);
            if (xmlNode2 != null)
            {
                xmlElement.ReplaceChild(xml.ImportNode(this.researchPrerequisite.node, true), xmlNode2);
                return;
            }
            xmlElement.AppendChild(xml.ImportNode(this.researchPrerequisite.node, true));
        }

        // Token: 0x040000A7 RID: 167
        public string defName;

        // Token: 0x040000A8 RID: 168
        public XmlContainer statBases;

        // Token: 0x040000A9 RID: 169
        public XmlContainer Properties;

        // Token: 0x040000AA RID: 170
        public XmlContainer AmmoUser;

        // Token: 0x040000AB RID: 171
        public XmlContainer FireModes;

        // Token: 0x040000AC RID: 172
        public XmlContainer weaponTags;

        // Token: 0x040000AD RID: 173
        public XmlContainer costList;

        // Token: 0x040000AE RID: 174
        public XmlContainer researchPrerequisite;
    }
}
