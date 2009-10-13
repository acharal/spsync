using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Security;
using System.Security.Permissions;
using System.Globalization;
using Microsoft.Win32;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.WebPartPages;
using Microsoft.SharePoint.Security;

namespace SampleSpSite
{
    [CLSCompliant(false)]
    [Guid("b654e365-f597-4b93-83dc-35c8da803f44")]
    partial class SiteProvisioning : SPFeatureReceiver
    {
        /// <summary>
        ///  These methods are required for deploying Site Definitions 
        ///  and should not be modified. Use SiteProvisioning.cs
        ///  for your custom provisioning code.
        /// </summary>

        #region SharePoint Site Definition generated code

        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            if (properties == null)
            {
                return;
            }

            SPWeb web = properties.Feature.Parent as SPWeb;
            string filePath = this.GetProvisioinerFilePath(properties.Definition);

            this.RestoreWebProperties(web, filePath);
            this.RestoreCss(web, filePath);
            this.RestoreDataViewOutZone(web, filePath);
            this.RestoreDataViewInZone(web, filePath);

            this.OnActivated(properties);
        }

        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
        }

        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        {
        }

        [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
        public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        {
        }

        private void RestoreWebProperties(SPWeb web, string filePath)
        {
            if (!File.Exists(filePath) || web == null)
            {
                return;
            }

            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(filePath);
            }
            catch (XmlException)
            {
                return;
            }

            XmlNode xWebProperties = doc.DocumentElement.SelectSingleNode("WebProperties");
            if (xWebProperties == null)
            {
                return;
            }

            foreach (XmlNode xWebProperty in xWebProperties.ChildNodes)
            {
                if (xWebProperty.Attributes["Key"] != null &&
                   xWebProperty.Attributes["Value"] != null)
                {
                    string key = xWebProperty.Attributes["Key"].Value;
                    string value = xWebProperty.Attributes["Value"].Value;
                    if (web.Properties.ContainsKey(key))
                    {
                        web.Properties[key] = value;
                    }
                    else
                    {
                        web.Properties.Add(key, value);
                    }
                }
            }

            web.Properties.Update();
        }

        private void RestoreCss(SPWeb web, string filePath)
        {
            if (!File.Exists(filePath) || web == null)
            {
                return;
            }

            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(filePath);
            }
            catch (XmlException)
            {
                return;
            }

            XmlNode xCustomizedCssFiles = doc.DocumentElement.SelectSingleNode("CustomizedCssFiles");
            if (xCustomizedCssFiles == null)
            {
                return;
            }

            List<string> customizedCssFiles = new List<string>();
            foreach (XmlNode xCustomizedCssFile in xCustomizedCssFiles.ChildNodes)
            {
                XmlAttribute xName = xCustomizedCssFile.Attributes["Name"];
                if (xName == null)
                {
                    continue;
                }

                customizedCssFiles.Add(xName.Value);
            }

            SPFolder rootFolder = web.RootFolder;
            SPFolder cssFolder = null;
            string targetFolder = "_styles";
            foreach (SPFolder subFolder in rootFolder.SubFolders)
            {
                if (subFolder.Name.ToLower(CultureInfo.InvariantCulture) == targetFolder)
                {
                    cssFolder = subFolder;
                    break;
                }
            }

            if (cssFolder == null)
            {
                return;
            }

            foreach (SPFile file in cssFolder.Files)
            {
                if (customizedCssFiles.Contains(file.Name))
                {
                    web.CustomizeCss(file.Name);
                    web.Update();
                }
            }
        }

        private void RestoreDataViewInZone(SPWeb web, string filePath)
        {
            if (!File.Exists(filePath) || web == null)
            {
                return;
            }

            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(filePath);
            }
            catch (XmlException)
            {
                return;
            }

            XmlNodeList xFixupFiles = doc.DocumentElement.SelectNodes("FixupFiles/FixupFile[@DataViewInZone=\"TRUE\"]");
            foreach (XmlNode xFixupFile in xFixupFiles)
            {
                XmlAttribute xRelativePath = xFixupFile.Attributes["RelativePath"];
                if (xRelativePath == null)
                {
                    continue;
                }
                string relativePath = xRelativePath.Value;

                SPFile file = web.GetFile(relativePath);
                if (file == null)
                {
                    continue;
                }

                SPLimitedWebPartManager manager = file.GetLimitedWebPartManager(System.Web.UI.WebControls.WebParts.PersonalizationScope.Shared);
                SPLimitedWebPartCollection pageWebParts = manager.WebParts;
                if (pageWebParts == null)
                {
                    continue;
                }

                foreach (System.Web.UI.WebControls.WebParts.WebPart webPart in pageWebParts)
                {
                    DataFormWebPart dataForm = webPart as DataFormWebPart;
                    if (dataForm == null)
                    {
                        continue;
                    }

                    this.SubstituteGuidInZone(web, manager, dataForm, filePath);
                }
            }
        }

        private void SubstituteGuidInZone(SPWeb web, SPLimitedWebPartManager manager, DataFormWebPart dataForm, string filePath)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(filePath);
            }
            catch (XmlException)
            {
                return;
            }

            XmlNode xListInstances = doc.DocumentElement.SelectSingleNode("ListInstances");
            if (xListInstances == null)
            {
                return;
            }
            foreach (XmlNode xListInstance in xListInstances.ChildNodes)
            {
                if (xListInstance.Attributes["Id"] == null ||
                    xListInstance.Attributes["Title"] == null)
                {
                    return;
                }

                string oldId = xListInstance.Attributes["Id"].Value;
                string title = xListInstance.Attributes["Title"].Value;

                SPList list = null;
                try
                {
                    list = web.Lists[title];
                }
                catch (ArgumentException)
                {
                    continue;
                }

                if (list == null)
                {
                    continue;
                }
                string newId = list.ID.ToString();

                dataForm.ParameterBindings = Regex.Replace(dataForm.ParameterBindings, oldId, newId, RegexOptions.IgnoreCase);
                dataForm.DataSourcesString = Regex.Replace(dataForm.DataSourcesString, oldId, newId, RegexOptions.IgnoreCase);
                dataForm.Xsl = Regex.Replace(dataForm.Xsl, oldId, newId, RegexOptions.IgnoreCase);

                manager.SaveChanges(dataForm);
            }
        }

        private void RestoreDataViewOutZone(SPWeb web, string filePath)
        {
            if (!File.Exists(filePath) || web == null)
            {
                return;
            }

            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(filePath);
            }
            catch (XmlException)
            {
                return;
            }

            XmlNodeList xFixupFiles = doc.DocumentElement.SelectNodes("FixupFiles/FixupFile[@DataViewOutZone=\"TRUE\"]");
            foreach (XmlNode xFixupFile in xFixupFiles)
            {
                XmlAttribute xRelativePath = xFixupFile.Attributes["RelativePath"];
                if (xRelativePath == null)
                {
                    continue;
                }
                string relativePath = xRelativePath.Value;

                SPFile file = web.GetFile(relativePath);
                if (file == null)
                {
                    continue;
                }

                string fileName = file.Name;

                string content = String.Empty;
                using (StreamReader reader = new StreamReader(file.OpenBinaryStream()))
                {
                    content = reader.ReadToEnd();
                    content = this.SubstituteGuid(web, filePath, content);
                }

                UTF8Encoding encoder = new UTF8Encoding();
                byte[] contentAsBytes = encoder.GetBytes(content);

                // store to temporary file
                SPFile temp = web.Files.Add("temp.aspx", contentAsBytes);

                SPLimitedWebPartManager sourceManager = file.GetLimitedWebPartManager(System.Web.UI.WebControls.WebParts.PersonalizationScope.Shared);
                SPLimitedWebPartCollection sourceWebParts = sourceManager.WebParts;

                SPLimitedWebPartManager targetManager = temp.GetLimitedWebPartManager(System.Web.UI.WebControls.WebParts.PersonalizationScope.Shared);
                foreach (System.Web.UI.WebControls.WebParts.WebPart webPart in sourceWebParts)
                {
                    string zoneId = sourceManager.GetZoneID(webPart);
                    targetManager.AddWebPart(webPart, zoneId, webPart.ZoneIndex);
                }

                foreach (SPWebPartConnection connection in sourceManager.SPWebPartConnections)
                {
                    targetManager.SPConnectWebParts(connection.Provider, connection.ProviderConnectionPoint, connection.Consumer, connection.ConsumerConnectionPoint, connection.Transformer);
                }

                file.Delete();

                temp.CopyTo(fileName);
                temp.Delete();

                web.Update();
            }
        }


        private string SubstituteGuid(SPWeb web, string filePath, string content)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(filePath);
            }
            catch (XmlException)
            {
                return String.Empty;
            }

            string correctContent = content;

            XmlNode xListInstances = doc.DocumentElement.SelectSingleNode("ListInstances");
            if (xListInstances == null)
            {
                return String.Empty;
            }

            foreach (XmlNode xListInstance in xListInstances.ChildNodes)
            {
                if (xListInstance.Attributes["Id"] == null ||
                    xListInstance.Attributes["Title"] == null)
                {
                    continue;
                }

                string oldId = xListInstance.Attributes["Id"].Value;
                string title = xListInstance.Attributes["Title"].Value;
                SPList list = null;
                try
                {
                    list = web.Lists[title];
                }
                catch (ArgumentException)
                {
                    continue;
                }

                if (list == null)
                {
                    return String.Empty;
                }
                string newId = list.ID.ToString();

                correctContent = Regex.Replace(correctContent, oldId, newId, RegexOptions.IgnoreCase);
            }

            return correctContent;
        }

        private string GetProvisioinerFilePath(SPFeatureDefinition featureDefinition)
        {
            string sharepointFeaturesDir = this.GetSharePointFeaturesDirectory();
            string filePath = String.Empty;
            if (featureDefinition != null && !String.IsNullOrEmpty(sharepointFeaturesDir))
            {
                string featureName = featureDefinition.DisplayName;
                string featureDir = Path.Combine(sharepointFeaturesDir, featureName);
                filePath = Path.Combine(featureDir, "SiteProvisioning.xml");
            }

            return filePath;
        }

        private string GetSharePointFeaturesDirectory()
        {
            string key = @"SOFTWARE\Microsoft\Shared Tools\Web Server Extensions\12.0";
            string name = "Location";

            string featuresDir = String.Empty;
            try
            {
                RegistryKey regKey = Registry.LocalMachine.OpenSubKey(key);
                string value = regKey.GetValue(name) as string;
                regKey.Close();

                featuresDir = Path.Combine(value, @"template\features");
            }
            catch (SecurityException)
            {
                featuresDir = String.Empty;
            }
            catch (ArgumentNullException)
            {
                featuresDir = String.Empty;
            }
            catch (ArgumentException)
            {
                featuresDir = String.Empty;
            }
            catch (ObjectDisposedException)
            {
                featuresDir = String.Empty;
            }
            catch (IOException)
            {
                featuresDir = String.Empty;
            }
            catch (UnauthorizedAccessException)
            {
                featuresDir = String.Empty;
            }

            return featuresDir;
        }

        #endregion
    }
}
