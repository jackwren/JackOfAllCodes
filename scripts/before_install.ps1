# Define source and destination directories
$sourceDir = "C:\inetpub\wwwroot\app"
$backupDir = "C:\backup"

# Check if backup directory exists, and if so, delete all files and folders inside it
if (Test-Path $backupDir) {
    Write-Host "Purging the backup directory..."
    Get-ChildItem -Path $backupDir -Recurse | Remove-Item -Force -Recurse
} else {
    # If the backup directory doesn't exist, create it
    Write-Host "Backup directory not found, creating it..."
    New-Item -Path $backupDir -ItemType Directory
}

# Ensure the source directory exists
if (Test-Path $sourceDir) {
    Write-Host "Moving files from $sourceDir to $backupDir..."

    # Move all contents (files and subdirectories) from source to backup directory
    Get-ChildItem -Path $sourceDir | ForEach-Object {
        Move-Item -Path $_.FullName -Destination $backupDir
    }

    Write-Host "Move completed successfully."
} else {
    Write-Host "Source directory $sourceDir does not exist."
}

