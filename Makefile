TSTAMP=`date +%F.%H.%M.%S`

build:
	gmcs -out:MyTronBot.exe *.cs

clean:
	-rm *.log *.exe *.zip

rebuild: clean build

entry: clean build
	zip MyTronBot-$(TSTAMP).zip *.cs
	cp *.zip entries/
