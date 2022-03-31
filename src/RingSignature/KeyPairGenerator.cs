using System.Numerics;

namespace RingSignature;

public class KeyPairGenerator
{
    private readonly PrimeOrderGroup _primeOrderGroup;
    private readonly IRandom _random;

    public KeyPairGenerator(PrimeOrderGroup primeOrderGroup, IRandom random)
    {
        _primeOrderGroup = primeOrderGroup;
        _random = random;
    }

    public (BigInteger privateKey, BigInteger publicKey) CreateKeyPair()
    {
        BigInteger privateKey = _random.GetRandomNumber(_primeOrderGroup.SubgroupSize);
        BigInteger publicKey = BigInteger.ModPow(_primeOrderGroup.Generator, privateKey, _primeOrderGroup.Prime);

        return (privateKey, publicKey);
    }
}