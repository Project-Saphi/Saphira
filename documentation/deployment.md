[<- Back](../README.md)

# Deployment

## Table of Contents

- [Overview](#overview)
- [Creation](#creation)
- [Updating](#updating)

## Overview


Deploying Saphira is quite straightforward: 

- Make sure you have [Docker](https://www.docker.com/products/docker-desktop/) installed
- Make sure you have performed all the necessary steps listed under [Setup](./setup.md)

Once you've done that, you can deploy Saphira.

## Creation

To deploy Saphira, just run the `docker-compose.yml` file under `deploy`:

```bash
$ docker-compose -f docker-compose.yml up
```

The `Dockerfile` will install build a container that contains all the necessary dependencies, so Saphira will just run inside the docker container. 

And that's it, you've successfully deployed Saphira!

## Updating

Saphira is relatively stateless and all necessary states are maintained in-memory. Restarting Saphira and losing all states is not an issue since only a handful states are kept in-memory, such as command cooldowns. These states can easily be discarded.

To update Saphira, just pull the newest commit and then update the docker container:

```bash
$ docker-compose -f docker-compose.yml down
$ docker-compose -f docker-compose.yml up
```
