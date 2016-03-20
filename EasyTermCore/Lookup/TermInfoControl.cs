using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EasyTermCore;
using System.Reflection;
using System.IO;

namespace EasyTermCore
{
    public partial class TermInfoControl : UserControl
    {
        public TermInfoControl()
        {
            InitializeComponent();
            txtTermBaseInfo.Text = "";
            txtID.Text = "";

#if DEBUG
            webControl.IsWebBrowserContextMenuEnabled = true;
#endif
        }

        List<string> _Terms = new List<string>();

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="termbaseInfo"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        /// <created>UPh,31.10.2015</created>
        /// <changed>UPh,31.10.2015</changed>
        // ********************************************************************************
        public void SetData(string termbaseInfo, TermInfo info)
        {
            try
            {
                _Terms.Clear();

                txtTermBaseInfo.ForeColor = Color.Black;
                txtTermBaseInfo.Text = termbaseInfo;

                txtID.Text = info.TermID == null ? "" : info.TermID.ToString();

                string html = MakeHTML(info);
#if true
                webControl.DocumentText = html;
#else
                // When AllowNavigation is set to false we need to use OpenNew/Write to replace existing content
                // But this steals the focus 
                if (webControl.Document == null)
                {
                    webControl.DocumentText = html;
                }
                else
                {
                    webControl.Document.OpenNew(true);
                    webControl.Document.Write(html);
                }
#endif

            }
            catch (Exception ex)
            {
                txtTermBaseInfo.Text = ex.Message;
                txtTermBaseInfo.ForeColor = Color.Red;
            }
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        /// <created>UPh,31.10.2015</created>
        /// <changed>UPh,31.10.2015</changed>
        // ********************************************************************************
        private string MakeHTML(TermInfo info)
        {
            TermInfoHTMLBuilder hb = new TermInfoHTMLBuilder();

            hb.AppendHeader();

            hb.AppendProperties(info.Props);

            foreach (TermInfo.LangSet langset in info.LanguageSets)
            {
                hb.AppendLanguage(langset.Language.DisplayName);

                hb.AppendProperties(langset.Props);


                foreach (TermInfo.Term term in langset.Terms)
                {
                    int iTerm = _Terms.Count;
                    _Terms.Add(term.Text);


                    hb.AppendTerm(term.Text, iTerm);
                    hb.AppendProperties(term.Props);
                }
            }

            hb.AppendFooter();
                
            return hb.ToString();
        }

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
        private void webControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // Ctrl + C - copy selected text
            if (e.Control && e.KeyCode == Keys.C)
            {
                webControl.Document.ExecCommand("COPY", false, null); 
                return;
            }

            // Ctrl + P - print
            if (e.Control && e.KeyCode == Keys.P)
            {
                webControl.ShowPrintDialog();
                return;
            }
        }

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
        private void webControl_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            string url = e.Url.ToString();

            if (url.StartsWith("copy:"))
            {
                int iTerm;
                if (int.TryParse(url.Substring(5), out iTerm) && iTerm >= 0 && iTerm < _Terms.Count)
                {
                    Clipboard.SetText(_Terms[iTerm]);
                }
                e.Cancel = true;
            }
        }

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
        private void webControl_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            
        }
    }


    // --------------------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
    // --------------------------------------------------------------------------------
    class TermInfoHTMLBuilder
    {
        StringBuilder _SB = new StringBuilder();

        public void AppendHeader()
        {
            _SB.AppendLine("<html>");
            _SB.AppendLine("<head>");
            _SB.AppendLine("<style>");
            _SB.AppendLine("h1.lang {font: bold 18px Arial; color: blue; margin-bottom: 3px;}");
            _SB.AppendLine("h2.term {font: bold 15px Arial; margin-bottom: 3px;}");
            _SB.AppendLine("p.def {font: italic 12px Arial; margin-top: 3px;}");
            _SB.AppendLine("p.value {font: italic 12px Arial; color: DimGray; margin-bottom: 3px; margin-top: 3px;}");
            _SB.AppendLine("span.key {font-weight: bold;}");
            //_SB.Append("span.value {font: italic 12px Arial; color=DimGray;}");
            _SB.AppendLine("</style>");
            _SB.AppendLine("</head>");
            _SB.AppendLine("<body>");
        }

        public void AppendFooter()
        {
            _SB.Append("</body></html>");
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="props"></param>
        /// <returns></returns>
        /// <created>UPh,31.10.2015</created>
        /// <changed>UPh,31.10.2015</changed>
        // ********************************************************************************
        public void AppendProperties(TermInfo.Properties props)
        {
            if (props == null)
                return;
            if (!string.IsNullOrEmpty(props.Definition))
                AppendDefinition(props.Definition);

            if (props.Status != TermStatus.none)
            {
                if (props.Status == TermStatus.prohibited)
                {
                    AppendValueWithImage("Status", props.Status.ToString(), "Prohibited");
                }
                else
                {
                    AppendValue("Status", props.Status.ToString());
                }
            }    

            if (props.Values != null)
            {
                foreach (var value in props.Values)
                {
                    AppendValue(value.Key, value.Value);
                }
            }

            _SB.AppendLine("");
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="definition"></param>
        /// <returns></returns>
        /// <created>UPh,01.11.2015</created>
        /// <changed>UPh,01.11.2015</changed>
        // ********************************************************************************
        public void AppendDefinition(string definition)
        {
            _SB.Append("<p class='def'>");
            _SB.Append(definition);
            _SB.AppendLine("</p>");
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <created>UPh,01.11.2015</created>
        /// <changed>UPh,01.11.2015</changed>
        // ********************************************************************************
        public void AppendValue(string key, string value)
        {
            _SB.Append("<p class='value'><span class='key'>");
            _SB.Append(key);
            _SB.Append(": ");
            _SB.Append("</span><span class='value'>");
            _SB.Append(value);
            _SB.AppendLine("</span></p>");
    

        }

        // ********************************************************************************
        /// <summary>
        /// Append a key value pair with image before the key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="image"></param>
        /// <created>UPh,20.03.2016</created>
        /// <changed>UPh,20.03.2016</changed>
        // ********************************************************************************
        public void AppendValueWithImage(string key, string value, string image)
        {
            string respath = Assembly.GetCallingAssembly().Location;
            respath = respath.Replace("\\", "%5C");

            _SB.Append("<p class='value'><span class='key'>");

            string img = string.Format(" <img style=\"vertical-align=middle\" hspace=2 border=0 src=\"res://{0}/PNG/{1}\" alt=\"{1}\"/>", respath, image);
            _SB.Append(img);

            _SB.Append(key);
            _SB.Append(": ");
            _SB.Append("</span><span class='value'>");
            _SB.Append(value);
            _SB.AppendLine("</span></p>");
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <created>UPh,01.11.2015</created>
        /// <changed>UPh,01.11.2015</changed>
        // ********************************************************************************
        public void AppendLanguage(string text)
        {
            _SB.Append("<h1 class='lang'>");
            AppendText(text);
            _SB.AppendLine("</h1>");
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <created>UPh,01.11.2015</created>
        /// <changed>UPh,01.11.2015</changed>
        // ********************************************************************************
        public void AppendTerm(string text, int iTerm)
        {
            _SB.Append("<h2 class='term'>");
            AppendText(text);

            string respath = Assembly.GetCallingAssembly().Location;
            respath = respath.Replace("\\", "%5C");

            //string img = string.Format("<img src=\"{0}/PNG/#101\" />", respath);

            string img = string.Format(" <a href=\"copy:{0}\"><img hspace=2 border=0 src=\"res://{1}/PNG/COPY\" alt=\"Copy\"/></a>", iTerm.ToString(), respath);

            _SB.AppendLine(img);
            _SB.AppendLine("</h2>");
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <created>UPh,01.11.2015</created>
        /// <changed>UPh,01.11.2015</changed>
        // ********************************************************************************
        public void AppendParagraph(string text)
        {
            AppendTag("p", text);
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <created>UPh,01.11.2015</created>
        /// <changed>UPh,01.11.2015</changed>
        // ********************************************************************************
        public void AppendTag(string tag, string text)
        {
            _SB.AppendFormat("<{0}>{1}</{0}>", tag, text);
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <created>UPh,20.03.2016</created>
        /// <changed>UPh,20.03.2016</changed>
        // ********************************************************************************
        public void AppendText(string text)
        {
            text = text.Replace("<", "&lt;");
            _SB.Append(text);
        }

        public override string ToString()
        {
            return _SB.ToString();
        }

    }
}
