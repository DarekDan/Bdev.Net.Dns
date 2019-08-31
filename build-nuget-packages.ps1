Param(
[Parameter(Mandatory=$True)]
[string]$versionSuffix
)
cd .\BDev.Net.Dns
Write-Output "Building release $versionSuffix nuget packages..."
dotnet pack --configuration Release --include-symbols --version-suffix $versionSuffix
Write-Output "Moving $versionSuffix nuget packages to releases folder..."
If(!(test-path ..\releases))
{
      New-Item -ItemType Directory -Force -Path ..\releases
}
Move-Item .\bin\Release\*.nupkg ..\releases -Force
Write-Output "Done."
