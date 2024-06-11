#!/usr/bin/env bash
set -o errexit # Exit the script with error if any of the commands fail
set +o xtrace  # Disable tracing.

# Environment variables used as input:
# NUGET_SIGN_CERTIFICATE_FINGERPRINT
# PACKAGES_SOURCE
# PACKAGES_SOURCE_KEY
# PACKAGE_VERSION


if [ -z "$PACKAGES_SOURCE" ]; then
  echo "PACKAGES_SOURCE variable should be set"
  exit 1
fi

if [ -z "$PACKAGES_SOURCE_KEY" ]; then
  echo "PACKAGES_SOURCE_KEY variable should be set"
  exit 1
fi

if [ -z "$PACKAGE_VERSION" ]; then
  echo "PACKAGE_VERSION variable should be set"
  exit 1
fi

clear_version_rx='^[0-9]+\.[0-9]+\.[0-9]+(-[a-zA-Z0-9]+)?$'
if [ "$PACKAGES_SOURCE" = "https://api.nuget.org/v3/index.json" ] && [[ ! "$PACKAGE_VERSION" =~ $clear_version_rx ]]; then
  echo "Cannot push dev version to nuget.org: '$PACKAGE_VERSION'"
  exit 1
fi

echo Verifying signature
dotnet nuget verify ./artifacts/nuget/"$package"."$PACKAGE_VERSION".nupkg --certificate-fingerprint "$NUGET_SIGN_CERTIFICATE_FINGERPRINT"

echo Pushing nuget package...
dotnet nuget push --source "$PACKAGES_SOURCE" --api-key "$PACKAGES_SOURCE_KEY" ./artifacts/nuget/MongoDB.Analyzer."$PACKAGE_VERSION".nupkg