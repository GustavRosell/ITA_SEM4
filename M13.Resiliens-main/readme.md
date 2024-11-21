# Resilient ASP.NET Core Service with Nginx Gateway

## Projektoversigt

Dette projekt demonstrerer en resilient opsætning for en ASP.NET Core-service, containeriseret med Docker og beskyttet af en Nginx API-gateway. Projektet omfatter to instanser af ASP.NET Core-servicen med forskellige konfigurationer, som håndteres via Nginx til både load balancing og circuit breaking.

## Projektstruktur

Her er en oversigt over projektets mappestruktur:

```plaintext
project-root/
├── CBService/              # ASP.NET Core service med Dockerfile
│   └── Dockerfile
├── DockerCompose/          # Docker Compose opsætning
│   ├── docker-compose.yml
│   └── nginx.conf          # Nginx-konfiguration
```
## Opsætning og Konfiguration

### 1. Docker Compose Opsætning

`docker-compose.yml` definerer følgende services:

- **servicea**: Første instans af ASP.NET Core-servicen med miljøvariablen `ToFail=yes`, som simulerer fejl.
- **serviceb**: Anden instans af ASP.NET Core-servicen med `ToFail=no`, som kører uden fejl.
- **nginx**: Nginx API-gateway, der balancerer belastningen og fungerer som en circuit breaker.

### 2. Nginx-konfiguration (`nginx.conf`)

Nginx-konfigurationen definerer en upstream-pulje `svc` med `servicea` og `serviceb`:

- Nginx sender først anmodninger til `servicea`.
- Hvis `servicea` fejler (returnerer HTTP 503), skifter Nginx automatisk til `serviceb`.
- Direktivet `proxy_next_upstream` sikrer, at Nginx reagerer på fejlkoder, tidsouts og andre fejl ved at skifte til en anden serviceinstans.

## Kom i Gang

For at køre projektet, følg disse trin:

1. **Klon projektet** og navigér til `DockerCompose`-mappen.
2. **Start containerne** med Docker Compose:

    ```bash
    docker-compose up -d --build
    ```

3. **Test API-gatewayen** ved at sende en anmodning til Nginx på `http://localhost:4000/CBService/GetService`:

    ```bash
    curl http://localhost:4000/CBService/GetService
    ```

Nginx vil automatisk balancere trafikken mellem `servicea` og `serviceb` og fungerer som en circuit breaker ved at skifte til `serviceb`, hvis `servicea` fejler.

## Observér Circuit Breaking

Når `servicea` fejler, vil Nginx videresende anmodningen til `serviceb` uden at returnere en fejl til klienten. For at ændre fejlkriteriet kan `proxy_next_upstream`-direktivet i `nginx.conf` justeres til at skifte ved andre fejlkoder (f.eks. `http_500`).



EXTRA note:  
docker compose up -d  
docker compose down  
curl http://localhost:4000/CBService/GetService
