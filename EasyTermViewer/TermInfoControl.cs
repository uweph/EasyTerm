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
            HTMLBuilder hb = new HTMLBuilder();

            hb.AppendHeader();

            hb.AppendProperties(info.Props);

            foreach (TermInfo.LangSet langset in info.LanguageSets)
            {
                hb.AppendHeader1(langset.Language.DisplayName);

                hb.AppendProperties(langset.Props);


                foreach (TermInfo.Term term in langset.Terms)
                {
                    hb.AppendHeader2(term.Text);
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
    class HTMLBuilder
    {
        StringBuilder _SB = new StringBuilder();

        public void AppendHeader()
        {
            _SB.Append("<html>");
            _SB.Append("<head>");
            _SB.Append("<style>");
            _SB.Append("h1 {font: bold 18px Arial; color: blue;}");
            _SB.Append("h2 {font: bold 15px Arial;}");
            _SB.Append("p.def {font: italic 12px Arial;}");
            _SB.Append("span.key {font: bold italic 12px Arial; color=DimGray;}");
            _SB.Append("span.value {font: italic 12px Arial; color=DimGray;}");
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

        public void AppendDefinition(string definition)
        {
            _SB.Append("<p class='def'>");
            _SB.Append(definition);
            _SB.Append("</p>");
        }

        public void AppendValue(string key, string value)
        {
            _SB.Append("<span class='key'>");
            _SB.Append(key);
            _SB.Append(": ");
            _SB.Append("</span><span class='value'>");
            _SB.Append(value);
            _SB.Append("</span><br/>");
    

        }

        public void AppendHeader1(string text)
        {
            AppendTag("h1", text);
        }

        public void AppendHeader2(string text)
        {
            AppendTag("h2", text);
        }

        public void AppendParagraph(string text)
        {
            AppendTag("p", text);
        }

        public void AppendTag(string tag, string text)
        {
            _SB.AppendFormat("<{0}>{1}</{0}>", tag, text);
        }



        public override string ToString()
        {
            return _SB.ToString();
        }

    }
}
