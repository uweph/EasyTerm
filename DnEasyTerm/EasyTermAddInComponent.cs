using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pass.AddIn.Framework;
using Pass.AddIn.Core;
using EasyTermCore;
using System.IO;

namespace DnEasyTerm
{
    public class EasyTermAddInComponent : Pass.AddIn.Framework.AddIn
    {
        CPAIAddIn _AddIn = null;

        internal TermBaseSet _TermBaseSet;


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="init"></param>
        /// <param name="apptools"></param>
        /// <param name="addin"></param>
        /// <returns></returns>
        /// <created>UPh,07.11.2015</created>
        /// <changed>UPh,07.11.2015</changed>
        // ********************************************************************************
        public override int Initialize(CPAIInit init, CPAIApplicationTools apptools, CPAIAddIn addin)
        {
            _AddIn = addin;

            init.Ident = "EasyTerm";
            init.InfoText = "Provides terminology from tbx,csv and MultiTerm files";
            init.Name = "EasyTerm";
            init.Type = AddinType.Translate | AddinType.Tools;
            init.Style = enmPAIStyle.SetupData;

            if (!init.IsRegistration)
            {
                _TermBaseSet = new TermBaseSet();
                string dataFolder = addin.GetDataFolder();
                _TermBaseSet.SettingsFile = Path.Combine(dataFolder, "settings.xml");
                _TermBaseSet.LoadStoredAndLocal();
            }

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
        public override int Setup()
        {
            if (_TermBaseSet == null)
                return 1;

            _TermBaseSet.EditTermBases();

            return 0;
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,13.11.2015</created>
        /// <changed>UPh,13.11.2015</changed>
        // ********************************************************************************
        public override int Exit()
        {
            if (_TermBaseSet != null &&
                _TermBaseSet.Query != null)
                _TermBaseSet.Query.StopRequests();

            return 0;
        }
    }
}
