FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY "CoursesApp.sln" "CoursesApp.sln"

COPY "src/EventBus/EventBus/EventBus.csproj" "src/EventBus/EventBus/EventBus.csproj"
COPY "src/EventBus/EventBusRabbitMQ/EventBusRabbitMQ.csproj" "src/EventBus/EventBusRabbitMQ/EventBusRabbitMQ.csproj"
COPY "src/JwtAuthenticationManager/JwtAuthenticationManager.csproj" "src/JwtAuthenticationManager/JwtAuthenticationManager.csproj"
COPY "src/Services/Catalog/Catalog.API/Catalog.API.csproj" "src/Services/Catalog/Catalog.API/Catalog.API.csproj"
COPY "src/Services/Catalog/Catalog.Services/Catalog.Services.csproj" "src/Services/Catalog/Catalog.Services/Catalog.Services.csproj"
COPY "src/Services/Catalog/Catalog.Data/Catalog.Data.csproj" "src/Services/Catalog/Catalog.Data/Catalog.Data.csproj"
COPY "src/Services/Catalog/Catalog.Core/Catalog.Core.csproj" "src/Services/Catalog/Catalog.Core/Catalog.Core.csproj"
COPY "src/Services/Catalog/Catalog.Tests/Catalog.Tests.csproj" "src/Services/Catalog/Catalog.Tests/Catalog.Tests.csproj"
COPY "src/Services/Enrolling/Enrolling.API/Enrolling.API.csproj" "src/Services/Enrolling/Enrolling.API/Enrolling.API.csproj"
COPY "src/Services/Enrolling/Enrolling.Tests/Enrolling.Tests.csproj" "src/Services/Enrolling/Enrolling.Tests/Enrolling.Tests.csproj"
COPY "src/Services/Identity/Identity.API/Identity.API.csproj" "src/Services/Identity/Identity.API/Identity.API.csproj"
COPY "src/ApiGateways/Web.Bff/Web.Bff.csproj" "src/ApiGateways/Web.Bff/Web.Bff.csproj"

RUN dotnet restore "CoursesApp.sln"

COPY . .
WORKDIR /src/src/Services/Enrolling/Enrolling.API
RUN dotnet build Enrolling.API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Enrolling.API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Enrolling.API.dll"]