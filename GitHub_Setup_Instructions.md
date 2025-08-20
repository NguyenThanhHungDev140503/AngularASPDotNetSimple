# GitHub Repository Setup Instructions for AngularASPDotNetSimple

This document provides step-by-step instructions to initialize a Git repository for your project and push it to GitHub.

## Project Overview

This is a full-stack application with:
- Backend: ASP.NET Core 9.0 Web API with Entity Framework Core
- Frontend: Angular application
- Database: SQL Server

## Steps to Set Up GitHub Repository

### 1. Initialize Git Repository

Open a terminal/command prompt in the project root directory (`TestCSharpCRUD`) and run:

```bash
git init
```

### 2. Create .gitignore File

Create a `.gitignore` file in the project root directory with the following content:

```gitignore
# Visual Studio
.vs/

# .NET Core build artifacts
**/bin/
**/obj/
**/*.user
**/*.suo

# Node.js dependencies
node_modules/

# Angular build artifacts
ProductAPI.Frontend/product-management/dist/
ProductAPI.Frontend/product-management/tmp/

# Log files
*.log

# Environment files
.env
.env.local
.env.production

# IDE files
*.swp
*.swo
.DS_Store
Thumbs.db
```

### 3. Add and Commit Files

Add all files to the repository and make an initial commit:

```bash
git add .
git commit -m "Initial commit: ASP.NET Core + Angular project"
```

### 4. Create GitHub Repository

1. Go to [GitHub](https://github.com) and log in to your account
2. Click the "+" icon in the top right corner and select "New repository"
3. Enter the repository name: `AngularASPDotNetSimple`
4. Optionally add a description
5. Keep the repository as "Public" (or "Private" if you prefer)
6. Do NOT initialize with a README, .gitignore, or license
7. Click "Create repository"

### 5. Push Code to GitHub

After creating the repository on GitHub, you'll see instructions for pushing an existing repository. Copy the repository URL and run these commands:

```bash
git remote add origin <your-github-repo-url>
git branch -M main
git push -u origin main
```

Replace `<your-github-repo-url>` with the actual URL of your GitHub repository.

## Additional Notes

- Make sure you have Git installed on your system
- Ensure you have a GitHub account
- The project structure includes both backend (.NET) and frontend (Angular) components
- All project documentation is included in the `docs/` folder