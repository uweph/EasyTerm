using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EasyTermCore
{
    // --------------------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
    // --------------------------------------------------------------------------------
    internal class TermBaseQueryWorker : IAbortTermQuery
    {
        private TermBaseQuery _TermbaseQuery;
        private TermBases _TermBases;
        private volatile bool _shouldStop = false;
        private Thread _Thread;
        private AutoResetEvent _DataAvailableEvent;
        private TermIndex _Index;

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="termbaseSet"></param>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        internal TermBaseQueryWorker(TermBaseQuery termbaseQuery, TermBases termbases)
        {
            _TermbaseQuery = termbaseQuery;
            _TermBases = termbases;
            _Index = new TermIndex();
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        internal bool IsStarted
        {
            get
            {
                return _Thread != null;
            }
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        internal void Start()
        {
            try
            {
                Stop();

                _shouldStop = false;
                _DataAvailableEvent = new AutoResetEvent(false);
                _Thread = new Thread(WorkerThread);
                _Thread.IsBackground = true;
                _Thread.Start();

            }
            catch (Exception ex)
            {
                throw new Exception("Cannot start thread.", ex);
            }
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        internal void Stop()
        {
            if (_Thread == null)
                return;

            try
            {
                RequestStop();
                _Thread.Join(5000);

                _Thread = null;
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot stop thread.", ex);
            }
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        internal void RequestStop()
        {
            _shouldStop = true;
            _DataAvailableEvent.Set();
        }

        volatile bool _Paused = false;
        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        internal void PauseRequests()
        {
            lock (_NewRequests)
            {
                _NewRequests.Clear();
                _Paused = true;

                // TODO Make sure, that thread is paused as well
            }
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,15.11.2015</created>
        /// <changed>UPh,15.11.2015</changed>
        // ********************************************************************************
        internal void ResetIndex()
        {
            if (!_Paused)
            {
                Debug.Assert(false);
                return;
            }

            _Index.ClearIndex();
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        internal void ResumeRequests()
        {
            _Paused = false;
        }



        private List<TermBaseRequest> _NewRequests = new List<TermBaseRequest>();

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        internal void PutRequest(TermBaseRequest request)
        {
            if (_Paused)
                return;

            lock (_NewRequests)
            {
                if (_NewRequests.Count > 0 && _NewRequests[0].Type == RequestType.TermList)
                {
                    // Don't throw TermList request away
                    if (_NewRequests.Count > 1)
                        _NewRequests.RemoveAt(1);

                    _NewRequests.Add(request);
                    _DataAvailableEvent.Set();
                    return;
                }

                _NewRequests.Clear();      // Clear all outstanding requests
                _NewRequests.Add(request); 
                _DataAvailableEvent.Set();
            }
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        private TermBaseRequest GetRequest()
        {
            lock (_NewRequests)
            {
                if (_NewRequests.Count == 0)
                    return null;

                TermBaseRequest request = _NewRequests[0];
                _NewRequests.RemoveAt(0);
                return request;
            }
        }   

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        private void WorkerThread()
        {
            while (!_shouldStop)
            {
                _DataAvailableEvent.WaitOne(1000);
                if (_shouldStop)
                    return;

                TermBaseRequest request = GetRequest();
                if (request == null)
                    continue;

                if (request.Type == RequestType.TermList)
                {
                    HandleTermListRequest(request);
                }
                else if (request.Type == RequestType.TermInfo)
                {
                    HandleTermInfoRequest(request);
                }
                else if (request.Type == RequestType.Terminology)
                {
                    HandleTerminologyRequest(request);
                }

                // TODO Bei _PauseRequest die aktuelle Suche abbrechen
            }

        }


        // ********************************************************************************
        /// <summary>
        /// Handle request to get term list items
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        private void HandleTermListRequest(TermBaseRequest request)
        {
            TermListItems items = RetrieveTermList();
            if (items == null)
                return;

            if (_Paused || _shouldStop)
                return;

            items.Sort((a,b) => string.Compare(a.Term, b.Term, true));
            

            _TermbaseQuery.FireTermListResult(request.ID, items);
            
        }

        // ********************************************************************************
        /// <summary>
        /// Handle request for a single term
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <created>UPh,30.10.2015</created>
        /// <changed>UPh,30.10.2015</changed>
        // ********************************************************************************
        private void HandleTermInfoRequest(TermBaseRequest request)
        {
            TermBase tb = _TermBases.FindTermBase(request.TermBaseID);
            if (tb == null)
                return;

            TermInfo info;
            if (!tb.GetTermInfo(request.TermID, out info, this))
                return;

            info.TermID = request.TermID;

            _TermbaseQuery.FireTermInfoResult(request.ID, request.TermBaseID, info);


        }


        // ********************************************************************************
        /// <summary>
        /// Handle request for terminology 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,17.11.2015</changed>
        // ********************************************************************************
        internal void HandleTerminologyRequest(TermBaseRequest request, List<TerminologyResultArgs> result = null)
        {
            bool bSync = (result != null);

            // Build index if necessary
            if (_Index.LCID != _TermbaseQuery.LCID1)
            {
                TermListItems items = RetrieveTermList();
                if (!bSync && _shouldStop)
                    return;

                // Index from items
                _Index.BuildIndex(_TermbaseQuery.LCID1, items);
            }

            
            WordSegments wordSegments = new WordSegments(request.Term);

            int nWords = wordSegments.Count;
            if (nWords == 0)
                return;

            // Loop all 1 to 3 word ranges
            for (int iWord0 = 0; iWord0 < nWords; iWord0++)
            {
                for (int iWord1 = iWord0; iWord1 < nWords && iWord1 < iWord0 + 3; iWord1++)
                {
                    int from = wordSegments.GetWordStart(iWord0);
                    int to   = wordSegments.GetWordEnd(iWord1);

                    foreach (IndexItem match in _Index.Matches(request.Term, from, to - from))
                    {
                        HandleTerminologyMatch(request.ID, match, from, to - from, result);
                    }
                }
            }
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        /// <created>UPh,17.11.2015</created>
        /// <changed>UPh,17.11.2015</changed>
        // ********************************************************************************
        internal void HandleSingleTermRequest(TermBaseRequest request, List<TerminologyResultArgs> result = null)
        {
            bool bSync = (result != null);

            // Build index if necessary
            if (_Index.LCID != _TermbaseQuery.LCID1)
            {
                TermListItems items = RetrieveTermList();
                if (!bSync && _shouldStop)
                    return;

                // Index from items
                _Index.BuildIndex(_TermbaseQuery.LCID1, items);
            }

            foreach (IndexItem match in _Index.Matches(request.Term))
            {
                HandleTerminologyMatch(request.ID, match, 0, request.Term.Length, result);
            }
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="term"></param>
        /// <param name="infos"></param>
        /// <returns></returns>
        /// <created>UPh,17.11.2015</created>
        /// <changed>UPh,17.11.2015</changed>
        // ********************************************************************************
        internal void HandleTermInfosRequest(string term, List<TermInfoResultArgs> result = null)
        {
            bool bSync = (result != null);

            // Build index if necessary
            if (_Index.LCID != _TermbaseQuery.LCID1)
            {
                TermListItems items = RetrieveTermList();
                if (!bSync && _shouldStop)
                    return;

                // Index from items
                _Index.BuildIndex(_TermbaseQuery.LCID1, items);
            }

            foreach (IndexItem match in _Index.Matches(term))
            {
                // Get TermInfo
                TermBase termbase = _TermBases.FindTermBase(match.TermBaseID);
                if (termbase == null)
                    continue;

                TermInfo terminfo = null;
                if (!termbase.GetTermInfo(match.TermID, out terminfo, this))
                    return;

                TermInfoResultArgs item = new TermInfoResultArgs();
                item.RequestID = 0;
                item.TermBaseID = match.TermBaseID;
                item.Info = terminfo;

                result.Add(item);
            }
        }


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        /// <created>UPh,14.11.2015</created>
        /// <changed>UPh,14.11.2015</changed>
        // ********************************************************************************
        private void HandleTerminologyMatch(long requestid, IndexItem match, int from, int len, List<TerminologyResultArgs> result = null)
        {
            // Get TermInfo
            TermBase termbase = _TermBases.FindTermBase(match.TermBaseID);
            if (termbase == null)
                return;

            TermInfo terminfo = null;
            if (!termbase.GetTermInfo(match.TermID, out terminfo, this))
                return;

            if (terminfo.LanguageSets.Count != 2)
                return;

            TermInfo.Term srcterm = null;

            foreach (TermInfo.Term term in terminfo.LanguageSets[0].Terms)
            {
                ulong hash = TermIndex.MakeGlossaryHashCode(term.Text);
                if (match.Hash != hash)
                    continue;

                srcterm = term;
                break;
            }

            if (srcterm == null)
                return;

            string definition = terminfo.Definition;


            foreach (TermInfo.Term term in terminfo.LanguageSets[1].Terms)
            {
                TerminologyResultArgs args = new TerminologyResultArgs();

                args.RequestID = requestid;
                args.FindFrom = from;
                args.FindLen = len;
                args.Term1 = srcterm.Text;
                args.Term2 = term.Text;
                args.Origin = termbase.File.DisplayName;
                args.Description = definition;

                if (result != null)
                {
                    result.Add(args);
                }
                else
                {
                    _TermbaseQuery.FireTerminologyResult(args);
                }
            }


        }

        // ********************************************************************************
        /// <summary>
        /// Loops all term bases and collects terms in current language
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,14.11.2015</created>
        /// <changed>UPh,14.11.2015</changed>
        // ********************************************************************************
        private TermListItems RetrieveTermList()
        {
            TermListItems items = new TermListItems();
            foreach (TermBase termbase in _TermBases)
            {
                if (_Paused || _shouldStop)
                    return null;

                TermListItems items2 = new TermListItems();
                termbase.GetTermList(items2, this);
                items.AddRange(items2);
            }

            return items;
        }


        // ********************************************************************************
        /// <summary>
        /// Used from term base functions to ask if operation should be stopped
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,30.10.2015</created>
        /// <changed>UPh,30.10.2015</changed>
        // ********************************************************************************
        public bool Abort()
        {
            return _Paused || _shouldStop;
        }
    }

    enum RequestType
    {
        TermList,      // Return list of all terms in current source language
        TermInfo,      // Return a term with a given id
        Terminology    // Find matching terms for a given string
    };


    // --------------------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
    // --------------------------------------------------------------------------------
    internal class TermBaseRequest
    {   
        // 
        internal RequestType Type{get; private set;}

        // For TermInfo
        internal int TermBaseID {get; private set;}
        internal int TermID { get; private set; }

        // Term to find
        internal string Term {get; private set;}

        // Request ID
        internal long ID {get; set;}

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        static internal TermBaseRequest MakeTermListRequest(long requestid)
        {
            TermBaseRequest request = new TermBaseRequest();
            request.ID = requestid;
            request.Type = RequestType.TermList;
            return request;
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="term"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <created>UPh,30.10.2015</created>
        /// <changed>UPh,30.10.2015</changed>
        // ********************************************************************************
        static internal TermBaseRequest MakeTermInfoRequest(int termbaseID, int termid, long requestid)
        {
            TermBaseRequest request = new TermBaseRequest();
            request.ID = requestid;
            request.Type = RequestType.TermInfo;
            request.TermBaseID = termbaseID;
            request.TermID = termid;
            return request;
        }


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        static internal TermBaseRequest MakeTerminologyRequest(string term, long requestid)
        {
            TermBaseRequest request = new TermBaseRequest();
            request.ID = requestid;
            request.Type = RequestType.Terminology;
            request.Term = term;
            return request;
        }
    }

    internal interface IAbortTermQuery
    {
        bool Abort();
    }
}
