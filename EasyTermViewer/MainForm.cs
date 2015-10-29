using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EasyTermCore;
using System.IO;
using System.Globalization;

namespace EasyTermViewer
{
    public partial class MainForm : Form
    {
        TermBaseSet _TermbaseSet;
        TermBaseQuery _TermBaseQuery;
        int _IgnoreNotification = 0;

        public MainForm()
        {
            InitializeComponent();

            _TermbaseSet = new TermBaseSet();
            _TermbaseSet.SettingsFile = Environment.ExpandEnvironmentVariables("%appdata%\\EasyTermViewer\\settings.xml");
            Directory.CreateDirectory(Path.GetDirectoryName(_TermbaseSet.SettingsFile));

            _TermbaseSet.LoadStoredAndLocal();
            _TermBaseQuery = _TermbaseSet.Query;

            _TermBaseQuery.TerminologyResult += TermBaseQuery_TerminologyResult;
            _TermBaseQuery.TermListResult    += TermBaseQuery_TermListResult;

            _TermListResult = new TermListResultCallback(OnTermListResult);

            InitializeLanguageSelection();
        }

        delegate void TermListResultCallback(TermListResultArgs e);
        TermListResultCallback _TermListResult;


        // ********************************************************************************
        /// <summary>
        /// Event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        void TermBaseQuery_TermListResult(object sender, TermListResultArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(_TermListResult, e);

                int a = e.RequestID;
                return;     
            }
            else
            {
                OnTermListResult(e);
            }


        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        void OnTermListResult(TermListResultArgs e)
        {
            lstTerms.Initialize(e.Items);
        }

        void TermBaseQuery_TerminologyResult(object sender, EventArgs e)
        {
            
        }

        class LanguageTag : CultureInfo
        {
            public LanguageTag(string name) :
            base(name)
            {
                
            }
            public override string ToString()
            {
                return DisplayName;
            }
        }


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,29.10.2015</created>
        /// <changed>UPh,29.10.2015</changed>
        // ********************************************************************************
        private void InitializeLanguageSelection()
        {
            cmdLanguage1.Items.Add(new LanguageTag("en"));
            cmdLanguage1.Items.Add(new LanguageTag("de"));
            cmdLanguage1.Items.Add(new LanguageTag("fr"));
            cmdLanguage1.Items.Add(new LanguageTag("es"));

            cmdLanguage2.Items.Add(new LanguageTag("en"));
            cmdLanguage2.Items.Add(new LanguageTag("de"));
            cmdLanguage2.Items.Add(new LanguageTag("fr"));
            cmdLanguage2.Items.Add(new LanguageTag("es"));

            _IgnoreNotification++;
            cmdLanguage1.SelectedIndex = 0;
            cmdLanguage2.SelectedIndex = 1;
            _IgnoreNotification--;

            OnLanguageSelectionChanged();
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,29.08.2015</created>
        /// <changed>UPh,29.08.2015</changed>
        // ********************************************************************************
        private void cmdTermBases_Click(object sender, EventArgs e)
        {
            _TermbaseSet.EditTermBases();

            InitializeTermList();
        }

        // ********************************************************************************
        /// <summary>
        /// Rebuild listbox with terms
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        void InitializeTermList()
        {
            try
            {
                lstTerms.Clear();
                _TermBaseQuery.RequestTermList();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _TermBaseQuery.Dispose();
            _TermBaseQuery = null;
        }

        DateTime _LastFilterTextChange = DateTime.MinValue;

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        private void txtFindTerm_TextChanged(object sender, EventArgs e)
        {
            _LastFilterTextChange = DateTime.Now;
            timerFilter.Enabled = true;
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        private void timerFilter_Tick(object sender, EventArgs e)
        {
            if (_LastFilterTextChange != DateTime.MinValue &&
                (DateTime.Now - _LastFilterTextChange).TotalMilliseconds >= 500)
            {
                lstTerms.Filter(txtFindTerm.Text);

                _LastFilterTextChange = DateTime.MinValue;
                timerFilter.Enabled = false;
            }
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        private void txtFindTerm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                lstTerms.SelectNextItem(true);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Up)
            {
                lstTerms.SelectNextItem(false);
                e.Handled = true;
            }
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,29.10.2015</created>
        /// <changed>UPh,29.10.2015</changed>
        // ********************************************************************************
        private void cmdLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnLanguageSelectionChanged();
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,29.10.2015</created>
        /// <changed>UPh,29.10.2015</changed>
        // ********************************************************************************
        private void OnLanguageSelectionChanged()
        {
            CultureInfo lang1 = cmdLanguage1.SelectedItem as CultureInfo;
            CultureInfo lang2 = cmdLanguage2.SelectedItem as CultureInfo;

            _TermBaseQuery.SetLanguagePair(lang1, lang2);

            InitializeTermList();
        }

    }
}
