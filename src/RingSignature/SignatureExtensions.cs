using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RingSignature
{
    public static class SignatureExtensions
    {
        public static int Size(this Signature signature)
        {
            int size = 0;

            size += signature.InitialChallenge.GetByteCount(true);
            size += signature.YTilda.GetByteCount(true);
            size += signature.Nonces.Sum(nonce => nonce.GetByteCount(true));

            return size;
        }
    }
}
