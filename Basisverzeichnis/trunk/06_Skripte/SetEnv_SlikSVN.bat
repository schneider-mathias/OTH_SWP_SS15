@echo off
echo Setup SlikSvn environment variables.

set "SWP_SLIKSVN_VERSION_MAJOR=1"
set "SWP_SLIKSVN_VERSION_MINOR=8"
set "SWP_SLIKSVN_VERSION_SUBMINOR=13"

set "SWP_SLIKSVN_VERSION=%SWP_SLIKSVN_VERSION_MAJOR%.%SWP_SLIKSVN_VERSION_MINOR%.%SWP_SLIKSVN_VERSION_SUBMINOR%"

set "SWP_SLIKSVN_ROOT=%SWP_TOOLS_ROOT%/SlikSvn-%SWP_SLIKSVN_VERSION%/"

set "SWP_SLIKSVN_CMD=%SWP_SLIKSVN_ROOT%/bin/svn.exe"

REM output all SlikSvn relevant environmental variables:
set SWP_SLIKSVN
