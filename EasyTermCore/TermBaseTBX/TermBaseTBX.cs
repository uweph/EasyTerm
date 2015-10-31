using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EasyTermCore
{
    class TermBaseTBX : TermBase
    {
        XmlDocument _Doc;
        XmlNamespaceManager _NamespaceManager;

        internal override void OnOpenFile()
        {
            try
            {
                _Doc = new XmlDocument();
                _Doc.XmlResolver = null;
                _Doc.Load(File.FilePath);

                _NamespaceManager = new XmlNamespaceManager(_Doc.NameTable);
                _NamespaceManager.AddNamespace("xml", "http://www.w3.org/XML/1998/namespace");

                CollectLanguageAttributes();

            }
            catch (Exception)
            {
                _Doc = null;
            }
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="languages"></param>
        /// <returns></returns>
        /// <created>UPh,30.10.2015</created>
        /// <changed>UPh,30.10.2015</changed>
        // ********************************************************************************
        internal override List<CultureInfo> GetLanguages()
        {
            if (_LanguageAttributes == null)
                return null;

            List<CultureInfo> cis = new List<CultureInfo>();
            cis.AddRange(_LanguageAttributes.Keys);

            return cis;

        }


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,30.10.2015</created>
        /// <changed>UPh,30.10.2015</changed>
        // ********************************************************************************
        private void CollectLanguageAttributes()
        {
            if (_Doc == null)
                return;

            if (_LanguageAttributes == null)
                _LanguageAttributes = new Dictionary<CultureInfo,string>();

            _LanguageAttributes.Clear();

            // Read up to 20 languageSets and find which langauges attributes are used
            string xpath = "/martif/text/body/termEntry/langSet";
            foreach (XmlNode langset in _Doc.SelectNodes(xpath, _NamespaceManager))
            {
                XmlAttribute att = langset.Attributes["xml:lang"];


                try
                {
                    CultureInfo ci = CultureInfo.GetCultureInfo(att.Value);

                    _LanguageAttributes[ci] = att.Value;


                }
                catch (Exception)
                {
                }
            }
        }

        List<XmlNode> _Langset1;
        Dictionary<CultureInfo, string> _LanguageAttributes;
        string _LangAttribute1;
        string _LangAttribute2;

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lang1"></param>
        /// <param name="lang2"></param>
        /// <returns></returns>
        /// <created>UPh,29.10.2015</created>
        /// <changed>UPh,29.10.2015</changed>
        // ********************************************************************************
        internal override void InitLanguagePair(System.Globalization.CultureInfo lang1, System.Globalization.CultureInfo lang2)
        {
            if (_Doc == null)
                return;


            if (_Langset1 == null)
                _Langset1 = new List<XmlNode>();
            _Langset1.Clear();

            if (lang1 == null || lang2 == null)
                return;

            _LanguageAttributes.TryGetValue(lang1, out _LangAttribute1);
            _LanguageAttributes.TryGetValue(lang2, out _LangAttribute2);
            if (_LangAttribute1 == null)
                return;


            string xpath = string.Format("/martif/text/body/termEntry/langSet[@xml:lang='{0}']", _LangAttribute1);

            foreach (XmlNode langset in _Doc.SelectNodes(xpath, _NamespaceManager))
            {
                _Langset1.Add(langset);
            }
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        /// <created>UPh,29.10.2015</created>
        /// <changed>UPh,29.10.2015</changed>
        // ********************************************************************************
        internal override void GetTermList(TermListItems items, IAbortTermQuery abort)
        {
            if (_Doc == null)
                return;

            foreach (XmlNode langSet in _Langset1)
            {
                XmlNode node = langSet.SelectSingleNode(".//term");
                if (node == null)
                    node = langSet.SelectSingleNode(".//ntig/termGrp/term");

                // Index of node is TermListItem ID
                if (node != null)
                    items.Add(File.ID, node.InnerText, items.Count);
            }
            
            
        }
    }
}
