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
using EasyTermViewer.Properties;
using PassLib;
using System.Threading;

namespace EasyTermViewer
{
    public partial class MainForm : Form
    {
        TermBaseSet _TermbaseSet;
        TermBaseQuery _TermBaseQuery;
        int _IgnoreNotification = 0;
        private PlStorePosition _StorePosition;

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,31.10.2015</created>
        /// <changed>UPh,31.10.2015</changed>
        // ********************************************************************************
        public MainForm()
        {
            CultureInfo ci = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            Icon = Resources.app;

            string inipath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            inipath = Path.Combine(inipath, "EasyTermViewer\\profile.ini");
            PlProfile.SetProfileName(inipath, PlProfile.ProfileType.IniFile);
            EasyTermCoreSettings.ProfilePath = inipath;

            _StorePosition = new PlStorePosition();
            _StorePosition.Initialize(this);

            InitializeComponent();

            _TermbaseSet = new TermBaseSet();
            _TermbaseSet.SettingsFile = Environment.ExpandEnvironmentVariables("%appdata%\\EasyTermViewer\\settings.xml");
            Directory.CreateDirectory(Path.GetDirectoryName(_TermbaseSet.SettingsFile));

            _TermbaseSet.LoadStoredAndLocal();
            _TermBaseQuery = _TermbaseSet.Query;

            _TermBaseQuery.TermListResult    += TermBaseQuery_TermListResult;
            _TermBaseQuery.TermInfoResult    += TermBaseQuery_TermInfoResult;
            _TermBaseQuery.TerminologyResult += TermBaseQuery_TerminologyResult;

            _TermListResult = new TermListResultCallback(OnTermListResult);
            _TermInfoResult = new TermInfoResultCallback(OnTermInfoResult);
            _TerminologyResult = new TerminologyResultCallback(OnTerminologyResult);

            lstTerms.TermBaseSet = _TermbaseSet;
            lstTerminology.TermBaseSet = _TermbaseSet;

            lstTerms.Dock = DockStyle.Fill;
            lstTerminology.Dock = DockStyle.Fill;

            FindType = FindTypes.Text;

            InitializeLanguageComboBoxes();
            InitializeLanguageSelection();
        }


        delegate void TermListResultCallback(TermListResultArgs e);
        TermListResultCallback _TermListResult;

        delegate void TermInfoResultCallback(TermInfoResultArgs e);
        TermInfoResultCallback _TermInfoResult;

        delegate void TerminologyResultCallback(TerminologyResultArgs e);
        TerminologyResultCallback _TerminologyResult;


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,19.11.2015</created>
        /// <changed>UPh,19.11.2015</changed>
        // ********************************************************************************
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            txtFind.Focus();

        }



        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        /// <created>UPh,19.11.2015</created>
        /// <changed>UPh,19.11.2015</changed>
        // ********************************************************************************
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F))
            {
                txtFind.Focus();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }


        // ********************************************************************************
        /// <summary>
        /// Event Handler for results on TermInfo query
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,31.10.2015</created>
        /// <changed>UPh,31.10.2015</changed>
        // ********************************************************************************
        void TermBaseQuery_TermInfoResult(object sender, TermInfoResultArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(_TermInfoResult, e);
                return;
            }
            else
            {
                OnTermInfoResult(e);
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

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,31.10.2015</created>
        /// <changed>UPh,31.10.2015</changed>
        // ********************************************************************************
        void OnTermInfoResult(TermInfoResultArgs e)
        {
            string name = _TermbaseSet.GetDisplayName(e.TermBaseID);

            termInfoControl.SetData(name, e.Info);            
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ci1"></param>
        /// <param name="ci2"></param>
        /// <returns></returns>
        /// <created>UPh,01.11.2015</created>
        /// <changed>UPh,01.11.2015</changed>
        // ********************************************************************************
        private void GetCurrentLanguagePair(out CultureInfo ci1, out CultureInfo ci2)
        {
            if (_TermBaseQuery.GetLanguagePair(out ci1, out ci2))
                return;

            // Use previous selection
            
            
            string name1 = PlProfile.GetString("Settings", "Language1", "en");
            string name2 = PlProfile.GetString("Settings", "Language2", "de");

            try
            {
                ci1 = CultureInfo.GetCultureInfo(name1);
                ci2 = CultureInfo.GetCultureInfo(name2);
                return;
            }
            catch (Exception)
            {

            }

            ci1 = CultureInfo.GetCultureInfo("en");
            ci2 = CultureInfo.GetCultureInfo("de");
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
            _IgnoreNotification++;

            cmdLanguage1.Items.Clear();
            cmdLanguage2.Items.Clear();

            CultureInfo lang1 = null;
            CultureInfo lang2 = null;
            GetCurrentLanguagePair(out lang1, out lang2);

            int select1 = -1;
            int select2 = -1;

            List<CultureInfo> cis = _TermBaseQuery.GetLanguages();

            cis.Sort((a,b) => string.Compare(a.DisplayName, b.DisplayName, true));

            foreach (CultureInfo ci in cis)
            {
                if (lang1.Name == ci.Name)
                    select1 = cmdLanguage1.Items.Count;
                if (lang2.Name == ci.Name)
                    select2 = cmdLanguage1.Items.Count;

                cmdLanguage1.Items.Add(ci);
                cmdLanguage2.Items.Add(ci);
            }

            if (select1 < 0)
                select1 = Math.Min(0, cmdLanguage1.Items.Count - 1);
            if (select2 < 0)
                select2 = Math.Min(1, cmdLanguage2.Items.Count - 1);

            if (select1 >= 0)
                cmdLanguage1.SelectedIndex = select1;
            if (select2 >= 0)
                cmdLanguage2.SelectedIndex = select2;
            _IgnoreNotification--;

            OnLanguageSelectionChanged(true);
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
            ResetFilterChange();

            try
            {
                if (_TermbaseSet.EditTermBases())
                {
                    _IgnoreNotification++;
                    txtFind.Clear();
                    InitializeLanguageSelection();
                    InitializeTermList();
                    _IgnoreNotification--;
                }

            }
            catch (Exception)
            {
            }
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
                lstTerms.SelectedIndices.Clear();
                lstTerms.ClearContent();
                _TermBaseQuery.RequestTermList(0);

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
        private void txtFind_TextChanged(object sender, EventArgs e)
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
            if (FindType == FindTypes.Text && 
                _LastFilterTextChange != DateTime.MinValue &&
                (DateTime.Now - _LastFilterTextChange).TotalMilliseconds >= 500)
            {
                ResetFilterChange();
                DoFind();
            }
        }

        // ********************************************************************************
        /// <summary>
        /// Resets the timer for filter update 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,19.11.2015</created>
        /// <changed>UPh,19.11.2015</changed>
        // ********************************************************************************
        private void ResetFilterChange()
        {
            _LastFilterTextChange = DateTime.MinValue;
            timerFilter.Enabled = false;
        }


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,19.11.2015</created>
        /// <changed>UPh,19.11.2015</changed>
        // ********************************************************************************
        private void txtFind_KeyDown(object sender, KeyEventArgs e)
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
            else if (e.KeyCode == Keys.Enter)
            {
                DoFind();
                e.Handled = true;
            }
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,19.11.2015</created>
        /// <changed>UPh,19.11.2015</changed>
        // ********************************************************************************
        private void DoFind()
        {
            switch (FindType)
	        {
		        case FindTypes.Text: DoFindText(); break;
		        case FindTypes.Terminology: DoFindTerminology(); break;
	        }
        }


#region Finding terms by text
        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,19.11.2015</created>
        /// <changed>UPh,19.11.2015</changed>
        // ********************************************************************************
        void DoFindText()
        {
            lstTerms.Filter(txtFind.Text);
        }

        // ********************************************************************************
        /// <summary>
        /// Event Handler for results on TermList query
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,22.11.2015</created>
        /// <changed>UPh,22.11.2015</changed>
        // ********************************************************************************
        private void lstTerminology_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplaySelectedTerm();
        }

        
#endregion


#region Finding terminology
        int _FindTermRequestID = 1;

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,19.11.2015</created>
        /// <changed>UPh,19.11.2015</changed>
        // ********************************************************************************
        void DoFindTerminology()
        {
            lstTerminology.Initialize();
            _TermBaseQuery.RequestTerminology(txtFind.Text, ++_FindTermRequestID);
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,31.10.2015</created>
        /// <changed>UPh,31.10.2015</changed>
        // ********************************************************************************
        void TermBaseQuery_TerminologyResult(object sender, TerminologyResultArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(_TerminologyResult, e);
                return;
            }
            else
            {
                OnTerminologyResult(e);
            }
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,20.11.2015</created>
        /// <changed>UPh,20.11.2015</changed>
        // ********************************************************************************
        void OnTerminologyResult(TerminologyResultArgs e)
        {
            if (FindType != FindTypes.Terminology ||
               _FindTermRequestID != e.RequestID)
               return;
               
            lstTerminology.AddItem(e);   
        }


        
#endregion
        
        // ********************************************************************************
        /// <summary>
        /// User changed selection in first language combo box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,07.11.2015</created>
        /// <changed>UPh,07.11.2015</changed>
        // ********************************************************************************
        private void cmdLanguage1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_IgnoreNotification > 0)
                return;

            OnLanguageSelectionChanged(true);
        }

        // ********************************************************************************
        /// <summary>
        /// User changed selection in second language combo box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,07.11.2015</created>
        /// <changed>UPh,07.11.2015</changed>
        // ********************************************************************************
        private void cmdLanguage2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_IgnoreNotification > 0)
                return;

            OnLanguageSelectionChanged(false);
        }

        // ********************************************************************************
        /// <summary>
        /// Handle change in one of the language combo boxes
        /// </summary>
        /// <param name="bLang1">true = first language, false = second language has changed</param>
        /// <returns></returns>
        /// <created>UPh,29.10.2015</created>
        /// <changed>UPh,07.11.2015</changed>
        // ********************************************************************************
        private void OnLanguageSelectionChanged(bool bLang1)
        {
            CultureInfo lang1 = cmdLanguage1.SelectedItem as CultureInfo;
            CultureInfo lang2 = cmdLanguage2.SelectedItem as CultureInfo;

            _TermBaseQuery.SetLanguagePair(lang1, lang2);

            if (lang1 != null && lang2 != null)
            {
                // Store language selection 
                PlProfile.WriteString("Settings", "Language1", lang1.Name);
                PlProfile.WriteString("Settings", "Language2", lang2.Name);
            }

            // If first language has changed we need to re-build the term list
            if (bLang1)
                InitializeTermList();

            DisplaySelectedTerm();
        }


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,29.10.2015</created>
        /// <changed>UPh,29.10.2015</changed>
        // ********************************************************************************
        private void InitializeLanguageComboBoxes()
        {
            cmdLanguage1.ComboBox.DrawMode = DrawMode.OwnerDrawFixed;
            cmdLanguage1.ComboBox.DrawItem += ComboBox_DrawItem;

            cmdLanguage2.ComboBox.DrawMode = DrawMode.OwnerDrawFixed;
            cmdLanguage2.ComboBox.DrawItem += ComboBox_DrawItem;
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
        void ComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            if (cmb == null)
                return;

            if (e.Index < 0 || e.Index >= cmb.Items.Count)
                return;

            CultureInfo ci = cmb.Items[e.Index] as CultureInfo;
            if (ci == null)
                return;

            e.DrawBackground();
            EasyTermCore.Tools.DrawLanguageString(e.Graphics, e.Font, e.ForeColor, e.Bounds, ci);
            e.DrawFocusRectangle();
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,31.10.2015</created>
        /// <changed>UPh,31.10.2015</changed>
        // ********************************************************************************
        private void lstTerms_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplaySelectedTerm();
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,31.10.2015</created>
        /// <changed>UPh,31.10.2015</changed>
        // ********************************************************************************
        private void DisplaySelectedTerm()
        {
            if (FindType == FindTypes.Text)
            {
                TermListItem item = lstTerms.GetSelectedItem();
                if (item == null)
                    return;

                _TermBaseQuery.RequestTermInfo(item, 0);
            }
            else if (FindType == FindTypes.Terminology)
            {
                TerminologyResultArgs tra = lstTerminology.GetSelectedItem();
                if (tra == null)
                    return;

                TermListItem item = new TermListItem(tra);

                _TermBaseQuery.RequestTermInfo(item, 0);

            }
        }


        enum FindTypes {Unknown, Text, Terminology};

        FindTypes _FindType = FindTypes.Unknown;
        /// <summary></summary>
        FindTypes FindType
        {
            get {return _FindType;}
            set 
            {
                if (_FindType == value)
                    return;

                _FindType = value;
                OnFindTypeChanged();
            }
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,21.11.2015</created>
        /// <changed>UPh,21.11.2015</changed>
        // ********************************************************************************
        private void btnFind_ButtonClick(object sender, EventArgs e)
        {
            DoFind();
        }


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,19.11.2015</created>
        /// <changed>UPh,19.11.2015</changed>
        // ********************************************************************************
        private void btnFindText_Click(object sender, EventArgs e)
        {
            FindType = FindTypes.Text;
            DoFind();
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,19.11.2015</created>
        /// <changed>UPh,19.11.2015</changed>
        // ********************************************************************************
        private void btnFindTerm_Click(object sender, EventArgs e)
        {
            FindType = FindTypes.Terminology;
            DoFind();
        }


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,20.11.2015</created>
        /// <changed>UPh,20.11.2015</changed>
        // ********************************************************************************
        void OnFindTypeChanged()
        {
            // Button gets text and image from menu entry
            ToolStripMenuItem btn;
            switch (_FindType)
            {
                case FindTypes.Text:
                    btn = btnFindText;
                    break;

                case FindTypes.Terminology:
                    btn = btnFindTerm;
                    break;
                default: return;
            }

            //
            btnFind.Text = btn.Text;
            btnFind.Image = btn.Image;
            btnFind.ToolTipText = btn.ToolTipText;

            lstTerminology.ClearContent();

            // Switch visibility of both lists
            lstTerms.Visible = (_FindType == FindTypes.Text);
            lstTerminology.Visible = (_FindType == FindTypes.Terminology);
        }

        // ********************************************************************************
        /// <summary>
        /// Click on language arrow -> switch languages
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,23.01.2016</created>
        /// <changed>UPh,23.01.2016</changed>
        // ********************************************************************************
        private void lblLanguageDirection_Click(object sender, EventArgs e)
        {
            SwitchLanguagePair();
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,23.01.2016</created>
        /// <changed>UPh,23.01.2016</changed>
        // ********************************************************************************
        private void SwitchLanguagePair()
        {
            int sel1 = cmdLanguage1.SelectedIndex;
            int sel2 = cmdLanguage2.SelectedIndex;
            if (sel1 < 0 || sel2 < 0)
                return;

            _IgnoreNotification++;

            cmdLanguage1.SelectedIndex = sel2;
            cmdLanguage2.SelectedIndex = sel1;

            _IgnoreNotification--;

            OnLanguageSelectionChanged(true);

        }
    }
}
