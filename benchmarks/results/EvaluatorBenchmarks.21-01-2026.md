```

BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.6466/22H2/2022Update)
Intel Core i7-5500U CPU 2.40GHz (Broadwell), 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.102
  [Host]         : .NET 10.0.2 (10.0.2, 10.0.225.61305), X64 RyuJIT x86-64-v3
  .NET 10.0      : .NET 10.0.2 (10.0.2, 10.0.225.61305), X64 RyuJIT x86-64-v3
  NativeAOT 10.0 : .NET 10.0.2, X64 NativeAOT x86-64-v3

InvocationCount=1  UnrollFactor=1  

```
| Method                              | Runtime        | Mean           | Error         | Median         | Ratio | RatioSD | Allocated | Alloc Ratio |
|------------------------------------ |--------------- |---------------:|--------------:|---------------:|------:|--------:|----------:|------------:|
| Evaluate_ComplexContent_1           | .NET 10.0      | 2,185,204.2 ns | 444,076.33 ns | 1,708,000.0 ns |  4.90 |    3.05 |   58848 B |       19.21 |
| Evaluate_ComplexContent_2           | .NET 10.0      | 1,786,833.5 ns | 285,440.44 ns | 1,601,850.0 ns |  4.01 |    1.96 |   67936 B |       22.17 |
| Evaluate_ComplexContent_3           | .NET 10.0      | 1,848,370.7 ns | 303,637.27 ns | 1,698,550.0 ns |  4.14 |    2.09 |   62064 B |       20.26 |
| Evaluate_LargeContent               | .NET 10.0      | 8,384,125.3 ns | 246,755.78 ns | 8,336,200.0 ns | 18.80 |    3.82 |  304024 B |       99.22 |
| Evaluate_MediumContent              | .NET 10.0      |   458,868.8 ns |  26,955.49 ns |   476,050.0 ns |  1.03 |    0.25 |    3064 B |        1.00 |
| Evaluate_SmallContent               | .NET 10.0      |    53,715.8 ns |   4,453.38 ns |    57,400.0 ns |  0.12 |    0.04 |     248 B |        0.08 |
|                                     |                |                |               |                |       |         |           |             |
| Evaluate_ComplexContent_1           | NativeAOT 10.0 |   778,949.0 ns |  75,406.80 ns |   819,150.0 ns |  7.22 |    3.06 |   58896 B |       18.93 |
| Evaluate_ComplexContent_2           | NativeAOT 10.0 |   851,332.5 ns |  25,806.68 ns |   860,150.0 ns |  7.89 |    2.50 |   68320 B |       21.95 |
| Evaluate_ComplexContent_3           | NativeAOT 10.0 | 1,058,785.6 ns |  30,354.34 ns | 1,074,400.0 ns |  9.81 |    3.09 |   62448 B |       20.07 |
| Evaluate_LargeContent               | NativeAOT 10.0 | 7,020,540.0 ns | 547,228.01 ns | 7,012,850.0 ns | 65.04 |   25.09 |  304408 B |       97.82 |
| Evaluate_MediumContent              | NativeAOT 10.0 |   116,192.8 ns |   9,989.56 ns |   117,900.0 ns |  1.08 |    0.43 |    3112 B |        1.00 |
| Evaluate_SmallContent               | NativeAOT 10.0 |    18,603.5 ns |   1,353.39 ns |    17,650.0 ns |  0.17 |    0.06 |     632 B |        0.20 |
|                                     |                |                |               |                |       |         |           |             |
| IsComment_False_Medium              | .NET 10.0      |       707.7 ns |      83.59 ns |       700.0 ns |  0.55 |    0.26 |         - |          NA |
| IsComment_False_Small               | .NET 10.0      |       668.1 ns |      95.50 ns |       600.0 ns |  0.52 |    0.28 |         - |          NA |
| IsComment_True_Medium               | .NET 10.0      |     1,329.3 ns |     124.66 ns |     1,300.0 ns |  1.04 |    0.44 |         - |          NA |
| IsComment_True_Small                | .NET 10.0      |     1,407.5 ns |     155.60 ns |     1,400.0 ns |  1.10 |    0.50 |         - |          NA |
|                                     |                |                |               |                |       |         |           |             |
| IsComment_False_Medium              | NativeAOT 10.0 |       598.9 ns |     113.89 ns |       500.0 ns |  0.59 |    0.43 |         - |          NA |
| IsComment_False_Small               | NativeAOT 10.0 |       595.7 ns |      87.31 ns |       600.0 ns |  0.59 |    0.37 |         - |          NA |
| IsComment_True_Medium               | NativeAOT 10.0 |       814.0 ns |     129.27 ns |       800.0 ns |  0.80 |    0.53 |         - |          NA |
| IsComment_True_Small                | NativeAOT 10.0 |     1,174.4 ns |     146.40 ns |     1,200.0 ns |  1.16 |    0.66 |         - |          NA |
|                                     |                |                |               |                |       |         |           |             |
| Evaluate_PrimitiveAction            | .NET 10.0      |    27,271.3 ns |   4,049.08 ns |    20,950.0 ns |     ? |       ? |     240 B |           ? |
| Evaluate_PrimitiveComment           | .NET 10.0      |    11,566.2 ns |   1,492.49 ns |    10,150.0 ns |     ? |       ? |      24 B |           ? |
| Evaluate_PrimitiveCommentWhitespace | .NET 10.0      |    14,467.7 ns |   2,352.82 ns |    11,500.0 ns |     ? |       ? |      24 B |           ? |
| Evaluate_PrimitiveLogic             | .NET 10.0      |    31,157.9 ns |   2,881.08 ns |    27,200.0 ns |     ? |       ? |     280 B |           ? |
| Evaluate_PrimitiveNewlines          | .NET 10.0      |    35,028.4 ns |   2,216.70 ns |    32,800.0 ns |     ? |       ? |      24 B |           ? |
|                                     |                |                |               |                |       |         |           |             |
| Evaluate_PrimitiveAction            | NativeAOT 10.0 |     8,024.2 ns |   1,101.56 ns |     6,700.0 ns |     ? |       ? |     528 B |           ? |
| Evaluate_PrimitiveComment           | NativeAOT 10.0 |     4,857.0 ns |     680.71 ns |     4,000.0 ns |     ? |       ? |      24 B |           ? |
| Evaluate_PrimitiveCommentWhitespace | NativeAOT 10.0 |     6,204.4 ns |     693.23 ns |     5,450.0 ns |     ? |       ? |     408 B |           ? |
| Evaluate_PrimitiveLogic             | NativeAOT 10.0 |    13,147.1 ns |   1,391.19 ns |    12,100.0 ns |     ? |       ? |     952 B |           ? |
| Evaluate_PrimitiveNewlines          | NativeAOT 10.0 |     9,511.6 ns |   1,743.13 ns |     7,200.0 ns |     ? |       ? |     696 B |           ? |
