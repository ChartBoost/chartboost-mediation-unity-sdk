#!/bin/bash
# Install the android-sdk and its tools, and accept its licenses for building.
curl -o android-tools.zip -L https://dl.google.com/android/repository/commandlinetools-mac-7583922_latest.zip
unzip -o android-tools.zip
export ANDROID_SDK_ROOT="/usr/local/share/android-sdk"
 # Installing the build tools in the background to avoid terminating the jobs due to the output.
./cmdline-tools/bin/sdkmanager --sdk_root=$ANDROID_SDK_ROOT
yes | ./cmdline-tools/bin/sdkmanager "platforms;android-32" --sdk_root=$ANDROID_SDK_ROOT > /dev/null 2>&1
yes | ./cmdline-tools/bin/sdkmanager "build-tools;32.0.0" --sdk_root=$ANDROID_SDK_ROOT > /dev/null 2>&1
exec bash
