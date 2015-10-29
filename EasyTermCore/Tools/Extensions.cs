using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EasyTermCore
{
    public static class Extensions
    {
        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ci"></param>
        /// <returns></returns>
        /// <created>UPh,29.10.2015</created>
        /// <changed>UPh,29.10.2015</changed>
        // ********************************************************************************
        public static int PrimaryLCID(this CultureInfo ci)
        {
            return ci.LCID & 0x3ff;
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ci"></param>
        /// <returns></returns>
        /// <created>UPh,29.10.2015</created>
        /// <changed>UPh,29.10.2015</changed>
        // ********************************************************************************
        public static int SubLCID(this CultureInfo ci)
        {
            return ci.LCID >> 10;
        }
    }
}
