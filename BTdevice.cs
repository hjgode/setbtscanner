#undef USE_SSAPI
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using Intermec.Communication.Bluetooth;

using System.IO;

namespace setBTscanner
{
    class BTdevice : IDisposable
    {
        private Intermec.Communication.Bluetooth.DeviceUtility _deviceUtility;
        /*
            <PSWDM0C FormatVersion="1.0">
            <Logging level="none"/>
               <UserInterface>
                  <Configuration name="PSWDM0C">
                     <FullScreen enabled="N"/>
                  </Configuration>
               </UserInterface>
               <Logging level="none"/>
               <Devices>
		            <Security enabled="Y" strict="N"/>
		            <Device>
			            <DeviceName>RS507 12088000500400   (0023686e70bc)</DeviceName>
			            <DeviceAddress>0023686e70bc</DeviceAddress>
			            <ClassOfDevice>080620</ClassOfDevice>
		            </Device>
		            <DefaultDevice name="RS507 12088000500400   (0023686e70bc)"/>
		            <BtPort>COM6:</BtPort>
	            </Devices>
            </PSWDM0C>
        */
        private string xmlFileName = @"\Windows\pswdm0c.xml";
        const string pswdmxml = "pswdm0c.xml";

        logging log;
        /// <summary>
        /// a list of possible passkeys
        /// </summary>
        string[] _sPasskeys = { "12345", "0000", "1234" };

        public BTdevice(ref logging lg)
        {
            log = lg;
            try
            {
                preparePSWMD0C();
#if USE_SSAPI 
                BTpower btPower = new BTpower();
                if (btPower.IsBluetoothOn() == false)
                {
                    log.WriteLog("BT power is OFF, trying to switch ON...");
                    btPower.TurnOnBluetooth();
                    if (btPower.IsBluetoothOn() == false)
                        log.WriteLog("BT power switch ON failed");
                    else
                        log.WriteLog("BT power is now ON");
                }
                else
                    log.WriteLog("BT power was ON, no change");
                
                //btPower.Dispose();
#endif
                _deviceUtility = new DeviceUtility();
                _deviceUtility.Initialize(xmlFileName);
            }
            catch (DeviceUtilityException ex)
            {
                log.WriteLog("DeviceUtilityException in BTdevice(): " + ex.Message);
                _deviceUtility = null;
            }
            catch (Exception ex)
            {
                log.WriteLog("Exception in BTdevice(): " + ex.Message);
                _deviceUtility = null;
            }
            log.WriteLog("BTdevice() OK");

        }

        public void Dispose()
        {
            if (_deviceUtility != null)
            {
                _deviceUtility.Close();
                _deviceUtility.Dispose();
                _deviceUtility = null;
            }
        }

        public bool DoAssociation(string btAddr)
        {
            if (btAddr == "")
            {
                log.WriteLog("Missing BT Address!");
                Win32.doBadBeeps();
                return false;
            }
            if (_deviceUtility == null)
            {
                log.WriteLog("DeviceUtility unavailable!");
                Win32.doBadBeeps();
                return false;
            }

            string strDeviceName;
            log.WriteLog("FindDevice '" + btAddr + "'...");
            if (!FindDevice(btAddr, out strDeviceName))
            {
                log.WriteLog("Device not found!");
                Win32.doBadBeeps();
                return false;
            }
            log.WriteLog("... Device was found");

            log.WriteLog("Association of '" + strDeviceName + "'...");
            foreach (string s in _sPasskeys)
            {
                try
                {
                    log.WriteLog("Bonding...");
                    if (_deviceUtility.IsBondWithDeviceNeeded(strDeviceName))
                    {
                        string strPasskey;
                        log.WriteLog("Authentication: trying '" + s + "'...");
                        //deviceUtility.GetDeviceProperty(strDeviceName, DeviceUtility.PropertyName.Passkey);
                        //if (string.Compare(strPasskey, "") == 0)
                        //{
                        strPasskey = s;
                        //strPasskey = "1111";
                        _deviceUtility.SetDeviceProperty(strDeviceName, DeviceUtility.PropertyName.Passkey, strPasskey);
                        //}
                        // Bond the computer and the device.
                        // TODO: Because radio connections can sometimes fail,
                        // you should build in retry logic when BondWithDevice
                        // fails. This sample just tries once.
                        _deviceUtility.BondWithDevice(strDeviceName);
                        log.WriteLog("... Association and bonding finished");
                        break;
                    }
                    else
                    {
                        log.WriteLog("... no bonding needed");
                        break;
                    }
                }
                catch (DeviceUtilityException ex)
                {
                    log.WriteLog("Error in Association: " + ex.Message);
                    return false;
                }
                catch (Exception ex)
                {
                    log.WriteLog("Error in Association: " + ex.Message);
                    return false;
                }
            }//foreach

            try
            {
                log.WriteLog("Registration...");
                _deviceUtility.SetDefaultDevice(null, null, strDeviceName);
                // Set up a port to use the selected device. Refer to 
                // the BtPort element in the XML file.

                //_deviceUtility.AddDevice(null, null, "BtPort");
                //_deviceUtility.SetDeviceProperty("BtPort", DeviceUtility.PropertyName.DeviceAddress, "COM6:");

                _deviceUtility.SetActiveDevice(null, null, strDeviceName);
                log.WriteLog("...Registration OK");
                Win32.doGoodBeeps();
                return true;
            }
            catch (DeviceUtilityException ex)
            {
                log.WriteLog("Error in Registration: " + ex.Message);
                Win32.doBadBeeps();
                return false;
            }
        }

        private bool FindDevice(string BDAAddress, out string strOutDeviceName)
        {
            bool bFound = false;
            strOutDeviceName = "";
            try
            {
                log.WriteLog("Discovering devices ...");                
                _deviceUtility.DiscoverDevices(null, null);
                log.WriteLog("...Discover finished");                
            }
            catch (DeviceUtilityException ex)
            {
                log.WriteLog("DiscoveredDevices exception: " + ex.Message);                
            }
            try
            {
                string[] temp = _deviceUtility.GetDiscoveredDevices();
                if (temp.Length > 0)
                {
                    // Add found devices to the list of known devices.
                    // When calling AddDevice with a device name returned by
                    // GetDiscoveredDevices, make sure you call AddDevice and
                    // GetDiscoveredDevices within the same Initialize/Close
                    // block. Otherwise,the device is added to the XML file 
                    // as a non-Bluetooth device because Bluetooth device 
                    // information retrieved by DiscoverDevices is lost when 
                    // Close is called.
                    foreach (string device in temp)
                    {
                        _deviceUtility.AddDevice(null, null, device);
                        log.WriteLog("Added '" + device + "'");
                    }
                }
                else
                {
                    log.WriteLog("No devices nearby");
                }
            }
            catch (DeviceUtilityException ex)
            {
                log.WriteLog("GetDiscoveredDevices exception: " + ex.Message);                
            }
            try
            {
                string deviceAddress;
                string[] temp = _deviceUtility.GetDevices(null, null);
                if (temp.Length > 0)
                {
                    foreach (string device in temp)
                    {
                        deviceAddress = _deviceUtility.GetDeviceProperty(device, DeviceUtility.PropertyName.DeviceAddress);
                        if (string.Compare(deviceAddress, BDAAddress, true) == 0)
                        {
                            bFound = true;
                            strOutDeviceName = device;
                            //string passkey = _deviceUtility.GetDeviceProperty(device, DeviceUtility.PropertyName.Passkey);  //throws exception see docu
                            //string authent = _deviceUtility.GetDeviceProperty(device, DeviceUtility.PropertyName.Authentication);
                            break;
                        }
                    }
                }
                else
                {
                    log.WriteLog("No devices by GetDevices()");
                }
            }
            catch (DeviceUtilityException ex)
            {
                log.WriteLog("GetDiscoveredDevices exception: " + ex.Message);                
            }

            if (bFound)
            {
                log.WriteLog("Found device with matching '" + BDAAddress + "'");
                return true;
            }
            else
            {
                log.WriteLog("No matching devices for '" + BDAAddress + "'");
                return false;
            }
        }

        void preparePSWMD0C()
        {
            string sXML = getResourceOrFile(pswdmxml);
            //save to windows
            StreamWriter s = new StreamWriter(xmlFileName);
            s.WriteLine(sXML);
            s.Flush();
            s.Close();
            log.WriteLog("Starting with pswdm0c.xml");
        }

        private string getResourceOrFile(string sFile)
        {
            string sReturn = "";
            if (existAppFile(sFile))
            {
                System.IO.TextReader tr = new System.IO.StreamReader(Helpers.AppPath() + sFile);
                sReturn = tr.ReadToEnd();
                tr.Close();
                log.WriteLog("using external resource '" + sFile + "'");
            }
            else
            {
                try
                {
                    using (Stream stream = this.GetType().Assembly.GetManifestResourceStream(Helpers.AssemblyName() + "." + sFile))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            sReturn = reader.ReadToEnd();
                            log.WriteLog("using embedded resource '" + sFile + "'");
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.WriteLog("Exception in getResourceOrFile() for '" + sFile + "'" + ex.Message);
                }
            }
            return sReturn;
        }
        private static bool existAppFile(string sFile)
        {
            return System.IO.File.Exists(Helpers.AppPath() + sFile);
        }
    }
    class Helpers
    {
        static bool logOnceApp = false;
        static bool logOnceAssembly = false;
        static string _AppPath = null;
        public static string AppPath()
        {
            if (_AppPath == null)
            {
                _AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                if (!_AppPath.EndsWith(@"\"))
                    _AppPath += @"\";
            }
            if (!logOnceApp)
            {
                System.Diagnostics.Debug.WriteLine("_AppPath='" + _AppPath + "'");
                logOnceApp = true;
            }
            return _AppPath;
        }
        static string _AssemblyName = null;
        public static string AssemblyName()
        {
            if (_AssemblyName == null)
            {
                _AssemblyName = System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            }
            if (!logOnceAssembly)
            {
                System.Diagnostics.Debug.WriteLine("_AssemblyName='" + _AssemblyName + "'");
                logOnceAssembly = true;
            }
            return _AssemblyName;
        }
        public static void logError(string s)
        {
            System.Diagnostics.Debug.WriteLine("Error: " + s);
            //logToFile(s);
        }
        public static void logInfo(string s)
        {
            System.Diagnostics.Debug.WriteLine("Info:  " + s);
            //logToFile(s);
        }
    }
}