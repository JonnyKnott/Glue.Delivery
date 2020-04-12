FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS source
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY Glue.Delivery.sln ./
COPY src/Glue.Delivery.WebApi/Glue.Delivery.WebApi.csproj src/Glue.Delivery.WebApi/
COPY src/Glue.Delivery.Models/Glue.Delivery.Models.csproj src/Glue.Delivery.Models/
COPY src/Glue.Delivery.Data/Glue.Delivery.Data.csproj src/Glue.Delivery.Data/
COPY src/Glue.Delivery.Services/Glue.Delivery.Services.csproj src/Glue.Delivery.Services/
COPY src/Glue.Delivery.Core/Glue.Delivery.Core.csproj src/Glue.Delivery.Core/
COPY test/Glue.Delivery.WebApi.Integration.Test/Glue.Delivery.WebApi.Integration.Test.csproj test/Glue.Delivery.WebApi.Integration.Test/
COPY test/Glue.Delivery.Services.Test/Glue.Delivery.Services.Test.csproj test/Glue.Delivery.Services.Test/
COPY test/Glue.Delivery.Data.Test/Glue.Delivery.Data.Test.csproj test/Glue.Delivery.Data.Test/
RUN dotnet restore

WORKDIR /src
COPY . .

FROM source as publish
WORKDIR /src/src/Glue.Delivery.WebApi
RUN dotnet build -c Release -o /app
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 as base
WORKDIR /ap
COPY --from=publish /app .
EXPOSE 80
ENTRYPOINT ["dotnet", "Glue.Delivery.WebApi.dll"]