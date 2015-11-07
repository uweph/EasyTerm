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
    }
}
