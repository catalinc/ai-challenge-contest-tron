#!/bin/sh

gmcs -out:MyTronBot.exe *.cs
java -jar engine/Tron.jar maps/$1.txt "mono MyTronBot.exe" "java -jar example_bots/$2.jar"
