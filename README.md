# Red Sea Markup Language
<img src="Assets/FullSizeLogo.png" alt="RSML Logo" align="right" width="96" height="96">

[![NuGet Version](https://img.shields.io/nuget/v/RSML?style=for-the-badge&logo=nuget&link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FRSML)](https://www.nuget.org/packages/RSML)
[![GitHub Release](https://img.shields.io/github/v/release/OceanApocalypseStudios/RedSeaMarkupLanguage?sort=semver&style=for-the-badge&color=orange)](https://github.com/OceanApocalypseStudios/RedSeaMarkupLanguage/releases/latest)
[![GitHub License](https://img.shields.io/github/license/OceanApocalypseStudios/RedSeaMarkupLanguage?style=for-the-badge)](https://raw.githubusercontent.com/OceanApocalypseStudios/RedSeaMarkupLanguage/main/LICENSE)
![GitHub top language](https://img.shields.io/github/languages/top/OceanApocalypseStudios/RedSeaMarkupLanguage?style=for-the-badge&logo=dotnet&logoSize=auto&color=darkgreen)
![GitHub Repo stars](https://img.shields.io/github/stars/OceanApocalypseStudios/RedSeaMarkupLanguage?style=for-the-badge&logo=github&color=yellow)
![NuGet Downloads](https://img.shields.io/nuget/dt/RSML?style=for-the-badge&logo=nuget&color=red)

> The modern fork of [MF's Crossroad](https://github.com/MF366-Coding/MFRoad) we're sure you'll love.

<hr />

## Coming Soon*er*: RSML v2.0.0
> [!NOTE]
> RSML v2.0.0 is coming first as a `prerelease`. To be exact, this will be `prerelease8` with no native interop support yet.

**Red Sea Markup Language v2.0.0** is currently **in the making** and will be a huge release, bringing native binding
support into the table _(this also means AOT "friendliness")_, which will let you use RSML in Python, Go, Rust _(if
you're about that)_, wherever you want, really _(that supports native interop)_.

RSML v2.0.0 will also improve the current API **a lot**, as v1.0.5 still has its fair share of issues, not to
mention [RSML for Python](https://github.com/OceanApocalypseStudios/RSML.Python)'s ones.

<hr />

## Programming Language Support
RSML v2.0.0 will support native interop by exporting a C ABI, meaning it can be used in any language that supports C interop.

However, to make it easier for people who are unexperienced with C, we plan on creating packages for popular languages so people can use high-level APIs instead of having to struggle with DLLs.

<table>
	<!-- Header -->
	<tr>
		<th>
			Languages
		</th>
		<th>
			Is available?
		</th>
		<th>
			Documentaion
		</th>
		<th>
			Is official?
		</th>
		<th>
			Package Manager
		</th>
		<th>
			GitHub Repository
		</th>
		<th>
			Other Links
		</th>
	</tr>
	<!-- Body -->
	<tr>
		<td>
			<img src="https://raw.githubusercontent.com/vscode-icons/vscode-icons/refs/heads/master/icons/file_type_csharp.svg" alt="C#" width="80" />
			<!-- Official C# logo
			<img src="https://learn.microsoft.com/en-us/dotnet/media/logo_csharp.png" alt="C#" width=75 />
			-->
		</td>
		<td>
			✔ As a pre-release.
		</td>
		<td>
			🔴 <a href="https://oceanapocalypsestudios.github.io/rsml-docs/api/csharp/">Out-of-date</a>
		</td>
		<td>
			✔
		</td>
		<td>
			<a href="https://www.nuget.org/packages/RSML">NuGet</a>
		</td>
		<td>
			<a href="https://github.com/OceanApocalypseStudios/RedSeaMarkupLanguage">Visit</a>
		</td>
		<td>
			---
		</td>
	</tr>
	<tr>
		<td>
			<img src="https://raw.githubusercontent.com/vscode-icons/vscode-icons/refs/heads/master/icons/file_type_fsharp.svg" alt="F#" width="80" />
		</td>
		<td>
			✔ As a pre-release.
		</td>
		<td>
			🟣 No specific documentation planned (refer to <strong>RSML for C#</strong> docs).
		</td>
		<td>
			✔
		</td>
		<td>
			<a href="https://www.nuget.org/packages/RSML">NuGet</a>
		</td>
		<td>
			<a href="https://github.com/OceanApocalypseStudios/RedSeaMarkupLanguage">Visit</a>
		</td>
		<td>
			---
		</td>
	</tr>
	<tr>
		<td>
			<img src="https://raw.githubusercontent.com/vscode-icons/vscode-icons/refs/heads/master/icons/file_type_vb.svg" alt="Visual Basic.NET" width="80" />
		</td>
		<td>
			✔ As a pre-release.
		</td>
		<td>
			🟣 No specific documentation planned (refer to <strong>RSML for C#</strong> docs).
		</td>
		<td>
			✔
		</td>
		<td>
			<a href="https://www.nuget.org/packages/RSML">NuGet</a>
		</td>
		<td>
			<a href="https://github.com/OceanApocalypseStudios/RedSeaMarkupLanguage">Visit</a>
		</td>
		<td>
			---
		</td>
	</tr>
	<tr>
		<td>
			<img src="https://raw.githubusercontent.com/vscode-icons/vscode-icons/refs/heads/master/icons/file_type_c.svg" alt="C" width="80" />
		</td>
		<td>
			❌
		</td>
		<td>
			---
		</td>
		<td>
			---
		</td>
		<td>
			---
		</td>
		<td>
			---
		</td>
		<td>
			---
		</td>
	</tr>
	<tr>
		<td>
			<img src="https://raw.githubusercontent.com/vscode-icons/vscode-icons/refs/heads/master/icons/file_type_cpp.svg" alt="C++" width="80" />
		</td>
		<td>
			❌
		</td>
		<td>
			---
		</td>
		<td>
			---
		</td>
		<td>
			---
		</td>
		<td>
			---
		</td>
		<td>
			---
		</td>
	</tr>
	<tr>
		<td>
			<img src="https://raw.githubusercontent.com/vscode-icons/vscode-icons/refs/heads/master/icons/file_type_lua.svg" alt="Lua" width="80" />
		</td>
		<td>
			❌
		</td>
		<td>
			---
		</td>
		<td>
			---
		</td>
		<td>
			---
		</td>
		<td>
			---
		</td>
		<td>
			---
		</td>
	</tr>
	<tr>
		<td>
			<img src="https://raw.githubusercontent.com/vscode-icons/vscode-icons/refs/heads/master/icons/file_type_python.svg" alt="Python 3" width="80" />
		</td>
		<td>
			❌ Is not compliant with the new API.
		</td>
		<td>
			🔴 <a href="https://oceanapocalypsestudios.github.io/rsml-docs/api/python/">Out-of-date</a>
		</td>
		<td>
			✔
		</td>
		<td>
			<a href="https://pypi.org/project/rsml-python/">PyPI</a>
		</td>
		<td>
			<a href="https://github.com/OceanApocalypseStudios/RSML.Python">Visit</a>
		</td>
		<td>
			---
		</td>
	</tr>
</table>

<hr />

## Performance
At OceanApocalypseStudios, we value performance.

Below is a comparison between the first benchmark of v2.0.0-prerelease8 and the latest one.

### The Before
| Friendly Name                                                                  | Method Name                  | Mean (ns)    | Error (ns)    | Gen 0     | Gen 1    | Gen 2    | Allocs    |
|------------------------------------------------------------------------------- |----------------------------- |-------------:|--------------:|----------:|---------:|---------:|----------:|
| Creation of a new evaluator and accessing its `Content` property (1 line)      | ContentProperty SmallContent |        279.1 |        722.46 |    0.2789 |        - |        - |     584 B |
| Creation of a new evaluator and accessing its `Content` property (10000 lines) | ContentProperty LargeContent |    405,550.6 |    117,660.61 |  110.8398 | 110.8398 | 110.8398 |  350181 B |
| Evaluating 1 line of RSML                                                      | Evaluate SmallContent        |      5,287.5 |        378.29 |    2.0752 |        - |        - |    4352 B |
| Evaluating 100 lines of RSML                                                   | Evaluate MediumContent       |     98,444.7 |     40,498.33 |   40.6494 |        - |        - |   85256 B |
| Evaluating 10000 lines of RSML                                                 | Evaluate LargeContent        | 11,233,432.8 | 12,026,706.66 | 4015.6250 | 109.3750 | 109.3750 | 8747973 B |
| Evaluating 500 lines of RSML, but with mixed statements                        | Evaluate ComplexContent      |  1,049,512.7 |    364,556.17 |  451.1719 |        - |        - |  943960 B |
| Checking if a short line is a comment                                          | IsComment True_Medium        |        109.3 |         28.13 |    0.1109 |        - |        - |     232 B |
| Checking if an extremely short line is a comment                               | IsComment True_Small         |        111.9 |         65.24 |    0.1109 |        - |        - |     232 B |
| Checking if a short line is a comment                                          | IsComment False_Medium       |        124.9 |         71.19 |    0.1109 |        - |        - |     232 B |
| Checking if an extremely short line is a comment                               | IsComment False_Small        |        184.0 |      1,105.59 |    0.1109 |        - |        - |     232 B |

### The After
| Friendly Name                                                                  | Method                       | Mean             | Error            | Gen 0     | Allocated |
|------------------------------------------------------------------------------- |----------------------------- |-----------------:|-----------------:|----------:|----------:|
| Creation of a new evaluator and accessing its `Content` property (1 line)      | ContentProperty SmallContent |       193.142 ns |     1,229.594 ns |    0.1605 |     336 B |
| Creation of a new evaluator and accessing its `Content` property (10000 lines) | ContentProperty LargeContent |       128.318 ns |       237.470 ns |    0.1605 |     336 B |
| Evaluating 1 line of RSML                                                      | Evaluate SmallContent        |     5,047.466 ns |     6,844.805 ns |    1.4343 |    3000 B |
| Evaluating 100 lines of RSML                                                   | Evaluate MediumContent       |   110,423.043 ns |   290,075.337 ns |   30.6396 |   64240 B |
| Evaluating 10000 lines of RSML                                                 | Evaluate LargeContent        | 9,369,561.458 ns | 2,808,413.891 ns | 3078.1250 | 6440560 B |
| Evaluating 500 lines of RSML, but with mixed statements                        | Evaluate ComplexContent      | 1,281,701.042 ns | 1,287,522.972 ns |  292.9688 |  613328 B |
| Checking if a short line is a comment                                          | IsComment True_Medium        |         7.858 ns |         2.828 ns |         - |         - |
| Checking if an extremely short line is a comment                               | IsComment True_Small         |         7.615 ns |         3.161 ns |         - |         - |
| Checking if a short line is a comment                                          | IsComment False_Medium       |        24.107 ns |         4.930 ns |         - |         - |
| Checking if an extremely short line is a comment                               | IsComment False_Small        |         6.202 ns |        16.032 ns |         - |         - |

### Our Next Goals for Performance
We plan on reducing the amount of allocated bytes, as well as reducing the time it takes for a large buffer.

However, considering most RSML files are up to 100 lines long, these times are good enough for a pre-release.

In the future, we may create an external package called `RSML.Performance` which will contain toolchain components as `ref structs`, at the cost of less control over the toolchain (such as not being able to use middlewares).

<hr />

> **Copyright (c) 2025 OceanApocalypseStudios**
>
> We :heart: open-source!
