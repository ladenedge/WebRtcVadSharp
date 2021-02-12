
all: lib build

lib:
	make -C WebRtcVad

build: 
	dotnet build

clean:
	make -C WebRtcVad clean
	dotnet clean

restore:
	dotnet restore

nuget-publish:
	dotnet publish -c Release -r linux-x64
	warp-packer --arch linux-x64 --input_dir bin/Release/netcoreapp2.1/linux-x64/publish --exec made --output made

test:
	dotnet test
