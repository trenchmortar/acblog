#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/AcBlog.Data/AcBlog.Data.csproj", "src/AcBlog.Data/"]
COPY ["src/AcBlog.Sdk/AcBlog.Sdk.csproj", "src/AcBlog.Sdk/"]
COPY ["src/AcBlog.Data.Repositories.FileSystem/AcBlog.Data.Repositories.FileSystem.csproj", "src/AcBlog.Data.Repositories.FileSystem/"]
COPY ["src/client/AcBlog.Client.Core/AcBlog.Client.Core.csproj", "src/client/AcBlog.Client.Core/"]
COPY ["src/client/AcBlog.Client.UI/AcBlog.Client.UI.csproj", "src/client/AcBlog.Client.UI/"]
COPY ["src/client/AcBlog.Client.Server/AcBlog.Client.Server.csproj", "src/client/AcBlog.Client.Server/"]
RUN dotnet restore "src/client/AcBlog.Client.Server/AcBlog.Client.Server.csproj" -s https://sparkshine.pkgs.visualstudio.com/StardustDL/_packaging/feed/nuget/v3/index.json -s https://api.nuget.org/v3/index.json
COPY . .
WORKDIR "/src/src/client/AcBlog.Client.Server"
RUN dotnet build "AcBlog.Client.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AcBlog.Client.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AcBlog.Client.Server.dll"]