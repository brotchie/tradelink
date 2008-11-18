@echo off
echo Attemping to switch working directory to %1
cd %1
echo Running tests in %2
C:\Users\jfranta\AppData\Roaming\cfix\bin\i386\cfix32.exe -z -u %2
