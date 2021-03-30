using System;
using System.Collections;
using System.Xml;
using Verse;
// AdeptusMechanicus.PatchOperationAddOrReplace
namespace AdeptusMechanicus
{
    /*
    * A custom patch operation to simplify sequence patch operations when defensively adding fields
    * Code by Lanilor (https://github.com/Lanilor)
    * This code is provided "as-is" without any warrenty whatsoever. Use it on your own risk.
    */
    public class PatchOperationAddOrReplace : PatchOperationPathed
    {

        protected string key;
        private XmlContainer value;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            XmlNode valNode = value.node;
            bool result = false;
            IEnumerator enumerator = xml.SelectNodes(xpath).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    object obj = enumerator.Current;
                    result = true;
                    XmlNode parentNode = obj as XmlNode;
                    XmlNode xmlNode = parentNode.SelectSingleNode(key);
                    if (xmlNode == null)
                    {
                        // Add - Add node if not existing
                        xmlNode = parentNode.OwnerDocument.CreateElement(key);
                        parentNode.AppendChild(xmlNode);
                    }
                    else
                    {
                        // Replace - Clear existing children
                        xmlNode.RemoveAll();
                    }
                    // (Re)add value
                    xmlNode.AppendChild(parentNode.OwnerDocument.ImportNode(valNode.FirstChild, true));
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
            return result;
        }

    }

}