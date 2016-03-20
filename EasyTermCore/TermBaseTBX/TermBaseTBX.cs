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

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,01.11.2015</created>
        /// <changed>UPh,01.11.2015</changed>
        // ********************************************************************************
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
                throw;
            }
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,01.11.2015</created>
        /// <changed>UPh,01.11.2015</changed>
        // ********************************************************************************
        internal override void OnCloseFile()
        {
            if (_Doc == null)
                return;

            _Doc = null;
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
        internal override List<int> GetLanguages()
        {
            if (_LanguageAttributes == null)
                return null;

            List<int> cis = new List<int>();
            cis.AddRange(_LanguageAttributes.Keys);

            return cis;

        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <created>UPh,01.11.2015</created>
        /// <changed>UPh,01.11.2015</changed>
        // ********************************************************************************
        int GetLCIDFromAttributeName(string name)
        {
            foreach (var pair in _LanguageAttributes)
            {
                if (pair.Value == name)
                    return pair.Key;
            }

            return -1;

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
        int GetLCIDFromName(string name)
        {
            try
            {
                CultureInfo ci = CultureInfo.GetCultureInfo(name);
                return ci.LCID;

            }
            catch (Exception)
            {
            }

            foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                if (string.Compare(ci.Name, name, true) == 0)
                    return ci.LCID;

                if (string.Compare(ci.EnglishName, name, true) == 0)
                    return ci.LCID;

                if (string.Compare(ci.DisplayName, name, true) == 0)
                    return ci.LCID;
            }

            return -1;
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
                _LanguageAttributes = new Dictionary<int,string>();

            _LanguageAttributes.Clear();

            // Read up to 20 languageSets and find which langauges attributes are used
            string xpath = "/martif/text/body/termEntry/langSet";
            foreach (XmlNode langset in _Doc.SelectNodes(xpath, _NamespaceManager))
            {
                XmlAttribute att = langset.Attributes["xml:lang"];

                try
                {
                    int lcid = GetLCIDFromName(att.Value);
                    if (lcid < 0)
                        continue;

                    _LanguageAttributes[lcid] = att.Value;
                }
                catch (Exception)
                {
                }
            }
        }

        List<XmlNode> _Langset1;

        // Maps LCIDs to language names used in TBX file (SDL does not use ISO locales)
        Dictionary<int, string> _LanguageAttributes;

        // Currently used language names
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
        internal override void InitLanguagePair(int lcid1, int lcid2)
        {
            if (_Doc == null)
                return;

            if (_Langset1 == null)
                _Langset1 = new List<XmlNode>();

            if (lcid1 < 0 || lcid2 < 0)
                return;

            string langAttribute1 = FindLanguage(lcid1);
            string langAttribute2 = FindLanguage(lcid2);

            if (langAttribute1 == null)
            {
                _Langset1.Clear();
                _LangAttribute1 = null;
                _LangAttribute2 = null;
                return;
            }

            if (langAttribute1 != _LangAttribute1)
            {
                _LangAttribute1 = langAttribute1;
                string xpath = string.Format("/martif/text/body/termEntry/langSet[@xml:lang='{0}']", _LangAttribute1);

                _Langset1.Clear();
                foreach (XmlNode langset in _Doc.SelectNodes(xpath, _NamespaceManager))
                {
                    _Langset1.Add(langset);
                }
            }

            if (langAttribute2 != _LangAttribute2)
            {
                _LangAttribute2 = langAttribute2;
            }
        }

        // ********************************************************************************
        /// <summary>
        /// Check if both languages are valid for this term base
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,18.11.2015</created>
        /// <changed>UPh,18.11.2015</changed>
        // ********************************************************************************
        internal override bool HasLanguagePair()
        {
            return _LangAttribute1 != null && _LangAttribute2 != null;
        }


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lcid"></param>
        /// <returns></returns>
        /// <created>UPh,15.11.2015</created>
        /// <changed>UPh,15.11.2015</changed>
        // ********************************************************************************
        string FindLanguage(int lcid)
        {
            string bestMatch = null;
            int bestRate = 0;

            foreach (var att in _LanguageAttributes)
            {
                int rate = Tools.GetLanguageMatch(att.Key, lcid);
                if (rate > bestRate)
                {
                    bestMatch = att.Value;
                }
            }

            return bestMatch;
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
        internal override void GetTermList(TermListItems items, IAbortTermQuery abort, bool bTargetLanguage)
        {
            if (_Doc == null)
                return;

            if (bTargetLanguage)
                return; // TODO

            for (int iLangset = 0; iLangset < _Langset1.Count; iLangset++)
            {
                XmlNode langSet = _Langset1[iLangset];
                foreach (XmlNode node in langSet.SelectNodes(".//term"))
                {
                    items.Add(File.ID, node.InnerText, iLangset);
                }

                //XmlNode node = langSet.SelectSingleNode(".//term");
                //if (node == null)
                //    node = langSet.SelectSingleNode(".//ntig/termGrp/term");

                //if (node != null)
                //    items.Add(File.ID, node.InnerText, items.Count);
            }
        }


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="props"></param>
        /// <returns></returns>
        /// <created>UPh,31.10.2015</created>
        /// <changed>UPh,31.10.2015</changed>
        // ********************************************************************************
        private void ReadProps(XmlNode node, ref TermInfo.Properties props)
        {
            foreach (XmlNode nodeDescrip in node.SelectNodes("./descripGrp/descrip"))
            {
                XmlAttribute attType = nodeDescrip.Attributes["type"];
                if (attType == null)
                    continue;

                if (props == null)
                    props = new TermInfo.Properties();

                if (!props.TrySetDefinition(attType.InnerText, nodeDescrip.InnerText) &&
                    !props.TrySetStatus(attType.InnerText, nodeDescrip.InnerText))
                {
                    props.AddValue(attType.InnerText, nodeDescrip.InnerText);
                }
            }

            foreach (XmlNode nodeDescrip in node.SelectNodes("./descrip"))
            {
                XmlAttribute attType = nodeDescrip.Attributes["type"];
                if (attType == null)
                    continue;

                if (props == null)
                    props = new TermInfo.Properties();

                if (!props.TrySetDefinition(attType.InnerText, nodeDescrip.InnerText) &&
                    !props.TrySetStatus(attType.InnerText, nodeDescrip.InnerText))
                {
                    props.AddValue(attType.InnerText, nodeDescrip.InnerText);
                }
            }

        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeLangset"></param>
        /// <param name="langset"></param>
        /// <returns></returns>
        /// <created>UPh,31.10.2015</created>
        /// <changed>UPh,31.10.2015</changed>
        // ********************************************************************************
        private void ReadLangset(XmlNode nodeLangset, TermInfo.LangSet langset)
        {
            // Read description
            ReadProps(nodeLangset, ref langset._Props);


            // Read terms
            foreach (XmlNode nodeTig in nodeLangset.SelectNodes("./tig"))
            {
                XmlNode nodeTerm = nodeTig.SelectSingleNode("./term");
                if (nodeTerm == null)
                    return;

                TermInfo.Term term = langset.AddTerm(nodeTerm.InnerText);

                ReadProps(nodeTig, ref term._Props);
            }

            // Read terms
            foreach (XmlNode nodeTig in nodeLangset.SelectNodes("./ntig"))
            {
                XmlNode nodeTerm = nodeTig.SelectSingleNode("./termGrp/term");
                if (nodeTerm == null)
                    return;

                TermInfo.Term term = langset.AddTerm(nodeTerm.InnerText);

                ReadProps(nodeTig, ref term._Props);
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
                info.TermID = termID;

                XmlNode nodeLangset1 = _Langset1[termID];

                // Entry
                XmlNode nodeEntry = nodeLangset1.ParentNode;
                ReadProps(nodeEntry, ref info._Props);

                // Language 1
                int lcid1 = GetLCIDFromAttributeName(_LangAttribute1);
                if (lcid1 < 0)
                    return false;

                TermInfo.LangSet langset1 = info.AddLanguage(lcid1);
                ReadLangset(nodeLangset1, langset1);


                // Language 2
                int lcid2 = GetLCIDFromAttributeName(_LangAttribute2);
                if (lcid2 < 0)
                    return true;


                string xpath = string.Format("./langSet[@xml:lang='{0}']", _LangAttribute2);
                XmlNode nodeLangset2 = nodeLangset1.ParentNode.SelectSingleNode(xpath, _NamespaceManager);

                if (nodeLangset2 != null)
                {
                    TermInfo.LangSet langset2 = info.AddLanguage(lcid2);
                    ReadLangset(nodeLangset2, langset2);
                }


                return true;
            }
	        catch (Exception ex)
	        {
                info = new TermInfo();
                info._Props = new TermInfo.Properties();
                info._Props.Definition = ex.Message;
                return true;
	        }
        }
    }
}
