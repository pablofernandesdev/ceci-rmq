#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CeciRMQ.WebApplication/CeciRMQ.WebApplication.csproj", "CeciRMQ.WebApplication/"]
COPY ["CeciRMQ.Service/CeciRMQ.Service.csproj", "CeciRMQ.Service/"]
COPY ["CeciRMQ.Infra.Data/CeciRMQ.Infra.Data.csproj", "CeciRMQ.Infra.Data/"]
COPY ["CeciRMQ.Infra.CrossCutting/CeciRMQ.Infra.CrossCutting.csproj", "CeciRMQ.Infra.CrossCutting/"]
COPY ["CeciRMQ.Domain/CeciRMQ.Domain.csproj", "CeciRMQ.Domain/"]
COPY ["CeciRMQ.WebApplication/CeciRMQ.Domain.xml", "CeciRMQ.WebApplication/"]
COPY ["CeciRMQ.WebApplication/CeciRMQ.WebApplication.xml", "CeciRMQ.WebApplication/"]
RUN dotnet restore "CeciRMQ.WebApplication/CeciRMQ.WebApplication.csproj"
COPY . .
WORKDIR "/src/CeciRMQ.WebApplication"
RUN dotnet build "CeciRMQ.WebApplication.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CeciRMQ.WebApplication.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CeciRMQ.WebApplication.dll"]