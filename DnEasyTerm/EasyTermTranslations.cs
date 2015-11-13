using EasyTermCore;
using Pass.AddIn.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnEasyTerm
{
    public class EasyTermTranslations : Pass.AddIn.Framework.Translations
    {
        EasyTermAddInComponent _AddInComponent = null;
        TermBaseSet _TermBaseSet = null;
        TermBaseQuery _Query = null;

        public EasyTermTranslations()
        {
            
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transData"></param>
        /// <returns></returns>
        /// <created>UPh,07.11.2015</created>
        /// <changed>UPh,07.11.2015</changed>
        // ********************************************************************************
        public override uint GetTranslateData(CPAITranslateData transData)
        {
            transData.Type = TranslationType.Terminology;


            return 0;
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,07.11.2015</created>
        /// <changed>UPh,07.11.2015</changed>
        // ********************************************************************************
        void InitializeTermbase()
        {
            _AddInComponent = AddInInstance as EasyTermAddInComponent;
            if (_AddInComponent == null)
                return;

            _TermBaseSet = _AddInComponent._TermBaseSet;
            _Query = _TermBaseSet.Query;

            _Query.TermListResult += Query_TermListResult;
        }


        int _Lcid1 = -1;
        int _Lcid2 = -1;
        TermListItems _Items = null;


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,07.11.2015</created>
        /// <changed>UPh,07.11.2015</changed>
        // ********************************************************************************
        void Query_TermListResult(object sender, TermListResultArgs e)
        {
            _Items = e.Items;
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="trans"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        /// <created>UPh,07.11.2015</created>
        /// <changed>UPh,07.11.2015</changed>
        // ********************************************************************************
        public override uint GetTerminology(string str, CPAITranslations trans, long cookie)
        {
            if (_TermBaseSet == null)
                InitializeTermbase();

            if (_TermBaseSet == null)
                return 1;

            int lcid1 = -1;
            int lcid2 = -1;
            trans.GetLanguages(ref lcid1, ref lcid2);

            if (lcid1 != _Lcid1)
            {
                _Lcid1 = lcid1;
                _Lcid2 = lcid2;
                _Query.SetLanguagePair(_Lcid1, _Lcid2);
                _Query.RequestTermList();       // Sollte query behalten und damit arbeiten
            }
            else if (lcid2 != _Lcid2)
            {
                _Lcid2 = lcid2;
                _Query.SetLanguagePair(_Lcid1, _Lcid2);
            }



            return 0;
        }

        // ********************************************************************************
        /// <summary>
        /// Called when application exits
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,08.11.2015</created>
        /// <changed>UPh,08.11.2015</changed>
        // ********************************************************************************
        internal void StopRequests()
        {
            _Query.StopRequests();
        }
    }
}
