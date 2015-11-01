using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PassLib;

namespace EasyTermCore
{
    public class EasyTermCoreSettings
    {
        static string _ProfilePath;
        static public string ProfilePath
        {
            get {return _ProfilePath;}
            set
            {
                _ProfilePath = value;

                PlProfile.SetProfileName(_ProfilePath, PlProfile.ProfileType.IniFile);
            }
        }
    }
}
