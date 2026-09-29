$ErrorActionPreference = "Stop"
$RepositoryRoot = Split-Path -Parent $PSScriptRoot
$Failures = New-Object System.Collections.Generic.List[string]
$SourceRoots = @("Scripts", "Tests", "Samples~")
$ExcludedFolders = '(^|[\\/])(\.git|Library|Temp|Logs|obj)([\\/]|$)'

function Get-RelativePath([string]$Path) {
    return $Path.Substring($RepositoryRoot.Length).TrimStart('\', '/')
}

$SourceFiles = foreach ($SourceRoot in $SourceRoots) {
    $AbsoluteRoot = Join-Path $RepositoryRoot $SourceRoot
    if (-not (Test-Path -LiteralPath $AbsoluteRoot)) {
        $Failures.Add("Missing source root: $SourceRoot")
        continue
    }
    Get-ChildItem -LiteralPath $AbsoluteRoot -Recurse -Filter "*.cs" -File
}

foreach ($File in $SourceFiles) {
    $RelativePath = Get-RelativePath $File.FullName
    $Content = [System.IO.File]::ReadAllText($File.FullName)
    $LineCount = ($Content -split "`n").Count

    if ($LineCount -gt 400) {
        $Failures.Add("Source exceeds 400 lines: $RelativePath ($LineCount)")
    }
    if ($Content -match '(?m)^\s*//|/\*|\*/') {
        $Failures.Add("Source comment found: $RelativePath")
    }
    if ($Content -match '(?i)\bTODO\b|\bFIXME\b|\bHACK\b') {
        $Failures.Add("Deferred-work marker found: $RelativePath")
    }
    if ($Content.Contains("`r")) {
        $Failures.Add("CRLF line ending found: $RelativePath")
    }
    if ($Content.Length -gt 0 -and [int]$Content[0] -eq 0xFEFF) {
        $Failures.Add("Byte order mark found: $RelativePath")
    }
    foreach ($Match in [regex]::Matches($Content, '(?m)^\s*namespace\s+([^\s{]+)')) {
        if (-not $Match.Groups[1].Value.StartsWith("MvvmUnity", [System.StringComparison]::Ordinal)) {
            $Failures.Add("Unexpected namespace in ${RelativePath}: $($Match.Groups[1].Value)")
        }
    }
}

$JsonFiles = Get-ChildItem -LiteralPath $RepositoryRoot -Recurse -File |
    Where-Object { ($_.Extension -eq ".json" -or $_.Extension -eq ".asmdef") -and (Get-RelativePath $_.FullName) -notmatch $ExcludedFolders }
foreach ($File in $JsonFiles) {
    try {
        [System.IO.File]::ReadAllText($File.FullName) | ConvertFrom-Json | Out-Null
    }
    catch {
        $Failures.Add("Invalid JSON: $(Get-RelativePath $File.FullName)")
    }
}

$MetaRoots = $SourceRoots | ForEach-Object { Join-Path $RepositoryRoot $_ } | Where-Object { Test-Path -LiteralPath $_ }
foreach ($Asset in (Get-ChildItem -LiteralPath $MetaRoots -Recurse -Force | Where-Object { $_.Name -notlike "*.meta" })) {
    if (-not (Test-Path -LiteralPath ($Asset.FullName + ".meta"))) {
        $Failures.Add("Missing meta file: $(Get-RelativePath $Asset.FullName)")
    }
}

$Guids = @{}
$MetaFiles = Get-ChildItem -LiteralPath $RepositoryRoot -Recurse -Force -Filter "*.meta" -File |
    Where-Object { (Get-RelativePath $_.FullName) -notmatch $ExcludedFolders }
foreach ($MetaFile in $MetaFiles) {
    $Match = [regex]::Match([System.IO.File]::ReadAllText($MetaFile.FullName), '(?m)^guid:\s*([0-9a-f]{32})\s*$')
    if (-not $Match.Success) {
        $Failures.Add("Invalid Unity GUID: $(Get-RelativePath $MetaFile.FullName)")
        continue
    }
    $Guid = $Match.Groups[1].Value
    if ($Guids.ContainsKey($Guid)) {
        $Failures.Add("Duplicate GUID ${Guid}: $(Get-RelativePath $Guids[$Guid]) and $(Get-RelativePath $MetaFile.FullName)")
    }
    else {
        $Guids[$Guid] = $MetaFile.FullName
    }
}

$Package = [System.IO.File]::ReadAllText((Join-Path $RepositoryRoot "package.json")) | ConvertFrom-Json
if ($Package.name -ne "com.mvvm.unity" -or $Package.version -notmatch '^\d+\.\d+\.\d+$') {
    $Failures.Add("Package identity or release version is invalid.")
}
foreach ($Sample in $Package.samples) {
    if (-not (Test-Path -LiteralPath (Join-Path $RepositoryRoot $Sample.path) -PathType Container)) {
        $Failures.Add("Missing declared sample: $($Sample.path)")
    }
}

$MarkdownFiles = Get-ChildItem -LiteralPath $RepositoryRoot -Recurse -Filter "*.md" -File |
    Where-Object { (Get-RelativePath $_.FullName) -notmatch $ExcludedFolders }
foreach ($MarkdownFile in $MarkdownFiles) {
    $Markdown = [System.IO.File]::ReadAllText($MarkdownFile.FullName)
    foreach ($Link in [regex]::Matches($Markdown, '\]\(([^)]+)\)|<img[^>]+src="([^"]+)"')) {
        $Target = if ($Link.Groups[1].Success) { $Link.Groups[1].Value } else { $Link.Groups[2].Value }
        if ($Target -match '^(https?://|mailto:|#)') {
            continue
        }
        $Target = [Uri]::UnescapeDataString(($Target -split '#', 2)[0])
        if (-not (Test-Path -LiteralPath (Join-Path $MarkdownFile.DirectoryName $Target))) {
            $Failures.Add("Broken documentation link in $(Get-RelativePath $MarkdownFile.FullName): $Target")
        }
    }
}

if (Test-Path -LiteralPath (Join-Path $RepositoryRoot ".git")) {
    Push-Location $RepositoryRoot
    $ErrorActionPreference = "Continue"
    try {
        $GitOutput = & git -c core.autocrlf=false diff --check 2>$null
        if ($LASTEXITCODE -ne 0) {
            $Failures.Add("git diff --check failed: $($GitOutput -join ' ')")
        }
    }
    finally {
        $ErrorActionPreference = "Stop"
        Pop-Location
    }
}

if ($Failures.Count -gt 0) {
    foreach ($Failure in $Failures) {
        [Console]::Error.WriteLine($Failure)
    }
    exit 1
}

Write-Host "MVVM Unity validation passed: $(@($SourceFiles).Count) C# files, $(@($JsonFiles).Count) JSON files, $($Guids.Count) unique Unity GUIDs."
