<!-- Copyright (c)  2025  OceanApocalypseStudios -->
<!-- Permission is granted to copy, distribute and/or modify this document -->
<!-- under the terms of the GNU Free Documentation License, Version 1.3 -->
<!-- or any later version published by the Free Software Foundation; -->
<!-- with no Invariant Sections, no Front-Cover Texts, and no Back-Cover Texts. -->


# RSML as a Language

## Abstract
**Red Sea Markup Language** (RSML) is a simple declarative markup language created for the purpose of following logic paths based on the host's operating system and CPU architecture.

It is a better altenative to scripting languages because of its simplicity, ease of use, and the fact it's not necessary to package a whole interpreter.

## Evaluation
The __"evaluation"__ is the act of going through every line of RSML and evaluating the ones that match the **logic path** syntax, while running the ones that match the **special action** syntax.

If an evaluation encounters the **return operator** _([see Operators](#operators))_ in a logic path that evaluates as `true`, its value is returned and evaluation ends there.

## Language Specification
!!! note
    Starting with version <!-- md:version 2.0.0 -->, RSML is a standardized language, in order to ensure consistency across different implementations. This also allows for less confusion when it comes to the language's features and syntax.

Below are the main concepts of RSML as a language.

- **Logic Path:** The interactive part of RSML, responsible for returning values based on machine matches.
- **Special Action:** Actions that are executed during evaluation, responsible for modifying real-time aspects of how RSML is evaluated. There are only 3 as of now.
- **Comments:** Lines that are ignored by the parser.

## Logic Path
A logic path is a line in RSML that contains several expressions, an operator and a value to return. If the expression matches the host's OS/architecture, the evaluator returns the value (if the operator is the return operator) or executes the operator's functionality.

### Operators
In RSML, there are two operators, named **return** and **throw-error**.

Below is a table with the operators, their tokens and what they actually do.

| Operator Name | Operator Token | Functionality _(triggered if the logic path matches the machine)_                  |
| ------------- | -------------- | ---------------------------------------------------------------------------------- |
| Return        | `->`           | Returns the logic path's value and ends evaluation.                                |
| Throw-Error   | `!>`           | Throws an error (error message set to the logic path's value) and ends evaluation. | 

### Syntax
The syntax for logic paths is extremely simple.
It has **multiple** overloads, depending on how many parameters you want to specify. Order **must** be followed as shown below, for the overload you want to use.

=== "Match-all (2 tokens)"
    ```rsea
    <operator> <value>
    ```

=== "With system name (3 tokens)"
    ```rsea
    <operator> <system-name> <value>
    ```

=== "With system name and CPU architecture (4 tokens)"
    ```rsea
    <operator> <system-name> <cpu-architecture> <value>
    ```

=== "With system name, major version and CPU architecture (5 tokens)"
    ```rsea
    <operator> <system-name> <system-major-version> <cpu-architecture> <value>
    ```

=== "Full overload (6 tokens)"
    ```rsea
    <operator> <system-name> <system-major-version-comparison-symbol> <system-major-version> <cpu-architecture> <value>
    ```

#### Parameters
The parameters in the syntax, although complex at first glance, are quite simple. However, they must appear in the exact order shown above (depending on the overload used).

`operator`

:   The operator to use. This is **mandatory**.

    **Supported Operators:**

    - <!-- md:version 2.0.0-prerelease8 --> **`->`** (return operator)
    - <!-- md:version 2.0.0-prerelease8 --> **`!>`** (throw-error operator)

`system-name` <!-- md:version 2.0.0-prerelease8 -->

:   The operating system name to match against. This is **optional**; but must be specified
    if you wish to use any overload that includes it. Can be set to `any` to
    match all operating systems as well. Can, additionally, be set to `defined` to match
    all operating systems as long as the OS is recognized by the RSML implementation.

    **Supported Systems:**

    - <!-- md:version 2.0.0-prerelease8 --> **`windows`**
    - <!-- md:version 2.0.0-prerelease8 --> **`linux`** (matches all Linux distributions, even if the specific distro is not recognized)
    - <!-- md:version 2.0.0-prerelease8 --> **`osx`** (MacCatalyst is untested but should work)
    - <!-- md:version 2.0.0-prerelease8 --> **`freebsd`** (OpenBSD and NetBSD are probably considered different OSes and thus not matched - that is up to .NET's implementation)
    - <!-- md:version 2.0.0-prerelease8 --> **`debian`**
    - <!-- md:version 2.0.0-prerelease8 --> **`ubuntu`**
    - <!-- md:version 2.0.0-prerelease8 --> **`archlinux`** (called `archlinux` instead of just `arch` to avoid confusion with CPU _arch_itectures)
    - <!-- md:version 2.0.0-prerelease8 --> **`fedora`**
    - More OSes may be added in future versions... _(for now, any other value will throw a lexer error)_

    **Allowed Wildcards:**

    - <!-- md:version 2.0.0-prerelease8 --> **`any`** (not an OS, but matches all OSes)
    - <!-- md:version 2.0.0-prerelease8 --> **`defined`** (not an OS, but matches all OSes, as long as there's information about them available to RSML)

    ???+ info "How OS names are determined internally"
        <!-- md:version 2.0.0-prerelease8 --> We use .NET's [`RuntimeInformation.IsOSPlatform`](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.runtimeinformation.isosplatform) method to determine the OS platform.

        E.g.: `#!csharp RuntimeInformation.IsOSPlatform(OSPlatform.Windows)` returns `true` if the host OS is Microsoft Windows.

        <!-- md:version 2.0.0-prerelease8 --> For macOS and FreeBSD, we use the same method to check if the OS is macOS or FreeBSD, respectively.

        <!-- md:version 2.0.0-prerelease8 --> For Linux, we use the same method to check if the OS is Linux, and then we read `/etc/os-release` to determine the specific distribution. In `etc/os-release`, we read the `ID` and `ID_LIKE` fields to determine the distribution name. The predominant field is `ID_LIKE` (Fedora is the only exception).


        Learn more about how we handle OS names: [`OceanApocalypseStudios.RSML.Machine.LocalMachine` class](https://github.com/OceanApocalypseStudios/RedSeaMarkupLanguage/blob/main/src/RSML/Machine/LocalMachine.cs).


`system-major-version-comparison-symbol` <!-- md:version 2.0.0-prerelease8 -->

:   The comparison symbol to use for the system major version. This is **optional**; but   
    must be specified if you want to specify the argument that comes next in order; if not
    specified, it defaults to `==`. Can be one of the following: `==`, `!=`, `<`, `>`,
    `<=`, `>=`. It cannot be set to `any` or `defined`. If not specified, it defaults to
    `==`.

    **Supported Comparison Symbols:**

    - <!-- md:version 2.0.0-prerelease8 --> **`==`** (equal to)
    - <!-- md:version 2.0.0-prerelease8 --> **`!=`** (not equal to)
    - <!-- md:version 2.0.0-prerelease8 --> **`<`** (less than)
    - <!-- md:version 2.0.0-prerelease8 --> **`>`** (greater than)
    - <!-- md:version 2.0.0-prerelease8 --> **`<=`** (less than or equal to)
    - <!-- md:version 2.0.0-prerelease8 --> **`>=`** (greater than or equal to)

    **Allowed Wildcards:**

    - <!-- md:version 2.0.0-prerelease8 --> _None_

`system-major-version` <!-- md:version 2.0.0-prerelease8 -->

:   The major version of the operating system to match against. This is **optional**; but
    must be specified if you want to specify the argument that comes next in order; if not
    specified, it matches all versions. Can be set to `any` to match all versions as well.
    Can, additionally, be set to `defined` to match all versions as long as the OS version
    is recognized by the RSML implementation. Can be specified with or without
    `system-major-version-comparison-symbol`.

    **Allowed Wildcards:**

    - <!-- md:version 2.0.0-prerelease8 --> **`any`** (not a version, but matches all versions)
    - <!-- md:version 2.0.0-prerelease8 --> **`defined`** (not a version, but matches all versions, as long as there's information about them available to RSML)

    **Note:** Wildcards cannot be used together with comparison symbols. If you're planning on using comparison symbols, you need to specify actual versions. E.g., these are invalid syntax: `debian >= any` and `osx != defined`.

    **Mapping for Windows versions:**

    | Windows Version Name | `system-major-version` Value | Last Changed                          |
    | -------------------- | ---------------------------- | ------------------------------------- |
    | Windows XP           | 5                            | <!-- md:version 2.0.0-prerelease8 --> |
    | Windows Vista        | 6                            | <!-- md:version 2.0.0-prerelease8 --> |
    | Windows 7            | 7                            | <!-- md:version 2.0.0-prerelease8 --> |
    | Windows 8            | 8                            | <!-- md:version 2.0.0-prerelease8 --> |
    | Windows 8.1          | 9                            | <!-- md:version 2.0.0-prerelease8 --> |
    | Windows 10           | 10                           | <!-- md:version 2.0.0-prerelease8 --> |
    | Windows 11           | 11                           | <!-- md:version 2.0.0-prerelease8 --> |

    ???+ info "How OS versions are determined internally"
        <!-- md:version 2.0.0-prerelease8 --> We use our own implementation to determine the OS version, as to avoid relying on inaccurate methods such as `Environment.OSVersion`, which is known to return wrong values on Windows 8.1 and later.

        <!-- md:version 2.0.0-prerelease8 --> On Windows, we use the `SOFTWARE\Microsoft\Windows NT\CurrentVersion` registry key. Inside it, we read `CurrentBuildNumber` and, if that's above 22000, we consider it Windows 11 (Windows 11 has its major set to 10, so this is necessary). Otherwise, we read `CurrentMajorVersionNumber` for the major version (which, most likely will be 10 - since Windows 11 is handled before this check, you can be sure this "10" will always be Windows 10 and not Windows 11). Lastly, we fallback to `CurrentVersion`, where we consider the system version to be:

        - <!-- md:version 2.0.0-prerelease8 --> `5` for **Windows XP** (if `CurrentVersion` is `5.1`)
        - <!-- md:version 2.0.0-prerelease8 --> `6` for **Windows Vista** (if `CurrentVersion` is `6.0`)
        - <!-- md:version 2.0.0-prerelease8 --> `7` for **Windows 7** (if `CurrentVersion` is `6.1`)
        - <!-- md:version 2.0.0-prerelease8 --> `8` for **Windows 8** (if `CurrentVersion` is `6.2`)
        - <!-- md:version 2.0.0-prerelease8 --> `9` for **Windows 8.1** (if `CurrentVersion` is `6.3`; in a future version, this will likely be changed to `81` to avoid confusion - we tried going with `8.1` but the `system-major-version` argument must be an integer)
        - <!-- md:version 2.0.0-prerelease8 --> `x + y` for all others (where `x` and `y` are such that `CurrentVersion` is `x.y` - to be fixed in a future version to only consider `x` for all other Windows versions)

        <!-- md:version 2.0.0-prerelease8 --> On Linux, we read `/etc/os-release` to determine the version. We read the `VERSION_ID` field to get the version. The major version is considered to be the integer part before the first dot (`.`). If there's no dot, the whole value is considered the major version. If `VERSION_ID` is not present, the version is `null`, meaning using `any` won't match, but `defined` will.

        <!-- md:version 2.0.0-prerelease8 --> On macOS, we use the `sw_vers` command to get the version. We read the `ProductVersion` value and consider the major version to be the integer part before the first dot (`.`). If there's no dot, the whole value is considered the major version. If the command fails for any reason, the version is `null`, meaning using `any` won't match, but `defined` will.

        <!-- md:version 2.0.0-prerelease8 --> On FreeBSD, we use the `uname -r` command to get the version. We consider the major version to be the integer part before the first dot (`.`). If there's no dot, the whole value is considered the major version. If the command fails for any reason, the version is `null`, meaning using `any` won't match, but `defined` will.

        Learn more about how we handle OS versions: [`OceanApocalypseStudios.RSML.Machine.LocalMachine` class](https://github.com/OceanApocalypseStudios/RedSeaMarkupLanguage/blob/main/src/RSML/Machine/LocalMachine.cs) | [`LocalMachine.Windows.cs`](https://github.com/OceanApocalypseStudios/RedSeaMarkupLanguage/blob/main/src/RSML/Machine/LocalMachine.Windows.cs).

`cpu-architecture` <!-- md:version 2.0.0-prerelease8 -->

:   The CPU architecture to match against. This is **optional**. Can be set to `any` to
    match all CPU architectures as well. Can, additionally, be set to `defined` to match
    all CPU architectures as long as the architecture is recognized by the RSML
    implementation.

    **Supported Architectures:**

    - <!-- md:version 2.0.0-prerelease8 --> **`x86`** (32-bit x86 architecture)
    - <!-- md:version 2.0.0-prerelease8 --> **`x64`** (64-bit x86 architecture)
    - <!-- md:version 2.0.0-prerelease8 --> **`arm64`** (64-bit ARM architecture)
    - <!-- md:version 2.0.0-prerelease8 --> **`arm32`** (32-bit ARM architecture; technically should be called just `arm`, but we use `arm32` for consistency)
    - <!-- md:version 2.0.0-prerelease8 --> **`loongarch64`** (64-bit LoongArch architecture)

    **Allowed Wildcards:**

    - <!-- md:version 2.0.0-prerelease8 --> **`any`** (not an architecture, but matches all architectures)
    - <!-- md:version 2.0.0-prerelease8 --> **`defined`** (not an architecture, but matches all architectures, as long as there's information about them available to RSML)

`value` <!-- md:version 2.0.0-prerelease8 -->

:   The value to return or use as an error message, depending on the operator used. This is
    **mandatory** and **must** be enclosed in double quotes (`"`).

#### Examples
=== "Increasing the amount of specified parameters"
    ```rsea
    # Match-all
    -> "This matches all OSes and architectures"

    # System name
    -> windows "This is Windows on any version and architecture"

    # System name and CPU architecture
    -> windows x64 "This is Windows on any version and x64 architecture"

    # System name, major version and CPU architecture
    -> windows 10 x64 "This is Windows 10 on x64 architecture"

    # Full overload
    -> windows >= 10 x64 "This is Windows 10 or above on x64 architecture"
    ```

=== "Matching Windows 10 on x64 architecture"
    ```rsea
    -> windows == 10 x64 "This is Windows 10 on x64"
    # or just
    -> windows 10 x64 "This is Windows 10 on x64"
    ```

=== "Matching any Linux distribution on any architecture"
    ```rsea
    -> linux "This is Linux on any architecture"
    ```

=== "Matching any Windows version as long as known"
    ```rsea
    -> windows defined any "This is Windows on any known version, and any architecture"
    # note how we had to add "any"
    # it's because there's no overload with only system name and system major version
    # hence we having to use the cpu-architecture argument as well

    # the only alternative in this acse was going with the full overload
    # some might say full with "any" is clearer cuz the docujment becomes structured
    # with a column-like look and it's easier to read and interpret what each argument is
    # however, some will say simplicity is key
    # here it goes, anyhoo:
    # -> windows == defined any "This is Windows on any known version, and any architecture"
    # wait a minute...
    # THAT'S INVALID SYNTAX!
    # a comparison symbol cannot be used with a wildcard - only actual versions!
    # nearly blew up prod, Jerry!!

    # for any other arguments, it'd be pretty much fine though, as only system version has that thing with comparison symbols :)
    ```

=== "Throwing an error on any macOS version below 11 on arm64 architecture"
    ```rsea
    !> osx &lt; 11 arm64 "Unsupported macOS version on arm64 architecture"
    ```

=== "Throwing an error"
    ```rsea
    !> any any any "This error is thrown on any OS and architecture"
    # this can be done at the end of the file if you want to ensure the machien can't just let it pass as "null" (this way, it forces evaluation to always end with an error if no other logic path matched)
    # if you do it at the start of the file, well... good luck reaching any other logic path :D
    # isn't that right, Jerry?
    ```

=== "Understanding the difference between 'any' and 'defined'"
    ```rsea
    # 'any' matches all OSes/architectures/versions, even if unknown
    -> any any any "This matches absolutely anything"

    # 'defined' matches all known OSes/architectures/versions, but not unknown ones
    -> defined defined defined "This does NOT match unknown OSes"

    # in summary,
    # while in the first case, RSML might not know what the OS is, what the architecture is, or what the version is and still match,
    # in the second case, RSML MUST know what the OS is, what the architecture is, and what the version is in order to match
    # any != defined
    # don't forget that!
    ```

=== "Getting complex"
    ```rsea
    # Matching Fedora Linux versions 34 and above on x64 architecture
    -> fedora >= 34 x64 "This is Fedora Linux 34 or above on x64 architecture"

    # Matching Ubuntu Linux versions below 20 on any architecture
    -> ubuntu < 20 any "This is Ubuntu Linux below version 20 on any architecture"

    # Matching Windows versions not equal to 10 on arm64 architecture
    -> windows != 10 arm64 "This is Windows on arm64 architecture, but NOT version 10"
    ```

=== "Real-life example: The Basics"
    ```rsea
    # This RSML snippet matches Windows 10 or above on x64 architecture,
    # and returns a specific value for that case.
    # If the machine is not Windows 10 or above on x64, it throws an error.

    -> windows >= 10 x64 "You are running Windows 10 or above on x64 architecture"
    !> any any any "Unsupported OS or architecture detected"
    ```

=== "Real-life example: Wildcards and Versions"
    ```rsea
    # This RSML snippet demonstrates the use of wildcards and version comparisons.
    # It matches any Linux distribution on any architecture,
    # and also matches macOS versions 11 and above on arm64 architecture.

    -> linux any any "You are running Linux on any architecture"
    -> osx >= 11 arm64 "You are running macOS 11 or above on arm64 architecture"
    !> any any any "Unsupported OS or architecture detected"
    ```

## Special Actions
Special Actions are lines in RSML that start with the `@` character, followed by the action name and an optional argument.

These are built into the language and cannot be changed whatsoever _(unless you implement your own unstandardized actions; however, that is not recommended for compatibility reasons)_.

| Action Name | Argument Requirement | Functionality                                   | Last Changed                          |
| ----------- | -------------------- | ----------------------------------------------- | ------------------------------------- |
| EndAll      | None                 | Ends evaluation immediately, with a null match. | <!-- md:version 2.0.0-prerelease8 --> |
| Void        | Optional             | Does nothing; can be used as a no-op.           | <!-- md:version 2.0.0-prerelease8 --> |
| ThrowError  | Mandatory            | Throws an error with the given message.         | <!-- md:version 2.0.0-prerelease8 --> |

???+ tip "ThrowError vs `!>` operator"
    <!-- md:version 2.0.0-prerelease8 --> Note how the `ThrowError` special action is practically the same as `!> any any any "<message>"` logic path. The difference is internal only and quite neggligible for most use cases. Some will argue the first is clearer, while others will argue the second is clearer. Choose whichever you prefer!

## Evaluation Process Flow
!!! info "Strictly markup"
    Despite the usage of wording such as _"return"_ and _"interpret"_, RSML is **purely declarative** - it can**not** execute, compile or transpile code.

RSML is evaluated from **start to finish** _([see Advanced Representation of the Process Flow](#advanced-representation))_, meaning that the **very first** logic path **with a return operator in it** that matches will be used and the evaluation ends there. All the logic beyond that point is ignored completely, including comments and special actions.

### Simplified Representation <!-- md:version 2.0.0-prerelease8 -->
``` mermaid
---
title: Simplified Representation of the Evaluation Process Flow
---
flowchart LR
  A[Start] --> B[Next line];
  B -->F{Match?};
  F -->|No| B;
  F -->|Yes| C{Return operator?};
  C -->|Yes| D[Return value + end];
  C -->|No| E[Execute operator];
  E -->B;
```

### Advanced Representation <!-- md:version 2.0.0-prerelease8 -->
``` mermaid
---
title: Advanced Representation of the Evaluation Process Flow
---
flowchart LR
  A[Start] --> B[Next line];
  B -->C{Valid?};
  C -->|Yes| D{Type?};
  C -->|No| B;
  D -->|Logic Path| E{Match?};
  D -->|Action| F{Ends eval?};
  D -->|Comment| B;
  E -->|Yes| G{Return op?};
  E -->|No| B;
  F -->|Yes| H[Finish];
  F -->|No| I[Run action];
  I -->B;
  G -->|Yes| J[Return + end];
  G -->|No| K[Execute operator];
  K -->B;
```

## Comments
We chose to talk about comments after the evalaution process, because, oh well, big surprise!, comments are ignored by the parser!

<!-- md:version 1.0.0 --> **Comments are quite simple in RSML.** If a `#!python #` character is at the start of a line, that line is considered a comment and will be fully ignored by the parser.

=== "Actual Syntax"
    ```c
    # ... // (1)!
    ```

    1. Worth mentioning that a `#!python #` in any other location other than the start of a line is **not** a comment.

=== "Example 1"
    ```c
    # a comment // (1)!
    ```

    1. This line is a comment.

=== "Example 2"
    ```py
    -> "Hello, Jerry!" # (1)!
    ```

    1. This line is valid, as it contains a logic path; therefore, **not** a comment.

## Files
The recommended file extension for RSML files is `.rsea`, but other extensions may be used as well, such as `.rsml`. In fact, you can even use no extension at all, if you want to _(ensures a lovely chaos mode)_.

## Syntax Reference
This is a quick reference sheet on RSML's syntax.

???+ example "Quick syntax reference"
    ```rsea
    # This is a comment

    -> "Match-all logic path"

    -> windows "Match Windows on any version and architecture"

    -> windows x64 "Match Windows on any version and x64 architecture"

    -> windows 10 x64 "Match Windows 10 on x64 architecture"

    -> windows >= 10 x64 "Match Windows 10 or above on x64 architecture"

    !> osx < 11 arm64 "Throw error on macOS below version 11 on arm64 architecture"

    -> any != 10 x86 "Don't confuse any..."

    -> defined != 10 x86 "...with defined"

    @EndAll

    @Void

    @ThrowError "This is an error message"
    ```

## What might be coming soon?
Well, you haven't heard this from me, but there are plans to add more features to RSML in future versions, such as:

- **Logical INBETWEEN/OR operators** in logic paths, to allow for more complex matching conditions.
- **Line-NOT and Argument-NOT operators** to negate matches.
- **Support for more operating systems and CPU architectures** as they become relevant.
- **Enhanced error handling and reporting** for better debugging.
- **Increased strictness levels** to enforce more rigorous syntax and evaluation rules and avoid ambiguity and silent line skips.
- **More special actions** to provide additional functionality during evaluation.
- **Hot fixes for certain system version detection methods** to improve accuracy.
- **Internal-only refactors and optimizations** to improve performance, maintainability and reduce the difficulty of implementing new features. We will try to do this with as least API breaking changes as possible. Hopefully, zero language changes (obviously, not because we care about you, but because I don't want to rewrite this documentation, love you too cutie <3 - jk).
