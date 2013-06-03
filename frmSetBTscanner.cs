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
        Timer timerClose;
        Timer timerConnect;

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
            
            timerConnect = new Timer();
            timerConnect.Interval = 10000;
            timerConnect.Tick += new EventHandler(timerConnect_Tick);
            timerConnect.Enabled = true;

        }

        void timerConnect_Tick(object sender, EventArgs e)
        {
            timerConnect.Enabled = false;
            button1_Click(this, new EventArgs());
        }

        BTdevice btdev;
        logging log;
        string _sBT = "";

        private void button1_Click(object sender, EventArgs e)
        {
            btnConnect.Enabled = false;
            if (txtBTmacAddress.Text.Length == 12)
                _sBT = txtBTmacAddress.Text;

            killKeyWedge();
            Cursor.Current = Cursors.WaitCursor;
            Application.DoEvents();

            btdev = new BTdevice(ref log);
#if DEBUG
            if (_sBT.Length != 12)
                _sBT = "0023686E70BC";
#endif
            txtBTmacAddress.Text = _sBT;
            
            log.WriteLog("Using BT MAC: '" + _sBT + "'");

            if (btdev.DoAssociation(_sBT))
                log.WriteLog("Association OK");
            else
                log.WriteLog("Association FAILED");

            btdev.Dispose();
            btdev = null;

            Cursor.Current = Cursors.Default;
            Application.DoEvents();
            
            btnConnect.Enabled = true;

            if (bAutoClose)
            {
                timerClose = new Timer();
                timerClose.Interval = 3000;
                timerClose.Tick += new EventHandler(timerClose_Tick);
                timerClose.Enabled = true;
            }
            Application.DoEvents();
        }

        void timerClose_Tick(object sender, EventArgs e)
        {
            timerClose.Enabled = false;
            this.Close();
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
            startKeyWedge();
            log.WriteLog("##### END #####");
        }
    }
}