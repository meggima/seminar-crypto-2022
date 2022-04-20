``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.22000
11th Gen Intel Core i7-11800H 2.30GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK=6.0.201
  [Host]     : .NET 6.0.3 (6.0.322.12309), X64 RyuJIT
  DefaultJob : .NET 6.0.3 (6.0.322.12309), X64 RyuJIT


```
|          Method | RingSize |       Mean |    Error |   StdDev | Allocated |
|---------------- |--------- |-----------:|---------:|---------:|----------:|
|     **SignMessage** |       **10** |   **143.8 ms** |  **2.79 ms** |  **4.09 ms** |    **289 KB** |
| VerifySignature |       10 |   144.0 ms |  2.80 ms |  2.99 ms |    295 KB |
|     **SignMessage** |      **100** | **1,416.1 ms** | **26.87 ms** | **28.75 ms** |  **7,977 KB** |
| VerifySignature |      100 | 1,419.0 ms | 25.41 ms | 29.26 ms |  7,972 KB |
