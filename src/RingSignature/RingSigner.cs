using System.Numerics;
using System.Security.Cryptography;

namespace RingSignature;

public class RingSigner : IRingSigner
{
    private readonly HashAlgorithm _hash1Function;
    private readonly HashAlgorithm _hash2Function;
    private readonly PrimeOrderGroup _primeOrderGroup;
    private readonly IRandom _random;

    public RingSigner(PrimeOrderGroup primeOrderGroup, IRandom random)
    {
        byte[] hash1Key = new byte[64];
        byte[] hash2Key = new byte[64];

        random.Fill(hash1Key);
        random.Fill(hash2Key);

        _hash1Function = new HMACSHA256(hash1Key);
        _hash2Function = new HMACSHA256(hash2Key);
        _primeOrderGroup = primeOrderGroup;
        _random = random;
    }

    public Signature Sign(byte[] message, BigInteger[] publicKeys, BigInteger signerPrivateKey, int signerPublicKeyIndex)
    {
        BigInteger h = Hash2(publicKeys);

        BigInteger yTilde = BigInteger.ModPow(h, signerPrivateKey, _primeOrderGroup.Prime);
        byte[] yTildeBytes = yTilde.ToByteArray(true, true);

        BigInteger[] sVector = new BigInteger[publicKeys.Length];

        BigInteger u = _random.GetRandomNumber(_primeOrderGroup.SubgroupSize);

        IList<byte[]> publicKeysBytes = publicKeys.Select(p => p.ToByteArray(true, true)).ToList();

        BigInteger currentC = Hash1(
            publicKeysBytes,
            yTildeBytes,
            message,
            BigInteger.ModPow(_primeOrderGroup.Generator, u, _primeOrderGroup.Prime).ToByteArray(true, true),
            BigInteger.ModPow(h, u, _primeOrderGroup.Prime).ToByteArray(true, true));

        BigInteger zeroC = BigInteger.Zero;
        BigInteger signerC = BigInteger.Zero;

        if ((signerPublicKeyIndex + 1) % publicKeys.Length == 0)
        {
            zeroC = currentC;
        }

        if ((signerPublicKeyIndex + 1) % publicKeys.Length == signerPublicKeyIndex)
        {
            signerC = currentC;
        }

        for (int i = (signerPublicKeyIndex + 1) % publicKeys.Length; i != signerPublicKeyIndex; i = (i + 1) % publicKeys.Length)
        {
            sVector[i] = _random.GetRandomNumber(_primeOrderGroup.SubgroupSize);

            BigInteger v1 = (BigInteger.ModPow(_primeOrderGroup.Generator, sVector[i], _primeOrderGroup.Prime) * BigInteger.ModPow(publicKeys[i], currentC, _primeOrderGroup.Prime))
                % _primeOrderGroup.Prime;

            BigInteger v2 = (BigInteger.ModPow(h, sVector[i], _primeOrderGroup.Prime) * BigInteger.ModPow(yTilde, currentC, _primeOrderGroup.Prime))
                % _primeOrderGroup.Prime;

            currentC = Hash1(
                publicKeysBytes,
                yTildeBytes,
                message,
                v1.ToByteArray(true, true),
                v2.ToByteArray(true, true));

            if ((i + 1) % publicKeys.Length == 0)
            {
                zeroC = currentC;
            }

            if ((i + 1) % publicKeys.Length == signerPublicKeyIndex)
            {
                signerC = currentC;
            }
        }

        BigInteger dividend = (u - signerPrivateKey * signerC);

        sVector[signerPublicKeyIndex] = dividend >= 0 ? dividend % _primeOrderGroup.SubgroupSize : _primeOrderGroup.SubgroupSize + (dividend % _primeOrderGroup.SubgroupSize);

        return new Signature(zeroC, sVector, yTilde);
    }

    public bool Verify(byte[] message, Signature signature, BigInteger[] publicKeys)
    {
        BigInteger h = Hash2(publicKeys);

        IList<byte[]> publicKeysBytes = publicKeys.Select(p => p.ToByteArray(true, true)).ToList();
        BigInteger yTilde = signature.Y;
        byte[] yTildeBytes = yTilde.ToByteArray(true, true);

        BigInteger zPrime = BigInteger.Zero;
        BigInteger zPrimePrime = BigInteger.Zero;
        BigInteger currentC = signature.C;

        for (int i = 0; i < publicKeys.Length; i++)
        {
            zPrime = (BigInteger.ModPow(_primeOrderGroup.Generator, signature.S[i], _primeOrderGroup.Prime) * BigInteger.ModPow(publicKeys[i], currentC, _primeOrderGroup.Prime))
                % _primeOrderGroup.Prime;

            zPrimePrime = (BigInteger.ModPow(h, signature.S[i], _primeOrderGroup.Prime) * BigInteger.ModPow(yTilde, currentC, _primeOrderGroup.Prime))
                % _primeOrderGroup.Prime;

            if (i < publicKeys.Length - 1)
            {
                currentC = Hash1(
                    publicKeysBytes,
                    yTildeBytes,
                    message,
                    zPrime.ToByteArray(true, true),
                    zPrimePrime.ToByteArray(true, true));
            }
        }

        BigInteger hashed = Hash1(
                    publicKeysBytes,
                    yTildeBytes,
                    message,
                    zPrime.ToByteArray(true, true),
                    zPrimePrime.ToByteArray(true, true));

        return signature.C == hashed;
    }

    public bool SignedBySameSigner(Signature signature1, Signature signature2)
    {
        return signature1.Y == signature2.Y;
    }

    private BigInteger Hash1(IList<byte[]> publicKeysBytes, params byte[][] components)
    {
        byte[] bytes = ByteHelper.ConcatBytes(publicKeysBytes, components);

        byte[] hash = _hash1Function.ComputeHash(bytes);

        return new BigInteger(hash, true, true) % _primeOrderGroup.SubgroupSize;
    }

    private BigInteger Hash2(IList<BigInteger> input)
    {
        byte[] bytes = ByteHelper.ConcatBytes(input);

        byte[] hash = _hash2Function.ComputeHash(bytes);

        return BigInteger.ModPow(_primeOrderGroup.Generator, new BigInteger(hash, true, true) % _primeOrderGroup.SubgroupSize, _primeOrderGroup.Prime);
    }
}

