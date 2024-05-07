# Define the IP address as a variable, change as needed
$ipAddress = "10.0.0.85"
# Directories to copy from the local runtimes directory
$runtimesToCopy = @("linux-arm", "linux-arm64", "unix")

# Folder where the binaries are stored
$localDir = ".\bin\Debug\net8.0"
$runtimesDir = "${localDir}/runtimes"
# Remote board
$remoteHost = "root@${ipAddress}"
$remoteDir = "/home/root/FusDotnetDemo"
$destination = "${remoteHost}:${remoteDir}"

# Create the destination directory on the remote server
ssh $remoteHost "mkdir -p ${remoteDir}"

# Copy everything from localDir to remotePath, excluding "runtimes"
Get-ChildItem -Path $localDir -Exclude "runtimes" | foreach {
    scp -r $_.FullName $destination
}

# Copy only defined runtimes $runtimesToCopy from the local runtimes directory to the remote server
ssh $remoteHost "mkdir -p ${remoteDir}/runtimes"
# Loop through the array and copy each directory to the remote server
foreach ($dir in $runtimesToCopy) {
    scp -r "${runtimesDir}/${dir}" "${destination}/runtimes/${dir}"
}