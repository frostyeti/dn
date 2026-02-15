namespace FrostYeti.Colors.Tests;

public class HexTests
{
    [Fact]
    public void DefaultConstructor_SetsBlack()
    {
#pragma warning disable SA1129
        var hex = new Hex();
        Assert.Equal(0, hex.R);
        Assert.Equal(0, hex.G);
        Assert.Equal(0, hex.B);
    }

    [Fact]
    public void Constructor_WithUInt_SetsComponents()
    {
        var hex = new Hex(0x112233);
        Assert.Equal(0x11, hex.R);
        Assert.Equal(0x22, hex.G);
        Assert.Equal(0x33, hex.B);
    }

    [Fact]
    public void Constructor_WithBytes_SetsComponents()
    {
        var hex = new Hex(0x11, 0x22, 0x33);
        Assert.Equal(0x11, hex.R);
        Assert.Equal(0x22, hex.G);
        Assert.Equal(0x33, hex.B);
    }

    [Fact]
    public void Constructor_WithString_SetsComponents()
    {
        var hex = new Hex("112233");
        Assert.Equal(0x11, hex.R);
        Assert.Equal(0x22, hex.G);
        Assert.Equal(0x33, hex.B);
    }

    [Fact]
    public void Constructor_WithSpan_SetsComponents()
    {
        var hex = new Hex("112233".AsSpan());
        Assert.Equal(0x11, hex.R);
        Assert.Equal(0x22, hex.G);
        Assert.Equal(0x33, hex.B);
    }

    [Fact]
    public void Properties_ReturnCorrectValues()
    {
        var hex = new Hex(255, 128, 64);
        Assert.Equal(255, hex.R);
        Assert.Equal(128, hex.G);
        Assert.Equal(64, hex.B);
    }

    [Fact]
    public void Parse_String_SixDigits_ReturnsCorrectHex()
    {
        var hex = Hex.Parse("FF8844");
        Assert.Equal(255, hex.R);
        Assert.Equal(136, hex.G);
        Assert.Equal(68, hex.B);
    }

    [Fact]
    public void Parse_String_ThreeDigits_ReturnsCorrectHex()
    {
        var hex = Hex.Parse("F84");
        Assert.Equal(255, hex.R);
        Assert.Equal(136, hex.G);
        Assert.Equal(68, hex.B);
    }

    [Fact]
    public void Parse_String_WithHashPrefix_ReturnsCorrectHex()
    {
        var hex = Hex.Parse("#FF8844");
        Assert.Equal(255, hex.R);
        Assert.Equal(136, hex.G);
        Assert.Equal(68, hex.B);
    }

    [Fact]
    public void Parse_Span_SixDigits_ReturnsCorrectHex()
    {
        var hex = Hex.Parse("FF8844".AsSpan());
        Assert.Equal(255, hex.R);
        Assert.Equal(136, hex.G);
        Assert.Equal(68, hex.B);
    }

    [Fact]
    public void Parse_Span_ThreeDigits_ReturnsCorrectHex()
    {
        var hex = Hex.Parse("F84".AsSpan());
        Assert.Equal(255, hex.R);
        Assert.Equal(136, hex.G);
        Assert.Equal(68, hex.B);
    }

    [Fact]
    public void Parse_InvalidLength_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => Hex.Parse("12345"));
    }

    [Fact]
    public void Parse_Empty_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => Hex.Parse(string.Empty));
    }

    [Fact]
    public void ParseAsUint_ValidInput_ReturnsCorrectValue()
    {
        var value = Hex.ParseAsUint("FF8844");
        Assert.Equal(0xFF8844u, value);
    }

    [Fact]
    public void ParseAsUint_ThreeDigits_ReturnsCorrectValue()
    {
        var value = Hex.ParseAsUint("F84");
        Assert.Equal(0xFF8844u, value);
    }

    [Fact]
    public void TryParse_String_Valid_ReturnsTrue()
    {
        var success = Hex.TryParse("FF8844", out var hex);
        Assert.True(success);
        Assert.Equal(255, hex.R);
        Assert.Equal(136, hex.G);
        Assert.Equal(68, hex.B);
    }

    [Fact]
    public void TryParse_Span_Valid_ReturnsTrue()
    {
        var success = Hex.TryParse("FF8844".AsSpan(), out var hex);
        Assert.True(success);
        Assert.Equal(255, hex.R);
        Assert.Equal(136, hex.G);
        Assert.Equal(68, hex.B);
    }

    [Fact]
    public void TryParse_InvalidHex_ReturnsFalse()
    {
        var success = Hex.TryParse("ZZZZZZ", out var hex);
        Assert.False(success);
        Assert.Equal(default(Hex), hex);
    }

    [Fact]
    public void TryParse_InvalidLength_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => Hex.TryParse("12345", out _));
    }

    [Fact]
    public void Deconstruct_ReturnsCorrectComponents()
    {
        var hex = new Hex(100, 150, 200);
        var (r, g, b) = hex;
        Assert.Equal(100, r);
        Assert.Equal(150, g);
        Assert.Equal(200, b);
    }

    [Fact]
    public void ToString_ReturnsHexFormat()
    {
        var hex = new Hex(255, 136, 68);
        Assert.Equal("FF8844", hex.ToString());
    }

    [Fact]
    public void ToString_ZeroPad_ReturnsSixDigits()
    {
        var hex = new Hex(1, 2, 3);
        Assert.Equal("010203", hex.ToString());
    }
}