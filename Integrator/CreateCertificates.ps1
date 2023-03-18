# Скрипт создания сертификатов клиента для HTTPS и электронной подписи.

$password = ConvertTo-SecureString 'password' -AsPlainText -Force

$certDeviceRoot = New-SelfSignedCertificate -Type Custom -KeySpec Signature -Subject "CN=EmeraldExamplesIntegratorHttpsOrganization" -KeyExportPolicy Exportable -HashAlgorithm sha256 -KeyLength 2048 -CertStoreLocation "Cert:\CurrentUser\My" -KeyUsageProperty Sign -KeyUsage DigitalSignature, KeyEncipherment, DataEncipherment -NotAfter (Get-Date).AddYears(20)
$certDeviceRoot | Export-PfxCertificate -FilePath "C:\Dev\Emerald.Examples\Integrator\emerald.examples.integrator.https.organization.pfx" -Password $password
$certDeviceRoot | Export-Certificate -Type cer -FilePath "C:\Dev\Emerald.Examples\Integrator\emerald.examples.integrator.https.organization.cer" -Force 

$certServerRoot = New-SelfSignedCertificate -Type Custom -Subject "CN=EmeraldExamplesIntegratorSignatureOrganization" -KeyExportPolicy Exportable -HashAlgorithm sha256  -KeyUsage DigitalSignature -KeyAlgorithm ECDSA_nistP192 -CurveExport CurveName -CertStoreLocation "Cert:\CurrentUser\My" -NotAfter (Get-Date).AddYears(20)
$certServerRoot | Export-PfxCertificate -FilePath "C:\Dev\Emerald.Examples\Integrator\emerald.examples.integrator.signature.organization.pfx" -Password $password
$certServerRoot | Export-Certificate -Type cer -FilePath "C:\Dev\Emerald.Examples\Integrator\emerald.examples.integrator.signature.organization.cer" -Force 
