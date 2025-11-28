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
set "TAR_FILE=%SCRIPT_DIR%bin\publish\%APP_NAME%.tar.gz"

echo.
echo 正在发布: %APP_NAME%

:: 1. 清理旧的发布目录
if exist "%PUBLISH_ROOT%" (
    echo 清理旧的发布目录...
    rmdir /s /q "%PUBLISH_ROOT%"
)

:: 2. 发布项目
echo 正在发布项目...
dotnet publish "%CURRENT_PROJECT%" -c %CONFIGURATION% -r %RUNTIME% -o "%PUBLISH_ROOT%" --no-self-contained

if not %ERRORLEVEL% == 0 (
    echo 错误: 项目发布失败!
    pause
    exit /b 1
)

:: 3. 检查发布结果
if not exist "%PUBLISH_ROOT%" (
    echo 错误: 发布目录未创建!
    pause
    exit /b 1
)

:: 4. 创建 tar 包
echo 正在创建 tar 包...
cd /d "%PUBLISH_ROOT%\.."

:: 检查 tar 命令是否可用
tar --version >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo 错误: 未找到 tar 命令，请安装 Git Bash 或 WSL
    echo 跳过打包步骤...
    goto :OPEN_FOLDER
)

:: 删除旧的 tar 包
if exist "%TAR_FILE%" (
    del /q "%TAR_FILE%"
)

:: 创建 tar.gz 包
tar -czf "%TAR_FILE%" "%APP_NAME%"

if not %ERRORLEVEL% == 0 (
    echo 警告: tar 打包失败，跳过打包步骤
    goto :OPEN_FOLDER
)

:: 5. 显示打包信息
set "TAR_SIZE=0"
for /f "tokens=3" %%i in ('dir "%TAR_FILE%" ^| find "个文件"') do set "TAR_SIZE=%%i"

echo.
echo ========================================
echo 发布完成!
echo 项目名称: %APP_NAME%
echo 发布目录: %PUBLISH_ROOT%
echo 压缩包: %TAR_FILE%
echo 文件大小: !TAR_SIZE!
echo ========================================

:OPEN_FOLDER
:: 6. 打开文件夹
echo.
echo 正在打开发布目录...
start "" explorer "%PUBLISH_ROOT%"

:: 7. 显示压缩包位置
if exist "%TAR_FILE%" (
    echo 压缩包已创建: %TAR_FILE%
    echo 可以使用以下命令解压:
    echo   tar -xzf %APP_NAME%.tar.gz
)

pause
exit /b 0