FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

# Copy source code and restore as distinct layers
COPY src/ ./src

# Cus the auth project links ..\..\docker\certs\Docker.pfx, so copy the docker directory as well
COPY docker/ ./docker

WORKDIR "/app/src/AspNetCore.IdentityServer4.Auth"
RUN dotnet restore
RUN dotnet publish --configuration Release --output "/app/publish"


FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime

# Arg for current environment, eg. Development, Docker, Production. Use docker-compose file to overwrite.
ARG env="Docker"

WORKDIR /app
COPY --from=build "/app/publish" ./
RUN mkdir -p /etc/docker/certs/ 
COPY docker/certs/${env}.pfx /etc/docker/certs/
RUN mv /etc/docker/certs/${env}.pfx /etc/docker/certs/docker.pfx
VOLUME /app/App_Data/Logs

ENV TZ "Asia/Taipei"
# ENV LANG "zh_TW.UTF-8"
# ENV LANGUAGE "zh_TW.UTF-8"
# ENV LC_ALL "zh_TW.UTF-8"
ENV ASPNETCORE_URLS "http://+:6000;https://+:6001"
ENV ASPNETCORE_ENVIRONMENT ${env}
ENV ASPNETCORE_Kestrel__Certificates__Default__Password ""
ENV ASPNETCORE_Kestrel__Certificates__Default__Path "/etc/docker/certs/docker.pfx"
EXPOSE 6000 6001

ENTRYPOINT ["dotnet", "AspNetCore.IdentityServer4.Auth.dll"]

LABEL org.opencontainers.image.source=https://github.com/karatejb/AspNetCore.IdentityServer4.Sample
