read -p 'Please provide the version number for tagging ' version
docker buildx build --platform linux/amd64,linux/arm64 . -t martinhillford/sefer-backend-api:latest -t martinhillford/sefer-backend-api:$version
echo "Tagged martinhillford/sefer-backend-api:latest"
echo "Tagged martinhillford/sefer-backend-api:$version"