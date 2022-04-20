using System.Numerics;

namespace RingSignature;

public interface IRingSigner
{
    Signature Sign(byte[] message, BigInteger[] publicKeys, BigInteger signerPrivateKey, int signerPublicKeyIndex);

    bool Verify(byte[] message, Signature signature, BigInteger[] publicKeys);

    bool SignedBySameSigner(Signature signature1, Signature signature2);
}
