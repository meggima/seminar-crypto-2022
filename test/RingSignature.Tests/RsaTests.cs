using System.Numerics;
using System.Security.Cryptography;
using Xunit;
using Xunit.Abstractions;

namespace RingSignature.Tests;
public class RsaTests
{
    private readonly ITestOutputHelper _output;

    public RsaTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void ShouldCreateRSAKeyPair()
    {
        RSA rsa = RSA.Create(2048);

        RSAParameters rsaParameters = rsa.ExportParameters(true);

        _output.WriteLine("Public key");
        _output.WriteLine($"n: {new BigInteger(rsaParameters.Modulus!, true, true)}");
        _output.WriteLine($"e: {new BigInteger(rsaParameters.Exponent!, true, true)}");

        _output.WriteLine("Private key");
        _output.WriteLine($"p: {new BigInteger(rsaParameters.P!, true, true)}");
        _output.WriteLine($"q: {new BigInteger(rsaParameters.Q!, true, true)}");
        _output.WriteLine($"d: {new BigInteger(rsaParameters.D!, true, true)}");
    }
}
