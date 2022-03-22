using System.Numerics;

namespace RingSignature;

public class KeyPairGenerator
{
    private readonly PrimeOrderGroup _primeOrderGroup;

    public KeyPairGenerator(PrimeOrderGroup primeOrderGroup)
    {
        _primeOrderGroup = primeOrderGroup;
    }

    public (BigInteger privateKey, BigInteger publicKey) CreateKeyPair()
    {
        BigInteger privateKey = BigIntegerRandom.GetRandomBigInteger(_primeOrderGroup.SubgroupSize);
        BigInteger publicKey = BigInteger.ModPow(_primeOrderGroup.Generator, privateKey, _primeOrderGroup.Prime);

        return (privateKey, publicKey);
    }
}