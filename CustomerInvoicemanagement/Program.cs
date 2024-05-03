using DataAccessLayer.Repository;
using DataAccessLayer.Services;

var builder = WebApplication.CreateBuilder(args);
string connectionString = builder.Configuration.GetSection("ConnectionString").Value.ToString();
string databaseName = builder.Configuration.GetSection("DatabaseName").Value.ToString();
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IInvoiceInterface>(serviceProvider =>
{
    return new InvoiceService(connectionString, databaseName);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
