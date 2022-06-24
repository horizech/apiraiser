FROM mcr.microsoft.com/dotnet/aspnet:5.0-focal AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

#ENV NODE_OPTIONS=--openssl-legacy-provider

ENV ASPNETCORE_URLS=http://*:5000

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

#FROM node as node-builder

FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build
WORKDIR /src
COPY ["Apiraiser.csproj", "./"]
RUN dotnet restore "Apiraiser.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Apiraiser.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Apiraiser.csproj" -c Release -o /app/publish

# Not using React build at the moment, copy a prebuilt build from ClientApp/build to /app/ClientApp/build
# #React build
# FROM node as nodebuilder

# # set working directory
# RUN mkdir /usr/src/app
# WORKDIR /usr/src/app

# # add `/usr/src/app/node_modules/.bin` to $PATH
# ENV PATH /usr/src/app/node_modules/.bin:$PATH


# # install and cache app dependencies
# COPY ClientApp/package.json /usr/src/app/package.json
# RUN export NODE_OPTIONS=--openssl-legacy-provider
# RUN npm install

# # add app

# COPY ClientApp/. /usr/src/app

# RUN npm run build

# #End React build

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir -p /app/ClientApp/build
# COPY --from=nodebuilder /usr/src/app/build/. /app/ClientApp/build/
COPY ClientApp/build/. /app/ClientApp/build/
ENTRYPOINT ["dotnet", "Apiraiser.dll"]
