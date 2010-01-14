using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Sp.Data.Caml
{
    public static class Caml
    {
        public static Field GetCamlField(this XElement xmlNode)
        {
            Field field = new Field();
            field.Name = xmlNode.Attribute("Name").Value;
            field.IsRequired  = xmlNode.AttributeBoolOrFalse("Required");
            field.IsReadOnly = xmlNode.AttributeBoolOrFalse("ReadOnly");
            field.IsHidden = xmlNode.AttributeBoolOrFalse("Hidden");
            field.FieldType   = xmlNode.Attribute("Type").Value;
            field.DisplayName = xmlNode.Attribute("DisplayName").Value;
            field.IsIndexed = xmlNode.AttributeBoolOrFalse("Indexed");
            field.List = xmlNode.AttributeValueOrDefault("List");
            field.IsPrimaryKey = xmlNode.AttributeBoolOrFalse("PrimaryKey");
            return field;
        }

        public static FieldRef GetCamlFieldRef(this XElement xmlNode)
        {
            FieldRef fieldRef = new FieldRef();
            fieldRef.Name = xmlNode.Attribute("Name").Value;
            fieldRef.DisplayName = xmlNode.Attribute("DisplayName") == null ? 
                null : xmlNode.Attribute("DisplayName").Value;
            fieldRef.IsHidden = xmlNode.AttributeBoolOrFalse("Hidden");
            fieldRef.IsReadOnly = xmlNode.AttributeBoolOrFalse("ReadOnly");
            fieldRef.IsRequired = xmlNode.AttributeBoolOrFalse("Required");
            return fieldRef;
        }

        public static List GetCamlList(this XElement xmlNode)
        {
            List list = new List();
            list.Name = xmlNode.Attribute("Title").Value;
            list.ID   = new Guid(xmlNode.Attribute("ID").Value);
            list.Description = xmlNode.AttributeValueOrDefault("Description");
            list.InternalName = xmlNode.Attribute("Name").Value;
            list.Version = Int32.Parse(xmlNode.Attribute("Version").Value);
            list.IsHidden = xmlNode.AttributeBoolOrFalse("Hidden");
            list.IsOrdered = xmlNode.AttributeBoolOrFalse("Ordered");
            return list;
        }

        public static ListDef GetCamlListDef(this XElement xmlNode)
        {
            ListDef listdef = new ListDef();

            listdef.List = xmlNode.GetCamlList();
            listdef.Fields = 
                (from fieldNode in xmlNode.Element(xmlNode.GetDefaultNamespace() + "Fields")
                     .Elements(xmlNode.GetDefaultNamespace() + "Field")
                 select fieldNode.GetCamlField()).ToList<Field>();
            
            
            return listdef;
        }

        public static ViewDef GetCamlViewDef(this XElement xmlNode)
        {
            XNamespace defaultNs = xmlNode.GetDefaultNamespace();
            ViewDef view = new ViewDef();

            XElement viewNode = xmlNode.Element(defaultNs + "View");
            view.Name = viewNode.Attribute("Name").Value;
            view.DisplayName = viewNode.Attribute("DisplayName").Value;
            view.Query = viewNode.Element(defaultNs + "Query").GetXmlNode();
            view.ViewFields = viewNode.Element(defaultNs + "ViewFields").Elements("FieldRef").Select(n => n.GetCamlFieldRef()).ToList();
            view.ListDef = xmlNode.Element(defaultNs + "List").GetCamlListDef();

            return view;
        }

        public static ListCollection GetCamlListCollection(this XElement xmlNode)
        {
            return new ListCollection(
                xmlNode
                .Elements(xmlNode.GetDefaultNamespace() + "List")
                .Select(l => l.GetCamlList()));
        }

        public static ListItem GetCamlListItem(this XElement xmlNode)
        {
            ListItem item = new ListItem();
            item.Fields = 
                (from att in xmlNode.Attributes()
                 where att.Name.LocalName.StartsWith("ows_")
                 select new KeyValuePair<string, string>(
                     att.Name.LocalName.Substring("ows_".Length), 
                     att.Value)).ToList();

            item.ID = Int32.Parse(item["ID"]);
            return item;
        }

        public static ListItemCollection GetCamlListItemCollection(this XElement xmlNode)
        {
            XNamespace zns = xmlNode.GetNamespaceOfPrefix("z");
            if (zns != null)
            {
                ListItemCollection items =
                    new ListItemCollection(
                        xmlNode
                        .Elements(zns + "row")
                        .Select(e => e.GetCamlListItem()));

                items.NextPage = xmlNode.AttributeValueOrDefault("ListItemCollectionPositionNext");
                items.ItemCount = Int32.Parse(xmlNode.Attribute("ItemCount").Value);
                return items;
            }
            else
                return new ListItemCollection(new List<ListItem>());
        }

        public static ChangeItem GetCamlChangeItem(this XElement xmlNode)
        {
            ChangeItem c = new ChangeItem();
            c.Command = xmlNode.Attribute("ChangeType").Value;
            if (c.Command != "InvalidToken")
                c.ListItemID = Int32.Parse(xmlNode.Value);
            return c;
        }

        public static ChangeLog GetCamlChangeLog(this XElement xmlNode)
        {
            ChangeLog log = new ChangeLog(
                xmlNode.Elements(xmlNode.GetDefaultNamespace() + "Id").Select(c => c.GetCamlChangeItem())
                );

            log.MoreChanges = xmlNode.AttributeBoolOrFalse("MoreChanges");
            log.NextLastChangeToken = xmlNode.AttributeValueOrDefault("LastChangeToken");

            XElement newListDef = xmlNode.Element(xmlNode.GetDefaultNamespace() + "List");

            if (newListDef != null)
                log.NewListDef = newListDef.GetCamlListDef();
            else
                log.NewListDef = null;

            return log;
        }

        public static ChangeBatch GetCamlChangeBatch(this XElement xmlNode)
        {
            ChangeBatch changes = new ChangeBatch();

            if (xmlNode.Element(xmlNode.GetDefaultNamespace() + "Changes") != null)
                changes.ChangeLog = xmlNode.Element(xmlNode.GetDefaultNamespace() + "Changes").GetCamlChangeLog();
            else
                changes.ChangeLog = new ChangeLog(new List<ChangeItem>());

            if (xmlNode.GetNamespaceOfPrefix("rs") != null)
                changes.ChangedItems = xmlNode.Element(xmlNode.GetNamespaceOfPrefix("rs") + "data").GetCamlListItemCollection();
            else
                changes.ChangedItems = new ListItemCollection(new List<ListItem>());

            if (xmlNode.Attribute("MinTimeBetweenSyncs") != null)
            {
                changes.MinTimeBetweenSyncs = Int32.Parse(xmlNode.Attribute("MinTimeBetweenSyncs").Value);
                changes.RecommendedTimeBetweenSyncs = Int32.Parse(xmlNode.Attribute("RecommendedTimeBetweenSyncs").Value);
            }
            return changes;
        }

        public static UpdateResults GetCamlUpdateResults(this XElement xmlNode)
        {
            UpdateResults results = new UpdateResults();
            XNamespace defaultNs = xmlNode.GetDefaultNamespace();
            var resultList = xmlNode.Elements(defaultNs + "Result").Select(n => n.GetCamlUpdateResult());
            
            foreach (UpdateResult r in resultList)
                results.Add(r);
            
            return results;
        }

        public static UpdateResult GetCamlUpdateResult(this XElement xmlNode)
        {
            UpdateResult result = new UpdateResult();
            XNamespace defaultns = xmlNode.GetDefaultNamespace();
            //XNamespace rowsetns = xmlNode.GetNamespaceOfPrefix("z");
            XNamespace rowsetns = "#RowsetSchema";
            string ResultID = xmlNode.Attribute("ID").Value;
            string[] IdAndCommand = ResultID.Split(",".ToCharArray());
            result.UpdateItemID = Int32.Parse(IdAndCommand[0]);
            result.Command = IdAndCommand[1];
            result.ErrorCode = xmlNode.Element(defaultns + "ErrorCode").Value;
            XElement errorMessageElement = xmlNode.Elements(defaultns + "ErrorText").FirstOrDefault();
            result.ErrorMessage = (errorMessageElement == null) ? null : errorMessageElement.Value;
            XElement listItemElement = xmlNode.Elements(rowsetns + "row").FirstOrDefault();
            result.ItemData = listItemElement == null ? null : listItemElement.GetCamlListItem();
            return result;
        }

        private static string AttributeValueOrDefault(this XElement xmlNode, string attribute)
        {
            if (xmlNode.Attribute(attribute) != null)
                return xmlNode.Attribute(attribute).Value;
            else
                return null;
        }

        private static bool AttributeBoolOrFalse(this XElement xmlNode, string attribute)
        {
            string b = xmlNode.AttributeValueOrDefault(attribute);
            return b != null && (b.ToUpper() == "YES" || b.ToUpper() == "TRUE");
        }

        public static XmlNode GetCamlQueryOptions(this QueryOptions options)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement queryOptions = doc.CreateElement("QueryOptions");
            XmlElement paging = doc.CreateElement("Paging");
            XmlElement dateInUtc = doc.CreateElement("DateInUtc");

            if (options.PagingToken != null)
            {
                paging.SetAttribute("ListItemCollectionPositionNext", options.PagingToken);
                queryOptions.AppendChild(paging);
            }

            if (options.DateInUtc)
            {
                dateInUtc.Value = "TRUE";
                queryOptions.AppendChild(dateInUtc);
            }

            return queryOptions;
        }

        public static XmlNode GetCamlUpdateItem(this UpdateItem item, XmlDocument doc)
        {
            XmlElement method = doc.CreateElement("Method");
            method.SetAttribute("ID", item.ID.ToString());
            method.SetAttribute("Cmd", item.Command);
            foreach (var kvp in item.ChangedItemData.Fields)
            {
                if (kvp.Key != "ID")
                {
                    XmlElement f = doc.CreateElement("Field");
                    f.SetAttribute("Name", kvp.Key);
                    f.InnerXml = kvp.Value;
                    method.AppendChild(f);
                }
            }

            XmlElement fid = doc.CreateElement("Field");
            fid.SetAttribute("Name", "ID");
            fid.InnerXml = item.Command == "New" ? "New" : item.ListItemID.ToString();
            method.AppendChild(fid);

            return method;
        }

        public static XmlNode GetCamlUpdateBatch(this UpdateBatch updateBatch)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement batch = doc.CreateElement("Batch");
            batch.SetAttribute("OnError", updateBatch.ContinueOnError ? "Continue" : "Return");

            foreach (UpdateItem i in updateBatch)
                batch.AppendChild(i.GetCamlUpdateItem(doc));
            return batch;
        }
    }
}
