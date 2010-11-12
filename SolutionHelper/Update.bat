@ECHO OFF

ERASE "C:\Users\oshuruev\Documents\Visual Studio 2010\Addins\SolutionHelper*"
XCOPY "SolutionHelper\bin\Debug\SolutionHelper.2010.AddIn" "C:\Users\oshuruev\Documents\Visual Studio 2010\Addins\"
XCOPY "SolutionHelper\bin\Debug\SolutionHelper.dll" "C:\Users\oshuruev\Documents\Visual Studio 2010\Addins\"

ERASE "C:\Users\oshuruev\Documents\Visual Studio 2008\Addins\SolutionHelper*"
XCOPY "SolutionHelper\bin\Debug\SolutionHelper.2008.AddIn" "C:\Users\oshuruev\Documents\Visual Studio 2008\Addins\"
XCOPY "SolutionHelper\bin\Debug\SolutionHelper.dll" "C:\Users\oshuruev\Documents\Visual Studio 2008\Addins\"

PAUSE
