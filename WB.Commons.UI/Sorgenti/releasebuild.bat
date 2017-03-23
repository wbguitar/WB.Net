%windir%\microsoft.net\framework\v4.0.30319\msbuild /t:clean "Commons.sln"
%windir%\microsoft.net\framework\v4.0.30319\msbuild /property:Configuration=Release "Commons.sln"
@IF %ERRORLEVEL% NEQ 0 PAUSE
