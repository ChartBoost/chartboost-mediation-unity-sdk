#!/bin/bash
# Install Java 11 and make it the default
brew tap AdoptOpenJDK/openjdk
brew install --cask adoptopenjdk11 --quiet
export JAVA_HOME=/Library/Java/JavaVirtualMachines/adoptopenjdk-11.jdk/Contents/Home
exec bash
