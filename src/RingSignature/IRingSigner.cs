using System.Numerics;

namespace RingSignature;

public interface IRingSigner
{
    /// <summary>
    ///     Signs a given <paramref name="message"/> using the given members (given by
    ///     their <paramref name="publicKeys"/>) for obfuscation and
    ///     the given <paramref name="signerPrivateKey"/>.
    /// </summary>
    /// <param name="message">The message to sign.</param>
    /// <param name="publicKeys">The public keys (including the one of the actual signer).</param>
    /// <param name="signerPrivateKey">The private key of the actual signer.</param>
    /// <param name="signerPublicKeyIndex">The index of the public key of the actual signer in <paramref name="publicKeys"/>.</param>
    /// <returns>The generated signature.</returns>
    Signature Sign(byte[] message, BigInteger[] publicKeys, BigInteger signerPrivateKey, int signerPublicKeyIndex);

    /// <summary>
    ///     Verifies whether the given signature is valid for the given <paramref name="message"/>.
    ///     This means one ring member (ring members given by their <paramref name="publicKeys"/>) signed the message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="signature">The signature to verify.</param>
    /// <param name="publicKeys">The possible signers.</param>
    /// <returns>True if the message was signed by one of the members, otherwise false.</returns>
    bool Verify(byte[] message, Signature signature, BigInteger[] publicKeys);

    /// <summary>
    ///     Checks whether the two signatures were created by the same ring member.
    /// </summary>
    /// <param name="signature1">The first signature.</param>
    /// <param name="signature2">The second signature.</param>
    /// <returns>True if the signature was created by the same signer, otherwise false.</returns>
    bool SignedBySameSigner(Signature signature1, Signature signature2);
}
