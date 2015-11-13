using EasyTermCore.Properties;
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
            DrawMode = DrawMode.OwnerDrawVariable;

            _BkBrush = new SolidBrush(Color.White);
            _BkBrushSel = new SolidBrush(Color.FromArgb(173, 214, 255));
            _FgBrush = new SolidBrush(Color.Black);
            _FgBrushError = new SolidBrush(Color.Red);
        }

        Font _BoldFont;
        SolidBrush _BkBrush;
        SolidBrush _BkBrushSel;
        SolidBrush _FgBrush;
        SolidBrush _FgBrushError;
        int _TextHeight = 0;

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,07.11.2015</created>
        /// <changed>UPh,07.11.2015</changed>
        // ********************************************************************************
        protected override void OnMeasureItem(MeasureItemEventArgs e)
        {
            if (DesignMode)
                return;

            if (_TextHeight == 0)
            {
                SizeF size = e.Graphics.MeasureString("X", Font);
                _TextHeight = (int) (2 + size.Height);
            }

            if (e.Index >= 0)
            {
                TermBaseFile file = Items[e.Index] as TermBaseFile;

                if (!string.IsNullOrEmpty(file.OpenError))
                    e.ItemHeight = 5 + 3 * _TextHeight;
                else
                    e.ItemHeight = 5 + 2 * _TextHeight;

            }

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
                Rectangle rcColor = e.Bounds;
                rcColor.Width = 4;
                SolidBrush brush = new SolidBrush(file.DisplayColor);
                e.Graphics.FillRectangle(brush, rcColor);


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

            rc.X += 8;
            rc.Y += 3;
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
            Rectangle rcCheck = GetCheckBoxRect(e.Index);

            Graphics g = e.Graphics;
            g.DrawImage(file.Active ? Resources.ImgageCheck : Resources.ImageUnCheck, rcCheck.Left, rcCheck.Top);
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

            using (Region rgnClip = new Region(e.Bounds))
            {
                g.Clip = rgnClip;

                rcText.X += 30;
                rcText.Y += 2;

                bool bError = !string.IsNullOrEmpty(file.OpenError);

                string name = Path.GetFileName(file.FilePath);
                string dir = Path.GetDirectoryName(file.FilePath);

                SizeF size = g.MeasureString(name, _BoldFont);

                g.DrawString(name, _BoldFont, _FgBrush, rcText);

                rcText.Y += (int)(size.Height + 2);
                g.DrawString(dir, e.Font, _FgBrush, rcText);

                if (bError)
                {
                    rcText.Y += (int)(size.Height + 2);
                    g.DrawString(file.OpenError, e.Font, _FgBrushError, rcText);
                }

                g.ResetClip();
            }
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