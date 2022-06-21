# products-inventory-minimal-api
A CRUD API using Minimal API

## Adding Migrations:

```bash
# PowerShell:
Add-Migrations InitialDatabase -Context ProductsContext -o "Infrastructure/Data/Migrations"
Add-Migrations InitialIdentity -Context IdentityContext -o "Infrastructure/Identity/Migrations"

Update-Database -Context ProductsContext
Update-Database -Context IdentityContext
```