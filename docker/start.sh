set -o xtrace
#!/bin/bash

# Create default .env file for docker-compose
cp .env.template .env

docker compose -f docker-compose.yml up -d --build --force-recreate

# Wait for fhir-api to be healthy and available

timeout=120
interval=5
elapsed=0

echo "Waiting for fhir-api to be healthy..."
while [[ "$(curl -s -o /dev/null -w '%{http_code}' localhost:8080/metadata)" != "200" && $elapsed -lt $timeout ]]; do
    echo "Still waiting... Elapsed time: $elapsed seconds"
    sleep $interval
    ((elapsed+=interval))
done

if [ $elapsed -ge $timeout ]; then
    echo "Timed out waiting for fhir-api to be healthy. Exiting."
    docker-compose -f docker-compose.yml logs
    docker-compose -f docker-compose.yml ps
    exit 1
fi

echo "fhir-api is healthy and available!"
