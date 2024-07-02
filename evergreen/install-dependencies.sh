#!/usr/bin/env bash
set -o errexit  # Exit the script with error if any of the commands fail

DOTNET_SDK_PATH="${DOTNET_SDK_PATH:-./.dotnet}"

if [[ $OS =~ [Ww]indows.* ]]; then
  echo "Downloading Windows .NET SDK installer..."
  curl -Lfo "./dotnet-install.ps1" https://dot.net/v1/dotnet-install.ps1
  echo "Installing ASP.NET Core 5.0 runtime..."
  powershell.exe "./dotnet-install.ps1" -Channel 3.1 -InstallDir "$DOTNET_SDK_PATH" -NoPath -Runtime aspnetcore
  echo "Installing .NET LTS SDK..."
  powershell.exe "./dotnet-install.ps1" -Channel LTS -InstallDir "$DOTNET_SDK_PATH" -NoPath
else
  echo "Downloading .NET SDK installer..."
  curl -Lfo "./dotnet-install.sh" https://dot.net/v1/dotnet-install.sh
  echo "Installing ASP.NET Core 5.0 runtime..."
  bash "./dotnet-install.sh" --channel 3.1 --install-dir "$DOTNET_SDK_PATH" --no-path --runtime aspnetcore
  echo "Installing .NET LTS SDK..."
  bash "./dotnet-install.sh" --channel LTS --install-dir "$DOTNET_SDK_PATH" --no-path
fi