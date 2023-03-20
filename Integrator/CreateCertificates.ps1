# Скрипт создания сертификатов клиента для HTTPS и электронной подписи.

$password = ConvertTo-SecureString 'password' -AsPlainText -Force

# Корневой сертфикат HTTPS клиента.

$certServerRoot = New-SelfSignedCertificate -Type Custom -KeySpec Signature -Subject "CN=RootEmeraldExamplesIntegratorHttpsOrganization" -KeyExportPolicy Exportable -HashAlgorithm sha256 -KeyLength 2048 -CertStoreLocation "Cert:\CurrentUser\My" -KeyUsageProperty Sign -KeyUsage CertSign -NotAfter (Get-Date).AddYears(20)
$certServerRoot | Export-PfxCertificate -FilePath "C:\Dev\Emerald.Examples\Integrator\root.emerald.examples.integrator.https.organization.pfx" -Password $password
$certServerRoot | Export-Certificate -Type cer -FilePath "C:\Dev\Emerald.Examples\Integrator\root.emerald.examples.integrator.https.organization.cer" -Force 

# Сертфикат HTTPS клиента, подписанный корневым сертфикатом HTTPS клиента.
#
# Сервер требует - RSA с длинной ключа не менее 2048 бит.
#

$certServer = New-SelfSignedCertificate -Type Custom -KeySpec Signature -Subject "CN=EmeraldExamplesIntegratorHttpsOrganization" -KeyExportPolicy Exportable -HashAlgorithm sha256 -KeyLength 2048 -CertStoreLocation "Cert:\CurrentUser\My" -KeyUsageProperty Sign -KeyUsage DigitalSignature, KeyEncipherment, DataEncipherment -Signer $certServerRoot -NotAfter (Get-Date).AddYears(20)
$certServer | Export-PfxCertificate -FilePath "C:\Dev\Emerald.Examples\Integrator\emerald.examples.integrator.https.organization.pfx" -Password $password
$certServer | Export-Certificate -Type cer -FilePath "C:\Dev\Emerald.Examples\Integrator\emerald.examples.integrator.https.organization.cer" -Force 

# Корневой сертфикат электронной подписи клиента.

$certServerRoot = New-SelfSignedCertificate -Type Custom -KeySpec Signature -Subject "CN=RootEmeraldExamplesIntegratorSignatureOrganization" -KeyExportPolicy Exportable -HashAlgorithm sha256 -KeyLength 2048 -CertStoreLocation "Cert:\CurrentUser\My" -KeyUsageProperty Sign -KeyUsage CertSign -NotAfter (Get-Date).AddYears(20)
$certServerRoot | Export-PfxCertificate -FilePath "C:\Dev\Emerald.Examples\Integrator\root.emerald.examples.integrator.signature.organization.pfx" -Password $password
$certServerRoot | Export-Certificate -Type cer -FilePath "C:\Dev\Emerald.Examples\Integrator\root.emerald.examples.integrator.signature.organization.cer" -Force 

# Сертфикат электронной подписи клиента, подписанный корневым сертфикатом электронной подписи клиента.
#
# Сервер требует - RSA с длинной ключа не менее 2048 бит и алгоритмом хэш-функции SHA-256.
#

$certServer = New-SelfSignedCertificate -Type Custom -KeySpec Signature -Subject "CN=EmeraldExamplesIntegratorSignatureOrganization" -KeyExportPolicy Exportable -HashAlgorithm sha256 -KeyLength 2048 -CertStoreLocation "Cert:\CurrentUser\My" -KeyUsageProperty Sign -KeyUsage DigitalSignature -Signer $certServerRoot -NotAfter (Get-Date).AddYears(20)
$certServer | Export-PfxCertificate -FilePath "C:\Dev\Emerald.Examples\Integrator\emerald.examples.integrator.signature.organization.pfx" -Password $password
$certServer | Export-Certificate -Type cer -FilePath "C:\Dev\Emerald.Examples\Integrator\emerald.examples.integrator.signature.organization.cer" -Force 
