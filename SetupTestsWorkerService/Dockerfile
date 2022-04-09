#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["SetupTestsWorkerService/SetupTestsWorkerService.csproj", "SetupTestsWorkerService/"]
COPY ["ModelsLibrary/ModelsLibrary.csproj", "ModelsLibrary/"]
RUN dotnet restore "SetupTestsWorkerService/SetupTestsWorkerService.csproj"
COPY . .
WORKDIR "/src/SetupTestsWorkerService"
RUN dotnet build "SetupTestsWorkerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SetupTestsWorkerService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SetupTestsWorkerService.dll"]
