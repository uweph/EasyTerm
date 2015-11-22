using EasyTermCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyTermViewer
{
    internal class TerminologyListBox : ListView
    {
        internal TermBaseSet TermBaseSet { get; set; }

        List<TerminologyResultArgs> _Items;

        Brush _TextBrush;
        Brush _SelectedItemBrush;

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,22.11.2015</created>
        /// <changed>UPh,22.11.2015</changed>
        // ********************************************************************************
        public TerminologyListBox()
        {
            VirtualMode = true;
            FullRowSelect = true;
            OwnerDraw = true;

            _TextBrush = new SolidBrush(Color.Black);
            _SelectedItemBrush = new SolidBrush(Color.FromArgb(173, 214, 255));

            if (DesignMode)
                return;
        }



        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        internal void Initialize()
        {
            if (Columns.Count == 0)
            {
                Columns.Add("Range", 50);
                Columns.Add("Source", 100);
                Columns.Add("Translation", 100);
                Columns.Add("Origin", 100);
                Columns.Add("Description", 100);
            }

            FullRowSelect = true;

            ClearContent();

            UpdateDisplay();
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,20.11.2015</created>
        /// <changed>UPh,20.11.2015</changed>
        // ********************************************************************************
        internal void ClearContent()
        {
            if (_Items != null)
                _Items.Clear();
            UpdateDisplay();
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <created>UPh,20.11.2015</created>
        /// <changed>UPh,20.11.2015</changed>
        // ********************************************************************************
        internal void AddItem(TerminologyResultArgs item)
        {
            if (_Items == null)
                _Items = new List<TerminologyResultArgs>();

            _Items.Add(item);

            UpdateDisplay();
        }


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,20.11.2015</created>
        /// <changed>UPh,20.11.2015</changed>
        // ********************************************************************************
        void UpdateDisplay()
        {
            VirtualListSize = _Items != null ? _Items.Count : 0;
            _LVICache = null;
            Invalidate();
        }
        


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inx"></param>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        public TerminologyResultArgs GetItemAt(int inx)
        {
            if (inx < 0)
                return null;

            if (inx >= _Items.Count)
                return null;

            return _Items[inx];

        }

        // ********************************************************************************
        /// <summary>
        /// Returns the selected TermListItem
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,31.10.2015</created>
        /// <changed>UPh,31.10.2015</changed>
        // ********************************************************************************
        public TerminologyResultArgs GetSelectedItem()
        {
            if (SelectedIndices.Count == 0)
                return null;

            return GetItemAt(SelectedIndices[0]);
        }

        // ********************************************************************************
        /// <summary>
        /// Creates a ListViewItem at a position
        /// </summary>
        /// <param name="inx"></param>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        ListViewItem CreateLVItemAt(int inx)
        {
            ListViewItem item = new ListViewItem("");

            TerminologyResultArgs termitem = GetItemAt(inx);
            if (termitem != null)
            {
                item.SubItems[0].Text = string.Format("{0} - {1}", termitem.FindFrom, termitem.FindFrom + termitem.FindLen);
                item.SubItems.Add(termitem.Term1);
                item.SubItems.Add(termitem.Term2);
                item.SubItems.Add(termitem.Origin);
                item.SubItems.Add(termitem.Description);
            }

            return item;
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
        protected override void OnRetrieveVirtualItem(RetrieveVirtualItemEventArgs e)
        {

            if (_LVICache != null && e.ItemIndex >= _LVIFirstItem && e.ItemIndex < _LVIFirstItem + _LVICache.Length)
            {
                e.Item = _LVICache[e.ItemIndex - _LVIFirstItem];
            }
            else
            {
                e.Item = CreateLVItemAt(e.ItemIndex);
            }
        }

        ListViewItem[] _LVICache;
        int _LVIFirstItem;

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        protected override void OnCacheVirtualItems(CacheVirtualItemsEventArgs e)
        {
            //We've gotten a request to refresh the cache.
            //First check if it's really necessary.
            if (_LVICache != null && e.StartIndex >= _LVIFirstItem && e.EndIndex <= _LVIFirstItem + _LVICache.Length)
            {
                //If the newly requested cache is a subset of the old cache, 
                //no need to rebuild everything, so do nothing.
                return;
            }

            //Now we need to rebuild the cache.
            _LVIFirstItem = e.StartIndex;
            int length = e.EndIndex - e.StartIndex + 1; //indexes are inclusive
            _LVICache = new ListViewItem[length];

            //Fill the cache with the appropriate ListViewItems.
            for (int i = 0; i < length; i++)
            {
                _LVICache[i] = CreateLVItemAt(_LVIFirstItem + i);
            }

        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,21.11.2015</created>
        /// <changed>UPh,21.11.2015</changed>
        // ********************************************************************************
        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
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
        protected override void OnDrawItem(DrawListViewItemEventArgs e)
        {
            // All drawing is done in OnDrawSubItem
            return;

        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,22.11.2015</created>
        /// <changed>UPh,22.11.2015</changed>
        // ********************************************************************************
        protected override void OnDrawSubItem(DrawListViewSubItemEventArgs e)
        {
            if (e.Item.Selected)
            {
                e.Graphics.FillRectangle(_SelectedItemBrush, e.Bounds);
                //e.DrawFocusRectangle();
            }
            else
            {
                e.DrawBackground();
            }

            if (e.ColumnIndex == 3)
            {
                Color tbcolor = Color.Empty;
                if (TermBaseSet != null)
                {
                    TerminologyResultArgs item = GetItemAt(e.ItemIndex);
                    if (item != null)
                        tbcolor = TermBaseSet.GetDisplayColor(item.TermBaseID);
                }

                if (tbcolor != Color.Empty && tbcolor.ToArgb() != Color.White.ToArgb())
                {
                    SolidBrush brush = new SolidBrush(tbcolor);
                    Rectangle rcBar = e.Bounds;
                    rcBar.Width = 4;
                    e.Graphics.FillRectangle(brush, rcBar);
                }
            }

            e.Graphics.DrawString(e.SubItem.Text, Font, _TextBrush, e.Bounds.Left + 4.0f, e.Bounds.Top + 2.0f);
        }


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bNext"></param>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        internal void SelectNextItem(bool bNext)
        {
            if (VirtualListSize == 0)
                return;

            if (SelectedIndices.Count == 0)
            {
                SelectedIndices.Add(0);
                return;
            }

            int sel = SelectedIndices[0];

            if (bNext)
            {
                if (sel >= VirtualListSize - 1)
                    return;
                sel++;
            }
            else
            {
                if (sel <= 0)
                    return;
                sel--;
            }

            SelectedIndices.Clear();
            SelectedIndices.Add(sel);
            EnsureVisible(sel);
        }
    }
}
