# `#!c# RSML.Exceptions.ImmutableActionException` Class
Exception that is thrown when a user attempts to redefine the primary action, which is immutable.

<!-- HIERARCHY -->

## Hierarchy
```mermaid
flowchart TB
    ImmutableActionException --> System.Exception
    System.Exception --> System.Object
```

---

<!-- CONSTRUCTORS -->

## Constructors
`ImmutableActionException` contains 3 constructor methods.

<!-- 1 -->

### `#!c# ImmutableActionException()`
Initializes a new instance of the `ImmutableActionException` class.

<!-- 2 -->

### `#!c# ImmutableActionException(System.String message)`
Initializes a new instance of the `ImmutableActionException` class with a specified error message.

#### Parameters
`#!c# System.String message`

:   The message that describes the error.

<!-- 3 -->

### `#!c# ImmutableActionException(System.String? message, Exception? innerException)`
Initializes a new instance of the `ImmutableActionException` class with a specified error message and a reference to the error that caused this exception.

#### Parameters
`#!c# System.String? message`

:   The message that describes the error.

`#!c# Exception? innerException`

:   The exception that's the cause of the current exception
