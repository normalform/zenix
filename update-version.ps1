#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Updates the major.minor version in version.json

.DESCRIPTION
    This script helps you update the major and minor version numbers.
    The build number is automatically incremented by GitHub Actions.

.PARAMETER Major
    The major version number

.PARAMETER Minor
    The minor version number

.PARAMETER Increment
    What to increment: 'major', 'minor', or 'none'

.EXAMPLE
    # Set specific version
    .\update-version.ps1 -Major 2 -Minor 1

.EXAMPLE
    # Increment major version (resets minor to 0)
    .\update-version.ps1 -Increment major

.EXAMPLE
    # Increment minor version
    .\update-version.ps1 -Increment minor
#>

param(
    [int]$Major,
    [int]$Minor,
    [ValidateSet('major', 'minor', 'none')]
    [string]$Increment = 'none'
)

$versionFile = "version.json"

if (-not (Test-Path $versionFile)) {
    Write-Error "version.json file not found!"
    exit 1
}

# Read current version
$currentVersion = Get-Content $versionFile | ConvertFrom-Json

if ($Increment -eq 'major') {
    $Major = $currentVersion.major + 1
    $Minor = 0
    Write-Host "🔼 Incrementing major version to $Major (minor reset to 0)" -ForegroundColor Green
}
elseif ($Increment -eq 'minor') {
    $Major = $currentVersion.major
    $Minor = $currentVersion.minor + 1
    Write-Host "🔼 Incrementing minor version to $Minor" -ForegroundColor Green
}
elseif ($Major -ne 0 -or $Minor -ne 0) {
    Write-Host "📝 Setting version to $Major.$Minor" -ForegroundColor Green
}
else {
    Write-Host "📋 Current version: $($currentVersion.major).$($currentVersion.minor)" -ForegroundColor Cyan
    Write-Host "ℹ️  Build number will be automatically incremented by CI" -ForegroundColor Gray
    exit 0
}

# Update version
$newVersion = @{
    major = $Major
    minor = $Minor
}

$newVersion | ConvertTo-Json | Set-Content $versionFile

Write-Host "✅ Version updated in $versionFile" -ForegroundColor Green
Write-Host "📦 New version: $Major.$Minor.{auto-build}" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Commit the version.json changes" -ForegroundColor Gray
Write-Host "2. Push to trigger CI build" -ForegroundColor Gray
Write-Host "3. Create a tag (v$Major.$Minor.{build}) for release" -ForegroundColor Gray
