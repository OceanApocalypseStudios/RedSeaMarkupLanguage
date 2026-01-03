<!-- Copyright (c)  2025  OceanApocalypseStudios -->
<!-- Permission is granted to copy, distribute and/or modify this document -->
<!-- under the terms of the GNU Free Documentation License, Version 1.3 -->
<!-- or any later version published by the Free Software Foundation; -->
<!-- with no Invariant Sections, no Front-Cover Texts, and no Back-Cover Texts. -->

# API Usage Examples
These are usage examples of the RSML API.

## Package Manager
In the example below, RSML is used in the context of a package manager. The user packaging their app adds a [`.rsea` file](../language/index.md#files) that the package manager evaluates. Each logic path of the file takes the package manager to a different script, given how different build/installation setup might be given the OS and CPU architecture. This example only exemplifies how RSMl comes into play, it is not a guide on how to create package managers.

=== "C#"
    ```c# linenums="1"
    using System.IO;
    using RSML.Parser;

    /*
        * Adapted from rsml-demos
        * For the full code, see https://github.com/OceanApocalypseStudios/rsml-demos/
    */

    string PACKAGE_NAME = "Example Package";

    // Reading from the file
    // (Assume we're passed a FileInfo named rsmlFile)
    string data = File.ReadAllText(rsmlFile.FullName);

    // Creating the parser
    RSParser parser = new(data);

    // Setting up the parser to match official-25
    parser.DefineOperator(OperatorType.Primary, "->");
    parser.DefineOperator(OperatorType.Secondary, "||");
    parser.DefineOperator(OperatorType.Tertiary, "^!");

    parser.RegisterAction(OperatorType.Secondary, (_, value) => Console.WriteLine(value));
    parser.RegisterAction(OperatorType.Tertiary, (_, value) => throw new RSMLRuntimeException(value));

    // Parsing
    string? result = parser.EvaluateRSML(true) ??
        throw new EvaluateException($"No setup scripts found for this machine in {PACKAGE_NAME}.");
    
    FileInfo file = new(result);

    return !file.Exists ?
        throw new FileNotFoundException("Such script does not exist.")
        : file;
    ```

=== "Python"
    ```python linenums="1"
    from rsml_python import RedSeaDocument, RedSeaCLIExecutable

    # Creating the executable
    PATH_TO_EXE: str = ... # insert path here
    RS_EXE: RedSeaCLIExecutable = RedSeaCLIExecutable(PATH_TO_EXE)

    # Creating the document
    doc: RedSeaDocument = RedSeaDocument()
    doc.load_from_string(data)

    # Loading the document into the executable
    RS_EXE.load_document(doc)

    # Parsing
    result: str = RS_EXE.evaluate_document(expands_any=True)

    # Checking the result
    if result.startswith(("[WARNING]", "[ERROR]")):
        raise ValueError(f"No setup scripts found for this machine in {PACKAGE_NAME}.")
    ```
