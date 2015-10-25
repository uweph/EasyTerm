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

namespace EasyTermViewer
{
    public partial class MainForm : Form
    {
        TermBaseSet _TermbaseSet;
        TermBaseQuery _TermBaseQuery;

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


            InitializeTermList();
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
            using (TermBaseSelectionForm form = new TermBaseSelectionForm(_TermbaseSet))
            {
                form.ShowDialog();
            }   

            InitializeTermList();
        }

        // ********************************************************************************
        /// <summary>
        /// 
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

    }
}
