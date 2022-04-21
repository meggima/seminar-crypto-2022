using FluentAssertions;
using System.Numerics;
using System.Security.Cryptography;
using Xunit;

namespace RingSignature.Tests
{
    public class LsagRingSignerTest
    {
        private readonly PrimeOrderGroup _primeOrderGroup;
        private readonly Random _random;
        private readonly KeyPairGenerator _keyPairGenerator;

        public LsagRingSignerTest()
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
            (BigInteger PrivateKey, BigInteger PublicKey)[] keyPairs = CreateKeyPairs(ringSize);
            BigInteger[] publicKeys = keyPairs.Select(k => k.PublicKey).ToArray();

            byte[] message = CreateMessage();

            LsagRingSigner ringSigner = new LsagRingSigner(_primeOrderGroup, _random);

            // Act
            Signature signature = ringSigner.Sign(message, publicKeys, keyPairs[0].PrivateKey, 0);
            bool isValidSignature = ringSigner.Verify(message, signature, publicKeys);

            // Assert
            isValidSignature.Should().BeTrue();
        }

        [Fact]
        public void RingSignature_ShouldNotBeValid_WhenRingMemberReplaced()
        {
            // Arrange
            const int ringSize = 10;
            (BigInteger PrivateKey, BigInteger PublicKey)[] keyPairs = CreateKeyPairs(ringSize);

            (BigInteger PrivateKey, BigInteger PublicKey) otherKeyPair = _keyPairGenerator.CreateKeyPair();

            byte[] message = CreateMessage();

            LsagRingSigner ringSigner = new LsagRingSigner(_primeOrderGroup, _random);

            // Act
            Signature signature = ringSigner.Sign(message, keyPairs.Select(k => k.PublicKey).ToArray(), keyPairs[0].PrivateKey, 0);

            IEnumerable<BigInteger> publicKeysWithOtherMember = keyPairs.Select(k => k.PublicKey)
                .Take(ringSize - 1) // Remove last member
                .Append(otherKeyPair.PublicKey); // Add other member

            bool isValidSignature = ringSigner.Verify(message, signature, publicKeysWithOtherMember.ToArray());

            // Assert
            isValidSignature.Should().BeFalse();
        }

        [Fact]
        public void RingSignature_ShouldNotBeValid_WhenRingMembersReordered()
        {
            // Arrange
            const int ringSize = 10;
            (BigInteger PrivateKey, BigInteger PublicKey)[] keyPairs = CreateKeyPairs(ringSize);

            byte[] message = CreateMessage();

            LsagRingSigner ringSigner = new LsagRingSigner(_primeOrderGroup, _random);

            // Act
            Signature signature = ringSigner.Sign(message, keyPairs.Select(k => k.PublicKey).ToArray(), keyPairs[0].PrivateKey, 0);

            IEnumerable<BigInteger> reversedPublicKeys = keyPairs.Select(k => k.PublicKey).Reverse();

            bool isValidSignature = ringSigner.Verify(message, signature, reversedPublicKeys.ToArray());

            // Assert
            isValidSignature.Should().BeFalse();
        }

        [Fact]
        public void SignedBySameSigner_ShouldBeTrue_WhenSignedBySameSigner()
        {
            // Arrange
            (BigInteger PrivateKey, BigInteger PublicKey)[] keyPairs = CreateKeyPairs(10);
            BigInteger[] publicKeys = keyPairs.Select(k => k.PublicKey).ToArray();

            byte[] message1 = CreateMessage();
            byte[] message2 = CreateMessage();

            LsagRingSigner ringSigner = new LsagRingSigner(_primeOrderGroup, _random);


            Signature s1 = ringSigner.Sign(message1, publicKeys, keyPairs[0].PrivateKey, 0);
            Signature s2 = ringSigner.Sign(message2, publicKeys, keyPairs[0].PrivateKey, 0);

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
            (BigInteger PrivateKey, BigInteger PublicKey)[] keyPairs = CreateKeyPairs(10);
            BigInteger[] publicKeys = keyPairs.Select(k => k.PublicKey).ToArray();

            byte[] message1 = CreateMessage();
            byte[] message2 = CreateMessage();

            LsagRingSigner ringSigner = new LsagRingSigner(_primeOrderGroup, _random);


            Signature s1 = ringSigner.Sign(message1, publicKeys, keyPairs[0].PrivateKey, 0);
            Signature s2 = ringSigner.Sign(message2, publicKeys, keyPairs[1].PrivateKey, 1); // Sign m2 with other signer

            // Act
            bool isValidS1 = ringSigner.Verify(message1, s1, publicKeys);
            bool isValidS2 = ringSigner.Verify(message2, s2, publicKeys);
            bool sameSigner = ringSigner.SignedBySameSigner(s1, s2);

            // Assert
            isValidS1.Should().BeTrue();
            isValidS2.Should().BeTrue();
            sameSigner.Should().BeFalse();
        }

        [Fact]
        public void SignedBySameSigner_ShouldBeFalse_WhenDifferentMembers()
        {
            // Arrange
            const int ringSize = 10;
            (BigInteger PrivateKey, BigInteger PublicKey)[] keyPairs = CreateKeyPairs(ringSize);

            (BigInteger PrivateKey, BigInteger PublicKey) otherKeyPair = _keyPairGenerator.CreateKeyPair();

            byte[] message1 = CreateMessage();

            LsagRingSigner ringSigner = new LsagRingSigner(_primeOrderGroup, _random);

            BigInteger[] publicKeys1 = keyPairs.Select(k => k.PublicKey).ToArray();
            BigInteger[] publicKeys2 = keyPairs.Select(k => k.PublicKey)
                .Take(ringSize - 1)
                .Append(otherKeyPair.PublicKey)
                .ToArray();

            Signature s1 = ringSigner.Sign(message1, publicKeys1, keyPairs[0].PrivateKey, 0);
            Signature s2 = ringSigner.Sign(message1, publicKeys2, keyPairs[0].PrivateKey, 0);

            // Act
            bool isValidS1 = ringSigner.Verify(message1, s1, publicKeys1);
            bool isValidS2 = ringSigner.Verify(message1, s2, publicKeys2);
            bool sameSigner = ringSigner.SignedBySameSigner(s1, s2);

            // Assert
            isValidS1.Should().BeTrue();
            isValidS2.Should().BeTrue();
            sameSigner.Should().BeFalse();

        }

        private (BigInteger PrivateKey, BigInteger PublicKey)[] CreateKeyPairs(int ringSize)
        {
            (BigInteger PrivateKey, BigInteger PublicKey)[] keyPairs = new (BigInteger PrivateKey, BigInteger PublicKey)[ringSize];

            for (int i = 0; i < ringSize; i++)
            {
                keyPairs[i] = _keyPairGenerator.CreateKeyPair();
            }

            return keyPairs;
        }

        private static byte[] CreateMessage(int length = 100)
        {
            byte[] message = new byte[length];
            RandomNumberGenerator.Fill(message);
            return message;
        }
    }
}
