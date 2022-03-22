using System.Numerics;
using System.Security.Cryptography;

namespace RingSignature;

public static class BigIntegerRandom {

    public static BigInteger GetRandomBigInteger(BigInteger max)
    {
        byte[] bytes = new byte[max.GetByteCount(true)];

        RandomNumberGenerator
            .Create()
            .GetBytes(bytes);

        return new BigInteger(bytes, true, true);
    }
}

