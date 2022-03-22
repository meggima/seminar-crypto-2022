using System.Globalization;
using System.Numerics;

namespace RingSignature;

public class PrimeOrderGroup
{
    private PrimeOrderGroup(BigInteger prime, BigInteger generator, BigInteger subgroupSize)
    {
        Prime = prime;
        Generator = generator;
        SubgroupSize = subgroupSize;
    }

    public BigInteger Prime { get; }

    public BigInteger Generator { get; }

    public BigInteger SubgroupSize { get; }

    public static PrimeOrderGroup FromHexParameters(string primeHex, string generatorHex, string subgroupSizeHex)
    {
        BigInteger prime = BigInteger.Parse(primeHex.Replace(" ", string.Empty), NumberStyles.AllowHexSpecifier);
        BigInteger generator = BigInteger.Parse(generatorHex.Replace(" ", string.Empty), NumberStyles.AllowHexSpecifier);
        BigInteger subgroupSize = BigInteger.Parse(subgroupSizeHex.Replace(" ", string.Empty), NumberStyles.AllowHexSpecifier);

        return new PrimeOrderGroup(prime, generator, subgroupSize);
    }
}

