FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ForkEat/ForkEat.Web/ForkEat.Web.csproj", "ForkEat.Web/"]
RUN dotnet restore "ForkEat.Web/ForkEat.Web.csproj"
COPY . .
WORKDIR "/src/ForkEat.Web"
RUN dotnet build "ForkEat.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ForkEat.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ForkEat.Web.dll"]
