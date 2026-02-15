using System.Runtime.InteropServices;

namespace FrostYeti.Secrets.OperatingSystem.Tests;

public class OsSecretsTests
{
    [Fact]
    public void IsOsSupported_ReturnsTrueOnSupportedPlatform()
    {
        Assert.True(OsSecrets.IsOsSupported);
    }

    [Fact]
    public void SetSecret_And_GetSecret_ReturnsCorrectValue()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
            !RuntimeInformation.IsOSPlatform(OSPlatform.Linux) &&
            !RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return;
        }

        var service = $"TestService_{Guid.NewGuid():N}";
        var account = "test@example.com";
        var secret = "my-secret-password";

        OsSecrets.SetSecret(service, account, secret);
        var retrieved = OsSecrets.GetSecret(service, account);

        Assert.Equal(secret, retrieved);

        OsSecrets.DeleteSecret(service, account);
    }

    [Fact]
    public void SetSecret_Bytes_And_GetSecretAsBytes_ReturnsCorrectValue()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
            !RuntimeInformation.IsOSPlatform(OSPlatform.Linux) &&
            !RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return;
        }

        var service = $"TestService_{Guid.NewGuid():N}";
        var account = "test@example.com";
        var secret = new byte[] { 1, 2, 3, 4, 5 };

        OsSecrets.SetSecret(service, account, secret);
        var retrieved = OsSecrets.GetSecretAsBytes(service, account);

        Assert.Equal(secret, retrieved);

        OsSecrets.DeleteSecret(service, account);
    }

    [Fact]
    public void GetSecret_ReturnsNull_WhenNotFound()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
            !RuntimeInformation.IsOSPlatform(OSPlatform.Linux) &&
            !RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return;
        }

        var service = $"NonExistent_{Guid.NewGuid():N}";
        var account = "nonexistent@example.com";

        var result = OsSecrets.GetSecret(service, account);

        Assert.Null(result);
    }

    [Fact]
    public void GetSecretAsSecureString_ReturnsSecureString()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
            !RuntimeInformation.IsOSPlatform(OSPlatform.Linux) &&
            !RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return;
        }

        var service = $"TestService_{Guid.NewGuid():N}";
        var account = "secure@example.com";
        var secret = "my-secure-password";

        OsSecrets.SetSecret(service, account, secret);
        using var secureString = OsSecrets.GetSecretAsSecureString(service, account);

        Assert.NotNull(secureString);
        Assert.True(secureString.Length > 0);

        OsSecrets.DeleteSecret(service, account);
    }

    [Fact]
    public void ListSecrets_ReturnsRecordsForService()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
            !RuntimeInformation.IsOSPlatform(OSPlatform.Linux) &&
            !RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return;
        }

        var service = $"ListTestService_{Guid.NewGuid():N}";
        var account1 = "user1@example.com";
        var account2 = "user2@example.com";
        var secret1 = "password1";
        var secret2 = "password2";

        OsSecrets.SetSecret(service, account1, secret1);
        OsSecrets.SetSecret(service, account2, secret2);

        var records = OsSecrets.ListSecrets(service);

        Assert.NotEmpty(records);
        Assert.Contains(records, r => r.Account == account1);
        Assert.Contains(records, r => r.Account == account2);

        OsSecrets.DeleteSecret(service, account1);
        OsSecrets.DeleteSecret(service, account2);
    }

    [Fact]
    public void ListSecrets_ReturnsEmptyList_WhenServiceNotFound()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
            !RuntimeInformation.IsOSPlatform(OSPlatform.Linux) &&
            !RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return;
        }

        var service = $"NonExistentService_{Guid.NewGuid():N}";

        var records = OsSecrets.ListSecrets(service);

        Assert.Empty(records);
    }

    [Fact]
    public void DeleteSecret_RemovesSecret()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
            !RuntimeInformation.IsOSPlatform(OSPlatform.Linux) &&
            !RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return;
        }

        var service = $"DeleteTestService_{Guid.NewGuid():N}";
        var account = "delete@example.com";
        var secret = "to-be-deleted";

        OsSecrets.SetSecret(service, account, secret);
        OsSecrets.DeleteSecret(service, account);
        var result = OsSecrets.GetSecret(service, account);

        Assert.Null(result);
    }
}