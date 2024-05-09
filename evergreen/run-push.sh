#!/usr/bin/env bash
set -o errexit  # Exit the script with error if any of the commands fail

if [ -z "$PACKAGE_VERSION" ]
then
  PACKAGE_VERSION=$(git describe --tags)
  echo Calculated PACKAGE_VERSION value: "$PACKAGE_VERSION"
fi
# validate "clear" version. x.y.z[-abc1]

echo Pushing nuget package...
dotnet nuget push ./artifacts/nuget/MongoDB.Analyzer."$PACKAGE_VERSION".nupkg  -s https://api.nuget.org/v3/index.json -k "$NUGET_KEY"
