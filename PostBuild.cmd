@ECHO OFF
SETLOCAL ENABLEEXTENSIONS ENABLEDELAYEDEXPANSION

IF "%~5"=="" (
   ECHO Include following line the Post-build event of your project^(s^):
   ECHO "$(ProjectDir)..\PostBuild.cmd" "$(DevEnvDir)" "$(SolutionPath)" "$(ProjectPath)" "$(TargetPath)" "$(ConfigurationName)"
   EXIT /B 1
)

SET DevEnvDir=%~1
SET SolutionPath=%~2
SET SolutionDir=%~dp2
SET SolutionFileName=%~nx2
SET SolutionName=%~n2
SET ProjectPath=%~3
SET ProjectDir=%~dp3
SET ProjectFileName=%~nx3
SET ProjectName=%~n3
SET TargetPath=%~4
SET TargetDir=%~dp4
SET TargetFileName=%~nx4
SET TargetName=%~n4
SET TargetExt=%~x4
SET ConfigurationName=%~5

:: Generate Nuget package (https://docs.nuget.org/create/creating-and-publishing-a-package)
IF EXIST "%~dpn3.nuspec" IF "%ConfigurationName%" NEQ "Debug" NUGET pack "%ProjectPath%" -IncludeReferencedProjects -Prop "Configuration=%ConfigurationName%"

:: Generate Symbols package (https://docs.nuget.org/create/creating-and-publishing-a-symbol-package)
IF EXIST "%~dpn3.nuspec" IF "%ConfigurationName%" NEQ "Debug" NUGET pack "%ProjectPath%" -Symbols -Prop "Configuration=%ConfigurationName%"