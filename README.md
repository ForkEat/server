# ForkEat server

[![codecov](https://codecov.io/gh/ForkEat/server/branch/main/graph/badge.svg?token=OW6YBGZ2P0)](https://codecov.io/gh/ForkEat/server)

[![Build Status](https://drone.thomaslacaze.fr/api/badges/ForkEat/server/status.svg)](https://drone.thomaslacaze.fr/ForkEat/server)

## 🛠️ Installation Steps

### 🐳 Option 1: Run from Docker run
```
Run the container
$ docker run -d \
  -p 80:80 \
  -p 443:443 \
  -e "DATABASE_URL=postgres://****:****@****:5432/****" \
  -e "JWT_SECRET=<RANDOM_STRING>"
  --name forkeat-server thomaslacaze/forkeat-server:linux-amd64
```
### 🐳 Option 2: Run from Docker-compose

**See [here](https://github.com/ForkEat/server/blob/main/docker/docker-compose.yml)** 

