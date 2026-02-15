namespace FrostYeti.Colors.Tests;

public class HexaTests
{
    [Fact]
    public void DefaultConstructor_SetsBlackOpaque()
    {
#pragma warning disable SA1129
        var hexa = new Hexa();
        Assert.Equal(0, hexa.R);
        Assert.Equal(0, hexa.G);
        Assert.Equal(0, hexa.B);
        Assert.Equal(255, hexa.A);
    }

    [Fact]
    public void Constructor_WithUInt_SetsComponents()
    {
        var hexa = new Hexa(0x11223344);
        Assert.Equal(0x11, hexa.R);
        Assert.Equal(0x22, hexa.G);
        Assert.Equal(0x33, hexa.B);
        Assert.Equal(0x44, hexa.A);
    }

    [Fact]
    public void Constructor_WithBytesAndByteAlpha_SetsComponents()
    {
        var hexa = new Hexa(0x11, 0x22, 0x33, (byte)0x44);
        Assert.Equal(0x11, hexa.R);
        Assert.Equal(0x22, hexa.G);
        Assert.Equal(0x33, hexa.B);
        Assert.Equal(0x44, hexa.A);
    }

    [Fact]
    public void Constructor_WithBytesAndAlpha_SetsComponents()
    {
        var hexa = new Hexa(0x11, 0x22, 0x33, new Alpha(0.5));
        Assert.Equal(0x11, hexa.R);
        Assert.Equal(0x22, hexa.G);
        Assert.Equal(0x33, hexa.B);
        Assert.Equal((byte)(0.5 * 255), hexa.A);
    }

    [Fact]
    public void Constructor_WithString_SetsComponents()
    {
        var hexa = new Hexa("11223344");
        Assert.Equal(0x11, hexa.R);
        Assert.Equal(0x22, hexa.G);
        Assert.Equal(0x33, hexa.B);
        Assert.Equal(0x44, hexa.A);
    }

    [Fact]
    public void Constructor_WithSpan_SetsComponents()
    {
        var hexa = new Hexa("11223344".AsSpan());
        Assert.Equal(0x11, hexa.R);
        Assert.Equal(0x22, hexa.G);
        Assert.Equal(0x33, hexa.B);
        Assert.Equal(0x44, hexa.A);
    }

    [Fact]
    public void Properties_ReturnCorrectValues()
    {
        var hexa = new Hexa(255, 128, 64, (byte)32);
        Assert.Equal(255, hexa.R);
        Assert.Equal(128, hexa.G);
        Assert.Equal(64, hexa.B);
        Assert.Equal(32, hexa.A);
    }

    [Fact]
    public void Parse_String_EightDigits_ReturnsCorrectHexa()
    {
        var hexa = Hexa.Parse("FF884480");
        Assert.Equal(255, hexa.R);
        Assert.Equal(136, hexa.G);
        Assert.Equal(68, hexa.B);
        Assert.Equal(128, hexa.A);
    }

    [Fact]
    public void Parse_String_SixDigits_ReturnsHexaWithOpaqueAlpha()
    {
        var hexa = Hexa.Parse("FF8844");
        Assert.Equal(255, hexa.R);
        Assert.Equal(136, hexa.G);
        Assert.Equal(68, hexa.B);
        Assert.Equal(255, hexa.A);
    }

    [Fact]
    public void Parse_String_ThreeDigits_ReturnsHexaWithOpaqueAlpha()
    {
        var hexa = Hexa.Parse("F84");
        Assert.Equal(255, hexa.R);
        Assert.Equal(136, hexa.G);
        Assert.Equal(68, hexa.B);
        Assert.Equal(255, hexa.A);
    }

    [Fact]
    public void Parse_String_WithHashPrefix_ReturnsCorrectHexa()
    {
        var hexa = Hexa.Parse("#FF884480");
        Assert.Equal(255, hexa.R);
        Assert.Equal(136, hexa.G);
        Assert.Equal(68, hexa.B);
        Assert.Equal(128, hexa.A);
    }

    [Fact]
    public void Parse_Span_EightDigits_ReturnsCorrectHexa()
    {
        var hexa = Hexa.Parse("FF884480".AsSpan());
        Assert.Equal(255, hexa.R);
        Assert.Equal(136, hexa.G);
        Assert.Equal(68, hexa.B);
        Assert.Equal(128, hexa.A);
    }

    [Fact]
    public void Parse_InvalidLength_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => Hexa.Parse("12345"));
    }

    [Fact]
    public void Parse_Empty_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => Hexa.Parse(string.Empty));
    }

    [Fact]
    public void TryParse_String_Valid_ReturnsTrue()
    {
        var success = Hexa.TryParse("FF884480", out var hexa);
        Assert.True(success);
        Assert.Equal(255, hexa.R);
        Assert.Equal(136, hexa.G);
        Assert.Equal(68, hexa.B);
        Assert.Equal(128, hexa.A);
    }

    [Fact]
    public void TryParse_Span_Valid_ReturnsTrue()
    {
        var success = Hexa.TryParse("FF884480".AsSpan(), out var hexa);
        Assert.True(success);
        Assert.Equal(255, hexa.R);
        Assert.Equal(136, hexa.G);
        Assert.Equal(68, hexa.B);
        Assert.Equal(128, hexa.A);
    }

    [Fact]
    public void TryParse_InvalidHex_ReturnsFalse()
    {
        var success = Hexa.TryParse("ZZZZZZZZ", out var hexa);
        Assert.False(success);
        Assert.Equal(default(Hexa), hexa);
    }

    [Fact]
    public void TryParse_InvalidLength_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => Hexa.TryParse("12345", out _));
    }

    [Fact]
    public void Deconstruct_ThreeParameters_ReturnsRgb()
    {
        var hexa = new Hexa(100, 150, 200, (byte)128);
        var (r, g, b) = hexa;
        Assert.Equal(100, r);
        Assert.Equal(150, g);
        Assert.Equal(200, b);
    }

    [Fact]
    public void Deconstruct_FourBytes_ReturnsCorrectComponents()
    {
        var hexa = new Hexa(100, 150, 200, (byte)128);
        var (r, g, b, a) = hexa;
        Assert.Equal(100, r);
        Assert.Equal(150, g);
        Assert.Equal(200, b);
        Assert.Equal(128, a);
    }

    [Fact]
    public void Equals_Hexa_SameValues_ReturnsTrue()
    {
        var hexa1 = new Hexa(100, 150, 200, (byte)128);
        var hexa2 = new Hexa(100, 150, 200, (byte)128);
        Assert.True(hexa1.Equals(hexa2));
    }

    [Fact]
    public void Equals_Hexa_DifferentValues_ReturnsFalse()
    {
        var hexa1 = new Hexa(100, 150, 200, (byte)128);
        var hexa2 = new Hexa(100, 150, 200, (byte)129);
        Assert.False(hexa1.Equals(hexa2));
    }

    [Fact]
    public void Equals_IRgb_SameValues_ReturnsTrue()
    {
        var hexa = new Hexa(100, 150, 200, (byte)128);
        IRgb? rgb = new Hexa(100, 150, 200, (byte)255);
        Assert.True(hexa.Equals(rgb));
    }

    [Fact]
    public void Equals_IRgb_DifferentValues_ReturnsFalse()
    {
        var hexa = new Hexa(100, 150, 200, (byte)128);
        IRgb? rgb = new Hexa(100, 150, 201, (byte)255);
        Assert.False(hexa.Equals(rgb));
    }

    [Fact]
    public void Equals_IRgb_Null_ReturnsFalse()
    {
        var hexa = new Hexa(100, 150, 200, (byte)128);
        Assert.False(hexa.Equals((IRgb?)null));
    }

    [Fact]
    public void Equals_IRgba_SameValues_ReturnsTrue()
    {
        var hexa = new Hexa(100, 150, 200, (byte)128);
        IRgba? rgba = new Hexa(100, 150, 200, (byte)128);
        Assert.True(((IEquatable<IRgba>)hexa).Equals(rgba));
    }

    [Fact]
    public void Equals_IRgba_DifferentValues_ReturnsFalse()
    {
        var hexa = new Hexa(100, 150, 200, (byte)128);
        IRgba? rgba = new Hexa(100, 150, 200, (byte)129);
        Assert.False(((IEquatable<IRgba>)hexa).Equals(rgba));
    }

    [Fact]
    public void Equals_IRgba_Null_ReturnsFalse()
    {
        var hexa = new Hexa(100, 150, 200, (byte)128);
        Assert.False(((IEquatable<IRgba>)hexa).Equals(null));
    }

    [Fact]
    public void ToString_ReturnsHexFormat()
    {
        var hexa = new Hexa(255, 136, 68, (byte)128);
        Assert.Equal("FF884480", hexa.ToString());
    }

    [Fact]
    public void ToString_ZeroPad_ReturnsEightDigits()
    {
        var hexa = new Hexa(1, 2, 3, (byte)4);
        Assert.Equal("01020304", hexa.ToString());
    }
}