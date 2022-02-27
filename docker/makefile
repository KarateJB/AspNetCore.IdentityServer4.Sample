.PHONY: build
.PHONY: run
.PHONY: down
.PHONY: publish

build:
	docker-compose build --no-cache

run: build
	docker-compose up -d

down:
	docker-compose down

publish: build
	docker tag idsrv4-auth:latest karatejb/idsrv4-auth:latest
	docker tag idsrv4-backend:latest karatejb/idsrv4-backend:latest
	docker tag idsrv4-nginx:latest karatejb/idsrv4-nginx:latest
	docker tag idsrv4-health:latest karatejb/idsrv4-health:latest
	docker push karatejb/idsrv4-auth:latest
  docker push karatejb/idsrv4-backend:latest
	docker push karatejb/idsrv4-nginx:latest
	docker push karatejb/idsrv4-health:latest

