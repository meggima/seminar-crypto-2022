using System.Numerics;

namespace RingSignature;

public static class ByteHelper
{
    public static byte[] ConcatBytes(IList<BigInteger> input)
    {
        long length = input.Select(x => x.GetByteCount(true)).Sum();

        byte[] bytes = new byte[length];

        int index = 0;

        foreach (BigInteger value in input)
        {
            value.TryWriteBytes(bytes.AsSpan(index), out int bytesWritten, true, true);
            index += bytesWritten;
        }

        return bytes;
    }

    public static byte[] ConcatBytes(IList<byte[]> publicKeysBytes, byte[][] components)
    {
        long length = publicKeysBytes
                    .Select(x => x.Length)
                    .Concat(components
                        .Select(x => x.Length))
                    .Sum();

        byte[] bytes = new byte[length];

        int index = 0;

        foreach (byte[] keyBytes in publicKeysBytes)
        {
            keyBytes.CopyTo(bytes, index);
            index += keyBytes.Length;
        }

        foreach (byte[] componentBytes in components)
        {
            componentBytes.CopyTo(bytes, index);
            index += componentBytes.Length;
        }

        return bytes;
    }
}

