# Windows Builds 

[ACR Build](https://aka.ms/acr/build) now supports Windows Containers. More interestingly, ACR Build supports all 2016 versions, including 1709, 1803 and ltsc2016 (Long Term Servicing Channel) for Windows Server Core and Nano Server

## Building Windows Containers with ACR Build
Use a version specific tag so you're assured you're building the image based on the version of windows you intend to deploy.
This is critical. If you're deployment environemnt only supports ltsc2016, building on 1709 or 1803 will fail when you attempt to run your built image. Windows container hosts must be the same version, or newer to run previous versions of Windows containers. 

## Chosing a version of Windows
Chosing a version of Windows is a balance between what version your production environemnt can currently support, and newer/smaller images. Windows has done a great job reducing the duplication of files in the base image, reducing the total image size. .NET has also done additional optimizations, further reducing the image sizes. Reducing the image size will reduce the build and deploy times as images are continually pulled across the network as newer base and higher level images are updated.

For more information, please see https://aka.ms/containercompat

To get a sense of the image sizes, below are representative images required for Windows Server and Nano Server. .NET Framework and .NET Core are used as representative uses of the versions of Windows.

### Windows Server Core, .NET Framework, ASP.NET
``` Bash
IMAGE ID            REPOSITORY                     TAG                                    SIZE
7d89a4baf66c        microsoft/windowsservercore    ltsc2016                               10.7GB
e2e283e28197        microsoft/windowsservercore    1709                                   6.39GB
fc9cd8b52f1a        microsoft/windowsservercore    1803                                   4.76GB

d467e2bc8d32        microsoft/dotnet-framework     4.7.2-sdk-windowsservercore-ltsc2016   14.9GB
9c85c0ccac79        microsoft/dotnet-framework     4.7.2-sdk-windowsservercore-1709       10.5GB
d15128e9f85c        microsoft/dotnet-framework     4.7.2-sdk-windowsservercore-1803       7.38GB
6d665d2c5480        microsoft/dotnet-framework     4.7.2-sdk                              7.17GB

a1ab67f36c66        microsoft/aspnet               4.7.2-windowsservercore-ltsc2016       13.6GB
a0428c1f673e        microsoft/aspnet               4.7.2-windowsservercore-1709           9.14GB
e7dcc3ca9b67        microsoft/aspnet               4.7.2-windowsservercore-1803           5.01GB
e7dcc3ca9b67        microsoft/aspnet               4.7.2                                  5.01GB

837277b7d31c        helloworld-windowsservercore   ltsc2016                               13.6GB
d819735d4142        helloworld-windowsservercore   1709                                   9.16GB
1f6748dcb31d        helloworld-windowsservercore   1803                                   5.04GB
```

### Windows Server Nano, .NET Core, ASP.NET Core

``` Bash
IMAGE ID            REPOSITORY                     TAG                                    SIZE
e2c314f76df6        microsoft/nanoserver           sac2016                                1.13GB
23ddb2d45ad9        microsoft/nanoserver           1709                                   329MB
3ba4e30fed90        microsoft/nanoserver           1803                                   337MB

1849ca4ef629        microsoft/aspnetcore-build     2.0-nanoserver-sac2016                 3.01GB
44618259ecd2        microsoft/aspnetcore-build     2.0-nanoserver-1709                    2.05GB
2af14f1cead8        microsoft/aspnetcore-build     2.0-nanoserver-1803                    2.06GB

9132983c48eb        microsoft/aspnetcore           2.0-nanoserver-sac2016                 1.29GB
db1a0a41a540        microsoft/aspnetcore           2.0-nanoserver-1709                    447MB
1c47c6da0e58        microsoft/aspnetcore           2.0-nanoserver-1803                    524MB

f192a627961b        helloworld-nanoserver          sac2016                                1.29GB
2b6110efa26e        helloworld-nanoserver          1709                                   449MB
89d27d5bd26c        helloworld-nanoserver          1803                                   527MB
```

### Defaulting the registry name:
To simplify your `acr build` commands, you can set a default registry name.
The following `acr build` commands assume you've set the following:

``` bash
az configure --defaults acr=[registryname]
```
example:
az configure --defaults acr=jengademos

## Windows Server Core
### multi-arch (version)
``` bash
# local docker build
docker build -t helloworld-windowsservercore:multi-arch -f aspnetmvcapp/multi-arch.Dockerfile .

# az acr build
az acr build -t helloworld-windowsservercore:multi-arch-{{.Build.ID}} -f aspnetmvcapp/multi-arch.Dockerfile --os windows https://github.com/AzureCR/acr-builder-os-tests.git#master:Windows/Servercore

# az acr build
az acr build-task create \
    -n helloworldwinservercoremultiarch \
    -t helloworld-windowsservercore:multi-arch-{{.Build.ID}} \
    -f helloworld-windowsservercore/multi-arch.Dockerfile \
    -c https://github.com/AzureCR/acr-builder-os-tests.git#master:Windows/Nanoserver \
    --os windows \
    --git-access-token $PAT

```

### ltsc2016
``` bash
# local docker build
docker build -t helloworld-windowsservercore:ltsc2016 -f aspnetmvcapp/ltsc2016.Dockerfile .

# az acr build
az acr build -t helloworld-windowsservercore:ltsc2016-{{.Build.ID}} -f aspnetmvcapp/ltsc2016.Dockerfile --os windows https://github.com/AzureCR/acr-builder-os-tests.git#master:Windows/Servercore

# az acr build
az acr build-task create \
    -n helloworldwinservercoreltsc2016 \
    -t helloworld-windowsservercore:ltsc2016-{{.Build.ID}} \
    -f helloworld-windowsservercore/ltsc2016.Dockerfile \
    -c https://github.com/AzureCR/acr-builder-os-tests.git#master:Windows/Nanoserver \
    --os windows \
    --git-access-token $PAT

```

### 1709
``` bash
# local docker build
docker build -t helloworld-windowsservercore:1709 -f aspnetmvcapp/1709.Dockerfile .

# az acr build
az acr build -t helloworld-windowsservercore:1709-{{.Build.ID}} -f aspnetmvcapp/1709.Dockerfile --os windows https://github.com/AzureCR/acr-builder-os-tests.git#master:Windows/Servercore

# az acr build
az acr build-task create \
    -n helloworldwinservercore1709 \
    -t helloworld-windowsservercore:1709-{{.Build.ID}} \
    -f helloworld-windowsservercore/1709.Dockerfile \
    -c https://github.com/AzureCR/acr-builder-os-tests.git#master:Windows/Nanoserver \
    --os windows \
    --git-access-token $PAT

```

### 1803
``` bash
# local docker build
docker build -t helloworld-windowsservercore:1803 -f aspnetmvcapp/1803.Dockerfile .

# az acr build
az acr build -t helloworld-windowsservercore:1803-{{.Build.ID}} -f aspnetmvcapp/1803.Dockerfile --os windows https://github.com/AzureCR/acr-builder-os-tests.git#master:Windows/Servercore

# acr build-task, triggered by git-commits and base image updates, such as windows server core
az acr build-task create \
    -n helloworldwinservercore1803 \
    -t helloworld-windowsservercore:1803-{{.Build.ID}} \
    -f helloworld-windowsservercore/1803.Dockerfile \
    -c https://github.com/AzureCR/acr-builder-os-tests.git#master:Windows/Nanoserver \
    --os windows \
    --git-access-token $PAT
```


## Windows Server Nano

### sac2016
``` bash
# local docker build
docker build -t helloworld-nanoserver:sac2016 -f helloworld-nanoserver/sac2016.Dockerfile .

# az acr build
az acr build -t helloworld-nanoserver:sac2016-{{.Build.ID}} -f helloworld-nanoserver/sac2016.Dockerfile  --os windows https://github.com/AzureCR/acr-builder-os-tests.git#master:Windows/Nanoserver 

az acr build-task create \
    -n helloworldnanoserversac2016 \
    -t helloworld-nanoserver:sac2016-{{.Build.ID}} \
    -f helloworld-nanoserver/sac2016.Dockerfile \
    -c https://github.com/AzureCR/acr-builder-os-tests.git#master:Windows/Nanoserver \
    --os windows \
    --git-access-token $PAT
```

### 1709
``` bash
# local docker build
docker build -t helloworld-nanoserver:1709 -f helloworld-nanoserver/1709.Dockerfile .

# az acr build
az acr build -t helloworld-nanoserver:1709-{{.Build.ID}} -f helloworld-nanoserver/1709.Dockerfile  --os windows https://github.com/AzureCR/acr-builder-os-tests.git#master:Windows/Nanoserver 

az acr build-task create \
    -n helloworldnanoserver1709 \
    -t helloworld-nanoserver:1709-{{.Build.ID}} \
    -f helloworld-nanoserver/1709.Dockerfile \
    -c https://github.com/AzureCR/acr-builder-os-tests.git#master:Windows/Nanoserver \
    --os windows \
    --git-access-token $PAT
```

### 1803
``` bash
# local docker build
docker build -t helloworld-nanoserver:1803 -f helloworld-nanoserver/1803.Dockerfile .

# az acr build
az acr build -t helloworld-nanoserver:1803-{{.Build.ID}} -f helloworld-nanoserver/1803.Dockerfile  --os windows https://github.com/AzureCR/acr-builder-os-tests.git#master:Windows/Nanoserver 

az acr build-task create \
    -n helloworldnanoserver1803 \
    -t helloworld-nanoserver:1803-{{.Build.ID}} \
    -f helloworld-nanoserver/1803.Dockerfile \
    -c https://github.com/AzureCR/acr-builder-os-tests.git#master:Windows/Nanoserver \
    --os windows \
    --git-access-token $PAT

```


