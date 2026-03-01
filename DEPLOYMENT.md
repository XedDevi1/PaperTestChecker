# PaperTestChecker — Free Deployment Guide

This guide explains how to deploy the entire PaperTestChecker stack **for free** using Neon (Database), Render (Backend), and Vercel/Netlify (Frontend).

---

## 1. Database Deployment (Neon.tech)

1. Create a free account at [Neon.tech](https://neon.tech/).
2. Create a new project (e.g., `papertestchecker-db`).
3. Neon will give you a **Postgres Connection String** that looks like this:
   `postgresql://your_user:your_password@ep-cool-butterfly-123456.eu-central-1.aws.neon.tech/neondb?sslmode=require`
4. Convert this to a .NET compatible connection string:
   `Host=ep-cool-butterfly-123456.eu-central-1.aws.neon.tech;Port=5432;Database=neondb;Username=your_user;Password=your_password;SslMode=Require`
5. Save this — you will need it for the Backend deployment.

---

## 2. Backend Deployment (Render.com)

We will deploy the .NET API using the `Dockerfile` we prepared.

1. Push your code to a GitHub repository.
2. Create a free account at [Render.com](https://render.com/).
3. Click **New +** and select **Web Service**.
4. Connect your GitHub repository.
5. Provide the following settings:
   - **Language**: `Docker`
   - **Branch**: `main` (or your default branch)
   - **Region**: Choose the one closest to your Neon database
   - **Instance Type**: Free
6. Scroll down to **Environment Variables** and add the following:

   | Key | Value | Description |
   |---|---|---|
   | `ConnectionStrings__DefaultConnection` | `Host=...;Port=5432;...` | The converted Neon string from Step 1 |
   | `Jwt__Secret` | `generate-a-long-random-string-here` | Any long random text (>32 chars) |
   | `Ai__ApiKey` | `gsk_your_groq_key` | Your Groq API key |
   | `AllowedOrigins` | *(leave blank for now, update after Step 3)* | The URL of your frontend |

7. Click **Create Web Service**. Render will build and deploy the .NET API. It gives you a URL (e.g., `https://papertest-api.onrender.com`). Copy this URL.

> **Note on Migrations**: Since the database is new, your tables need to be created. Our `Program.cs` doesn't auto-migrate on startup for safety. You can temporarily add `using (var scope = app.Services.CreateScope()) { scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate(); }` to `Program.cs` and push, or run the migration locally against the remote database.

---

## 3. Frontend Deployment (Vercel.com)

1. Create a free account at [Vercel.com](https://vercel.com/) and connect your GitHub.
2. Click **Add New** -> **Project** and import your repository.
3. Keep the default settings (Framework Preset: Vite, Root Directory: `./frontend`).
4. Open the **Environment Variables** dropdown and add:

   | Name | Value |
   |---|---|
   | `VITE_API_URL` | `https://papertest-api.onrender.com/api` *(the URL from Render Step 7)* |

5. Click **Deploy**. Vercel will build the React app and give you a public URL (e.g., `https://papertest-frontend.vercel.app`).

---

## 4. Finalizing the Connection (CORS)

Now that the frontend is live, we need to tell the backend to trust it.

1. Go back to your **Render Dashboard** for the Backend Web Service.
2. Go to **Environment Variables**.
3. Update the `AllowedOrigins` variable you left blank earlier:
   - **Key**: `AllowedOrigins`
   - **Value**: `https://papertest-frontend.vercel.app` (your actual Vercel URL without the trailing slash)
4. Render will automatically restart your backend.

**🎉 Congratulations! Your full stack is now deployed and running for free.**
