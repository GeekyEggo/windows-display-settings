SCRIPTPATH="$( cd -- "$(dirname "$0")" >/dev/null 2>&1 ; pwd -P )"

pushd $SCRIPTPATH/..
dotnet publish -c Release -o ./dist/com.geekyeggo.windowsdisplaysettings.sdPlugin -r win-x64 --sc -p:PublishTrimmed=true -p:PublishSingleFile=true
cp ./src/manifest.json ./dist/com.geekyeggo.windowsdisplaysettings.sdPlugin/manifest.json
popd

pushd $SCRIPTPATH
rm com.geekyeggo.windowsdisplaysettings.streamDeckPlugin
./DistributionTool.exe -b -i ../dist/com.geekyeggo.windowsdisplaysettings.sdPlugin -o ../dist
popd
