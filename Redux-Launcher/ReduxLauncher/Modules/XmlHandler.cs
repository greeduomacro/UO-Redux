//Copyright (C) 2015 Redux Dev Team (uo-redux.com) All Rights Reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ReduxLauncher.Modules
{
    internal static class PatchHelper
    {
        internal static string PatchURL = string.Empty;
        internal static string MasterURL = string.Empty;
        internal static string VersionURL = string.Empty;
        internal static string BackgroundURL = string.Empty;
    }

    class XmlHandler
    {
        private static readonly string savePath = "XML";

        private static List<PatchFile> patches = new List<PatchFile>();

        internal static void GeneratePatchXML(string xmlPath, List<PatchFile> toPatch)
        {
            try
            {
                if (ReadPatchFile(xmlPath))
                {
                    /// Undecided.
                }

                for (int i = 0; i < toPatch.Count; i++)
                    AddPatch(toPatch[i]);

                if (patches.Count > 0)
                    GeneratePatchIndex(patches);
            }

            catch (Exception e) { LogHandler.LogErrors(e.ToString()); }
        }

        internal static void AddPatch(PatchFile patch)
        {
            patches.Add(patch);
        }

        internal static bool CanReadSettings(string path)
        {
            if (!File.Exists(path))
            {
                LogHandler.LogErrors("Unable to find patch file");
                return false;
            }

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

        internal static void GeneratePatchIndex(List<PatchFile> patches)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter
                    (Path.Combine("patch.xml")))
                {
                    XmlTextWriter xml = new XmlTextWriter(writer);

                    xml.Formatting = Formatting.Indented;
                    xml.IndentChar = '\t';
                    xml.Indentation = 2;

                    xml.WriteStartDocument(true);
                    xml.WriteStartElement("Patch-Index");

                    IndexPatches(patches, xml);

                    xml.WriteEndElement();
                    xml.WriteEndDocument();
                    xml.Close();
                }
            }

            catch (Exception e) { LogHandler.LogErrors(e.ToString()); }
        }

        internal static void GenerateXMLSizeIndex(LocalDirectory directory, PatchHandler handler)
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

        private static void IndexPatches
            (List<PatchFile> patches, XmlTextWriter xml)
        {
            IteratePatchIndex(patches, xml);
        }

        private static void ParseLocalDirectory
            (LocalDirectory directory, PatchHandler handler, XmlTextWriter xml)
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

        internal static void IteratePatchIndex(List<PatchFile> patches, XmlTextWriter xml)
        {
            try
            {
                for (int n = 0; n < patches.Count; n++)
                {
                    xml.WriteStartElement("Patch");
                    xml.WriteAttributeString("Date", patches[n].creation.ToString());
                    xml.WriteAttributeString("Name", patches[n].fileName);
                    xml.WriteAttributeString("Location", patches[n].filePath);
                    xml.WriteAttributeString("Bytes", patches[n].fileBytes.ToString("N0"));
                    xml.WriteString(n.ToString("N0"));
                    xml.WriteEndElement();
                }
            }

            catch (Exception e) { LogHandler.LogErrors(e.ToString()); }
        }

        internal static bool ReadPatchFile(string path)
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
                doc.Load("patch.xml");
                root = doc["Patch-Index"];

                foreach (XmlElement ele in root.GetElementsByTagName("Patch"))
                {
                    string name = ele.Attributes["Name"].Value;
                    string location = ele.Attributes["Location"].Value;

                    long bytes;

                    if (!long.TryParse(ele.Attributes["Bytes"].Value, out bytes))
                        LogHandler.LogErrors(string.Format("Unable to parse size of [{0}]", name));

                    patches.Add(new PatchFile(location, name, bytes));
                }

                return true;
            }

            catch (Exception e) { LogHandler.LogErrors(e.ToString()); return false; }
        }
    }
}
