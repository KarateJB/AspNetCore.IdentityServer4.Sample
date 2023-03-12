FROM grafana/grafana:latest

ARG env="Docker"

COPY ./certs/${env}.crt /etc/grafana/docker.crt
COPY ./certs/${env}.key /etc/grafana/docker.key

# DO NOT use the following path inside container since the path is mounted.
# COPY ./certs/${env}.crt /var/lib/grafana/ssl
# COPY ./certs/${env}.key /var/lib/grafana/ssl

LABEL org.opencontainers.image.source=https://github.com/karatejb/AspNetCore.IdentityServer4.Sample
