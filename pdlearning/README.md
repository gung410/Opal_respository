# OPAL 2.0

## Before going to any detail of this project, please REMEMBER

* I, Toan Nguyen defined rules, so please don't argue if you're new to this project. **Our place, our rules!**
* We care to every detail of the source code like **spacing, comma, naming conventions and coding standards** so YOU (developers) should follow it strictly.

## How to setup RabbitMQ, Redis Cache, SQL Server locally

* Install Docker.
* Understand more about docker command: <https://docs.docker.com/compose/reference/up/>
* Run the following command:

``` bash
docker-compose -f .\docker-compose.yml -f .\docker-compose.override.yml up --build redis rabbitmq sqldata
```

## How to setup and start FULL backend services

* Install Docker.
* Run the following command:

``` bash
docker-compose -f .\docker-compose.yml -f .\docker-compose.override.yml up --build
```

To remove/cleanup volumes created by docker-compose:

``` bash
docker-compose down --volumes
```

## Build Docker images in parallel

If you want to build all images in parallel, please use:

``` bash
docker-compose -f .\docker-compose.yml -f .\docker-compose.override.yml build --parallel
```

Then you can up:

``` bash
docker-compose -f .\docker-compose.yml -f .\docker-compose.override.yml up
```

Notes:
* The flag --build is used for the fist time run, you need to build the image or when you have code-changes.
* Your need to set docker preference with computing resources of at least 2GB memory in order to run SQL Server
* RabbitMQ admin dashboard: http://localhost:15672


## Access control data migration guideline

Following [access control readme](/src/microservices/shared/Conexus.Opal.AccessControl/README.md)

## Common build tasks

There are several tasks that we usually use:

``` bash
.\build.cmd -r -rebuild /t:TASKNAME
```

The TASKNAME is described as below:

**Task**                            | **Description**                                                                    | **Sample**
------------------------------------|------------------------------------------------------------------------------------|-----------------------------------------
Cow                                 | Everyone should try it to see the gift!                                            | ```.\build.cmd -r /t:Cow```
GenerateProjectList                 | Generate ProjectReferences.props                                                   | ```.\build.cmd -r /t:GenerateProjectList```
GenerateDependenciesPropsFile       | Generate Versions.props                                                        | ```.\build.cmd -r /t:GenerateDependenciesPropsFile```

Sample:

``` bash
.\build.cmd /t:Cow
```

## Common build parameters

**Parameter**                       | **Description**
------------------------------------|------------------------------------------------------------------------------------
-bl                                 | Generate binlog file for debugging purpose. ```.\build.cmd -r -bl /t:Cow```
-Verbose                            | Write verbose log ```.\build.cmd -r /t:Cow -Verbose```
/verbosity:diagnostic               | Set diagnostic level for logging. ```.\build.cmd -r /t:Cow /verbosity:diagnostic```
