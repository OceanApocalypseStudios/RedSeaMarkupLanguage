# API Reference _(C\#)_
The API reference for RSML in C# is organized by **namespace** and then **class**.

## Overview
* [`RSML`](RSML/index.md)
    - [`RSDocument`](RSML/RSDocument.md)
        - `#!c# EvaluateDocument()`
        - `#!c# EvaluateDocument(string)`
        - `#!c# EvaluateDocument(bool, string?)`
        - `#!c# EvaluateDocument(string, string?)`
        - `#!c# EvaluateDocument(string, bool, string?)`
        - `#!c# LoadRSMLFromFile(string)`
        - `#!c# LoadRSMLFromFileAsync(string)`
        - `#!c# LoadRSMLFromFileIntoDocument(string)`
        - `#!c# NewFromFile(string)`
        - `#!c# RSDocument(string)`
        - `#!c# RSDocument(StringReader)`
        - `#!c# RSDocument(RSParser)`
        - `#!c# SaveRSMLToFile(string)`
        - `#!c# ToString()`

* [`RSML.Exceptions`](RSML.Exceptions/index.md)
    - [`ImmutableActionException`](RSML.Exceptions/ImmutableActionException.md)
        - `#!c# ImmutableActionException()`
        - `#!c# ImmutableActionException(string)`
        - `#!c# ImmutableActionException(string?, Exception?)`

    - [`RSMLRuntimeException`](RSML.Exceptions/RSMLRuntimeException.md)
        - `#!c# RSMLRuntimeException()`
        - `#!c# RSMLRuntimeException(string)`
        - `#!c# RSMLRuntimeException(string?, Exception?)`

    - [`UndefinedActionException`](RSML.Exceptions/UndefinedActionException.md)
        - `#!c# UndefinedActionException()`
        - `#!c# UndefinedActionException(string)`
        - `#!c# UndefinedActionException(string?, Exception?)`

    - [`UndefinedSpecialException`](RSML.Exceptions/UndefinedSpecialException.md)
        - `#!c# UndefinedSpecialException()`
        - `#!c# UndefinedSpecialException(string)`
        - `#!c# UndefinedSpecialException(string?, Exception?)`

* [`RSML.Parser`](RSML.Parser/index.md)
    - [`OperatorType`](RSML.Parser/OperatorType.md)

    - [`ReadyToGoParser`](RSML.Parser/ReadyToGoParser.md)
        - `#!c# CreateMFRoadLike(string)`
        - `#!c# CreateNew(string)`
        - `#!c# CreateNewFromFilepath(string)`

    - [`RSParser`](RSML.Parser/RSParser.md)
        - `#!c# DefineOperator(OperatorType, string)`
        - `#!c# EvaluateRSML(string)`
        - `#!c# EvaluateRSML(bool, string)`
        - `#!c# EvaluateRSMLWithCustomRid(string, string)`
        - `#!c# EvaluateRSMLWithCustomRid(string, bool, string)`
        - `#!c# RegisterAction(OperatorType, Action<RSParser, string>)`
        - `#!c# RegisterSpecialFunction(string, Func<RSParser, string, byte>)`
        - `#!c# RSParser(string)`
        - `#!c# RSParser(StringReader)`
        - `#!c# RSParser(RSDocument)`
        - `#!c# RSParser(RSParser)`
        - `#!c# ToString()`
