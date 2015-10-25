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
            InitializeComponent();
            _TermbaseSet = set;

            lstFiles.FillList(_TermbaseSet);
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

        private void lstFiles_ActiveChanged(object sender, ActiveChangedEventArgs file)
        {
            _DataChanged = true;
        }



    }


}
