using Configuration.Extensions;
using DbContext.Extensions;
using DbRepos;
using Encryption.Extensions;
using Services;
using Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddSecrets(builder.Environment);

// Infra
builder.Services.AddEncryptions(builder.Configuration);
builder.Services.AddJwtToken(builder.Configuration);
builder.Services.AddDatabaseConnections(builder.Configuration);
builder.Services.AddEnvironmentInfo();
builder.Services.AddUserBasedDbContext();

// Repos
builder.Services.AddScoped<AdminDbRepos>();
builder.Services.AddScoped<FriendsDbRepos>();
builder.Services.AddScoped<AddressesDbRepos>();
builder.Services.AddScoped<PetsDbRepos>();
builder.Services.AddScoped<QuotesDbRepos>();
builder.Services.AddScoped<LoginDbRepos>();

// Services
builder.Services.AddScoped<IAdminService, AdminServiceDb>();
builder.Services.AddScoped<IFriendsService, FriendsServiceDb>();
builder.Services.AddScoped<IAddressesService, AddressesServiceDb>();
builder.Services.AddScoped<IPetsService, PetsServiceDb>();
builder.Services.AddScoped<IQuotesService, QuotesServiceDb>();
builder.Services.AddScoped<ILoginService, LoginService>();

builder.Services.AddRazorPages();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();
app.Run();
