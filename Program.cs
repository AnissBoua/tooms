using Microsoft.EntityFrameworkCore;
using tooms.data;
using tooms.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDBContext>(options => {
    options.UseMySQL("server=localhost;database=tooms;user=root;password=root");
});

//Permet d'ajouter les class de /Services avec les Dependencies injections ;)
builder.Services.Scan(scan => scan
    .FromAssemblyOf<UserService>()
    .AddClasses(classes => classes.InNamespaceOf<UserService>())
    .AsSelfWithInterfaces()
    .WithScopedLifetime());

builder.Services.AddControllers();
var app = builder.Build();

app.UseWebSockets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
