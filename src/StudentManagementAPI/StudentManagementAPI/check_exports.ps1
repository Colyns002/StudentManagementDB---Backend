$asm = [System.Reflection.Assembly]::LoadFile('C:\Users\Administrator\.nuget\packages\microsoft.openapi\2.4.1\lib\net8.0\Microsoft.OpenApi.dll')
$asm.GetExportedTypes() | Select-Object FullName | Select -First 15
