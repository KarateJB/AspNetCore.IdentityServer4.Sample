# Deploy to Kubernetes

## Publish Docker images


### Build Docker images and push

Under the root path of the repository, run the following commands:

```s
$ docker build -t <Docker_ID>/idsrv4-auth -f docker\auth.dockerfile .
$ docker build -t <Docker_ID>/idsrv4-backend -f docker\backend.dockerfile .
$ docker push <Docker_ID>/idsrv4-auth:latest
$ docker push <Docker_ID>/idsrv4-backend:latest
```

Or use [Github Action](https://karatejb.blogspot.com/2021/05/github-action-docker-image.html).



## Create namespace

```s
$ kubectl create namespace idsrv-demo
```

Or use the yaml file:

```s
$ kubectl apply -f kubernetes-namespace.yaml
namespace/idsrv-demo created
```


## Secrets

### Creating Docker registry's credential to Secret

> See [Creating a secret with a Docker config](https://kubernetes.io/docs/concepts/containers/images/#creating-a-secret-with-a-docker-config)

```s
$ kubectl create secret docker-registry <secret_name> --docker-server=<docker_registry_host> --docker-username=<user_name> --docker-password=<password> --docker-email=<email_addr> --namespace idsrv-demo
```


### Use appsettings.Kubernetes.json file

```s
$ cd kubernetes
$ kubectl create secret generic secret-appsettings-auth --from-file=./artifects/auth/appsettings.Kubernetes.json --namespace idsrv-demo
$ kubectl create secret generic secret-appsettings-backend --from-file=./artifects/backend/appsettings.Kubernetes.json --namespace idsrv-demo
$ kubectl create secret generic secret-js-appconfig-backend --from-file=./artifects/backend/app-config.js --namespace idsrv-demo
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




## Create/Apply Services

```s
$ cd kubernetes
$ kubectl apply -f kubernetes-idsrv-deployments.yaml --namespace idsrv-demo
```

## Remove Resources

### Delete all resources in namespace

```s
$ kubectl delete all --all --namespace idsrv-demo
```

or delete certain resources by type,
```s
$ kubectl delete deploy,service,pod,pvc,pv --all --namespace idsrv-demo
```

## Remove Namespace

```s
$ kubectl delete namespace idsrv-demo
```


## Trouble Shooting

### Debug command

```s
$ kubectl get events --all-namespaces  --sort-by='.metadata.creationTimestamp'
$ kubectl get events --namespace idsrv-demo  --sort-by='.metadata.creationTimestamp'
```

or

```s
$ kubectl logs <pod_name> [-c <container_name>] --namespace idsrv-demo
$ kubectl describe pods <pod_name> --namespace idsrv-demo
```


Use the following command to enter the container,

```s
$ kubectl exec -it <pod_name> -- bash
```


