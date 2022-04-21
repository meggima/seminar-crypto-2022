using System.Globalization;
using System.Numerics;

namespace RingSignature;

/// <summary>
///     Represents a group of prime order based on a generator.
/// </summary>
public class PrimeOrderGroup
{
    /// <summary>
    ///     Creates an instance of a <see cref="PrimeOrderGroup"/> using the given parameters.
    /// </summary>
    /// <param name="prime">The prime p of the group.</param>
    /// <param name="generator">The generator g of the group.</param>
    /// <param name="subgroupSize">The size of the subgroup induced by g.</param>
    private PrimeOrderGroup(BigInteger prime, BigInteger generator, BigInteger subgroupSize)
    {
        Prime = prime;
        Generator = generator;
        SubgroupSize = subgroupSize;
    }

    public BigInteger Prime { get; }

    public BigInteger Generator { get; }

    public BigInteger SubgroupSize { get; }

    /// <summary>
    ///     Initialized a <see cref="PrimeOrderGroup"/> with its parameters in HEX string form.
    /// </summary>
    /// <param name="primeHex">The prime in HEX.</param>
    /// <param name="generatorHex">The generator in HEX.</param>
    /// <param name="subgroupSizeHex">The subgroup size in HEX.</param>
    /// <returns></returns>
    public static PrimeOrderGroup FromHexParameters(string primeHex, string generatorHex, string subgroupSizeHex)
    {
        BigInteger prime = BigInteger.Parse(primeHex.Replace(" ", string.Empty), NumberStyles.AllowHexSpecifier);
        BigInteger generator = BigInteger.Parse(generatorHex.Replace(" ", string.Empty), NumberStyles.AllowHexSpecifier);
        BigInteger subgroupSize = BigInteger.Parse(subgroupSizeHex.Replace(" ", string.Empty), NumberStyles.AllowHexSpecifier);

        return new PrimeOrderGroup(prime, generator, subgroupSize);
    }
}

