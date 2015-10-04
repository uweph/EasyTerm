using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EasyTermCore
{
    public class TermbaseSet
    {
        public List<TermBaseFile> Files { get; private set; }
        public string SettingsFile { get; set; }

        public TermbaseSet()
        {
            Files = new List<TermBaseFile>();
        }


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,28.08.2015</created>
        /// <changed>UPh,28.08.2015</changed>
        // ********************************************************************************
        public void ClearData()
        {
            Files.Clear();
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <created>UPh,28.08.2015</created>
        /// <changed>UPh,28.08.2015</changed>
        // ********************************************************************************
        public void Save()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            using (XmlWriter writer = XmlWriter.Create(SettingsFile, settings))
            {
                writer.WriteStartElement("termbase_set");

                writer.WriteStartElement("termbases");

                foreach (TermBaseFile file in Files)
                {
                    writer.WriteStartElement("file");
                    writer.WriteAttributeString("path", file.StoragePath);
                    writer.WriteAttributeString("active", file.Active ? "true" : "false");
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndElement();
            }

        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <created>UPh,28.08.2015</created>
        /// <changed>UPh,28.08.2015</changed>
        // ********************************************************************************
        public void Load()
        {
            ClearData();
            if (!File.Exists(SettingsFile))
                return;

            using (XmlReader reader = XmlReader.Create(SettingsFile))
            {
                CXmlNode elRoot = new CXmlNode();
                if (!reader.ReadRootElement(ref elRoot))
                    return;

                CXmlNode elChild = new CXmlNode();
                while (elRoot.ReadChildNode(ref elChild))
                {
                    if (elChild.IsElement("termbases"))
                    {
                        CXmlNode elTermBase = new CXmlNode();
                        while (elChild.ReadChildNode(ref elTermBase, "file"))
                        {
                            string path;
                            bool active;
                            if (elTermBase.GetAttribute("path", out path) &&
                                elTermBase.GetBoolAttribute("active", out active))
                            {
                                TermBaseFile file = new TermBaseFile(path);
                                file.Active = active;
                                Files.Add(file);

                            }
                        }
                    }
                }
            }
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,29.08.2015</created>
        /// <changed>UPh,29.08.2015</changed>
        // ********************************************************************************
        public void LoadStoredAndLocal()
        {
            Load();
            if (AddLocalFiles())
                Save();
        }


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,29.08.2015</created>
        /// <changed>UPh,29.08.2015</changed>
        // ********************************************************************************
        private bool AddLocalFiles()
        {

            string localdir = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            bool bChanged = false;

            List<string> reldirs = new List<string>();
            reldirs.Add("TermBases");
            reldirs.Add("..\\TermBases");

            // Hold local files to find which one can be deleted
            List<TermBaseFile> localFiles = new List<TermBaseFile>();
            foreach (TermBaseFile file in Files)
            {
                if (file.IsLocal)
                    localFiles.Add(file);
            }

            // Loop directories where we expect termbases
            foreach (string reldir in reldirs)
            {
                string searchpath = Path.GetFullPath(Path.Combine(localdir, reldir));
                if (!Directory.Exists(searchpath))
                    continue;

                // Collect files in that directory
                var filteredFiles = Directory
                    .EnumerateFiles(searchpath)
                    .Where(file => string.Compare(Path.GetExtension(file), ".tbx", true) == 0 ||
                                   string.Compare(Path.GetExtension(file), ".sdltb", true) == 0)
                    .ToList();

                // Loop files
                foreach (string filepath in filteredFiles)
                {
                    string storagePath = "%local%\\" + Tools.MakeRelativePath(localdir + "\\x", filepath);
                    
                    TermBaseFile existingFile = FindStoragePath(storagePath);
                    if (existingFile == null)
                    {
                        // Add new file
                        existingFile = new TermBaseFile(storagePath);
                        existingFile.Active = true;
                        Files.Add(existingFile);
                        bChanged = true;
                    }
                    else
                    {
                        // Remove from localFiles
                        int inx = localFiles.IndexOf(existingFile);
                        if (inx >= 0)
                            localFiles.RemoveAt(inx);
                    }
                }
            }

            // Remove remaining local files
            foreach (TermBaseFile file in localFiles)
            {
                int inx = Files.IndexOf(file);
                if (inx >= 0)
                {
                    Files.RemoveAt(inx);
                }
            }

            return bChanged;
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <created>UPh,29.08.2015</created>
        /// <changed>UPh,29.08.2015</changed>
        // ********************************************************************************
        TermBaseFile FindStoragePath(string path)
        {
            foreach (TermBaseFile file in Files)
            {
                if (string.Compare(file.StoragePath, path, true) == 0)
                    return file;
            }

            return null;
        }

    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
    // --------------------------------------------------------------------------------
    public class TermBaseFile
    {
        public TermBaseFile(string storagePath)
        {
            StoragePath = storagePath;
        }

        public string StoragePath {get; private set;}              // Path of file, starts with %local% if loaded automatically
        public string FilePath
        {
            get
            {
                if (!IsLocal)
                    return StoragePath;

                string dir = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
                string path = dir + StoragePath.Substring(7);
                path = Path.GetFullPath(path);
                return path;
            }
        }
        public bool Active { get; set; }    // True = file is active
        public bool IsLocal
        {
            get { return StoragePath.StartsWith("%local%"); }
        }
    }
}
