using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnEasyTerm
{
    class EasyTermTools : Pass.AddIn.Framework.Tools
    {


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nIndex"></param>
        /// <param name="apptools"></param>
        /// <returns></returns>
        /// <created>UPh,29.11.2015</created>
        /// <changed>UPh,29.11.2015</changed>
        // ********************************************************************************
        public override uint ExecuteTool(int nIndex, Pass.AddIn.Core.CPAIApplicationTools apptools)
        {
            return 0;
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nIndex"></param>
        /// <param name="toolInfo"></param>
        /// <returns></returns>
        /// <created>UPh,29.11.2015</created>
        /// <changed>UPh,29.11.2015</changed>
        // ********************************************************************************
        public override uint GetToolInfo(int nIndex, ref Pass.AddIn.Core.CPAIToolInfo toolInfo)
        {
            return 1;
        }
    }
}
