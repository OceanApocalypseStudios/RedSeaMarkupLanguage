# Real-World Usage of the CLI
In this page, you can find real-life examples on the CLI's usage.

## Evaluating from a file
Assuming we have a file called `myfile.rsea`, it's easy to evaluate it without any coding skills.

=== "myfile.rsea"
    ```python
    # !standard: official-25
    # myfile.rsea
    win.+ -> "You're on Windows"
    (ubuntu|debian).* -> "Debian-based Linux"
    .+ -> "Sorry, I don't know your OS"
    ```

=== "In the terminal"
    ```bash
    >>> cat myfile.rsea | RSML.CLI eval --expand-any
    Debian-based Linux
    ```
