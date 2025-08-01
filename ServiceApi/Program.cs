using Hangfire;
using Hangfire.Common;
using Microsoft.EntityFrameworkCore;
using ServiceApi.Model;
using ServiceApi.Servise;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

string ContectionString = $"Server=db,1433;Database=VibeBotDataBase;User Id=sa;Password={Environment.GetEnvironmentVariable("MYSQL_ROOT_PASSWORD")};TrustServerCertificate=True;";


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(ContectionString));

builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(ContectionString));
builder.Services.AddHangfireServer();
builder.Services.AddHttpClient();

builder.Services.AddTransient<VibeSevice>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

using (var scope = app.Services.CreateScope())
{
    var _vibeService = scope.ServiceProvider.GetRequiredService<VibeSevice>();
    await _vibeService.SendVibeRequest(0);
}


using (var scope = app.Services.CreateScope())
{
    var _vibeService = scope.ServiceProvider.GetRequiredService<VibeSevice>();
    await _vibeService.SendVibeRequest(0);

    var recurringJobs = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

    recurringJobs.AddOrUpdate(
        "job-morning",
        Job.FromExpression<VibeSevice>(s => s.SendVibeRequest(0)),
        "0 08 * * *",
        new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });

    recurringJobs.AddOrUpdate(
        "job-afternoon",
        Job.FromExpression<VibeSevice>(s => s.SendVibeRequest(1)),
        "0 14 * * *",
        new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });

    recurringJobs.AddOrUpdate(
        "job-evning",
        Job.FromExpression<VibeSevice>(s => s.SendVibeRequest(2)),
        "0 20 * * *",
        new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
}

app.Run();


