%windir%\microsoft.net\framework\v3.5\msbuild /t:clean "TetSistemi.Commons.sln"
%windir%\microsoft.net\framework\v3.5\msbuild /property:Configuration=Release "TetSistemi.Commons.sln"
@IF %ERRORLEVEL% NEQ 0 PAUSE
