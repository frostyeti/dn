namespace FrostYeti.Colors.Tests;

public class RgbaTests
{
    [Fact]
    public void DefaultConstructor_SetsBlackOpaque()
    {
#pragma warning disable SA1129
        var rgba = new Rgba();
        Assert.Equal(0, rgba.R);
        Assert.Equal(0, rgba.G);
        Assert.Equal(0, rgba.B);
    }

    [Fact]
    public void Constructor_WithUInt_SetsComponents()
    {
        var rgba = new Rgba(0x6496C880);
        Assert.Equal(0x64, rgba.R);
        Assert.Equal(0x96, rgba.G);
        Assert.Equal(0xC8, rgba.B);
    }

    [Fact]
    public void Constructor_WithIRgb_SetsComponentsOpaqueAlpha()
    {
        IRgb rgb = new Hexa(100, 150, 200, (byte)255);
        var rgba = new Rgba(rgb);
        Assert.Equal(100, rgba.R);
        Assert.Equal(150, rgba.G);
        Assert.Equal(200, rgba.B);
    }

    [Fact]
    public void Constructor_WithIRgba_SetsComponents()
    {
        IRgba source = new Hexa(100, 150, 200, (byte)128);
        var rgba = new Rgba(source);
        Assert.Equal(100, rgba.R);
        Assert.Equal(150, rgba.G);
        Assert.Equal(200, rgba.B);
    }

    [Fact]
    public void Constructor_WithThreeBytes_SetsComponentsOpaqueAlpha()
    {
        var rgba = new Rgba(100, 150, 200);
        Assert.Equal(100, rgba.R);
        Assert.Equal(150, rgba.G);
        Assert.Equal(200, rgba.B);
    }

    [Fact]
    public void Constructor_WithFourBytes_SetsComponents()
    {
        var rgba = new Rgba(100, 150, 200, (byte)128);
        Assert.Equal(100, rgba.R);
        Assert.Equal(150, rgba.G);
        Assert.Equal(200, rgba.B);
    }

    [Fact]
    public void Constructor_WithBytesAndAlpha_SetsComponents()
    {
        var rgba = new Rgba(100, 150, 200, new Alpha(0.5));
        Assert.Equal(100, rgba.R);
        Assert.Equal(150, rgba.G);
        Assert.Equal(200, rgba.B);
    }

    [Fact]
    public void Properties_ReturnCorrectValues()
    {
        var rgba = new Rgba(255, 128, 64, (byte)32);
        Assert.Equal(255, rgba.R);
        Assert.Equal(128, rgba.G);
        Assert.Equal(64, rgba.B);
    }

    [Fact]
    public void Deconstruct_ReturnsCorrectComponents()
    {
        var rgba = new Rgba(100, 150, 200, (byte)128);
        var (r, g, b, a) = rgba;
        Assert.Equal(100, r);
        Assert.Equal(150, g);
        Assert.Equal(200, b);
    }

    [Fact]
    public void Equals_Rgba_SameValues_ReturnsTrue()
    {
        var rgba1 = new Rgba(100, 150, 200, (byte)128);
        var rgba2 = new Rgba(100, 150, 200, (byte)128);
        Assert.True(rgba1.Equals(rgba2));
    }

    [Fact]
    public void Equals_Rgba_DifferentRgb_ReturnsFalse()
    {
        var rgba1 = new Rgba(100, 150, 200, (byte)128);
        var rgba2 = new Rgba(100, 150, 201, (byte)128);
        Assert.False(rgba1.Equals(rgba2));
    }

    [Fact]
    public void Equals_IRgba_Null_ReturnsFalse()
    {
        var rgba = new Rgba(100, 150, 200, (byte)128);
        Assert.False(rgba.Equals((IRgba?)null));
    }

    [Fact]
    public void Equals_Object_SameType_ReturnsTrue()
    {
        var rgba1 = new Rgba(100, 150, 200, (byte)128);
        object rgba2 = new Rgba(100, 150, 200, (byte)128);
        Assert.True(rgba1.Equals(rgba2));
    }

    [Fact]
    public void Equals_Object_DifferentType_ReturnsFalse()
    {
        var rgba = new Rgba(100, 150, 200, (byte)128);
        Assert.False(rgba.Equals("not a rgba"));
    }

    [Fact]
    public void Equals_Object_Null_ReturnsFalse()
    {
        var rgba = new Rgba(100, 150, 200, (byte)128);
        Assert.False(rgba.Equals(null));
    }

    [Fact]
    public void GetHashCode_SameValues_ReturnsSameHashCode()
    {
        var rgba1 = new Rgba(100, 150, 200, (byte)128);
        var rgba2 = new Rgba(100, 150, 200, (byte)128);
        Assert.Equal(rgba1.GetHashCode(), rgba2.GetHashCode());
    }
}