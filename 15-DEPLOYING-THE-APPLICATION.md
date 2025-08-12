# Azure Deployment Guide: Separate Creation

## ✅ Microsoft Docs: Separate Creation Guide

### 🔹 Create an Azure SQL Database
[Microsoft SQL Database Quickstart](https://learn.microsoft.com/en-us/azure/azure-sql/database/single-database-create-quickstart?view=azuresql&tabs=azure-portal)

### 🔹 Create an Azure App Service  
[Microsoft App Service Quickstart](https://learn.microsoft.com/en-us/azure/app-service/quickstart-dotnetcore?tabs=net60&pivots=development-environment-vs)

## 🧾 Step-by-Step Summary

### 🗂 Step 1: Create Azure SQL Database (Portal)
1. **Go to Azure Portal**
2. **Search for SQL databases** → **Create**
3. **Fill in:**
   - Database name
   - **New SQL server** → set admin username & password
   - Choose basic/standard pricing for testing
4. **Click Review + Create, then Create**

#### 🔒 After deployment:
- Go to your **SQL Server** → **Set firewall rules** to allow your IP
- **Note down the connection string** from the database → **ADO.NET string**

### 🌐 Step 2: Create Azure App Service (Portal)
1. **In the portal, search for App Services** → **Create**
2. **Select:**
   - **Runtime stack**: .NET (Core)
   - **Operating System**: Windows
   - **Region**: same as your SQL database
   - **Hosting plan**: create or use existing
   - **Resource group**: reuse or create new
3. **Click Review + Create, then Create**

### 🔄 Step 3: Deploy Your .NET App (Via Visual Studio)
1. **Right-click your project** → **Publish**
2. **Select Azure** → **App Service (Existing)**
3. **Choose the App Service** you just created
4. **Click Finish** → then **Publish**

#### 🔧 If Visual Studio Publish Fails:
1. **Go to Azure Portal** → **App Service** → **Get publish profile**
2. **Download the `.pubxml` file**
3. **In Visual Studio**: Right-click project → **Publish** → **Import Profile**
4. **Select the downloaded `.pubxml` file**
5. **Click Publish**

### 📝 Step 3.5: Update appsettings.json (Before Deployment)
1. **Open your project's `appsettings.json`**
2. **Replace the connection string** with your Azure SQL Database connection string:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=tcp:your-server.database.windows.net,1433;Initial Catalog=your-database;Persist Security Info=False;User ID=your-username;Password=your-password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
     }
   }
   ```
3. **Save the file** before publishing

### 🔌 Step 4: Connect App to SQL Database
1. **In Azure Portal** → **App Service** → **Configuration** → **Connection strings**
2. **Add:**
   - **Name**: `DefaultConnection` (or whatever your app expects)
   - **Value**: Your SQL ADO.NET connection string (with `Server=…;Initial Catalog=…;User ID=…;Password=…`)
   - **Type**: `SQLAzure`

### 🛡 Optionally: Use Managed Identity instead of username/password

## **Troubleshooting Tips**

### **If Something Goes Wrong:**

#### **1. Check Log Stream**
- Go to your Web App → **Log stream**
- Look for error messages during startup
- Common issues: missing connection strings, database errors

#### **2. Database Connection Issues**
- Verify connection string in **Configuration** → **Connection strings**
- Check if database exists in Azure SQL databases
- Ensure firewall allows Azure services

#### **3. Application Not Starting**
- Check **Log stream** for startup errors
- Verify all services are registered in `Program.cs`
- Ensure `ASPNETCORE_ENVIRONMENT = Production`

#### **4. 404/500 Errors**
- Test `/swagger` endpoint first
- Check if application is actually running
- Look for specific error messages in logs

#### **5. Deployment Failures**
- Check GitHub Actions logs if using CI/CD
- Verify build succeeds locally first
- Check Azure resource limits in your region

### **Quick Commands:**
```bash
# Check app status
az webapp show --name your-app-name --resource-group your-rg

# View logs
az webapp log tail --name your-app-name --resource-group your-rg

# Restart app
az webapp restart --name your-app-name --resource-group your-rg
```

### **Expected URLs:**
- `https://your-app-name.azurewebsites.net/` - Root
- `https://your-app-name.azurewebsites.net/swagger` - API docs
- `https://your-app-name.azurewebsites.net/api/books` - Books endpoint

## **Cost Estimate:**
- **App Service Basic B1**: ~$13/month
- **SQL Database Basic**: ~$5/month
- **Total**: ~$18/month

**References**: 
- [Microsoft SQL Database Tutorial](https://learn.microsoft.com/en-us/azure/azure-sql/database/single-database-create-quickstart?view=azuresql&tabs=azure-portal)
- [Microsoft App Service Tutorial](https://learn.microsoft.com/en-us/azure/app-service/quickstart-dotnetcore?tabs=net60&pivots=development-environment-vs)
