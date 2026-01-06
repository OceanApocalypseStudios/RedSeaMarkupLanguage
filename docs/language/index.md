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
It has **two** overloads: one where you can't specify a version and one where you can.

=== "Without version specification"
    ```rsea
    <operator> [<system-name>] [<system-major-version> = any] [<cpu-architecture>] <value>
    ```

=== "With version specification"
    ```rsea
    <operator> [<system-name>] [<system-major-version-comparison-symbol>] [<system-major-version>] [<cpu-architecture>] <value>
    ```

#### Parameters
The parameters in the syntax, although complex at first glance, are quite simple. However, they must appear in the exact order shown above (depending on the overload used).

`operator`

: The operator to use. This is **mandatory**.

`system-name`

: The operating system name to match against. This is **optional**; but must be specified if you want to specify the argument that comes next in order. Can be set to `any` to match all operating systems as well. Can, additionally, be set to `defined` to match all operating systems as long as the OS is recognized by the RSML implementation.

`system-major-version-comparison-symbol`

: The comparison symbol to use for the system major version. This is **optional**; but must be specified if you want to specify the argument that comes next in order; if not specified, it defaults to `==`. Can be one of the following: `==`, `!=`, `<`, `>`, `<=`, `>=`. It cannot be set to `any` or `defined`. If not specified, `system-major-version` must also not be specified (or set to `any`).

`system-major-version`

: The major version of the operating system to match against. This is **optional**; but must be specified if you want to specify the argument that comes next in order; if not specified, it matches all versions. Can be set to `any` to match all versions as well. Can, additionally, be set to `defined` to match all versions as long as the OS version is recognized by the RSML implementation. If `system-major-version-comparison-symbol` is specified, this parameter must also be specified (and vice-versa).

`cpu-architecture`

: The CPU architecture to match against. This is **optional**. Can be set to `any` to match all CPU architectures as well. Can, additionally, be set to `defined` to match all CPU architectures as long as the architecture is recognized by the RSML implementation.

`value`

: The value to return or use as an error message, depending on the operator used. This is **mandatory** and **must** be enclosed in double quotes (`"`).

<!-- todo: keep working on this -->

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
