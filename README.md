# Implementation of Linkable Spontaneous Anonymous Group signature scheme (LSAG)

This project implements the LSAG signature scheme proposed by [Liu et al.](https://eprint.iacr.org/2004/027.pdf) in 2004.

The implementation was created during the seminar "Cryptography and Data Security" at the university of Bern, Switzerland in 2022. The implementation is not to be used for production usage and serves only for demo purposes.

## Getting started

Install the [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0).

The project is packaged as a library and, thus, contains no executable. Instead you can run the tests to see the implementation in action:

Open a terminal and navigate to the root of the project.

Then run the command `dotnet test` to run the tests.

### Benchmarks

The project contains benchmarks using Benchmark.NET. To run them, do the following:

```
cd tests/RingSignature.Benchmarks
dotnet run -c RELEASE
```