# RSML vs its alternatives
There are multiple alternatives to RSML that provide its base funcionality, each with their own benefits and drawbacks. Below is a comparison table of RSML against its alternatives, showcasing why RSML is the best choice for machine-dependent logic evaluation in your projects.

<div class="grid cards" markdown>



-   :material-new-box:{ .lg .middle } **RSML v2.0.0+** (Modern)
    
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


-   :material-numeric-1-circle:{ .lg .middle } **RSML v1.x.x** (Legacy)

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

-   :fontawesome-solid-road:{ .lg .middle } **MF's CrossRoad Solution** (Deprecated)

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

In summary, as shwon above, RSML v2.0.0+ stands out as the most modern, efficient, and user-friendly solution for machine-dependent logic evaluation compared to its alternatives. Its logic-first design, simple pattern matching syntax, cross-platform support, and high performance make it the ideal choice for developers looking to implement robust logic evaluation in their applications.
