#Requires -Version 7.0
<#
.SYNOPSIS
    Shared helper functions for CefRaw CI/CD workflows.
.DESCRIPTION
    Provides functions to fetch the Spotify CEF builds index, download and extract
    CEF binary distributions, and generate .csproj files for NuGet packaging.
#>

# Spotify CDN base URL
$Script:CefCdnBase = "https://cef-builds.spotifycdn.com"

# Platform → .NET RID mapping
$Script:PlatformToRid = @{
    "windows64"    = "win-x64"
    "windows32"    = "win-x86"
    "windowsarm64" = "win-arm64"
    "linux64"      = "linux-x64"
    "linux32"      = "linux-x86"
    "linuxarm64"   = "linux-arm64"
    "linuxarm32"   = "linux-arm"
    "macos64"      = "osx-x64"
    "macosarm64"   = "osx-arm64"
}

# CEF platform → OS category
$Script:PlatformToOs = @{
    "windows64"    = "win"
    "windows32"    = "win"
    "windowsarm64" = "win"
    "linux64"      = "linux"
    "linux32"      = "linux"
    "linuxarm64"   = "linux"
    "linuxarm32"   = "linux"
    "macos64"      = "mac"
    "macosarm64"   = "mac"
}

<#
.SYNOPSIS
    Fetches and parses the Spotify CEF builds index.
.DESCRIPTION
    Downloads https://cef-builds.spotifycdn.com/index.json and returns
    a hashtable of platform → build entries. Each build entry has:
    - FileName: the tar.bz2 filename
    - Sha1: SHA-1 hash
    - Size: file size in bytes
    - CefVersion: parsed CEF version (e.g., "151.3.11")
    - ChromiumVersion: parsed Chromium version
    - IsDebugSymbols: true if this is a debug symbols build
.EXAMPLE
    $builds = Get-CefBuildIndex
    $builds['windows64']['standard'][0]
#>
function Get-CefBuildIndex {
    [CmdletBinding()]
    param()

    $indexUrl = "$Script:CefCdnBase/index.json"
    Write-Host "Fetching CEF build index from $indexUrl"

    try {
        $response = Invoke-WebRequest -Uri $indexUrl -UseBasicParsing -ErrorAction Stop
        $index = $response.Content | ConvertFrom-Json -AsHashtable
    }
    catch {
        Write-Error "Failed to fetch or parse CEF build index: $_"
        throw
    }

    $result = @{}

    foreach ($platform in $index.Keys) {
        $platformBuilds = $index[$platform]
        $standard = @()
        $symbols = @()

        foreach ($fileName in $platformBuilds.Keys) {
            $entry = $platformBuilds[$fileName]
            $buildInfo = @{
                FileName        = $fileName
                Sha1            = $entry['sha1']
                Size            = $entry['size']
                IsDebugSymbols  = $fileName -match '_debug_symbols'
            }

            # Parse CEF version from filename
            # Pattern: cef_binary_{version}+g{hash}+chromium-{version}_{platform}.tar.bz2
            # or:      cef_binary_{version}+g{hash}+chromium-{version}_{platform}_debug_symbols.tar.bz2
            if ($fileName -match 'cef_binary_(\d+\.\d+\.\d+)\+g[0-9a-f]+\+chromium-(\d+\.\d+\.\d+\.\d+)') {
                $buildInfo.CefVersion = $Matches[1]
                $buildInfo.ChromiumVersion = $Matches[2]
            }
            elseif ($fileName -match 'cef_binary_(\d+\.\d+\.\d+)') {
                $buildInfo.CefVersion = $Matches[1]
                $buildInfo.ChromiumVersion = "unknown"
            }
            else {
                Write-Warning "Could not parse version from filename: $fileName"
                $buildInfo.CefVersion = "0.0.0"
                $buildInfo.ChromiumVersion = "unknown"
            }

            if ($buildInfo.IsDebugSymbols) {
                $symbols += $buildInfo
            }
            else {
                $standard += $buildInfo
            }
        }

        $result[$platform] = @{
            Standard = $standard
            Symbols  = $symbols
        }
    }

    return $result
}

<#
.SYNOPSIS
    Gets the latest build for a given platform and type.
.DESCRIPTION
    Returns the build entry with the highest CefVersion for the given platform.
    Type can be 'Standard' or 'Symbols'.
#>
function Get-LatestBuild {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$Platform,

        [Parameter(Mandatory = $true)]
        [ValidateSet('Standard', 'Symbols')]
        [string]$Type,

        [Parameter(Mandatory = $false)]
        [hashtable]$Index = $null
    )

    if (-not $Index) {
        $Index = Get-CefBuildIndex
    }

    $builds = $Index[$Platform][$Type]
    if (-not $builds -or $builds.Count -eq 0) {
        Write-Error "No $Type builds found for platform '$Platform'"
        return $null
    }

    # Sort by CefVersion (semantic version) and return the latest
    $latest = $builds | Sort-Object { [System.Version]$_.CefVersion } -Descending | Select-Object -First 1
    return $latest
}

<#
.SYNOPSIS
    Constructs the download URL for a CEF build.
#>
function Get-CefDownloadUrl {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$FileName
    )
    return "$Script:CefCdnBase/$FileName"
}

<#
.SYNOPSIS
    Downloads and extracts a CEF tar.bz2 archive.
.DESCRIPTION
    Downloads the specified CEF build archive and extracts it to the given output directory.
    Returns the path to the extracted directory.
#>
function Expand-CefArchive {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$FileName,

        [Parameter(Mandatory = $true)]
        [string]$OutputDirectory
    )

    $url = Get-CefDownloadUrl -FileName $FileName
    $archivePath = Join-Path $OutputDirectory $FileName

    Write-Host "Downloading $url"
    Write-Host "  to $archivePath"

    if (-not (Test-Path $OutputDirectory)) {
        New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null
    }

    # Download with progress
    try {
        $ProgressPreference = 'SilentlyContinue'
        Invoke-WebRequest -Uri $url -OutFile $archivePath -UseBasicParsing -ErrorAction Stop
        $ProgressPreference = 'Continue'
    }
    catch {
        Write-Error "Failed to download $url : $_"
        throw
    }

    Write-Host "Extracting $FileName"
    # tar -xjf works on Windows 10 1803+, Linux, and macOS
    $extractDir = Join-Path $OutputDirectory "extracted"
    if (Test-Path $extractDir) {
        Remove-Item -Recurse -Force $extractDir
    }
    New-Item -ItemType Directory -Path $extractDir -Force | Out-Null

    tar -xjf $archivePath -C $extractDir 2>&1 | Out-Null
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to extract $archivePath (exit code: $LASTEXITCODE)"
        throw
    }

    # The archive contains a single top-level directory
    $innerDir = Get-ChildItem -Path $extractDir -Directory | Select-Object -First 1
    if (-not $innerDir) {
        Write-Error "Extracted archive has unexpected structure (no inner directory)"
        throw
    }

    Write-Host "Extracted to $($innerDir.FullName)"
    return $innerDir.FullName
}

<#
.SYNOPSIS
    Gets the .NET Runtime Identifier for a CEF platform.
#>
function Get-Rid {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$CefPlatform
    )
    if ($Script:PlatformToRid.ContainsKey($CefPlatform)) {
        return $Script:PlatformToRid[$CefPlatform]
    }
    Write-Error "Unknown CEF platform: $CefPlatform"
    return $null
}

<#
.SYNOPSIS
    Gets the OS category for a CEF platform (win/linux/mac).
#>
function Get-OsCategory {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$CefPlatform
    )
    if ($Script:PlatformToOs.ContainsKey($CefPlatform)) {
        return $Script:PlatformToOs[$CefPlatform]
    }
    Write-Error "Unknown CEF platform: $CefPlatform"
    return $null
}

<#
.SYNOPSIS
    Creates a .csproj NuGet package project for a set of CEF native binaries.
.DESCRIPTION
    Walks the extracted CEF build directory, enumerates native binaries and resources,
    and generates a .csproj file configured for NuGet packaging.

    Platform-specific layouts:
    - Windows/Linux: Native binaries at root, resources at root (pak files, icudtl.dat),
      locales in locales/ subfolder.
    - macOS: The framework is flattened — main binary and Libraries/*.dylib at root,
      resources in Resources/ subfolder, locales as Resources/{locale}.lproj/locale.pak.
.PARAMETER Platform
    The CEF platform name (e.g., "windows64", "linux64", "macosarm64").
.PARAMETER Configuration
    "Debug" or "Release".
.PARAMETER CefVersion
    The CEF version string to use as the NuGet package version.
.PARAMETER ExtractedDir
    Path to the extracted CEF build directory (the inner directory, not the {Config}/ subdir).
.PARAMETER OutputDir
    Directory where the .csproj project will be created.
.PARAMETER IsSymbols
    If true, this is a debug symbols package (no DLLs, only .pdb/.debug/.dSYM files).
#>
function New-CefBinariesProject {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$Platform,

        [Parameter(Mandatory = $true)]
        [ValidateSet('Debug', 'Release')]
        [string]$Configuration,

        [Parameter(Mandatory = $true)]
        [string]$CefVersion,

        [Parameter(Mandatory = $true)]
        [string]$ExtractedDir,

        [Parameter(Mandatory = $true)]
        [string]$OutputDir,

        [Parameter(Mandatory = $false)]
        [switch]$IsSymbols
    )

    $rid = Get-Rid -CefPlatform $Platform
    $os = Get-OsCategory -CefPlatform $Platform

    $configDir = Join-Path $ExtractedDir $Configuration
    $resourcesDir = Join-Path $ExtractedDir "Resources"

    # For macOS, everything is inside the framework bundle
    $isMacOS = $os -eq "mac"
    if ($isMacOS) {
        $frameworkName = "Chromium Embedded Framework.framework"
        $frameworkDir = Join-Path $configDir $frameworkName
        if (-not (Test-Path $frameworkDir)) {
            Write-Error "macOS framework not found at $frameworkDir"
            throw
        }
    }

    # Sanitize platform for package ID (capitalize first letter of each segment)
    $platformPascal = ($Platform -split '-' | ForEach-Object {
        if ($_.Length -gt 0) { $_.Substring(0,1).ToUpper() + $_.Substring(1) } else { $_ }
    }) -join ''

    # e.g., "RawCef.Binaries.Win64.Debug" or "RawCef.Binaries.MacosArm64.Symbols"
    if ($IsSymbols) {
        $packageId = "RawCef.Binaries.$platformPascal.Symbols"
        $description = "CEF $CefVersion debug symbols for $Platform ($Configuration)"
    }
    else {
        $packageId = "RawCef.Binaries.$platformPascal.$Configuration"
        $description = "CEF $CefVersion native binaries for $Platform ($Configuration)"
    }

    # Create project directory
    $projectDir = Join-Path $OutputDir $packageId
    if (Test-Path $projectDir) {
        Remove-Item -Recurse -Force $projectDir
    }
    New-Item -ItemType Directory -Path $projectDir -Force | Out-Null

    Write-Host "Creating project: $packageId"
    Write-Host "  Output dir: $projectDir"
    Write-Host "  Platform: $Platform, RID: $rid, OS: $os, Config: $Configuration"

    # Collect files to include in the package
    $contentItems = @()
    $nativeDir = Join-Path $projectDir "native"
    New-Item -ItemType Directory -Path $nativeDir -Force | Out-Null

    if ($IsSymbols) {
        # --- Symbol package: collect symbol files ---
        $symbolFiles = @()
        if (Test-Path $ExtractedDir) {
            # Windows: .pdb files at root of extracted symbols archive
            $symbolFiles += Get-ChildItem -Path $ExtractedDir -File -Filter "*.pdb" -ErrorAction SilentlyContinue
            # Linux: .debug files
            $symbolFiles += Get-ChildItem -Path $ExtractedDir -File -Filter "*.debug" -ErrorAction SilentlyContinue
            # macOS: .dSYM bundles
            $symbolFiles += Get-ChildItem -Path $ExtractedDir -Directory -Filter "*.dSYM" -ErrorAction SilentlyContinue
        }
        if ($symbolFiles.Count -eq 0) {
            Write-Warning "No symbol files found in $ExtractedDir"
        }
        foreach ($file in $symbolFiles) {
            $destDir = if ($file -is [System.IO.DirectoryInfo]) {
                Join-Path $nativeDir $file.Name
            } else {
                $nativeDir
            }
            if (-not (Test-Path $destDir)) {
                New-Item -ItemType Directory -Path $destDir -Force | Out-Null
            }
            if ($file -is [System.IO.DirectoryInfo]) {
                Copy-Item -Recurse -Force $file.FullName $destDir
            } else {
                Copy-Item -Force $file.FullName $destDir
            }
            $contentItems += @{
                Source = $file.FullName
                IsDir  = $file -is [System.IO.DirectoryInfo]
                Name   = $file.Name
            }
        }
    }
    else {
        # --- Standard package: collect native binaries and resources ---
        if ($isMacOS) {
            # macOS: framework → flattened
            $fwRes = Join-Path $frameworkDir "Resources"
            $fwLib = Join-Path $frameworkDir "Libraries"

            # Main framework binary → copy to native/ as the main lib
            $fwBinary = Join-Path $frameworkDir "Chromium Embedded Framework"
            if (Test-Path $fwBinary) {
                Copy-Item -Force $fwBinary $nativeDir
                Write-Host "  Copied: Chromium Embedded Framework"
                $contentItems += @{ Source = $fwBinary; IsDir = $false; Name = "Chromium Embedded Framework" }
            }

            # Libraries → copy to native/
            if (Test-Path $fwLib) {
                foreach ($lib in Get-ChildItem -Path $fwLib -File) {
                    Copy-Item -Force $lib.FullName $nativeDir
                    Write-Host "  Copied: Libraries/$($lib.Name)"
                    $contentItems += @{ Source = $lib.FullName; IsDir = $false; Name = $lib.Name }
                }
                # Also copy non-dylib files from Libraries (like .json)
                foreach ($f in Get-ChildItem -Path $fwLib) {
                    if ($f -is [System.IO.FileInfo]) { continue } # already handled above
                }
            }

            # Resources → copy to native/Resources/ (macOS convention)
            $resDest = Join-Path $nativeDir "Resources"
            if (-not (Test-Path $resDest)) {
                New-Item -ItemType Directory -Path $resDest -Force | Out-Null
            }
            if (Test-Path $fwRes) {
                # Top-level resource files (.pak, .dat, .bin, .plist)
                foreach ($res in Get-ChildItem -Path $fwRes -File) {
                    Copy-Item -Force $res.FullName $resDest
                    Write-Host "  Copied: Resources/$($res.Name)"
                    $contentItems += @{ Source = $res.FullName; IsDir = $false; Name = "Resources/$($res.Name)" }
                }
                # Locale .lproj directories
                foreach ($lproj in Get-ChildItem -Path $fwRes -Directory -Filter "*.lproj") {
                    $lprojDest = Join-Path $resDest $lproj.Name
                    Copy-Item -Recurse -Force $lproj.FullName $lprojDest
                    Write-Host "  Copied: Resources/$($lproj.Name)/"
                    # Add each file inside the lproj
                    foreach ($lf in Get-ChildItem -Path $lproj.FullName -File) {
                        $contentItems += @{
                            Source = $lf.FullName
                            IsDir  = $false
                            Name   = "Resources/$($lproj.Name)/$($lf.Name)"
                        }
                    }
                }
            }
        }
        else {
            # Windows/Linux: native files at root, resources at root
            if (-not (Test-Path $configDir)) {
                Write-Error "Configuration directory not found: $configDir"
                throw
            }

            # Copy all files from {Config}/ to native/
            foreach ($file in Get-ChildItem -Path $configDir -File) {
                Copy-Item -Force $file.FullName $nativeDir
                Write-Host "  Copied: $($file.Name)"
                $contentItems += @{ Source = $file.FullName; IsDir = $false; Name = $file.Name }
            }

            # Copy Resources/ if it exists
            # On Windows/Linux: resources go to root level, locales to locales/
            if (Test-Path $resourcesDir) {
                # Top-level resource files (.pak, .dat) → root of native/
                foreach ($res in Get-ChildItem -Path $resourcesDir -File) {
                    Copy-Item -Force $res.FullName $nativeDir
                    Write-Host "  Copied: Resources/$($res.Name)"
                    $contentItems += @{ Source = $res.FullName; IsDir = $false; Name = $res.Name }
                }
                # locales/ subfolder → native/locales/
                $localesDir = Join-Path $resourcesDir "locales"
                if (Test-Path $localesDir) {
                    $localesDest = Join-Path $nativeDir "locales"
                    if (-not (Test-Path $localesDest)) {
                        New-Item -ItemType Directory -Path $localesDest -Force | Out-Null
                    }
                    foreach ($loc in Get-ChildItem -Path $localesDir -File) {
                        Copy-Item -Force $loc.FullName $localesDest
                        $contentItems += @{
                            Source = $loc.FullName
                            IsDir  = $false
                            Name   = "locales/$($loc.Name)"
                        }
                    }
                }
            }
        }
    }

    # Generate the .csproj
    $csprojPath = Join-Path $projectDir "$packageId.csproj"
    Write-Host "Generating $csprojPath"

    $csprojContent = @"
<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <TargetFramework>netstandard2.0</TargetFramework>
        <RootNamespace>$packageId</RootNamespace>
        <PackageId>$packageId</PackageId>
        <Version>$CefVersion</Version>
        <Description>$description</Description>
        <IncludeBuildOutput>false</IncludeBuildOutput>
        <SuppressDependenciesWhenPacking>true</SuppressDependenciesWhenPacking>
        <NoWarn>$(NoWarn);NU5100;NU5104</NoWarn>
    </PropertyGroup>

    <ItemGroup>
"@

    foreach ($item in $contentItems) {
        $pkgPath = if ($IsSymbols) {
            "runtimes/$rid/native/$($item.Name)"
        } else {
            "runtimes/$rid/native/$($item.Name)"
        }
        # Replace backslashes with forward slashes for cross-platform compat
        $pkgPath = $pkgPath.Replace('\', '/')
        $itemName = $item.Name.Replace('\', '/')

        $csprojContent += @"
        <Content Include="native\$itemName">
            <Pack>true</Pack>
            <PackagePath>$pkgPath</PackagePath>
            <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
        </Content>
"@
    }

    $csprojContent += @"
    </ItemGroup>

</Project>
"@

    Set-Content -Path $csprojPath -Value $csprojContent -Encoding UTF8

    Write-Host "Project created with $($contentItems.Count) content items"
    return @{
        ProjectDir  = $projectDir
        CsprojPath  = $csprojPath
        PackageId   = $packageId
        Version     = $CefVersion
    }
}

<#
.SYNOPSIS
    Packs a .csproj into a .nupkg and optionally pushes to NuGet feeds.
#>
function Publish-NuGetPackage {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$ProjectDir,

        [Parameter(Mandatory = $true)]
        [string]$OutputDirectory,

        [Parameter(Mandatory = $false)]
        [switch]$NoPush
    )

    Write-Host "Packing project in $ProjectDir"
    $packResult = dotnet pack $ProjectDir --configuration Release --output $OutputDirectory 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Error "dotnet pack failed: $packResult"
        throw
    }
    Write-Host $packResult

    $nupkgFiles = Get-ChildItem -Path $OutputDirectory -Filter "*.nupkg"
    Write-Host "Generated packages:"
    foreach ($nupkg in $nupkgFiles) {
        Write-Host "  $($nupkg.Name) ($([math]::Round($nupkg.Length / 1MB, 2)) MB)"
    }

    return $nupkgFiles
}

Write-Host "CEF Builds Helper Module loaded."
