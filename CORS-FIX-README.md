# CORS Configuration Fix for Ebooks Platform

## Problem Description
The frontend application deployed to Azure Static Web Apps was experiencing CORS (Cross-Origin Resource Sharing) errors when trying to access the backend API. The error message was:

```
Access to fetch at 'https://onlinebookstore-backend-f4ejgsdudbghhkfz.australiaeast-01.azurewebsites.net/api/books' 
from origin 'https://happy-smoke-0d1f54100.2.azurestaticapps.net' has been blocked by CORS policy: 
Response to preflight request doesn't pass access control check: No 'Access-Control-Allow-Origin' header is present on the requested resource.
```

## Root Cause
The backend CORS configuration only allowed localhost origins for development, but didn't include the production frontend URL.

## Solution Applied

### 1. Updated Backend CORS Configuration
Modified `EbooksPlatfor.Server/Program.cs` to include the production frontend URL:

```csharp
// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var allowedOrigins = new[]
        {
            "https://localhost:52441",
            "http://localhost:52441",
            "https://happy-smoke-0d1f54100.2.azurestaticapps.net"
        };
        
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetIsOriginAllowedToAllowWildcardSubdomain();
    });
});
```

### 2. Enhanced Security Headers
Added additional security headers to improve the overall security posture:

```csharp
// Add security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    await next();
});
```

### 3. Improved Frontend Error Handling
Enhanced the API service in `ebooksplatfor.client/src/services/api.ts` to:
- Add retry logic for CORS errors
- Include proper CORS mode and credentials
- Provide better error messages for CORS issues

### 4. Added Health Check Endpoints
Added health check endpoints for easier debugging:

```csharp
// Health check endpoints
app.MapGet("/health", () => new { Status = "Healthy", Timestamp = DateTime.UtcNow, Environment = app.Environment.EnvironmentName });
app.MapGet("/api/health", () => new { Status = "Healthy", Timestamp = DateTime.UtcNow, Environment = app.Environment.EnvironmentName });
```

## Files Modified
- `EbooksPlatfor.Server/Program.cs` - CORS configuration and security headers
- `ebooksplatfor.client/src/services/api.ts` - Enhanced error handling and retry logic
- `EbooksPlatfor.Server/wwwroot/test-cors.html` - CORS testing tool
- `deploy-and-test.ps1` - Deployment and testing script

## Deployment Steps

### Option 1: Using the PowerShell Script
1. Ensure you have Azure CLI installed and are logged in
2. Run the deployment script:
   ```powershell
   .\deploy-and-test.ps1
   ```

### Option 2: Manual Deployment
1. Build the project:
   ```bash
   dotnet build "EbooksPlatfor.Server/OnlineBookstore.csproj" --configuration Release
   ```

2. Publish to Azure:
   ```bash
   dotnet publish "EbooksPlatfor.Server/OnlineBookstore.csproj" --configuration Release --output ./publish
   ```

3. Deploy the published files to your Azure App Service

## Testing the Fix

### 1. Test Health Endpoint
Visit: `https://onlinebookstore-backend-f4ejgsdudbghhkfz.australiaeast-01.azurewebsites.net/api/health`

### 2. Use the CORS Test Tool
Open `EbooksPlatfor.Server/wwwroot/test-cors.html` in your browser to test CORS configuration.

### 3. Test Frontend Application
After deployment, test your frontend application to ensure it can now fetch data from the backend.

## Verification
To verify the fix is working:

1. Check that the `Access-Control-Allow-Origin` header is present in API responses
2. Verify that your frontend can successfully fetch data from the backend
3. Check the browser console for any remaining CORS errors

## Additional Considerations

### Environment-Specific Configuration
Consider moving the allowed origins to configuration files for easier management across environments:

```json
{
  "CorsSettings": {
    "AllowedOrigins": [
      "https://localhost:52441",
      "http://localhost:52441",
      "https://happy-smoke-0d1f54100.2.azurestaticapps.net"
    ]
  }
}
```

### Monitoring
Monitor your application logs for any CORS-related issues after deployment.

### Security
The current configuration allows credentials. Ensure this aligns with your security requirements.

## Troubleshooting

### If CORS errors persist:
1. Check Azure App Service logs for any deployment issues
2. Verify the CORS middleware is being applied in the correct order
3. Test the health endpoints to ensure the API is accessible
4. Check if there are any proxy or firewall rules blocking the requests

### Common Issues:
- **Order of middleware**: CORS middleware must be applied before authentication/authorization
- **Case sensitivity**: Ensure the origin URLs match exactly
- **Protocol mismatch**: Ensure both frontend and backend use HTTPS in production

## Support
If you continue to experience issues after implementing these changes, check:
1. Azure App Service configuration
2. Network security groups and firewall rules
3. Application Insights logs for detailed error information
