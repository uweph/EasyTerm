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
                else if (request.Type == RequestType.Term)
                {
                    HandleTermRequest(request);
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
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        private void HandleTermListRequest(TermBaseRequest request)
        {
            TermListItems items = new TermListItems();
            foreach (TermBase termbase in _TermBases)
            {
                if (_Paused || _shouldStop)
                    return;

                TermListItems items2 = new TermListItems();
                termbase.GetTermList(items2, this);
                items.AddRange(items2);
            }

            items.Sort((a,b) => string.Compare(a.Term, b.Term, true));
            

            _TermbaseQuery.FireTermListResult(request.ID, items);
            
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <created>UPh,30.10.2015</created>
        /// <changed>UPh,30.10.2015</changed>
        // ********************************************************************************
        private void HandleTermRequest(TermBaseRequest request)
        {
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        private void HandleTerminologyRequest(TermBaseRequest request)
        {
        }


        // ********************************************************************************
        /// <summary>
        /// I
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
        Term,          // Return a term with a given id
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

        // Term to find
        internal string Term {get; private set;}

        // If a special term is searched
        internal long TermID {get; private set;}

        // Request ID
        internal int ID {get; set;}

        static int _NewRequestID = 100;

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        static internal TermBaseRequest MakeTermListRequest()
        {
            TermBaseRequest request = new TermBaseRequest();
            Interlocked.Increment(ref _NewRequestID);
            request.ID = _NewRequestID;
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
        static internal TermBaseRequest MakeTermRequest(string term, long termid)
        {
            TermBaseRequest request = new TermBaseRequest();
            request.ID = 0;
            request.Type = RequestType.Term;
            request.Term = term;
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
        static internal TermBaseRequest MakeTerminologyRequest(string term)
        {
            TermBaseRequest request = new TermBaseRequest();
            request.ID = 0;
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
