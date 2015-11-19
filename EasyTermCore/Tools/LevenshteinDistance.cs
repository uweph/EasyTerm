using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyTermCore
{
    class LevenshteinDistance
    {
        int[] v0;
        int[] v1;
     
        
        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxlength"></param>
        /// <returns></returns>
        /// <created>UPh,18.11.2015</created>
        /// <changed>UPh,18.11.2015</changed>
        // ********************************************************************************
        public LevenshteinDistance(int maxlength = 255)
        {
            v0 = new int[maxlength + 1];
            v1 = new int[maxlength + 1];
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text1"></param>
        /// <param name="text2"></param>
        /// <returns></returns>
        /// <created>UPh,18.11.2015</created>
        /// <changed>UPh,18.11.2015</changed>
        // ********************************************************************************
        int DistanceOf(string text1, string text2)
        {
            // degenerate cases
            if (text1 == text2) return 0;
            if (text1.Length == 0) return text2.Length;
            if (text2.Length == 0) return text1.Length;

            if (text2.Length > v0.Length)
                return text2.Length;

            // initialize v0 (the previous row of distances)
            // this row is A[0][i]: edit distance for an empty s
            // the distance is just the number of characters to delete from t
            for (int i = 0; i < v0.Length; i++)
                v0[i] = i;

            for (int i = 0; i < text1.Length; i++)
            {
                // calculate v1 (current row distances) from the previous row v0

                // first element of v1 is A[i+1][0]
                //   edit distance is delete (i+1) chars from s to match empty t
                v1[0] = i + 1;

                // use formula to fill in the rest of the row
                for (int j = 0; j < text2.Length; j++)
                {
                    var cost = (text1[i] == text2[j]) ? 0 : 1;
                    v1[j + 1] = Math.Min(v1[j] + 1, Math.Min(v0[j + 1] + 1, v0[j] + cost));
                }

                // copy v1 (current row) to v0 (previous row) for next iteration
                for (int j = 0; j < v0.Length; j++)
                    v0[j] = v1[j];
            }

            return v1[text2.Length];
        }

    }
}
