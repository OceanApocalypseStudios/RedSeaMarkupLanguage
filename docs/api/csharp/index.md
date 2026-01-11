# RSML in C\#
RSML for C# is a great and solid choice for learning to use RSML programmatically, as it has the best support.

## Installation
RSML can be installed in 2 ways: **package reference** (via NuGet) or **project reference** (building RSML inside your solution).

### Package Reference
Installing RSML via its NuGet package is the recommended way to install RSML, as it guarantees stability.

=== ".NET CLI"
    ```bash
    dotnet add package OceanApocalypseStudios.RSML --version 2.0.0
    ```

=== "Project File"
    ```xml title="ItemGroup"
    <PackageReference Include="OceanApocalypseStudios.RSML" Version="2.0.0" />
    ```

=== "Visual Studio (PMC)"
    ```powershell title="VS Package Manager Console"
    NuGet\Install-Package OceanApocalypseStudios.RSML -Version 2.0.0
    ```

=== "Central Package Management (CPM)"
    ```xml title="Directory.Packages.props"
    <PackageVersion Include="OceanApocalypseStudios.RSML" Version="2.0.0" />
    ```

    ```xml title="Project File"
    <PackageReference Include="OceanApocalypseStudios.RSML" />
    ```

### Project Reference _(Not recommended)_
If, for whatever reason, you need the latest nightly updates, you may also add the RSML project to your solution.

!!! warning
    Please keep in mind that not all nightly updates are guaranteed to be bug-free or stable. Nightly updates are also not documented, only stable versions are.

```bash
# Adapt this to your solution structure
cd src/MyAwesomeProject/
dotnet add reference ../../include/RSML/
```

???+ tip "Git Submodules"
    If your solution is included in a `git` repository already, consider using [Git Submodules](https://git-scm.com/docs/git-submodule) instead.
