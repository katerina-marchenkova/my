@ECHO OFF

ERASE "%USERPROFILE%\Documents\Visual Studio 2010\Addins\SolutionHelper*"
XCOPY "SolutionHelper\bin\Debug\SolutionHelper.2010.AddIn" "%USERPROFILE%\Documents\Visual Studio 2010\Addins\"
XCOPY "SolutionHelper\bin\Debug\SolutionHelper.dll" "%USERPROFILE%\Documents\Visual Studio 2010\Addins\"

ERASE "%USERPROFILE%\Documents\Visual Studio 2008\Addins\SolutionHelper*"
XCOPY "SolutionHelper\bin\Debug\SolutionHelper.2008.AddIn" "%USERPROFILE%\Documents\Visual Studio 2008\Addins\"
XCOPY "SolutionHelper\bin\Debug\SolutionHelper.dll" "%USERPROFILE%\Documents\Visual Studio 2008\Addins\"

PAUSE
