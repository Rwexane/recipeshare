```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.22631.3880/23H2/2023Update/SunValley3)
13th Gen Intel Core i5-1335U 1.30GHz, 1 CPU, 12 logical and 10 physical cores
.NET SDK 9.0.300
  [Host]     : .NET 8.0.16 (8.0.1625.21506), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.16 (8.0.1625.21506), X64 RyuJIT AVX2


```
| Method     | Mean     | Error     | StdDev    | Allocated |
|----------- |---------:|----------:|----------:|----------:|
| GetRecipes | 7.517 ms | 0.1784 ms | 0.5260 ms |   7.33 KB |
