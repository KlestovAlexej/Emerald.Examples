using System;
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
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using ShtrihM.Emerald.Integrator.Api.Common.Dtos.Tokens;
using System.Security.Cryptography;

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
    /// Приватный сертификат клиента для HTTPS.
    /// </summary>
    private X509Certificate2 m_certificateHttps;

    /// <summary>
    /// Приватный сертификат клиента для создания электронной подписи.
    /// </summary>
    private X509Certificate2 m_certificateSignature;

    /// <summary>
    /// Публичный корневой сертификат сервера для HTTPS.
    /// </summary>
    private X509Certificate2 m_rootServerCertificateHttps;

    [SetUp]
    public void SetUp()
    {
        var certificateHttpsBytes = File.ReadAllBytes(@"emerald.examples.integrator.https.organization.pfx");
        m_certificateHttps = new X509Certificate2(certificateHttpsBytes, "password");
    
        var certificateSignatureBytes = File.ReadAllBytes(@"emerald.examples.integrator.signature.organization.pfx");
        m_certificateSignature = new X509Certificate2(certificateSignatureBytes, "password");

        var rootServerCertificateHttpsBytes = File.ReadAllBytes(@"root.emerald.integrator.server.cer");
        m_rootServerCertificateHttps = new X509Certificate2(rootServerCertificateHttpsBytes);
    }

    /// <summary>
    /// <see cref="HttpClient"/> создаётся вручную.
    /// Получить описание сервера.
    /// </summary>
    [Test]
    public async Task Example_HttpClient_Manual()
    {
        var handler =
            new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    (_, _, chain, _) =>
                    {
                        foreach (var element in chain.ChainElements)
                        {
                            if (element.Certificate.Thumbprint == m_certificateSignature.Thumbprint)
                            {
                                return true;
                            }
                        }

                        return false;
                    },
            };

        handler.ClientCertificates.Add(m_certificateHttps);

        var httpClient =
            new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseAddress),
            };

        var restClient =
            new RestClient(
                httpClient,
                disposeHttpClient: true,
                configureSerialization: s => s.UseNewtonsoftJson(JsonExtensions.CreateSettings()));

        using var client = new Client(restClient, true);
        var description = await client.GetDescriptionAsync();

        Assert.IsNotNull(description);
        Console.WriteLine(description.ToJsonText(true));
    }

    /// <summary>
    /// <see cref="HttpClient"/> создаётся автоматически.
    /// Получить описание сервера.
    /// </summary>
    [Test]
    public async Task Example_HttpClient_Auto()
    {
        using var client = new Client(BaseAddress, m_certificateHttps);
        var description = await client.GetDescriptionAsync();

        Assert.IsNotNull(description);
        Console.WriteLine(description.ToJsonText(true));
    }

    /// <summary>
    /// Получить описание сервера.
    /// </summary>
    [Test]
    public async Task Example_GetDescriptionAsync()
    {
        using var client = new Client(BaseAddress, m_certificateHttps);
        var description = await client.GetDescriptionAsync();

        Assert.IsNotNull(description);
        Console.WriteLine(description.ToJsonText(true));
    }

    /// <summary>
    /// Отправить документ организаци - создание услуги токена (банковская карта) - билет с ограниченным сроком действия.
    /// </summary>
    [Test]
    public async Task Example_AddDocumentAsync_DocumentTokenBankCardCreateTicketTimeLimited()
    {
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

        using var client = new Client(BaseAddress, m_certificateHttps);
        var documentResult = await client.AddDocumentAsync(document, m_certificateSignature);

        Assert.IsNotNull(documentResult);
        Console.WriteLine(documentResult.ToJsonText(true));
    }

    /// <summary>
    /// Отправить документ организаци - создание услуги токена (банковская карта) - билет с ограниченным сроком действия и ограниченным числом поездок.
    /// </summary>
    [Test]
    public async Task Example_AddDocumentAsync_DocumentTokenBankCardCreateTicketTimeLimitedTravelsLimited()
    {
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

        using var client = new Client(BaseAddress, m_certificateHttps);
        var documentResult = await client.AddDocumentAsync(document, m_certificateSignature);

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
            new CmsSigner(SubjectIdentifierType.SubjectKeyIdentifier, m_certificateSignature)
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

        using var client = new Client(BaseAddress, m_certificateHttps);
        var documentResult = await client.AddDocumentAsync(documentMessage);

        Assert.IsNotNull(documentResult);
        Console.WriteLine(documentResult.ToJsonText(true));
    }

    /// <summary>
    /// Отправить документ организаци.
    /// Электронная подпись создаётся автоматически.
    /// </summary>
    [Test]
    public async Task Example_AddDocumentAsync_Auto_Signature()
    {
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

        using var client = new Client(BaseAddress, m_certificateHttps);
        var documentResult = await client.AddDocumentAsync(document, m_certificateSignature);

        Assert.IsNotNull(documentResult);
        Console.WriteLine(documentResult.ToJsonText(true));
    }

    /// <summary>
    /// Проверить существование PAN банковской карты.
    /// </summary>
    [Test]
    public async Task Example_TokenBankCardExistsAsync()
    {
        using var client = new Client(BaseAddress, m_certificateHttps);
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
