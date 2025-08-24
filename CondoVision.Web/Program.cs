
using CondoVision.Data;
using CondoVision.Data.Entities;
using CondoVision.Data.Helper;
using CondoVision.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;

})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<DataContext>()
.AddDefaultTokenProviders();

// Configuração da autenticação (Google)
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        var clientId = builder.Configuration["Google:ClientId"];
        if (string.IsNullOrEmpty(clientId))
        {
            throw new ArgumentNullException("Google:ClientId não foi configurado em appsettings.json.");
        }
        options.ClientId = clientId;

        var clientSecret = builder.Configuration["Google:ClientSecret"];
        if (string.IsNullOrEmpty(clientSecret))
        {
            throw new ArgumentNullException("Google:ClientSecret não foi configurado em appsettings.json.");
        }
        options.ClientSecret = clientSecret;
    })
    .AddCookie(options =>
    {

        options.ExpireTimeSpan = TimeSpan.FromDays(30); // 30 dias
        options.SlidingExpiration = true;
    });

builder.Services.AddScoped<IUserHelper, UserHelper>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ICondominiumRepository, CondominiumRepository>();
builder.Services.AddScoped<IConverterHelper, ConverterHelper>();
builder.Services.AddScoped<IEMailHelper, EMailHelper>();
builder.Services.AddScoped<IBlobHelper, BlobHelper>();
builder.Services.AddScoped<IUserRepository, UserRepository>(); 
builder.Services.AddScoped<IUnitRepository, UnitRepository>();
builder.Services.AddScoped<IFractionOwnerRepository, FractionOwnerRepository>();
builder.Services.AddTransient<SeedDb>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    try
    {
        var seedDb = services.GetRequiredService<SeedDb>();
        await seedDb.SeedAsync();
        Console.WriteLine("Seeding concluído com sucesso." + DateTime.Now.ToString("HH:mm:ss"));
    }
    catch (Exception ex)
    {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "Ocorreu um erro durante o seeding: {Message}", ex.Message);
    }
}
// Configuração do pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
      
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
