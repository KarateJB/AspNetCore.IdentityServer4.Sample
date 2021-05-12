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


### Creating Docker registry's credential to Secret

> See [Creating a secret with a Docker config](https://kubernetes.io/docs/concepts/containers/images/#creating-a-secret-with-a-docker-config)

```s
$ kubectl create secret docker-registry <secret_name> --docker-server=<docker_registry_host> --docker-username=<user_name> --docker-password=<password> --docker-email=<email_addr>
```


### Use appsettings.Kubernetes.json file

```s
$ kubectl create secret generic secret-appsettings-auth --from-file=./artifects/auth/appsettings.Kubernetes.json
$ kubectl create secret generic secret-appsettings-backend --from-file=./artifects/backend/appsettings.Kubernetes.json
$ kubectl create secret generic secret-js-appconfig-backend --from-file=./artifects/backend/app-config.js
```

To edit the secret (base64 encoded string):

```s
$ kubectl edit secrets <secret_name>
```


To delete the secret:

```s
$ kubectl get secrets
NAME                           TYPE                                  DATA   AGE
secret-appsettings-auth        Opaque                                1      7h58m
secret-appsettings-backend     Opaque                                1      7h57m
secret-js-appconfig-backend    Opaque                                1      7h57m

$ kubectl delete secrets secret-appsettings-auth
$ kubectl delete secrets secret-appsettings-backend
$ kubectl delete secrets secret-js-appconfig-backend
```



```s
kubectl logs idsrv-backend-5c6494dfb4-jqxwg -c idsrv-backend
kubectl exec -it idsrv-auth-7588488bd7-7b5mw -- bash
kubectl create -f kubernetes-idsrv-deployments.yml
kubectl port-forward --address 192.168.107.137 idsrv-auth-7588488bd7-9dfbf 6001:6001
kubectl port-forward --address 192.168.107.137 idsrv-backend-5c6494dfb4-bnpr9 5001:5001
```