using BillsPC_CleanArchitecture.Application.Services;
using BillsPC_CleanArchitecture.Data.Interfaces;
using BillsPC_CleanArchitecture.Data.Implementation;
using Microsoft.AspNetCore.Connections;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Register DI services
builder.Services.AddScoped<IDataAccess, DataAccess>();
builder.Services.AddScoped<IDbConnectionFactory>(sp =>
    new SqlConnectionFactory(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPokemonService, PokemonService>();
builder.Services.AddHttpClient<IPokeApiService, PokeApiService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
