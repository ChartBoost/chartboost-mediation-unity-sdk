#!/bin/bash

# Environment variables
UNITY_EXE_PATH="${UNITY_EXE_PATH:-/Applications/Unity/Hub/Editor/2022.3.30f1/Unity.app/Contents/MacOS/Unity}"
PROJECT_PATH=com.chartboost.core.canary     # The unity project where this package is being used

# Generate .csproj files
"$UNITY_EXE_PATH" -quit -batchmode -nographics -projectPath $PROJECT_PATH -executeMethod Packages.Rider.Editor.RiderScriptEditor.SyncSolution

# Copy files
echo "Copying files..."
cp com.chartboost.mediation/README.md com.chartboost.mediation/Documentation/index.md
cp com.chartboost.mediation/CHANGELOG.md com.chartboost.mediation/Documentation/CHANGELOG.md

# Update links in index.md
echo "Updating links..."
sed -i '' 's|\(com.chartboost.mediation/Documentation/\)\([^)]*\)|\2|g' com.chartboost.mediation/Documentation/index.md

# Generate Documentation
echo "Generating documentation.."
dotnet docfx com.chartboost.mediation/Documentation/docfx.json

echo "Generating documentation complete"
