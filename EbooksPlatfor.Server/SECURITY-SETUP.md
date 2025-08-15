# Security Setup Guide

## ⚠️ IMPORTANT: Secure Your Configuration

Your application contains sensitive information that should NEVER be committed to version control.

## Option 1: Environment Variables (Recommended)

### Step 1: Set Environment Variables

#### Windows (PowerShell as Administrator):
```powershell
# Run the provided script
.\set-env-vars.ps1

# Or set manually:
[Environment]::SetEnvironmentVariable("DB_PASSWORD", "YOUR_ACTUAL_PASSWORD", "Machine")
[Environment]::SetEnvironmentVariable("JWT_SECRET_KEY", "YOUR_JWT_SECRET", "Machine")
[Environment]::SetEnvironmentVariable("PAYPAL_CLIENT_SECRET", "YOUR_PAYPAL_SECRET", "Machine")
```

#### Windows (Command Prompt as Administrator):
```cmd
setx DB_PASSWORD "YOUR_ACTUAL_PASSWORD" /M
setx JWT_SECRET_KEY "YOUR_JWT_SECRET" /M
setx PAYPAL_CLIENT_SECRET "YOUR_PAYPAL_SECRET" /M
```

#### Linux/macOS:
```bash
export DB_PASSWORD="YOUR_ACTUAL_PASSWORD"
export JWT_SECRET_KEY="YOUR_JWT_SECRET"
export PAYPAL_CLIENT_SECRET="YOUR_PAYPAL_SECRET"
```

### Step 2: Restart Your Application
After setting environment variables, restart your application for changes to take effect.

## Option 2: User Secrets (Development Only)

For development, you can use .NET User Secrets:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YOUR_CONNECTION_STRING"
dotnet user-secrets set "JwtSettings:SecretKey" "YOUR_JWT_SECRET"
dotnet user-secrets set "PayPal:ClientSecret" "YOUR_PAYPAL_SECRET"
```

## Option 3: Azure Key Vault (Production)

For production environments, use Azure Key Vault:

```csharp
// In Program.cs
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
    new DefaultAzureCredential());
```

## Required Environment Variables

| Variable | Description | Example |
|----------|-------------|---------|
| `DB_SERVER` | Database server address | `tcp:onlinebookstore-app-server.database.windows.net,1433` |
| `DB_NAME` | Database name | `OnlineBookstore` |
| `DB_USERNAME` | Database username | `onlinebookstore-admin` |
| `DB_PASSWORD` | Database password | `YOUR_ACTUAL_PASSWORD` |
| `JWT_SECRET_KEY` | JWT signing key | `YOUR_JWT_SECRET_KEY` |
| `PAYPAL_CLIENT_ID` | PayPal client ID | `YOUR_PAYPAL_CLIENT_ID` |
| `PAYPAL_CLIENT_SECRET` | PayPal client secret | `YOUR_PAYPAL_CLIENT_SECRET` |

## Security Checklist

- [ ] Remove hardcoded passwords from appsettings.json
- [ ] Set environment variables with actual values
- [ ] Update .gitignore to exclude sensitive files
- [ ] Restart application after changes
- [ ] Verify sensitive data is not in version control
- [ ] Use strong, unique passwords
- [ ] Rotate secrets regularly

## Troubleshooting

If you get configuration errors:
1. Verify environment variables are set correctly
2. Check if variables are accessible to your application
3. Ensure application has been restarted after setting variables
4. Use `echo $env:VARIABLE_NAME` (PowerShell) or `echo $VARIABLE_NAME` (bash) to verify

## Next Steps

1. Set your actual environment variables
2. Test the application
3. Remove any remaining hardcoded credentials
4. Consider using Azure Key Vault for production
