@echo off
dotnet clean -c Debug
dotnet clean -c Release
rm -rf .\bin
dotnet build -c Release
dotnet pack -c Release 
:: dotnet nuget add source https://nuget.pkg.github.com/csinkers/index.json -n github -u username -p token
:: dotnet nuget push "bin/Release/AdlMidi.NET.VERSION.nupkg" --source "github"
:: dotnet nuget push "bin/Release/AdlMidi.NET.VERSION.nupkg" --source nuget.org -k NUGET_KEY
:: dotnet nuget remove source github

:: dotnet nuget add source https://nuget.pkg.github.com/csinkers/index.json -n github -u username -p token
:: dotnet nuget push "src/bin/Release/SerdesNet.VERSION.nupkg" --source "github"
:: dotnet nuget push "src/bin/Release/SerdesNet.VERSION.nupkg" --source nuget.org -k NUGET_KEY
:: dotnet nuget remove source github
