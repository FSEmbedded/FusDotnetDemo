$ipAddress = "10.0.0.130" # Define the IP address as a variable
$localDir = ".\bin\Debug\net8.0"
$remotePath = "root@${ipAddress}:/home/root/IoTLib_Test" # Use the $ipAddress variable in the $remotePath
$runtimesPath = "$localDir/runtimes"

# Remove directories except "linux-arm64" and "unix" from the runtimes directory
Get-ChildItem -Path $runtimesPath -Directory -Exclude "linux-arm64", "unix" | Remove-Item -Recurse -Force

# Create the destination directory on the remote server if it doesn't exist
ssh root@$ipAddress "mkdir -p /home/root/IoTLib_Test"

# Copy files from the local directory to the remote server
scp -r $localDir/* $remotePath