#!/usr/bin/env bash

set -eu
set -o pipefail

cd `dirname $0`

FSIARGS="--fsiargs -d:MONO"

mono .paket/paket.bootstrapper.exe

if [ ! -e ~/.config/.mono/certs ]
then
  mozroots --import --sync --quiet
fi

mono .paket/paket.exe restore

chmod +x build.fsx

mono packages/tools/FAKE/tools/FAKE.exe "$@" $FSIARGS build.fsx