using Xunit;

namespace FrostYeti.Colors.Tests;

public class ColorTests
{
    [Fact]
    public void Constructor_FromSystemDrawingColor_SetsCorrectValues()
    {
        var systemColor = System.Drawing.Color.FromArgb(255, 100, 150, 200);
        var color = new Color(systemColor);
        Assert.Equal(100, color.R);
        Assert.Equal(150, color.G);
        Assert.Equal(200, color.B);
        Assert.Equal(255, (byte)color.A);
    }

    [Fact]
    public void Constructor_FromSystemDrawingColor_WithTransparency_SetsCorrectValues()
    {
        var systemColor = System.Drawing.Color.FromArgb(128, 100, 150, 200);
        var color = new Color(systemColor);
        Assert.Equal(100, color.R);
        Assert.Equal(150, color.G);
        Assert.Equal(200, color.B);
        Assert.Equal((Alpha)128, color.A);
    }

    [Fact]
    public void DefaultConstructor_SetsExpectedValues()
    {
#pragma warning disable SA1129
        var color = new Color();
        Assert.Equal(0, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal((Alpha)255, color.A);
    }

    [Fact]
    public void Equals_SameValues_ReturnsTrue()
    {
        var color1 = new Color(100, 150, 200, 255);
        var color2 = new Color(100, 150, 200, 255);
        Assert.True(color1.Equals(color2));
        Assert.True(color1 == color2);
    }

    [Fact]
    public void Equals_DifferentValues_ReturnsFalse()
    {
        var color1 = new Color(100, 150, 200, 255);
        var color2 = new Color(100, 150, 201, 255);
        Assert.False(color1.Equals(color2));
        Assert.True(color1 != color2);
    }

    [Fact]
    public void Deconstruct_ThreeParameters_SetsCorrectValues()
    {
        var color = new Color(100, 150, 200);
        var (r, g, b) = color;
        Assert.Equal(100, r);
        Assert.Equal(150, g);
        Assert.Equal(200, b);
    }

    [Fact]
    public void Deconstruct_FourParameters_SetsCorrectValues()
    {
        var color = new Color(100, 150, 200, 128);
        var (r, g, b, a) = color;
        Assert.Equal(100, r);
        Assert.Equal(150, g);
        Assert.Equal(200, b);
        Assert.Equal(128, a);
    }

    [Fact]
    public void GetHashCode_SameValues_ReturnsSameHashCode()
    {
        var color1 = new Color(100, 150, 200, 255);
        var color2 = new Color(100, 150, 200, 255);
        Assert.Equal(color1.GetHashCode(), color2.GetHashCode());
    }

    [Fact]
    public void ImplicitOperator_FromRgb_ConvertsCorrectly()
    {
        var rgb = new Rgb(100, 150, 200);
        Color color = rgb;
        Assert.Equal(100, color.R);
        Assert.Equal(150, color.G);
        Assert.Equal(200, color.B);
        Assert.Equal(Alpha.Opaque, color.A);
    }

    [Fact]
    public void ImplicitOperator_FromRgba_ConvertsCorrectly()
    {
        var rgba = new Rgba(100, 150, 200, (byte)128);
        Color color = rgba;
        Assert.Equal(100, color.R);
        Assert.Equal(150, color.G);
        Assert.Equal(200, color.B);
    }

    [Fact]
    public void ImplicitOperator_ToSystemDrawingColor_ConvertsCorrectly()
    {
        var color = new Color(100, 150, 200, 128);
        System.Drawing.Color systemColor = color;
        Assert.Equal(128, systemColor.A);
        Assert.Equal(100, systemColor.R);
        Assert.Equal(150, systemColor.G);
        Assert.Equal(200, systemColor.B);
    }
}