using Microsoft.EntityFrameworkCore;
using ProvaPub.Repository;
using ProvaPub.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<TempoProviderPadrao>();
builder.Services.AddScoped<RandomService>();
builder.Services.AddScoped<ITempoProvider, TempoProviderPadrao>();
builder.Services.AddScoped<PagadorPix>();
builder.Services.AddScoped<PagadorCartao>();
builder.Services.AddScoped<PagadorPaypal>();
builder.Services.AddScoped<IPagamentoTipo, PagadorPix>();
builder.Services.AddScoped<IPagamentoTipo, PagadorCartao>();
builder.Services.AddScoped<IPagamentoTipo, PagadorPaypal>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddDbContext<TestDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("ctx")));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
