# Prerequisites
- dotnet sdk 10.0
- docker
- `dotnet tool install -g Aspire.Cli --prerelease`

# How to deploy
- `dotnet publish /t:PublishContainer`
- `aspire publish -o deploy/`
- `cd .\deploy\`
- `docker compose up`