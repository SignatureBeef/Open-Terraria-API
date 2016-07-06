#!/usr/bin/env bash

MODS=`find . -name '*.cs' -print0 |xargs -0 grep ": ModificationBase" |sed -r "s/public class (.*?) : ModificationBase/\1/"`
IFS=$'\n'

for mod in $MODS; do
	FILEPATH=$(echo $mod |cut -d':' -f 1 |xargs)
	MODNAME=$(echo $mod |cut -d':' -f 2 |xargs)

	DIR=../MODS/OTAPI.Modification.$MODNAME

	echo $FILEPATH $MODNAME

	mkdir -p $DIR
	pushd $DIR

	dotnet new --type lib 

	json -I -f project.json -e 'this.dependencies["OTAPI.Patcher.Engine"]="0.20.2"; this.frameworks={}; this.frameworks.net451={};'

	rm Library.cs

	popd

	cp $FILEPATH $DIR
done
