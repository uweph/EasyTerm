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

            if (!string.IsNullOrWhiteSpace(info.Description))
                hb.AppendText(info.Description);

            foreach (TermInfo.LangSet langset in info.LanguageSets)
            {
                hb.AppendHeader1(langset.Language.DisplayName);


                foreach (TermInfo.Term term in langset.Terms)
                {
                    hb.AppendHeader2(term.Text);
                    hb.AppendText(term.Description);
                }
            }
                
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

        public void AppendHeader1(string text)
        {
            AppendTag("h1", text);
        }

        public void AppendHeader2(string text)
        {
            AppendTag("h2", text);
        }

        public void AppendText(string text)
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
