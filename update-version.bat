@echo off
setlocal enabledelayedexpansion

if "%1"=="" goto :usage
if "%2"=="" goto :usage

set major=%1
set minor=%2

echo {> version.json
echo   "major": %major%,>> version.json
echo   "minor": %minor%>> version.json
echo }>> version.json

echo ✅ Version updated to %major%.%minor%
echo 📦 Build number will be auto-incremented by CI
echo.
echo Next steps:
echo 1. git add version.json
echo 2. git commit -m "Bump version to %major%.%minor%"
echo 3. git push
goto :end

:usage
echo Usage: update-version.bat [major] [minor]
echo Example: update-version.bat 1 2
echo This will set version to 1.2.{auto-build}

:end
