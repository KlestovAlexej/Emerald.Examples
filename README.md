# Emerald.Examples

Примеры использования API облачного транспорта.

Клиент API написан 100% на [C#](https://ru.wikipedia.org/wiki/C_Sharp) под [.NET 8](https://devblogs.microsoft.com/dotnet/announcing-dotnet-8/).

## Примеры использования API интеграции внешних организаций

Пакеты **nuget** клиента начинаются с префикса [ShtrihM.Emerald.Integrator.Api.Clients](https://www.nuget.org/packages?q=ShtrihM.Emerald.Integrator.Api.Clients)

Примеры в файле [Examples.cs](/Integrator/Examples.cs).

Все примеры оформлены как [NUnit](https://nunit.org/)-тесты для запуска в ОС Windows из-под [Visual Studio 2022](https://visualstudio.microsoft.com/ru/vs/) (проверено на версии 17.8.1).

Скрипт [PowerShell](https://learn.microsoft.com/ru-ru/powershell/) создания сертификатов клиента для HTTPS и электронной подписи в файле [CreateCertificates.ps1](/Integrator/CreateCertificates.ps1).
