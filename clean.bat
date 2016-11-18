@ECHO OFF

SET OTAPI_ROOT_DIR=%~dp0
ECHO [OTAPI] Pre build cleanup to ensure configuration items are cleaned up
ECHO [OTAPI] Base directory %OTAPI_ROOT_DIR%

cd %OTAPI_ROOT_DIR%OTAPI.Modifications

for /D %%s in (*) DO (
echo [OTAPI] Removing %OTAPI_ROOT_DIR%OTAPI.Modifications\%%s\bin
rmdir /s /q %OTAPI_ROOT_DIR%OTAPI.Modifications\%%s\bin

echo [OTAPI] Removing %OTAPI_ROOT_DIR%OTAPI.Modifications\%%s\obj
rmdir /s /q %OTAPI_ROOT_DIR%OTAPI.Modifications\%%s\obj
)