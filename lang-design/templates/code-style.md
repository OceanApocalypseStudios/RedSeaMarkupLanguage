# Proposal: [Code style rule]
**Author:** [Your name]

**Date:** [YYYY-MM-DD]

---

## 1. Metadata
* **Default Severity:** [Info/Warning/Error]
* **Toggled by Default:** [yes/no]
* **Rule Name:** [Name goes here]
* **Rule Code:** RS[XXXX]

## 2. Context
Provide a concise summary of what leads to this rule, without explaining why it's better to have this rule.

*Example: if adding a code style rule that enforces the avoidance of X, explain what X is without explaining why X is bad.*

## 2. Motivation
Explain why this rule improves developer experience and overall code style.

## 3. Non-compliant examples
Add multiple non-compliant examples.

```rsea
# Example 01: mutable variable that doesn't suffer mutations
let mut myToast = $Toast.new;  # non-compliant
```

## 4. Compliant examples
Add at least one fix that makes the examples above compliant.

```rsea
# Example 01
let myToast = $Toast.new;  # now it's compliant
```

## 5. Exceptions
Are there any exceptions to this rule?

*Example: if a mutable variable is never mutated, but a function requires it to be mutable, then this rule does not trigger.*

## 6. Potential Drawbacks
Name the drawbacks this feature could/would add, including performance issues, ambiguities, and more.

## 7. Detailed Specification
Use this section to add even more details.
