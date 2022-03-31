using System.Numerics;

namespace RingSignature;

public interface IRandom
{
    BigInteger GetRandomNumber(BigInteger bytes);
    void Fill(byte[] hash1Key);
}
