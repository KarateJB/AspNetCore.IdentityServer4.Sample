FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime

# Arg for current environment, eg. Development, Docker, Production. Use docker-compose file to overwrite.
ARG env="Docker"

WORKDIR /app
VOLUME /app/App_Data/Logs

RUN mkdir -p /etc/docker/certs/ 
COPY ./certs/${env}.pfx /etc/docker/certs/
RUN mv /etc/docker/certs/${env}.pfx /etc/docker/certs/docker.pfx
COPY ./build/auth ./

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
