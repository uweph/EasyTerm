using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace PassLib
{
    internal class PlStorePosition
    {
        //********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,27.12.2007</created>
        /// <changed>UPh,27.12.2007</changed>
        //********************************************************************************
        internal PlStorePosition()
        {
                
        }

        // Form und Kontrollen, deren Positionen gespeichert werden sollen
        private Control m_Container;     // Form oder UserControl, das bei Initialize übergeben wird
        private Form m_RefForm = null;
        private bool m_bHasRefForm = false;
        private List<Control> m_Controls = new List<Control>(); 
        
        //********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        /// <created>UPh,27.12.2007</created>
        /// <changed>UPh,27.12.2007</changed>
        //********************************************************************************
        internal void Initialize(Form form, bool bStorePosition)
        {
            m_Container = form;
            
            form.Load   += OnLoad;
            form.Closed += OnClose;
            
            if (bStorePosition)
                m_Controls.Add(form);
        }  

        //********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="usercontrol"></param>
        /// <returns></returns>
        /// <created>UPh,27.12.2007</created>
        /// <changed>UPh,27.12.2007</changed>
        //********************************************************************************
        internal void Initialize(UserControl usercontrol)
        {
            m_Container = usercontrol;
            usercontrol.Load += OnLoad;
        }
        
        //********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        /// <created>UPh,27.12.2007</created>
        /// <changed>UPh,27.12.2007</changed>
        //********************************************************************************
        internal void Initialize(Form form)
        {
            Initialize(form, true);
        }

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        //--------------------------------------------------------------------------------
        internal Form RefForm
        {
            get { return m_RefForm; }
            set { m_RefForm = value; m_bHasRefForm = true;}
        }
        
        //********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="splitContainer1"></param>
        /// <returns></returns>
        /// <created>UPh,27.12.2007</created>
        /// <changed>UPh,27.12.2007</changed>
        //********************************************************************************
        internal void Add(Control c)
        {
            m_Controls.Add(c);
        }

		//********************************************************************************
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		/// <created>UPh,27.12.2007</created>
		/// <changed>UPh,27.12.2007</changed>
		//********************************************************************************
		private void OnLoad(object sender, System.EventArgs e)
        {
            Form form = null;
            
            if (m_Container is Form)
            {
                form = (Form) m_Container;

                if (!m_bHasRefForm)
                {
                    if (form.Parent != null)
                        m_RefForm = null; // Koordinaten sind immer relativ
                    else if (form.Owner != null)
                        m_RefForm = form.Owner;
                    else if (Form.ActiveForm != null)
                        m_RefForm = Form.ActiveForm;
                    m_bHasRefForm = true;
                }
            }
            else if (m_Container is UserControl)
            {
                // Bei UserControls wird jetzt die OnClose-Funktion verbunden
                form = ((UserControl) m_Container).ParentForm;
                if (form == null)
                    return;
                    
                form.Closed += OnClose;
            }
            
            if (form == null)
                return;
                
            form.SuspendLayout();
            foreach (Control c in m_Controls)
            {
                if (c is Form)
                    RestorePosition((Form) c, m_RefForm);
                else if (c is SplitContainer)
                    RestorePosition((SplitContainer) c);
                else if (c is ListView)
                    RestorePosition((ListView)c);
            }
            form.ResumeLayout();
        }

        //********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <created>UPh,27.12.2007</created>
        /// <changed>UPh,27.12.2007</changed>
        //********************************************************************************
        private void OnClose(object sender, System.EventArgs e)
        {
            foreach (Control c in m_Controls)
            {
                if (c is Form)
                    SavePosition((Form)sender, m_RefForm);
                else if (c is SplitContainer)
                    SavePosition((SplitContainer)c);
                else if (c is ListView)
                    SavePosition((ListView)c);
            }
        }
        
        //********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        /// <created>UPh,27.12.2007</created>
        /// <changed>UPh,27.12.2007</changed>
        //********************************************************************************
        static string MakeControlName(Control c)
        {
            string name = c.Name;
            for (Control parent = c.Parent; parent != null; parent = parent.Parent)
            {
                name = parent.Name + "." + name;
                if (parent is Form)
                    break;
            }
            return name;
        }
        
        //********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strPos"></param>
        /// <returns></returns>
        /// <created>UPh,27.12.2007</created>
        /// <changed>UPh,27.12.2007</changed>
        //********************************************************************************
        static private int [] GetIntArray(string strPos)
        {
            string [] arr = strPos.Split(',');
            int size = arr.Length;
            if (size == 0)
                return null;
            
            int [] intarr = new int[size];
            for (int i = 0; i < size; i++)
            {
                int val;
                if (!Int32.TryParse(arr[i], out val))
                    return null;
                intarr[i] = val;
            }
            return intarr;
        }   

        #region Form
        //********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <param name="refform"></param>
        /// <returns></returns>
        /// <created>UPh,27.12.2007</created>
        /// <changed>UPh,27.12.2007</changed>
        //********************************************************************************
        static internal void RestorePosition(Form form, Form refform)
        {
            string strPos = PlProfile.GetString("Pos", form.Name);
            int[]arr = GetIntArray(strPos);
            if (arr == null)
                return;
                
            if (arr.Length != 4 && arr.Length != 6)
                return;

            int left = arr[0];
            int top = arr[1];
            int width = arr[2];
            int height = arr[3];
            if (form.Parent == null && arr.Length == 6 && refform != null && refform.WindowState != FormWindowState.Minimized)
            {
                int dx = arr[4];
                int dy = arr[5];
                left = refform.Left + dx;
                top = refform.Top + dy;
            }
            form.Left = left;
            form.Top = top;
            if (form.FormBorderStyle == FormBorderStyle.Sizable ||
                form.FormBorderStyle == FormBorderStyle.SizableToolWindow)
            {
                form.Width = width;
                form.Height = height;
            }

            MakeVisible(form);
        }
        

        //********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        /// <created>UPh,27.12.2007</created>
        /// <changed>UPh,27.12.2007</changed>
        //********************************************************************************
        static internal void MakeVisible(Form form)
        {
            Rectangle rcBounds;

            if (form.Parent != null)
            {
                rcBounds = form.Parent.ClientRectangle;
            }
            else
            {
                Screen screen = Screen.FromControl(form);
                if (screen == null)
                    screen = Screen.PrimaryScreen;
                if (screen == null)
                    return;

                rcBounds = screen.Bounds;
            }

            Rectangle rcNewWnd = new Rectangle(form.Left, form.Top, form.Width, form.Height);

            if (rcNewWnd.Right > rcBounds.Right)
                rcNewWnd.Offset(rcBounds.Right - rcNewWnd.Right, 0);
            if (rcNewWnd.Bottom > rcBounds.Bottom)
                rcNewWnd.Offset(0, rcBounds.Bottom - rcNewWnd.Bottom);
            if (rcNewWnd.Left < rcBounds.Left)
                rcNewWnd.Offset(rcBounds.Left - rcNewWnd.Left, 0);
            if (rcNewWnd.Top < rcBounds.Top)
                rcNewWnd.Offset(0, rcBounds.Top - rcNewWnd.Top);

            form.SetDesktopBounds(rcNewWnd.Left, rcNewWnd.Top, rcNewWnd.Width, rcNewWnd.Height);
        }
        

        //********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <param name="refform"></param>
        /// <returns></returns>
        /// <created>UPh,27.12.2007</created>
        /// <changed>UPh,27.12.2007</changed>
        //********************************************************************************
        static void SavePosition(Form form, Form refform)
        {
            if (form.WindowState == FormWindowState.Minimized ||
                form.WindowState == FormWindowState.Maximized)
                return;

            int x = form.Left;
            int y = form.Top;
            int cx = form.Width;
            int cy = form.Height;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0},{1},{2},{3}", x, y, cx, cy);

            // Immer möglichst die relative Position mit abspeichern
            if (form.Parent == null && refform != null && refform.WindowState != FormWindowState.Minimized)
            {
                int dx = x - refform.Left;
                int dy = y - refform.Top;
                sb.AppendFormat(",{0},{1}", dx, dy);
            }

            PlProfile.WriteString("Pos", form.Name, sb.ToString());
        }
        #endregion

        #region SplitContainer
        //********************************************************************************
        /// <summary>               
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        /// <created>UPh,27.12.2007</created>
        /// <changed>UPh,27.12.2007</changed>
        //********************************************************************************
        static internal void RestorePosition(SplitContainer c)
        {
            int max = 0;
            if (c.Orientation == Orientation.Horizontal)
                max = c.Height;
            else
                max = c.Width;
            if (max <= 1)
                return;

            string strPos = PlProfile.GetString("Pos", MakeControlName(c));
            int pos;
            if (!Int32.TryParse(strPos, out pos) || pos < 0 || pos > 10000)
                return;
                
            c.SplitterDistance = (pos * max) / 10000;
        }

        //********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        /// <created>UPh,27.12.2007</created>
        /// <changed>UPh,27.12.2007</changed>
        //********************************************************************************
        static void SavePosition(SplitContainer c)
        {
            int max = 0;
            if (c.Orientation == Orientation.Horizontal)
                max = c.Height;
            else
                max = c.Width;
            if (max <= 1)
                return;
                
            int percent = (c.SplitterDistance * 10000) / max;
            PlProfile.WriteString("Pos", MakeControlName(c), percent.ToString());
            
        }
        #endregion

        #region ListView
        //********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        /// <created>UPh,27.12.2007</created>
        /// <changed>UPh,27.12.2007</changed>
        //********************************************************************************
        static internal void RestorePosition(ListView c)
        {
            string strPos = PlProfile.GetString("Pos", MakeControlName(c));
            int[] arr = GetIntArray(strPos);
            if (arr == null)
                return;
                
            if (arr.Length != c.Columns.Count)
                return;
                
            for (int i = 0; i < arr.Length; i++)
                c.Columns[i].Width = arr[i];    
        }

        //********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        /// <created>UPh,27.12.2007</created>
        /// <changed>UPh,27.12.2007</changed>
        //********************************************************************************
        static void SavePosition(ListView c)
        {
            StringBuilder strPos = new StringBuilder();
            
            for (int i = 0; i < c.Columns.Count; i++)
            {
                if (i > 0)
                    strPos.Append(",");
                strPos.AppendFormat("{0}", c.Columns[i].Width);
            }
            
            PlProfile.WriteString("Pos", MakeControlName(c), strPos.ToString());

        }
        #endregion
    }
}
