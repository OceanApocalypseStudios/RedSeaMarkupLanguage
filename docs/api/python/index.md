# RSML in Python
RSML for Python is not as powerful as [RSML for C#](../csharp/index.md), as it needs the CLI to work and does not have as many features.

## Installation
To start using RSML in Python, simply install it with `pip`.

=== "General Installation"
    ```bash
    pip install rsml-python==1.0.5
    ```

=== "Windows"
    ```powershell
    python -m pip install rsml-python==1.0.5
    ```

## Module Reference Overview
* [`RedSeaCLIExecutable`](RedSeaCLIExecutable.md)
    - `__init__(str | None)`
    - `evaluate_document(str | None, bool, tuple[str | None, str], bool)`
    - `evaluate_document_as_mfroad()`
    - `get_runtime_id()`
    - `load_document(RedSeaDocument)`
    - `repository`
    - `repository_python`
    - `version`

* [`RedSeaDocument`](RedSeaDocument.md)
    - `__init__()`
    - `get_document_data()`
    - `load_from_file(str, str)`
    - `load_from_string(str)`
    - `write_document_to_file(str, str)`
    - `write_document_to_new_list()`
