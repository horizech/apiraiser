# Apiraiser

Content Management System written in .Net 5 and React

## Update all dependencies

```bash
dotnet restore
```

## Run

```bash
dotnet run
```

## Install packages

- JwtBearer

  ```bash
  dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
  ```

- PostgreSQL

  ```bash
  dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
  dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL.Design
  ```

- Swashbuckle (Swagger)

  ```bash
  dotnet add package Swashbuckle.AspNetCore
  ```

- Bcrypt
  ```bash
  dotnet add package BCrypt.Net-Next
  ```

## Environment Setup

- To be able to run application, you need to define the following environment variables:

  - Database setup:
    Environment Variable | Postgresql Config
    --- | ---
    APIRAISER_DATABASE_TYPE | Postgresql
    APIRAISER_DATABASE_HOST | (localhost)
    APIRAISER_DATABASE_PORT | (5432)
    APIRAISER_DATABASE_NAME | (postgres)
    APIRAISER_DATABASE_USERNAME | (postgres)
    APIRAISER_DATABASE_PASSWORD | (postgres)

  - JWT Config
    Environment Variable | Value
    --- | ---  
    APIRAISER_JWT_TOKEN_ISSUER | Apiraiser
    APIRAISER_JWT_TOKEN_SECRET_KEY | (key)

  - Logging
    Environment Variable | Value
    --- | ---
    APIRAISER_LOGGING_LEVEL | (anyone from: Errors, Debug, Info, All)

## Install Certificates

- To trust the certificate (Mac OSX and Windows only), run:
  ```bash
  dotnet dev-certs https --trust
  ```

## Debug

```bash
cd ClientApp/ && npm run build && cd .. && dotnet build && dotnet run
```

## Publish

- Debug
  ```bash
  dotnet publish -r osx-x64
  dotnet publish -r win-x64
  dotnet publish -r linux-x64
  ```
- Release
  ```bash
  dotnet publish -c Release -r osx-x64 -p:PublishReadyToRun=true
  dotnet publish -c Release -r win-x64 -p:PublishReadyToRun=true
  dotnet publish -c Release -r linux-x64 -p:PublishReadyToRun=true
  ```
