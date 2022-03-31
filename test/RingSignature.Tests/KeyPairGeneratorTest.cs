using FluentAssertions;
using System.Numerics;
using Xunit;

namespace RingSignature.Tests;

public class KeyPairGeneratorTest
{
    [Fact]
    public void GenerateKeyPair_ShouldGenerateKeyPair()
    {
        // Arrange
        KeyPairGenerator keyPairGenerator = new(WellKnownPrimeOrderGroups.RFC5114_2_3_256, new Random());

        // Act
        (BigInteger PrivateKey, BigInteger PublicKey) keyPair = keyPairGenerator.CreateKeyPair();

        // Assert
        keyPair.PrivateKey.Should().BeGreaterThan(BigInteger.Zero);
        keyPair.PublicKey.Should().BeGreaterThan(BigInteger.Zero);
    }
}
