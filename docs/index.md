---
hide:
  - navigation
---

<!-- Copyright (c)  2025  OceanApocalypseStudios -->
<!-- Permission is granted to copy, distribute and/or modify this document -->
<!-- under the terms of the GNU Free Documentation License, Version 1.3 -->
<!-- or any later version published by the Free Software Foundation; -->
<!-- with no Invariant Sections, no Front-Cover Texts, and no Back-Cover Texts. -->

# Welcome to Red Sea Markup Language Docs
!!! warning
    This documentation refers to RSML v2.0.0+. RSML has changed a **lot** from v1.x.x to v2.0.0.

**Red Sea Markup Language** is **the** powerful and robust fork of [MF's CrossRoad Solution](https://mf366-dev.github.io/documentation/mfroad/mfroad_1.0.html "MFRoad"), a ==language designed to dynamically interpret different logic paths based on the local host OS and CPU architecture.==

RSML *(short for Red Sea Markup Language)* can currently be used in C# and Python, as well as in its own CLI. We are currently working in the native C ABI as well, which will let you use RSML in whatever programming language you like (as long as it allows for C interop).

> **This documentation is licensed under the GFDL.**
>
> Copyright &copy; 2025  OceanApocalypseStudios
> 
> Permission is granted to copy, distribute and/or modify this document
> under the terms of the GNU Free Documentation License, Version 1.3
> or any later version published by the Free Software Foundation;
> with no Invariant Sections, no Front-Cover Texts, and no Back-Cover Texts.

## RSML vs The Alternatives
<div class="grid cards" markdown>



-   :material-new-box:{ .lg .middle } **RSML v2.0.0+**
    
    ---
    :material-language-csharp: Made in C#

    :material-check-all: Logic-first design

    :material-check-all: Pattern matching via simple, fresh syntax

    :material-check-all: Command-line Interface

    :material-check-all: C# and Python support

    :material-check-all: Exposes a C ABI

    :material-check-all: Organized, easy-to-use API

    :material-check-all: Open Source (MIT License)

    :material-check-all: Cross-platform logic evaluation

    :material-check-all: Extremely Performant


-   :material-numeric-1-circle:{ .lg .middle } **RSML v1.x.x**

    ---
    :material-language-csharp: Made in C#

    :material-check-all: Logic-first design

    :material-check: Pattern matching *(RSML v1.x.x uses Regex, which is not that simple and slows down evaluation time)*

    :material-check: Command-line Interface with limited support for RSML's API

    :material-check-all: C# and Python support

    :octicons-x-12: Does not expose a C ABI

    :octicons-x-12: The API is not property organized and is confusing for beginners

    :material-check-all: Open Source (MIT License)

    :material-check-all: Cross-platform logic evaluation

    :octicons-x-12: Slower and allocates a lot more memory

-   :fontawesome-solid-road:{ .lg .middle } **MF's CrossRoad Solution**

    ---

    :fontawesome-brands-python: Made in Python 3

    :material-check: Logic-first design

    :octicons-x-12: Pattern matching via unnecessarily complicated syntax *(MFRoad's syntax is extremely strict, not allowing for spacing and the lack of delimiting on strings feels off to beginners)*

    :octicons-x-12: Command-line Interface

    :material-check: Python support only

    :octicons-x-12: Does not expose a C ABI

    :octicons-x-12: The API is incredibly limited

    :material-check-all: Open Source

    :material-check-all: Cross-platform logic evaluation

    :material-table-question: Performance untested

-   :simple-python:{ .lg .middle } **Scripting Languages _(such as Python)_**

    ---

    :material-check-all: You can use any language you want...

    :octicons-x-12: ...but most times, you'll end up having to pack a whole interpreter when that shouldn't be needed for a simple decision on what logic path to take.

</div>

## Useful Links
To further enhance your experience with RSML, we offer a list of useful links in this journey through our logic-heavy markup language.

<div class="grid cards" markdown>

-   :simple-github:{ .lg .middle } **GitHub Repository**

    ---

    The official repository for Red Sea Markup Language.

    [:octicons-arrow-right-24: Visit](https://github.com/OceanApocalypseStudios/RedSeaMarkupLanguage)

-   :simple-python:{ .lg .middle } **RSML for Python**

    ---

    RSML for Python was the first port of RSML to another programming language to exist.

    [:octicons-arrow-right-24: Visit the repository](https://github.com/OceanApocalypseStudios/RSML.Python/)

-   :simple-nuget:{ .lg .middle } **NuGet Packages** (over 1.4k downloads)

    ---

    RSML for C# is the main way to use RSML, so the NuGet link is going to be one of your best bets for getting help, among other things, even if you're using RSML with a language other than C#.

    [:octicons-arrow-right-24: Modern Version (Recommended)](https://www.nuget.org/packages/OceanApocalypseStudios.RSML/)

    [:octicons-arrow-right-24: Legacy Version (Out of Support)](https://www.nuget.org/packages/RSML/)

-   :material-file-document:{ .lg .middle } **RSML's Documentation Repository.**

    ---

    While it might seem a bit redundant _(after all, you're already here)_, it's useful if you wish to build the documentation locally, that is, if you want a offline copy of it.

    [:octicons-arrow-right-24: Visit the repository](https://github.com/OceanApocalypseStudios/rsml-docs/)

</div>

<!-- md:version 1.0.5 -->
