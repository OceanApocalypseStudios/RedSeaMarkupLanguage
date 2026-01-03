# `#!c# RSML.Exceptions.UndefinedSpecialException` Class
Exception that is thrown when a special action is used but is undefined.

<!-- HIERARCHY -->

## Hierarchy
```mermaid
flowchart TB
    UndefinedSpecialException --> System.Exception
    System.Exception --> System.Object
```

---

<!-- CONSTRUCTORS -->

## Constructors
`UndefinedSpecialException` contains 3 constructor methods.

<!-- 1 -->

### `#!c# UndefinedSpecialException()`
Initializes a new instance of the `UndefinedSpecialException` class.

<!-- 2 -->

### `#!c# UndefinedSpecialException(System.String message)`
Initializes a new instance of the `UndefinedSpecialException` class with a specified error message.

#### Parameters
`#!c# System.String message`

:   The message that describes the error.

<!-- 3 -->

### `#!c# UndefinedSpecialException(System.String? message, Exception? innerException)`
Initializes a new instance of the `UndefinedSpecialException` class with a specified error message and a reference to the error that caused this exception.

#### Parameters
`#!c# System.String? message`

:   The message that describes the error.

`#!c# Exception? innerException`

:   The exception that's the cause of the current exception
