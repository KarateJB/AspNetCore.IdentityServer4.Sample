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

# Create in-memory initial config

```
$ cd AspNetCore.IdentityServer4.Auth
$ mkdir -p Utils/Config
$ touch Utils/Config/InMemoryInitConfig.cs
```