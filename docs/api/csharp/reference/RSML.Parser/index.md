# Namespace Overview
* [`OperatorType`](OperatorType.md)

* [`ReadyToGoParser`](ReadyToGoParser.md)
    - `#!c# CreateMFRoadLike(string)`
    - `#!c# CreateNew(string)`
    - `#!c# CreateNewFromFilepath(string)`

* [`RSParser`](RSParser.md)
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
