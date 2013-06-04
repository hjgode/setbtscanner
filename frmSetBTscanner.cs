using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace setBTscanner
{
    public partial class setBTscanner : Form
    {
        private EventHandler m_deleInfo;
        const string sKeyWedgeFullName = @"\Windows\KeyWedge.exe";
        const string sKeyWedgeName = "KeyWedge.exe";
        bool bAutoClose = false;

        System.Threading.Thread threadConnect=null;
        System.Threading.Thread threadCloseApp=null;
        int delayConnect = 2000;
        int delayAutoClose = 2000;

        public setBTscanner()
        {
            InitializeComponent();
            
            m_deleInfo = new EventHandler(loginfoCallback);

            log = new logging(ref textBox1, m_deleInfo);

            log.WriteLog("+++++ START +++++");
            
        }

        public setBTscanner(string sBT)
        {
            InitializeComponent();

            _sBT = sBT;
            txtBTmacAddress.Text = _sBT;

            m_deleInfo = new EventHandler(loginfoCallback); //calback delegate for log display

            log = new logging(ref textBox1, m_deleInfo);

            log.WriteLog("+++++ START +++++");
            bAutoClose = true;

            threadConnect = new System.Threading.Thread(connectthread);
            threadConnect.Start();
        }

        void connectthread()
        {
            System.Diagnostics.Debug.WriteLine("+++ connectthread started");
            object[] myArray = new object[1];
            myArray[0]=false;
            try
            {
                this.BeginInvoke(new ButtonDelegate(EnableButton), myArray);
                System.Threading.Thread.Sleep(delayConnect);
                doConnect();
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine("### connectthread Exception: " + ex.Message);
            }
            myArray[0] = true;
            this.BeginInvoke(new ButtonDelegate(EnableButton), myArray);
            System.Diagnostics.Debug.WriteLine("--- connectthread ended");
        }
        
        void closethread()
        {
            System.Diagnostics.Debug.WriteLine("closethread started");
            try{
                System.Threading.Thread.Sleep(delayAutoClose);
                this.BeginInvoke(new InvokeDelegate(CloseMe));
                //this.Close();
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine("### closethread Exception: " + ex.Message); 
            }
            System.Diagnostics.Debug.WriteLine("--- closethread ended");
        }

        public delegate void InvokeDelegate();
        public void CloseMe()
        {
            this.Close();
        }

        public delegate void ButtonDelegate(bool bEnable);
        public void EnableButton(bool bEnable)
        {
            if(bEnable)
                Cursor.Current = Cursors.Default;
            else
                Cursor.Current = Cursors.WaitCursor;
            btnConnect.Enabled = bEnable;
            txtBTmacAddress.Enabled = bEnable;
        }

        BTdevice btdev;
        logging log;
        string _sBT = "";

        void doConnect()
        {
            killKeyWedge();
            Application.DoEvents();

            btdev = new BTdevice(ref log);

            
            log.WriteLog("Using BT MAC: '" + _sBT + "'");

            if (btdev.DoAssociation(_sBT))
                log.WriteLog("Association OK");
            else
                log.WriteLog("Association FAILED");

            btdev.Dispose();
            btdev = null;

            if (bAutoClose)
            {
                threadCloseApp = new System.Threading.Thread(closethread);
                threadCloseApp.Start();
            }
            Application.DoEvents();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            btnConnect.Enabled = false;
            Cursor.Current = Cursors.WaitCursor;
            Application.DoEvents();

#if DEBUG
            if (_sBT.Length != 12)
                _sBT = "0023686E70BC";
#endif
            txtBTmacAddress.Text = _sBT;

            if (txtBTmacAddress.Text.Length == 12)
                _sBT = txtBTmacAddress.Text; 
            doConnect();

            btnConnect.Enabled = true;
            Cursor.Current = Cursors.Default;
            Application.DoEvents();

        }

        /// <summary>
        /// StartupCallback - Interthread delegate.
        /// </summary>
        /// <param name="sender">unused</param>
        /// <param name="e">unused</param>
        private void loginfoCallback(object sender, System.EventArgs e)
        {
            textBox1.Text += log.strBuffer + "\r\n";
            textBox1.Select(textBox1.Text.Length - 1, 0);
            textBox1.ScrollToCaret();
        }

        bool killKeyWedge()
        {
            bool bRet = false;
            try
            {
                Terranova.API.ProcessInfo[] procs = Terranova.API.ProcessCE.GetProcesses();
                foreach (Terranova.API.ProcessInfo pi in procs)
                {
                    if (pi.FullPath.ToUpper().EndsWith(sKeyWedgeName.ToUpper())){
                        //Terranova.API.ProcessCE.Kill(pi.Pid);   //crashes device!
                        Terranova.API.ProcessCE.quitWindow("KeyWedge");
                        log.WriteLog("Ended Keywedge.exe");
                        bRet = true;
                    }
                }
            }
            catch (Exception ex)
            {
                log.WriteLog("Exception in KeyWedge.exe testing:" + ex.Message);
            }
            return bRet;
        }
        bool startKeyWedge()
        {
            bool bRet = false;
            try
            {
                System.Diagnostics.Process proc = System.Diagnostics.Process.Start(sKeyWedgeFullName, "");
                log.WriteLog("KeyWedge started");
                bRet = true;
            }
            catch (Exception ex)
            {
                log.WriteLog("Exception in StartKeyWedge: " + ex.Message);
            }
            return bRet;
        }

        private void Form1_Closing(object sender, CancelEventArgs e)
        {
            if (threadCloseApp != null)
                threadCloseApp.Abort();
            if (threadConnect != null)
                threadConnect.Abort();

            startKeyWedge();
            log.WriteLog("##### END #####");
        }
    }
}