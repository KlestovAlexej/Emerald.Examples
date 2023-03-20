# Скрипт создания сертификатов клиента для HTTPS и электронной подписи.

$password = ConvertTo-SecureString 'password' -AsPlainText -Force

$certServerRoot = New-SelfSignedCertificate -Type Custom -KeySpec Signature -Subject "CN=RootEmeraldExamplesIntegratorHttpsOrganization" -KeyExportPolicy Exportable -HashAlgorithm sha256 -KeyLength 2048 -CertStoreLocation "Cert:\CurrentUser\My" -KeyUsageProperty Sign -KeyUsage CertSign -NotAfter (Get-Date).AddYears(20)
$certServerRoot | Export-PfxCertificate -FilePath "C:\Dev\Emerald.Examples\Integrator\root.emerald.examples.integrator.https.organization.pfx" -Password $password
$certServerRoot | Export-Certificate -Type cer -FilePath "C:\Dev\Emerald.Examples\Integrator\root.emerald.examples.integrator.https.organization.cer" -Force 

$certServer = New-SelfSignedCertificate -Type Custom -KeySpec Signature -Subject "CN=EmeraldExamplesIntegratorHttpsOrganization" -KeyExportPolicy Exportable -HashAlgorithm sha256 -KeyLength 2048 -CertStoreLocation "Cert:\CurrentUser\My" -KeyUsageProperty Sign -KeyUsage DigitalSignature, KeyEncipherment, DataEncipherment -Signer $certServerRoot -NotAfter (Get-Date).AddYears(20)
$certServer | Export-PfxCertificate -FilePath "C:\Dev\Emerald.Examples\Integrator\emerald.examples.integrator.https.organization.pfx" -Password $password
$certServer | Export-Certificate -Type cer -FilePath "C:\Dev\Emerald.Examples\Integrator\emerald.examples.integrator.https.organization.cer" -Force 

$certServerRoot = New-SelfSignedCertificate -Type Custom -KeySpec Signature -Subject "CN=RootEmeraldExamplesIntegratorSignatureOrganization" -KeyExportPolicy Exportable -HashAlgorithm sha256 -KeyLength 2048 -CertStoreLocation "Cert:\CurrentUser\My" -KeyUsageProperty Sign -KeyUsage CertSign -NotAfter (Get-Date).AddYears(20)
$certServerRoot | Export-PfxCertificate -FilePath "C:\Dev\Emerald.Examples\Integrator\root.emerald.examples.integrator.signature.organization.pfx" -Password $password
$certServerRoot | Export-Certificate -Type cer -FilePath "C:\Dev\Emerald.Examples\Integrator\root.emerald.examples.integrator.signature.organization.cer" -Force 

$certServer = New-SelfSignedCertificate -Type Custom -KeySpec Signature -Subject "CN=EmeraldExamplesIntegratorSignatureOrganization" -KeyExportPolicy Exportable -HashAlgorithm sha256 -KeyLength 2048 -CertStoreLocation "Cert:\CurrentUser\My" -KeyUsageProperty Sign -KeyUsage DigitalSignature -Signer $certServerRoot -NotAfter (Get-Date).AddYears(20)
$certServer | Export-PfxCertificate -FilePath "C:\Dev\Emerald.Examples\Integrator\emerald.examples.integrator.signature.organization.pfx" -Password $password
$certServer | Export-Certificate -Type cer -FilePath "C:\Dev\Emerald.Examples\Integrator\emerald.examples.integrator.signature.organization.cer" -Force 
