
---

### ✅ `SOLUTION.md`

```md
# RecipeShare: Architecture & Solution Design

## 1. 🔍 Architecture Overview

RecipeShare is a containerized full-stack web app designed to showcase recipe creation and management. It consists of:

- **Frontend**: Angular SPA, served via the .NET backend (`wwwroot`)
- **Backend**: .NET 8 REST API with layered architecture (Controller → Service → Repository)
- **Persistence**: SQL Server database via Entity Framework Core
- **Authentication**: JWT-based auth with role claims (e.g. `SystemAdmin`)
- **Deployment**: Docker with multi-stage build (Angular + .NET + SQL Server)

## 2. ⚖️ Design Trade-offs

| Choice                        | Trade-off                                                                 |
|-----------------------------|--------------------------------------------------------------------------|
| Serving Angular via .NET    | Simpler deployment; harder to scale independently                        |
| Docker for local dev        | Consistency; requires more initial setup for new developers              |
| JWT Authentication          | Stateless and scalable; needs secure key rotation                        |
| SQL Server in Docker        | Easy setup; not production-grade performance for large scale             |

## 3. 🔐 Security & Monitoring

### Security

- JWT tokens validated via `SymmetricSecurityKey`
- Role-based authorization for admin-only endpoints
- CORS policy allows all origins in dev – should be restricted in prod
- HTTPS redirection disabled in dev, must be enforced in production

### Monitoring (Suggestions)

- Add **Serilog** for structured logging
- Integrate **Health Checks** for containers
- Setup **Application Insights** or **Prometheus + Grafana** in production

## 4. 💰 Cost Strategies

| Area             | Strategy                                                                 |
|------------------|--------------------------------------------------------------------------|
| Hosting          | Host API on Azure App Service or Render.com; frontend can use same host |
| Database         | Use Azure SQL with minimal tier (DTU-based) for cost control             |
| CI/CD            | GitHub Actions with Docker caching to save build time and cost           |
| Monitoring       | Use free tiers (e.g. App Insights or Prometheus OSS)                     |

---

## ✅ Future Enhancements

- Add user registration and profile management
- Image upload support (via Azure Blob or S3)
- Pagination + filtering on recipes
- Unit & integration testing