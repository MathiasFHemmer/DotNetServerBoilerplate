# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set the working directory inside the container
WORKDIR /src

# Copy the solution file to the container
COPY *.sln ./

# Copy project definition files (.csproj) only to leverage Docker caching
COPY src/MHemmer.Boilerplate.Api/MHemmer.Boilerplate.Api.csproj MHemmer.Boilerplate.Api/
COPY src/MHemmer.Boilerplate.Domain/MHemmer.Boilerplate.Domain.csproj MHemmer.Boilerplate.Domain/
COPY src/MHemmer.Boilerplate.Common/MHemmer.Boilerplate.Common.csproj MHemmer.Boilerplate.Common/
COPY src/MHemmer.Boilerplate.Infra/MHemmer.Boilerplate.Infra.csproj MHemmer.Boilerplate.Infra/

# Restore NuGet dependencies for the entry-point project (API)
RUN dotnet restore MHemmer.Boilerplate.Api/MHemmer.Boilerplate.Api.csproj

# Copy the full source code into the container
COPY src/ .

# Build the API project in Release mode, outputting binaries to /build
RUN dotnet build MHemmer.Boilerplate.Api/MHemmer.Boilerplate.Api.csproj -c Release -o /build

# Stage 2: Publish
FROM build AS publish

# Publish the API project to /app (optimized, release-ready files)
RUN dotnet publish MHemmer.Boilerplate.Api/MHemmer.Boilerplate.Api.csproj -c Release -o /app

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

# Set the working directory for the runtime container
WORKDIR /app

# Copy the published app from the publish stage
COPY --from=publish /app .

# Expose ports that the API will listen on
# HTTP
EXPOSE 8080
# HTTPS
EXPOSE 8081

# Define the container startup command
ENTRYPOINT ["dotnet", "MHemmer.Boilerplate.Api.dll"]
