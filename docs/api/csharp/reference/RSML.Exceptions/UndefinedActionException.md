# `#!c# RSML.Exceptions.UndefinedActionException` Class
Exception that is thrown when a secondary or tertiary action is used but is undefined.

<!-- HIERARCHY -->

## Hierarchy
```mermaid
flowchart TB
    UndefinedActionException --> System.Exception
    System.Exception --> System.Object
```

---

<!-- CONSTRUCTORS -->

## Constructors
`UndefinedActionException` contains 3 constructor methods.

<!-- 1 -->

### `#!c# UndefinedActionException()`
Initializes a new instance of the `UndefinedActionException` class.

<!-- 2 -->

### `#!c# UndefinedActionException(System.String message)`
Initializes a new instance of the `UndefinedActionException` class with a specified error message.

#### Parameters
`#!c# System.String message`

:   The message that describes the error.

<!-- 3 -->

### `#!c# UndefinedActionException(System.String? message, Exception? innerException)`
Initializes a new instance of the `UndefinedActionException` class with a specified error message and a reference to the error that caused this exception.

#### Parameters
`#!c# System.String? message`

:   The message that describes the error.

`#!c# Exception? innerException`

:   The exception that's the cause of the current exception
