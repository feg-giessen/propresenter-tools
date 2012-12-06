@ECHO OFF
pushd %~dp0

cd lib
IF NOT EXIST pro-presenter MKDIR pro-presenter

DEL /F /Q pro-presenter\*

IF EXIST "C:\Program Files\Renewed Vision\ProPresenter 5\" SET pp=C:\Program Files\Renewed Vision\ProPresenter 5
IF EXIST "C:\Program Files (x86)\Renewed Vision\ProPresenter 5\" SET pp=C:\Program Files (x86)\Renewed Vision\ProPresenter 5

IF NOT EXIST "%pp%" GOTO :ERROR

echo "%pp%\csExWB.dll"
XCOPY "%pp%\csExWB.dll" "pro-presenter\"
COPY "%pp%\Interop.Shell32.dll" "pro-presenter\"
COPY "%pp%\Ionic.Zip.dll" "pro-presenter\"
COPY "%pp%\log4net.dll" "pro-presenter\"
COPY "%pp%\Microsoft.Expression.Encoder.dll" "pro-presenter\"
COPY "%pp%\Microsoft.Expression.Encoder.Types.dll" "pro-presenter\"
COPY "%pp%\Microsoft.Expression.Encoder.Utilities.dll" "pro-presenter\"
COPY "%pp%\Microsoft.WindowsAPICodePack.dll" "pro-presenter\"
COPY "%pp%\Microsoft.WindowsAPICodePack.Shell.dll" "pro-presenter\"
COPY "%pp%\ProPresenter*.dll" "pro-presenter\"
COPY "%pp%\System.Data.SQLite.dll" "pro-presenter\"
COPY "%pp%\taglib-sharp.dll" "pro-presenter\"

ProPatcher.exe pro-presenter\ProPresenter.DO.PCO.dll

GOTO :END

:ERROR

:End
popd