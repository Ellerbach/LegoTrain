#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Builds the LegoTrain ASP.NET Docker image.

.DESCRIPTION
    Builds a Docker image for the LegoTrain project with specified platform and version.

.PARAMETER Platform
    The target platform architecture (amd64, arm32, or arm64). Default: amd64

.PARAMETER Version
    The version tag for the image. Default: 1.0

.PARAMETER Registry
    The Docker registry/repository name. Default: ellerbach/legotrain

.PARAMETER Push
    If specified, pushes the image to the registry after building.

.EXAMPLE
    .\build-docker.ps1
    
.EXAMPLE
    .\build-docker.ps1 -Platform arm32 -Version 1.0
    
.EXAMPLE
    .\build-docker.ps1 -Platform arm64 -Version 2.3 -Registry myregistry/legotrain -Push
#>

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("amd64", "arm32", "arm64")]
    [string]$Platform = "amd64",
    
    [Parameter(Mandatory=$false)]
    [string]$Version = "1.0",
    
    [Parameter(Mandatory=$false)]
    [string]$Registry = "ellerbach/legotrain",
    
    [Parameter(Mandatory=$false)]
    [switch]$Push
)

# Set error action preference
$ErrorActionPreference = "Stop"

# Determine which Dockerfile to use
$dockerfilePath = switch ($Platform) {
    "amd64" { "LegoTrain/Dockerfile" }
    "arm32" { "LegoTrain/Dockerfile.arm32" }
    "arm64" { "LegoTrain/Dockerfile.arm64" }
}

# Build the image tag
$imageTag = "${Registry}:${Platform}-${Version}"

Write-Host "Building LegoTrain Docker image..." -ForegroundColor Cyan
Write-Host "  Platform:    $Platform" -ForegroundColor Yellow
Write-Host "  Version:     $Version" -ForegroundColor Yellow
Write-Host "  Dockerfile:  $dockerfilePath" -ForegroundColor Yellow
Write-Host "  Image Tag:   $imageTag" -ForegroundColor Yellow
Write-Host ""

try {
    # Build the Docker image
    docker build -f $dockerfilePath -t $imageTag .
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host ""
        Write-Host "Build failed with exit code $LASTEXITCODE" -ForegroundColor Red
        exit $LASTEXITCODE
    }
    
    Write-Host ""
    Write-Host "Build completed successfully!" -ForegroundColor Green
    Write-Host "Image: $imageTag" -ForegroundColor Green
    
    # Push if requested
    if ($Push) {
        Write-Host ""
        Write-Host "Preparing to push image to registry..." -ForegroundColor Cyan
        
        # Extract registry hostname (everything before the first slash, or docker.io if no slash)
        $registryHost = if ($Registry -match '^([^/]+)/') { $matches[1] } else { "docker.io" }
        
        # Check if already logged in by attempting to get auth token
        Write-Host "Checking authentication for registry: $registryHost" -ForegroundColor Yellow
        
        # Try to inspect the config to see if we're logged in
        $configPath = "$env:USERPROFILE\.docker\config.json"
        $needsLogin = $true
        
        if (Test-Path $configPath) {
            try {
                $config = Get-Content $configPath | ConvertFrom-Json
                if ($config.auths.PSObject.Properties.Name -contains $registryHost -or 
                    $config.auths.PSObject.Properties.Name -contains "https://$registryHost") {
                    $needsLogin = $false
                    Write-Host "Already authenticated to $registryHost" -ForegroundColor Green
                }
            }
            catch {
                # Ignore errors reading config
            }
        }
        
        # Prompt for login if needed
        if ($needsLogin) {
            Write-Host ""
            Write-Host "Not authenticated to $registryHost" -ForegroundColor Yellow
            Write-Host "Please log in to the registry:" -ForegroundColor Cyan
            
            docker login $registryHost
            
            if ($LASTEXITCODE -ne 0) {
                Write-Host ""
                Write-Host "Login failed. Image will not be pushed." -ForegroundColor Red
                exit $LASTEXITCODE
            }
        }
        
        # Push the image
        Write-Host ""
        Write-Host "Pushing image: $imageTag" -ForegroundColor Cyan
        docker push $imageTag
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host ""
            Write-Host "Image pushed successfully!" -ForegroundColor Green
        } else {
            Write-Host ""
            Write-Host "Push failed with exit code $LASTEXITCODE" -ForegroundColor Red
            exit $LASTEXITCODE
        }
    } else {
        Write-Host ""
        Write-Host "To push the image, run:" -ForegroundColor Cyan
        Write-Host "  docker push $imageTag" -ForegroundColor White
        Write-Host "Or re-run with the -Push flag" -ForegroundColor Cyan
    }
}
catch {
    Write-Host ""
    Write-Host "Error during build: $_" -ForegroundColor Red
    exit 1
}
