mkdir Nuget
nuget pack -IncludeReferencedProjects SAS.Jakyl.Core/SAS.Jakyl.Core.csproj -OutputDirectory Nuget
nuget pack -IncludeReferencedProjects SAS.Jakyl/SAS.Jakyl.csproj -OutputDirectory Nuget