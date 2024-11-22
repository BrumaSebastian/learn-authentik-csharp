# learn-authentik-csharp

Learn to use an authentication provider

# This Section will include all things that I learned:

## General

- command to create gitignore: `dotnet new gitignore`

## VS Code

- hid obj/bin files with this setting in Preferences: Open Settings (JSON):

  ```
  "files.exclude": {
    "**/bin": true,
    "**/obj": true
  }

  ```

## Authentik

- used with docker desktop

Step 1.

> https://docs.goauthentik.io/docs/installation/docker-compose

- download [docker image](https://goauthentik.io/docker-compose.yml)
- run these commands to create env file:

```
echo "PG_PASS=$(openssl rand -base64 36 | tr -d '\n')" >> .env
echo "AUTHENTIK_SECRET_KEY=$(openssl rand -base64 60 | tr -d '\n')" >> .env
```

- compose the containers

```
docker compose up -d
```

- when first starting the docker, a configuration for an admin account is required:
  > http://localhost:9000/if/flow/initial-setup/

![after succesfully setup of the admin account](/setup/image-2.png)

- Press 'Create With Wizard'

