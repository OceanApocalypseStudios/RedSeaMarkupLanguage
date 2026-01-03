# Official 25
**Name:** Official 25

**ID:** `official-25`

**Author:** [OceanApocalypseStudios](https://oceanapocalypsestudios.github.io/)

**Year:** 2025

**License:** Unlicense

The language standard that's closest to an official language specification. Designed in 2025 by OceanApocalypseStudios to be universal and, for the lack of a better word, standard. It's used as the main language standard in the CLI as of v1.0.1 of RSML.

## Operators
Official 25 defines the following tokens as operators.

| Operator Name | Operator Token | Functionality                                                     |
| ------------- | -------------- | ----------------------------------------------------------------- |
| Primary       | `->`           | Returns `val`.                                                    |
| Secondary     | `||`           | Outputs `val` to the standard output _(`stdout`)_.                |
| Tertiary      | `^!`           | Throws an error _(and ends evaluation)_ with error message `val`. |

## Special Actions
!!! note
    `@EndAll` is built into RSML _([see Special Actions](../index.md#special-actions))_.

Official 25 does **not** define any special actions.
