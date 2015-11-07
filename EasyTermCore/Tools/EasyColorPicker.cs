using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace EasyTermCore
{
    public partial class EasyColorPicker : UserControl
    {
        // Base measures
        int _MeasureGap = 4;
        int _MeasureX0 = -1;
        int _MeasureY0 = -1;
        int _FieldWidth = -1;
        int _FieldHeight = -1;

        Pen _FieldPen;
        Pen _FieldPenHilight;


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,26.04.2015</created>
        /// <changed>UPh,26.04.2015</changed>
        // ********************************************************************************
        public EasyColorPicker()
        {
            InitializeComponent();

            _MeasureGap = 4;
            _MeasureX0 = lblColors.ClientRectangle.Left + 4;
            _MeasureY0 = lblColors.ClientRectangle.Top + 4;

            _FieldWidth  = ((lblColors.ClientRectangle.Width - 8) - (7 * _MeasureGap)) / 8;
            _FieldHeight = ((lblColors.ClientRectangle.Height - 8) - (7 * _MeasureGap)) / 8;

            _FieldPen = new Pen(Color.Black);
            _FieldPenHilight = new Pen(Color.DarkBlue, 3);
        }




        public delegate void ColorSelectedHandler(object sender, ColorSelectedEventArgs e);
        public event ColorSelectedHandler ColorSelected;

        private void button_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null || btn.Tag == null)
                return;

            Color color = (Color)btn.Tag;


            ToolStripDropDown host = Parent as ToolStripDropDown;
            if (host != null)
                host.Close();

            if (ColorSelected != null)
                ColorSelected(this, new ColorSelectedEventArgs(color));
        }


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="p"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        /// <created>UPh,25.04.2015</created>
        /// <changed>UPh,25.04.2015</changed>
        // ********************************************************************************
        public GraphicsPath GetRoundRect(int x, int y, int width, int height, int radius)
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddArc(x + width - (radius * 2), y, radius * 2, radius * 2, 270, 90); 
            gp.AddArc(x + width - (radius * 2), y + height - (radius * 2), radius * 2, radius * 2, 0, 90); 
            gp.AddArc(x, y + height - (radius * 2), radius * 2, radius * 2, 90, 90); 
            gp.AddArc(x, y, radius * 2, radius * 2, 180, 90); 
            gp.CloseFigure();

            return gp;

        }


        /// <summary></summary>
        static Color[]ColorPalette = new Color[8 * 8]
        {
            Color.FromArgb(0,0,0),       Color.FromArgb(68,68,68),    Color.FromArgb(102,102,102), Color.FromArgb(153,153,153), Color.FromArgb(170,170,187), Color.FromArgb(238,238,238), Color.FromArgb(243,243,243), Color.FromArgb(255,255,255),
                                                                  
            Color.FromArgb(255,0,0),     Color.FromArgb(255,153,0),   Color.FromArgb(255,255,0),   Color.FromArgb(0,255,0),     Color.FromArgb(0,255,255),   Color.FromArgb(0,0,255),     Color.FromArgb(153,0,255),   Color.FromArgb(255,0,255),

            Color.FromArgb(244,204,204), Color.FromArgb(252,229,205), Color.FromArgb(255,242,204), Color.FromArgb(217,234,211), Color.FromArgb(208,224,227), Color.FromArgb(207,226,243), Color.FromArgb(217,210,233), Color.FromArgb(234,209,220),
            Color.FromArgb(234,153,153), Color.FromArgb(249,203,156), Color.FromArgb(255,229,153), Color.FromArgb(182,215,168), Color.FromArgb(162,196,201), Color.FromArgb(159,197,232), Color.FromArgb(180,167,214), Color.FromArgb(213,166,189),
            Color.FromArgb(224,102,102), Color.FromArgb(246,178,107), Color.FromArgb(255,217,102), Color.FromArgb(147,196,125), Color.FromArgb(118,165,175), Color.FromArgb(111,168,220), Color.FromArgb(142,124,195), Color.FromArgb(194,123,160),
            Color.FromArgb(204,0,0),     Color.FromArgb(230,145,56),  Color.FromArgb(241,194,50),  Color.FromArgb(106,168,79),  Color.FromArgb(69,129,142),  Color.FromArgb(61,133,198),  Color.FromArgb(103,78,167),  Color.FromArgb(166,77,121), 
            Color.FromArgb(153,0,0),     Color.FromArgb(180,95,6),    Color.FromArgb(191,144,0),   Color.FromArgb(56,118,29),   Color.FromArgb(19,79,92),    Color.FromArgb(11,83,148),   Color.FromArgb(53,28,117),   Color.FromArgb(116,27,71),
            Color.FromArgb(102,0,0),     Color.FromArgb(120,63,4),    Color.FromArgb(127,96,0),    Color.FromArgb(39,78,19),    Color.FromArgb(12,52,61),    Color.FromArgb(7,55,99),     Color.FromArgb(32,18,77),    Color.FromArgb(76,17,48),
        };



        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,25.04.2015</created>
        /// <changed>UPh,25.04.2015</changed>
        // ********************************************************************************
        private void linkMore_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (ColorDialog dlg = new ColorDialog())
            {
                dlg.FullOpen = true;
                if (dlg.ShowDialog() != DialogResult.OK)
                    return;


                ToolStripDropDown host = Parent as ToolStripDropDown;
                if (host != null)
                    host.Close();

                if (ColorSelected != null)
                    ColorSelected(this, new ColorSelectedEventArgs(dlg.Color));
            }

        }

        int _iHilightRow = -1;
        int _iHilightCol = -1;
        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        /// <created>UPh,26.04.2015</created>
        /// <changed>UPh,26.04.2015</changed>
        // ********************************************************************************
        void HilightField(int col, int row)
        {
            if (_iHilightCol == col && _iHilightRow == row)
                return;

            if (_iHilightCol != -1 && _iHilightRow != -1)
            {
                Rectangle rc = GetFieldRect(_iHilightCol, _iHilightRow);
                rc.Inflate(2,2);
                lblColors.Invalidate(rc);
            }

            _iHilightCol = col;
            _iHilightRow = row;

            if (_iHilightCol != -1 && _iHilightRow != -1)
            {
                Rectangle rc = GetFieldRect(_iHilightCol, _iHilightRow);
                rc.Inflate(2,2);
                lblColors.Invalidate(rc);
            }
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,25.04.2015</created>
        /// <changed>UPh,25.04.2015</changed>
        // ********************************************************************************
        private void lblColors_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            for (int i = 0; i < 64; i++)
            {
                int col = i % 8;
                int row = i / 8;

                bool bHilight = (col == _iHilightCol && row == _iHilightRow);

                int x = _MeasureX0 + col * (_FieldWidth  + _MeasureGap);
                int y = _MeasureY0 + row * (_FieldHeight + _MeasureGap);

                using (GraphicsPath path = GetRoundRect(x, y, _FieldWidth, _FieldHeight, 3))
                {
                    Color color = ColorPalette[i];
                    SolidBrush brush = new SolidBrush(color);
                    e.Graphics.FillPath(brush, path);

                    e.Graphics.DrawPath(bHilight ? _FieldPenHilight : _FieldPen, path);
                }
            }
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,26.04.2015</created>
        /// <changed>UPh,26.04.2015</changed>
        // ********************************************************************************
        private void lblColors_MouseMove(object sender, MouseEventArgs e)
        {
            int col, row;
            if (HitTest(e.Location, out col, out row))
            {
                HilightField(col, row);
            }
            else
            {
                HilightField(-1, -1);
            }
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,26.04.2015</created>
        /// <changed>UPh,26.04.2015</changed>
        // ********************************************************************************
        private void lblColors_MouseLeave(object sender, EventArgs e)
        {
            HilightField(-1, -1);
        }


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,25.04.2015</created>
        /// <changed>UPh,25.04.2015</changed>
        // ********************************************************************************
        private void lblColors_MouseClick(object sender, MouseEventArgs e)
        {
            int col, row;
            if (HitTest(e.Location, out col, out row))
            {
                ToolStripDropDown host = Parent as ToolStripDropDown;
                if (host != null)
                    host.Close();

                if (ColorSelected != null)
                    ColorSelected(this, new ColorSelectedEventArgs(ColorPalette[8 * row + col]));
            }
        }


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        /// <created>UPh,26.04.2015</created>
        /// <changed>UPh,26.04.2015</changed>
        // ********************************************************************************
        Rectangle GetFieldRect(int col, int row)
        {
            return new Rectangle(
                _MeasureX0 + col * (_FieldWidth + _MeasureGap),        
                _MeasureY0 + row * (_FieldHeight + _MeasureGap),
                _FieldWidth,
                _FieldHeight);
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        /// <created>UPh,26.04.2015</created>
        /// <changed>UPh,26.04.2015</changed>
        // ********************************************************************************
        bool HitTest(Point pt, out int col, out int row)
        {
            row = -1;
            col = -1;


            int col0 = (pt.X - _MeasureX0) / (_FieldWidth + _MeasureGap);
            int row0 = (pt.Y - _MeasureY0) / (_FieldHeight + _MeasureGap);

            if (col0 < 0 || col0 >= 8 || row0 < 0 || row0 >= 8)
                return false;

            row = row0;
            col = col0;
            return true;
        }

    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
    // --------------------------------------------------------------------------------
    public class ColorSelectedEventArgs : EventArgs
    {
        public Color Color { get; private set; }
        public ColorSelectedEventArgs(Color color)
        {
            Color = color;
        }
    }
}
