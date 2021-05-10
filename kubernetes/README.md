# Minikub

## Install Minikub

> Reference: [minikub start](https://minikube.sigs.k8s.io/docs/start/)

```s
$ curl -LO https://storage.googleapis.com/minikube/releases/latest/minikube-linux-amd64
$ sudo install minikube-linux-amd64 /usr/local/bin/minikube
$ apt-get install -y conntrack 
```

## Start Minikub

```s
$ minikub start --driver=none
```


## Publish Docker images


### Build Docker images and push

Under the root path of the repository, run the following commands,

```s
$ docker build -t karatejb/idsrv4-auth -f docker\auth.dockerfile .
$ docker build -t karatejb/idsrv4-backend -f docker\backend.dockerfile .
$ docker build -t karatejb/idsrv4-nginx -f docker\nginx.dockerfile .
$ docker push karatejb/idsrv4-auth:latest
$ docker push karatejb/idsrv4-backend:latest
$ docker push karatejb/idsrv4-nginx:latest
```

Or use [Github Action]().



