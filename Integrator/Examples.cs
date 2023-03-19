using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using NUnit.Framework;
using ShtrihM.Emerald.Integrator.Api.Clients;
using ShtrihM.Emerald.Integrator.Api.Common.Dtos.Documents;
using ShtrihM.Emerald.Integrator.Api.Common;
using ShtrihM.Wattle3.Testing;
using ShtrihM.Wattle3.Json.Extensions;
using System.Security.Cryptography.Pkcs;
using System.Text;
using ShtrihM.Emerald.Integrator.Api.Common.Dtos.Tokens;

namespace ShtrihM.Emerald.Examples.Integrator;

/// <summary>
/// Примеры использования API интеграции внешних организаций.
/// </summary>
[TestFixture]
public class Examples
{
    /// <summary>
    /// Базовый URL API облачного транспорта.
    /// </summary>
    private static readonly string BaseAddress = $"https://localhost:{Common.Constants.DefaultPortHttpsApiIntegrator}";

    /// <summary>
    /// Получить описание сервера.
    /// <see cref="HttpClient"/> создаётся автоматически.
    /// </summary>
    [Test]
    public async Task Example_GetDescriptionAsync_Auto_HttpClient()
    {
        var certificateBytes = await File.ReadAllBytesAsync(@"emerald.examples.integrator.https.organization.pfx");
        var certificate = new X509Certificate2(certificateBytes, "password");

        var client = new Client(BaseAddress, certificate);
        var description = await client.GetDescriptionAsync();

        Assert.IsNotNull(description);
        Console.WriteLine(description.ToJsonText(true));
    }

    /// <summary>
    /// Получить описание сервера.
    /// <see cref="HttpClient"/> создаётся вручную.
    /// </summary>
    [Test]
    public async Task Example_GetDescriptionAsync_Manual_HttpClient()
    {
        var handler =
            new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
            };

        var certificateBytes = await File.ReadAllBytesAsync(@"emerald.examples.integrator.https.organization.pfx");
        var certificate = new X509Certificate2(certificateBytes, "password");
        handler.ClientCertificates.Add(certificate!);

        var httpClient =
            new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseAddress),
            };

        var client = new Client(httpClient);
        var description = await client.GetDescriptionAsync();

        Assert.IsNotNull(description);
        Console.WriteLine(description.ToJsonText(true));
    }

    /// <summary>
    /// Отправить документ организаци - создание услуги токена (банковская карта) - билет с ограниченным сроком действия.
    /// Электронная подпись создаётся автоматически.
    /// </summary>
    [Test]
    public async Task Example_AddDocumentAsync_Auto_Signature_DocumentTokenBankCardCreateTicketTimeLimited()
    {
        var certificateHttpsBytes = await File.ReadAllBytesAsync(@"emerald.examples.integrator.https.organization.pfx");
        var certificateHttps = new X509Certificate2(certificateHttpsBytes, "password");
        var client = new Client(BaseAddress, certificateHttps);
        var certificateSignatureBytes = await File.ReadAllBytesAsync(@"emerald.examples.integrator.signature.organization.pfx");
        var certificateSignature = new X509Certificate2(certificateSignatureBytes, "password");

        var document =
            new DocumentTokenBankCardCreateTicketTimeLimited
            {
                CreateDate = DateTimeOffset.Now,
                DateBegin = DateTime.Now.Date,
                DateEnd = DateTime.Now.Date.AddDays(30),
                Key = Guid.NewGuid(),
                PanHash = ProviderRandomValues.GetBytes(FieldsConstants.Sha256Length),
                Type = 1,
            };
        var documentResult = await client.AddDocumentAsync(document, certificateSignature);

        Assert.IsNotNull(documentResult);
        Console.WriteLine(documentResult.ToJsonText(true));
    }

    /// <summary>
    /// Отправить документ организаци - создание услуги токена (банковская карта) - билет с ограниченным сроком действия и ограниченным числом поездок.
    /// Электронная подпись создаётся автоматически.
    /// </summary>
    [Test]
    public async Task Example_AddDocumentAsync_Auto_Signature_DocumentTokenBankCardCreateTicketTimeLimitedTravelsLimited()
    {
        var certificateHttpsBytes = await File.ReadAllBytesAsync(@"emerald.examples.integrator.https.organization.pfx");
        var certificateHttps = new X509Certificate2(certificateHttpsBytes, "password");
        var client = new Client(BaseAddress, certificateHttps);
        var certificateSignatureBytes = await File.ReadAllBytesAsync(@"emerald.examples.integrator.signature.organization.pfx");
        var certificateSignature = new X509Certificate2(certificateSignatureBytes, "password");

        var document =
            new DocumentTokenBankCardCreateTicketTimeLimitedTravelsLimited
            {
                CreateDate = DateTimeOffset.Now,
                DateBegin = DateTime.Now.Date,
                DateEnd = DateTime.Now.Date.AddDays(30),
                Key = Guid.NewGuid(),
                PanHash = ProviderRandomValues.GetBytes(FieldsConstants.Sha256Length),
                Type = 1,
                Count = 12,
            };
        var documentResult = await client.AddDocumentAsync(document, certificateSignature);

        Assert.IsNotNull(documentResult);
        Console.WriteLine(documentResult.ToJsonText(true));
    }

    /// <summary>
    /// Отправить документ организаци.
    /// Электронная подпись создаётся вручную.
    /// </summary>
    [Test]
    public async Task Example_AddDocumentAsync_Mnual_Signature()
    {
        var certificateHttpsBytes = await File.ReadAllBytesAsync(@"emerald.examples.integrator.https.organization.pfx");
        var certificateHttps = new X509Certificate2(certificateHttpsBytes, "password");
        var client = new Client(BaseAddress, certificateHttps);
        var certificateSignatureBytes = await File.ReadAllBytesAsync(@"emerald.examples.integrator.signature.organization.pfx");
        var certificateSignature = new X509Certificate2(certificateSignatureBytes, "password");

        var document =
            new DocumentTokenBankCardCreateTicketTimeLimited
            {
                CreateDate = DateTimeOffset.Now,
                DateBegin = DateTime.Now.Date,
                DateEnd = DateTime.Now.Date.AddDays(30),
                Key = Guid.NewGuid(),
                PanHash = ProviderRandomValues.GetBytes(FieldsConstants.Sha256Length),
                Type = 1,
            };
        var documentText = document.ToJsonText(true);
        var documentBytes = Encoding.UTF8.GetBytes(documentText);
        var contentInfo = new ContentInfo(documentBytes);
        var signedCms = new SignedCms(contentInfo, false);
        var signer =
            new CmsSigner(SubjectIdentifierType.SubjectKeyIdentifier, certificateSignature)
            {
                IncludeOption = X509IncludeOption.EndCertOnly
            };
        signedCms.ComputeSignature(signer);
        var message = signedCms.Encode();
        var documentMessage =
            new DocumentMessage
            {
                Message = message,
            };
        var documentResult = await client.AddDocumentAsync(documentMessage);

        Assert.IsNotNull(documentResult);
        Console.WriteLine(documentResult.ToJsonText(true));
    }

    /// <summary>
    /// Проверить существование PAN банковской карты.
    /// </summary>
    [Test]
    public async Task Example_TokenBankCardExistsAsync()
    {
        var certificateBytes = await File.ReadAllBytesAsync(@"emerald.examples.integrator.https.organization.pfx");
        var certificate = new X509Certificate2(certificateBytes, "password");

        var client = new Client(BaseAddress, certificate);
        var existsResult =
            await client.TokenBankCardExistsAsync(
                new BankCardPanInfo
                {
                    PanHash = new byte[FieldsConstants.Sha256Length],
                });
        Assert.IsNotNull(existsResult);
        Console.WriteLine(existsResult.ToJsonText(true));
    }
}
