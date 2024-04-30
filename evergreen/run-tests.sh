#!/usr/bin/env bash
set -o errexit  # Exit the script with error if any of the commands fail

dotnet clean "./MongoDB.Analyzer.sln"

if [ "${DRIVER_VERSION}" == "latest" ]; then
    echo "Getting latest driver version from MyGet"	
    sudo apt-get --assume-yes install jq

    DRIVER_VERSION=$(curl "https://www.myget.org/F/mongodb/api/v3/query?prerelease=true&take=1&q=PackageId:MongoDB.Driver" | jq ".data[0].version")
    echo Latest driver version in MyGet: "$DRIVER_VERSION"
fi;

dotnet build "./tests/MongoDB.Analyzer.Tests.Common.ClassLibrary" -f netstandard2.0 -c Debug
dotnet test "./MongoDB.Analyzer.sln" -e DRIVER_VERSION=${DRIVER_VERSION} --framework ${TARGET_FRAMEWORK} -c Release --results-directory ./build/test-results --logger "junit;LogFileName=TEST_MongoDB.Analyzer.xml;FailureBodyFormat=Verbose"
