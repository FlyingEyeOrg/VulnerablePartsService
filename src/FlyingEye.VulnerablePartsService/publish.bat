@echo off
chcp 65001 > nul
setlocal enabledelayedexpansion

:: 使用dotnet命令获取版本信息
set "CURRENT_PROJECT=%~dp0..\FlyingEye.VulnerablePartsService\FlyingEye.VulnerablePartsService.csproj"

for /f "tokens=1,2 delims=: " %%i in ('dotnet msbuild "%CURRENT_PROJECT%" /t:GetVersion /nologo') do (
    if "%%i"=="Version" set "VERSION=%%j"
)

if "%VERSION%"=="" set "VERSION=1.0.0"

echo 项目版本: %VERSION%

:: 基础配置
set "APP_NAME=VulnerablePartsService-v%VERSION%"
set "SCRIPT_DIR=%~dp0"
set "PUBLISH_ROOT=%SCRIPT_DIR%bin\publish\%APP_NAME%"
set "CONFIGURATION=Release"
set "RUNTIME=linux-x64"

echo.
echo 正在发布: %APP_NAME%
dotnet publish "%CURRENT_PROJECT%" -c %CONFIGURATION% -r %RUNTIME% -o "%PUBLISH_ROOT%" --no-self-contained

echo.
echo 发布完成!
start "" explorer "%PUBLISH_ROOT%"

pause
exit /b 0