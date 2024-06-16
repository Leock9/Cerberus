# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the .csproj file and restore any dependencies
COPY *.sln .
COPY src/Api/Cerberus.Api/*.csproj ./src/Api/Cerberus.Api/
RUN dotnet restore

# Copy the remaining source code and build the application
COPY . .
WORKDIR /app/src/Api/Cerberus.Api
RUN dotnet publish -c Release -o out

# Use the official ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/src/Api/Cerberus.Api/out ./

# Expose the port the application runs on
EXPOSE 80

# Run the application
ENTRYPOINT ["dotnet", "Cerberus.Api.dll"]