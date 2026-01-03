# CLI Commands
In this page, you can find all of the available commands in the CLI.

## Global Options
These are options that can be used globally and are compatible with all the commands.

|         Option         | Datatype |                   Description                   |                Default Value                |
| ---------------------- | -------- | ----------------------------------------------- | ------------------------------------------- |
| `--no-pretty`          | `bool`   | Disables colors and ASCII art.                  | `false` (Pretty Output defaults to enabled) |
| `--version`            | N/A      | Returns the CLI's version.                      | N/A                                         |
| `--help` / `-?` / `-h` | N/A      | Shows the help message for the current command. | N/A                                         |

---

## `evaluate`
This command evaluates RSML using the [`official-25` language standard](../language/standards/official-25.md).

It reads the RSML data from the `stdin`.

**Aliases:**

* `eval`
* `parse`

### Options
|              Option              | Datatype |                       Description                       |                       Default Value                       |
| -------------------------------- | -------- | ------------------------------------------------------- | --------------------------------------------------------- |
| `-F` / `--antierror-fallback`    | `string` | A message to display when an error happens.             | `#!c# $"[ERROR] User-triggered error occured -> {value}"` |
| `-f` / `--antinull-fallback`     | `string` | A message to display when there are no matches.         | `#!c# "[WARNING] No match was found."`                    |
| `-r` / `--custom-rid` / `--rid`  | `string` | Custom RID to check against instead of the host's RID.  | Host's RID                                                |
| `-x` / `--expand-any`            | `bool`   | Expands `any` into `.+`.                                | `false`                                                   |
| `-p` / `--primary-only`          | `bool`   | Executes only the primary operator, if applicable.      | `false`                                                   |

---

## `roadlike`
This command evaluates RSML using the [`roadlike` language standard](../language/standards/roadlike.md).

It reads the RSML data from the `stdin`.

**Aliases:**

* `emulate-mfroad`
* `mfroad-like`

It cannot be customized and uses the default values of the options in the [`eval` command](#evaluate).

---

## `get-rid`
This command returns the host's RID.

**Aliases:**

* `get-runtime-id`
* `get-runtime-identifier`
* `rid-get`

---

## `repository`
This command returns a link to [RSML's GitHub Repository](https://github.com/OceanApocalypseStudios/RedSeaMarkupLanguage/).

**Aliases:**

* `about`
* `github`
* `online`
* `repo`
