using Microsoft.EntityFrameworkCore;
using WebApp1.Data;
using WebApp1.Extensions;
using WebApp1.Models;
using WebApp1.Options;
using WebApp1.Services.ClientService;
using WebApp1.Services.EventService;
using WebApp1.Services.TemplateService;
using WebApp1.Services.TicketService;
using WebApp1.Services.TokenService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));
builder.Services.Configure<SmsOptions>(builder.Configuration.GetSection("Sms"));

builder.Services.AddAuthorization(x =>
{
    x.AddPolicy("RegisterConfirmation", y =>
    {
        y.RequireClaim("EmailConfirmed");
        y.RequireClaim("PhoneConfirmed");
    });
    x.AddPolicy("Default", y => y.RequireAuthenticatedUser());
    x.DefaultPolicy = x.GetPolicy("Default")!;
});
builder.Services.AddQticketsApiProvider();
builder.Services.AddMessageSenders();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<ITemplateService, TemplateService>();

builder.Services.AddDbContextFactory<ApplicationDbContext>(o => o.UseNpgsql(builder.Configuration["ConnectionStrings:DefaultConnection"]));
builder.AddFileManagers();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.LoginPath = "/Account/Login";
});

builder.Services.AddDefaultIdentity<User>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;

        options.SignIn.RequireConfirmedPhoneNumber = true;
        options.SignIn.RequireConfirmedEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

builder.Services.AddEndpointsApiExplorer().AddSwaggerGen();

builder.SetUpLogging();

var app = builder.Build();

app.UseLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

try
{
    using var scope = app.Services.CreateScope();
    await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.EnsureCreatedAsync();

    await Seeding.SeedAdmin(scope.ServiceProvider);
}
catch (Exception e)
{
    Console.WriteLine($"Cannot create DB: {e}");
}

app.Run();