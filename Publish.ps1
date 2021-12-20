# Publishes Nuget packages to various sources - should be run on master branch after feature branch has been merged
$nuget_version = Get-Content NugetVersion.txt
$package = $nuget_version[0]
$version = $nuget_version[1]
$path = "./pkg/$($package).$($version).nupkg"

dotnet build -c Release
dotnet pack -c Release -o ./pkg /p:Version=$version

# Nuget.org
# nuget push $path $env:WVN_NUGET_API_KEY -Source 'nuget.org'

# WVN github - delete does not work on CLI
# nuget delete $package $version -ApiKey $env:WVN_GITHUB_API_KEY -Source 'github' -NonInteractive
dotnet nuget push $path  --source 'github' --api-key $env:WVN_GITHUB_API_KEY

# Create version tag in source control
git tag $version
git push --tags