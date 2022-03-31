using FluentAssertions;
using System.Numerics;
using System.Security.Cryptography;
using Xunit;

namespace RingSignature.Tests
{
    public class RingSignerTest
    {
        private readonly PrimeOrderGroup _primeOrderGroup;
        private readonly Random _random;
        private readonly KeyPairGenerator _keyPairGenerator;

        public RingSignerTest()
        {
            _primeOrderGroup = WellKnownPrimeOrderGroups.RFC5114_2_3_256;
            _random = new Random();
            _keyPairGenerator = new KeyPairGenerator(_primeOrderGroup, _random);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(100)]
        public void RingSignature_ShouldBeValid(int ringSize)
        {
            // Arrange
            (BigInteger privateKey, BigInteger publicKey)[] keyPairs = CreateKeyPairs(ringSize);

            byte[] message = new byte[100];
            RandomNumberGenerator.Fill(message);

            RingSigner ringSigner = new RingSigner(_primeOrderGroup, _random);

            // Act
            Signature signature = ringSigner.Sign(message, keyPairs.Select(k => k.publicKey).ToArray(), keyPairs[0].privateKey, 0);
            bool isValidSignature = ringSigner.Verify(message, signature, keyPairs.Select(k => k.publicKey).ToArray());

            // Assert
            isValidSignature.Should().BeTrue();
        }

        [Fact]
        public void RingSignature_ShouldNotBeValid_WhenRingMemberReplaced()
        {
            // Arrange
            const int ringSize = 10;
            (BigInteger privateKey, BigInteger publicKey)[] keyPairs = CreateKeyPairs(ringSize);

            var otherKeyPair = _keyPairGenerator.CreateKeyPair();

            byte[] message = new byte[100];
            RandomNumberGenerator.Fill(message);

            RingSigner ringSigner = new RingSigner(_primeOrderGroup, _random);

            // Act
            Signature signature = ringSigner.Sign(message, keyPairs.Select(k => k.publicKey).ToArray(), keyPairs[0].privateKey, 0);

            IEnumerable<BigInteger> publicKeysWithOtherMember = keyPairs.Select(k => k.publicKey)
                .Take(ringSize - 1) // Remove last member
                .Append(otherKeyPair.publicKey); // Add other member

            bool isValidSignature = ringSigner.Verify(message, signature, publicKeysWithOtherMember.ToArray());

            // Assert
            isValidSignature.Should().BeFalse();
        }

        [Fact]
        public void SignedBySameSigner_ShouldBeTrue_WhenSignedBySameSigner()
        {
            // Arrange
            (BigInteger privateKey, BigInteger publicKey)[] keyPairs = CreateKeyPairs(10);
            BigInteger[] publicKeys = keyPairs.Select(k => k.publicKey).ToArray();

            byte[] message1 = new byte[100];
            RandomNumberGenerator.Fill(message1);

            byte[] message2 = new byte[100];
            RandomNumberGenerator.Fill(message2);

            RingSigner ringSigner = new RingSigner(_primeOrderGroup, _random);


            Signature s1 = ringSigner.Sign(message1, publicKeys, keyPairs[0].privateKey, 0);
            Signature s2 = ringSigner.Sign(message2, publicKeys, keyPairs[0].privateKey, 0);

            // Act
            bool isValidS1 = ringSigner.Verify(message1, s1, publicKeys);
            bool isValidS2 = ringSigner.Verify(message2, s2, publicKeys);
            bool sameSigner = ringSigner.SignedBySameSigner(s1, s2);

            // Assert
            isValidS1.Should().BeTrue();
            isValidS2.Should().BeTrue();
            sameSigner.Should().BeTrue();
        }

        [Fact]
        public void SignedBySameSigner_ShouldBeFalse_WhenSignedByDifferentSigners()
        {
            // Arrange
            (BigInteger privateKey, BigInteger publicKey)[] keyPairs = CreateKeyPairs(10);
            BigInteger[] publicKeys = keyPairs.Select(k => k.publicKey).ToArray();

            byte[] message1 = new byte[100];
            RandomNumberGenerator.Fill(message1);

            byte[] message2 = new byte[100];
            RandomNumberGenerator.Fill(message2);

            RingSigner ringSigner = new RingSigner(_primeOrderGroup, _random);


            Signature s1 = ringSigner.Sign(message1, publicKeys, keyPairs[0].privateKey, 0);
            Signature s2 = ringSigner.Sign(message2, publicKeys, keyPairs[1].privateKey, 1); // Sign m2 with other signer

            // Act
            bool isValidS1 = ringSigner.Verify(message1, s1, publicKeys);
            bool isValidS2 = ringSigner.Verify(message2, s2, publicKeys);
            bool sameSigner = ringSigner.SignedBySameSigner(s1, s2);

            // Assert
            isValidS1.Should().BeTrue();
            isValidS2.Should().BeTrue();
            sameSigner.Should().BeFalse();
        }

        private (BigInteger privateKey, BigInteger publicKey)[] CreateKeyPairs(int ringSize)
        {
            (BigInteger privateKey, BigInteger publicKey)[] keyPairs = new (BigInteger privateKey, BigInteger publicKey)[ringSize];

            for (int i = 0; i < ringSize; i++)
            {
                keyPairs[i] = _keyPairGenerator.CreateKeyPair();
            }

            return keyPairs;
        }
    }
}
