using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyTermCore
{
    // --------------------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
    // --------------------------------------------------------------------------------
    public class TermBaseFileList : ListBox
    {
        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,29.08.2015</created>
        /// <changed>UPh,29.08.2015</changed>
        // ********************************************************************************
        public TermBaseFileList()
        {
            DrawMode = DrawMode.OwnerDrawFixed;
            ItemHeight *= 3;

            _BkBrush = new SolidBrush(Color.White);
            _BkBrushSel = new SolidBrush(Color.FromArgb(173, 214, 255));
            _FgBrush = new SolidBrush(Color.Black);

        }

        Font _BoldFont;
        Font _CheckBoxFont;
        SolidBrush _BkBrush;
        SolidBrush _BkBrushSel;
        SolidBrush _FgBrush;

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,29.08.2015</created>
        /// <changed>UPh,29.08.2015</changed>
        // ********************************************************************************
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (DesignMode)
                return;

            Graphics g = e.Graphics;


            if ((e.State & DrawItemState.Selected) != 0)
            {
                g.FillRectangle(_BkBrushSel, e.Bounds);
            }
            else
            {
                g.FillRectangle(_BkBrush, e.Bounds);
            }

            TermBaseFile file = null;

            if (e.Index >= 0)
                file = Items[e.Index] as TermBaseFile;

            if (_BoldFont == null)
                _BoldFont = new Font(e.Font, FontStyle.Bold);

            if (file != null)
            {
                DrawCheckBox(e, file);
                DrawText(e, file);
            }

            e.DrawFocusRectangle();
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inx"></param>
        /// <returns></returns>
        /// <created>UPh,29.08.2015</created>
        /// <changed>UPh,29.08.2015</changed>
        // ********************************************************************************
        private Rectangle GetCheckBoxRect(int inx)
        {
            Rectangle rc = GetItemRectangle(inx);

            rc.X += 2;
            rc.Y += 2;
            rc.Width = 18;
            rc.Height = 18;

            return rc;
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <created>UPh,29.08.2015</created>
        /// <changed>UPh,29.08.2015</changed>
        // ********************************************************************************
        private void DrawCheckBox(DrawItemEventArgs e, TermBaseFile file)
        {
            if (_CheckBoxFont == null)
                _CheckBoxFont = new Font("Arial", 14);

            Rectangle rcCheck = GetCheckBoxRect(e.Index);

            Graphics g = e.Graphics;

            g.DrawString(file.Active ? "\u2611" : "\u2610",
                _CheckBoxFont, _FgBrush, rcCheck);

            


        }


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="g"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <created>UPh,29.08.2015</created>
        /// <changed>UPh,29.08.2015</changed>
        // ********************************************************************************
        private void DrawText(DrawItemEventArgs e, TermBaseFile file)
        {
            Rectangle rcText = e.Bounds;
            Graphics g = e.Graphics;

            rcText.X += 24;
            rcText.Y += 2;

            string name = Path.GetFileName(file.FilePath);
            string dir = Path.GetDirectoryName(file.FilePath);

            SizeF size = g.MeasureString(name, _BoldFont);

            g.DrawString(name, _BoldFont, _FgBrush, rcText);

            rcText.Y += (int)(size.Height + 2);
            g.DrawString(dir, e.Font, _FgBrush, rcText);
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,29.08.2015</created>
        /// <changed>UPh,29.08.2015</changed>
        // ********************************************************************************
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            HitPosition hit;
            int inx = HitTest(e.Location, out hit);
            if (inx < 0)
                return;

            if (hit == HitPosition.CheckBox)
            {
                TermBaseFile file = Items[inx] as TermBaseFile;
                if (file != null)
                {
                    file.Active = !file.Active;

                    // Raise Event without args
                    if (ActiveChanged != null)
                        ActiveChanged(this, new ActiveChangedEventArgs(file));


                    Invalidate(GetCheckBoxRect(inx));
                }
            }
        }

        enum HitPosition {None, Item, CheckBox, DeleteButton};

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="hit"></param>
        /// <returns></returns>
        /// <created>UPh,29.08.2015</created>
        /// <changed>UPh,29.08.2015</changed>
        // ********************************************************************************
        int HitTest(Point pt, out HitPosition hit)
        {
            hit = HitPosition.None;

            for (int i = TopIndex; i < Items.Count; i++)
            {
                Rectangle rc = GetItemRectangle(i);
                if (!rc.Contains(pt))
                    continue;
                hit = HitPosition.Item;

                if (GetCheckBoxRect(i).Contains(pt))
                    hit = HitPosition.CheckBox;

                return i;
            }
            return -1;
        }


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        /// <created>UPh,29.08.2015</created>
        /// <changed>UPh,29.08.2015</changed>
        // ********************************************************************************
        internal void FillList(TermBaseSet set)
        {
            Items.Clear();
            
            Items.AddRange(set.Files.ToArray());            
        }

        internal delegate void ActiveChangedHandler(object sender, ActiveChangedEventArgs e);
        internal event ActiveChangedHandler ActiveChanged;
    }

    internal class ActiveChangedEventArgs : EventArgs
    {
        public TermBaseFile File{ get; set; }

        public ActiveChangedEventArgs(TermBaseFile file)
        {
            File = file;
        }
    }
}