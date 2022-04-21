using System.Numerics;
using System.Security.Cryptography;

namespace RingSignature;

public class Random : IRandom {
    private readonly RandomNumberGenerator _randomNumberGenerator;

    public Random()
    {
        _randomNumberGenerator = RandomNumberGenerator.Create();
    }

    public BigInteger GetRandomNumber(BigInteger max)
    {
        byte[] bytes = new byte[max.GetByteCount(true)];

        _randomNumberGenerator.GetBytes(bytes);

        return new BigInteger(bytes, true, true);
    }

    public void Fill(byte[] destination)
    {
        _randomNumberGenerator.GetBytes(destination);
    }
}
