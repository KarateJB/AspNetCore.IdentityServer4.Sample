FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

# Copy source code and restore as distinct layers
COPY src/ ./
RUN dotnet restore
WORKDIR "/app/AspNetCore.IdentityServer4.WebApi"
RUN dotnet publish --configuration release --output "/app/publish"


FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime

# Arg for current environment, eg. Development, Docker, Production. Use docker-compose file to overwrite.
ARG env="Docker"

WORKDIR /app
COPY --from=build "/app/publish" ./
RUN mkdir -p /etc/docker/certs/ 
COPY ./docker/certs/${env}.crt /etc/docker/certs/
COPY ./docker/certs/${env}.key /etc/docker/certs/
COPY ./docker/certs/${env}.pfx /etc/docker/certs/
RUN mv /etc/docker/certs/${env}.pfx /etc/docker/certs/docker.pfx
VOLUME /app/App_Data/Logs

ENV TZ "Asia/Taipei"
# ENV LANG "zh_TW.UTF-8"
# ENV LANGUAGE "zh_TW.UTF-8"
# ENV LC_ALL "zh_TW.UTF-8"
ENV ASPNETCORE_URLS "http://+:5000;https://+:5001"
ENV ASPNETCORE_ENVIRONMENT ${env}
ENV ASPNETCORE_Kestrel__Certificates__Default__Password ""
ENV ASPNETCORE_Kestrel__Certificates__Default__Path "/etc/docker/certs/docker.pfx"
EXPOSE 5000 5001

ENTRYPOINT ["dotnet", "AspNetCore.IdentityServer4.WebApi.dll"]
