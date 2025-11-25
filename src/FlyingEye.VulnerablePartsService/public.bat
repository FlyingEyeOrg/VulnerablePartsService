@echo off
:: 设置控制台编码为UTF-8
chcp 65001 > nul
setlocal enabledelayedexpansion

:: 1. 基础配置
set "APP_NAME=VulnerablePartsService-v1.0.0"
set "SCRIPT_DIR=%~dp0"
set "CURRENT_PROJECT=%SCRIPT_DIR%..\FlyingEye.VulnerablePartsService\FlyingEye.VulnerablePartsService.csproj"
set "PUBLISH_ROOT=%SCRIPT_DIR%bin\publish\%APP_NAME%"
set "CONFIGURATION=Release"
set "RUNTIME=linux-x64"

echo.
echo 正在发布主程序...
dotnet publish "%CURRENT_PROJECT%" -c %CONFIGURATION% -r %RUNTIME% -o "%PUBLISH_ROOT%" --no-self-contained

echo.
echo 发布完成
start "" explorer "%PUBLISH_ROOT%"

pause
exit /b 0