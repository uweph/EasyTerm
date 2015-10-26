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
    internal class TermBaseQueryWorker
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
                else if (request.Type == RequestType.TermList)
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
                TermListItems items2 = new TermListItems();
                termbase.GetTermList(items2);
                items.AddRange(items2);
            }

            items.Sort((a,b) => string.Compare(a.Term, b.Term, true));
            

            _TermbaseQuery.FireTermListResult(request.ID, items);
            
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

    }

    enum RequestType
    {
        TermList,
        Terminology
    };


    // --------------------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
    // --------------------------------------------------------------------------------
    internal class TermBaseRequest
    {   
        internal RequestType Type{get; private set;}
        internal string Term {get; private set;}
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
}
