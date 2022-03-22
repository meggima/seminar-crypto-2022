using FluentAssertions;
using System.Numerics;
using Xunit;

namespace RingSignature.Tests
{
    public class ByteHelperTests
    {
        [Fact]
        public void ConcatBytes_ShouldConcatBytesOfIntegers()
        {
            // Arrange
            BigInteger int1 = BigInteger.One;       //           0000 0001
            BigInteger int2 = new BigInteger(2);    //           0000 0010
            BigInteger int3 = new BigInteger(8);    //           0000 0100
            BigInteger int4 = new BigInteger(1024); // 0000 0100 0000 0000

            IList<BigInteger> ints = new[] { int1, int2, int3, int4 };

            // Act
            byte[] bytes = ByteHelper.ConcatBytes(ints);

            // Assert
            bytes.Should().HaveCount(5);
            bytes[0].Should().Be(0x01);
            bytes[1].Should().Be(0x02);
            bytes[2].Should().Be(0x08);
            bytes[3].Should().Be(0x04);
            bytes[4].Should().Be(0x00);
        }

        [Fact]
        public void ConcatBytes_ShouldConcatBytes()
        {
            // Arrange
            byte[][] publicKeyBytes = new byte[][]
            {
                new byte[]{ 0x01, 0x02, 0x03, 0x04 },
                new byte[]{ 0x05, 0x06 },
                new byte[]{ 0x07, 0x08, 0x09 }
            };

            byte[][] components = new byte[][]
            {
                new byte[] { 0xA, 0xB, 0xC, 0xD },
                new byte[] { 0xE, 0xF }
            };

            // Act
            byte[] bytes = ByteHelper.ConcatBytes(publicKeyBytes, components);

            // Assert
            bytes.Should().ContainInOrder(new byte[] {
                0x01, 0x02, 0x03, 0x04,
                0x05, 0x06, 0x07, 0x08,
                0x09, 0x0A, 0x0B, 0x0C,
                0x0D, 0x0E, 0x0F });
        }
    }
}
