using PassLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyTermCore
{
    public partial class TermBaseSelectionForm : Form
    {
        TermBaseSet _TermbaseSet;
        bool _DataChanged = false;
        private PlStorePosition _StorePosition;

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        /// <created>UPh,29.08.2015</created>
        /// <changed>UPh,29.08.2015</changed>
        // ********************************************************************************
        public TermBaseSelectionForm(TermBaseSet set)
        {
            if (PlProfile.Type != PlProfile.ProfileType.Unknown)
            {
                _StorePosition = new PlStorePosition();
                _StorePosition.Initialize(this);
            }
            InitializeComponent();
            _TermbaseSet = set;

            lstFiles.FillList(_TermbaseSet);
            lstFiles.ActiveChanged += lstFiles_ActiveChanged;

            UpdateControls();
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,04.10.2015</created>
        /// <changed>UPh,04.10.2015</changed>
        // ********************************************************************************
        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "Term base files|*.tbx;*.sdltb;*.csv|All files (*.*)|*.*";
                if (dlg.ShowDialog() != DialogResult.OK)
                    return;

                foreach (TermBaseFile file in _TermbaseSet.Files)
                {
                    if (string.Compare(file.FilePath, dlg.FileName, true) == 0)
                    {
                        MessageBox.Show("This file is already in the list.");
                        return;
                    }
                }

                TermBaseFile newfile = new TermBaseFile(dlg.FileName);
                newfile.Active = true;
                _TermbaseSet.Files.Add(newfile);

                int index = lstFiles.Items.Add(newfile);

                lstFiles.SelectedIndex = index;

                _DataChanged = true;
            }
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,01.11.2015</created>
        /// <changed>UPh,01.11.2015</changed>
        // ********************************************************************************
        private void btnRemove_Click(object sender, EventArgs e)
        {
            TermBaseFile file = lstFiles.SelectedItem as TermBaseFile;
            if (file == null)
                return;

            if (file.IsAutomatic)
            {
                MessageBox.Show("This file is added automatically,\n" +
                     "because it's in one of the configured term base folders.\n\n" +
                     "It cannot be removed from the list.\n" +
                     "Clear the activation check box if you don't want to use this file", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Do you want to remove this term base file from the list?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            int index = _TermbaseSet.Files.IndexOf(file);
            if (index >= 0)
                _TermbaseSet.Files.RemoveAt(index);

            lstFiles.Items.RemoveAt(lstFiles.SelectedIndex);
            UpdateControls();

            _DataChanged = true;
        }


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,04.10.2015</created>
        /// <changed>UPh,04.10.2015</changed>
        // ********************************************************************************
        private void btnClose_Click(object sender, EventArgs e)
        {
            if (_DataChanged)
                _TermbaseSet.Save();

            Close();
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,01.11.2015</created>
        /// <changed>UPh,01.11.2015</changed>
        // ********************************************************************************
        private void lstFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <created>UPh,01.11.2015</created>
        /// <changed>UPh,01.11.2015</changed>
        // ********************************************************************************
        private void lstFiles_ActiveChanged(object sender, ActiveChangedEventArgs file)
        {
            _DataChanged = true;
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
        void picker_ColorSelected(object sender, ColorSelectedEventArgs e)
        {
            TermBaseFile file = lstFiles.SelectedItem as TermBaseFile;
            if (file == null)
                return;

            file.DisplayColor = e.Color;
            lstFiles.Invalidate();
            
            _DataChanged = true;
        }   


        ToolStripDropDown _ColorPopup;
        ToolStripDropDown ColorPopup
        {
            get
            {
                if (_ColorPopup == null)
                {
                    _ColorPopup = new ToolStripDropDown();
                    _ColorPopup.Margin = Padding.Empty;
                    _ColorPopup.Padding = Padding.Empty;

                    EasyColorPicker picker = new EasyColorPicker();
                    picker.ColorSelected += picker_ColorSelected;

                    ToolStripControlHost host = new ToolStripControlHost(picker);
                    host.Margin = Padding.Empty;
                    host.Padding = Padding.Empty;
                    _ColorPopup.Items.Add(host);


                }

                return _ColorPopup;
            }
        }


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,01.11.2015</created>
        /// <changed>UPh,01.11.2015</changed>
        // ********************************************************************************
        private void btnDisplayColor_Click(object sender, EventArgs e)
        {
            TermBaseFile file = lstFiles.SelectedItem as TermBaseFile;
            if (file == null)
                return;

            Rectangle rc = btnDisplayColor.Bounds;
            rc = RectangleToScreen(rc);

            ColorPopup.Show(new Point(rc.Right, rc.Bottom), ToolStripDropDownDirection.BelowLeft);
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,01.11.2015</created>
        /// <changed>UPh,01.11.2015</changed>
        // ********************************************************************************
        void UpdateControls()
        {
            TermBaseFile file = lstFiles.SelectedItem as TermBaseFile;
            btnRemove.Enabled = (file != null);
            btnDisplayColor.Enabled = (file != null);
        }
    }
}
