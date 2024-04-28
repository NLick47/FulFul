#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM jrottenberg/ffmpeg:4.3 AS ffmpeg_builder
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["VideoService.WebAPI/VideoService.WebAPI.csproj", "VideoService.WebAPI/"]
COPY ["CommonInitializer/CommonInitializer.csproj", "CommonInitializer/"]
COPY ["Bli.JWT/Bli.JWT.csproj", "Bli.JWT/"]
COPY ["Bli.Common/Bli.Common.csproj", "Bli.Common/"]
COPY ["VideoService.Domain/VideoService.Domain.csproj", "VideoService.Domain/"]
COPY ["VideoService.Infrastructure/VideoService.Infrastructure.csproj", "VideoService.Infrastructure/"]
RUN dotnet restore "VideoService.WebAPI/VideoService.WebAPI.csproj"
COPY . .
WORKDIR "/src/VideoService.WebAPI"
RUN dotnet build "VideoService.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VideoService.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VideoService.WebAPI.dll"]