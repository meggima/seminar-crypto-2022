using FluentAssertions;
using System.Numerics;
using System.Security.Cryptography;
using Xunit;

namespace RingSignature.Tests
{
    public class RingSignerTest
    {
        [Fact]
        public void RingSignature_ShouldGenerateVerifiableSignature()
        {
            // Arrange
            PrimeOrderGroup primeOrderGroup = WellKnownPrimeOrderGroups.RFC5114_2_3_256;

            KeyPairGenerator keyPairGenerator = new KeyPairGenerator(primeOrderGroup);

            (BigInteger privateKey, BigInteger publicKey)[] keyPairs = new (BigInteger privateKey, BigInteger publicKey)[10];

            for (int i = 0; i < 10; i++)
            {
                keyPairs[i] = keyPairGenerator.CreateKeyPair();
            }

            byte[] message = new byte[100];
            RandomNumberGenerator.Fill(message);

            RingSigner ringSigner = new RingSigner(primeOrderGroup);

            // Act
            Signature signature = ringSigner.Sign(message, keyPairs.Select(k => k.publicKey).ToArray(), keyPairs[0].privateKey, 0);
            bool isValidSignature = ringSigner.Verify(message, signature, keyPairs.Select(k => k.publicKey).ToArray());

            // Assert
            isValidSignature.Should().BeTrue();
        }
    }
}
