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
        // Maps LCIDs to the language names used in the DB
        Dictionary<int, string> _LanguageAttributes;
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
                throw;
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
        /// <param name="lang1"></param>
        /// <param name="lang2"></param>
        /// <returns></returns>
        /// <created>UPh,01.11.2015</created>
        /// <changed>UPh,01.11.2015</changed>
        // ********************************************************************************
        internal override void InitLanguagePair(int lcid1, int lcid2)
        {
            if (_DataBase == null)
                return;

            if (lcid1 < 0 || lcid2 < 0)
                return;

            _LangAttribute1 = FindLanguage(lcid1);
            _LangAttribute2 = FindLanguage(lcid2);

            if (_LangAttribute1 == null)
                return;
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
                        int lcid1 = GetLCIDFromAttributeName(_LangAttribute1);
                        if (lcid1 >= 0)
                        {
                            TermInfo.LangSet langset1 = info.AddLanguage(lcid1);
                            ReadLangset(nodeLangset1.ParentNode, langset1);
                        }
                    }


                    // Language 2
                    string xpath2 = string.Format("/cG/lG/l[@type='{0}']", _LangAttribute2);
                    XmlNode nodeLangset2 = doc.SelectSingleNode(xpath2);

                    if (nodeLangset2 != null)
                    {
                        int lcid2 = GetLCIDFromAttributeName(_LangAttribute2);
                        if (lcid2 >= 0)
                        {
                            TermInfo.LangSet langset2 = info.AddLanguage(lcid2);
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
                _LanguageAttributes = new Dictionary<int, string>();

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
                    _LanguageAttributes[ci.LCID] = name;
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
