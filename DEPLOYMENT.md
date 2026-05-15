# MVC17 Deployment Guide

> ASP.NET Core 8 · Docker · GitHub Actions CI/CD · SQL Server 2019

---

## Table of Contents

1. [Prerequisites](#1-prerequisites)
2. [Project Architecture](#2-project-architecture)
3. [Environment Variables Reference](#3-environment-variables-reference)
4. [Local Testing with Docker Compose](#4-local-testing-with-docker-compose)
5. [GitHub Actions Configuration](#5-github-actions-configuration)
6. [First-Time Production Setup](#6-first-time-production-setup)
7. [Deployment Procedure](#7-deployment-procedure)
8. [Rollback Procedure](#8-rollback-procedure)
9. [Database Backup & Restore](#9-database-backup--restore)
10. [Nginx Reverse Proxy (HTTPS)](#10-nginx-reverse-proxy-https)
11. [Monitoring & Logging](#11-monitoring--logging)
12. [Troubleshooting](#12-troubleshooting)

---

## 1. Prerequisites

### Local Machine
- [Docker Desktop](https://www.docker.com/products/docker-desktop) ≥ 24
- [.NET SDK 8.0](https://dotnet.microsoft.com/download) (for local development only)
- Git

### Production Server
- Ubuntu 22.04 LTS (or equivalent)
- Docker Engine ≥ 24
- Nginx (for reverse proxy + TLS)
- Certbot (Let's Encrypt SSL)
- A domain name pointing to your server IP

### GitHub
- GitHub repository with Actions enabled
- Docker Hub account

---

## 2. Project Architecture

```
                 ┌─────────────────────────────────────────┐
                 │            GitHub Actions                │
                 │  push → build → docker push → SSH deploy │
                 └─────────────┬───────────────────────────┘
                               │ SSH
                 ┌─────────────▼──────────────────────────┐
  Internet ──── Nginx (443/80) ──► mvc17_app:8080          │
                 │            Production Server             │
                 │  Docker containers:                      │
                 │  • mvc17_app  (ASP.NET Core 8)           │
                 │  • SQL Server (external / managed DB)    │
                 └────────────────────────────────────────┘
```

**Secrets flow:** GitHub Secrets → `docker run -e` flags → ASP.NET Core env var config provider → app.  
No secrets are ever baked into the Docker image.

---

## 3. Environment Variables Reference

| Variable | Required | Description | Example |
|----------|----------|-------------|---------|
| `SQL_CONNECTION_STRING` | ✅ | Full ADO.NET connection string | `Data Source=...` |
| `JWT_SECRET` | ✅ | JWT signing key (≥ 32 chars) | `random-long-string` |
| `JWT_ISSUER` | Optional | JWT issuer claim | `MyApp` |
| `JWT_AUDIENCE` | Optional | JWT audience claim | `MyAppUsers` |
| `GMAIL_SENDER_EMAIL` | ✅ | Sender email address | `you@gmail.com` |
| `EMAIL_SMTP_PASSWORD` | ✅ | Gmail App Password | `xxxx xxxx xxxx xxxx` |
| `GMAIL_HOST` | Optional | SMTP host | `smtp.gmail.com` |
| `GMAIL_PORT` | Optional | SMTP port | `587` |
| `GMAIL_SENDER_NAME` | Optional | Display name | `DDH` |

> ASP.NET Core maps environment variables with `__` as section separator.  
> `ConnectionStrings__MyConnectString` → `Configuration["ConnectionStrings:MyConnectString"]`

---

## 4. Local Testing with Docker Compose

```bash
# 1. Clone the repo
git clone <your-repo-url>
cd MVC17

# 2. Create your local .env from the template
cp .env.example .env
# Edit .env and fill in real values (use SA_PASSWORD for SQL Server)

# 3. Start everything
docker-compose up -d

# 4. Wait ~30s for SQL Server to be ready, then open:
#    http://localhost:8080

# 5. View logs
docker-compose logs -f app

# 6. Stop
docker-compose down          # keep DB volume
docker-compose down -v       # also delete DB volume (reset)
```

### Restore your database into the local container

```bash
# Copy your .bak file into the SQL Server container
docker cp DBMVC05_backup.bak mvc17_db:/var/opt/mssql/backup/DBMVC05_latest.bak

# Then run the restore section in Scripts/init-database.sql
docker exec -it mvc17_db \
  /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" \
  -i /var/opt/mssql/backup/restore.sql
```

---

## 5. GitHub Actions Configuration

### Required GitHub Secrets

Go to **Settings → Secrets and variables → Actions → New repository secret** for each:

| Secret Name | Value |
|-------------|-------|
| `DOCKER_USERNAME` | Your Docker Hub username |
| `DOCKER_PASSWORD` | Docker Hub access token (not your password!) |
| `PROD_SERVER_HOST` | Production server IP or hostname |
| `PROD_SERVER_USER` | SSH user (e.g. `ubuntu`, `deploy`) |
| `PROD_SERVER_SSH_KEY` | Private SSH key content (entire key file) |
| `SQL_CONNECTION_STRING` | Production connection string |
| `JWT_SECRET` | Production JWT signing key |
| `GMAIL_SENDER_EMAIL` | Production sender email |
| `EMAIL_SMTP_PASSWORD` | Production Gmail App Password |

### How to generate a Docker Hub access token

1. Log in to [hub.docker.com](https://hub.docker.com)
2. Account Settings → Security → **New Access Token**
3. Name it `github-actions`, scope: **Read & Write**
4. Copy and save as `DOCKER_PASSWORD` secret

### How to generate an SSH key pair for deployment

```bash
# On your local machine
ssh-keygen -t ed25519 -C "github-actions-deploy" -f deploy_key -N ""

# Copy public key to your prod server
ssh-copy-id -i deploy_key.pub user@your-server-ip

# Add private key content (deploy_key) as PROD_SERVER_SSH_KEY secret
cat deploy_key
```

### Pipeline overview

```
push to main/master
    │
    ▼
┌─────────────────┐
│  build          │  dotnet restore → build → (test)
└────────┬────────┘
         │ success
         ▼
┌─────────────────┐
│  docker         │  build image → tag (sha + latest) → push to Hub
└────────┬────────┘
         │ success + push event only
         ▼
┌─────────────────┐
│  deploy         │  SSH → docker pull → stop old → docker run new
└─────────────────┘
```

---

## 6. First-Time Production Setup

```bash
# 1. Install Docker on your server
curl -fsSL https://get.docker.com | sh
sudo usermod -aG docker $USER

# 2. Install Nginx
sudo apt update && sudo apt install -y nginx certbot python3-certbot-nginx

# 3. Create backup directory for SQL backups
sudo mkdir -p /var/opt/mssql/backup
sudo chmod 777 /var/opt/mssql/backup

# 4. Create the Nginx site config (see Section 10)

# 5. Obtain SSL certificate
sudo certbot --nginx -d yourdomain.com

# 6. Pull and run the app for the first time
docker pull yourdockerhubusername/mvc17:latest
docker run -d \
  --name mvc17_app \
  --restart unless-stopped \
  -p 127.0.0.1:8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e "ConnectionStrings__MyConnectString=<your-connection-string>" \
  -e "Jwt__Key=<your-jwt-secret>" \
  -e "GmailSettings__AppPassword=<your-app-password>" \
  -e "GmailSettings__SenderEmail=<your-email>" \
  yourdockerhubusername/mvc17:latest
```

---

## 7. Deployment Procedure

### Automatic (via GitHub Actions)

1. Commit and push to `main`/`master`
2. GitHub Actions automatically: builds → pushes Docker image → deploys to prod
3. Check the Actions tab for progress

### Manual deployment

```bash
# On the production server
docker pull yourdockerhubusername/mvc17:latest
docker stop mvc17_app
docker rm mvc17_app
docker run -d --name mvc17_app --restart unless-stopped \
  -p 127.0.0.1:8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  # ... (add all env vars) ...
  yourdockerhubusername/mvc17:latest
```

---

## 8. Rollback Procedure

### Option A: Re-deploy a specific image tag

GitHub Actions tags every image with the commit SHA (`sha-XXXXXXX`).

```bash
# On the production server — replace SHA with the good commit
docker stop mvc17_app && docker rm mvc17_app
docker run -d --name mvc17_app --restart unless-stopped \
  -p 127.0.0.1:8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  # ... (all env vars) ...
  yourdockerhubusername/mvc17:sha-XXXXXXX
```

### Option B: Git revert + re-deploy

```bash
# Locally
git revert HEAD --no-edit
git push origin main
# → triggers GitHub Actions → auto-deploy
```

---

## 9. Database Backup & Restore

### Pre-deployment backup (ALWAYS do this before deploying)

```bash
# Run the backup script against your prod SQL Server
sqlcmd -S <server> -U sa -P <password> -i Scripts/backup-database.sql
```

Backups land in `C:\SQLBackups\` (Windows) or configure `/var/opt/mssql/backup/` for Linux containers.

### Restore from backup

```sql
USE master;
RESTORE DATABASE [DBMVC05]
    FROM DISK = N'/path/to/DBMVC05_YYYY-MM-DD.bak'
    WITH REPLACE, STATS = 10;
GO
```

### Automated backup schedule (Linux cron)

```bash
# Edit crontab
crontab -e

# Add — daily backup at 02:00 AM
0 2 * * * /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" \
  -Q "BACKUP DATABASE [DBMVC05] TO DISK=N'/var/opt/mssql/backup/DBMVC05_$(date +\%Y\%m\%d).bak' WITH FORMAT, COMPRESSION;" \
  >> /var/log/db-backup.log 2>&1
```

---

## 10. Nginx Reverse Proxy (HTTPS)

Create `/etc/nginx/sites-available/mvc17`:

```nginx
server {
    listen 80;
    server_name yourdomain.com www.yourdomain.com;
    return 301 https://$host$request_uri;
}

server {
    listen 443 ssl http2;
    server_name yourdomain.com www.yourdomain.com;

    ssl_certificate     /etc/letsencrypt/live/yourdomain.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/yourdomain.com/privkey.pem;
    include             /etc/letsencrypt/options-ssl-nginx.conf;

    # Proxy to the Docker container
    location / {
        proxy_pass         http://127.0.0.1:8080;
        proxy_http_version 1.1;
        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection keep-alive;
        proxy_set_header   Host $host;
        proxy_set_header   X-Real-IP $remote_addr;
        proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
        proxy_read_timeout 90;
    }
}
```

```bash
sudo ln -s /etc/nginx/sites-available/mvc17 /etc/nginx/sites-enabled/
sudo nginx -t && sudo systemctl reload nginx
```

---

## 11. Monitoring & Logging

### View container logs

```bash
# Tail live logs
docker logs -f mvc17_app

# Last 100 lines
docker logs --tail 100 mvc17_app

# Since a specific time
docker logs --since 2025-01-01T00:00:00 mvc17_app
```

### Container health

```bash
docker inspect --format='{{.State.Health.Status}}' mvc17_app
docker stats mvc17_app
```

### Disk usage

```bash
docker system df
docker image prune -f   # clean up old images
```

---

## 12. Troubleshooting

| Symptom | Likely Cause | Fix |
|---------|-------------|-----|
| Container exits immediately | Missing env var / DB not reachable | `docker logs mvc17_app` — check for config errors |
| 502 Bad Gateway from Nginx | App not running on :8080 | `docker ps`, check container is up |
| DB connection refused | SQL Server not ready / wrong host | Check connection string, test with `sqlcmd` |
| JWT errors in browser | `JWT_SECRET` mismatch | Ensure same key in all environments |
| Email not sending | Wrong App Password / less-secure app | Re-generate Gmail App Password |
| Image push fails in CI | `DOCKER_PASSWORD` expired | Rotate Docker Hub access token |

---

*Generated by Antigravity — last updated automatically on each deployment.*
