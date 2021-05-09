rem Publishes Nuget packages to various sources - should be run on master branch after feature branch has been merged

@echo OFF
set nugetversion=1.0.1
set packageid=WVN.SimpleMapper
set packagepath=./artifacts/%packageid%.%nugetversion%.nupkg
set src=.\src\%packageid%.csproj

dotnet build -c Release
dotnet pack %src% -c Release -o ./artifacts /p:Version=%nugetversion%

rem test nuget
rem nuget push %packagepath% -Source "C:\Box\NuGet"

rem WVN Nuget
rem nuget delete %packageid% %nugetversion% -Source https://api.nuget.org/v3/index.json -ApiKey %WVN_NUGET_API_KEY% -NonInteractive
rem nuget push %packagepath% %WVN_NUGET_API_KEY% -Source https://api.nuget.org/v3/index.json

rem WVN github
dotnet nuget push %packagepath%  --source "github"

rem Create version tag in source control
git tag %nugetversion%
git push --tags