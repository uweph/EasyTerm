using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EasyTermCore
{
    class TermBaseDB : TermBase
    {
        Dictionary<CultureInfo, string> _LanguageAttributes;
        string _LangAttribute1;
        string _LangAttribute2;

        OleDbConnection _DataBase = null;

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
                string connectionstring =
            string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Persist Security Info=False;", File.FilePath);

                _DataBase = new OleDbConnection(connectionstring);
                _DataBase.Open();

            }
            catch (Exception)
            {
                _DataBase = null;
            }            

            CollectLanguageAttributes();
            
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
            if (_DataBase == null)
                return;

            _DataBase.Close();
            _DataBase = null;
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="System.Globalization.CultureInfo"></typeparam>
        /// <returns></returns>
        /// <created>UPh,01.11.2015</created>
        /// <changed>UPh,01.11.2015</changed>
        // ********************************************************************************
        internal override List<System.Globalization.CultureInfo> GetLanguages()
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
        /// <param name="lang1"></param>
        /// <param name="lang2"></param>
        /// <returns></returns>
        /// <created>UPh,01.11.2015</created>
        /// <changed>UPh,01.11.2015</changed>
        // ********************************************************************************
        internal override void InitLanguagePair(System.Globalization.CultureInfo lang1, System.Globalization.CultureInfo lang2)
        {
            if (_DataBase == null)
                return;

            if (lang1 == null || lang2 == null)
                return;

            _LanguageAttributes.TryGetValue(lang1, out _LangAttribute1);
            _LanguageAttributes.TryGetValue(lang2, out _LangAttribute2);
            if (_LangAttribute1 == null)
                return;
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="abort"></param>
        /// <returns></returns>
        /// <created>UPh,01.11.2015</created>
        /// <changed>UPh,01.11.2015</changed>
        // ********************************************************************************
        internal override void GetTermList(TermListItems items, IAbortTermQuery abort)
        {
            if (_DataBase == null)
                return;

            if (string.IsNullOrEmpty(_LangAttribute1))
                return;

            try
            {
                string sql = string.Format("SELECT origterm,conceptid FROM {0}", "I_" + _LangAttribute1);

                using (OleDbCommand cmd = new OleDbCommand(sql, _DataBase))
                {
                    //cmd.Parameters.AddWithValue("@langtable", "I_" + _LangAttribute1);

                    using(OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                string term = reader[0].ToString();
                                int termID = reader.GetInt32(1);

                                items.Add(File.ID, term, termID);
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }
                    }
                }

            }
            catch (Exception)
            {
                items.Clear();
            }
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="termID"></param>
        /// <param name="info"></param>
        /// <param name="abort"></param>
        /// <returns></returns>
        /// <created>UPh,01.11.2015</created>
        /// <changed>UPh,01.11.2015</changed>
        // ********************************************************************************
        internal override bool GetTermInfo(int termID, out TermInfo info, IAbortTermQuery abort)
        {
            info = null;

            if (_DataBase == null)
                return false;

            if (string.IsNullOrEmpty(_LangAttribute1))
                return false;

            try
            {
                string sql = "SELECT text FROM mtConcepts WHERE conceptid = @id";

                using (OleDbCommand cmd = new OleDbCommand(sql, _DataBase))
                {
                    cmd.Parameters.AddWithValue("@id", termID);

                    string text = cmd.ExecuteScalar() as string;

                    XmlDocument doc = new XmlDocument();
                    doc.XmlResolver = null;
                    doc.LoadXml(text);


                    info = new TermInfo();

                    XmlNode nodeProps = doc.SelectSingleNode("./cG");
                    if (nodeProps != null)
                        ReadProps(nodeProps, ref info._Props);


                    // Language 1
                    string xpath1 = string.Format("/cG/lG/l[@type='{0}']", _LangAttribute1);
                    XmlNode nodeLangset1 = doc.SelectSingleNode(xpath1);

                    if (nodeLangset1 != null)
                    {
                        CultureInfo ci1 = GetCultureInfoFromAttributeName(_LangAttribute1);
                        if (ci1 != null)
                        {
                            TermInfo.LangSet langset1 = info.AddLanguage(ci1);
                            ReadLangset(nodeLangset1.ParentNode, langset1);
                        }
                    }


                    // Language 2
                    string xpath2 = string.Format("/cG/lG/l[@type='{0}']", _LangAttribute2);
                    XmlNode nodeLangset2 = doc.SelectSingleNode(xpath2);

                    if (nodeLangset2 != null)
                    {
                        CultureInfo ci2 = GetCultureInfoFromAttributeName(_LangAttribute2);
                        if (ci2 != null)
                        {
                            TermInfo.LangSet langset2 = info.AddLanguage(ci2);
                            ReadLangset(nodeLangset2.ParentNode, langset2);
                        }
                    }

                }

            }
            catch (Exception)
            {
                return false;
            }

            return true;
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
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,01.11.2015</created>
        /// <changed>UPh,01.11.2015</changed>
        // ********************************************************************************
        void CollectLanguageAttributes()
        {
            if (_DataBase == null)
                return;

            if (_LanguageAttributes == null)
                _LanguageAttributes = new Dictionary<CultureInfo, string>();

            _LanguageAttributes.Clear();


            OleDbCommand cmd = new OleDbCommand("SELECT * FROM mtIndexes", _DataBase);
            //cmd.Parameters.AddWithValue("@1", userName)

            OleDbDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                try
                {
                    string name = reader[0].ToString();
                    string locale = reader[1].ToString();
                    CultureInfo ci = CultureInfo.GetCultureInfo(locale);
                    _LanguageAttributes[ci] = name;
                }
                catch (Exception)
                {
                    continue;
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
        /// <created>UPh,01.11.2015</created>
        /// <changed>UPh,01.11.2015</changed>
        // ********************************************************************************
        private void ReadLangset(XmlNode nodeLangset, TermInfo.LangSet langset)
        {
            // Read description
            ReadProps(nodeLangset, ref langset._Props);


            // Read terms
            foreach (XmlNode nodeTig in nodeLangset.SelectNodes("./tG"))
            {
                XmlNode nodeTerm = nodeTig.SelectSingleNode("./t");
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
        /// <param name="node"></param>
        /// <param name="props"></param>
        /// <returns></returns>
        /// <created>UPh,01.11.2015</created>
        /// <changed>UPh,01.11.2015</changed>
        // ********************************************************************************
        private void ReadProps(XmlNode node, ref TermInfo.Properties props)
        {
            foreach (XmlNode nodeDescrip in node.SelectNodes("./dG/d"))
            {
                XmlAttribute attType = nodeDescrip.Attributes["type"];
                if (attType == null)
                    continue;

                if (props == null)
                    props = new TermInfo.Properties();

                if (string.Compare(attType.InnerText, "definition", true) == 0)
                    props.Definition = nodeDescrip.InnerText;
                else
                    props.AddValue(attType.InnerText, nodeDescrip.InnerText);
            }


        }


    }
}
