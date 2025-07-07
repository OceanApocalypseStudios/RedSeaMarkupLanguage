# Red Sea Markup Language
<img src="Assets/FullSizeLogo.png" alt="RSML Logo" align="right" width="96" height="96">

[![NuGet Version](https://img.shields.io/nuget/v/RSML?style=for-the-badge&logo=nuget&link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FRSML)](https://www.nuget.org/packages/RSML)
[![GitHub Release](https://img.shields.io/github/v/release/OceanApocalypseStudios/RedSeaMarkupLanguage?sort=semver&style=for-the-badge&color=orange)](https://github.com/OceanApocalypseStudios/RedSeaMarkupLanguage/releases/latest)
[![GitHub License](https://img.shields.io/github/license/OceanApocalypseStudios/RedSeaMarkupLanguage?style=for-the-badge)](https://raw.githubusercontent.com/OceanApocalypseStudios/RedSeaMarkupLanguage/main/LICENSE)
![GitHub top language](https://img.shields.io/github/languages/top/OceanApocalypseStudios/RedSeaMarkupLanguage?style=for-the-badge&logo=dotnet&logoSize=auto&color=darkgreen)
![GitHub Repo stars](https://img.shields.io/github/stars/OceanApocalypseStudios/RedSeaMarkupLanguage?style=for-the-badge&logo=github&color=yellow)
![NuGet Downloads](https://img.shields.io/nuget/dt/RSML?style=for-the-badge&logo=nuget&color=red)

> The modern fork of [MF's Crossroad](https://github.com/MF366-Coding/MFRoad) we're sure you'll love.

Below is a list of features (or reasons why RSML is better than MFRoad - call them what you want) in this markup language.

<hr />

## Regex Support
Instead of those awful `win32:any` and `any:amd64`, you have Regex support in RSML.

```python
win.+ -> "Return value"
# ^ this literally stands for "starts with win and goes on undefinitely"
# pure regex, as simple as it can get, eh eh
```

<hr />

## More *space* to breathe
One of the low points of MFRoad was not allowing for spaces in statements. Worry no more.

```python
# all of the below statements are valid
win -> "valid"
win->"valid"
win                      ->               "valid"
win                                 ->"valid"
win->          "valid"
```

<hr />

## Known system identifier format
MFRoad used the poorly-documented `sys.platform` from python and, for Linux distros, the `distro` module.

Since RSML is made in C#, we decided to change things up a bit: **system identifiers are now the same as [MSBuild Runtime Identifiers](https://learn.microsoft.com/en-us/dotnet/core/rid-catalog),** which are pretty well documented.

<hr />

## Directly in your terminal
RSML is not just a library for .NET: you can also use it directly in your terminal.

```bash
cat somefile.rsea | RSML.CLI evaluate --no-pretty
```

> [!NOTE]
> `--no-pretty` disables ASCII art and copyright messages so you can easily pipe the evaluation into another command without it also piping the ASCII art.

<hr />

## Documentation build process
You may build the documentation for RSML yourself!

Just try doing this:
```bash
RSML.Docs
```

Or, if you're scared of the commandline, just run the `main.py` that's bundled with **RSML.Docs**.

<hr />

## Still kind of compatible?
Yes, RSML is still kind of compatible with MFRoad. It supports MFRoad's operators as long as they're specified.

It can't emulate the `any` or the `<system>:<architecture>` behavior though.

<hr />

## Comments
This might be hard to take in at first but... everything that's not a statement is a comment. Yup.

```python
# this is a comment - the recommended way as these are always ignored
// but this is technically also a valid comment

in fact, any statement that does not start with special actions
and does not have any operators

yup, it's a comment

the problem arises when you do things like this:
etc etc -> etc etc

problem? RSML considers that to be a valid statement because of the operator in it

so my recommendation?
# always stick with the default comments, as they're always ignored
```

<hr />

## Custom actions
MFRoad only allowed 3 actions: returning a value (primary), outputting text (secondary) and throwing an error (tertiary).

RSML still only allows 3 main actions, but they can be customized (does not apply to the CLI).

Hell, you can even customize the operators that those actions use.

```c#
using System;

// ...

// let's say your RSParser is at a variable named "parser"
parser.DefineOperator(OperatorType.Primary, ";)")
// ;) will now be your primary operator (basically, it'll replace ->)

// you may also register an action
parser.RegisterAction(OperatorType.Secondary, (_, argument) => Console.WriteLine(argument));
// _ is a parameter of type RSParser. I've discarded it as we don't need it
// argument is a string

// you can register actions only for secondary, tertiary and special operators
// the primary action cannot be redefined - it's always the return action
```

<hr />

## Special Actions
Special actions are also a new thing.

```ruby
@MySpecialAction TheArgument # arguments here cannot contain any spaces
```

There is only one built-in special action, an argument-less one. All of the others must be defined for them to work.

```ruby
@EndAll # the only built-in special action in RSML
```

You may define special actions but keep in mind they take 2 arguments, like normal main actions but also return a value, of type `byte`.

```c#
parser.RegisterSpecialFunction("TestFunc", (_, _) => 251);
// 251 is a special return code that removes all defined special actions (except for @EndAll)
// other special return codes are 250 and 252
```

This `TestFunc` can then be used in RSML.

```ruby
@TestFunc
@MySpecial
# even if MySpecial was defined, it'll become undefined after @TestFunc
```

<hr />

## What about other languages?
*~~As for right now, we don't intend in porting RSML library to other languages, but~~* You could include the DLLs in your project and call them.
Not a graceful solution, but not a bad one either.

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
			✔ RSML is intended for use in C#.
		</td>
		<td>
			🟡 <a href="https://oceanapocalypsestudios.github.io/rsml-docs/api/csharp/">In the making</a>
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
			<img src="https://raw.githubusercontent.com/vscode-icons/vscode-icons/refs/heads/master/icons/file_type_python.svg" alt="Python 3" width="80" />
		</td>
		<td>
			✔ Since June 12, 2025.
		</td>
		<td>
			🟡 <a href="https://oceanapocalypsestudios.github.io/rsml-docs/api/python/">In the making</a>
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
			<img src="https://raw.githubusercontent.com/vscode-icons/vscode-icons/refs/heads/master/icons/file_type_vb.svg" alt="Visual Basic.NET" width="80" />
		</td>
		<td>
			⚠ Not tested but should be compatible with the C# package, since they're both .NET languages.
		</td>
		<td>
			🟣 No specific documentation planned (refer to RSML for C# docs).
		</td>
		<td>
			⚠ Hasn't been officially tested on VB.NET, but yes.
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
</table>

<hr />

> **Copyright (c) 2025 OceanApocalypseStudios**
> 
> We :heart: open-source!
