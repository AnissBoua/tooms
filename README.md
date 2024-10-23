#### Prerequisites
- .NET 8 SDK
- MySQL

#### Setup
Clone the repository:
``` bash
git clone https://github.com/your-repo/tooms.git
```

Navigate to the project directory:
```bash
cd tooms
```

Update the database connection string in `Program.cs`:
```cs
server=localhost;database=tooms;user=<user>;password=<password>;
```

#### Migrations: 
This command applies pending migrations to update your database schema.
``` bash
dotnet ef database update
```

#### CORS Configuration 
Ensure that the frontend's URL is properly configured to communicate with the backend. In `Program.cs`, add or modify the CORS policy as follows: 
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
Update the `WithOrigins` value based on the URL of your frontend.

## Authentication 
This project uses Microsoft authentication. Only two URLs are allowed for security reasons: 
- http://localhost:3000 
- https://tooms.anisse-bouainbi.fr/ 
If you need to use another URL, please contact the IT Service Administrator responsible for Azure.

## Running the Application  
To run the application locally:  
``` bash 
dotnet run
```

Navigate to [https://localhost:7164/swagger/index.html](https://localhost:7164/swagger/index.html) (or another specified port) in your browser to access the backend.

> [!WARNING]  
> The project need to be run on HTTPS for the WebSockets to work properly to do so please run the project on Visual Studio, they provide an execution on HTTPS