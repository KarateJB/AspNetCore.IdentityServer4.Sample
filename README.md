# Tutorials

- [[OpenLDAP] Create an OpenLDAP container](https://karatejb.blogspot.com/2019/07/openldap-create-openldap-container.html)
- [[ASP.NET Core] Identity Server 4 – LDAP authentication](https://karatejb.blogspot.com/2019/07/aspnet-core-identity-server-4-ldap.html)
- [[ASP.NET Core] Identity Server 4 – Secure WebAPI]()



# Create dotnet project

```
$ dotnet new webapi --name AspNetCore.IdentityServer4.Auth
$ dotnet new sln --name AspNetCore.IdentityServer4
$ dotnet sln AspNetCore.IdentityServer4.sln add AspNetCore.IdentityServer4.Auth/AspNetCore.IdentityServer4.Auth.csproj
```

# Install packages

```
$ cd AspNetCore.IdentityServer4.Auth
$ dotnet add package IdentityServer4 --version 2.4.0
$ dotnet add package IdentityServer.LdapExtension --version 2.1.8
```

# Update appsettings.json file

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