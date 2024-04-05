$ipAddress = "10.0.0.136" # Define the IP address as a variable
$localDir = ".\bin\Debug\net8.0"
$remotePath = "root@${ipAddress}:/home/root/IoTLib_Test" # Use the $ipAddress variable in the $remotePath
$runtimesPath = "$localDir/runtimes"

# Remove directories except "linux-arm64" from the runtimes directory
Get-ChildItem -Path $runtimesPath -Directory | Where-Object { $_.Name -ne "linux-arm64" } | Remove-Item -Recurse -Force

# Create the destination directory on the remote server if it doesn't exist
ssh root@$ipAddress "mkdir -p /home/root/IoTLib_Test"

# Copy files from the local directory to the remote server
scp -r $localDir/* $remotePath