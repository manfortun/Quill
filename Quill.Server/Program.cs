
using Quill.Server.DataAccess;
using Quill.Server.Services;

namespace Quill.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddMemoryCache();

        builder.Services
            .AddSingleton<IConfiguration>(builder.Configuration)
            .AddTransient<TempFileService>()
            .AddTransient<NoteRepository>()
            .AddTransient<TableOfContentService>()
            .AddSingleton<CacheService>()
            .AddTransient<BackupService>()
            .AddSingleton<AutoBackupService>();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin",
                builder =>
                {
                    builder.WithOrigins("https://localhost:5173", "https://localhost:88", "http://192.168.1.10:88")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
        });

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseCors("AllowSpecificOrigin");

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.MapFallbackToFile("/index.html");

        var autobackupService = app.Services.GetRequiredService<AutoBackupService>();

        autobackupService?.ExecuteAutoBackup();

        app.Run();
    }
}
