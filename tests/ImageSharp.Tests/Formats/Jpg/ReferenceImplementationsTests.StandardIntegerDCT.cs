// ReSharper disable InconsistentNaming
namespace SixLabors.ImageSharp.Tests.Formats.Jpg
{
    using System;

    using SixLabors.ImageSharp.Formats.Jpeg.Common;
    using SixLabors.ImageSharp.Tests.Formats.Jpg.Utils;

    using Xunit;
    using Xunit.Abstractions;

    public partial class ReferenceImplementationsTests
    {
        public class StandardIntegerDCT : JpegUtilityTestFixture
        {
            public StandardIntegerDCT(ITestOutputHelper output)
                : base(output)
            {
            }

            [Theory(Skip = "Sandboxing only! (Incorrect reference implementation)")]
            [InlineData(42)]
            [InlineData(1)]
            [InlineData(2)]
            public void IDCT_IsEquivalentTo_AccurateImplementation(int seed)
            {
                int[] data = Create8x8RandomIntData(-1000, 1000, seed);

                Block8x8 source = default(Block8x8);
                source.LoadFrom(data);

                Block8x8 expected = ReferenceImplementations.AccurateDCT.TransformIDCT(ref source);
                Block8x8 actual = ReferenceImplementations.StandardIntegerDCT.TransformIDCT(ref source);

                Block8x8F sourceF = source.AsFloatBlock();
                Block8x8F wut0 = ReferenceImplementations.FastFloatingPointDCT.TransformIDCT(ref sourceF);
                Block8x8 wut1 = wut0.RoundAsInt16Block();

                long diff = Block8x8.TotalDifference(ref expected, ref actual);
                this.Output.WriteLine(expected.ToString());
                this.Output.WriteLine(actual.ToString());
                this.Output.WriteLine(wut1.ToString());
                this.Output.WriteLine("DIFFERENCE: "+diff);

                Assert.True(diff < 4);
            }

            [Theory]
            [InlineData(42)]
            [InlineData(1)]
            [InlineData(2)]
            public void FDCT_IsEquivalentTo_AccurateImplementation(int seed)
            {
                int[] data = Create8x8RandomIntData(-1000, 1000, seed);

                Block8x8F source = default(Block8x8F);
                source.LoadFrom(data);

                Block8x8F expected = ReferenceImplementations.AccurateDCT.TransformFDCT(ref source);

                source += 128;
                Block8x8 temp = source.RoundAsInt16Block();
                Block8x8 actual8 = ReferenceImplementations.StandardIntegerDCT.Subtract128_TransformFDCT_Upscale8(ref temp);
                Block8x8F actual = actual8.AsFloatBlock();
                actual /= 8;
                
                this.CompareBlocks(expected, actual, 1f);
            }


            [Theory]
            [InlineData(42, 0)]
            [InlineData(1, 0)]
            [InlineData(2, 0)]
            public void ForwardThenInverse(int seed, int startAt)
            {
                Span<int> original = JpegUtilityTestFixture.Create8x8RandomIntData(-200, 200, seed);

                Span<int> block = original.AddScalarToAllValues(128);

                ReferenceImplementations.StandardIntegerDCT.Subtract128_TransformFDCT_Upscale8_Inplace(block);

                for (int i = 0; i < 64; i++)
                {
                    block[i] /= 8;
                }

                ReferenceImplementations.StandardIntegerDCT.TransformIDCTInplace(block);

                for (int i = startAt; i < 64; i++)
                {
                    float expected = original[i];
                    float actual = (float)block[i];

                    Assert.Equal(expected, actual, new ApproximateFloatComparer(3f));
                }
            }
        }
    }
}