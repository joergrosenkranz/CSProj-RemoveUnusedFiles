# Show or remove unused files in a VS 2015 or VS 2017 project

[![Build Status](https://dev.azure.com/joergrosenkranz/Build-CSProj-RemoveUnusedFiles/_apis/build/status/joergrosenkranz.CSProj-RemoveUnusedFiles?branchName=master)](https://dev.azure.com/joergrosenkranz/Build-CSProj-RemoveUnusedFiles/_build/latest?definitionId=2?branchName=master)

The tool can be used to show or remove `.csproj`or `.resx`files that are not referenced by
the supplied `.csproj` file. This is usefull when migrating to the new .NET Core project 
format that references these files per globbing rules.
