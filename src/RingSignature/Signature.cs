using System.Numerics;

namespace RingSignature;

public record Signature(
    BigInteger InitialChallenge, 
    BigInteger[] Nonces, 
    BigInteger YTilda);

