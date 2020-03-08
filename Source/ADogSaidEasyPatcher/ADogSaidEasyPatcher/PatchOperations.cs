using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Verse;

namespace ADogSaidEasyPatcher
{
    class PatchOperationFindAnyMod : PatchOperation
    {
        private List<string> modNames;
        protected override bool ApplyWorker(XmlDocument xml)
        {
            if (modNames.NullOrEmpty())
                return false;
            foreach (string s in modNames)
                if (ModsConfig.ActiveModsInLoadOrder.Any(m => m.Name == s))
                    return true;
                    
            return false;
        }
    }

    public class PatchOperationAddAndLog : PatchOperationPathed
    {
        private XmlContainer value;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            XmlNode node = this.value.node;
            bool result = false;
            foreach (object current in xml.SelectNodes(this.xpath))
            {
                result = true;
                XmlNode xmlNodeToAddTo = current as XmlNode;
                //Verse.Log.Message("Patching: " + xmlNodeToAddTo?.ParentNode?.ParentNode?.ParentNode["defName"]?.InnerText);

                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    xmlNodeToAddTo.AppendChild(xmlNodeToAddTo.OwnerDocument.ImportNode(node.ChildNodes[i], true));
                }
            }
            return result;
        }
    }

    class PatchOperationSequenceNoFail : PatchOperation
    {
        private List<PatchOperation> operations;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            foreach (PatchOperation p in operations)
            {
                try
                {
                    p.Apply(xml);
                }
                catch (System.Exception ex)
                {
                    Verse.Log.Error("Could not apply Patch for some reason: " + ex.ToString());
                }
            }
            return true;
        }
    }

    public class PatchOperationAppend : PatchOperationPathed
    {
        private XmlContainer value;
        private string nodeToAddTo;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            XmlNode node = this.value.node;
            bool result = false;
            // Verse.Log.Message("ADS Easily Patching animals...");
            int count = 0;
            string log = "ADS Easily Patched ";
            foreach (object current in xml.SelectNodes(this.xpath))
            {
                count++;
                result = true;
                XmlNode xmlNodeToAddTo = current as XmlNode;
                //Verse.Log.Message("ADS Easily Patching: " + xmlNodeToAddTo["defName"]?.InnerText);
                log += xmlNodeToAddTo["defName"]?.InnerText;
                if (!nodeToAddTo.NullOrEmpty())
                {
                    xmlNodeToAddTo = xmlNodeToAddTo[nodeToAddTo];
                    if (xmlNodeToAddTo == null)
                    {
                        log += "+(" + nodeToAddTo + ")";
                        //Verse.Log.Message((current as XmlNode)["defName"]?.InnerText + " does not have a " + nodeToAddTo + " node, adding it.");
                        XmlNode parent = current as XmlNode;
                        xmlNodeToAddTo = xml.CreateElement(nodeToAddTo);
                        parent.AppendChild(xmlNodeToAddTo);
                    }
                }
                log += ", ";
                
                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    xmlNodeToAddTo.AppendChild(xmlNodeToAddTo.OwnerDocument.ImportNode(node.ChildNodes[i], true));
                }
            }
            if (count > 0)
                Verse.Log.Message(log);
            return result;
        }
    }
}
