#### Prérequis
- .NET 8 SDK
- MySQL

> [!WARNING]  
> Le projet doit être exécuté en HTTPS pour que les WebSockets fonctionnent correctement. Pour ce faire, veuillez exécuter le projet sur Visual Studio, qui permet une exécution en HTTPS.

#### Setup
Cloner le dépôt:
``` bash
git clone https://github.com/your-repo/tooms.git
```

Naviguer vers le répertoire du projet:
```bash
cd tooms
```

Mettre à jour la chaîne de connexion à la base de données dans `Program.cs`:
```cs
server=localhost;database=tooms;user=<user>;password=<password>;
```

#### Migrations: 
Cette commande applique les migrations en attente pour mettre à jour votre schéma de base de données.
``` bash
dotnet ef database update
```

#### Configuration CORS
Assurez-vous que l'URL du frontend est correctement configurée pour communiquer avec le backend. Dans `Program.cs`, ajoutez ou modifiez la politique CORS comme suit: 
``` cs
builder.Services.AddCors(options => {
    options.AddPolicy("React",
        builder => {
            builder.WithOrigins("http://localhost:3000") // React app URL during development
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials(); // If you need to allow cookies, JWT, etc.
        });
});
```
Mettez à jour la valeur de `WithOrigins` en fonction de l'URL de votre frontend.

## Authentification
Ce projet utilise l'authentification Microsoft. Seules deux URL sont autorisées pour des raisons de sécurité :
- http://localhost:3000 
- https://tooms.anisse-bouainbi.fr/ 
Si vous avez besoin d'utiliser une autre URL, veuillez contacter l'administrateur des services informatiques responsable d'Azure.

## Exécution de l'application
Pour exécuter l'application localement:  
``` bash 
dotnet run
```

Accédez à [https://localhost:7164/swagger/index.html](https://localhost:7164/swagger/index.html) (ou un autre port spécifié) dans votre navigateur pour accéder au backend.