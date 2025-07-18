# Section 3: Getting Started

Welcome to the first hands-on step of your Ebooks Platform journey! 🚀

In this section, we’ll set up your development environment and create your project using Visual Studio’s powerful React + ASP.NET Core template. By the end, you’ll have a running full-stack app and a clear understanding of your project’s structure.

---

## 🖥️ Step 1: Open Visual Studio and Create a New Project

1. **Launch Visual Studio 2022**
2. Click **"Create a new project"**
3. In the search box, type **"React and ASP.NET Core"**
4. **Select the template:**
   - **React and ASP.NET Core (TypeScript)** (recommended)
   - If you see both JavaScript and TypeScript options, choose **TypeScript** for modern best practices
5. Click **Next**

---

## 📝 Step 2: Configure Your Project

- **Project name:** `EbooksPlatform`
- **Location:** Choose a folder where you want your solution to live
- **Solution name:** `EbooksPlatform` (or your preferred name)
- Click **Next**

**Framework settings:**
- **.NET version:** `.NET 7.0` (or latest LTS)
- **Authentication type:** `None` (we’ll add authentication later)
- **Configure for HTTPS:** ✅ Checked
- **Enable Docker:** ❌ Unchecked (unless you want to use containers)
- **Use controllers:** ✅ Checked
- **Enable OpenAPI support:** ✅ Checked
- Click **Create**

---

## 🏗️ Step 3: Explore the Project Structure

Visual Studio will generate a solution with two main projects:

```
EbooksPlatform/
├── EbooksPlatform/         # ASP.NET Core backend (C#)
│   ├── Controllers/
│   ├── Models/
│   ├── Data/
│   ├── Services/
│   ├── Program.cs
│   └── appsettings.json
└── ebooksplatfor.client/   # React frontend (TypeScript)
    ├── src/
    ├── public/
    └── package.json
```

- **EbooksPlatform:** Your backend API project (C#)
- **ebooksplatfor.client:** Your React frontend project (TypeScript)

**What’s already set up?**
- React and ASP.NET Core are connected out of the box
- CORS and proxy settings are pre-configured
- You can debug both frontend and backend together

---

## ▶️ Step 4: Run the Template for the First Time

1. **Set the solution as the startup project** (right-click the solution → Configure Setup Projects… → select both projects if needed)
2. **Press F5** or click the **Start** button
3. Visual Studio will:
   - Start the ASP.NET Core backend
   - Start the React development server
   - Open your browser to the React app (usually at `http://localhost:3000`)

**You should see the default React welcome page!**

---

## 🧭 What’s Next?

Now that your project is running, you’re ready to start building your data models and core features. In the next section, we’ll design the data structure for your ebooks platform.

**Pro Tip:**
- Try making a change in the React `src/App.tsx` file and see it update live in your browser!
- Explore the `Controllers/WeatherForecastController.cs` file to see a sample API endpoint.

---

**Next up:**
- [Section 4: Building the Data Structure (Model Design)](./04-BUILDING-DATA-STRUCTURE.md) 