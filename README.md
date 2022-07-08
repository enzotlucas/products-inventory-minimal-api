# Products Inventoy Minimal API (.net 6)
A showcase of a simple CRUD Minimal API project

## Preparing the environment:
First, we need to create the database, using this commands:

```bash
# PowerShell:
Add-Migration InitialDatabase -Context ProductsContext -o "Infrastructure/Data/Migrations"
Add-Migration InitialIdentity -Context IdentityContext -o "Infrastructure/Identity/Migrations"

Update-Database -Context ProductsContext
Update-Database -Context IdentityContext
```

## Documentation
Visit [Wiki](https://github.com/enzotlucas/products-inventory-minimal-api/wiki) page for documentation.

## Give a Star 🌟
If you liked the project, please give a star.
