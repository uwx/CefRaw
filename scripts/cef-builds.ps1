<#
.SYNOPSIS
    Shared helper functions for CefRaw CI/CD workflows.
.DESCRIPTION
    Provides functions to fetch the Spotify CEF builds index, download and extract
    CEF binary distributions, and generate .csproj files for NuGet packaging.
#>

# Spotify CDN base URL
$Script:CefCdnBase = "https://cef-builds.spotifycdn.com"

# Platform → .NET RID mapping (matches Spotify CDN platform names)
$Script:PlatformToRid = @{
    "windows64"    = "win-x64"
    "windows32"    = "win-x86"
    "windowsarm64" = "win-arm64"
    "linux64"      = "linux-x64"
    "linux32"      = "linux-x86"
    "linuxarm64"   = "linux-arm64"
    "linuxarm"     = "linux-arm"
    "macosx64"     = "osx-x64"
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
    "linuxarm"     = "linux"
    "macosx64"     = "mac"
    "macosarm64"   = "mac"
}

<#
.SYNOPSIS
    Fetches and parses the Spotify CEF builds index.
.DESCRIPTION
    Downloads https://cef-builds.spotifycdn.com/index.json and returns
    a structured object. The index.json schema is:
      { "<platform>": { "versions": [ { "cef_version": "...", "channel": "...",
        "chromium_version": "...", "files": [ { "name": "...", "sha1": "...",
        "size": n, "type": "<standard|minimal|client|debug_symbols|release_symbols|signed|tools>",
        "last_modified": "..." } ], "sandbox_compat": "..." } ] } }
    Returns a hashtable: platform → @{ Versions = @(...) }
    Each version entry: @{ CefVersion, ChromiumVersion, Channel, Files = @(...) }
    Each file entry: @{ Name, Sha1, Size, Type, LastModified }
.EXAMPLE
    $index = Get-CefBuildIndex
    $index['windows64'].Versions[0].Files  # all files for latest windows64 version
#>
function Get-CefBuildIndex {
    [CmdletBinding()]
    param()

    $indexUrl = "$Script:CefCdnBase/index.json"
    Write-Host "Fetching CEF build index from $indexUrl"

    try {
        $response = Invoke-WebRequest -Uri $indexUrl -UseBasicParsing -ErrorAction Stop
        $raw = $response.Content | ConvertFrom-Json
    }
    catch {
        Write-Error "Failed to fetch or parse CEF build index: $_"
        throw
    }

    $result = @{}

    foreach ($platform in $raw.PSObject.Properties.Name) {
        $platformData = $raw.$platform
        $versions = @()

        foreach ($v in $platformData.versions) {
            # Parse pure CEF version from "151.3.12+gd9cea67+chromium-151.0.7922.47"
            $cefVersionStr = $v.cef_version
            $pureCefVersion = "0.0.0"
            if ($cefVersionStr -match '^(\d+\.\d+\.\d+)') {
                $pureCefVersion = $Matches[1]
            }

            $files = @()
            foreach ($f in $v.files) {
                $files += @{
                    Name         = $f.name
                    Sha1         = $f.sha1
                    Size         = $f.size
                    Type         = $f.type
                    LastModified = $f.last_modified
                }
            }

            $versions += @{
                CefVersion       = $pureCefVersion
                CefVersionFull   = $cefVersionStr
                ChromiumVersion  = $v.chromium_version
                Channel          = $v.channel
                SandboxCompat    = $v.sandbox_compat
                Files            = $files
            }
        }

        $result[$platform] = @{
            Versions = $versions
        }
    }

    return $result
}

<#
.SYNOPSIS
    Gets the latest build file for a given platform and file type.
.DESCRIPTION
    Returns the file entry (Name, Sha1, Size, etc.) for the latest version of the
    given platform, filtered by the specified file type.
    Valid file types: standard, minimal, client, debug_symbols, release_symbols, signed, tools.
.PARAMETER Platform
    CEF platform name (e.g., "windows64", "linux64", "macosarm64").
.PARAMETER FileType
    File type to filter by (e.g., "standard", "debug_symbols", "release_symbols").
.PARAMETER Index
    Optional pre-fetched index (avoids re-fetching).
.EXAMPLE
    $file = Get-LatestBuild -Platform "windows64" -FileType "standard"
    $file.Name  # "cef_binary_151.3.12+..._windows64.tar.bz2"
#>
function Get-LatestBuild {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$Platform,

        [Parameter(Mandatory = $true)]
        [string]$FileType,

        [Parameter(Mandatory = $false)]
        [hashtable]$Index = $null
    )

    if (-not $Index) {
        $Index = Get-CefBuildIndex
    }

    if (-not $Index.ContainsKey($Platform)) {
        Write-Error "Platform '$Platform' not found in CEF build index"
        return $null
    }

    $versions = $Index[$Platform].Versions
    if (-not $versions -or $versions.Count -eq 0) {
        Write-Error "No versions found for platform '$Platform'"
        return $null
    }

    # Sort versions by CefVersion (descending) and take the latest
    $sortedVersions = $versions | Sort-Object { [System.Version]$_.CefVersion } -Descending

    foreach ($ver in $sortedVersions) {
        $matchingFile = $ver.Files | Where-Object { $_.Type -eq $FileType } | Select-Object -First 1
        if ($matchingFile) {
            # Return a combined result with both file and version info
            return @{
                Name             = $matchingFile.Name
                Sha1             = $matchingFile.Sha1
                Size             = $matchingFile.Size
                Type             = $matchingFile.Type
                LastModified     = $matchingFile.LastModified
                CefVersion       = $ver.CefVersion
                CefVersionFull   = $ver.CefVersionFull
                ChromiumVersion  = $ver.ChromiumVersion
                Channel          = $ver.Channel
            }
        }
    }

    Write-Warning "No '$FileType' file found in any version for platform '$Platform'"
    return $null
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
    $isMac = $os -eq "mac"
    if ($isMac) {
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
        if ($isMac) {
            # macOS: restructure framework for C# DllImport compatibility.
            # C# uses DllImport("libcef") which resolves to libcef.dylib, but
            # CEF ships a framework bundle. We flatten it as CefGlue does:
            #   - Chromium Embedded Framework → libcef.dylib (root)
            #   - Libraries/* → root
            #   - Resources/* → Resources/ subfolder
            #   - Resources/en.lproj/* → Resources/ (English locale, flattened)
            $fwRes = Join-Path $frameworkDir "Resources"
            $fwLib = Join-Path $frameworkDir "Libraries"

            # Main framework binary → libcef.dylib at root
            $fwBinary = Join-Path $frameworkDir "Chromium Embedded Framework"
            if (Test-Path $fwBinary) {
                $destLib = Join-Path $nativeDir "libcef.dylib"
                Copy-Item -Force $fwBinary $destLib
                Write-Host "  Copied: Chromium Embedded Framework → libcef.dylib"
                $contentItems += @{ Source = $fwBinary; IsDir = $false; Name = "libcef.dylib" }
            }

            # Libraries → root (libEGL.dylib, libGLESv2.dylib, etc.)
            if (Test-Path $fwLib) {
                foreach ($lib in Get-ChildItem -Path $fwLib -File) {
                    Copy-Item -Force $lib.FullName $nativeDir
                    Write-Host "  Copied: Libraries/$($lib.Name)"
                    $contentItems += @{ Source = $lib.FullName; IsDir = $false; Name = $lib.Name }
                }
            }

            # Resources → Resources/ subfolder
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

                # English locale → flat into Resources/ (as CefGlue does)
                $enLproj = Join-Path $fwRes "en.lproj"
                if (Test-Path $enLproj) {
                    foreach ($lf in Get-ChildItem -Path $enLproj -File) {
                        Copy-Item -Force $lf.FullName $resDest
                        Write-Host "  Copied: Resources/en.lproj/$($lf.Name) → Resources/"
                        $contentItems += @{ Source = $lf.FullName; IsDir = $false; Name = "Resources/$($lf.Name)" }
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
                # Only include base locale variants (skip _FEMININE, _MASCULINE,
                # _NEUTER gender variants — they bloat the package with 150+
                # unnecessary files).
                $localesDir = Join-Path $resourcesDir "locales"
                if (Test-Path $localesDir) {
                    $localesDest = Join-Path $nativeDir "locales"
                    if (-not (Test-Path $localesDest)) {
                        New-Item -ItemType Directory -Path $localesDest -Force | Out-Null
                    }
                    foreach ($loc in Get-ChildItem -Path $localesDir -File) {
                        # Skip gender-variant locale files
                        if ($loc.Name -match '_(FEMININE|MASCULINE|NEUTER)\.pak$') {
                            continue
                        }
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

    # Strip debug symbols from Linux binaries to reduce package size.
    # CEF distributes .so files with embedded DWARF symbols even in Release
    # builds — stripping can reduce libcef.so from ~1.5 GB to ~200 MB.
    # macOS dylib symbols are already small, so skip stripping there.
    if (-not $IsSymbols -and $os -eq 'linux') {
        Write-Host "Stripping debug symbols from Linux binaries..."

        $filesToStrip = @(Get-ChildItem -Path $nativeDir -File -ErrorAction SilentlyContinue | Where-Object {
            $_.Extension -eq '.so'
        })
        if ($filesToStrip.Count -eq 0) {
            Write-Host "  No .so files to strip"
        }
        else {
            # Detect ELF architecture from the first .so file so we can pick
            # the right cross-strip tool (the runner may not match the binary
            # arch). readelf -h prints the ELF header; Machine: shows e_machine.
            $sampleElf = ($filesToStrip | Select-Object -First 1).FullName
            $readelfOutput = & readelf -h $sampleElf 2>&1 | Out-String
            $global:LASTEXITCODE = 0
            # Use [regex]::Match to avoid $Matches pollution from earlier -match calls
            $machineMatch = [regex]::Match($readelfOutput, 'Machine:\s*(.+)')
            $elfMachine = if ($machineMatch.Success) { $machineMatch.Groups[1].Value.Trim() } else { '' }
            Write-Host "  Detected ELF architecture: $elfMachine"

            # Map ELF machine name to strip tool + required apt package
            $stripInfo = switch -Wildcard ($elfMachine) {
                'Advanced Micro Devices X86-64' { @{ Tool = 'strip'                     ; Pkg = $null } }
                'Intel 80386'                   { @{ Tool = 'strip'                     ; Pkg = $null } }
                'AArch64'                       { @{ Tool = 'aarch64-linux-gnu-strip'   ; Pkg = 'binutils-aarch64-linux-gnu' } }
                'ARM'                           { @{ Tool = 'arm-linux-gnueabihf-strip' ; Pkg = 'binutils-arm-linux-gnueabihf' } }
                default                         { @{ Tool = $null; Pkg = $null } }
            }

            if ($stripInfo.Tool) {
                # Install cross-binutils if needed
                if ($stripInfo.Pkg) {
                    Write-Host "  Installing cross-strip tool: $($stripInfo.Pkg)"
                    sudo apt-get update -qq 2>&1 | Out-Null
                    sudo apt-get install -y -qq $stripInfo.Pkg 2>&1 | Out-Null
                    if ($LASTEXITCODE -ne 0) {
                        Write-Warning "  Failed to install $($stripInfo.Pkg) — skipping strip."
                        $global:LASTEXITCODE = 0
                        $stripInfo = @{ Tool = $null; Pkg = $null }
                    }
                }

                if ($stripInfo.Tool) {
                    $stripTool = $stripInfo.Tool
                    Write-Host "  Using strip tool: $stripTool"

                    foreach ($file in $filesToStrip) {
                        $beforeSize = [math]::Round($file.Length / 1MB, 1)
                        & $stripTool --strip-debug $file.FullName 2>&1 | Out-Null
                        if ($LASTEXITCODE -eq 0) {
                            $afterSize = [math]::Round((Get-Item $file.FullName).Length / 1MB, 1)
                            Write-Host "  Stripped: $($file.Name) — $beforeSize MB → $afterSize MB"
                        }
                        else {
                            Write-Warning "  Strip failed for $($file.Name) ($beforeSize MB)"
                        }
                        $global:LASTEXITCODE = 0
                    }
                }
            }
            else {
                Write-Warning "  Unrecognised ELF machine '$elfMachine' — cannot strip, skipping."
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
        <Description>$description</Description>
        <IncludeBuildOutput>false</IncludeBuildOutput>
        <SuppressDependenciesWhenPacking>true</SuppressDependenciesWhenPacking>
        <NoWarn>`$(NoWarn);NU5100;NU5104</NoWarn>
    </PropertyGroup>

    <ItemGroup>
"@

    foreach ($item in $contentItems) {
        $pkgPath = "runtimes/$rid/native/$($item.Name)"
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

    # Generate the .targets file that copies native binaries from runtime
    # subdirectories to the root output directory at build time.
    $targetsPath = Join-Path $projectDir "$packageId.targets"
    $targetsContent = @"
<Project>
    <Target Name="CopyCefBinariesToOutputRoot"
            AfterTargets="CopyFilesToOutputDirectory"
            BeforeTargets="CopyNativeBinaries">
        <ItemGroup>
            <_CefNativeFiles Include="
                `$(MSBuildThisFileDirectory)../runtimes/*/native/**"
                Condition=" '%(Extension)' != '.targets' " />
        </ItemGroup>
        <Copy SourceFiles="@(_CefNativeFiles)"
              DestinationFolder="`$(OutputPath)"
              SkipUnchangedFiles="true"
              Condition=" '@(_CefNativeFiles)' != '' " />
        <Message Importance="low"
                 Text="Copied @(_CefNativeFiles->Count()) CEF native file(s) to `$(OutputPath)" />
    </Target>
</Project>
"@
    Set-Content -Path $targetsPath -Value $targetsContent -Encoding UTF8

    # Include the .targets in the package so it auto-imports into consumer projects
    $csprojContent += @"
        <Content Include="$targetsPath">
            <Pack>true</Pack>
            <PackagePath>build\$packageId.targets</PackagePath>
        </Content>
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
