using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyTermCore
{
    // --------------------------------------------------------------------------------
    /// <summary>
    /// Access to all term base queries
    /// </summary>
    // --------------------------------------------------------------------------------
    public class TermBaseQuery : IDisposable
    {
        private TermBaseSet _TermbaseSet;
        private TermBaseQueryWorker _Worker;

        internal TermBaseQuery(TermBaseSet termbaseSet)
        {
            _TermbaseSet = termbaseSet;
            _Worker = new TermBaseQueryWorker(this,_TermbaseSet.TermBases);
        }

        internal CultureInfo Language1 {get; set;}
        internal CultureInfo Language2 { get; set; }



#region Public query functions

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lang1"></param>
        /// <param name="lang2"></param>
        /// <returns></returns>
        /// <created>UPh,24.10.2015</created>
        /// <changed>UPh,24.10.2015</changed>
        // ********************************************************************************
        public void SetLanguagePair(CultureInfo lang1, CultureInfo lang2)
        {
            Language1 = lang1;
            Language2 = lang2;


            // Inform all termbases
            foreach (TermBase termbase in _TermbaseSet.TermBases)
            {
                termbase.InitLanguagePair(Language1, Language2);
            }
        
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lcid1"></param>
        /// <param name="lcid2"></param>
        /// <returns></returns>
        /// <created>UPh,07.11.2015</created>
        /// <changed>UPh,07.11.2015</changed>
        // ********************************************************************************
        public void SetLanguagePair(int lcid1, int lcid2)
        {
            try
            {
                SetLanguagePair(CultureInfo.GetCultureInfo(lcid1), CultureInfo.GetCultureInfo(lcid2));
            }
            catch (Exception)
            {
                
                
            }
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
        public bool GetLanguagePair(out CultureInfo lang1, out CultureInfo lang2)
        {
            lang1 = Language1;
            lang2 = Language2;

            return lang1 != null && lang2 != null;
        }


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="CultureInfo"></typeparam>
        /// <returns></returns>
        /// <created>UPh,30.10.2015</created>
        /// <changed>UPh,30.10.2015</changed>
        // ********************************************************************************
        public List<CultureInfo> GetLanguages()
        {
            List<CultureInfo> cis = new List<CultureInfo>();

            foreach (TermBase termbase in _TermbaseSet.TermBases)
            {
                List<CultureInfo> cis2 = termbase.GetLanguages();
                if (cis2 == null)
                    continue;

                foreach (CultureInfo ci2 in cis2)
                {
                    if (cis.IndexOf(ci2) < 0)
                        cis.Add(ci2);
                }

            }

            return cis;
        }


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        public void RequestTermList(long requestid)
        {
            if (!_Worker.IsStarted)
                _Worker.Start();

            _Worker.PutRequest(TermBaseRequest.MakeTermListRequest(requestid));
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <created>UPh,31.10.2015</created>
        /// <changed>UPh,31.10.2015</changed>
        // ********************************************************************************
        public void RequestTermInfo(TermListItem item, long requestid)
        {
            if (!_Worker.IsStarted)
                _Worker.Start();

            _Worker.PutRequest(TermBaseRequest.MakeTermInfoRequest(item.TermBaseID, item.TermID, requestid));
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        public void RequestTerminology(string text, long requestid)
        {
            if (!_Worker.IsStarted)
                _Worker.Start();

            _Worker.PutRequest(TermBaseRequest.MakeTerminologyRequest(text, requestid));
        }


        public delegate void TermListResultHandler(object sender, TermListResultArgs e);
        public event TermListResultHandler TermListResult;

        public delegate void TermInfoResultHandler(object sender, TermInfoResultArgs e);
        public event TermInfoResultHandler TermInfoResult;

        public delegate void TerminologyResultHandler(object sender, TerminologyResultArgs e);
        public event TerminologyResultHandler TerminologyResult;

        // ********************************************************************************
        /// <summary>
        /// From worker
        /// </summary>
        /// <param name="requestid"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        /// <created>UPh,31.10.2015</created>
        /// <changed>UPh,31.10.2015</changed>
        // ********************************************************************************
        internal void FireTermListResult(long requestid, TermListItems items)
        {
            if (TermListResult == null)
                return;

            TermListResultArgs args = new TermListResultArgs();
            args.RequestID = requestid;
            args.Items = items;

            TermListResult(this, args);
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestid"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        /// <created>UPh,31.10.2015</created>
        /// <changed>UPh,31.10.2015</changed>
        // ********************************************************************************
        internal void FireTermInfoResult(long requestid, int termbaseID, TermInfo info)
        {
            if (TermListResult == null)
                return;

            TermInfoResultArgs args = new TermInfoResultArgs();
            args.RequestID = requestid;
            args.TermBaseID = termbaseID;
            args.Info = info;

            TermInfoResult(this, args);
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestid"></param>
        /// <param name="rate"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        /// <created>UPh,14.11.2015</created>
        /// <changed>UPh,14.11.2015</changed>
        // ********************************************************************************
        internal void FireTerminologyResult(long requestid, int rate, int from, int len, string term1, string term2, string origin, string description)
        {
            if (TerminologyResult == null)
                return;

            TerminologyResultArgs args = new TerminologyResultArgs();

            args.RequestID = requestid;
            args.FindFrom = from;
            args.FindLen = len;
            args.Term1 = term1;
            args.Term2 = term2;
            args.Origin = origin;
            args.Description = description;

            TerminologyResult(this, args);
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,08.11.2015</created>
        /// <changed>UPh,08.11.2015</changed>
        // ********************************************************************************
        public void StopRequests()
        {
            _Worker.Stop();
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        public void PauseRequests()
        {
            _Worker.PauseRequests();
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        public void ResumeRequests()
        {
            _Worker.ResumeRequests();
        }
        
        
#endregion



        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,24.10.2015</created>
        /// <changed>UPh,24.10.2015</changed>
        // ********************************************************************************
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        bool _bDisposed = false;
        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        protected virtual void Dispose(bool disposing)
        {
            if (_bDisposed)
                return;

            if (disposing)
            {
                _Worker.Stop();

                
            }

            _bDisposed = true;

        }
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
    // --------------------------------------------------------------------------------
    public class TermListItem
    {
        public int TermBaseID {get; internal set;}
        public string Term {get; internal set;}
        public int TermID {get; internal set;}

        internal TermListItem () {}
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Result from TermList query
    /// </summary>
    // --------------------------------------------------------------------------------
    public class TermListItems : List<TermListItem>
    {
        public void Add(int termbaseID, string term, int termID)
        {
            TermListItem item = new TermListItem();
            item.TermBaseID = termbaseID;
            item.Term = term;
            item.TermID = termID;
            Add(item);
        }
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Result from TermInfo Query
    /// </summary>
    // --------------------------------------------------------------------------------
    public class TermInfo
    {
        internal TermInfo()
        {
            LanguageSets = new List<LangSet>();
            TermID = null;
        }

        public int? TermID {get; internal set;}


        internal Properties _Props = null;
        public Properties Props { get { return _Props; } }


        // Add new LangSet
        internal LangSet AddLanguage(CultureInfo language)
        {
            LangSet langset = new LangSet();
            langset.Language = language;
            LanguageSets.Add(langset);

            return langset;
        }

        // --------------------------------------------------------------------------------
        /// <summary>
        /// Additional properties for TermInfo,LangSet or Term
        /// </summary>
        // --------------------------------------------------------------------------------
        public class Properties
        {
            public string Definition {get; internal set;}
            public List<KeyValuePair<string,string>> Values;

            // Adds new key/value 
            internal void AddValue(string key, string value)
            {
                if (Values == null)
                    Values = new List<KeyValuePair<string,string>>();
                Values.Add(new KeyValuePair<string,string>(key, value));
            }

        }

        // --------------------------------------------------------------------------------
        /// <summary>
        /// A language set
        /// </summary>
        // --------------------------------------------------------------------------------
        public class LangSet
        {
            internal LangSet()
            {
                Terms = new List<Term>();
            }


            // Adds new term
            internal Term AddTerm(string text)
            {
                Term term = new Term();
                term.Text = text;

                Terms.Add(term);

                return term;
            }


            public CultureInfo Language {get; internal set;}
            internal Properties _Props = null;
            public Properties Props{ get {return _Props;}}
            public List<Term> Terms { get; internal set; }
        }   

        // --------------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        // --------------------------------------------------------------------------------
        public class Term
        {
            public string Text { get; internal set; }
            internal Properties _Props = null;
            public Properties Props { get { return _Props; } }
        }

        public List<LangSet> LanguageSets {get; internal set;}
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
    // --------------------------------------------------------------------------------
    public class TermListResultArgs : EventArgs
    {
        public long RequestID {get; set;}
        public TermListItems Items {get; internal set;}
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
    // --------------------------------------------------------------------------------
    public class TermInfoResultArgs : EventArgs
    {
        public long RequestID { get; set; }
        public int TermBaseID {get; set;}
        public TermInfo Info {get; internal set;}

    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
    // --------------------------------------------------------------------------------
    public class TerminologyResultArgs : EventArgs
    {
        public long RequestID { get; set; }
        public int TermBaseID { get; set; }
        public int FindFrom {get; set;} // Position of term in request string
        public int FindLen { get; set; } // Length of term in request string
        public string Term1 { get; set; } // Source term
        public string Term2 { get; set; } // Target term
        public string Origin { get; set; } // Origin of term (which Termbase)
        public string Description { get; set; } // Description of term
    }
}
