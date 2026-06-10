# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy sln and csproj files first to cache restore
COPY BidaPlatform_BE.sln ./
COPY src/BidaPlatform.Domain/BidaPlatform.Domain.csproj src/BidaPlatform.Domain/
COPY src/BidaPlatform.Application/BidaPlatform.Application.csproj src/BidaPlatform.Application/
COPY src/BidaPlatform.Infrastructure/BidaPlatform.Infrastructure.csproj src/BidaPlatform.Infrastructure/
COPY src/BidaPlatform.Presentation/BidaPlatform.Presentation.csproj src/BidaPlatform.Presentation/

RUN dotnet restore

# Copy all source code and publish
COPY . ./
RUN dotnet publish -c Release -o out src/BidaPlatform.Presentation/BidaPlatform.Presentation.csproj

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Bind to port 8080 (standard default for ASP.NET Core 8.0 container)
ENV ASPNETCORE_URLS=http://*:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "BidaPlatform.Presentation.dll"]
