$localDir = "C:\Users\Bruegel\Source\CSharp\IoTLib_Test\bin\Debug\net8.0"
$remotePath = "root@10.0.0.55:/home/root/IoTLib_Test"
$runtimesPath = "$localDir/runtimes"

Get-ChildItem -Path $runtimesPath -Directory | Where-Object { $_.Name -ne "linux-arm64" } | Remove-Item -Recurse -Force

scp -r $localDir/* $remotePath