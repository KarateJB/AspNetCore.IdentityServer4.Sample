FROM prom/prometheus:v2.33.1

ARG env="Docker"

RUN mkdir /etc/prometheus/certs
COPY ./prometheus/web-config.yml /etc/prometheus/
COPY ./prometheus/prometheus.yml /etc/prometheus/
COPY ./certs/${env}.crt /home/prometheus/certs/docker.crt
COPY ./certs/${env}.key /home/prometheus/certs/docker.key

LABEL org.opencontainers.image.source=https://github.com/karatejb/AspNetCore.IdentityServer4.Sample
