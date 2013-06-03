using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Reflection;

namespace setBTscanner
{
    class logging
    {
        int MAX_LOG_SIZE = 1024 * 1024 * 100; // 100 Kb

        private string _logName = "LogBT.txt";
        private string _logFullName;

        public string strBuffer;       // Inter-thread buffer
        System.Windows.Forms.TextBox m_ctlInvokeTarget; // Inter-thread control
        private EventHandler m_deleCallback; // Inter-thread delegate

        public logging(ref System.Windows.Forms.TextBox ctrl, EventHandler dele)
        {
            m_ctlInvokeTarget = ctrl;
            m_deleCallback = dele;
            string CurDir;
            CurDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            if (!CurDir.EndsWith("\\"))
                CurDir += "\\";
            _logFullName = CurDir + _logName;
        }

        public logging()
        {
            m_ctlInvokeTarget = null;
            string CurDir;
            CurDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            if (!CurDir.EndsWith("\\"))
                CurDir += "\\";
            _logFullName = CurDir + _logName;
        }

        public void WriteLog(string Msg)
        {
            DateTime t = DateTime.Now;

            FileInfo f = new FileInfo(_logFullName);
            StreamWriter S;
            if (!f.Exists)
            {
                S = File.AppendText(_logFullName);
            }
            else
                if (f.Length >= MAX_LOG_SIZE)
                {
                    f.CopyTo(_logFullName+".bak");
                    S = File.CreateText(_logFullName);
                }
                else
                {
                    S = File.AppendText(_logFullName);
                }

            S.WriteLine(t.ToShortDateString() + "," + t.ToLongTimeString() + " : " + Msg);
            System.Diagnostics.Debug.WriteLine(Msg);
            S.Close();
            try
            {
                if (m_ctlInvokeTarget != null)
                {
                    strBuffer = Msg;
                    m_ctlInvokeTarget.Invoke(m_deleCallback);
                }
            }
            catch (Exception) { }
        }
    }
}
