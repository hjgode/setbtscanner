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
    public partial class Form1 : Form
    {
        private EventHandler m_deleInfo;
        const string sKeyWedgeFullName = @"\Windows\KeyWedge.exe";
        const string sKeyWedgeName = "KeyWedge.exe";
        
        public Form1()
        {
            InitializeComponent();
            
            m_deleInfo = new EventHandler(loginfoCallback);

            log = new logging(ref textBox1, m_deleInfo);

            log.WriteLog("+++++ START +++++");
        }

        public Form1(string sBT)
        {
            InitializeComponent();

            _sBT = sBT;

            m_deleInfo = new EventHandler(loginfoCallback);

            log = new logging(ref textBox1, m_deleInfo);

            log.WriteLog("+++++ START +++++");
        }

        BTdevice btdev;
        logging log;
        string _sBT = "";

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;

            killKeyWedge();
            Cursor.Current = Cursors.WaitCursor;
            Application.DoEvents();

            btdev = new BTdevice(ref log);
            if (_sBT.Length != 12)
                _sBT = "0023686E70BC";

            log.WriteLog("Using BT MAC: '" + _sBT + "'");

            if (btdev.DoAssociation(_sBT))
                log.WriteLog("Association OK");
            else
                log.WriteLog("Association FAILED");

            btdev.Dispose();
            btdev = null;

            Cursor.Current = Cursors.Default;
            Application.DoEvents();
            
            button1.Enabled = true;

        }
        /// <summary>
        /// StartupCallback - Interthread delegate.
        /// </summary>
        /// <param name="sender">unused</param>
        /// <param name="e">unused</param>
        private void loginfoCallback(object sender, System.EventArgs e)
        {
            textBox1.Text += log.strBuffer + "\r\n";
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