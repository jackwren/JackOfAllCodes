# Retrieve the DEPLOYMENT_GROUP_ID environment variable from CodeDeploy
$deploymentGroupId = $env:DEPLOYMENT_GROUP_ID

# Retrieve the DEPLOYMENT_ID environment variable from CodeDeploy
$deploymentId = $env:DEPLOYMENT_ID

# Define the base deployment root path for CodeDeploy on Windows
$baseDeploymentDir = "C:\ProgramData\Amazon\CodeDeploy"

# Construct the full path to the deployment-archive directory dynamically
$deploymentDir = Join-Path $baseDeploymentDir $deploymentGroupId 
$deploymentDir = Join-Path $deploymentDir $deploymentId 
$deploymentArchive = Join-Path $deploymentDir "deployment-archive"

# Log the deployment directory (optional, for debugging purposes)
Write-Host "Deployment Directory: $deploymentArchive"

# Define the path to the zip file inside the deployment-archive directory
$zipFile = Join-Path $deploymentArchive "application.zip"

# Check if the zip file exists
if (Test-Path $zipFile) {
    # Unzip the application.zip file into the deployment-archive directory
    Expand-Archive -Path $zipFile -DestinationPath $deploymentArchive -Force

    # Optionally, remove the zip file after extraction
    Remove-Item $zipFile

    Write-Host "Unzipping completed successfully."
}
else {
    Write-Host "Zip file not found in the deployment directory."
}
