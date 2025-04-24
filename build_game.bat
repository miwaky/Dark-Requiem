@echo off
setlocal enabledelayedexpansion

:: Nom du dossier final et du projet
set PROJECT=Dark Requiem
set OUTPUT=JeuFinal

echo.
echo ╔══════════════════════════════════════╗
echo ║   🚀 Compilation du jeu !%PROJECT%!   ║
echo ╚══════════════════════════════════════╝
echo.

echo 🛠️  Compilation...
dotnet publish -c Release -r win-x64 --self-contained true ^
    /p:PublishSingleFile=true ^
    /p:AssemblyName="%PROJECT%" ^
    /p:OutputPath=bin\Release\%OUTPUT%

if %ERRORLEVEL% neq 0 (
    echo ❌ La compilation a échoué.
    pause
    exit /b
)

echo 🧹 Nettoyage de l'ancien dossier "%OUTPUT%"...
if exist %OUTPUT% rmdir /s /q %OUTPUT%
mkdir %OUTPUT%

echo 🗃️  Copie des fichiers publiés...
xcopy /s /y /q bin\Release\%OUTPUT%\* %OUTPUT%\

echo 📁 Copie des assets...
xcopy /s /y /q assets %OUTPUT%\assets

echo ✅ Build terminé ! Fichiers copiés dans "%OUTPUT%"
echo 🔄 Lancement du jeu...
echo.

cd %OUTPUT%
start "" "%PROJECT%.exe"

endlocal
pause
