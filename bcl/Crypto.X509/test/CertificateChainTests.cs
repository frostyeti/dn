using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace FrostYeti.Crypto.X509.Tests;

public class CertificateChainTests
{
    [Fact]
    public void Create_SelfSigned_Root_CA()
    {
        using var rootCa = new CertificateRequestBuilder()
            .WithSubject("CN=Test Root CA, O=Test Organization, C=US")
            .AsCertificateAuthority()
            .WithPathLengthConstraint(1, critical: true)
            .WithKeyUsage(X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign)
            .WithEnhancedKeyUsages(EnhancedKeyUsageOids.ServerAuthentication)
            .WithRsa(4096)
            .WithDateRange(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(20))
            .BuildCertificate();

        Assert.NotNull(rootCa);
        Assert.Equal("CN=Test Root CA, O=Test Organization, C=US", rootCa.Subject);
        Assert.Equal("CN=Test Root CA, O=Test Organization, C=US", rootCa.Issuer);
        Assert.True(rootCa.HasPrivateKey);

        var basicConstraints = rootCa.Extensions.OfType<X509BasicConstraintsExtension>().FirstOrDefault();
        Assert.NotNull(basicConstraints);
        Assert.True(basicConstraints.CertificateAuthority);
        Assert.True(basicConstraints.HasPathLengthConstraint);
        Assert.Equal(1, basicConstraints.PathLengthConstraint);

        var keyUsage = rootCa.Extensions.OfType<X509KeyUsageExtension>().FirstOrDefault();
        Assert.NotNull(keyUsage);
        Assert.True(keyUsage.KeyUsages.HasFlag(X509KeyUsageFlags.KeyCertSign));
        Assert.True(keyUsage.KeyUsages.HasFlag(X509KeyUsageFlags.CrlSign));
    }

    [Fact]
    public void Create_Intermediate_CA_Signed_By_Root()
    {
        using var rootCa = CreateRootCa();

        using var intermediateCa = new CertificateRequestBuilder()
            .WithSubject("CN=Test Intermediate CA, O=Test Organization, C=US")
            .WithIssuer(rootCa)
            .AsCertificateAuthority()
            .WithPathLengthConstraint(0, critical: true)
            .WithKeyUsage(X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign)
            .WithRsa(4096)
            .WithDateRange(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(10))
            .BuildCertificate();

        Assert.NotNull(intermediateCa);
        Assert.Equal("CN=Test Intermediate CA, O=Test Organization, C=US", intermediateCa.Subject);
        Assert.Equal("CN=Test Root CA, O=Test Organization, C=US", intermediateCa.Issuer);
        Assert.True(intermediateCa.HasPrivateKey);

        var basicConstraints = intermediateCa.Extensions.OfType<X509BasicConstraintsExtension>().FirstOrDefault();
        Assert.NotNull(basicConstraints);
        Assert.True(basicConstraints.CertificateAuthority);
        Assert.True(basicConstraints.HasPathLengthConstraint);
        Assert.Equal(0, basicConstraints.PathLengthConstraint);

        var authorityKeyIdentifier = intermediateCa.Extensions
            .FirstOrDefault(e => e.Oid?.Value == Oids.AuthorityKeyIdentifier);
        Assert.NotNull(authorityKeyIdentifier);
    }

    [Fact]
    public void Create_Server_Certificate_Signed_By_Intermediate()
    {
        using var rootCa = CreateRootCa();
        using var intermediateCa = CreateIntermediateCa(rootCa);

        using var serverCert = new CertificateRequestBuilder()
            .WithSubject("CN=test.example.com, O=Test Organization, C=US")
            .WithIssuer(intermediateCa)
            .WithDnsNames("test.example.com", "www.example.com", "localhost")
            .WithIpAddresses(IPAddress.Parse("127.0.0.1"), IPAddress.Parse("::1"))
            .WithKeyUsage(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment)
            .WithEnhancedKeyUsages(EnhancedKeyUsageOids.ServerAuthentication, EnhancedKeyUsageOids.ClientAuthentication)
            .WithRsa(2048)
            .WithDateRange(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(2))
            .BuildCertificate();

        Assert.NotNull(serverCert);
        Assert.Equal("CN=test.example.com, O=Test Organization, C=US", serverCert.Subject);
        Assert.Equal("CN=Test Intermediate CA, O=Test Organization, C=US", serverCert.Issuer);
        Assert.True(serverCert.HasPrivateKey);

        var basicConstraints = serverCert.Extensions.OfType<X509BasicConstraintsExtension>().FirstOrDefault();
        Assert.NotNull(basicConstraints);
        Assert.False(basicConstraints.CertificateAuthority);

        var keyUsage = serverCert.Extensions.OfType<X509KeyUsageExtension>().FirstOrDefault();
        Assert.NotNull(keyUsage);
        Assert.True(keyUsage.KeyUsages.HasFlag(X509KeyUsageFlags.DigitalSignature));
        Assert.True(keyUsage.KeyUsages.HasFlag(X509KeyUsageFlags.KeyEncipherment));

        var eku = serverCert.Extensions.OfType<X509EnhancedKeyUsageExtension>().FirstOrDefault();
        Assert.NotNull(eku);
        var oids = eku.EnhancedKeyUsages;
        Assert.True(oids.OfType<Oid>().Any(o => o.Value == Oids.ServerAuthentication), "ServerAuthentication EKU not found");
        Assert.True(oids.OfType<Oid>().Any(o => o.Value == Oids.ClientAuthentication), "ClientAuthentication EKU not found");

        var san = serverCert.Extensions.OfType<X509SubjectAlternativeNameExtension>().FirstOrDefault();
        Assert.NotNull(san);
    }

    [Fact]
    public void Create_Device_Certificate_Signed_By_Intermediate()
    {
        using var rootCa = CreateRootCa();
        using var intermediateCa = CreateIntermediateCa(rootCa);

        using var deviceCert = new CertificateRequestBuilder()
            .WithSubject("CN=device-001, OU=Devices, O=Test Organization, C=US")
            .WithIssuer(intermediateCa)
            .WithDnsNames("device-001.local")
            .WithKeyUsage(X509KeyUsageFlags.DigitalSignature)
            .WithEnhancedKeyUsages(EnhancedKeyUsageOids.ClientAuthentication)
            .WithRsa(2048)
            .WithDateRange(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(5))
            .BuildCertificate();

        Assert.NotNull(deviceCert);
        Assert.Equal("CN=device-001, OU=Devices, O=Test Organization, C=US", deviceCert.Subject);
        Assert.Equal("CN=Test Intermediate CA, O=Test Organization, C=US", deviceCert.Issuer);
        Assert.True(deviceCert.HasPrivateKey);

        var basicConstraints = deviceCert.Extensions.OfType<X509BasicConstraintsExtension>().FirstOrDefault();
        Assert.NotNull(basicConstraints);
        Assert.False(basicConstraints.CertificateAuthority);

        var eku = deviceCert.Extensions.OfType<X509EnhancedKeyUsageExtension>().FirstOrDefault();
        Assert.NotNull(eku);
        var oids = eku.EnhancedKeyUsages;
        Assert.True(oids.OfType<Oid>().Any(o => o.Value == Oids.ClientAuthentication), "ClientAuthentication EKU not found");
    }

    [Fact]
    public void Verify_Certificate_Chain()
    {
        using var rootCa = CreateRootCa();
        using var intermediateCa = CreateIntermediateCa(rootCa);
        using var serverCert = CreateServerCert(intermediateCa);

        using var chain = new X509Chain();
        chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
        chain.ChainPolicy.ExtraStore.Add(rootCa);

        var isValid = chain.Build(serverCert);
        Assert.True(isValid, $"Chain validation failed: {string.Join(", ", chain.ChainStatus.Select(s => s.Status))}");
    }

    [Fact]
    public void Create_Certificate_With_ECDSA()
    {
        using var rootCa = new CertificateRequestBuilder()
            .WithSubject("CN=ECDsa Root CA, O=Test Organization, C=US")
            .AsCertificateAuthority()
            .WithKeyUsage(X509KeyUsageFlags.KeyCertSign)
            .WithECDsa(256)
            .BuildCertificate();

        Assert.NotNull(rootCa);
        Assert.True(rootCa.HasPrivateKey);

        using var serverCert = new CertificateRequestBuilder()
            .WithSubject("CN=ecdsa.example.com")
            .WithIssuer(rootCa)
            .WithDnsNames("ecdsa.example.com")
            .WithKeyUsage(X509KeyUsageFlags.DigitalSignature)
            .WithEnhancedKeyUsages(EnhancedKeyUsageOids.ServerAuthentication)
            .WithECDsa(256)
            .BuildCertificate();

        Assert.NotNull(serverCert);
        Assert.True(serverCert.HasPrivateKey);
    }

    [Fact]
    public void Create_Certificate_With_Email_And_UPN()
    {
        using var rootCa = CreateRootCa();

        using var userCert = new CertificateRequestBuilder()
            .WithSubject("CN=Test User, E=test@example.com")
            .WithIssuer(rootCa)
            .WithEmails("test@example.com")
            .WithUserPrincipalNames("test@example.com")
            .WithKeyUsage(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment)
            .WithEnhancedKeyUsages(EnhancedKeyUsageOids.ClientAuthentication, EnhancedKeyUsageOids.SecureEmail)
            .WithRsa(2048)
            .BuildCertificate();

        Assert.NotNull(userCert);

        var san = userCert.Extensions.OfType<X509SubjectAlternativeNameExtension>().FirstOrDefault();
        Assert.NotNull(san);
    }

    [Fact]
    public void Create_Certificate_With_Custom_Serial()
    {
        using var rootCa = CreateRootCa();

        var serial = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };
        using var cert = new CertificateRequestBuilder()
            .WithSubject("CN=Serial Test")
            .WithIssuer(rootCa)
            .WithSerialNumber(serial)
            .WithRsa(2048)
            .BuildCertificate();

        Assert.NotNull(cert);
        Assert.StartsWith("0102030405060708", cert.SerialNumber, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Builder_Reset_Clears_State()
    {
        var builder = new CertificateRequestBuilder()
            .WithSubject("CN=Test")
            .WithKeyUsage(X509KeyUsageFlags.DigitalSignature)
            .WithEnhancedKeyUsages(EnhancedKeyUsageOids.ServerAuthentication)
            .AsCertificateAuthority();

        builder.Reset();

        builder.WithSubject("CN=Reset Test");
        using var cert = builder.BuildCertificate();

        var basicConstraints = cert.Extensions.OfType<X509BasicConstraintsExtension>().FirstOrDefault();
        Assert.NotNull(basicConstraints);
        Assert.False(basicConstraints.CertificateAuthority);
    }

    [Fact]
    public void Build_Throws_When_Subject_Not_Set()
    {
        var builder = new CertificateRequestBuilder();

        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }

    [Fact]
    public void WithRsa_Throws_For_Invalid_KeySize()
    {
        var builder = new CertificateRequestBuilder();

        Assert.Throws<ArgumentOutOfRangeException>(() => builder.WithRsa(1024));
    }

    [Fact]
    public void WithECDsa_Throws_For_Invalid_KeySize()
    {
        var builder = new CertificateRequestBuilder();

        Assert.Throws<ArgumentOutOfRangeException>(() => builder.WithECDsa(128));
    }

    [Fact]
    public void WithSerialNumber_Throws_For_Negative_Value()
    {
        var builder = new CertificateRequestBuilder();

        Assert.Throws<ArgumentOutOfRangeException>(() => builder.WithSerialNumber(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => builder.WithSerialNumber((int)-1));
    }

    [Fact]
    public void BasicConstraints_Is_Critical_For_CA_Certificate()
    {
        using var ca = new CertificateRequestBuilder()
            .WithSubject("CN=Test CA")
            .AsCertificateAuthority(critical: true)
            .WithKeyUsage(X509KeyUsageFlags.KeyCertSign, critical: true)
            .WithRsa(2048)
            .BuildCertificate();

        var basicConstraints = ca.Extensions.OfType<X509BasicConstraintsExtension>().FirstOrDefault();
        Assert.NotNull(basicConstraints);
        Assert.True(basicConstraints.Critical);
    }

    [Fact]
    public void KeyUsage_Can_Be_Set_Critical()
    {
        using var cert = new CertificateRequestBuilder()
            .WithSubject("CN=Test Cert")
            .WithKeyUsage(X509KeyUsageFlags.DigitalSignature, critical: true)
            .WithRsa(2048)
            .BuildCertificate();

        var keyUsage = cert.Extensions.OfType<X509KeyUsageExtension>().FirstOrDefault();
        Assert.NotNull(keyUsage);
        Assert.True(keyUsage.Critical);
    }

    [Fact]
    public void EnhancedKeyUsage_Can_Be_Set_Critical()
    {
        using var cert = new CertificateRequestBuilder()
            .WithSubject("CN=Test Server")
            .WithEnhancedKeyUsages(critical: true, EnhancedKeyUsageOids.ServerAuthentication)
            .WithRsa(2048)
            .BuildCertificate();

        var eku = cert.Extensions.OfType<X509EnhancedKeyUsageExtension>().FirstOrDefault();
        Assert.NotNull(eku);
        Assert.True(eku.Critical);
    }

    [Fact]
    public void Non_CA_Certificate_Has_Non_Critical_BasicConstraints_By_Default()
    {
        using var cert = new CertificateRequestBuilder()
            .WithSubject("CN=Test End Entity")
            .WithKeyUsage(X509KeyUsageFlags.DigitalSignature)
            .WithRsa(2048)
            .BuildCertificate();

        var basicConstraints = cert.Extensions.OfType<X509BasicConstraintsExtension>().FirstOrDefault();
        Assert.NotNull(basicConstraints);
        Assert.False(basicConstraints.Critical);
        Assert.False(basicConstraints.CertificateAuthority);
    }

    private static X509Certificate2 CreateRootCa()
    {
        return new CertificateRequestBuilder()
            .WithSubject("CN=Test Root CA, O=Test Organization, C=US")
            .AsCertificateAuthority(critical: true)
            .WithPathLengthConstraint(1, critical: true)
            .WithKeyUsage(X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign, critical: true)
            .WithRsa(4096)
            .WithDateRange(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(20))
            .BuildCertificate();
    }

    private static X509Certificate2 CreateIntermediateCa(X509Certificate2 rootCa)
    {
        return new CertificateRequestBuilder()
            .WithSubject("CN=Test Intermediate CA, O=Test Organization, C=US")
            .WithIssuer(rootCa)
            .AsCertificateAuthority(critical: true)
            .WithPathLengthConstraint(0, critical: true)
            .WithKeyUsage(X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign, critical: true)
            .WithRsa(4096)
            .WithDateRange(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(10))
            .BuildCertificate();
    }

    private static X509Certificate2 CreateServerCert(X509Certificate2 issuer)
    {
        return new CertificateRequestBuilder()
            .WithSubject("CN=test.example.com, O=Test Organization, C=US")
            .WithIssuer(issuer)
            .WithDnsNames("test.example.com", "www.example.com")
            .WithKeyUsage(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment)
            .WithEnhancedKeyUsages(EnhancedKeyUsageOids.ServerAuthentication)
            .WithRsa(2048)
            .WithDateRange(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(2))
            .BuildCertificate();
    }
}