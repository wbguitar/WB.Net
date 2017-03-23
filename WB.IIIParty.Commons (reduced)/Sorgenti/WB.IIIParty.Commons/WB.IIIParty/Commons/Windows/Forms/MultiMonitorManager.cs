// ------------------------------------------------------------------------
//Società: WB IIIParty
//Anno: 2008 
//Progetto: AMIL5
//Autore: Acquisti Leonardo
//Nome modulo software: WB.IIIParty.Commons.dll
//Data ultima modifica: $LastChangedDate: 2013-05-16 16:47:38 +0200 (gio, 16 mag 2013) $
//Versione: $Rev: 135 $
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WB.IIIParty.Commons.Windows.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public class MultiMonitorManager
    {
        // API: crea il device Context per il monitor di interesse
        [DllImport("Gdi32.dll")]
        private static extern IntPtr CreateDC(string strDriver, string strDevice, string strOutput, IntPtr pData);

        // struttura per la memorizzazione dei dati dei monitor 
        private struct DysplayDescription
        {
            public string DeviceName;
            public Rectangle RectDisplay;
            public string type;
            public Rectangle workArea;
            public int left;
            public int top;
            public int right;
            public int bottom;
            public bool prime;
        }

        // struttura per il salvataggio delle misure della form da visualizzare
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        // API: rileva le informazioni sulle dimensioni del monitor scelto rispetto al primo
        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, RECT mi);

        // API: sposta la form in base alle coordinate fornite
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
        private static extern void MoveWindow(IntPtr hwnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        // API: fornisce le dimensioni del riquadro della form di cui passiamo l'handle
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr hWnd, ref RECT rect);

        [DllImport("User32.dll")]
        private static extern int SwitchToThisWindow(IntPtr hWnd, bool fAltTab);
        
        [DllImport("User32.dll")]
        private static extern int SetActiveWindow(IntPtr hwnd);

        [DllImport("User32.dll")]
        private static extern int ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        static readonly IntPtr HWND_TOP = new IntPtr(0);
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        // From winuser.h
        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 SWP_NOZORDER = 0x0004;
        const UInt32 SWP_NOREDRAW = 0x0008;
        const UInt32 SWP_NOACTIVATE = 0x0010;
        const UInt32 SWP_FRAMECHANGED = 0x0020;  /* The frame changed: send WM_NCCALCSIZE */
        const UInt32 SWP_SHOWWINDOW = 0x0040;
        const UInt32 SWP_HIDEWINDOW = 0x0080;
        const UInt32 SWP_NOCOPYBITS = 0x0100;
        const UInt32 SWP_NOOWNERZORDER = 0x0200;  /* Don't do owner Z ordering */
        const UInt32 SWP_NOSENDCHANGING = 0x0400;  /* Don't send WM_WINDOWPOSCHANGING */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        public static void ShowWindow(IntPtr window)
        {
            SwitchToThisWindow(window, true);
            ShowWindow(window, 9);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        public static void ActiveWindow(IntPtr window)
        {
            SetActiveWindow(window);
        }
        const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public static bool WindowTopMost(IntPtr window)
        {
            return SetWindowPos(window, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        /// <param name="monitor"></param>
        /// <returns></returns>
        public static bool MoveWindow(IntPtr window, int monitor)
        {
            return MoveWindow(window, monitor, 0, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        /// <param name="monitor"></param>
        /// <returns></returns>
        public static bool MoveWindow(IntPtr window,int monitor,int x,int y)
        {
            IntPtr hdcScreen = CreateDC("", null, null, IntPtr.Zero);
            Screen[] screens = Screen.AllScreens;

            if (monitor >= screens.Length)
                return false;

            DysplayDescription ds = new DysplayDescription();
            DysplayDescription dsTmp = new DysplayDescription();

            DysplayDescription[] arrayDescription = new DysplayDescription[10];


            ds.DeviceName = screens[monitor].DeviceName;
            ds.RectDisplay = screens[monitor].Bounds;
            ds.type = screens[monitor].GetType().ToString();
            ds.workArea = screens[monitor].WorkingArea;
            ds.left = screens[monitor].WorkingArea.Left;
            ds.right = screens[monitor].WorkingArea.Right;
            ds.top = screens[monitor].WorkingArea.Top;
            ds.bottom = screens[monitor].WorkingArea.Bottom;
            ds.prime = screens[monitor].Primary;
            arrayDescription[monitor] = ds;
            hdcScreen = CreateDC(arrayDescription[monitor].DeviceName, null, null, IntPtr.Zero);
                dsTmp = arrayDescription[monitor];
            

            RECT Rect = new RECT();

            //Process current = Process.GetCurrentProcess();
            //current.WaitForInputIdle();
           
            GetWindowRect(window, ref Rect); 
            // passo l'handle della form da visualizzare
            //Console.WriteLine(this.newProcessServer.MainWindowHandle);
            // gli passo le dimensioni che vengono da monitor info 
            // che sono relaative al monitor su cui si deve visualizzare

            MoveWindow(window, (dsTmp.left + x), (dsTmp.top + y) , (Rect.right - Rect.left), (Rect.bottom - Rect.top), true);

            return true;
        }


    }
}
