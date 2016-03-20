using EasyTermCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EasyTermViewer.Properties;

namespace EasyTermViewer
{
    internal class TermListBox : ListView
    {
        internal TermBaseSet TermBaseSet { get; set; }

        TermListItems _Items;
        int[] _Filter;
        int _FilterSize = -1;

        Brush _TextBrush;
        Brush _SelectedItemBrush;

        public TermListBox()
        {
            VirtualMode = true;
            FullRowSelect = true;
            OwnerDraw = true;
            _TextBrush = new SolidBrush(Color.Black);
            _SelectedItemBrush = new SolidBrush(Color.FromArgb(173, 214, 255));
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
        internal void Initialize(TermListItems items)
        {
            if (Columns.Count == 0)
                Columns.Add("Term");
            Columns[0].Width = 200;

            FullRowSelect = true;

            _Items = items;
            _Filter = null;
            _FilterSize = -1;

            UpdateDisplay();
        }


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,21.11.2015</created>
        /// <changed>UPh,21.11.2015</changed>
        // ********************************************************************************
        internal void ClearContent()
        {
            if (_Items != null)
                _Items.Clear();
            UpdateDisplay();
        }
        

        // ********************************************************************************
        /// <summary>
        /// Split a string into parts
        /// </summary>
        /// <typeparam name="String"></typeparam>
        /// <param name="line">string to split</param>
        /// <param name="delimiter">character used as delimiter</param>
        /// <param name="textQualifier">character used as text qualifier. (keep words together even if they contain the delimiter)</param>
        /// <returns></returns>
        /// <created>UPh,04.11.2015</created>
        /// <changed>UPh,22.11.2015</changed>
        // ********************************************************************************
        public static List<String> SplitToParts(String line, Char delimiter, Char textQualifier)
        {
            List<string> parts = new List<string>();

            if (line == null)
                return parts;

            Char prevChar = '\0';
            Char nextChar = '\0';
            Char currentChar = '\0';

            Boolean inString = false;

            StringBuilder token = new StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                currentChar = line[i];

                if (i > 0)
                    prevChar = line[i - 1];
                else
                    prevChar = '\0';

                if (i + 1 < line.Length)
                    nextChar = line[i + 1];
                else
                    nextChar = '\0';

                if (currentChar == textQualifier && (prevChar == '\0' || prevChar == delimiter) && !inString)
                {
                    inString = true;
                    continue;
                }

                if (currentChar == textQualifier && (nextChar == '\0' || nextChar == delimiter) && inString)
                {
                    inString = false;
                    continue;
                }

                if (currentChar == delimiter && !inString)
                {
                    parts.Add(token.ToString());
                    token = token.Remove(0, token.Length);
                    continue;
                }

                token = token.Append(currentChar);

            }

            parts.Add(token.ToString());

            return parts;
        }


        // ********************************************************************************
        /// <summary>
        /// Collect all words that contain a given filter text
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        internal void Filter(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                _FilterSize = -1;
                UpdateDisplay();
                return;
            }

            // Create filter array with same size as item array
            if (_Filter == null)
                _Filter = new int[_Items.Count];

            // Start with empty filter
            _FilterSize = 0;

            // Split to parts
            var parts = SplitToParts(word, ' ', '\"');

            for (int i = 0; i < _Items.Count; i++)
            {
                TermListItem item = _Items[i];

                bool bFound = true;

                foreach (string part in parts)
                {
                    if (item.Term.IndexOf(part, StringComparison.InvariantCultureIgnoreCase) < 0)
                    {
                        bFound = false;
                        break;
                    }
                }

                if (bFound)
                    _Filter[_FilterSize++] = i;
            }


            UpdateDisplay();
        }

        // ********************************************************************************
        /// <summary>
        /// Updates the virtual list 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        void UpdateDisplay()
        {
            if (_Filter != null && _FilterSize >= 0)
                VirtualListSize = _FilterSize;
            else
                VirtualListSize = _Items == null ? 0 : _Items.Count;

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
        public TermListItem GetItemAt(int inx)
        {
            if (inx < 0)
                return null;

            if (_Filter != null && _FilterSize >= 0)
            {
                if (inx >= _FilterSize)
                    return null;

                inx = _Filter[inx];
            }

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
        public TermListItem GetSelectedItem()
        {
            if (SelectedIndices.Count == 0)
                return null;

            return GetItemAt(SelectedIndices[0]);
        }

        // ********************************************************************************
        /// <summary>
        /// Creates a ListViewItem at a position
        /// </summary>
        /// <param name="inx">Position index</param>
        /// <returns>new ListViewItem</returns>
        /// <created>UPh,25.10.2015</created>
        /// <changed>UPh,25.10.2015</changed>
        // ********************************************************************************
        ListViewItem CreateLVItemAt(int inx)
        {
            ListViewItem item = new ListViewItem("");

            TermListItem termitem = GetItemAt(inx);
            if (termitem != null)
            {
                item.SubItems[0].Text = termitem.Term;
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
        /// <created>UPh,31.10.2015</created>
        /// <changed>UPh,31.10.2015</changed>
        // ********************************************************************************
        protected override void OnDrawItem(DrawListViewItemEventArgs e)
        {
            if (e.Item.Selected)
                e.Graphics.FillRectangle(_SelectedItemBrush, e.Bounds);
            else
                e.DrawBackground();

            TermListItem item = GetItemAt(e.ItemIndex);

            Color tbcolor = Color.Empty;
            if (TermBaseSet != null)
            {
                if (item != null)
                    tbcolor = TermBaseSet.GetDisplayColor(item.TermBaseID);
            }

            if (tbcolor != Color.Empty)
            {
                SolidBrush brush = new SolidBrush(tbcolor);
                Rectangle rcBar = e.Bounds;
                rcBar.Width = 4;
                e.Graphics.FillRectangle(brush, rcBar);
            }

            Rectangle rect = e.Bounds;
            rect.X += 4;
            rect.Y += 2;

            if (item != null && item.Status == TermStatus.prohibited)
            {
                e.Graphics.DrawImage(Resources.Prohibited_sm, rect.Left, rect.Top);
                rect.X += 12;
            }

            e.Graphics.DrawString(e.Item.Text, Font, _TextBrush, rect.X, rect.Y);

            e.DrawFocusRectangle();
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
