using CustomerInvoiceManagement.Utility;
using DataAccessLayer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
string connectionString = builder.Configuration.GetSection("ConnectionString").Value.ToString();
string databaseName = builder.Configuration.GetSection("DatabaseName").Value.ToString();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<ICustomerInterface, DataAccessLayer.Repository.CustomerService>(serviceProvider =>
{
	return new DataAccessLayer.Repository.CustomerService(connectionString, databaseName);
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
	options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddSingleton<IInvoiceInterface>(serviceProvider =>
{
    return new InvoiceService(connectionString, databaseName);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Add the custom exception middleware here
app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "MyAreas",
	pattern: "{area:exists}/{controller=Customers}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
