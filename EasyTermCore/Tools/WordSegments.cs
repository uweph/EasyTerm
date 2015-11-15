using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
Es gibt auch ein API dafür:
https://code.msdn.microsoft.com/windowsapps/Text-Segmentation-API-be73de71/sourcecode?fileId=86741&pathId=1282315147
*/
 
namespace EasyTermCore
{
    internal class WordSegments
    {
        List<Tuple<int, int>> _Ranges = null;

        /// <summary>Number of words</summary>
        public int Count {get {return _Ranges == null ? 0 : _Ranges.Count;}}


        // ********************************************************************************
        /// <summary>
        /// Constructor, parses the text for word ranges
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <created>UPh,15.11.2015</created>
        /// <changed>UPh,15.11.2015</changed>
        // ********************************************************************************
        public WordSegments(string text)
        {
            _Ranges = GetWordRanges(text);
        }

        // ********************************************************************************
        /// <summary>
        /// Returns the char position of a word start
        /// </summary>
        /// <param name="iWord"></param>
        /// <returns></returns>
        /// <created>UPh,15.11.2015</created>
        /// <changed>UPh,15.11.2015</changed>
        // ********************************************************************************
        public int GetWordStart(int iWord)
        {
            return _Ranges[iWord].Item1;
        }

        // ********************************************************************************
        /// <summary>
        /// Returns the char position of a word end (exclusive)
        /// </summary>
        /// <param name="iWord"></param>
        /// <returns></returns>
        /// <created>UPh,15.11.2015</created>
        /// <changed>UPh,15.11.2015</changed>
        // ********************************************************************************
        public int GetWordEnd(int iWord)
        {
            return _Ranges[iWord].Item2;
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Tuple<int"></typeparam>
        /// <typeparam name="int"></typeparam>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <created>UPh,15.11.2015</created>
        /// <changed>UPh,15.11.2015</changed>
        // ********************************************************************************
        private static List<Tuple<int, int>> GetWordRanges(string text)
        {
            int len = text.Length;
            if (len <= 0)
                return null;

            // Find all words
            List<Tuple<int, int>> wordRanges = new List<Tuple<int, int>>();


            int start = -1;
            for (int i = 0; i < len; i++)
            {
                char c = text[i];

                if (!char.IsWhiteSpace(c) && !char.IsPunctuation(c))
                {
                    if (start < 0)
                        start = i;
                    continue;
                }
                else
                {
                    // Whitespace
                    if (start >= 0)
                    {
                        Tuple<int, int> t = new Tuple<int, int>(start, i);
                        wordRanges.Add(t);
                        start = -1;
                    }
                }
            }

            // Add last word
            if (start >= 0)
            {
                Tuple<int, int> t = new Tuple<int, int>(start, len);
                wordRanges.Add(t);
            }

            return wordRanges;

        }
    }
}
