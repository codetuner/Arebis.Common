@ECHO OFF

REM See: https://docs.nuget.org/create/creating-and-publishing-a-package

SETLOCAL

IF /I "%~1" NEQ "/skiprebuild" (

   :: Clean solution:
   CALL CLEAN.CMD

   :: Rebuild solution:
   MSBuild Arebis.Common.sln /t:Rebuild /p:Configuration=Release
)

:: Publish packages:
CALL :Publish Arebis.CodeAnalysis.Static\bin\Release\Arebis.CodeAnalysis.Static.*.0.nupkg
CALL :Publish Arebis.Common\bin\Release\Arebis.Common.*.0.nupkg
CALL :Publish Arebis.Contract\bin\Release\Arebis.Contract.*.0.nupkg
CALL :Publish Arebis.Data\bin\Release\Arebis.Data.*.0.nupkg
CALL :Publish Arebis.Data.Entity\bin\Release\Arebis.Data.Entity.*.0.nupkg
CALL :Publish Arebis.Imaging\bin\Release\Arebis.Imaging.*.0.nupkg
CALL :Publish Arebis.Logging.GrayLog\bin\Release\Arebis.Logging.GrayLog.*.0.nupkg
CALL :Publish Arebis.Modeling\bin\Release\Arebis.Modeling.*.0.nupkg
CALL :Publish Arebis.Office\bin\Release\Arebis.Office.*.0.nupkg
CALL :Publish Arebis.Parsing\bin\Release\Arebis.Parsing.*.0.nupkg
CALL :Publish Arebis.Pdf\bin\Release\Arebis.Pdf.*.0.nupkg
CALL :Publish Arebis.Testing\bin\Release\Arebis.Testing.*.0.nupkg
CALL :Publish Arebis.Web\bin\Release\Arebis.Web.*.0.nupkg
CALL :Publish Arebis.Windows\bin\Release\Arebis.Windows.*.0.nupkg
EXIT /B 0

:Publish
IF NOT EXIST "%~1" EXIT /B 0
ECHO.
DIR "%~1" /B
CHOICE /C YN /M "Publish ? "
IF %ERRORLEVEL% NEQ 1 EXIT /B 0
NUGET push "%~1" -ApiKey %APIKEY_NUGET_CODETUNER%
EXIT /B 0