[![License](https://img.shields.io/badge/License-BSD%203--Clause-blue.svg)](https://opensource.org/licenses/BSD-3-Clause)

# Tutorials

- [[OpenLDAP] Create an OpenLDAP container](https://karatejb.blogspot.com/2019/07/openldap-create-openldap-container.html)
- [[ASP.NET Core] Identity Server 4 – LDAP authentication](https://karatejb.blogspot.com/2019/07/aspnet-core-identity-server-4-ldap.html)
- [[ASP.NET Core] Identity Server 4 – Secure WebAPI](https://karatejb.blogspot.com/2019/07/aspnet-core-identity-server-4-secure.html)
- [[ASP.NET Core] Identity Server 4 – Custom EventSink](https://karatejb.blogspot.com/2019/07/aspnet-core-identity-server-4-secure.html)
- [[ASP.NET Core] Identity Server 4 – Refresh Token](https://karatejb.blogspot.com/2019/09/aspnet-core-identity-server-4-refresh.html)
- [[ASP.NET Core] Identity Server 4 – Role based authorization](https://karatejb.blogspot.com/2019/10/aspnet-core-identity-server-4-role.html)
- [[ASP.NET Core] Identity Server 4 – Policy based authorization](https://karatejb.blogspot.com/2019/10/aspnet-core-identity-server-4-policy.html)


# Create New Poject

## Create dotnet project

```
$ dotnet new webapi --name AspNetCore.IdentityServer4.Auth
$ dotnet new sln --name AspNetCore.IdentityServer4
$ dotnet sln AspNetCore.IdentityServer4.sln add AspNetCore.IdentityServer4.Auth/AspNetCore.IdentityServer4.Auth.csproj
```

```
$ dotnet new webapi --name AspNetCore.IdentityServer4.WebApi
$ dotnet sln AspNetCore.IdentityServer4.sln add AspNetCore.IdentityServer4.WebApi/AspNetCore.IdentityServer4.WebApi.csproj
```

## Install packages

```
$ cd AspNetCore.IdentityServer4.Auth
$ dotnet add package IdentityServer4 --version 2.4.0
$ dotnet add package IdentityServer.LdapExtension --version 2.1.8
```

## Update appsettings.json file

Update the following config to connect to your own OpenOLAP service.

```
"LdapServer": {
    "Url": "localhost",
    "Port": 389,
    "Ssl": false,
    "BindDn": "cn=admin,dc=example,dc=org",
    "BindCredentials": "admin",
    "SearchBase": "dc=example,dc=org",
    "searchFilter": "(&(objectClass=person)(uid={0}))"
  }
```


# Run The Exist Project


## Restore packages

```
$ cd src
$ dotnet restore
```


## (Optional) Use Gulp to Run multple applications in the same time

You can use gulp to run Auth Serice and API Service in the same time 

1. Install package globally 

```
$ npm install -g gulp
$ npm install -g gulp-exec
```

2. Create npm link locally

```
$ npm link gulp
$ npm link gulp-exec
```

3. Run applications

```
$ gulp run
```

or run Auth Service(`auth`) or API Service(`webapi`) individually.

```
$ gulp run auth
$ gulp run webapi
```

# Run on docker

## Restore packages

```
$ cd src
$ dotnet restore
```


## Build the projects

1. Auth server

```
$ cd src/AspNetCore.IdentityServer4.Auth
$ dotnet publish --output ../../docker/build/auth --configuration release
```

2. Backend (Web API)

```
$ cd src/AspNetCore.IdentityServer4.WebApi
$ dotnet publish --output ../../docker/build/backend --configuration release
```


## Build Docker images and start containers

```
$ cd docker
$ docker-compose build [--no-cache]
$ docker-compose up -d
```


# Reference

- [OpenLDAP – How To Add a User](https://tylersguides.com/guides/openldap-how-to-add-a-user/)
- [OpenLdap: How to create users in command line?](https://github.com/osixia/docker-openldap/issues/227#issuecomment-431375243)


