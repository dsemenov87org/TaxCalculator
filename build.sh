#!/bin/bash
set -ex;

if [ ! "dotnet tool list -g | grep fake-cli " ]; then
  dotnet tool install fake-cli -g
fi

fake run ./build.fsx -t $0