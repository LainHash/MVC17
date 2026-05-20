# ============================================================
# Stage 1: Build
# ============================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution & project files first for layer-cache optimization
COPY ["MVC17/MVC17.csproj", "MVC17/"]
RUN dotnet restore "MVC17/MVC17.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/MVC17"
RUN dotnet build "MVC17.csproj" -c Release -o /app/build

# ============================================================
# Stage 2: Publish
# ============================================================
FROM build AS publish
RUN dotnet publish "MVC17.csproj" -c Release -o /app/publish --no-restore

# ============================================================
# Stage 3: Runtime (lean image)
# ============================================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create a non-root user for security
RUN addgroup --system --gid 1001 appgroup && \
    adduser  --system --uid 1001 --ingroup appgroup appuser

# Copy published output
COPY --from=publish /app/publish .

# Set ownership so non-root user can write (e.g. DataProtection keys)
RUN chown -R appuser:appgroup /app

USER appuser

# Expose HTTP (reverse proxy terminates TLS externally)
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "MVC17.dll"]
