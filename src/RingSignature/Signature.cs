using System.Numerics;

namespace RingSignature;

public record Signature(BigInteger C, BigInteger[] S, BigInteger Y);

