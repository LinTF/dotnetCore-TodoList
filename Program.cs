// var builder = WebApplication.CreateBuilder(args);

// // Add services to the container.

// builder.Services.AddControllersWithViews();

// var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (!app.Environment.IsDevelopment())
// {
//     // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//     app.UseHsts();
// }

// app.UseHttpsRedirection();
// app.UseStaticFiles();
// app.UseRouting();


// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller}/{action=Index}/{id?}");

// app.MapFallbackToFile("index.html");

// app.Run();


using Swashbuckle.AspNetCore.Swagger;
var builder = WebApplication.CreateBuilder(args);
//先實現跨域再訪問，因此要放在底下程式之下，在此配置的是前端vue的網址。
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        // builder.WithOrigins("http://localhost:8080").AllowAnyHeader();
        builder.WithOrigins("http://localhost:44438").AllowAnyHeader();
    });
});

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
//啟用跨域中間件
app.UseCors();//放在 app.UseHttpsRedirection();下面

app.UseAuthorization();

app.MapControllers();

app.Run();