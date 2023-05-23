#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=https://+:443
# ENV NODE_OPTIONS=--openssl-legacy-provider
EXPOSE 443
 
RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y libpng-dev libjpeg-dev curl libxi6 build-essential libgl1-mesa-glx
# RUN curl -sL https://deb.nodesource.com/setup_lts.x | bash -
RUN curl -sL https://deb.nodesource.com/setup_16.x | bash -
RUN apt-get install -y nodejs

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y libpng-dev libjpeg-dev curl libxi6 build-essential libgl1-mesa-glx
# RUN curl -sL https://deb.nodesource.com/setup_lts.x | bash -
RUN curl -sL https://deb.nodesource.com/setup_16.x | bash -
RUN apt-get install -y nodejs
WORKDIR /src
COPY ["RAPITest/RAPITest.csproj", "RAPITest/"]
COPY ["ModelsLibrary/ModelsLibrary.csproj", "ModelsLibrary/"]
RUN dotnet restore "RAPITest/RAPITest.csproj"
COPY . .
WORKDIR "/src/RAPITest"
RUN dotnet build "RAPITest.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RAPITest.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY RAPITest/certs/certificate.pfx ./certs/
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RAPITest.dll"]




