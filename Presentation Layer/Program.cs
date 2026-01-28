
using Business_Layer.Business;
using Business_Layer.SearchTries;
using Business_Layer.Timer;
using Data_Layer.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Options;
using Presentation_Layer.Authentication;
using Presentation_Layer.Authorization;
using Presentation_Layer.Filters;
using Presentation_Layer.Middlewares;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<PermissionBaseAuthorizationFilter>();
});

//builder.Services.AddAntiforgery(options =>
//  options.HeaderName = "CSRF-TOKEN"
//);

builder.Services.AddOpenApi();


var connectionString = builder.Configuration.GetConnectionString("Default");
var jwtOption = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();
var paypalOptions = builder.Configuration.GetSection("PayPalKeys").Get<PaypalOptions>();
var paypalUrls = builder.Configuration.GetSection("Urls").Get<PaypalUrls>();
var storeUrls = builder.Configuration.GetSection("StoreUrls").Get<StoreUrls>();
var inventoryOptions = builder.Configuration.GetSection("Ecommerce_Inventory_Shared_Key").Get<InventoryOptions>();
var cacheKeys = builder.Configuration.GetSection("CacheKeys").Get<CacheKeys>();
var productTrie = new ProductTrie();


builder.Services.AddSingleton<string>(connectionString);
builder.Services.AddSingleton<JwtOptions>(jwtOption);
builder.Services.AddSingleton<PaypalOptions>(paypalOptions);
builder.Services.AddSingleton<PaypalUrls>(paypalUrls);
builder.Services.AddSingleton<StoreUrls>(storeUrls);
builder.Services.AddSingleton<InventoryOptions>(inventoryOptions);
builder.Services.AddSingleton<CacheKeys>(cacheKeys);
builder.Services.AddSingleton<ProductTrie>(productTrie);




builder.Services.AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOption.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOption.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOption.SigningKey))
        };
    });




builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

builder.Services.AddScoped<ImagesBusiness>();
builder.Services.AddScoped<UsersBusiness>();
builder.Services.AddScoped<CartItemBusiness>();
builder.Services.AddScoped<CartsBusiness>();
builder.Services.AddScoped<CategoryBusiness>();
builder.Services.AddScoped<ProductsBusiness>();
builder.Services.AddScoped<PayPalBusiness>();
builder.Services.AddScoped<PromoCodeBusiness>();
builder.Services.AddScoped<SalesBusiness>();
builder.Services.AddScoped<OrdersBusiness>();
builder.Services.AddScoped<EmailBusiness>();
builder.Services.AddScoped<SellerBusiness>();
builder.Services.AddScoped<AuthorizeBusiness>();
builder.Services.AddScoped<InventoryKeyGenerator>();
builder.Services.AddSingleton<FileSystem>();

builder.Services.AddScoped<UsersData>();
builder.Services.AddScoped<CartItemsData>();
builder.Services.AddScoped<CartsData>();
builder.Services.AddScoped<CategoryData>();
builder.Services.AddScoped<ProductData>();
builder.Services.AddScoped<PayPalData>();
builder.Services.AddScoped<PromoCodeData>();
builder.Services.AddScoped<SalesData>();
builder.Services.AddScoped<OrderData>();
builder.Services.AddScoped<SellerData>();
builder.Services.AddScoped<EmailData>();
builder.Services.AddScoped<AuthorizeData>();

builder.Services.AddScoped<AuthenticateHelper>();
builder.Services.AddScoped<AuthorizeHelper>();

builder.Services.AddMemoryCache();

builder.Services.AddSingleton<TimerService>(provider =>
{
    var productsBusiness = provider.GetRequiredService<ProductsBusiness>();
    var productTrie = provider.GetRequiredService<ProductTrie>();
    return new TimerService(productsBusiness, productTrie);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("MorfaCors", policy =>
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
    );
});


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Images/ProductImage")),
    RequestPath = "/Images/ProductImage"
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Images/CategoryImage")),
    RequestPath = "/Images/CategoryImage"
});


//app.UseHttpsRedirection();
//app.UseAntiforgery();


app.UseMiddleware<ContentSecurityPolicyMiddleware>();
app.UseCors("MorfaCors");

app.UseAuthentication();
app.UseAuthorization();


app.UseMiddleware<ExceptionLoggingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.MapControllers();

app.Run();