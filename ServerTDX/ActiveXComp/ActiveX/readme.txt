NOTE: Please open TDAACTX.CHM file for detailed documentation of the ActiveX control. (Windows Help File format)

Make sure to register the TDAACTX.OCX before referencing it in your program or using the Samples provided that reference the OCX file

For example, if the OCX is in the C:\TDA-API\ActiveX  folder, then to manually register the OCX, you would Run the following command:

REGSVR32  C:\TDA-API\ActiveX\TDAACTX.OCX

NOTE: If this is done for an UPDATE - if you already had TDAACTX.OCX registered previously and used it in Excel, you will need to delete the caching info that Excel keeps. To do that, delete the TDAAX.EXD file in the Temp folder, Excel sub-folder. If doing it from the command prompt, type:

CD %TMP%
then do a DIR and look for Excel folders. It may be "Excel8.0" for example, or just Excel. Then go to that folder and delete the TDAACTX.EXD file.

Until you do that, Excel will not see the change, even if the new OCX is registered

VISTA NOTE: Due to Vista's new security features,  to register the ActiveX control to the local machine (not only current user) successfully, you must right click on “Command Prompt” in Accessories and click “Run as Administrator”, then run the REGSVR32 command above. If you do not have a command prompt, create a new shortcut for CMD first.



