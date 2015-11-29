using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pass.AddIn.Core;

namespace DnEasyTerm
{
    public class EasyTermTokenCheck : Pass.AddIn.Framework.TokenCheck
    {
        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="checker"></param>
        /// <returns></returns>
        /// <created>UPh,29.11.2015</created>
        /// <changed>UPh,29.11.2015</changed>
        // ********************************************************************************
        public override uint BeginCheckTokens(Pass.AddIn.Core.CPAIResource resource, Pass.AddIn.Core.CPAITokenCheck checker)
        {
            return 0;
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="resource"></param>
        /// <param name="checker"></param>
        /// <returns></returns>
        /// <created>UPh,29.11.2015</created>
        /// <changed>UPh,29.11.2015</changed>
        // ********************************************************************************
        public override uint CheckToken(Pass.AddIn.Core.CPAIToken token, Pass.AddIn.Core.CPAIResource resource, Pass.AddIn.Core.CPAITokenCheck checker)
        {
            return 0;
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="checker"></param>
        /// <returns></returns>
        /// <created>UPh,29.11.2015</created>
        /// <changed>UPh,29.11.2015</changed>
        // ********************************************************************************
        public override uint EndCheckTokens(Pass.AddIn.Core.CPAIResource resource, Pass.AddIn.Core.CPAITokenCheck checker)
        {
            return 0;
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="check"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <created>UPh,29.11.2015</created>
        /// <changed>UPh,29.11.2015</changed>
        // ********************************************************************************
        public override uint GetCustomCheckOption(int check, ref string text)
        {
            switch (check)
            {
                case PslConstant.TCO_CUSTOM_0:
                    text = "Term ... not used in translation";
                    return 0;

                case PslConstant.TCO_CUSTOM_1:
                text = "Forbidden term used in translation";
                    return 0;
            }

            return 1;
        }
    }
}
