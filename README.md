setbtscanner
============

Associate BT barcode scanner with local COM port and launch KeyWedge.exe.

You can use this small tool on ITC devices to automatically associate a BT barcode scanner using it's MAC address.

The tool can be either used manually or started using a connect argument:

   setBTscanner -connect 0023686E70BC
   
will connect the BT device 0023686E70BC with the comm port defined in pswdm0c.xml:

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
		<BtPort>COM9:</BtPort>
	  </Devices>
	</PSWDM0C>

The tool comes with a built-in pswdm0c.xml but uses an external file if present. 
The Device settings inside pswmd0c.xml are replaced by the new device during the connect. 
You may only alter the BtPort setting in the xml file.

Inside the program dir you will find a log file to verify the function.

pswdm0c.cab has to be installed before using setBTscanner.   

## Installation

Copy files onto device:

- Put "itc setBTscanner.CAB" and "pswdm0c.cab" onto the device into folder "\Flash File Store\CabFiles\".
- Reboot the device

setBTscanner will be installed to \Program Files\setBTscanner. A lnk file will be placed into "\Windows\Start Menu\Programs\StartUp". 
The lnk file already contains the arguments "-connect 0023686E70BC"

    255#"\Program Files\setBTscanner\setBTscanner.exe" -connect 0023686E70BC
   
You have to edit this lnk file, if you want setBTscanner to connect automatically to a different device.

Additionally KeyWedge.exe is installed to \Windows. setBTscanner will stop and start KeyWegde.exe before and after a connect. 
Keywedge will be installed with the following settings.

	HKLM,"Software\Intermec\SSKeyWedge","comport",0x00000000,"COM9:"
	HKLM,"Software\Intermec\SSKeyWedge","parity",0x00010001,0
	HKLM,"Software\Intermec\SSKeyWedge","stopbits",0x00010001,0
	HKLM,"Software\Intermec\SSKeyWedge","databits",0x00010001,8
	HKLM,"Software\Intermec\SSKeyWedge","handshake",0x00010001,3
	HKLM,"Software\Intermec\SSKeyWedge","baudrate",0x00010001,57600
	HKLM,"Software\Intermec\SSKeyWedge","BeepAfterRead",0x00010001,1
	HKLM,"Software\Intermec\SSKeyWedge","Preamble",0x00000000,""
	HKLM,"Software\Intermec\SSKeyWedge","Postamble",0x00000000,""
	HKLM,"Software\Intermec\SSKeyWedge","UseCharSend",0x00000000,"0"
	HKLM,"Software\Intermec\SSKeyWedge","sendcharbychar",0x00010001,1

The setBTscanner communcations port association is not saved in the registry. The connection is only valid until a reboot. 
Therefor setBTscanner will be launched by the lnk file in the StartUp folder on every reboot of the device.

If you need to change the default COMx port you have to edit pswdm0c.xml (BtPort) and the registry (comport).

