using FluentAssertions;
using System.Numerics;
using System.Security.Cryptography;
using Xunit;

namespace RingSignature.Tests
{
    public class RingSignerTest
    {
        private const int RingSize = 100;

        [Fact]
        public void RingSignature_ShouldBeValid()
        {
            // Arrange
            PrimeOrderGroup primeOrderGroup = WellKnownPrimeOrderGroups.RFC5114_2_3_256;

            KeyPairGenerator keyPairGenerator = new KeyPairGenerator(primeOrderGroup);

            (BigInteger privateKey, BigInteger publicKey)[] keyPairs = new (BigInteger privateKey, BigInteger publicKey)[RingSize];

            for (int i = 0; i < RingSize; i++)
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

        [Fact]
        public void RingSignature_ShouldNotBeValid()
        {
            // Arrange
            PrimeOrderGroup primeOrderGroup = WellKnownPrimeOrderGroups.RFC5114_2_3_256;

            KeyPairGenerator keyPairGenerator = new KeyPairGenerator(primeOrderGroup);

            (BigInteger privateKey, BigInteger publicKey)[] keyPairs = new (BigInteger privateKey, BigInteger publicKey)[RingSize];

            for (int i = 0; i < RingSize; i++)
            {
                keyPairs[i] = keyPairGenerator.CreateKeyPair();
            }

            var otherKeyPair = keyPairGenerator.CreateKeyPair();

            byte[] message = new byte[100];
            RandomNumberGenerator.Fill(message);

            RingSigner ringSigner = new RingSigner(primeOrderGroup);

            // Act
            Signature signature = ringSigner.Sign(message, keyPairs.Select(k => k.publicKey).ToArray(), keyPairs[0].privateKey, 0);

            IEnumerable<BigInteger> publicKeysWithOtherMember = keyPairs.Select(k => k.publicKey)
                .Take(RingSize - 1) // Remove last member
                .Append(otherKeyPair.publicKey); // Add other member

            bool isValidSignature = ringSigner.Verify(message, signature, publicKeysWithOtherMember.ToArray());

            // Assert
            isValidSignature.Should().BeFalse();
        }
    }
}
