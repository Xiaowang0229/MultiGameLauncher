@echo off
echo 正在发布……
dotnet publish -c Release -r win-x64 MultiGameLauncher.csproj --output="bin\Publish"
pause