# ACR Build for Windows
Note! This is preview documentation. Official docs will soon be completed and pushed to https://aka.ms/acr/docs

[ACR Build](https://aka.ms/acr/build) now supports Windows Containers. More interestingly, ACR Build supports all 2016 versions, including 1709, 1803 and ltsc2016 (Long Term Servicing Channel) for Windows Server Core and Nano Server

## Building Windows Containers with ACR Build
As a best practice, we recommend using  a version specific tag so you're assured you're building the image based on the version of Windows you intend to deploy.
This is fairly critical. If the deployment environment only supports ltsc2016, building an image on 1709 or 1803 will fail when a newer versioned image is run on an older version of the host. Windows container hosts must be the same version, or newer to run previous versions of Windows containers. 

## Choosing a version of Windows
Choosing a version of Windows is a balance between what version the production environment can currently support, and newer/smaller images. Windows has done a great job reducing the duplication of files in the base image, reducing the total image size. .NET has also done additional optimizations, further reducing the image sizes. Reducing the image size will reduce the build and deploy times as images are continually pulled across the network as newer base and higher-level images are updated.

For more information, please see https://aka.ms/containercompat

## Windows Server Image Versions

To get a sense of the image sizes, below are representative images required for Windows Server and Nano Server. .NET Framework and .NET Core are used as representative uses of Windows.

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

# Building Windows images with ACR Build
The following commands are provided to test and iterate with various approaches. These include `az acr build` for *quick builds*, meaning you can request a build that isn't based on a git commit. You can build from a local directory, or point **acr build** to an existing git repo. This is the equivalent of using `docker build`, except docker isn't required on the machine you're issuing `acr build` from.

`acr build-task` represents a build definition that's awaiting changes. By default, **acr build** will trigger on git-commits and runtime base image updates. For more information, see https://aka.ms/acr/build

## Acquiring Windows Support for the AZ CLI
To use ACR Build for Windows, as of 7/12/18, use the docker instructions for running the CLI at: https://docs.microsoft.com/en-us/cli/azure/run-azure-cli-docker?view=azure-cli-latest

However, use: `docker pull azurecr/azure-cli` as the Windows support hasn't yet been merged with the public **az cli**


## Defaulting the registry name:
To simplify your `acr build` commands, you can set a default registry name.
The following `acr build` commands assume you've set the following:

``` bash
az configure --defaults acr=[registryname]
```
example:
az configure --defaults acr=jengademos

## Getting a personal access token

The default for build-task requires a git-access-token. This will configure a webhook on the repository to trigger your build-task when commits are made. 

To use ACR Build with a repository you don't own, such as https://github.com/AzureCR/acr-builder-os-tests, you can pass the additional flag `--commit-trigger-enabled=false` which removes the dependency on a personal access token. As a result, the build-task will not automatically triggger on commits, but you can manually start the build task: `az acr build-task run -n HelloworldWinServercoreLtsc2016`

See: [create-a-build-task](https://docs.microsoft.com/en-us/azure/container-registry/container-registry-tutorial-build-task#create-a-build-task) in docs for configuring a personal access token. 

```bash
PAT=[token]
```

## Windows Server Core
### multi-arch (version)
``` bash
# local docker build
docker build \
    -t helloworld-windowsservercore:multi-arch \
    -f helloworld-windowsservercore/multi-arch.Dockerfile \
    .

# az acr build
az acr build \
    -t helloworld-windowsservercore:multi-arch-{{.Build.ID}} \
    -f helloworld-windowsservercore/multi-arch.Dockerfile \
    --os windows \
    https://github.com/AzureCR/acr-builder-os-tests.git#master:Windows/Servercore

# acr build-task, triggered by git-commits and base image updates, such as windows server core
az acr build-task create \
    -n HelloworldWinServercoreMultiarch \
    -t helloworld-windowsservercore:multi-arch-{{.Build.ID}} \
    -f helloworld-windowsservercore/multi-arch.Dockerfile \
    -c https://github.com/AzureCR/acr-builder-os-tests.git#master:Windows/Servercore \
    --os windows \
    --git-access-token $PAT
```

### ltsc2016
``` bash
# local docker build
docker build \
    -t helloworld-windowsservercore:ltsc2016 \
    -f helloworld-windowsservercore/ltsc2016.Dockerfile \
    .

# az acr build
az acr build \
    -t helloworld-windowsservercore:ltsc2016-{{.Build.ID}} \
    -f helloworld-windowsservercore/ltsc2016.Dockerfile \
    --os windows \
    https://github.com/AzureCR/acr-builder-os-tests.git#master:Windows/Servercore

# acr build-task, triggered by git-commits and base image updates, such as windows server core
az acr build-task create \
    -n HelloworldWinServercoreLtsc2016 \
    -t helloworld-windowsservercore:ltsc2016-{{.Build.ID}} \
    -f helloworld-windowsservercore/ltsc2016.Dockerfile \
    -c https://github.com/AzureCR/acr-builder-os-tests.git#master:Windows/Servercore \
    --os windows \
    --git-access-token $PAT
```

### 1709
``` bash
# local docker build
docker build \
    -t helloworld-windowsservercore:1709 \
    -f helloworld-windowsservercore/1709.Dockerfile \
    .

# az acr build
az acr build \
    -t helloworld-windowsservercore:1709-{{.Build.ID}} \
    -f helloworld-windowsservercore/1709.Dockerfile \
    --os windows \
    https://github.com/AzureCR/acr-builder-os-tests.git#master:Windows/Servercore

# acr build-task, triggered by git-commits and base image updates, such as windows server core
az acr build-task create \
    -n HelloworldWinServercore1709 \
    -t helloworld-windowsservercore:1709-{{.Build.ID}} \
    -f helloworld-windowsservercore/1709.Dockerfile \
    -c https://github.com/AzureCR/acr-builder-os-tests.git#master:Windows/Servercore \
    --os windows \
    --git-access-token $PAT
```

### 1803
``` bash
# local docker build
docker build \
    -t helloworld-windowsservercore:1803 \
    -f helloworld-windowsservercore/1803.Dockerfile \
    .

# az acr build
az acr build \
    -t helloworld-windowsservercore:1803-{{.Build.ID}} \
    -f helloworld-windowsservercore/1803.Dockerfile \
    --os windows \
    https://github.com/AzureCR/acr-builder-os-tests.git#master:Windows/Servercore

# acr build-task, triggered by git-commits and base image updates, such as windows server core
az acr build-task create \
    -n HelloworldWinServercore1803 \
    -t helloworld-windowsservercore:1803-{{.Build.ID}} \
    -f helloworld-windowsservercore/1803.Dockerfile \
    -c https://github.com/AzureCR/acr-builder-os-tests.git#master:Windows/Servercore \
    --os windows \
    --git-access-token $PAT
```


## Windows Server Nano

### Nanoserver Multi-Arch
``` bash
# local docker build
docker build \
    -t helloworld-nanoserver:multi-arch \
    -f helloworld-nanoserver/multi-arch.Dockerfile \
    .

# az acr build
az acr build \
    -t helloworld-nanoserver:multi-arch-{{.Build.ID}} \
    -f helloworld-nanoserver/multi-arch.Dockerfile  \
    --os windows \
    https://github.com/AzureCR/acr-builder-os-tests.git#master:Windows/Nanoserver 

az acr build-task create \
    -n HelloworldNanoServerMultiarch \
    -t helloworld-nanoserver:multi-arch-{{.Build.ID}} \
    -f helloworld-nanoserver/multi-arch.Dockerfile \
    -c https://github.com/AzureCR/acr-builder-os-tests.git#master:Windows/Nanoserver \
    --os windows \
    --git-access-token $PAT
```

### sac2016
``` bash
# local docker build
docker build \
    -t helloworld-nanoserver:sac2016 \
    -f helloworld-nanoserver/sac2016.Dockerfile \
    .

# az acr build
az acr build \
    -t helloworld-nanoserver:sac2016-{{.Build.ID}} \
    -f helloworld-nanoserver/sac2016.Dockerfile  \
    --os windows \
    https://github.com/AzureCR/acr-builder-os-tests.git#master:Windows/Nanoserver 

az acr build-task create \
    -n HelloworldNanoServerSac2016 \
    -t helloworld-nanoserver:sac2016-{{.Build.ID}} \
    -f helloworld-nanoserver/sac2016.Dockerfile \
    -c https://github.com/AzureCR/acr-builder-os-tests.git#master:Windows/Nanoserver \
    --os windows \
    --git-access-token $PAT
```

### 1709
``` bash
# local docker build
docker build \
    -t helloworld-nanoserver:1709 \
    -f helloworld-nanoserver/1709.Dockerfile \
    .

# az acr build
az acr build \
    -t helloworld-nanoserver:1709-{{.Build.ID}} \
    -f helloworld-nanoserver/1709.Dockerfile  \
    --os windows \
    https://github.com/AzureCR/acr-builder-os-tests.git#master:Windows/Nanoserver 

az acr build-task create \
    -n HelloworldNanoServer1709 \
    -t helloworld-nanoserver:1709-{{.Build.ID}} \
    -f helloworld-nanoserver/1709.Dockerfile \
    -c https://github.com/AzureCR/acr-builder-os-tests.git#master:Windows/Nanoserver \
    --os windows \
    --git-access-token $PAT
```

### 1803
``` bash
# local docker build
docker build \
    -t helloworld-nanoserver:1803 \
    -f helloworld-nanoserver/1803.Dockerfile \
    .

# az acr build
az acr build \
    -t helloworld-nanoserver:1803-{{.Build.ID}} \
    -f helloworld-nanoserver/1803.Dockerfile  \
    --os windows \
    https://github.com/AzureCR/acr-builder-os-tests.git#master:Windows/Nanoserver 

az acr build-task create \
    -n HelloworldNanoServer1803 \
    -t helloworld-nanoserver:1803-{{.Build.ID}} \
    -f helloworld-nanoserver/1803.Dockerfile \
    -c https://github.com/AzureCR/acr-builder-os-tests.git#master:Windows/Nanoserver \
    --os windows \
    --git-access-token $PAT
```


## ACR and ACR Build Feedback
For feedback, please review:
- [ACR Roadmap: https://aka.ms/acr/roadmap](https://aka.ms/acr/roadmap)

Then file a topic via: 
- [Feedback: http://aka.ms/acr/feedback](http://aka.ms/acr/feedback)
- [Issues/Bugs: http://aka.ms/acr/issues](http://aka.ms/acr/issues)
- [Requests: https://aka.ms/acr/uservoice](https://aka.ms/acr/uservoice)
