
To use,

1. Download and install Visual Express c# 2005 OR SharpDevelop

http://www.microsoft.com/express/2005/
http://www.icsharpcode.net/OpenSource/SD/Download/#SharpDevelop22

Make sure to install prerequisites (.NET framework 2)
To use the TradeLink tests, you'll need to install Nunit (http://nunit.org)

2. Build the TradeLinkSuite.sln solution.

Open sharp develop, select file->open, browse to this directory.
Open the TradeLinkSuite.sln solution file.
In sharp develop, select Build->setConfiguration  -> Release
Then select Build->Build Solution.

This will create release executables of all the tradelink applets.

To build into an installer, open an explorer window in this folder.

navigate to the InstallSuite directory.
Run the BuildInstaller.bat file.

After this script has completed, you should have a TradeLinkSuite.exe installer in this directory.  

Run it to install all the applets.  

Now you have a working TradeLink installation.


TRADELINK SUITE OF APPLETS


1. CHARTOGRAPHER

Chart yearly and intra-day stocks


2. QUOTOPIA
Realtime stock quoting, order entry, tick archival, algorithm trading


3. REPLAY

Replay archived ticks to Quotopia or any Tradelink client

4. BOX-EXAMPLES

Examples of algorithmic trading strategies (Boxes)


5. Gauntlet

Backtest Boxes against historical ticks


6. EPF2IDX

Convert EPF tick files into stock index files

7. TimeAndSales

view tick files in time & sales format

8. SplitEPF

split multi-day EPF file into many single EPFs (one per day)

9. TradeLib

Utility classes and core api for above.  


FOR MORE INFORMATION : http://code.google.com/p/tradelink

