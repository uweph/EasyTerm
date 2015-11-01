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

namespace EasyTermViewer
{
    public partial class TermInfoControl : UserControl
    {
        public TermInfoControl()
        {
            InitializeComponent();
        }

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
                txtTermBaseInfo.ForeColor = Color.Black;
                txtTermBaseInfo.Text = termbaseInfo;

                string html = MakeHTML(info);
                if (webControl.Document == null)
                {
                    webControl.DocumentText = html;
                }
                else
                {
                    webControl.Document.OpenNew(true);
                    webControl.Document.Write(html);
                }

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
                    hb.AppendTerm(term.Text);
                    hb.AppendProperties(term.Props);
                }
            }

            hb.AppendFooter();
                
            return hb.ToString();
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
            _SB.Append("<html>");
            _SB.Append("<head>");
            _SB.Append("<style>");
            _SB.Append("h1.lang {font: bold 18px Arial; color: blue; margin-bottom: 3px;}");
            _SB.Append("h2.term {font: bold 15px Arial; margin-bottom: 3px;}");
            _SB.Append("p.def {font: italic 12px Arial; margin-top: 3px;}");
            _SB.Append("p.value {font: italic 12px Arial; color: DimGray; margin-bottom: 3px; margin-top: 3px;}");
            _SB.Append("span.key {font-weight: bold;}");
            //_SB.Append("span.value {font: italic 12px Arial; color=DimGray;}");
            _SB.Append("</style>");
            _SB.Append("</head>");
            _SB.Append("<body>");
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
            if (props.Values != null)
            {
                foreach (var value in props.Values)
                {
                    AppendValue(value.Key, value.Value);
                }
            }

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
            _SB.Append("</p>");
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
            _SB.Append("</span></p>");
    

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
            _SB.Append("</h1>");
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
        public void AppendTerm(string text)
        {
            _SB.Append("<h2 class='term'>");
            AppendText(text);
            _SB.Append("</h2>");
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
