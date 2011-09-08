@echo off
echo using cfix in %3 \cfix\bin\
echo Attemping to switch working directory to %1
cd %1
echo Running tests in %2
%3\cfix\bin\cfix32.exe -z -u %2
