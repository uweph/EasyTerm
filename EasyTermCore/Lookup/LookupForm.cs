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
    public partial class LookupForm : Form
    {
        TermBaseSet _TermbaseSet;
        List<TermInfoResultArgs> _Result;

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcTerm"></param>
        /// <param name="termbaseSet"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        /// <created>UPh,17.11.2015</created>
        /// <changed>UPh,17.11.2015</changed>
        // ********************************************************************************
        public LookupForm()
        {
            InitializeComponent();
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcTerm"></param>
        /// <param name="termbaseSet"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        /// <created>UPh,17.11.2015</created>
        /// <changed>UPh,17.11.2015</changed>
        // ********************************************************************************
        public void SetData(string srcTerm, TermBaseSet termbaseSet, List<TermInfoResultArgs> result)
        {
            _TermbaseSet = termbaseSet;
            _Result = result;

            txtSrcTerm.Text = srcTerm;
            FillCombo();

            if (cmbMatches.Items.Count > 0)
                cmbMatches.SelectedIndex = 0;
        }


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,17.11.2015</created>
        /// <changed>UPh,17.11.2015</changed>
        // ********************************************************************************
        private void FillCombo()
        {
            cmbMatches.Items.Clear();

            int n = 1;

            foreach (TermInfoResultArgs info in _Result)
            {
                string text = string.Format("{0}: {1} - {2}",
                    n++,
                    _TermbaseSet.GetDisplayName(info.TermBaseID),
                     info.Info.TermID);

                cmbMatches.Items.Add(text);
            }
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,17.11.2015</created>
        /// <changed>UPh,17.11.2015</changed>
        // ********************************************************************************
        private void cmbMatches_SelectedIndexChanged(object sender, EventArgs e)
        {
            int inx = cmbMatches.SelectedIndex;
            if (inx < 0 || inx >= _Result.Count)
                return;

            TermInfoResultArgs item = _Result[inx];

            string name = _TermbaseSet.GetDisplayName(item.TermBaseID);
            termInfoControl.SetData(name, item.Info);

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
