# Products Inventoy Minimal API (.NET 6)
A showcase of a simple CRUD Minimal API project

## Preparing the environment:
### Visual Studio
If you are using Visual Studio, execute the commands below on the Package Manager Console. </br>
To open the console, go to Tools > Library Package Manager > Package Manager Console.

```bash
# PowerShell:
Add-Migration InitialDatabase -Context ProductsContext -o "Infrastructure/Data/Migrations"
Add-Migration InitialIdentity -Context IdentityContext -o "Infrastructure/Identity/Migrations"

Update-Database -Context ProductsContext
Update-Database -Context IdentityContext
```
Then, you are able to run the project.

-------------------------------------------------------------------

### Visual Studio Code
If you are using Visual Studio Code, execute the commands below on the terminal.
```bash
# .Net CLI:
dotnet ef migrations add InitialDatabase -c ProductsContext -o "Infrastructure/Data/Migrations"
dotnet ef migrations add InitialIdentity -c IdentityContext -o "Infrastructure/Identity/Migrations"

dotnet ef database update -c ProductsContext
dotnet ef database update -c IdentityContext
```
Then, you are able to run using the command:
```bash
dotnet run --project ProductsInventory.API
```

The project will open the Swagger on localhost:5125/swagger/index.html

-------------------------------------------------------------------

## Documentation
Visit [Wiki](https://github.com/enzotlucas/products-inventory-minimal-api/wiki) page for documentation.

-------------------------------------------------------------------

## Give a Star 🌟
If you liked the project, please give a star.
