namespace FrostYeti.Colors.Tests;

public class RgbTests
{
    [Fact]
    public void DefaultConstructor_SetsAllZero()
    {
#pragma warning disable SA1129
        var rgb = new Rgb();
        Assert.Equal(0, rgb.R);
        Assert.Equal(0, rgb.G);
        Assert.Equal(0, rgb.B);
    }

    [Fact]
    public void Constructor_WithUInt_SetsComponents()
    {
        var rgb = new Rgb(0x112233);
        Assert.Equal(0x11, rgb.R);
        Assert.Equal(0x22, rgb.G);
        Assert.Equal(0x33, rgb.B);
    }

    [Fact]
    public void Constructor_WithBytes_SetsComponents()
    {
        var rgb = new Rgb(0x11, 0x22, 0x33);
        Assert.Equal(0x11, rgb.R);
        Assert.Equal(0x22, rgb.G);
        Assert.Equal(0x33, rgb.B);
    }

    [Fact]
    public void Properties_ReturnCorrectValues()
    {
        var rgb = new Rgb(255, 128, 64);
        Assert.Equal(255, rgb.R);
        Assert.Equal(128, rgb.G);
        Assert.Equal(64, rgb.B);
    }

    [Fact]
    public void ImplicitOperator_FromRgba_ConvertsCorrectly()
    {
        var rgba = new Rgba(100, 150, 200, (byte)128);
        Rgb rgb = rgba;
        Assert.Equal(100, rgb.R);
        Assert.Equal(150, rgb.G);
        Assert.Equal(200, rgb.B);
    }

    [Fact]
    public void ImplicitOperator_FromColor_ConvertsCorrectly()
    {
        var color = new Color(100, 150, 200, (byte)128);
        Rgb rgb = color;
        Assert.Equal(100, rgb.R);
        Assert.Equal(150, rgb.G);
        Assert.Equal(200, rgb.B);
    }

    [Fact]
    public void Deconstruct_ReturnsCorrectComponents()
    {
        var rgb = new Rgb(100, 150, 200);
        var (r, g, b) = rgb;
        Assert.Equal(100, r);
        Assert.Equal(150, g);
        Assert.Equal(200, b);
    }

    [Fact]
    public void Equals_SameValues_ReturnsTrue()
    {
        var rgb1 = new Rgb(100, 150, 200);
        var rgb2 = new Rgb(100, 150, 200);
        Assert.True(rgb1.Equals(rgb2));
    }

    [Fact]
    public void Equals_DifferentValues_ReturnsFalse()
    {
        var rgb1 = new Rgb(100, 150, 200);
        var rgb2 = new Rgb(100, 150, 201);
        Assert.False(rgb1.Equals(rgb2));
    }
}