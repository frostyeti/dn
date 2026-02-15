using FrostYeti.Colors;

namespace FrostYeti.Colors.Tests;

public class ColorExtensionsTests
{
    [Fact]
    public void To256Color_Pure_Red_Returns_Correct_Index()
    {
        var color = new Rgb(255, 0, 0);
        var index = color.To256Color();
        Assert.True(index >= 16 && index < 232);
    }

    [Fact]
    public void To256Color_Pure_Green_Returns_Correct_Index()
    {
        var color = new Rgb(0, 255, 0);
        var index = color.To256Color();
        Assert.True(index >= 16 && index < 232);
    }

    [Fact]
    public void To256Color_Pure_Blue_Returns_Correct_Index()
    {
        var color = new Rgb(0, 0, 255);
        var index = color.To256Color();
        Assert.True(index >= 16 && index < 232);
    }

    [Fact]
    public void To256Color_Black_Returns_Grayscale_Index()
    {
        var color = new Rgb(0, 0, 0);
        var index = color.To256Color();
        Assert.Equal(232, index);
    }

    [Fact]
    public void To256Color_White_Returns_Grayscale_Index()
    {
        var color = new Rgb(255, 255, 255);
        var index = color.To256Color();
        Assert.Equal(255, index);
    }

    [Fact]
    public void To256Color_Grayscale_Returns_Grayscale_Range()
    {
        var color = new Rgb(128, 128, 128);
        var index = color.To256Color();
        Assert.True(index >= 232 && index <= 255);
    }

    [Fact]
    public void To256Color_MidGray_Returns_Correct_Index()
    {
        var color = new Rgb(128, 128, 128);
        var index = color.To256Color();
        Assert.Equal(244, index);
    }

    [Fact]
    public void To256Color_DarkGray_Returns_Lower_Grayscale_Index()
    {
        var color = new Rgb(64, 64, 64);
        var index = color.To256Color();
        Assert.True(index >= 232 && index < 244);
    }

    [Fact]
    public void To256Color_LightGray_Returns_Higher_Grayscale_Index()
    {
        var color = new Rgb(192, 192, 192);
        var index = color.To256Color();
        Assert.True(index > 244 && index <= 255);
    }

    [Fact]
    public void To256Color_Yellow_Returns_Correct_Index()
    {
        var color = new Rgb(255, 255, 0);
        var index = color.To256Color();
        Assert.True(index >= 16 && index < 232);
    }

    [Fact]
    public void To256Color_Cyan_Returns_Correct_Index()
    {
        var color = new Rgb(0, 255, 255);
        var index = color.To256Color();
        Assert.True(index >= 16 && index < 232);
    }

    [Fact]
    public void To256Color_Magenta_Returns_Correct_Index()
    {
        var color = new Rgb(255, 0, 255);
        var index = color.To256Color();
        Assert.True(index >= 16 && index < 232);
    }

    [Fact]
    public void To256Color_Similar_Colors_Return_Same_Index()
    {
        var color1 = new Rgb(255, 0, 0);
        var color2 = new Rgb(250, 5, 5);
        var index1 = color1.To256Color();
        var index2 = color2.To256Color();
        Assert.Equal(index1, index2);
    }
}