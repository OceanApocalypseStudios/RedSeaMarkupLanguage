# Road-Like
**Name:** Road-Like

**ID:** `roadlike`

**Author:** [OceanApocalypseStudios](https://oceanapocalypsestudios.github.io/)

**Year:** 2025

**License:** Unlicense

A standard made for those who wish to migrate from [MFRoad](https://github.com/mf366-dev/MFRoad) to RSML. It uses MFRoad's operators, so users only need to focus on re-learning the base [syntax](../index.md#syntax-reference), not the operators too.

## Operators
Road-Like defines the following tokens as operators.

| Operator Name | Operator Token | Functionality                                                     |
| ------------- | -------------- | ----------------------------------------------------------------- |
| Primary       | `???`          | Returns `val`.                                                    |
| Secondary     | `<<`           | Outputs `val` to the standard output _(`stdout`)_.                |
| Tertiary      | `!!!`          | Throws an error _(and ends evaluation)_ with error message `val`. |

## Special Actions
!!! note
    `@EndAll` is built into RSML _([see Special Actions](../index.md#special-actions))_.

Road-Like does **not** define any special actions.
