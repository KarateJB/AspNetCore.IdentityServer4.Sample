FROM nginx:1.17.5

# Arg for current environment, eg. Development, Docker, Production. Use docker-compose file to overwrite.
ARG env="Docker"

COPY ./nginx/web-servers.conf /etc/nginx/sites-available/web-servers.conf
COPY ./nginx/nginx.conf /etc/nginx/nginx.conf
COPY ./certs/${env}.crt /etc/docker/certs/
COPY ./certs/${env}.key /etc/docker/certs/
RUN mv /etc/docker/certs/${env}.crt /etc/docker/certs/docker.crt
RUN mv /etc/docker/certs/${env}.key /etc/docker/certs/docker.key
RUN ln -s /etc/nginx/sites-available/web-servers.conf  /etc/nginx/conf.d/web-servers.conf

ENV TZ "Asia/Taipei"

EXPOSE 80 5000 5001 6000 6001