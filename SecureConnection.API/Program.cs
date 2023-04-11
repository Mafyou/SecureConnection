using Newtonsoft.Json;
using SecureConnection.DTO;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-XSRF-TOKEN";
    options.Cookie.Name = "__Host-X-XSRF-TOKEN";
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapPost("/encryptedUser", (IFormFile file) =>
{
    using var aesCryptor = Aes.Create();
    var decryptor = aesCryptor.CreateDecryptor(aesCryptor.Key, aesCryptor.IV);

    using var memoryStream = new MemoryStream();
    file.CopyTo(memoryStream);
    var bytesArray = memoryStream.ToArray();

    using var ms = new MemoryStream(bytesArray);
    using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
    using var sw = new StreamReader(cs);
    var decryptedUser = sw.ReadToEnd();
    var user = JsonConvert.DeserializeObject<UserDTO>(decryptedUser);
    return (user.Name == "Mafyou") && (user.Password == "test");
});

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
