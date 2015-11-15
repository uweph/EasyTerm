using System;
using System.Collections.Generic;
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
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        private void HandleTerminologyRequest(TermBaseRequest request)
        {
            // Build index if necessary
            if (_Index.Language != _TermbaseQuery.Language1.Name)
            {
                TermListItems items = RetrieveTermList();
                if (_shouldStop)
                    return;

                // Index from items
                _Index.BuildIndex(_TermbaseQuery.Language1.Name, items);
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
                        HandleTerminologyMatch(request.ID, match, from, to - from);
                        //_TermbaseQuery.FireTerminologyResult(request.ID, 100, info);
                    }
                }
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
        private void HandleTerminologyMatch(long requestid, IndexItem match, int from, int len)
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


            // Compare source term
            // TODO ...

            foreach (TermInfo.Term term in terminfo.LanguageSets[1].Terms)
            {
                _TermbaseQuery.FireTerminologyResult(requestid, 100, from, len, "", term.Text, termbase.File.DisplayName, "");
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
