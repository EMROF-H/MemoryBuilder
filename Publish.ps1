param(
    [Parameter(Mandatory = $true)]
    [string]$Version,

    [Parameter(Mandatory = $true)]
    [string]$Target  # Local folder path or NuGet API key
)

function ThrowIfFailed($message) {
    if (!$?) {
        Write-Error "❌ $message"
        exit 1
    }
}

$Root = "$PSScriptRoot"
$nupkgOut = "$Root\nupkgs"
$localRepo = $Target
$isRemotePublish = -not (Test-Path $Target)

$AttributesProj = "$Root\MemoryBuilder.Attributes\MemoryBuilder.Attributes.csproj"
$GeneratorProj  = "$Root\MemoryBuilder.Generator\MemoryBuilder.Generator.csproj"
$MainProj       = "$Root\MemoryBuilder\MemoryBuilder.csproj"

$Projects = @($AttributesProj, $GeneratorProj, $MainProj)

Write-Host "`n🔧 Setting <Version>$Version</Version> in all .csproj files..."
foreach ($proj in $Projects) {
    (Get-Content $proj) -replace '<Version>.*?</Version>', "<Version>$Version</Version>" |
            Set-Content $proj
}
ThrowIfFailed "Failed to set version in project files."

Write-Host "`n🧹 Running dotnet clean..."
dotnet clean $Root
ThrowIfFailed "dotnet clean failed."

Remove-Item -Recurse -Force $nupkgOut -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Path $nupkgOut | Out-Null

if ($isRemotePublish) {
    Write-Host "`n🚡 You are about to publish to NuGet.org!"
    $confirmation = Read-Host "Type 'yes' to confirm"
    if ($confirmation -ne "yes") {
        Write-Error "❌ Remote publish aborted by user."
        exit 1
    }

    Write-Host "`n🧪 Performing remote publish checks..."

    $branch = git rev-parse --abbrev-ref HEAD
    if ($branch -ne "main") {
        Write-Error "❌ Current branch is '$branch'. You must be on 'main'."
        exit 1
    }

    $status = git status --porcelain
    if ($status) {
        Write-Error "❌ Uncommitted changes detected."
        exit 1
    }

    $localHash = (& git rev-parse HEAD).Trim()
    $remoteRaw = & git rev-parse "@{u}" 2>$null
    if (-not $remoteRaw) {
        Write-Error "❌ Current branch has no upstream set. Use 'git push -u origin <branch>' first."
        exit 1
    }
    $remoteHash = $remoteRaw.Trim()
    if ($localHash -ne $remoteHash) {
        Write-Error "❌ Local and remote branches are not in sync."
        exit 1
    }

    $tagExists = git tag --list "V$Version"
    if ($tagExists) {
        Write-Error "❌ Tag V$Version already exists."
        exit 1
    }

    git tag "V$Version"
    git push origin "V$Version"
    ThrowIfFailed "Failed to push git tag."

    Write-Host "`n📆 Building and packing for release..."
    dotnet build $Root -c Release -p:UsePackageReference=true
    ThrowIfFailed "dotnet build failed."

    foreach ($proj in $Projects) {
        dotnet pack $proj -c Release -p:UsePackageReference=true -o $nupkgOut
        ThrowIfFailed "dotnet pack failed for $proj"
    }

    $Packages = Get-ChildItem "$nupkgOut\*.nupkg"
    Write-Host "`n🌐 Pushing packages to NuGet.org..."
    foreach ($pkg in $Packages) {
        dotnet nuget push $pkg.FullName `
            --api-key $Target `
            --source https://api.nuget.org/v3/index.json `
            --skip-duplicate
        ThrowIfFailed "Failed to push $($pkg.Name)"
    }

    Write-Host "✅ Remote publish complete!"
} else {
    Write-Host "`n📆 Building and packing dependencies for local publish..."

    dotnet build $AttributesProj -c Release
    dotnet pack $AttributesProj -c Release -o $nupkgOut

    dotnet build $GeneratorProj -c Release
    dotnet pack $GeneratorProj -c Release -o $nupkgOut

    New-Item -ItemType Directory -Force -Path $localRepo | Out-Null
    Get-ChildItem "$nupkgOut\MemoryBuilder.*.nupkg" | ForEach-Object {
        Copy-Item $_.FullName -Destination $localRepo -Force
    }

    dotnet restore $MainProj -s $localRepo
    dotnet build $MainProj -c Release -p:UsePackageReference=true
    dotnet pack  $MainProj -c Release -p:UsePackageReference=true -o $nupkgOut

    Copy-Item "$nupkgOut\MemoryBuilder.*.nupkg" -Destination $localRepo -Force
    Write-Host "✅ Local publish complete at: $localRepo"
}

Write-Host "`n🎉 Publish completed for version $Version!"
