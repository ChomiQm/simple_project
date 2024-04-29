using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Task_Backend.Models;
using Task_Backend.Seeders;

/* BUILDER */

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: true);

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new ArgumentNullException("Connection string for 'DefaultConntection' not found");
}

builder.Services.AddDbContext<ModelContext>(options =>
    options.UseSqlServer(connectionString)); 

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.BearerScheme;
    options.DefaultChallengeScheme = IdentityConstants.BearerScheme;
})
    .AddBearerToken(IdentityConstants.BearerScheme)
    .AddIdentityCookies();

builder.Services.AddAuthorizationBuilder();

builder.Services.AddIdentityCore<User>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ModelContext>()
    .AddApiEndpoints();

builder.Services.ConfigureApplicationCookie(options => {
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(5);
    options.LoginPath = "/login";
    options.SlidingExpiration = true;
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequiredLength = 8;
});

builder.Services.AddControllers();

builder.Services.AddControllersWithViews();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "OAuth2 Auth",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", builder =>
    {
        builder.WithOrigins("https://localhost:5173")
            .AllowAnyMethod()
            .WithHeaders("content-type", "authorization")
            .AllowCredentials();
    });
});


/* APP */

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ModelContext>();
    if (!context.Documents.Any() && !context.DocumentItems.Any())
    {
        string documentsPath = "Seeders/Documents.csv";
        string documentItemsPath = "Seeders/DocumentItems.csv";
        DbSeeder.SeedData(context, documentsPath, documentItemsPath);
    }

}

else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("DefaultCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();
app.MapIdentityApi<User>();

//app.UseMiddleware<LastActivityMiddleware>();

app.MapControllers();

app.Run();
