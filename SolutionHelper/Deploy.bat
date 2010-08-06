@ECHO OFF

IF EXIST SolutionHelper.zip ERASE SolutionHelper.zip

"%ProgramFiles%\WinRAR\Rar.exe" a -ep -x@DeployExclude.txt SolutionHelper.zip "SolutionHelper\bin\Debug\*.*"

XCOPY "SolutionHelper.zip" "\\rufrt-vxbuild\d$\Program Files\CruiseControl.NET-1.5.6804.1\WebDashboard\download\" /Y

IF EXIST SolutionHelper.zip ERASE SolutionHelper.zip

PAUSE
