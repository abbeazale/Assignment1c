using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);


builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = int.MaxValue;
});

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = int.MaxValue;
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 100_000_000;
});


builder.Services.Configure<Microsoft.AspNetCore.Mvc.MvcOptions>(options =>
{
    options.MaxModelBindingRecursionDepth = 1000;
});

//fix cors errors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
       builder =>
       {
           builder
               .WithOrigins("https://localhost:16778")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
       });
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build(); 


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();

app.Run();