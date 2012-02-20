@echo off
setlocal
:PROMPT
SET /P AREYOUSURE=Are you sure (Y/[N])?
IF /I "%AREYOUSURE%" NEQ "Y" GOTO END
tools/nuget.exe push package/Catnap.0.1.0.0.nupkg
:END
endlocal