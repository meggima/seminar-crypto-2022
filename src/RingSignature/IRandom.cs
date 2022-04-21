using System.Numerics;

namespace RingSignature;

public interface IRandom
{
    BigInteger GetRandomNumber(BigInteger max);
    void Fill(byte[] destination);
}
