# PowerShell script to deploy and test CORS configuration
# Make sure you have Azure CLI installed and are logged in

Write-Host "🚀 Starting CORS configuration deployment and testing..." -ForegroundColor Green

# Step 1: Build the project
Write-Host "📦 Building the project..." -ForegroundColor Yellow
dotnet build "EbooksPlatfor.Server/OnlineBookstore.csproj" --configuration Release

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Build successful!" -ForegroundColor Green

# Step 2: Publish to Azure
Write-Host "🌐 Publishing to Azure..." -ForegroundColor Yellow
dotnet publish "EbooksPlatfor.Server/OnlineBookstore.csproj" --configuration Release --output ./publish

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Publish failed!" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Publish successful!" -ForegroundColor Green

# Step 3: Test CORS configuration
Write-Host "🧪 Testing CORS configuration..." -ForegroundColor Yellow

# Test the health endpoint
try {
    $response = Invoke-RestMethod -Uri "https://onlinebookstore-backend-f4ejgsdudbghhkfz.australiaeast-01.azurewebsites.net/api/health" -Method Get
    Write-Host "✅ Health endpoint test successful: $($response.Status)" -ForegroundColor Green
} catch {
    Write-Host "❌ Health endpoint test failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test CORS headers
try {
    $response = Invoke-WebRequest -Uri "https://onlinebookstore-backend-f4ejgsdudbghhkfz.australiaeast-01.azurewebsites.net/api/books" -Method Options
    $corsHeaders = $response.Headers["Access-Control-Allow-Origin"]
    
    if ($corsHeaders) {
        Write-Host "✅ CORS headers found: $corsHeaders" -ForegroundColor Green
    } else {
        Write-Host "⚠️  CORS headers not found in response" -ForegroundColor Yellow
    }
} catch {
    Write-Host "❌ CORS headers test failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "🎯 Deployment and testing completed!" -ForegroundColor Green
Write-Host "📝 Next steps:" -ForegroundColor Cyan
Write-Host "   1. Check your Azure App Service logs for any errors" -ForegroundColor White
Write-Host "   2. Test your frontend application" -ForegroundColor White
Write-Host "   3. Use the test-cors.html file to verify CORS is working" -ForegroundColor White
