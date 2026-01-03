# Template Standard
<!-- NAME -> a friendly name, like "My Standard" -->
<!-- ID -> a unique (or almost unique) ID, used for identifying your standard, such as "my-standard" or "mystand" -->

**Name:** FRIENDLY_NAME

**ID:** `YOUR_STANDARD`

**Author:** YOUR_NAME_HERE

**Year:** YEAR_GOES_HERE

<!--
If your standard is licensed, put the license in which it's licensed, not the copyright message.
--
If your standard is NOT licensed, you may either replace "LICENSE_NAME_HERE" with "Unlicense"
OR
you may simply hide "License" and "Copyright" fields, which is simpler. -->
**License:** LICENSE_NAME_HERE

<!-- If your standard is licensed, uncomment the line below. -->
<!-- _Copyright &copy; <name>  <year>_ -->

Brief description of your standard goes here.

## Operators
YOUR_STANDARD defines the following tokens as operators.

<!-- In the table below, don't change the operator names. -->
<!-- In the table below, don't change the functionality of the secondary operator. -->
<!-- In the table below, use `val` as the argument that's passed. -->

| Operator Name | Operator Token | Functionality  |
| ------------- | -------------- | -------------- |
| Primary       | `YOUR_TOKEN`   | Returns `val`. |
| Secondary     | `YOUR_TOKEN`   | WHAT_IT_DOES   |
| Tertiary      | `YOUR_TOKEN`   | WHAT_IT_DOES   |

## Special Actions
!!! note
    `@EndAll` is built into RSML _([see Special Actions](../index.md#special-actions))_.

<!-- The link above will work when your standard gets included in the documentation. -->

<!--
UNCOMMENT IF YOUR STANDARD DOES NOT HAVE ANY SPECIAL ACTIONS
============================================================
YOUR_STANDARD does **not** define any special actions.
-->

<!-- COMMENT THE NEXT LINES IF YOUR STANDARD DOES NOT HAVE ANY SPECIAL ACTIONS -->

YOUR_STANDARD defines the following special actions.

<!-- In the table below, do NOT add EndAll. -->
<!-- In the table below, add as many rows as necessary. -->
<!-- In the table below, if your action does not take any arguments, replace "WHAT_IT_MEANS" with "Argument-less.", as shown below. -->
<!-- In the table below, if your action does not take any arguments, replace "BYTE" with the actual number, such as 2. -->

| Action Name | Brief Description | Argument Description | Return Value |
| ----------- | ----------------- | -------------------- | ------------ |
| `MY_ACTION` | WHAT_IT_DOES      | Argument-less.       | BYTE         |
| `MY_ACTION` | WHAT_IT_DOES      | Argument-less.       | BYTE         |
| `MY_ACTION` | WHAT_IT_DOES      | WHAT_IT_MEANS        | BYTE         |
| `MY_ACTION` | WHAT_IT_DOES      | WHAT_IT_MEANS        | BYTE         |
