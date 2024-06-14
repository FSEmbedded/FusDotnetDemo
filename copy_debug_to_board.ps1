# Define variables, adapt as needed
$ipAddress = "10.0.0.136"
$projectName = "FusDotnetDemo"

# Directories to copy from the local runtimes directory
$runtimesToCopy = @("unix", "linux-arm", "linux-arm64")
# Local folder where the binaries are stored
$sourceDir = ".\bin\Debug\net8.0"
# Remote board
$remoteHost = "root@${ipAddress}"
$remoteDir = "/home/root"
# Temporary files and folders for the tar process
$tempDir = ".\bin\Debug\tempDir"
$tempSource = "${tempDir}\${projectName}"
$tarFileName = "${projectName}_Debug.tar"
$tarFilePath = ".\bin\Debug\${tarFileName}"

# Function to copy files and folders
function Copy-Files {
    param (
        [string]$source,
        [string]$destination,
        [string]$exclude
    )
    robocopy $source $destination /e /xd $exclude | Out-Null
}

# Copy files and folders to temporary directory, exclude runtimes directory
Copy-Files -source $sourceDir -destination $tempSource -exclude "runtimes"

# Copy required runtimes to the temporary directory
foreach ($runtime in $runtimesToCopy) {
    $runtimePath = Join-Path -Path $sourceDir -ChildPath "runtimes\$runtime"
    if (Test-Path $runtimePath) {
        Copy-Files -source $runtimePath -destination "${tempSource}\runtimes\$runtime"
    }
}

# Create .tar archive with all contents from tempDir
tar -cf $tarFilePath -C $tempDir .

# Stop the app and delete old project folder on remote board
ssh $remoteHost "killall dotnet && rm -rf ${projectName}"
# Copy tar archive to remote board
scp $tarFilePath "${remoteHost}:${remoteDir}"
# Extract tar archive on remote board and remove it
ssh $remoteHost "tar -xf ${tarFileName} && rm ${tarFileName}"

# Start the app on the remote board
#ssh $remoteHost "dotnet ${projectName}/${projectName}.dll"

# Clean up temporary directory
Remove-Item -Path $tempDir -Recurse
# Delete local .tar file
Remove-Item -Path $tarFilePath
