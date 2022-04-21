using System.Numerics;

namespace RingSignature;

/// <summary>
///     Creates public/private key pairs based on the given <see cref="PrimeOrderGroup"/>.
///     Public key generation is based on modular exponention.
/// </summary>
public class KeyPairGenerator
{
    private readonly PrimeOrderGroup _primeOrderGroup;
    private readonly IRandom _random;

    /// <summary>
    ///     Creates an instance of <see cref="KeyPairGenerator"/>.
    /// </summary>
    /// <param name="primeOrderGroup">The <see cref="PrimeOrderGroup"/> to use for key generation.</param>
    /// <param name="random">The <see cref="IRandom"/> random number generator to use for key generation.</param>
    public KeyPairGenerator(PrimeOrderGroup primeOrderGroup, IRandom random)
    {
        _primeOrderGroup = primeOrderGroup;
        _random = random;
    }

    /// <summary>
    ///     Creates a public/private key pair.
    /// </summary>
    /// <returns>The public key and associated private key.</returns>
    public (BigInteger privateKey, BigInteger publicKey) CreateKeyPair()
    {
        BigInteger privateKey = _random.GetRandomNumber(_primeOrderGroup.SubgroupSize);
        BigInteger publicKey = BigInteger.ModPow(_primeOrderGroup.Generator, privateKey, _primeOrderGroup.Prime);

        return (privateKey, publicKey);
    }
}