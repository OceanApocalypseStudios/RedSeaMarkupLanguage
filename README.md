#
<div align="center">
	<img src="assets/logo.png" alt="RSML Logo" align="center" width="150">
	<h1>Red Sea Markup Language</h1>
</div>
<div align="center">
	<a href="https://www.nuget.org/packages/OceanApocalypseStudios.RSML" target="_blank"><img src="https://img.shields.io/nuget/v/OceanApocalypseStudios.RSML?style=for-the-badge&logo=nuget&logoColor=white&logoSize=auto&label=Available%20on%20NuGet&labelColor=%231265fb&color=%2308317b"/></a>
	<a href="https://github.com/OceanApocalypseStudios/RedSeaMarkupLanguage/releases/latest"><img src="https://img.shields.io/github/v/release/OceanApocalypseStudios/RedSeaMarkupLanguage?sort=semver&display_name=tag&style=for-the-badge&logo=github&logoColor=white&logoSize=auto&label=Latest&labelColor=%23161616&color=%23000308"/></a>
	<!--<a href="COPYING.txt"><img src="https://img.shields.io/badge/ignored-Custom_clause-ignored?style=for-the-badge&logo=opensourceinitiative&logoColor=white&logoSize=auto&label=repository%20licensing&labelColor=%23496300&color=%232b3c00"/></a>-->
	<a href="LICENSE.txt"><img src="https://img.shields.io/badge/ignored-mit-ignored?style=for-the-badge&logo=opensourceinitiative&logoColor=white&logoSize=auto&label=code%20license&labelColor=%23496300&color=%232b3c00"/></a>
	<a href="LICENSE-DOCS.txt"><img src="https://img.shields.io/badge/ignored-cc0_1.0_universal-ignored?style=for-the-badge&logo=opensourceinitiative&logoColor=white&logoSize=auto&label=documentation%20license&labelColor=%23496300&color=%232b3c00"/></a>
	<img src="https://img.shields.io/github/languages/top/OceanApocalypseStudios/RedSeaMarkupLanguage?style=for-the-badge&logo=dotnet&logoColor=white&logoSize=auto&label=%20&labelColor=%234929ca&color=%234929ca" />
	<img src="https://img.shields.io/github/stars/OceanApocalypseStudios/RedSeaMarkupLanguage?style=for-the-badge&logo=star&logoColor=black&logoSize=auto&labelColor=%2379610b&color=%23413405" />
	<img src="https://img.shields.io/nuget/dt/OceanApocalypseStudios.RSML?style=for-the-badge&logo=nuget&logoColor=white&logoSize=auto&label=downloads%20(modern)&labelColor=%234929ca&color=%2327156f" />
	<img src="https://img.shields.io/nuget/dt/RSML?style=for-the-badge&logo=nuget&logoColor=white&logoSize=auto&label=Downloads%20(Legacy)&labelColor=%23ad4734&color=%236e2d20" />
	<a href="https://marketplace.visualstudio.com/items?itemName=OceanApocalypseStudios.oas-rsml-support-vs" target="_blank"><img src="https://img.shields.io/badge/RSML_for_Visual_Studio-b35ff2?style=for-the-badge"></a>
	<a href="https://marketplace.visualstudio.com/items?itemName=OceanApocalypseStudios.rsml" target="_blank"><img src="https://img.shields.io/badge/RSML_for_VS_Code-1f9cf0?style=for-the-badge"></a>

</div>
<hr/>
<div align="center">
<strong style="font-size: small">
An OceanApocalypseStudios project,
</strong>
<p>
the modern language designed to dynamically interpret different logic paths based on an host's OS and CPU architecture.
</p>
</div>

---

## Contents
- [Red Sea Markup Language (RSML)](#section)
	- [Why RSML?](#why-rsml)
    - [How to build RSML?](#how-to-build-rsml)

<details open>
<summary><strong>Where's the "How to use" section?</strong></summary>

You can find full documentation [here](https://oceanapocalypsestudios.org/rsml-docs/).

</details>

<details open>
<summary><strong>How can I see what is being worked on in RSML?</strong></summary>

You can find the official bug tracker and roadmap [here](https://github.com/orgs/OceanApocalypseStudios/projects/6/views/2).

</details>

---

## Why RSML?
RSML solves the issue of resolving logic paths dynamically based on a given host's characteristics.
When assigned an host, which could be the local host, RSML solves the logic paths and returns the first match's associated value.
The important part here is to note RSML does this dynamically: if you were to switch hosts or pass different data, RSML would adapt accordingly.

**Still unsure about RSML?** You can find some usage examples [here](#examples).

---

## How to build RSML?
<details>
	<summary><strong>Debug build</strong></summary>

1. Clone [this repository](https://github.com/OceanApocalypseStudios/RedSeaMarkupLanguage).
2. Open the terminal in the root RSML directory (the one with the solution file).
	
```
dotnet build -c Debug RedSeaMarkupLanguage.slnx
./src/RSML.CLI/bin/Debug/net10.0/RSML.CLI.exe
```

</details>

<details>
	<summary><strong>Optimized build</strong></summary>

1. Clone [this repository](https://github.com/OceanApocalypseStudios/RedSeaMarkupLanguage).
2. Open the terminal in the root RSML directory (the one with the solution file).
	
```
dotnet build -c Release RedSeaMarkupLanguage.slnx
./src/RSML.CLI/bin/Release/net10.0/RSML.CLI.exe
```

</details>
<details open>
	<summary><strong>Native build</strong> <em>(compiles into a shared library)</em></summary>

1. Clone [this repository](https://github.com/OceanApocalypseStudios/RedSeaMarkupLanguage).
2. Open the terminal in the root RSML directory (the one with the solution file).
	
```
dotnet publish -c Release -r <rid> src/RSML.Native/RSML.Native.csproj
```

> [!WARNING]
> If you're on Windows and using Visual Studio, this will require both the .NET and C++ development workloads.
> If something goes wrong with the MSVC side of things, the fix is almost always repairing the Visual Studio installation *(backup settings and everything you need first)*.

Once that is complete, the DLL will be located at `src/RSML.Native/bin/<arch>/Release/net10.0/<rid>/publish/RSML.Native.dll`. The XML files in the same directory are purely for documentation purposes and the `.pdb` file only matters for debugging.

</details>

---

## RSML on NuGet Trends
> [!NOTE]
> In the image below, **RSML** refers to Legacy RSML (RSML v1.x.x), while **OceanApocalypseStudios.RSML** refers to
> Modern RSML (RSML v2.0.0).
> In any other context, RSML and OceanApocalypseStudios.RSML have the exact same meaning.

[![RSML on NuGet Trends](extras/NuGetTrends_July5.png)](https://nugettrends.com/packages?ids=RSML&ids=OceanApocalypseStudios.RSML&months=12)

---

## See Also
<ul>
	<li>
		<img src="https://raw.githubusercontent.com/vscode-icons/vscode-icons/refs/heads/master/icons/file_type_csharp.svg" alt="C# Logo" height="20" width="20" style="vertical-align: middle;" /> <a href="LANGUAGES.md">Programming Language Support</a>
	</li>
	<li>
		<img src="https://raw.githubusercontent.com/vscode-icons/vscode-icons/refs/heads/master/icons/file_type_sln.svg" alt="Visual Studio Logo" height="20" width="20" style="vertical-align: middle;" /> <a href="EDITOR.md">Editor Support</a>
	</li>
	<li>
		<img src="https://raw.githubusercontent.com/dotnet/BenchmarkDotNet/refs/heads/master/docs/logo/icon.svg" alt="BenchmarkDotNet Icon" height="20" width="20" style="vertical-align: middle;" /> <a href="BENCHMARKS.md">Benchmarks</a>
	</li>
</ul>

<hr />

> **Copyright (c) 2025 OceanApocalypseStudios**
>
> We :heart: open-source!
