using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyTermCore
{
    class TermBaseCSV : TermBase
    {
        FileStream _Stream;

        // List of languages per column
        List<int> _Languages;
        int _LangIndex1 = -1;
        int _LangIndex2 = -1;

        List<Tuple<string, string>>_Terms = new List<Tuple<string,string>>();


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        internal override void OnOpenFile()
        {
            try
            {
                _Stream = new FileStream(File.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);


                using (StreamReader reader = new StreamReader(_Stream, Encoding.Default, true, 1024, true))
                {
                    // Read first line to get languages
                    string line = reader.ReadLine();
                    ParseLanguages(line);
                }


            }
            catch (Exception)
            {
                _Stream = null;
                _Languages = null;
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
            if (_Stream == null)
                return;

            _Stream.Dispose();
            _Stream = null;
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
            return _Languages;
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lang1"></param>
        /// <param name="lang2"></param>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        internal override void InitLanguagePair(int lcid1, int lcid2)
        {
            int index1 = FindLanguage(lcid1);
            int index2 = FindLanguage(lcid2);

            if (index1 < 0 || index2 < 0)
            {
                // Reset...
                return;
            }

            if (_LangIndex1 != index1 ||
                _LangIndex2 != index2)
            {
                _LangIndex1 = index1;
                _LangIndex2 = index2;
                ParseLines();
            }
        }


        // ********************************************************************************
        /// <summary>
        /// Determine column count and language per column
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        private void ParseLanguages(string line)
        {
            _Languages = new List<int>();
            string [] fields = line.Split('\t');

            foreach (string field in fields)
            {
                try
                {
                    CultureInfo info  = CultureInfo.GetCultureInfo(field);
                    _Languages.Add(info.LCID);
                }
                catch (Exception)
                {
                    _Languages.Add(-1);
                }

            }
        }

        static int PRIMARYLANGID(int lgid)
        {
            return lgid & 0x3ff;
        }


        static int SUBLANGID(int lgid)
        {
            return lgid >> 10;
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        private int FindLanguage(int lcid)
        {
            if (lcid < 0)
                return -1;

            int iBest = -1;
            int iRate = 0;


            for (int i = 0; i < _Languages.Count; i++)
            {
                int iRate2 = Tools.GetLanguageMatch(lcid, _Languages[i]);

                if (iRate2 > iRate)
                {
                    iBest = i;
                    iRate = iRate2;
                }

            }

            return iBest;
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        void ParseLines()
        {
            _Terms.Clear();

            _Stream.Seek(0, SeekOrigin.Begin);

            using (StreamReader reader = new StreamReader(_Stream, Encoding.Default, true, 1024, true))
            {
                for(int n = 0; ;n++)
                {
                    long pos = _Stream.Position;

                    string line = reader.ReadLine();
                    if (line == null)
                        break;

                    if (n == 0)
                        continue; // First line contains language names

                    string [] fields = line.Split('\t');
                    if (_LangIndex1 >= fields.Length ||
                        _LangIndex2 >= fields.Length)
                        continue;

                    var tuple = new Tuple<string, string>(fields[_LangIndex1], fields[_LangIndex2]);
                    _Terms.Add(tuple);
                }   
            }

        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="bAbort"></param>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        internal override void GetTermList(TermListItems items, IAbortTermQuery abort)
        {
            if (_Stream == null)
                return;      
                
            for (int i = 0; i < _Terms.Count; i++)
            {
                var tuple = _Terms[i];

                TermListItem item = new TermListItem();
                items.Add(File.ID, tuple.Item1, i);
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

            if (_LangIndex1 < 0 || _LangIndex2 < 0)
                return false;

            if (termID < 0 || termID >= _Terms.Count)
                return false;

            info = new TermInfo();

            TermInfo.LangSet langset1 = info.AddLanguage(_Languages[_LangIndex1]);
            if (langset1 == null)
                return false;

            langset1.AddTerm(_Terms[termID].Item1);

            TermInfo.LangSet langset2 = info.AddLanguage(_Languages[_LangIndex2]);
            if (langset2 == null)
                return false;

            langset2.AddTerm(_Terms[termID].Item2);

            return true;
        }
    }
}
