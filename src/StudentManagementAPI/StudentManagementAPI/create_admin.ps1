$body = @{
    email = "admin@portal.com"
    password = "Admin@123"
    role = "Admin"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "http://localhost:5178/api/Account/register" -Method Post -Body $body -ContentType "application/json"
    Write-Host "Success: $response" -ForegroundColor Green
    Write-Host "You can now login with: admin@portal.com / Admin@123" -ForegroundColor Cyan
} catch {
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
}
