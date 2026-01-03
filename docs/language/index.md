<!-- Copyright (c)  2025  OceanApocalypseStudios -->
<!-- Permission is granted to copy, distribute and/or modify this document -->
<!-- under the terms of the GNU Free Documentation License, Version 1.3 -->
<!-- or any later version published by the Free Software Foundation; -->
<!-- with no Invariant Sections, no Front-Cover Texts, and no Back-Cover Texts. -->


# RSML as a Language
??? note
    To make RSML easier to understand for beginners, this page starts with the _"easiest"_ details of the language.

## Files
The official standard file extension for **Red Sea Markup Language** files is `.rsea`, to avoid it being confused with other languages that also use the RSML abbreviation.

The `.rsml` file extension is also fine, but `.rsea` is preferred.

## Evaluation
The _"evaluation"_ is the act of going through every **logic path** and **special action** and evaluate the first, while running the second.

If an evaluation encounters a **primary operator** _([see Operators](#operators))_ in a true logic path, its value is returned and evaluation ends there.

## Use of MSBuild RIDs
[MSBuild Runtime Identifiers](https://learn.microsoft.com/en-us/dotnet/core/rid-catalog) are used for identifying systems and CPU architectures in Red Sea.

You'll need to know them in order to write RSML.

Here's a short example with common RIDs.

| RID         | Meaning                                                               | Example Usage in RSML (`official-25`)           |
| ----------- | --------------------------------------------------------------------- | ----------------------------------------------- |
| win-x64     | :fontawesome-brands-windows: Windows, 64-bit (based on x86)           | `win-x64 -> "Windows on x86-64"`                |
| linux-x64   | :fontawesome-brands-linux: Linux, 64-bit (based on x86)               | `linux-x64 || "Glad to see Linux getting love"` |
| linux-x86   | :fontawesome-brands-linux: Linux, 32-bit (based on x86)               | `linux-x86 ^! "32-bit Linux not supported"`     |
| osx-arm64   | :fontawesome-brands-apple: macOS, ARM 64-bit (based on Apple Silicon) | `osx-arm64 -> "An apple a day..."`              |
| win-arm64   | :fontawesome-brands-windows: Windows, ARM 64-bit                      | `win-arm64 || "Windows on ARM... Interesting."` |
| linux-arm64 | :fontawesome-brands-linux: Linux, ARM 64-bit                          | `linux-arm64 -> "Tux is happy"`                 |
| linux-arm   | :fontawesome-brands-linux: Linux, ARM 32-bit                          | `linux-arm || "Detected Linux on ARM32"`        |

## Use of Regex in RSML
!!! tip
    RSML uses Regex at its core, so it might be worth learning Regex first, even if only the basics.

RSML makes use of **regular expressions** to match [MSBuild RIDs](https://learn.microsoft.com/en-us/dotnet/core/rid-catalog) for different operating systems and CPU architectures.

This means you can write flexible patterns to match a wide range of platforms with few characters.

```python title="Usage of Regex in RSML"
win.+ -> "This logic path matches any Windows system" # (1)!
```

1. Or, to be precise, this logic path matches `win-x86`, `win-x64`, `win-arm64`, `win-arm`, etc.

## Language Specification
!!! warning "Non-standardized"
    Red Sea is **not** a [standardized](standards/index.md) language. **For the sake of examplifying without getting too technical**, **we'll be using the [**official-25**](standards/official-25.md) standard** in most of the documentation, as it's the closest to an official specification.

**Red Sea** is quite a simple language, **but the lack of an official specification** means behavior may vary across developer's implementations.

### Static Functionality (Standardized :green_circle:)
The following features are fully standardized and ==cannot be altered in any way by any language standards==.

* The use of **double quotes** for **enclosing values in logic path lines** being **mandatory** and no standard being able to modify that.
* The use of MSBuild **Runtime Identifiers**.
* The use of **standard Regex**.
* **Comment symbol** (`#!python #`).
* **Invalid lines** being **comments**.

### Extensible Functionality (Partially Standardized :yellow_circle:)
The following features are partially standardized. For each feature, it's explained which part is standardized and which one isn't.

* **[Special Actions.](#special-actions)** Special actions can be added and customized by language standards, but the built-in one (`#!python @EndAll`) cannot be removed or changed in *any* way.
* **[Operators](#operators).** Language standards can pick which tokens (including common words) they wish to use as operators, but operators are still partially standardized as there must always be **three operators** and only the behaviors for the secondary and tertiary can be altered - the **primary operator** will **always** be the return operator.

### Implementation-specific Functionality (Non-Standardized :red_circle:)
The following features are not standardized at all and can be customized at free will.

* **Tokens used by operators.** Each language standard can customize which tokens represent operators. **Example:** `official-25` uses operators `->`, `||`, `^!`.

## Operators
In RSML, there are always **three operators**, named **primary**, **secondary** and **tertiary**.

Below is a table with the operators, their [tokens](#extensible-functionality-partially-standardized) in `official-25` and what they actually do. For all of these, consider `val` as the argument they're passed.

| Operator Name | Operator Token (according to `official-25`) | Functionality                                                           | Functionality (according to `official-25`)                          |
| ------------- | ------------------------------------------- | ----------------------------------------------------------------------- | ------------------------------------------------------------------- |
| Primary       | `->`                                        | Returns `val` _(standards can't change this operator's functionality)_. | Returns `val`.                                                      |
| Secondary     | `||`                                        | Non-standardized.                                                       | Outputs `val` to the `stdout`.                                      |
| Tertiary      | `^!`                                        | Non-standardized.                                                       | Throws an error _(error message set to `val`)_ and ends evaluation. |

## Evaluation Process Flow
!!! tip "See also"
    * [Logic Path examples](#logic-paths)
    * [Special Action handling](#special-actions)

??? info "Strictly markup"
    Despite the usage of wording such as _"return"_ and _"interpret"_, RSML is **purely declarative** - it can **not** execute, compile or transpile code.

RSML is evaluated from **start to finish** _([see Advanced Representation of the Process Flow](#advanced-representation))_, meaning that the **very first** logic path **with a primary operator in it** that matches will be used and the evaluation ends there. All the logic beyond that point is ignored completely.

### Simplified Representation
``` mermaid
---
title: Simplified Representation of the Evaluation Process Flow
---
flowchart LR
  A[Start] --> B[Next line];
  B -->F{Match?};
  F -->|No| B;
  F -->|Yes| C{Primary operator?};
  C -->|Yes| D[Return value + end];
  C -->|No| E[Execute operator];
  E -->B;
```

### Advanced Representation
``` mermaid
---
title: Advanced Representation of the Evaluation Process Flow
---
flowchart LR
  A[Start] --> B[Next line];
  B -->C{Valid?};
  C -->|Yes| D{Type?};
  C -->|No| B;
  D -->|Logic| E{Match?};
  D -->|Action| F{Ends eval?};
  D -->|Comment| B;
  E -->|Yes| G{Primary op?};
  E -->|No| B;
  F -->|Yes| H[Finish];
  F -->|No| I[Run action];
  I -->B;
  G -->|Yes| J[Return + end];
  G -->|No| K[Execute operator];
  K -->B;
```

??? note "Return operator"
    Only the **primary operator** triggers the return of a logic path's value. The other operators, unless explicitly defined to do such, will never stop the evaluation.

## Logic Paths
A logic path is the _"interactive"_ part of RSML - the logic paths are what's evaluated.

The syntax for this is quite simple.

Things worth mentioning:

* The operator **must** be one of the **3 defined in the standard you're using** _([see Operators](#operators))_.
* The value _(argument)_ **must** be enclosed in **double quotes** (`"`).
* **Spacing does not affect logic paths** (`win.+ -> "hey"` is the same as `win.+       ->"hey"`).

=== "Actual Syntax"
    ```rsea
    <regex-expression> <operator> <value>
    ```

=== "Example 1"
    ```py
    win.+ -> "This is a valid logic path syntax" # (1)!
    ```

    1. The line is valid, as long as the `->` operator is defined in the standard you're using. This will match all hosts with the **Windows** operating system.

=== "Example 2"
    ```py
    (arch|ubuntu|debian)-x\d\d || "Linux is nice" # (1)!
    ```

    1. This will match **Debian**, **Ubuntu** and **Arch** **Linux** distributions, as long as the **CPU architecture** is `x86` based (`x86` or `x64`).

=== "Example 3"
    ```py
    osx.* || "An apple a day keeps the doctor away, I guess..." # (1)!
    ```

    1. Keep in mind **return values** must be enclosed in double quotes.

=== "Example 4"
    ```py
    win.* -> "I can smell windows.h" # (1)!
    osx.* || "An apple a day keeps the doctor away, I guess..." # (2)!
    .+-x64 -> "64-bits is standard nowadays" # (3)!
    ```

    1. In **`official-25`**, this operator ==**returns the logic path's value** and **ends evaluation**== (**only** if there's a match of course).
    2. In **`official-25`**, this operator ==**outputs the logic path's value** and does ***NOT* end evaluation**== (**only** if there's a match of course).
    3. If one is on 64-bit Windows, this line **wouldn't have been reached** since the **first line would have been a match** and **would've returned a value**.


## Special Actions
Special Actions _([see Language Specification](#language-specification))_ are a partially-standardized feature of RSML, being responsible for evaluation-time modification to RSML aspects.

!!! info "Built-in Special Action"
    [As noted in the language specification section](#language-specification), `@EndAll` is the only **immutable, built-in** special action in RSML:
    
    * **Name cannot be changed:** Must always be `@EndAll` (case-sensitive)
    * **Accepts no arguments:** `@EndAll` is always argument-less, so any arguments given will be ignored.
    * **Functionality:** Terminates evaluation immediately.
    * **Standards compliance:** All implementations support this, as it's built-in.

Special actions can change how the RSML parser behaves. Their usage varies with the [programming language you're using for the RSML API](../api/index.md). For that reason, in this page, we'll only talk about the syntax for these actions.

=== "Actual Syntax"
    ```py
    @<special-action> [<argument>] # (1)!
    ```

    1. The argument is optional, being an empty string if not specified. The argument must not contain **any** spaces and does not require enclosure.

=== "Example 1"
    ```py
    @EndAll # (1)!
    ```

    1. This is the only built-in standardized special action available. It'll be discussed later.

=== "Example 2"
    ```py
    @MyAwesomeAction SomeValue # (1)!
    ```

    1. `SomeValue` does **not** need to be enclosed in double quotes.

=== "Example 3"
    ```py
    @@ technically valid # (1)!
    ```

    1. This line is valid, as there's no limitation to action names. Since the argument contains spaces, RSML will only use the first part of the argument (`technically`) as the whole argument.

## Comments
**Comments are quite simple in RSML.** If a `#!python #` character is at the start of a line, that line is considered a comment and will be fully ignored by the parser.

_However_, every single line that's considered invalid is also a comment. Here are some examples with reasons as of why the lines are invalid.

=== "Actual Syntax"
    ```c
    # ... // (1)!
    ```

    1. Worth mentioning that a `#!python #` in any other location other than the start of a line is **not** a comment, unless the line is invalid.

=== "Example 1"
    ```py
    some random text # (1)!
    ```

    1. According to `official-25`, this line does not contain any operators or special actions, so it is invalid; therefore, a comment.

=== "Example 2"
    ```py
    // wrong type of comment # (1)!
    ```

    1. According to `official-25`, this line does not contain any operators or special actions, so it is invalid; therefore, a comment.

=== "Example 3"
    ```c
    # a comment // (1)!
    ```

    1. This line is a comment.

=== "Example 4"
    ```py
    win.+ || "Valid line" # (1)!
    ```

    1. This line is valid, as it contains a logic path with an operator; therefore, **not** a comment.

## Syntax Reference
This is a quick reference sheet on RSML's syntax.

??? example "Quick syntax reference"
    ```py
    # Logic Paths
    <regex-expression> <operator> <value>
    win.+ -> "My value"

    # Special Actions
    @<action-name> [<argument>]
    @MyAction MyValue
    @EndAll

    # Comments
    # at the start of a sentence
    # this is a comment

    # Spacing is flexible
    win.+ -> "valid"
    .+-x64          ->"valid"
    linux.*->     "valid"
    osx-arm64->"valid"
    ```
