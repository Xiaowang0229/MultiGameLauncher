@echo off
echo 正在发布……
dotnet publish -c Release MultiGameLauncher.csproj --output="bin\Publish"
pause