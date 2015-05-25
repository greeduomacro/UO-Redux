//Copyright (C) 2015 Redux Dev Team (uo-redux.com) All Rights Reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PatchBuilder.Modules
{
    internal static class PatchHelper
    {
        internal static string PatchURL = string.Empty;
        internal static string MasterURL = string.Empty;
        internal static string VersionURL = string.Empty;
        internal static string BackgroundURL = string.Empty;

        static byte[] VersionBytes()
        {
            if (VersionURL != string.Empty)
            {
                try
                {
                    using (WebClient webClient = new WebClient()) {
                        return webClient.DownloadData(VersionURL);
                    }
                }

                catch (Exception e) {
                    LogHandler.LogErrors(e.ToString());
                }
            }

            else LogHandler.LogErrors("Attempted to gather version bytes from empty URL.");

            return new byte[0];
        }

        internal static string VersionString()
        {
            return System.Text.Encoding.UTF8.GetString(VersionBytes());
        }
    }

    class XmlHandler
    {
        private static readonly string savePath = "XML";

        private static List<PatchFile> patches = new List<PatchFile>();

        internal static void GeneratePatchXML(string xmlPath, List<PatchFile> toPatch)
        {
            try
            {
                ClearPatches();

                if (!CanReadPatchFile(xmlPath))
                    LogHandler.LogErrors("Unable to find or find current patch file.");

                for (int i = 0; i < toPatch.Count; i++)
                    AddPatch(toPatch[i]);

                if (patches.Count > 0)
                    GeneratePatchIndex(patches);
            }

            catch (Exception e) { LogHandler.LogErrors(e.ToString()); }
        }

        internal static void AddPatchManually(string xmlPath, PatchFile patch)
        {
            try
            {
                if (!CanReadPatchFile(xmlPath))
                    LogHandler.LogErrors("Unable to find or find current patch file.");

                AddPatch(patch);

                if (patches.Count > 0)
                    GeneratePatchIndex(patches);
            }

            catch (Exception e) { LogHandler.LogErrors(e.ToString()); }
        }

        internal static void AddPatch(PatchFile patch) { patches.Add(patch); }
        internal static void ClearPatches() { patches.Clear(); }

        internal static void GenerateSettingsXml()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter("settings.xml"))
                {
                    XmlTextWriter xml = new XmlTextWriter(writer);

                    xml.Formatting = Formatting.Indented;
                    xml.IndentChar = '\t';
                    xml.Indentation = 2;

                    xml.WriteStartDocument(true);
                    xml.WriteStartElement("Patch-Settings");

                    xml.WriteAttributeString("Patch"     , PatchHelper.PatchURL);
                    xml.WriteAttributeString("Master"    , PatchHelper.MasterURL);
                    xml.WriteAttributeString("Version"   , PatchHelper.VersionURL);
                    xml.WriteAttributeString("Background", PatchHelper.BackgroundURL);                  

                    xml.WriteEndElement();
                    xml.WriteEndDocument();
                    xml.Close();
                }
            }

            catch (Exception e) { LogHandler.LogErrors(e.ToString()); }
        }

        internal static void GeneratePatchIndex(List<PatchFile> patches)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter("patch.xml"))
                {
                    XmlTextWriter xml = new XmlTextWriter(writer);

                    xml.Formatting = Formatting.Indented;
                    xml.IndentChar = '\t';
                    xml.Indentation = 2;

                    xml.WriteStartDocument(true);
                    xml.WriteStartElement("Patch-Index");

                    IteratePatchIndex(patches, xml, PatchHelper.VersionString());

                    xml.WriteEndElement();
                    xml.WriteEndDocument();
                    xml.Close();
                }
            }

            catch (Exception e) { LogHandler.LogErrors(e.ToString()); }
        }

        internal static void GenerateXMLSizeIndex(LocalDirectory directory, BuildHandler handler)
        {
            try
            {
                if (!Directory.Exists(savePath))
                    Directory.CreateDirectory(savePath);

                using (StreamWriter writer = new StreamWriter
                    (Path.Combine(savePath, "index.xml")))
                {
                    XmlTextWriter xml = new XmlTextWriter(writer);

                    xml.Formatting = Formatting.Indented;
                    xml.IndentChar = '\t';
                    xml.Indentation = 2;

                    xml.WriteStartDocument(true);
                    xml.WriteStartElement("Size-Index");

                    ParseLocalDirectory(directory, handler, xml);

                    xml.WriteEndElement();
                    xml.WriteEndDocument();
                    xml.Close();
                }
            }

            catch (Exception e) { LogHandler.LogErrors(e.ToString()); }
        }

        private static void ParseLocalDirectory
            (LocalDirectory directory, BuildHandler handler, XmlTextWriter xml)
        {
            IterateFileSizeIndex
                (directory.FileIndex, directory.DirectoryPath, xml);

            for (int i = 0; i < directory.subDirectories.Count; i++ )
            {
                ParseLocalDirectory(directory.subDirectories[i], handler, xml);
            }
        }

        internal static void IterateFileSizeIndex(List<FileInfo> files, string path, XmlTextWriter xml)
        {
            try
            {
                xml.WriteStartElement(path.Replace(":", string.Empty).Replace("\\", "-"));

                for (int n = 0; n < files.Count; n++)
                {
                    xml.WriteStartElement(files[n].Name);
                    xml.WriteAttributeString("Bytes", files[n].Length.ToString("N0"));
                    xml.WriteString(files[n].FullName);
                    xml.WriteEndElement();
                }

                xml.WriteEndElement();
            }

            catch (Exception e) { LogHandler.LogErrors(e.ToString()); }
        }

        internal static void IteratePatchIndex(List<PatchFile> patches, XmlTextWriter xml, string version)
        {
            try
            {
                xml.WriteStartElement("Patch");
                xml.WriteAttributeString("Version", version);

                for (int n = 0; n < patches.Count; n++)
                {
                    xml.WriteStartElement("File");
                    xml.WriteAttributeString("Date", patches[n].creation.ToString());
                    xml.WriteAttributeString("Name", patches[n].fileName);
                    xml.WriteAttributeString("Location", patches[n].filePath);
                    xml.WriteAttributeString("Bytes", patches[n].fileBytes.ToString("N0"));
                    xml.WriteEndElement();
                }

                xml.WriteEndElement();
            }

            catch (Exception e) { LogHandler.LogErrors(e.ToString()); }
        }

        internal static bool CanReadSettings(string path)
        {
            if (!File.Exists(path))
                LogHandler.LogErrors("Unable to find settings file.");

            XmlDocument doc = new XmlDocument();
            XmlElement root;

            try
            {
                doc.Load(path);
                root = doc["Patch-Settings"];

                PatchHelper.PatchURL = root.GetAttribute("Patch");
                PatchHelper.MasterURL = root.GetAttribute("Master");
                PatchHelper.VersionURL = root.GetAttribute("Version");
                PatchHelper.BackgroundURL = root.GetAttribute("Background");
            }

            catch (Exception e) { LogHandler.LogErrors(e.ToString()); return false; }

            return true;
        }

        internal static bool CanReadPatchFile(string path)
        {
            if (!File.Exists(path))
            {
                LogHandler.LogErrors("Unable to find patch file.");
                return false;
            }

            XmlDocument doc = new XmlDocument();
            XmlElement root;

            try
            {
                doc.Load("settings.xml");
                root = doc["Patch-Index"];

                foreach (XmlElement ele in root.GetElementsByTagName("Patch"))
                {
                    string version = ele.Attributes["Version"].Value;

                    foreach (XmlElement subEle in ele.GetElementsByTagName("File"))
                    {
                        long bytes;

                        string location = ele.Attributes["Location"].Value;
                        string name = ele.Attributes["Name"].Value;
                        if (!long.TryParse(ele.Attributes["Bytes"].Value, out bytes))
                            LogHandler.LogErrors(string.Format("Unable to parse size of [{0}]", name));

                        patches.Add(new PatchFile(location, name, bytes, PatchHelper.VersionString()));
                    }
                }

                return true;
            }

            catch (Exception e) { LogHandler.LogErrors(e.ToString()); return false; }
        }
    }
}
