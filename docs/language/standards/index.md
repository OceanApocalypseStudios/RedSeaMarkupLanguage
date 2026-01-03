# RSML Language Standards
!!! tip
    Before proceeding, ensure you understand:

    * [Evaluation Process](../index.md#evaluation-process-flow)
    * [Language Specification](../index.md#language-specification)
    * [Basic Syntax](../index.md#syntax-reference)

Since anyone can create their own RSML language standard, documenting standards is crucial for interoperability and maintenance.

## Specifying which standard is in use
While optional, declaring your standard helps ensure consistent interpretation. That can be done by adding a comment to your [RSML file](../index.md#files), as shown below.

??? note
    * The declaration is purely informational - it doesn't affect parsing.
    * Custom parsers may use this hint to apply correct interpretation rules.
    * Case-sensitive (`official-25` ≠ `Official-25`).

=== "Specifying the standard"
    ```python
    # !standard: <standard-name> (1)
    ```

    1. Must be the first line of the file.

=== "Example 1"
    ```python linenums="1" hl_lines="1"
    # !standard: official-25
    win.+ -> "Primary operator"
    linux.+ || "Secondary operator"
    osx.+ ^! "Tertiary operator"
    ```

=== "Example 2"
    ```python linenums="1" hl_lines="1"
    # !standard: roadlike
    win.+ ??? "Primary operator"
    linux.+ << "Secondary operator"
    osx.+ !!! "Tertiary operator"
    ```

## Documenting your standards
It's impossible for us to document every single standard, but we try to keep the most important ones here.

If you'd like to document your standard, you can follow this simple guide:

1. **Fork** the [rsml-docs repository](https://github.com/OceanApocalypseStudios/rsml-docs/).
2. **Copy** [this file](https://github.com/OceanApocalypseStudios/rsml-docs/blob/main/standard-documentation-template.md) into `/docs/language/standards/`.
3. Give it the **name of your standard** _(if your standard is `mystand`, you'd name it `mystand.md`)_.
4. **Edit the file** with the details of your standard.
5. **Make a [pull request](../../contributing.md).**
6. **After reviewing,** it'll either be **accepted** _(you can then delete your fork)_ or **rejected**, in which case you can either make changes to it and try again or... not.

??? tip
    As long as you just follow the template and only edit what's relevant, your PR should be accepted.

If your standard is relevant enough, we'll do our best to add it here. However, we'd still appreciate the _Pull Request_.
