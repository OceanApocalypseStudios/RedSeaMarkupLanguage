# Hello, World! Guide
This guide will teach you the very basics of evaluating RSML via a programming language.

This guide assumes you're familiar with your language of choice and already [setup RSML for it](../index.md).

## The actual RSML
To keep things simple, we will store our RSML as a `string`.

=== "C#"
    ```c# linenums="1" hl_lines="3 15"
    using System;

    using RSML.Parser;


    namespace MyAwesomeProgram
    {

        internal class Program
        {

            static void Main(string[] args)
            {

                string myAwesomeRsml = ".+ -> \"Hello, World!\"";

            }

        }

    } 
    ```

=== "Python"
    ```python linenums="1" hl_lines="1 3"
    from rsml_python import RedSeaCLIExecutable, RedSeaDocument

    my_awesome_rsml: str = ".+ -> \"Hello, World!\""
    ```

`#!c# "Hello, World!"` will be triggered no matter what, as our Regex expression matches anything that has more than 1 character, which is the case of all MSBuild RIDs.

## Setting up the parser
=== "C#"
    ```c# linenums="12" hl_lines="5"
    static void Main(string[] args)
    {

        string myAwesomeRsml = ".+ -> \"Hello, World!\"";
        var parser = ReadyToGoParser.CreateNew(myAwesomeRsml); // (1)!

    }
    ```

    1. This line creates a [`RSParser`](../csharp/reference/RSML.Parser/RSParser.md) with the [`official-25` standard](../../language/standards/official-25.md).

=== "Python"
    ```python linenums="1" hl_lines="5"
    from rsml_python import RedSeaCLIExecutable, RedSeaDocument

    my_awesome_rsml: str = ".+ -> \"Hello, World!\""

    EXECUTABLE = RedSeaCLIExecutable("Insert the path to the CLI") # (1)!
    ```

    1. You don't have an actual parser in [RSML for Python](../python/index.md).

If you wish to use a different [language standard](../../language/standards/index.md), you can freely do so in C#. In Python, it's slightly trickier as you can only pick from `official-25` and [`roadlike`](../../language/standards/roadlike.md).

## Evaluating
=== "C#"
    ```c# linenums="15" hl_lines="3"
    string myAwesomeRsml = ".+ -> \"Hello, World!\"";
    var parser = ReadyToGoParser.CreateNew(myAwesomeRsml); // (1)!
    var output = parser.EvaluateRSML(); // (2)!
    ```

    1. This line creates a [`RSParser`](../csharp/reference/RSML.Parser/RSParser.md) with the [`official-25` standard](../../language/standards/official-25.md).
    2. The output will be `#!c# null` if there are no matches.

=== "Python"
    ```python linenums="1" hl_lines="7-11"
    from rsml_python import RedSeaCLIExecutable, RedSeaDocument

    my_awesome_rsml: str = ".+ -> \"Hello, World!\""

    EXECUTABLE = RedSeaCLIExecutable("Insert the path to the CLI") # (1)!

    doc = RedSeaDocument()
    doc.load_from_string(my_awesome_rsml)

    EXECUTABLE.load_document(doc)
    output = EXECUTABLE.evaluate_document() # (2)!
    ```

    1. You don't have an actual parser in [RSML for Python](../python/index.md).
    2. This will use the `official-25` language standard.

Note that, for Python, there is a whole new class, called the **CLI Executable**, which points to an existing executable of [**RSML's CLI**](../../cli/index.md) on disk.

For C#, evaluation is simpler and more customizable, too.

## Getting Output
Finally, we can output the result... as long as it isn't `#!c# null`, of course.

=== "C#"
    ```c# linenums="15" hl_lines="5-13"
    string myAwesomeRsml = ".+ -> \"Hello, World!\"";
    var parser = ReadyToGoParser.CreateNew(myAwesomeRsml); // (1)!
    var output = parser.EvaluateRSML(); // (2)!

    if (output is null) // (3)!
    {

        Console.WriteLine("No match!");
        return;

    }

    Console.WriteLine($"Output: {output}");
    ```

    1. This line creates a [`RSParser`](../csharp/reference/RSML.Parser/RSParser.md) with the [`official-25` standard](../../language/standards/official-25.md).
    2. The output will be `#!c# null` if there are no matches.
    3. This situation will never be true, considering our RSML string contains a **wildcard** situation (`.+`) to avoid no-match behaviors.

=== "Python"
    ```python linenums="1" hl_lines="13-17"
    from rsml_python import RedSeaCLIExecutable, RedSeaDocument

    my_awesome_rsml: str = ".+ -> \"Hello, World!\""

    EXECUTABLE = RedSeaCLIExecutable("Insert the path to the CLI") # (1)!

    doc = RedSeaDocument()
    doc.load_from_string(my_awesome_rsml)

    EXECUTABLE.load_document(doc)
    output = EXECUTABLE.evaluate_document() # (2)!

    if output.startswith("[WARNING]"): # (3)!
        print("No match!") # (4)!
        exit()

    print(f"Output: {output}")
    ```

    1. You don't have an actual parser in [RSML for Python](../python/index.md).
    2. This will use the `official-25` language standard.
    3. Default error messages start with `[ERROR]`, while default warning messages start with `[WARNING]`. No-match behavior is considered a non-critical error by the CLI, which means it's treated as a **warning**.
    4. This situation will never be true, considering our RSML string contains a **wildcard** situation (`.+`) to avoid no-match behaviors.

## Use of `any`
Right now, we are using `.+` as our wildcard match, but that is not readable to someone who doesn't know that much Regex. Thankfully, both the **C#** and **Python** APIs support the **expansion of any**. Basically, when the word `any` is encountered in the position of a _possible match_, it is "expanded" (replaced by) to the Regex expression `.+`, so you don't have to write it manually.

The only change we'll need to do is the following one:
=== "C#"
    ```c# linenums="17"
    var output = parser.EvaluateRSML(true); // (1)!
    ```

    1. This uses a different overload of the `EvaluateRSML()` method.

=== "Python"
    ```python linenums="11"
    output = EXECUTABLE.evaluate_document(expand_any=True)
    ```

## Conclusion
You have finished this guy and evaluated your very first RSML string. **Congrats!**

RSML is extremely powerful when you want **dynamic logic path resolution** based on the OS and CPU architecture. Note that it does not benefit you in any way to use RSML as if it were an `if`/`else` based on the host.

It is supposed to be embedded and for it to decide on certain aspects that should only be set if the host has certain characteristics.
