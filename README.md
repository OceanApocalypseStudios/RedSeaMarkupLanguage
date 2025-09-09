# Red Sea Markup Language
<!--suppress HtmlDeprecatedAttribute -->
<img src="Assets/FullSizeLogo.png" alt="RSML Logo" align="right" width="100">

[![NuGet Version](https://img.shields.io/nuget/v/RSML?style=for-the-badge&logo=nuget&link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FRSML)](https://www.nuget.org/packages/RSML)
[![GitHub Release](https://img.shields.io/github/v/release/OceanApocalypseStudios/RedSeaMarkupLanguage?sort=semver&style=for-the-badge&color=orange)](https://github.com/OceanApocalypseStudios/RedSeaMarkupLanguage/releases/latest)
[![GitHub License](https://img.shields.io/github/license/OceanApocalypseStudios/RedSeaMarkupLanguage?style=for-the-badge)](https://raw.githubusercontent.com/OceanApocalypseStudios/RedSeaMarkupLanguage/main/LICENSE)
![GitHub top language](https://img.shields.io/github/languages/top/OceanApocalypseStudios/RedSeaMarkupLanguage?style=for-the-badge&logo=dotnet&logoSize=auto&color=darkgreen)
![GitHub Repo stars](https://img.shields.io/github/stars/OceanApocalypseStudios/RedSeaMarkupLanguage?style=for-the-badge&logo=github&color=yellow)
![NuGet Downloads](https://img.shields.io/nuget/dt/RSML?style=for-the-badge&logo=nuget&color=red)

> The modern fork of [MF's Crossroad](https://github.com/MF366-Coding/MFRoad) we're sure you'll love.

---

## RSML v2.0.0 is here. What's next?
- [ ] Finishing a stable version of `RSML.Native`
- [ ] Creating a Python package for RSML
- [ ] Creating documentation

---

## CLI v2.0.0: An Improved Experience
The CLI now has a **lot** more power. You can evaluate and tokenize RSML directly from the commandline and adjust things like what machine it's evaluating for, via JSON.

---

## Shells and JSON (CLI Issue)
We encountered issues with JSON parsing via commandline arguments in certain shells, where even escaping quotes failed.

We present the solutions to said issues here.

Speaking of JSON, the schema for **local-machine parsing** can be found [**here**](https://oceanapocalypsestudios.org/schemas/rsml_cli_machine_schema.json).

### Bash
Bash did not present any issues.

```bash
./RSML.CLI.exe evaluate -m "{ \"processor\": { \"architecture\": \"arm64\" } }"
```

### PowerShell
PowerShell presented a weird issue, where the quotes enveloping the property names seemed to vanish. Even escaping or introducing a here-string failed.

The solution was a weird one, since usually escaping via `""` is done on CMD only.

```powershell
.\RSML.CLI.exe evaluate -m '{ ""test"": ""value"" }'
```

### CMD
The CMD did not present any issues.

```batch
.\RSML.CLI.exe evaluate -m "{ ""test"": ""value"" }"
```

---


## See Also
<ul>
	<li>
		<img src="https://raw.githubusercontent.com/vscode-icons/vscode-icons/refs/heads/master/icons/file_type_csharp.svg" alt="C# Logo" width="20" style="vertical-align: middle;" /> <a href="LANGUAGES.md">Programming Language Support</a>
	</li>
	<li>
		<img src="https://raw.githubusercontent.com/vscode-icons/vscode-icons/refs/heads/master/icons/file_type_sln.svg" alt="Visual Studio Logo" width="20" style="vertical-align: middle;" /> <a href="EDITOR.md">Editor Support</a>
	</li>
	<li>
		<img src="https://raw.githubusercontent.com/dotnet/BenchmarkDotNet/refs/heads/master/docs/logo/icon.svg" alt="BenchmarkDotNet Icon" width="20" style="vertical-align: middle;" /> <a href="BENCHMARKS.md">Benchmarks</a>
	</li>
</ul>

<hr />

> **Copyright (c) 2025 OceanApocalypseStudios**
>
> We :heart: open-source!
