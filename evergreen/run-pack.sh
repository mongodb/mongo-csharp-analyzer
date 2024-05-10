#!/usr/bin/env bash
set -o errexit # Exit the script with error if any of the commands fail
set +o xtrace  # Disable tracing.

echo Creating nuget package...

dotnet clean "./MongoDB.Analyzer.sln"
dotnet build "./MongoDB.Analyzer.sln" -c Release
dotnet pack ./src/MongoDB.Analyzer.Package/MongoDB.Analyzer.Package.csproj -o ./artifacts/nuget -c Release -p:Version="$PACKAGE_VERSION" -p:ContinuousIntegrationBuild=true