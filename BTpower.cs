using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using Intermec.DeviceManagement.SmartSystem;
using System.Xml;

namespace setBTscanner
{
    class BTpower:IDisposable
    {
        Intermec.DeviceManagement.SmartSystem.ITCSSApi SSAPI;

        public BTpower()
        {
            SSAPI = new ITCSSApi();
        }
        public void Dispose()
        {
            SSAPI.Dispose();
            SSAPI = null;
        }

        public string XmlSetBluetoothPowerFormat
        {
            get
            {
                return "<Subsystem Name=\"Bluetooth\"><Field Name=\"Power\">{0}</Field></Subsystem>";
            }
        }

        public string XmlGetBluetoothDiscoverableFormat
        {
            get
            {
                return "<Subsystem Name=\"Bluetooth\"><Field Name=\"Discoverable\"></Field></Subsystem>";
            }
        }

        public string XmlGetBluetoothPowerFormat
        {
            get
            {
                return "<Subsystem Name=\"Bluetooth\"><Field Name=\"Power\"></Field></Subsystem>";
            }
        }

        private string GetSSAPIValue(string xmlGet, string errorText)
        {
            string response = "";
            try
            {
                uint num = this.DoAction(xmlGet, "Get", out response);
                if (num == 0)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("OK for SSAPI.get() {0}", response));
                    return this.ExtractField(response, errorText);
                }
                System.Diagnostics.Debug.WriteLine(string.Format("Failed to get {0}", errorText), "Error");
                //string message = string.Format("Failed to get {0}, status={1:X}", errorText, num);
                //Log.main.LogString(EventLog.Severity.Error, message);
            }
            catch
            {
                string text = string.Format("Attempting to get {0} caused exception", errorText);
                System.Diagnostics.Debug.WriteLine(text, "Error");
                //Log.main.LogString(EventLog.Severity.Error, text);
            }
            return "";
        }

        private bool IsBluetoothDiscoverable()
        {
            bool flag = false;
            if (this.IsBluetoothOn())
            {
                flag = this.GetSSAPIValue(this.XmlGetBluetoothDiscoverableFormat, "Bluetooth discoverable") == "Enable";
            }
            return flag;
        }

        public bool IsBluetoothOn()
        {
            return (this.GetSSAPIValue(this.XmlSetBluetoothPowerFormat, "Bluetooth power") == "On");
        }


        public void TurnOnBluetooth()
        {
            string xmlSet = string.Format(this.XmlSetBluetoothPowerFormat, "On");
            this.SetSSAPIValue(xmlSet, "Turn on Bluetooth");
        }
        private void TurnOffBluetooth()
        {
            string xmlSet = string.Format(this.XmlSetBluetoothPowerFormat, "Off");
            this.SetSSAPIValue(xmlSet, "Turn off Bluetooth");
        }

        uint DoAction(string doSett, string sAction, out string sResponse)
        {
            uint uRet = 0;
            StringBuilder str = new StringBuilder(2048);
            int iSize=2048;
            if (sAction.ToLower() == "get")
                uRet = SSAPI.Get(doSett, str, ref iSize, 10000);
            else
                uRet = SSAPI.Set(doSett, str, ref iSize, 10000);
            sResponse = str.ToString();
            return uRet;
        }

        private void SetSSAPIValue(string xmlSet, string errorText)
        {
            String str;
            uint num = this.DoAction(xmlSet, "Set", out str);            
            if (num != 0)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("{0} failed", errorText), "Error");
                //string message = string.Format("{0} failed, status={1:X}", errorText, num);
                //Log.main.LogString(EventLog.Severity.Error, message);
            }
        }
        private string ExtractField(string response, string errorText)
        {
            XmlDocument document = new XmlDocument();
            string innerText = "";
            try
            {
                document.LoadXml(response.ToString());
                innerText = document.InnerText;
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Attempting to parse getting {0} caused exception", errorText), "Error");
            }
            return innerText;
        }


        public bool BluetoothDiscoverable
        {
            get
            {
                return this.IsBluetoothDiscoverable();
            }
        }
    }
}
