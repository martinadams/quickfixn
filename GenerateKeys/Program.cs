using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

if (args.Length == 0)
{
    Console.WriteLine("First parameter must be the certificate destination path.");
    Console.WriteLine("(The path should be whatever <reporoot>/Examples is on your system)");
    return;
}

string path = args[0];

string caCertificatePath = Path.Join(path, "QuickFixn-TestCA.cer");
string serverPfxCertificatePath = Path.Join(path, "QuickFixn-TestServer.pfx");
string clientPfxCertificatePath = Path.Join(path, "QuickFixn-TestClient.pfx");

const string pfxPassword = "qfnpass123";

var caCertificate = CreateCaCertificate();
Console.WriteLine($"Writing CACertificate: {caCertificatePath}");
File.WriteAllBytes(caCertificatePath, caCertificate.Export(X509ContentType.Cert));

var serverCertificate = CreateAppCertificate(caCertificate,
    "CN=QuickFixn-TestServer", new Oid("1.3.6.1.5.5.7.3.1"), [0, 0, 0, 0, 0, 0, 0, 1]);
Console.WriteLine($"Writing ServerCertificate: {serverPfxCertificatePath}");
File.WriteAllBytes(serverPfxCertificatePath, serverCertificate.Export(X509ContentType.Pfx, pfxPassword));

var clientCertificate = CreateAppCertificate(caCertificate,
    "CN=QuickFixn-TestClient", new Oid("1.3.6.1.5.5.7.3.2"), [0, 0, 0, 0, 0, 0, 0, 2]);
Console.WriteLine($"Writing ClientCertificate: {clientPfxCertificatePath}");
File.WriteAllBytes(clientPfxCertificatePath, clientCertificate.Export(X509ContentType.Pfx, pfxPassword));

return;


static X509Certificate2 CreateCaCertificate()
{
    using var rsa = RSA.Create();
    var request = new CertificateRequest("CN=QuickFixn-TestCA", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    request.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true));
    request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign | X509KeyUsageFlags.DigitalSignature, true));

    X509Certificate2 certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(30));
    return certificate;
}

static X509Certificate2 CreateAppCertificate(X509Certificate2 caCertificate, string subjectName, Oid oid, ReadOnlySpan<byte> serialNumber)
{
    using RSA rsa = RSA.Create();
    CertificateRequest request = new(subjectName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    request.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, true));
    request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment, true));

    SubjectAlternativeNameBuilder sanBuilder = new();
    sanBuilder.AddDnsName("localhost");
    sanBuilder.AddIpAddress(IPAddress.Loopback);
    request.CertificateExtensions.Add(sanBuilder.Build());

    OidCollection enhancedKeyUsages = new() { oid };
    request.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(enhancedKeyUsages, true));

    X509Certificate2 certificate = request.Create(caCertificate, DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(5), serialNumber);
    return certificate.CopyWithPrivateKey(rsa);
}
