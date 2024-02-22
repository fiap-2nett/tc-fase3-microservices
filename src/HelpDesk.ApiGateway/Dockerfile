#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/HelpDesk.ApiGateway/HelpDesk.ApiGateway.csproj", "src/HelpDesk.ApiGateway/"]
RUN dotnet restore "./src/HelpDesk.ApiGateway/HelpDesk.ApiGateway.csproj"
COPY . .
WORKDIR "/src/src/HelpDesk.ApiGateway"
RUN dotnet build "./HelpDesk.ApiGateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./HelpDesk.ApiGateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HelpDesk.ApiGateway.dll"]