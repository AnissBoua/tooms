using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using tooms.data;
using tooms.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDBContext>(options => {
    options.UseMySQL("server=localhost;database=tooms;user=root;password=");
});

//Permet d'ajouter les class de /Services avec les Dependencies injections ;)
builder.Services.Scan(scan => scan
    .FromAssemblyOf<UserService>()
    .AddClasses(classes => classes.InNamespaceOf<UserService>())
    .AsSelfWithInterfaces()
    .WithScopedLifetime());

builder.Services.AddCors(options => {
    options.AddPolicy("React",
        builder => {
            builder.WithOrigins("http://localhost:3000") // React app URL during development
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials(); // If you need to allow cookies, JWT, etc.
        });
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseWebSockets();

// Use the CORS policy
app.UseCors("React");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable authentication and authorization middleware
app.UseAuthentication();
app.MapControllers();
app.UseAuthorization();
app.Run();
