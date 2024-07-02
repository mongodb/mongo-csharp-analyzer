#!/usr/bin/env bash
set -o xtrace   # Write all commands first to stderr
set -o errexit  # Exit the script with error if any of the commands fail

echo Testing with driver: "${DRIVER_VERSION}" "${TARGET_FRAMEWORK}"

dotnet clean "./MongoDB.Analyzer.sln"

dotnet build "./tests/MongoDB.Analyzer.Tests.Common.ClassLibrary" -f netstandard2.0 -c Debug

dotnet test "./MongoDB.Analyzer.sln" -e DRIVER_VERSION="${DRIVER_VERSION}" --framework "${TARGET_FRAMEWORK}" -c Release --results-directory ./build/test-results --logger "junit;LogFileName=TEST-MongoDB.Analyzer.xml;FailureBodyFormat=Verbose"