FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["GrpcHelloWorld.Server/GrpcHelloWorld.Server.csproj", "GrpcHelloWorld.Server/"]
RUN dotnet restore "GrpcHelloWorld.Server/GrpcHelloWorld.Server.csproj"
COPY . .
WORKDIR "/src/GrpcHelloWorld.Server"
RUN dotnet build "GrpcHelloWorld.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GrpcHelloWorld.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GrpcHelloWorld.Server.dll"]
