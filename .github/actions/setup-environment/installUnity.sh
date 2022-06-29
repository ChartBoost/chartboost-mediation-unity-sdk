#!/bin/bash
FILE="/Applications/Unity/Unity.app"

if [ -d $FILE ]
then
    echo "Unity Installed. Skipping Step"
else
    echo "Unity Not Found. Installing Now"
    # Download Unity 2020.3.15f2
    curl -o Unity-2020.3.15f2.pkg https://download.unity3d.com/download_unity/6cf78cb77498/MacEditorInstaller/Unity.pkg
    # Install Unity 2020.3.15f2
    sudo installer -target / -dumplog -package Unity-2020.3.15f2.pkg
fi

exec bash
