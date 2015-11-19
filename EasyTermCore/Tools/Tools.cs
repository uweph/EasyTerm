using EasyTermCore.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyTermCore
{
    public class Tools
    {
        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// </summary>
        /// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
        /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
        /// <returns>The relative path from the start directory to the end path.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UriFormatException"></exception>
        /// <exception cref="InvalidOperationException"></exception>


        // ********************************************************************************
        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// </summary>
        /// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
        /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
        /// <returns>The relative path from the start directory to the end path.</returns>
        /// <created>UPh,29.08.2015</created>
        /// <changed>UPh,29.08.2015</changed>
        // ********************************************************************************
        public static String MakeRelativePath(String fromPath, String toPath)
        {
            if (String.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
            if (String.IsNullOrEmpty(toPath)) throw new ArgumentNullException("toPath");

            Uri fromUri = new Uri(fromPath);
            Uri toUri = new Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.ToUpperInvariant() == "FILE")
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

        static int GetFlagIndex(string name)
        {
            switch(name.ToLower())
            {
                case "en":    return 1;     // English
                case "en-gb": return 2;     // English, Great Britain
                case "de": return 3;        // German
                case "fr": return 4;        // French
                case "es": return 5;        // Spanish
                case "pt": return 6;        // Portuguese
                case "pt-br": return 7;     // Portuguese, Brazil
                case "it": return 8;        // Italian
                case "pl": return 9;        // Polish
                case "tr": return 10;       // Turkish
                case "zh": return 11;       // Chinese
                case "ja": return 12;       // Japanese
                case "sv": return 13;       // Svedish
            }

            return -1;
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ci"></param>
        /// <returns></returns>
        /// <created>UPh,15.11.2015</created>
        /// <changed>UPh,15.11.2015</changed>
        // ********************************************************************************
        static int GetFlagIndex(CultureInfo ci)
        {
            int inx = GetFlagIndex(ci.Name);
            if (inx < 0 && ci.Parent != null)
                inx = GetFlagIndex(ci.Parent.Name);

            if (inx < 0)
                inx = 0; // Neutral flag

            return inx;
        }


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="brush"></param>
        /// <param name="font"></param>
        /// <param name="rect"></param>
        /// <param name="ci"></param>
        /// <returns></returns>
        /// <created>UPh,29.10.2015</created>
        /// <changed>UPh,29.10.2015</changed>
        // ********************************************************************************
        public static void DrawLanguageString(Graphics g, Font font, Color color, Rectangle rect, CultureInfo ci)
        {
            SolidBrush brush = new SolidBrush(color);

            RectangleF rectF = new RectangleF(rect.Left + 20, rect.Top, rect.Width, rect.Height);

            g.DrawString(ci.DisplayName, font, brush, rectF);

            int inx = GetFlagIndex(ci);
            if (inx >= 0)
            {
                Rectangle rcFlag = new Rectangle(rect.Left + 2, rect.Top + 1, 16, 16);
                g.DrawImage(Resources.Flags, rcFlag, 16 * inx, 0, 16, 16, GraphicsUnit.Pixel, null);
            }
        }

        public static int GetPrimaryLangID(int lcid)
        {
            return lcid & 0x3ff;
        }

        public static int GetSubLangID(int lcid)
        {
            return lcid >> 10;
        }

        public static int GetNeutralLCID(int lcid)
        {
            return MakeLangID(GetPrimaryLangID(lcid), 0);
        }

        public static int MakeLangID(int primary, int sub)
        {
            return (((ushort)sub) << 10) | ((ushort)primary);
        }


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lcid1"></param>
        /// <param name="lcid2"></param>
        /// <returns>0: no match, 1: Primary language matches, 2: Full match</returns>
        /// <created>UPh,15.11.2015</created>
        /// <changed>UPh,15.11.2015</changed>
        // ********************************************************************************
        public static int GetLanguageMatch(int lcid1, int lcid2)
        {
            if (lcid1 == lcid2)
                return 2;

            if (GetSubLangID(lcid1) != 0 && GetSubLangID(lcid2) != 0)
                return 0; // Both LCIDs have different country

            // One LCID is neutral
            if (GetPrimaryLangID(lcid1) == GetPrimaryLangID(lcid2))
                return 1;

            return 0;
        }

        int LevenshteinDistance(string text1, string text2)
        {
            // degenerate cases
            if (text1 == text2) return 0;
            if (text1.Length == 0) return text2.Length;
            if (text2.Length == 0) return text1.Length;

            // create two work vectors of integer distances
            int[] v0 = new int[text2.Length + 1];
            int[] v1 = new int[text2.Length + 1];

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
