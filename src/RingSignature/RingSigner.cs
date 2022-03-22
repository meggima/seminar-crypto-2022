﻿using System.Numerics;
using System.Security.Cryptography;

namespace RingSignature;

public class RingSigner
{
    private readonly HashAlgorithm _hash1Function;
    private readonly HashAlgorithm _hash2Function;
    private readonly PrimeOrderGroup _primeOrderGroup;

    public RingSigner(PrimeOrderGroup primeOrderGroup)
    {
        byte[] hash1Key = new byte[64];
        byte[] hash2Key = new byte[64];

        RandomNumberGenerator.Fill(hash1Key);
        RandomNumberGenerator.Fill(hash2Key);

        _hash1Function = new HMACSHA256(hash1Key);
        _hash2Function = new HMACSHA256(hash1Key);
        _primeOrderGroup = primeOrderGroup;
    }

    public Signature Sign(byte[] message, BigInteger[] publicKeys, BigInteger signerPrivateKey, int signerPublicKeyIndex)
    {
        BigInteger h = Hash2(publicKeys);

        BigInteger yTilde = BigInteger.ModPow(h, signerPrivateKey, _primeOrderGroup.Prime);
        byte[] yTildeBytes = yTilde.ToByteArray(true, true);

        BigInteger[] cVector = new BigInteger[publicKeys.Length];
        BigInteger[] sVector = new BigInteger[publicKeys.Length];

        BigInteger u = BigIntegerRandom.GetRandomBigInteger(_primeOrderGroup.SubgroupSize);

        IList<byte[]> publicKeysBytes = publicKeys.Select(p => p.ToByteArray(true, true)).ToList();

        cVector[(signerPublicKeyIndex + 1) % publicKeys.Length] = Hash1(
            publicKeysBytes,
            yTildeBytes,
            message,
            BigInteger.ModPow(_primeOrderGroup.Generator, u, _primeOrderGroup.Prime).ToByteArray(true, true),
            BigInteger.ModPow(h, u, _primeOrderGroup.Prime).ToByteArray(true, true));

        for (int i = (signerPublicKeyIndex + 1) % publicKeys.Length; i != signerPublicKeyIndex; i = (i + 1) % publicKeys.Length)
        {
            sVector[i] = BigIntegerRandom.GetRandomBigInteger(_primeOrderGroup.SubgroupSize);

            BigInteger v1 = (BigInteger.ModPow(_primeOrderGroup.Generator, sVector[i], _primeOrderGroup.Prime) * BigInteger.ModPow(publicKeys[i], cVector[i], _primeOrderGroup.Prime))
                % _primeOrderGroup.Prime;

            BigInteger v2 = (BigInteger.ModPow(h, sVector[i], _primeOrderGroup.Prime) * BigInteger.ModPow(yTilde, cVector[i], _primeOrderGroup.Prime))
                % _primeOrderGroup.Prime;

            cVector[(i + 1) % publicKeys.Length] = Hash1(
                publicKeysBytes,
                yTildeBytes,
                message,
                v1.ToByteArray(true, true),
                v2.ToByteArray(true, true));
        }

        BigInteger dividend = (u - signerPrivateKey * cVector[signerPublicKeyIndex]);

        sVector[signerPublicKeyIndex] = dividend >= 0 ? dividend % _primeOrderGroup.SubgroupSize : _primeOrderGroup.Prime + (dividend % _primeOrderGroup.SubgroupSize);

        return new Signature(cVector[0], sVector, yTilde);
    }

    public bool Verify(byte[] message, Signature signature, BigInteger[] publicKeys)
    {
        BigInteger h = Hash2(publicKeys);

        IList<byte[]> publicKeysBytes = publicKeys.Select(p => p.ToByteArray(true, true)).ToList();
        BigInteger yTilde = signature.Y;
        byte[] yTildeBytes = yTilde.ToByteArray(true, true);

        BigInteger[] zPrimeVector = new BigInteger[publicKeys.Length];
        BigInteger[] zPrimePrimeVector = new BigInteger[publicKeys.Length];
        BigInteger[] cVector = new BigInteger[publicKeys.Length];

        cVector[0] = signature.C;

        for (int i = 0; i < publicKeys.Length; i++)
        {
            zPrimeVector[i] = (BigInteger.ModPow(_primeOrderGroup.Generator, signature.S[i], _primeOrderGroup.Prime) * BigInteger.ModPow(publicKeys[i], cVector[i], _primeOrderGroup.Prime))
                % _primeOrderGroup.Prime;

            zPrimePrimeVector[i] = (BigInteger.ModPow(h, signature.S[i], _primeOrderGroup.Prime) * BigInteger.ModPow(yTilde, cVector[i], _primeOrderGroup.Prime))
                % _primeOrderGroup.Prime;

            if (i < publicKeys.Length - 1)
            {
                cVector[i + 1] = Hash1(
                    publicKeysBytes,
                    yTildeBytes,
                    message,
                    zPrimeVector[i].ToByteArray(true, true),
                    zPrimePrimeVector[i].ToByteArray(true, true));
            }
        }

        BigInteger hashed = Hash1(
                    publicKeysBytes,
                    yTildeBytes,
                    message,
                    zPrimeVector[publicKeys.Length - 1].ToByteArray(true, true),
                    zPrimePrimeVector[publicKeys.Length - 1].ToByteArray(true, true));

        return signature.C == hashed;
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
