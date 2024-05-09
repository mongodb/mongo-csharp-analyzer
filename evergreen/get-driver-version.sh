#!/usr/bin/env bash
set -o xtrace   # Write all commands first to stderr
set -o errexit  # Exit the script with error if any of the commands fail

if [ "${DRIVER_VERSION}" == "latest" ]; then
    echo "Getting latest driver version from MyGet"	    
    curl "https://www.myget.org/F/mongodb/api/v3/query?prerelease=true&take=1&q=PackageId:MongoDB.Driver" -o latest_driver.json

    if [[ "$OS" =~ Windows|windows ]]; then
        DRIVER_VERSION=$(powershell.exe "(Get-Content latest_driver.json | ConvertFrom-Json).data[0].version")
    else
        DRIVER_VERSION=$(curl "https://www.myget.org/F/mongodb/api/v3/query?prerelease=true&take=1&q=PackageId:MongoDB.Driver" | jq ".data[0].version" -r)
    fi
fi;

cat << EOF >> ./version-expansion.yml
DRIVER_VERSION: "$DRIVER_VERSION"
EOF