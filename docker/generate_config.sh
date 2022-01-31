#!/usr/bin/env bash

set -o pipefail


if [ -f .env ]; then
  read -r -p "A config file exists and will be overwritten, are you sure you want to continue? [y/N] " response
  case $response in
    [yY][eE][sS]|[yY])
      mv .env .env_backup
      chmod 600 .env_backup
      ;;
    *)
      exit 1
    ;;
  esac
fi

if [ -a /etc/timezone ]; then
  DETECTED_TZ=$(cat /etc/timezone)
elif [ -a /etc/localtime ]; then
  DETECTED_TZ=$(readlink /etc/localtime|sed -n 's|^.*zoneinfo/||p')
fi

POSTGRE_PASSWORD=$(LC_ALL=C </dev/urandom tr -dc A-Za-z0-9 | head -c 28)
POSTGRE_USER="forkeat"

cat << EOF > .env
# ------------------------------
# General configuration
# ------------------------------

TZ=${DETECTED_TZ}

# ------------------------------
# PostgreSQL database configuration
# ------------------------------

POSTGRE_PASSWORD=${POSTGRE_PASSWORD}
POSTGRE_USER=${POSTGRE_USER}

# ------------------------------
# Fork-Eat backend configuration
# ------------------------------

DATABASE_URL=postgres://${POSTGRE_USER}:${POSTGRE_PASSWORD}@postgres:5432/forkeat
JWT_SECRET=$(LC_ALL=C </dev/urandom tr -dc A-Za-z0-9 | head -c 28)
EOF
