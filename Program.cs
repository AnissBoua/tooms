using Microsoft.EntityFrameworkCore;
using tooms.data;
using tooms.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDBContext>(options => {
    options.UseMySQL("server=localhost;database=tooms;user=anisse;password=dj68481935");
});

//Permet d'ajouter les class de /Services avec les Dependencies injections ;)
builder.Services.Scan(scan => scan
    .FromAssemblyOf<UserService>()
    .AddClasses(classes => classes.InNamespaceOf<UserService>())
    .AsSelfWithInterfaces()
    .WithScopedLifetime());

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("React",
        builder => {
            builder.WithOrigins("http://localhost:3000") // React app URL during development
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials(); // If you need to allow cookies, JWT, etc.
        });
});
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
app.MapControllers();
app.Run();
