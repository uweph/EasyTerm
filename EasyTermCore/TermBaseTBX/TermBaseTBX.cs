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

        CultureInfo GetCultureInfoFromAttributeName(string name)
        {
            foreach (var pair in _LanguageAttributes)
            {
                if (pair.Value == name)
                    return pair.Key;
            }

            return null;

        }

        // ********************************************************************************
        /// <summary>
        /// SDL TBX does not used correct ISO languages, so we need to search all available cultures to find the correct one
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <created>UPh,31.10.2015</created>
        /// <changed>UPh,31.10.2015</changed>
        // ********************************************************************************
        CultureInfo GetCultureInfoFromName(string name)
        {
            try
            {
                CultureInfo ci = CultureInfo.GetCultureInfo(name);
                return ci;

            }
            catch (Exception)
            {
            }

            foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                if (string.Compare(ci.Name, name, true) == 0)
                    return ci;

                if (string.Compare(ci.EnglishName, name, true) == 0)
                    return ci;

                if (string.Compare(ci.DisplayName, name, true) == 0)
                    return ci;
            }

            return null;
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
                    CultureInfo ci = GetCultureInfoFromName(att.Value);
                    if (ci == null)
                        continue;

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

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="termID"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        /// <created>UPh,31.10.2015</created>
        /// <changed>UPh,31.10.2015</changed>
        // ********************************************************************************
        internal override bool GetTermInfo(int termID, out TermInfo info, IAbortTermQuery abort)
        {
            info = null;

            if (termID < 0 || termID >= _Langset1.Count)
                return false;

            try 
	        {	        
		        info = new TermInfo();

                XmlNode nodeLangset1 = _Langset1[termID];

                // Language 1
                CultureInfo ci1 = GetCultureInfoFromAttributeName(_LangAttribute1);
                if (ci1 == null)
                    return false;

                TermInfo.LangSet langset1 = info.AddLanguage(ci1);

                foreach (XmlNode node in nodeLangset1.SelectNodes(".//term"))
                {
                    langset1.AddTerm(node.InnerText);  
                }


                // Language 1
                CultureInfo ci2 = GetCultureInfoFromAttributeName(_LangAttribute2);
                if (ci2 == null)
                    return true;

                TermInfo.LangSet langset2 = info.AddLanguage(ci2);

                string xpath = string.Format("./langSet[@xml:lang='{0}']", _LangAttribute2);
                XmlNode nodeLangset2 = nodeLangset1.ParentNode.SelectSingleNode(xpath, _NamespaceManager);

                if (nodeLangset2 != null)
                {
                    foreach (XmlNode node in nodeLangset2.SelectNodes(".//term"))
                    {
                        langset2.AddTerm(node.InnerText);
                    }
                }

                return true;
            }
	        catch (Exception ex)
	        {
                info = new TermInfo();
                info.Description = ex.Message;
                return true;
	        }
        }
    }
}
